using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Moq;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Credits;
using Sc.Credits.Domain.Managment.Services.Customers;
using Sc.Credits.Domain.Managment.Services.Stores;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Common.Gateway;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Helper.Test.Model;
using Sc.Credits.Helpers.ObjectsUtils;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.Domain.Managment.Tests.Credits
{
    public class CreditCommonsServiceTest
    {
        private readonly Mock<IAppParametersService> _parametersServiceMock = new Mock<IAppParametersService>();
        private readonly Mock<ICommons> _commonsMock = new Mock<ICommons>();
        private readonly Mock<ICustomerService> _customerServiceMock = new Mock<ICustomerService>();
        private readonly Mock<IStoreService> _storeServiceMock = new Mock<IStoreService>();
        private readonly Mock<ICreditNotificationRepository> _creditNotificationRepositoryMock = new Mock<ICreditNotificationRepository>();
        private readonly Mock<ITokenRepository> _tokenRepositoryMock = new Mock<ITokenRepository>();

        private ICreditCommonsService CreditCommonsService =>
            new CreditCommonsService(_commonsMock.Object,
                _customerServiceMock.Object,
                _creditNotificationRepositoryMock.Object,
                _tokenRepositoryMock.Object,
                _storeServiceMock.Object);

        public CreditCommonsServiceTest()
        {
            _parametersServiceMock.Setup(mock => mock.GetSettings())
                .Returns(new CredinetAppSettings
                {
                    ValidateTokenOnCreate = true
                });

            _commonsMock.SetupGet(mock => mock.AppParameters)
                .Returns(_parametersServiceMock.Object);

            _commonsMock.SetupGet(mock => mock.CredinetAppSettings)
                .Returns(_parametersServiceMock.Object.GetSettings());
        }

        [Fact]
        public async Task ShouldSendCreditEvent()
        {
            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMaster();

            _creditNotificationRepositoryMock.Setup(mock => mock.SendEventAsync(It.IsAny<CreditMaster>(), It.IsAny<Customer>(), It.IsAny<Store>(), It.IsAny<Credit>(),
                It.IsAny<TransactionType>(), It.IsAny<string>()))
                .Verifiable();

            _customerServiceMock.Setup(mock => mock.TrySendCreditLimitUpdateAsync(It.IsAny<Customer>()))
                .Verifiable();

            await CreditCommonsService.SendEventAsync(creditMaster, creditMaster.Customer, creditMaster.Store);

            _creditNotificationRepositoryMock.VerifyAll();

            _customerServiceMock.VerifyAll();
        }

        [Fact]
        public async Task ShouldGenerateToken()
        {
            _tokenRepositoryMock.Setup(mock => mock.GenerateCreditTokenAsync(It.IsAny<CreditTokenRequest>()))
                .ReturnsAsync(new TokenResponse());

            TokenResponse tokenResult = await CreditCommonsService.GenerateTokenAsync(new CreditTokenRequest());

            Assert.NotNull(tokenResult);
        }

        [Fact]
        public async Task ShouldTokenCallRequest()
        {
            _tokenRepositoryMock.Setup(mock => mock.CreditTokenCallRequestAsync(It.IsAny<CreditTokenCallRequest>()))
                .Verifiable();

            await CreditCommonsService.TokenCallRequestAsync(new CreditTokenCallRequest());

            _tokenRepositoryMock.VerifyAll();
        }


        [Fact]
        public async Task ShouldThrowsBusinessExcepcionInvalidToken()
        {
            _tokenRepositoryMock.Setup(mock => mock.IsValidCreditTokenAsync(It.IsAny<CreditTokenRequest>()))
                .ReturnsAsync(false);

            BusinessException exception = await Assert.ThrowsAsync<BusinessException>(() =>
                CreditCommonsService.ValidateTokenAsync(CreditHelperTest.GetCreateCreditRequest(), CustomerHelperTest.GetCustomer()));

            Assert.Equal((int)BusinessResponse.TokenIsNotValid, exception.code);
        }
    }
}