using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Moq;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Model.Common.Gateway;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.Helper.Test.Model;
using Sc.Credits.Helpers.Commons.Cache;
using Sc.Credits.Helpers.Commons.Settings;
using Sc.Credits.Helpers.ObjectsUtils;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.Domain.Managment.Tests.Common
{
    public class AppParametersServiceTest
    {
        private readonly Mock<IAppParametersRepository> _appParametersRepositoryMock = new Mock<IAppParametersRepository>();
        private readonly Mock<ICache> _cacheMock = new Mock<ICache>();
        private readonly Mock<ISettings<CredinetAppSettings>> _appSettingsMock = new Mock<ISettings<CredinetAppSettings>>();

        private IAppParametersService AppParametersService =>
    new AppParametersService(_appParametersRepositoryMock.Object,
        _cacheMock.Object,
        _appSettingsMock.Object);

        public AppParametersServiceTest()
        {
            _appSettingsMock.Setup(mock => mock.Get())
                 .Returns(new CredinetAppSettings
                 {
                     ParametersCacheMilliseconds = 1
                 });
        }

        private void SetupCache<TItem>()
        {
            _cacheMock.Setup(mock => mock.GetOrCreateAsync(It.IsAny<CacheItem<TItem>>()))
                .Returns(async (CacheItem<TItem> item) =>
                   {
                       return await item.GetItemAsync();
                   });
        }

        [Fact]
        public async Task ShouldGetParameters()
        {
            List<Parameter> parameters = ParameterHelperTest.GetParameters();

            _appParametersRepositoryMock.Setup(mock => mock.GetAllParametersAsync())
                .ReturnsAsync(parameters);

            SetupCache<List<Parameter>>();

            AppParameters parametersResult = await AppParametersService.GetAppParametersAsync();

            Assert.NotNull(parametersResult);
            Assert.Equal(2, parametersResult.DecimalNumbersRound);
        }

        [Fact]
        public async Task ShouldGetTransactionType()
        {
            TransactionType transactionType = TransactionTypeHelperTest.GetCreateCreditType();

            _appParametersRepositoryMock.Setup(mock => mock.GetTransactionTypeAsync(It.IsAny<int>()))
                .ReturnsAsync(transactionType);

            SetupCache<TransactionType>();

            TransactionType transactionTypeResult = await AppParametersService.GetTransactionTypeAsync(1);

            Assert.NotNull(transactionTypeResult);
        }

        [Fact]
        public async Task ShouldGetTransactionTypeThrowsBusinessException()
        {
            TransactionType transactionType = null;
            _appParametersRepositoryMock.Setup(mock => mock.GetTransactionTypeAsync(It.IsAny<int>()))
                .ReturnsAsync(transactionType);

            BusinessException businessException = await Assert.ThrowsAsync<BusinessException>(() => AppParametersService.GetTransactionTypeAsync(1));
            Assert.Equal((int)BusinessResponse.TransactionTypeNotFound, businessException.code);
        }

        [Fact]
        public async Task ShouldGetStatus()
        {
            Status status = StatusHelperTest.GetActiveStatus();

            _appParametersRepositoryMock.Setup(mock => mock.GetStatusAsync(It.IsAny<int>()))
                .ReturnsAsync(status);

            SetupCache<Status>();

            Status statusResult = await AppParametersService.GetStatusAsync(1);

            Assert.NotNull(statusResult);
        }

        [Fact]
        public async Task ShouldGetStatusThrowsBusinessException()
        {
            Status status = null;
            _appParametersRepositoryMock.Setup(mock => mock.GetStatusAsync(It.IsAny<int>()))
                .ReturnsAsync(status);

            BusinessException businessException = await Assert.ThrowsAsync<BusinessException>(() => AppParametersService.GetStatusAsync(1));
            Assert.Equal((int)BusinessResponse.StatusNotFound, businessException.code);
        }

        [Fact]
        public async Task ShouldGetSource()
        {
            Source source = SourceHelperTest.GetCredinetSource();

            SetupCache<Source>();

            _appParametersRepositoryMock.Setup(mock => mock.GetSourceAsync(It.IsAny<int>()))
                .ReturnsAsync(source);

            Source sourceResult = await AppParametersService.GetSourceAsync(1);

            Assert.NotNull(sourceResult);
        }

        [Fact]
        public async Task ShouldGetSourceThrowsBusinessException()
        {
            Source source = null;
            _appParametersRepositoryMock.Setup(mock => mock.GetSourceAsync(It.IsAny<int>()))
                .ReturnsAsync(source);

            BusinessException businessException = await Assert.ThrowsAsync<BusinessException>(() => AppParametersService.GetSourceAsync(1));

            Assert.Equal((int)BusinessResponse.SourceNotFound, businessException.code);
        }

        [Fact]
        public async Task ShouldGetPaymentType()
        {
            PaymentType paymentType = PaymentTypeHelperTest.GetOrdinaryPaymentType();

            SetupCache<PaymentType>();

            _appParametersRepositoryMock.Setup(mock => mock.GetPaymentTypeAsync(It.IsAny<int>()))
                .ReturnsAsync(paymentType);

            PaymentType paymentTypeResult = await AppParametersService.GetPaymentTypeAsync(1);

            Assert.NotNull(paymentTypeResult);
        }

        [Fact]
        public async Task ShouldGetPaymentTypeThrowsBusinessException()
        {
            PaymentType paymentType = null;
            _appParametersRepositoryMock.Setup(mock => mock.GetPaymentTypeAsync(It.IsAny<int>()))
                .ReturnsAsync(paymentType);

            BusinessException businessException = await Assert.ThrowsAsync<BusinessException>(() => AppParametersService.GetPaymentTypeAsync(1));
            Assert.Equal((int)BusinessResponse.PaymentTypeNotFound, businessException.code);
        }

        [Fact]
        public async Task ShouldGetAuthMethod()
        {
            AuthMethod authMethod = AuthMethodHelperTest.GetTokenAuthMethod();

            SetupCache<AuthMethod>();

            _appParametersRepositoryMock.Setup(mock => mock.GetAuthMethodAsync(It.IsAny<int>()))
                .ReturnsAsync(authMethod);

            AuthMethod authMethodResult = await AppParametersService.GetAuthMethodAsync(1);

            Assert.NotNull(authMethodResult);
        }

        [Fact]
        public async Task ShouldGetAuthMethodThrowsBusinessException()
        {
            AuthMethod authMethod = null;
            _appParametersRepositoryMock.Setup(mock => mock.GetAuthMethodAsync(It.IsAny<int>()))
                .ReturnsAsync(authMethod);

            BusinessException businessException = await Assert.ThrowsAsync<BusinessException>(() => AppParametersService.GetAuthMethodAsync(1));
            Assert.Equal((int)BusinessResponse.AuthMethodNotFound, businessException.code);
        }

        [Fact]
        public void ShouldGetSettings()
        {
            CredinetAppSettings credinetAppSettings = AppParametersService.GetSettings();

            Assert.NotNull(credinetAppSettings);
        }
    }
}