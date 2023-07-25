using Moq;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Credits;
using Sc.Credits.Domain.Managment.Services.Customers;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Domain.UseCase.Credits;
using Sc.Credits.Helper.Test.Model;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace Sc.Credits.Domain.Managment.Tests.Credits
{
    public class TotalCancelationCreditServiceTest
    {
        private readonly Mock<ICreditMasterRepository> _creditMasterRepositoryMock = new Mock<ICreditMasterRepository>();
        private readonly Mock<IRequestCancelPaymentRepository> _requestCancelPaymentRepositoryMock = new Mock<IRequestCancelPaymentRepository>();
        private readonly Mock<IRequestCancelCreditRepository> _requestCancelCreditRepositoryMock = new Mock<IRequestCancelCreditRepository>();
        private readonly Mock<ICreditUsesCase> _creditUsesCaseMock = new Mock<ICreditUsesCase>();
        private readonly Mock<ICancelUseCase> _cancelUseCaseMock = new Mock<ICancelUseCase>();
        private readonly Mock<ICreditCommonsService> _creditCommonServiceMock = new Mock<ICreditCommonsService>();
        private readonly Mock<ICustomerService> _customerServiceMock = new Mock<ICustomerService>();
        private readonly Mock<ICommons> _commonsMock = new Mock<ICommons>();
        private readonly Mock<IAppParametersService> _appParametersServiceMock = new Mock<IAppParametersService>();
        private readonly Mock<CredinetAppSettings> _appSettingsMock = new Mock<CredinetAppSettings>();
        private readonly Mock<ILoggerService<TotalCancelationCreditService>> _loggerTotalCancelationCreditServiceMock = new Mock<ILoggerService<TotalCancelationCreditService>>();

        private CreditCommons CreditCommons =>
           CreditCommonsHelperTest.Create(_creditMasterRepositoryMock,
               _requestCancelCreditRepositoryMock,
               _requestCancelPaymentRepositoryMock,
               _creditUsesCaseMock,
               new Mock<ICreditPaymentUsesCase>(),
               _cancelUseCaseMock,
               _creditCommonServiceMock);

        private ITotalCancelationCreditService totalCancelationCreditService =>
            new TotalCancelationCreditService(CreditCommons, _loggerTotalCancelationCreditServiceMock.Object);

        public TotalCancelationCreditServiceTest()
        {
            _commonsMock.SetupGet(mock => mock.AppParameters)
               .Returns(_appParametersServiceMock.Object);

            _creditCommonServiceMock.SetupGet(mock => mock.Commons)
                .Returns(_commonsMock.Object);

            _creditCommonServiceMock.SetupGet(mock => mock.CustomerService)
                .Returns(_customerServiceMock.Object);

            _creditCommonServiceMock.SetupGet(mock => mock.Commons.CredinetAppSettings)
                .Returns(CredinetAppSettingsHelperTest.GetCredinetAppSettings());
        }

        [Fact]
        public async Task ApproveCancellationAsync()
        {
            //Arrange

            RequestCancelCredit requestCancelCredit = RequestCancelCreditHelperTest.GetRequestCancelCredit();
            CreditMaster creditMasterMock = CreditMasterHelperTest.GetCreditMaster();
            _cancelUseCaseMock.Setup(mock => mock.CancelCredit(It.IsAny<CancelCreditDomainRequest>()));

            _creditMasterRepositoryMock.Setup(mock => mock.AddTransactionAsync(It.IsAny<CreditMaster>(), It.IsAny<IEnumerable<Field>>(),
                                                        It.IsAny<Transaction>()));

            //Act

            await totalCancelationCreditService.ApproveCancellationAsync(new UserInfo("", ""), requestCancelCredit, creditMasterMock);

            //Assert

            Assert.Equal((int)RequestStatuses.Cancel, requestCancelCredit.GetRequestStatusId);

            _creditMasterRepositoryMock.Verify(mock => mock.AddTransactionAsync(It.IsAny<CreditMaster>(),
                It.IsAny<IEnumerable<Field>>(), It.IsAny<Transaction>()), Times.Once);

            _cancelUseCaseMock.Verify(mock => mock.CancelCredit(It.IsAny<CancelCreditDomainRequest>()), Times.Once);

            _requestCancelCreditRepositoryMock.Verify(mock => mock.UpdateAsync(It.IsAny<RequestCancelCredit>(),
                It.IsAny<IEnumerable<Field>>(), It.IsAny<Transaction>()));

            _creditCommonServiceMock.Verify(mock => mock.SendEventAsync(It.IsAny<CreditMaster>(), It.IsAny<Customer>(), It.IsAny<Store>(),
                It.IsAny<Credit>(), It.IsAny<TransactionType>(), It.IsAny<string>()), Times.Once);

            _requestCancelPaymentRepositoryMock.Verify(mock => mock.GetByStatusFromMasterAsync(It.IsAny<CreditMaster>(), It.IsAny<RequestStatuses>(),
                It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()), Times.Once);
        }

        [Fact]
        public async Task ApproveCancellationAsyncThrowException()
        {
            //Arrange

            RequestCancelCredit requestCancelCredit = RequestCancelCreditHelperTest.GetRequestCancelCredit();
            CreditMaster creditMasterMock = CreditMasterHelperTest.GetCreditMaster();
            _cancelUseCaseMock.Setup(mock => mock.CancelCredit(It.IsAny<CancelCreditDomainRequest>()));

            _creditMasterRepositoryMock.Setup(mock => mock.AddTransactionAsync(It.IsAny<CreditMaster>(), It.IsAny<IEnumerable<Field>>(),
                                                        It.IsAny<Transaction>())).ThrowsAsync(new Exception());

            //Act

            await totalCancelationCreditService.ApproveCancellationAsync(new UserInfo("", ""), requestCancelCredit, creditMasterMock);

            //Assert

            _loggerTotalCancelationCreditServiceMock.Verify(mock => mock.LogError(It.IsAny<string>(), It.IsAny<string>(),
                                                                                 It.IsAny<object>(), It.IsAny<Exception>()), Times.Once);

            _loggerTotalCancelationCreditServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(),
                                                                                    It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Once);
        }
    }
}