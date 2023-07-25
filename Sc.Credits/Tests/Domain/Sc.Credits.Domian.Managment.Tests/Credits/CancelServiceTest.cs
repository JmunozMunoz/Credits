using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Moq;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Credits;
using Sc.Credits.Domain.Managment.Services.Customers;
using Sc.Credits.Domain.Managment.Services.Stores;
using Sc.Credits.Domain.Managment.Tests.Entities.Common;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.Model.Refinancings.Gateway;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Domain.UseCase.Credits;
using Sc.Credits.Helper.Test.Model;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace Sc.Credits.Domain.Managment.Tests.Credits
{
    public class CancelServiceTest
    {
        private readonly Mock<ICreditMasterRepository> _creditMasterRepositoryMock = new Mock<ICreditMasterRepository>();
        private readonly Mock<IRequestCancelCreditRepository> _requestCancelCreditRepositoryMock = new Mock<IRequestCancelCreditRepository>();
        private readonly Mock<IRequestCancelPaymentRepository> _requestCancelPaymentRepositoryMock = new Mock<IRequestCancelPaymentRepository>();
        private readonly Mock<ICancelUseCase> _cancelUseCaseMock = new Mock<ICancelUseCase>();
        private readonly Mock<ICreditUsesCase> _creditUsesCaseMock = new Mock<ICreditUsesCase>();
        private readonly Mock<ICreditCommonsService> _creditCommonServiceMock = new Mock<ICreditCommonsService>();
        private readonly Mock<ICustomerService> _customerServiceMock = new Mock<ICustomerService>();
        private readonly Mock<IStoreService> _storeServiceMock = new Mock<IStoreService>();
        private readonly Mock<ICommons> _commonsMock = new Mock<ICommons>();
        private readonly Mock<IAppParametersService> _appParametersServiceMock = new Mock<IAppParametersService>();
        private readonly Mock<ILoggerService<CancelCreditService>> _loggerCancelCreditServiceMock = new Mock<ILoggerService<CancelCreditService>>();
        private readonly Mock<ILoggerService<CancelPaymentService>> _loggerCancelPaymentServiceMock = new Mock<ILoggerService<CancelPaymentService>>();
        private readonly Mock<CredinetAppSettings> _appSettingsMock = new Mock<CredinetAppSettings>();
        private readonly Mock<ICancelCreditContextService> _cancelCreditContextService = new Mock<ICancelCreditContextService>();
        private readonly Mock<IRefinancingLogRepository> _mockRefinancingLogRepository = new Mock<IRefinancingLogRepository>();
        private readonly Mock<ITemplatesService> _templatesServiceMock = new Mock<ITemplatesService>();

        private CreditCommons CreditCommons =>
            CreditCommonsHelperTest.Create(_creditMasterRepositoryMock,
                _requestCancelCreditRepositoryMock,
                _requestCancelPaymentRepositoryMock,
                _creditUsesCaseMock,
                new Mock<ICreditPaymentUsesCase>(),
                _cancelUseCaseMock,
                _creditCommonServiceMock);

        private ICancelCreditService CancelCreditService =>
            new CancelCreditService(CreditCommons, _loggerCancelCreditServiceMock.Object, _cancelCreditContextService.Object);

        private ICancelPaymentService CancelPaymentService =>
            new CancelPaymentService(CreditCommons, _loggerCancelPaymentServiceMock.Object, _mockRefinancingLogRepository.Object);

        public CancelServiceTest()
        {
            _commonsMock.SetupGet(mock => mock.AppParameters)
                .Returns(_appParametersServiceMock.Object);

            _creditCommonServiceMock.SetupGet(mock => mock.Commons)
                .Returns(_commonsMock.Object);

            _creditCommonServiceMock.SetupGet(mock => mock.CustomerService)
                .Returns(_customerServiceMock.Object);

            _creditCommonServiceMock.SetupGet(mock => mock.StoreService)
                .Returns(_storeServiceMock.Object);

            _creditCommonServiceMock.SetupGet(mock => mock.Commons.CredinetAppSettings)
                .Returns(CredinetAppSettingsHelperTest.GetCredinetAppSettings());

            _commonsMock.SetupGet(mock => mock.Templates)
                .Returns(_templatesServiceMock.Object);
        }

        [Fact]
        public async Task ShouldRequestCancelCreditThrowsCreditsNotFound()
        {
            CreditMaster creditMaster = null;

            CancelCreditRequest cancelCreditRequest = new CancelCreditRequest()
            {
                CreditId = Guid.NewGuid(),
                StoreId = "111e9acbb818c50bc8e3261f",
                UserId = "5d829c00c201720001f1b03e",
                Reason = "Pruebas",
                CancellationType = 1
            };

            _creditMasterRepositoryMock.Setup(mock => mock.GetWithTransactionsAsync(It.IsAny<Guid>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
               .ReturnsAsync(creditMaster);

            BusinessException businessException =
                await Assert.ThrowsAsync<BusinessException>(() => CancelCreditService.RequestAsync(cancelCreditRequest));

            Assert.Equal((int)BusinessResponse.CreditsNotFound, businessException.code);
        }

        [Fact]
        public async Task ShouldRequestCancelCredit()
        {
            var store = StoreHelperTest.GetStore();

            var cancelCreditRequestLocal = new CancelCreditRequest()
            {
                CreditId = Guid.NewGuid(),
                StoreId = store.Id,
                UserId = "5d829c00c201720001f1b03e",
                Reason = "Pruebas",
                CancellationType = 1,
                ValueCancel = 40000
            };

            MailNotificationRequest mailNotificationRequest = new MailNotificationRequestBuilder().Build();
            AppParameters appParameters = ParameterHelperTest.GetAppParameters();
            SmsNotificationRequest smsNotificationRequest = new SmsNotificationRequest();

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _storeServiceMock.Setup(mock => mock.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(StoreHelperTest.GetStore());

            _creditMasterRepositoryMock.Setup(mock => mock.GetWithTransactionsAsync(It.IsAny<Guid>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
               .ReturnsAsync(CreditMasterHelperTest.GetCreditMaster());

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync()).ReturnsAsync(appParameters).Verifiable();
            _templatesServiceMock.Setup(mock => mock.GetAsync(It.IsAny<string>())).ReturnsAsync("ApprovalCreditCancellation.txt").Verifiable();
            _creditCommonServiceMock.Setup(mock => mock.Commons.Notification.SendMailAsync(It.IsAny<MailNotificationRequest>(),
                    It.IsAny<string>(), It.IsAny<bool>())).Returns(Task.CompletedTask).Verifiable();
            _creditCommonServiceMock.Setup(mock => mock.Commons.Notification.SendSmsAsync(It.IsAny<SmsNotificationRequest>(),
                    It.IsAny<string>(), It.IsAny<bool>())).Returns(Task.CompletedTask).Verifiable();

            await CancelCreditService.RequestAsync(cancelCreditRequestLocal);

            _requestCancelCreditRepositoryMock.Verify(mock => mock.AddAsync(It.IsAny<RequestCancelCredit>(),
                It.IsAny<Transaction>()), Times.Once);

            _creditMasterRepositoryMock.Verify(mock => mock.UpdateAsync(It.IsAny<CreditMaster>(), It.IsAny<IEnumerable<Field>>(),
                It.IsAny<Transaction>()), Times.Once);
        }

        [Fact]
        public async Task GetPendingsAsync()
        {
            //Arrange

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
               .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            RequestCancelCreditPaged requestCancelCreditMock = RequestCancelCreditHelperTest.GetRequestCancelCreditListWihtCreditMaster();

            _requestCancelCreditRepositoryMock.Setup(mock => mock.GetByVendorAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<RequestStatuses>(),
                It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(),
                It.IsAny<IEnumerable<Field>>())).ReturnsAsync(requestCancelCreditMock);

            //Act

            var result = await CancelCreditService.GetPendingsAsync("4sdf6s4d",1,1,false);

            //Assert

            Assert.Equal(typeof(CancelCreditDetailResponsePaged), result.GetType());
            Assert.NotNull(result.CreditDetailResponses.FirstOrDefault(x => x.CancellationType == CancellationTypes.Total.ToString()));
            Assert.NotNull(result.CreditDetailResponses.FirstOrDefault(x => x.CancellationType == CancellationTypes.Parcial.ToString()));
        }

        [Fact]
        public async Task ShouldRejectCancelCredit()
        {
            CreditMaster creditMasterMock = CreditMasterHelperTest.GetCreditMaster();
            creditMasterMock.SetStatusId((int)Statuses.CancelRequest);
            RequestCancelCredit requestCancelCreditMock = RequestCancelCreditHelperTest.GetRequestCancelCredit();
            requestCancelCreditMock.UpdateStatus((int)RequestStatuses.Pending, "Prueba", "54545425");
            Store store = StoreHelperTest.GetStore();

            MailNotificationRequest mailNotificationRequest = new MailNotificationRequestBuilder().Build();
            AppParameters appParameters = ParameterHelperTest.GetAppParameters();
            SmsNotificationRequest smsNotificationRequest = new SmsNotificationRequest();

            _creditMasterRepositoryMock.Setup(mock => mock.GetWithTransactionsAsync(It.IsAny<Guid>(),
                   It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(),
                   It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
              .ReturnsAsync(CreditMasterHelperTest.GetCreditMaster());

            _requestCancelCreditRepositoryMock.Setup(mock => mock.GetByStatusAsync(It.IsAny<Guid>(),
                    It.IsAny<RequestStatuses>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(requestCancelCreditMock);

            _creditUsesCaseMock.Setup(mock => mock.UpdateStatus(It.IsAny<int>(), It.IsAny<CreditMaster>()))
                .Verifiable();

            _cancelUseCaseMock.Setup(mock => mock.UpdateStatusRequestCreditCancel(It.IsAny<RequestStatuses>(), It.IsAny<RequestCancelCredit>(),
                    It.IsAny<UserInfo>()))
                .Verifiable();

            _creditUsesCaseMock.Setup(mock => mock.UpdateStatus(It.IsAny<int>(), It.IsAny<CreditMaster>()))
               .Verifiable();

            async void callbackInvoke(Func<Transaction, Task> handle) =>
                await handle.Invoke(Transaction.Current);

            _creditMasterRepositoryMock.Setup(mock => mock.ExcecuteOnTransactionAsync(It.IsAny<Func<Transaction, Task>>()))
                .Callback<Func<Transaction, Task>>(callbackInvoke);

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync()).ReturnsAsync(appParameters).Verifiable();
            _templatesServiceMock.Setup(mock => mock.GetAsync(It.IsAny<string>())).ReturnsAsync("RejectCreditCancellation.txt").Verifiable();
            _creditCommonServiceMock.Setup(mock => mock.Commons.Notification.SendMailAsync(It.IsAny<MailNotificationRequest>(),
                    It.IsAny<string>(), It.IsAny<bool>())).Returns(Task.CompletedTask).Verifiable();
            _creditCommonServiceMock.Setup(mock => mock.Commons.Notification.SendSmsAsync(It.IsAny<SmsNotificationRequest>(),
                    It.IsAny<string>(), It.IsAny<bool>())).Returns(Task.CompletedTask).Verifiable();

            await CancelCreditService.RejectAsync(new CancelCredit
            {
                CreditId = CreditMasterHelperTest.GetCreditMaster().Id,
                UserId = string.Empty
            });

            _creditUsesCaseMock.VerifyAll();
            _cancelUseCaseMock.VerifyAll();

            _requestCancelCreditRepositoryMock.Verify(mock => mock.UpdateAsync(It.IsAny<RequestCancelCredit>(),
                It.IsAny<IEnumerable<Field>>(), It.IsAny<Transaction>()), Times.Once);

            _creditMasterRepositoryMock.Verify(mock => mock.UpdateAsync(It.IsAny<CreditMaster>(), It.IsAny<IEnumerable<Field>>(),
                It.IsAny<Transaction>()), Times.Once);
        }

        [Fact]
        public async Task RejectUnprocessedRequestAsync()
        {
            CreditMaster creditMasterMock = CreditMasterHelperTest.GetCreditMaster();
            creditMasterMock.SetStatusId((int)Statuses.CancelRequest);
            List<RequestCancelCredit> requestCancelCreditMock = RequestCancelCreditHelperTest.GetRequestCancelCreditList();

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditMasterRepositoryMock.Setup(mock => mock.GetWithTransactionsAsync(It.IsAny<Guid>(),
                   It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(),
                   It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
              .ReturnsAsync(CreditMasterHelperTest.GetCreditMaster());

            _requestCancelCreditRepositoryMock.Setup(mock => mock.GetByStatusUntilDateAsync(It.IsAny<DateTime>(),
                    It.IsAny<RequestStatuses>()))
                .ReturnsAsync(requestCancelCreditMock);

            _creditUsesCaseMock.Setup(mock => mock.UpdateStatus(It.IsAny<int>(), It.IsAny<CreditMaster>()))
                .Verifiable();

            _cancelUseCaseMock.Setup(mock => mock.UpdateStatusRequestCreditCancel(It.IsAny<RequestStatuses>(), It.IsAny<RequestCancelCredit>(),
                    It.IsAny<UserInfo>()))
                .Verifiable();

            _creditUsesCaseMock.Setup(mock => mock.UpdateStatus(It.IsAny<int>(), It.IsAny<CreditMaster>()))
               .Verifiable();

            async void callbackInvoke(Func<Transaction, Task> handle) =>
                await handle.Invoke(Transaction.Current);

            _creditMasterRepositoryMock.Setup(mock => mock.ExcecuteOnTransactionAsync(It.IsAny<Func<Transaction, Task>>()))
                .Callback<Func<Transaction, Task>>(callbackInvoke);

            await CancelCreditService.RejectUnprocessedRequestAsync();

            _creditUsesCaseMock.VerifyAll();
            _cancelUseCaseMock.VerifyAll();

            _requestCancelCreditRepositoryMock.Verify(mock => mock.UpdateAsync(It.IsAny<RequestCancelCredit>(),
                It.IsAny<IEnumerable<Field>>(), It.IsAny<Transaction>()), Times.Exactly(3));

            _creditMasterRepositoryMock.Verify(mock => mock.UpdateAsync(It.IsAny<CreditMaster>(), It.IsAny<IEnumerable<Field>>(),
                It.IsAny<Transaction>()), Times.Exactly(3));
        }

        [Fact]
        public async Task ShouldRejectCancelPayments()
        {
            CreditMaster creditMasterMock = CreditMasterHelperTest.GetCreditMaster();
            creditMasterMock.SetStatusId((int)Statuses.CancelRequest);

            List<RequestCancelPayment> requestCancelPaymentsMock = new List<RequestCancelPayment>
            {
                RequestCancelPaymentHelperTest.GetRequestCancelPayment(),
                RequestCancelPaymentHelperTest.GetRequestCancelPayment()
            };

            MailNotificationRequest mailNotificationRequest = new MailNotificationRequestBuilder().Build();
            AppParameters appParameters = ParameterHelperTest.GetAppParameters();
            SmsNotificationRequest smsNotificationRequest = new SmsNotificationRequest();

            requestCancelPaymentsMock.ForEach(mock => mock.UpdateStatus((int)RequestStatuses.Pending, "Prueba", "54as54fe54a"));

            _requestCancelPaymentRepositoryMock.Setup(mock => mock.GetByStatusFromMasterAsync(It.IsAny<Guid>(),
                    It.IsAny<RequestStatuses>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(requestCancelPaymentsMock);

            _creditMasterRepositoryMock.Setup(mock => mock.GetWithTransactionsAsync(It.IsAny<Guid>(),
                  It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(),
                  It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
             .ReturnsAsync(CreditMasterHelperTest.GetCreditMaster());

            async void callbackInvoke(Func<Transaction, Task> handle) =>
                await handle.Invoke(Transaction.Current);

            _creditMasterRepositoryMock.Setup(mock => mock.ExcecuteOnTransactionAsync(It.IsAny<Func<Transaction, Task>>()))
                .Callback<Func<Transaction, Task>>(callbackInvoke);

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync()).ReturnsAsync(appParameters).Verifiable();
            _templatesServiceMock.Setup(mock => mock.GetAsync(It.IsAny<string>())).ReturnsAsync("ApprovalCreditCancellation.txt").Verifiable();
            _creditCommonServiceMock.Setup(mock => mock.Commons.Notification.SendMailAsync(It.IsAny<MailNotificationRequest>(),
                    It.IsAny<string>(), It.IsAny<bool>())).Returns(Task.CompletedTask).Verifiable();
            _creditCommonServiceMock.Setup(mock => mock.Commons.Notification.SendSmsAsync(It.IsAny<SmsNotificationRequest>(),
                    It.IsAny<string>(), It.IsAny<bool>())).Returns(Task.CompletedTask).Verifiable();

            await CancelPaymentService.RejectAsync(
                new CancelPayments
                {
                    CreditId = Guid.NewGuid(),
                    UserId = "Pepito"
                });

            _requestCancelPaymentRepositoryMock.Verify(mock => mock.UpdateAsync(It.IsAny<RequestCancelPayment>(),
                It.IsAny<IEnumerable<Field>>(), It.IsAny<Transaction>()), Times.Exactly(2));

            _cancelUseCaseMock.Verify(mock => mock.UpdateStatusRequestPaymentCancel(It.IsAny<RequestStatuses>(), It.IsAny<RequestCancelPayment>(),
                It.IsAny<UserInfo>()), Times.Exactly(2));
        }

        [Fact]
        public async Task ShouldRejectUnprocessedRequest()
        {
            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            List<RequestCancelPayment> requestCancelPaymentsMock = new List<RequestCancelPayment>
            {
                RequestCancelPaymentHelperTest.GetRequestCancelPayment(),
                RequestCancelPaymentHelperTest.GetRequestCancelPayment()
            };

            requestCancelPaymentsMock.ForEach(mock => mock.UpdateStatus((int)RequestStatuses.Pending, "Prueba", "54as54fe54a"));

            _requestCancelPaymentRepositoryMock.Setup(mock => mock.GetByStatusUntilDate(It.IsAny<DateTime>(),
                    It.IsAny<RequestStatuses>()))
                .ReturnsAsync(requestCancelPaymentsMock);

            async void callbackInvoke(Func<Transaction, Task> handle) =>
                await handle.Invoke(Transaction.Current);

            _creditMasterRepositoryMock.Setup(mock => mock.ExcecuteOnTransactionAsync(It.IsAny<Func<Transaction, Task>>()))
                .Callback<Func<Transaction, Task>>(callbackInvoke);

            await CancelPaymentService.RejectUnprocessedRequestAsync();

            _requestCancelPaymentRepositoryMock.Verify(mock => mock.UpdateAsync(It.IsAny<RequestCancelPayment>(),
                It.IsAny<IEnumerable<Field>>(), It.IsAny<Transaction>()), Times.Exactly(2));

            _cancelUseCaseMock.Verify(mock => mock.UpdateStatusRequestPaymentCancel(It.IsAny<RequestStatuses>(), It.IsAny<RequestCancelPayment>(),
                It.IsAny<UserInfo>()), Times.Exactly(2));
        }

        [Fact]
        public async Task ShouldCancelPayments()
        {
            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMaster();
            Store store = StoreHelperTest.GetStore();

            CancelPayments cancelPayment = new CancelPayments
            {
                CreditId = creditMaster.Id,
                UserId = "sdsdkjsdka445sa58d",
                UserName = "Test user"
            };

            CreditMasterHelperTest.AddTransaction(creditMaster, TransactionTypes.Payment, creditMaster.Store, Statuses.Active, DateTime.Now);
            CreditMasterHelperTest.AddTransaction(creditMaster, TransactionTypes.Payment, creditMaster.Store, Statuses.Active, DateTime.Now);
            CreditMasterHelperTest.AddTransaction(creditMaster, TransactionTypes.Payment, creditMaster.Store, Statuses.Active, DateTime.Now);

            MailNotificationRequest mailNotificationRequest = new MailNotificationRequestBuilder().Build();
            AppParameters appParameters = ParameterHelperTest.GetAppParameters();
            SmsNotificationRequest smsNotificationRequest = new SmsNotificationRequest();

            List<RequestCancelPayment> requestCancelPayments = creditMaster
                .History
                    .Where(h => h.CreditPayment.GetTransactionTypeId == (int)TransactionTypes.Payment)
                    .Select(h =>
                        {
                            return RequestCancelPaymentHelperTest.GetRequestCancelPayment(h.Id).SetCreditMaster(creditMaster);
                        }).ToList();

            requestCancelPayments.ForEach(r => { r.SetStore(store); r.SetCanceled(creditMaster.Current,"Prueba","Anulacion1111"); });

            _creditMasterRepositoryMock.Setup(mock => mock.GetWithTransactionsAsync(It.IsAny<Guid>(),
                It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(),
                It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(creditMaster);

            _storeServiceMock.Setup(ce => ce.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                   It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
               .ReturnsAsync(store);

            _requestCancelPaymentRepositoryMock.Setup(mock => mock.GetByStatusFromMasterAsync(It.IsAny<CreditMaster>(), It.IsAny<RequestStatuses>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(requestCancelPayments);

            _appParametersServiceMock.Setup(mock => mock.GetTransactionTypeAsync(It.IsAny<int>()))
                .ReturnsAsync(TransactionTypeHelperTest.GetCancelPaymentType());

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _appParametersServiceMock.Setup(mock => mock.GetStatusAsync(It.IsAny<int>()))
                .ReturnsAsync(StatusHelperTest.GetActiveStatus());

            _creditMasterRepositoryMock.Setup(mock => mock.ExcecuteOnTransactionAsync(It.IsAny<Func<Transaction, Task<List<RequestCancelPayment>>>>()))
                .ReturnsAsync(requestCancelPayments);

            requestCancelPayments.ForEach(r => r.UpdateStatus((int)RequestStatuses.Cancel, cancelPayment.UserName, cancelPayment.UserId));

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync()).ReturnsAsync(appParameters).Verifiable();
            _templatesServiceMock.Setup(mock => mock.GetAsync(It.IsAny<string>())).ReturnsAsync("ApprovalCreditCancellation.txt").Verifiable();
            _creditCommonServiceMock.Setup(mock => mock.Commons.Notification.SendMailAsync(It.IsAny<MailNotificationRequest>(),
                    It.IsAny<string>(), It.IsAny<bool>())).Returns(Task.CompletedTask).Verifiable();
            _creditCommonServiceMock.Setup(mock => mock.Commons.Notification.SendSmsAsync(It.IsAny<SmsNotificationRequest>(),
                    It.IsAny<string>(), It.IsAny<bool>())).Returns(Task.CompletedTask).Verifiable();

            await CancelPaymentService.CancelAsync(cancelPayment);

            Assert.True(requestCancelPayments.All(p => p.GetRequestStatusId == (int)RequestStatuses.Cancel));

            _creditCommonServiceMock.Verify(mock => mock.SendEventAsync(It.IsAny<CreditMaster>(), It.IsAny<Customer>(), It.IsAny<Store>(),
                It.IsAny<Credit>(), It.IsAny<TransactionType>(), It.IsAny<string>()), Times.Exactly(4));
        }

        [Fact]
        public async Task ShouldCancelPaymentsTransaction()
        {
            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMaster();

            CreditMasterHelperTest.AddTransaction(creditMaster, TransactionTypes.Payment, creditMaster.Store, Statuses.Active, DateTime.Now, DateTime.Now);
            CreditMasterHelperTest.AddTransaction(creditMaster, TransactionTypes.Payment, creditMaster.Store, Statuses.Active, DateTime.Now, DateTime.Now);
            CreditMasterHelperTest.AddTransaction(creditMaster, TransactionTypes.Payment, creditMaster.Store, Statuses.Active, DateTime.Now, DateTime.Now);

            List<RequestCancelPayment> requestCancelPayments = creditMaster
                .History
                    .Where(h => h.CreditPayment.GetTransactionTypeId == (int)TransactionTypes.Payment)
                    .Select(h =>
                    {
                        return RequestCancelPaymentHelperTest.GetRequestCancelPayment(h.Id).SetCreditMaster(creditMaster).SetPayment(h);
                    }).ToList();

            List<RequestCancelPayment> allRequestCancelPayments =
                requestCancelPayments.Union(new List<RequestCancelPayment>
                {
                    RequestCancelPaymentHelperTest.GetRequestCancelPayment(Guid.NewGuid()).SetCreditMaster(creditMaster)
                }).ToList();

            _appParametersServiceMock.Setup(mock => mock.GetTransactionTypeAsync(It.IsAny<int>()))
                .ReturnsAsync(TransactionTypeHelperTest.GetCancelPaymentType());

            _appParametersServiceMock.Setup(mock => mock.GetStatusAsync(It.IsAny<int>()))
                .ReturnsAsync(StatusHelperTest.GetActiveStatus());

            Func<Transaction, Task<List<RequestCancelPayment>>> cancelPaymentTransactionFunc = CancelPaymentService.CancelTransactionFunc(
                new CancelPaymentsTransactionRequest(creditMaster, requestCancelPayments, allRequestCancelPayments,
                    new UserInfo("Test user", "sdsdkjsdka445sa58d")),
                TransactionTypeHelperTest.GetCancelPaymentType(),
                StatusHelperTest.GetActiveStatus());

            await cancelPaymentTransactionFunc(Transaction.Current);

            Assert.Equal(3, requestCancelPayments.Count(p => p.GetRequestStatusId == (int)RequestStatuses.Cancel));

            _cancelUseCaseMock.Verify(mock => mock.CancelPayment(It.IsAny<CancelPaymentDomainRequest>(),It.IsAny<decimal>(), It.IsAny<bool>()), Times.Exactly(3));

            _creditMasterRepositoryMock.Verify(mock => mock.AddTransactionAsync(It.IsAny<CreditMaster>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<Transaction>()),
                Times.Exactly(4));
        }

        [Fact]
        public async Task ShouldRequestCancelPayment()
        {
            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMaster();

            CreditMasterHelperTest.AddTransaction(creditMaster, TransactionTypes.Payment, creditMaster.Store, Statuses.Active, DateTime.Now);

            CancelPaymentRequest cancelPaymentRequest = new CancelPaymentRequest
            {
                PaymentId = creditMaster.Current.Id,
                Reason = "Anulación de prueba",
                StoreId = creditMaster.GetStoreId,
                UserId = "as54as87de5ds45d",
                UserName = "Test user"
            };

            MailNotificationRequest mailNotificationRequest = new MailNotificationRequestBuilder().Build();
            AppParameters appParameters = ParameterHelperTest.GetAppParameters();
            SmsNotificationRequest smsNotificationRequest = new SmsNotificationRequest();

            _creditMasterRepositoryMock.Setup(mock => mock.GetWithTransactionsByCreditIdAsync(It.IsAny<Guid>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(creditMaster);

            _storeServiceMock.Setup(mock => mock.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(StoreHelperTest.GetStore());

            _requestCancelPaymentRepositoryMock.Setup(mock => mock.GetUndismissedForMasterAsync(It.IsAny<Guid>(),
                    It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(Enumerable.Empty<RequestCancelPayment>().ToList());

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync()).ReturnsAsync(appParameters).Verifiable();
            _templatesServiceMock.Setup(mock => mock.GetAsync(It.IsAny<string>())).ReturnsAsync("ApprovalCreditCancellation.txt").Verifiable();
            _creditCommonServiceMock.Setup(mock => mock.Commons.Notification.SendMailAsync(It.IsAny<MailNotificationRequest>(),
                    It.IsAny<string>(), It.IsAny<bool>())).Returns(Task.CompletedTask).Verifiable();
            _creditCommonServiceMock.Setup(mock => mock.Commons.Notification.SendSmsAsync(It.IsAny<SmsNotificationRequest>(),
                    It.IsAny<string>(), It.IsAny<bool>())).Returns(Task.CompletedTask).Verifiable();

            await CancelPaymentService.RequestAsync(cancelPaymentRequest);

            _requestCancelPaymentRepositoryMock.Verify(mock => mock.AddAsync(It.IsAny<RequestCancelPayment>(), It.IsAny<Transaction>()),
                Times.Once);
        }

        [Fact]
        public async Task ShouldCancelCredit()
        {
            CancelCredit cancelCredit = new CancelCredit
            {
                CreditId = Guid.NewGuid(),
                UserId = "54a5s45asds3d52",
                UserName = "Test user"
            };

            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMaster();
            RequestCancelCredit requestCancelCredit = RequestCancelCreditHelperTest.GetRequestCancelCredit();
            MailNotificationRequest mailNotificationRequest = new MailNotificationRequestBuilder().Build();
            AppParameters appParameters = ParameterHelperTest.GetAppParameters();
            SmsNotificationRequest smsNotificationRequest = new SmsNotificationRequest();

            _requestCancelCreditRepositoryMock.Setup(mock => mock.GetByStatusAsync(It.IsAny<Guid>(),
                    It.IsAny<RequestStatuses>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(requestCancelCredit);

            _creditMasterRepositoryMock.Setup(mock => mock.GetWithTransactionsAsync(It.IsAny<Guid>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(creditMaster);

            _appParametersServiceMock.Setup(mock => mock.GetTransactionTypeAsync(It.IsAny<int>()))
                .ReturnsAsync(TransactionTypeHelperTest.GetCancelPaymentType);

            _appParametersServiceMock.Setup(mock => mock.GetStatusAsync(It.IsAny<int>()))
                .ReturnsAsync(StatusHelperTest.GetCalceledStatus);

            _cancelCreditContextService.Setup(mock => mock.ApproveCancellationAsync(It.IsAny<UserInfo>(), It.IsAny<RequestCancelCredit>(),
                                                                                   It.IsAny<CreditMaster>()));

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync()).ReturnsAsync(appParameters).Verifiable();
            _templatesServiceMock.Setup(mock => mock.GetAsync(It.IsAny<string>())).ReturnsAsync("ApprovalCreditCancellation.txt").Verifiable();
            _creditCommonServiceMock.Setup(mock => mock.Commons.Notification.SendMailAsync(It.IsAny<MailNotificationRequest>(),
                    It.IsAny<string>(), It.IsAny<bool>())).Returns(Task.CompletedTask).Verifiable();
            _creditCommonServiceMock.Setup(mock => mock.Commons.Notification.SendSmsAsync(It.IsAny<SmsNotificationRequest>(),
                    It.IsAny<string>(), It.IsAny<bool>())).Returns(Task.CompletedTask).Verifiable();

            await CancelCreditService.CancelAsync(cancelCredit);

            _requestCancelCreditRepositoryMock.Verify(mock => mock.GetByStatusAsync(It.IsAny<Guid>(),
                    It.IsAny<RequestStatuses>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()), Times.Once);

            _creditMasterRepositoryMock.Verify(mock => mock.GetWithTransactionsAsync(It.IsAny<Guid>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()), Times.Once);

        }
    }
}