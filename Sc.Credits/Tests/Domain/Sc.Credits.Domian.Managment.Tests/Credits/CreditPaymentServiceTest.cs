using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Moq;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Credits;
using Sc.Credits.Domain.Managment.Services.Customers;
using Sc.Credits.Domain.Managment.Services.Stores;
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
    public class CreditPaymentServiceTest
    {
        private readonly Mock<ICreditUsesCase> _creditUsesCaseMock = new Mock<ICreditUsesCase>();
        private readonly Mock<ICreditMasterRepository> _creditMasterRepositoryMock = new Mock<ICreditMasterRepository>();
        private readonly Mock<ICreditPaymentUsesCase> _creditPaymentUsesCaseMock = new Mock<ICreditPaymentUsesCase>();
        private readonly Mock<ICreditCommonsService> _creditCommonsServiceMock = new Mock<ICreditCommonsService>();
        private readonly Mock<IAppParametersService> _appParametersServiceMock = new Mock<IAppParametersService>();
        private readonly Mock<IRequestCancelPaymentRepository> _requestCancelPaymentRepositoryMock = new Mock<IRequestCancelPaymentRepository>();
        private readonly Mock<ICommons> _commonsMock = new Mock<ICommons>();
        private readonly Mock<INotificationService> _notificationServiceMock = new Mock<INotificationService>();
        private readonly Mock<ITemplatesService> _templatesServiceMock = new Mock<ITemplatesService>();
        private readonly Mock<ICustomerService> _customerServiceMock = new Mock<ICustomerService>();
        private readonly Mock<IStoreService> _storeServiceMock = new Mock<IStoreService>();
        private readonly Mock<ICreditPaymentEventsRepository> _creditPaymentEventsRepositoryMock = new Mock<ICreditPaymentEventsRepository>();
        private readonly Mock<ILoggerService<CreditPaymentService>> _mockLoggerService = new Mock<ILoggerService<CreditPaymentService>>();
        private readonly Mock<IRefinancingLogRepository> _mockRefinancingLogRepository = new Mock<IRefinancingLogRepository>();
        private readonly Mock<CredinetAppSettings> _credinetAppSettings = new Mock<CredinetAppSettings>();

        private CreditCommons CreditCommons =>
            CreditCommonsHelperTest.Create(_creditMasterRepositoryMock,
                new Mock<IRequestCancelCreditRepository>(),
                _requestCancelPaymentRepositoryMock,
                _creditUsesCaseMock,
                _creditPaymentUsesCaseMock,
                new Mock<ICancelUseCase>(),
                _creditCommonsServiceMock);

        private ICreditPaymentService CreditPaymentService =>
            new CreditPaymentService(CreditCommons, _creditPaymentEventsRepositoryMock.Object, _mockLoggerService.Object,
                                    _mockRefinancingLogRepository.Object);

        public CreditPaymentServiceTest()
        {
            _appParametersServiceMock.Setup(mock => mock.GetSettings())
               .Returns(new CredinetAppSettings
               {
                   PaymentFutureSecondsLimit = "5",
                   RefinancingSourcesAllowed = "5"
               });

            _commonsMock.SetupGet(mock => mock.AppParameters)
                .Returns(_appParametersServiceMock.Object);

            _creditCommonsServiceMock.SetupGet(mock => mock.Commons)
                .Returns(_commonsMock.Object);

            _creditCommonsServiceMock.SetupGet(mock => mock.CustomerService)
                .Returns(_customerServiceMock.Object);

            _creditCommonsServiceMock.SetupGet(mock => mock.StoreService)
                .Returns(_storeServiceMock.Object);

            _commonsMock.SetupGet(mock => mock.Notification)
                .Returns(_notificationServiceMock.Object);

            _commonsMock.SetupGet(mock => mock.Templates)
                .Returns(_templatesServiceMock.Object);

            _commonsMock.SetupGet(mock => mock.CredinetAppSettings)
                .Returns(_appParametersServiceMock.Object.GetSettings());
        }

        [Fact]
        public async Task ShouldPayCredit()
        {
            PaymentCreditRequestComplete paymentCreditRequest = new PaymentCreditRequestComplete
            {
                BankAccount = string.Empty,
                CalculationDate = new DateTime(2019, 9, 23),
                CreditId = Guid.Parse("a8876e4b-23be-c51f-89bc-08d73fb1ffef"),
                Location = "1,1",
                StoreId = "5c8964fbfe8e3e43d48bf393",
                TotalValuePaid = 25430.78M,
                UserId = "50BC0535-9E3A-43BC-B451-33C8DE162FD0",
                UserName = "Test User"
            };

            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMaster();

            _creditMasterRepositoryMock.Setup(mock => mock.GetWithCurrentAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(creditMaster);

            _appParametersServiceMock.Setup(mock => mock.GetTransactionTypeAsync(It.IsAny<int>()))
                .ReturnsAsync(TransactionTypeHelperTest.GetPaymentType());

            _storeServiceMock.Setup(mock => mock.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(StoreHelperTest.GetStore());

            _customerServiceMock.Setup(mock => mock.GetAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(CustomerHelperTest.GetCustomer());

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            decimal totalPayment = paymentCreditRequest.TotalValuePaid;
            _creditPaymentUsesCaseMock.Setup(mock =>
                mock.GetPaymentAlternatives(It.IsAny<CreditMaster>(), It.IsAny<Credit>(), It.IsAny<AppParameters>(),
                    It.IsAny<DateTime>()))
                .Returns(new PaymentAlternatives { MinimumPayment = 0, TotalPayment = totalPayment });

            _creditPaymentUsesCaseMock.Setup(mock => mock.PayAsync(It.IsAny<PaymentDomainRequest>(), It.IsAny<PaymentType>(), It.IsAny<Transaction>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(new PaymentCreditResponse
                {
                    CreditMaster = creditMaster
                });

            PaymentCreditResponse paymentCreditResponse = await CreditPaymentService.PayCreditAsync(paymentCreditRequest, true);

            Assert.IsType<PaymentCreditResponse>(paymentCreditResponse);

            _creditCommonsServiceMock.Verify(mock => mock.SendEventAsync(It.IsAny<CreditMaster>(), It.IsAny<Customer>(), It.IsAny<Store>(),
                It.IsAny<Credit>(), It.IsAny<TransactionType>(), It.IsAny<string>()), Times.Once);

            _commonsMock.Verify(mock => mock.Notification.SendMailAsync(It.IsAny<MailNotificationRequest>(), It.IsAny<string>(),
                    It.IsAny<bool>()),
                Times.Once);

            _commonsMock.Verify(mock => mock.Notification.SendSmsAsync(It.IsAny<SmsNotificationRequest>(), It.IsAny<string>(),
                    It.IsAny<bool>()),
                Times.Once);
        }

        [Fact]
        public async Task ShouldPayCreditpaymentCreditRequestShort()
        {
            PaymentCreditRequest paymentCreditRequest = new PaymentCreditRequest
            {
                BankAccount = string.Empty,
                CreditId = Guid.Parse("a8876e4b-23be-c51f-89bc-08d73fb1ffef"),
                Location = "1,1",
                StoreId = "5c8964fbfe8e3e43d48bf393",
                TotalValuePaid = 25430.78M,
                UserId = "50BC0535-9E3A-43BC-B451-33C8DE162FD0",
                UserName = "Test User"
            };

            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMaster();

            _creditMasterRepositoryMock.Setup(mock => mock.GetWithCurrentAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(creditMaster);

            _appParametersServiceMock.Setup(mock => mock.GetTransactionTypeAsync(It.IsAny<int>()))
                .ReturnsAsync(TransactionTypeHelperTest.GetPaymentType());

            _storeServiceMock.Setup(mock => mock.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(StoreHelperTest.GetStore());

            _customerServiceMock.Setup(mock => mock.GetAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(CustomerHelperTest.GetCustomer());

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            decimal totalPayment = paymentCreditRequest.TotalValuePaid;

            _creditPaymentUsesCaseMock.Setup(mock =>
                mock.GetPaymentAlternatives(It.IsAny<CreditMaster>(), It.IsAny<Credit>(), It.IsAny<AppParameters>(), It.IsAny<DateTime>()))
                .Returns(new PaymentAlternatives { MinimumPayment = 0, TotalPayment = totalPayment });

            _creditPaymentUsesCaseMock.Setup(mock => mock.PayAsync(It.IsAny<PaymentDomainRequest>(), It.IsAny<PaymentType>(), It.IsAny<Transaction>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(new PaymentCreditResponse
                {
                    CreditMaster = creditMaster
                });

            PaymentCreditResponse paymentCreditResponse = await CreditPaymentService.PayCreditAsync(paymentCreditRequest, true);

            Assert.IsType<PaymentCreditResponse>(paymentCreditResponse);

            _creditPaymentUsesCaseMock.Verify(mock => mock.PayAsync(It.IsAny<PaymentDomainRequest>(), It.IsAny<PaymentType>(), It.IsAny<Transaction>(), It.IsAny<bool>(), It.IsAny<bool>()),
                Times.Once);

            _creditCommonsServiceMock.Verify(mock => mock.SendEventAsync(It.IsAny<CreditMaster>(), It.IsAny<Customer>(), It.IsAny<Store>(),
                It.IsAny<Credit>(), It.IsAny<TransactionType>(), It.IsAny<string>()), Times.Once);

            _commonsMock.Verify(mock => mock.Notification.SendMailAsync(It.IsAny<MailNotificationRequest>(), It.IsAny<string>(),
                    It.IsAny<bool>()),
                Times.Once);

            _commonsMock.Verify(mock => mock.Notification.SendSmsAsync(It.IsAny<SmsNotificationRequest>(), It.IsAny<string>(),
                    It.IsAny<bool>()),
                Times.Once);
        }

        [Fact]
        public async Task ShouldPayCreditMultiple()
        {
            List<CreditMaster> creditMasters = CreditMasterHelperTest.GetCreditMasterList().Where(item => item.Id != Guid.Empty).ToList();

            PayCreditsRequest payCreditsRequest = new PayCreditsRequest
            {
                BankAccount = string.Empty,
                Location = "1,1",
                StoreId = "5c8964fbfe8e3e43d48bf393",
                UserId = "50BC0535-9E3A-43BC-B451-33C8DE162FD0",
                UserName = "Test User",
                CreditPaymentDetails = creditMasters
                    .Select(creditMaster =>
                        new PaymentCreditMultipleDetail
                        {
                            CreditId = creditMaster.Id,
                            TotalValuePaid = 1000
                        }).ToList()
            };

            Store store = StoreHelperTest.GetStore();

            _creditMasterRepositoryMock.Setup(mock => mock.GetWithCurrentAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync((Guid creditMasterId, IEnumerable<Field> fields, IEnumerable<Field> transactionFields,
                    IEnumerable<Field> customerFields, IEnumerable<Field> storeFields, IEnumerable<Field> transactionStoreFields) =>
                    creditMasters
                        .First(creditMaster =>
                            creditMaster.Id == creditMasterId));

            _appParametersServiceMock.Setup(mock => mock.GetTransactionTypeAsync(It.IsAny<int>()))
                .ReturnsAsync(TransactionTypeHelperTest.GetPaymentType());

            _storeServiceMock.Setup(mock => mock.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(store);

            _customerServiceMock.Setup(mock => mock.GetAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(CustomerHelperTest.GetCustomer());
          
            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            List<PaymentCreditResponse> expectedResponses = new List<PaymentCreditResponse>();
            _creditMasterRepositoryMock.Setup(mock => mock.ExcecuteOnTransactionAsync(It.IsAny<Func<Transaction, Task<List<PaymentCreditResponse>>>>()))
                .Callback<Func<Transaction, Task<List<PaymentCreditResponse>>>>(async handle => expectedResponses = await handle.Invoke(null))
                .ReturnsAsync(() => expectedResponses);

            decimal totalPayment = 200000;

            _creditPaymentUsesCaseMock.Setup(mock => mock.GetPaymentAlternatives(It.IsAny<CreditMaster>(), It.IsAny<Credit>(), It.IsAny<AppParameters>(),
                It.IsAny<DateTime>()))
                .Returns(new PaymentAlternatives { MinimumPayment = 20000, TotalPayment = totalPayment });

            _creditPaymentUsesCaseMock.Setup(mock => mock.PayAsync(It.IsAny<PaymentDomainRequest>(), It.IsAny<PaymentType>(), It.IsAny<Transaction>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync((PaymentDomainRequest paymentDomainRequest, PaymentType paymentType, Transaction transaction,bool simulation, bool setCreditLimit) =>
                    new PaymentCreditResponse
                    {
                        CreditMaster = creditMasters
                        .First(creditMaster =>
                            creditMaster.Id == paymentDomainRequest.CreditMaster.Id)
                    });

            List<PaymentCreditResponse> paymentCreditResponses = await CreditPaymentService.PayCreditMultipleAsync(payCreditsRequest);

            int expectedActions = paymentCreditResponses.Count;

            Assert.IsType<List<PaymentCreditResponse>>(paymentCreditResponses);
        }

        [Fact]
        public async Task PayCreditMultipleNotifyAsync()
        {
            // Arrange
            List<CreditMaster> creditMasters = CreditMasterHelperTest.GetCreditMasterList().Where(item => item.Id != Guid.Empty).ToList();
            PayCreditsRequest paymentCreditsRequest = new PayCreditsRequest()
            {
                UserName = "sebastian",
                UserId = "4455",
                StoreId = "243ft35t2rf34t534",
                CreditPaymentDetails = creditMasters
                    .Select(creditMaster =>
                        new PaymentCreditMultipleDetail
                        {
                            CreditId = creditMaster.Id,
                            TotalValuePaid = 1000
                        }).ToList(),
                TransactionId = "2f4rt2tr23rer3rdwq"
            };
            Store store = StoreHelperTest.GetStore();

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync()).ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _appParametersServiceMock.Setup(mock => mock.GetTransactionTypeAsync(It.IsAny<int>()))
                .ReturnsAsync(TransactionTypeHelperTest.GetPaymentType());

            _creditMasterRepositoryMock.Setup(mock => mock.GetWithCurrentAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync((Guid creditMasterId, IEnumerable<Field> fields, IEnumerable<Field> transactionFields,
                    IEnumerable<Field> customerFields, IEnumerable<Field> storeFields, IEnumerable<Field> transactionStoreFields) =>
                    creditMasters
                        .First(creditMaster =>
                            creditMaster.Id == creditMasterId));

            _storeServiceMock.Setup(mock => mock.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(store);

            _customerServiceMock.Setup(mock => mock.GetAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(CustomerHelperTest.GetCustomer());

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
               .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            List<PaymentCreditResponse> expectedResponses = new List<PaymentCreditResponse>();
            _creditMasterRepositoryMock.Setup(mock => mock.ExcecuteOnTransactionAsync(It.IsAny<Func<Transaction, Task<List<PaymentCreditResponse>>>>()))
                .Callback<Func<Transaction, Task<List<PaymentCreditResponse>>>>(async handle => expectedResponses = await handle.Invoke(null))
                .ReturnsAsync(() => expectedResponses);

            decimal totalPayment = 200000;

            _creditPaymentUsesCaseMock.Setup(mock => mock.GetPaymentAlternatives(It.IsAny<CreditMaster>(), It.IsAny<Credit>(), It.IsAny<AppParameters>(),
                It.IsAny<DateTime>()))
                .Returns(new PaymentAlternatives { MinimumPayment = 20000, TotalPayment = totalPayment });

            _creditPaymentUsesCaseMock.Setup(mock => mock.PayAsync(It.IsAny<PaymentDomainRequest>(), It.IsAny<PaymentType>(), It.IsAny<Transaction>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync((PaymentDomainRequest paymentDomainRequest, PaymentType paymentType, Transaction transaction,bool simulation, bool setCreditLimit) =>
                    new PaymentCreditResponse
                    {
                        CreditMaster = creditMasters
                        .First(creditMaster =>
                            creditMaster.Id == paymentDomainRequest.CreditMaster.Id)
                    });
            //Act
            await CreditPaymentService.PayCreditMultipleAndNotifyAsync(paymentCreditsRequest);

            //Assert

            _creditPaymentEventsRepositoryMock.Verify(x => x.SendPayCreditsEventsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task PayCreditMultipleNotifyAsync_SendException()
        {
            // Arrange
            PayCreditsRequest paymentCreditsRequest = new PayCreditsRequest()
            {
                UserName = "sebastian",
                UserId = "4455",
                StoreId = "243ft35t2rf34t534",
                CreditPaymentDetails = new List<PaymentCreditMultipleDetail>()
                {
                    new PaymentCreditMultipleDetail()
                    {
                      CreditId= Guid.NewGuid(),
                      TotalValuePaid= 2000
                    },
                    new PaymentCreditMultipleDetail()
                    {
                      CreditId= Guid.NewGuid(),
                      TotalValuePaid= 4000
                    }
                },
                TransactionId = "2f4rt2tr23rer3rdwq"
            };

            _appParametersServiceMock.Setup(mock => mock.GetTransactionTypeAsync(It.IsAny<int>()))
                .ReturnsAsync(TransactionTypeHelperTest.GetPaymentType());

            _creditMasterRepositoryMock.Setup(mock => mock.ExcecuteOnTransactionAsync(It.IsAny<Func<Transaction, Task<List<PaymentCreditResponse>>>>()))
               .ThrowsAsync(new Exception());

            //Act
            await CreditPaymentService.PayCreditMultipleAndNotifyAsync(paymentCreditsRequest);

            //Assert
            _creditPaymentEventsRepositoryMock.Verify(x => x.SendPayCreditsEventsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task PayCreditMultipleNotifyAsync_SendBusinessException()
        {
            // Arrange
            PayCreditsRequest paymentCreditsRequest = new PayCreditsRequest()
            {
                UserName = "sebastian",
                UserId = "4455",
                StoreId = "243ft35t2rf34t534",
                CreditPaymentDetails = new List<PaymentCreditMultipleDetail>()
                {
                    new PaymentCreditMultipleDetail()
                    {
                      CreditId= Guid.NewGuid(),
                      TotalValuePaid= 2000
                    }
                },
                TransactionId = "2f4rt2tr23rer3rdwq"
            };

            _appParametersServiceMock.Setup(mock => mock.GetTransactionTypeAsync(It.IsAny<int>()))
                .ReturnsAsync(TransactionTypeHelperTest.GetPaymentType());

            //Act
            await CreditPaymentService.PayCreditMultipleAndNotifyAsync(paymentCreditsRequest);

            //Assert

            _creditPaymentEventsRepositoryMock.Verify(x => x.SendPayCreditsEventsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task ShouldGetPaymentFees()
        {
            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMaster();

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditMasterRepositoryMock.Setup(mock => mock.GetWithCurrentAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(creditMaster);

            var paymentFees = new PaymentFeesResponse() { PendingFees = 0, PaymentFees = new List<PaymentFee>(new PaymentFee[3]) };

            decimal totalPayment;
            _creditPaymentUsesCaseMock.Setup(mock => mock.GetPaymentFees(It.IsAny<CreditMaster>(), It.IsAny<Credit>(), It.IsAny<AppParameters>(),
                It.IsAny<DateTime>(), out totalPayment))
                .Returns(paymentFees);

            Guid creditId = creditMaster.Id;

            var result = await CreditPaymentService.GetPaymentFeesAsync(creditId);

            Assert.NotNull(result);
            Assert.IsType<PaymentFeesResponse>(result);
        }

        [Fact]
        public async Task ShouldGetPaymentFeesError_CreditNotActive()
        {
            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMaster();
            creditMaster.SetStatus(StatusHelperTest.GetCancelRequestStatus());

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditMasterRepositoryMock.Setup(mock => mock.GetWithCurrentAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(creditMaster);

            var paymentFees = new PaymentFeesResponse() { PendingFees = 0, PaymentFees = new List<PaymentFee>(new PaymentFee[3]) };

            decimal totalPayment;
            _creditPaymentUsesCaseMock.Setup(mock => mock.GetPaymentFees(It.IsAny<CreditMaster>(), It.IsAny<Credit>(), It.IsAny<AppParameters>(),
                It.IsAny<DateTime>(), out totalPayment))
                .Returns(paymentFees);

            Guid creditId = creditMaster.Id;

            BusinessException businessException = await Assert.ThrowsAsync<BusinessException>(() =>
               CreditPaymentService.GetPaymentFeesAsync(creditId));

            Assert.Equal((int)BusinessResponse.CreditNotActive, businessException.code);
        }

        [Fact]
        public async Task ShouldGetPaymentTemplatesSuccessfully()
        {
            List<Credit> credits = CreditMasterHelperTest.GetCreditMasterListTemplate().SelectMany(item => item.History).ToList();

            credits.ForEach(credit =>
            {
                credit.SetCreditMaster(CreditMasterHelperTest.GetCreditMasterListTemplate().First(item => item.GetCreditNumber == credit.GetCreditNumber));
            });

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditMasterRepositoryMock.Setup(mock => mock.GetPaymentsAsync(It.IsAny<List<Guid>>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>()))
                .Returns(Task.FromResult(credits));

            _creditPaymentUsesCaseMock.Setup(item => item.GetPaymentTemplate(It.IsAny<PaymentTemplateResponse>(), It.IsAny<int>(), It.IsAny<bool>()));

            List<Guid> paymentIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

            var result = await CreditPaymentService.GetPaymentTemplatesAsync(paymentIds, false);

            Assert.NotNull(result);
            Assert.Contains(result, r => r.TotalBalance == 0);
            Assert.IsType<List<PaymentTemplateResponse>>(result);
        }

        [Fact]
        public async Task GetPaymentTemplates_NoPaymentIds_ThrowsBusinessException()
        {
            List<Guid> paymentIds = new List<Guid> { };

            var businessException =
                await Assert.ThrowsAsync<BusinessException>(() => CreditPaymentService.GetPaymentTemplatesAsync(paymentIds, true));

            Assert.Equal((int)BusinessResponse.RequestValuesInvalid, businessException.code);
        }

        [Fact]
        public async Task ShouldGetCurrentAmortizationScheduleAsyncSuccessfully()
        {
            CurrentAmortizationScheduleResponse currentAmortizationScheduleResponse =
                AmortizationScheduleHelperTest.GetCurrentAmortizationScheduleResponse();

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditPaymentUsesCaseMock.Setup(mock => mock.GetCurrentAmortizationSchedule(It.IsAny<CurrentAmortizationScheduleRequest>(), It.IsAny<DateTime>(),
                    It.IsAny<AppParameters>()))
                .Returns(currentAmortizationScheduleResponse);

            CurrentAmortizationScheduleResponse result =
                await CreditPaymentService.GetCurrentAmortizationScheduleAsync(
                    AmortizationScheduleHelperTest.GetCurrentAmortizationScheduleRequest(creditValue: 4000M, initialDate: new DateTime(2019, 10, 23),
                        feeValue: 20632.1M, interestRate: 0.0209971486861902M, frequency: 30, fees: 2, downPayment: 0M, assuranceValue: 4000M,
                        assuranceFeeValue: 2000.0M, assuranceTotalFeeValue: 2380M, calculationDate: new DateTime(2019, 10, 23),
                        lastPaymentDate: new DateTime(2019, 9, 23), balance: 30000.0M, assuranceBalance: 4760.0M, hasArrearsCharge: false,
                        arrearsCharges: 0));

            Assert.NotNull(result);
            Assert.Equal(currentAmortizationScheduleResponse.AssuranceValue, result.AssuranceValue);
        }

        [Fact]
        public async Task ShouldGetCurrentPaymentScheduleAsyncSuccessfully()
        {
            CurrentPaymentScheduleRequest currentPaymentScheduleRequest = CurrentPaymentScheduleHelperTest.GetCurrentPaymentScheduleRequest();
            currentPaymentScheduleRequest.InterestRate = 0;

            CurrentPaymentScheduleResponse currentPaymentScheduleResponse =
                AmortizationScheduleHelperTest.GetCurrentPaymentScheduleAsyncResponse();

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditPaymentUsesCaseMock.Setup(mock => mock.GetCurrentPaymentSchedule(It.IsAny<CurrentPaymentScheduleRequest>(), It.IsAny<DateTime>(),
                    It.IsAny<AppParameters>()))
                .Returns(currentPaymentScheduleResponse);

            CurrentPaymentScheduleResponse result =
                await CreditPaymentService.GetCurrentPaymentScheduleAsync(
                    CurrentPaymentScheduleHelperTest.GetCurrentPaymentScheduleRequest(), DateTime.Today);

            Assert.NotNull(result);
            _creditPaymentUsesCaseMock.Verify(mock => mock.GetCurrentPaymentSchedule(It.IsAny<CurrentPaymentScheduleRequest>(), It.IsAny<DateTime>(), It.IsAny<AppParameters>()), Times.Once);
        }

        [Fact]
        public async Task ShouldGetCurrentPaymentScheduleAsync_WithException()
        {
            CurrentPaymentScheduleRequest currentPaymentScheduleRequest = CurrentPaymentScheduleHelperTest.GetCurrentPaymentScheduleRequest();
            currentPaymentScheduleRequest.CreditValue = 0;
            currentPaymentScheduleRequest.UpdatedPaymentPlanValue = 0;

            CurrentPaymentScheduleResponse currentPaymentScheduleResponse =
                AmortizationScheduleHelperTest.GetCurrentPaymentScheduleAsyncResponse();

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditPaymentUsesCaseMock.Setup(mock => mock.GetCurrentPaymentSchedule(It.IsAny<CurrentPaymentScheduleRequest>(), It.IsAny<DateTime>(),
                    It.IsAny<AppParameters>()))
                .Returns(currentPaymentScheduleResponse);

            var result =
                await Assert.ThrowsAsync<BusinessException>(() => CreditPaymentService.GetCurrentPaymentScheduleAsync(currentPaymentScheduleRequest, DateTime.Today));

            Assert.Equal((int)BusinessResponse.RequestValuesInvalid, result.code);
        }

        [Fact]
        public async Task ShouldGetCurrentPaymentScheduleAsync_InvalidUpdatedPaymentPlanValue_SholuldThrowBusinessException()
        {
            CurrentPaymentScheduleRequest currentPaymentScheduleRequest = CurrentPaymentScheduleHelperTest.GetCurrentPaymentScheduleRequest();
            currentPaymentScheduleRequest.UpdatedPaymentPlanValue = -1;

            CurrentPaymentScheduleResponse currentPaymentScheduleResponse =
                AmortizationScheduleHelperTest.GetCurrentPaymentScheduleAsyncResponse();

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditPaymentUsesCaseMock.Setup(mock => mock.GetCurrentPaymentSchedule(It.IsAny<CurrentPaymentScheduleRequest>(), It.IsAny<DateTime>(),
                    It.IsAny<AppParameters>()))
                .Returns(currentPaymentScheduleResponse);

            var result =
                await Assert.ThrowsAsync<BusinessException>(() => CreditPaymentService.GetCurrentPaymentScheduleAsync(currentPaymentScheduleRequest, DateTime.Today));

            Assert.Equal((int)BusinessResponse.RequestValuesInvalid, result.code);
        }

        [Fact]
        public async Task ShouldGetCurrentPaymentScheduleAsync_InvalidCalculationDate_SholuldThrowBusinessException()
        {
            CurrentPaymentScheduleRequest currentPaymentScheduleRequest = CurrentPaymentScheduleHelperTest.GetCurrentPaymentScheduleRequest();
            currentPaymentScheduleRequest.CreditValue = 0;

            CurrentPaymentScheduleResponse currentPaymentScheduleResponse =
                AmortizationScheduleHelperTest.GetCurrentPaymentScheduleAsyncResponse();

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditPaymentUsesCaseMock.Setup(mock => mock.GetCurrentPaymentSchedule(It.IsAny<CurrentPaymentScheduleRequest>(), It.IsAny<DateTime>(),
                    It.IsAny<AppParameters>()))
                .Returns(currentPaymentScheduleResponse);

            var result =
                await Assert.ThrowsAsync<BusinessException>(() => CreditPaymentService.GetCurrentPaymentScheduleAsync(currentPaymentScheduleRequest, new DateTime(2019, 10, 23)));

            Assert.Equal((int)BusinessResponse.RequestValuesInvalid, result.code);
        }

        [Fact]
        public async Task ShouldGetCurrentPaymentScheduleAsync_InvalidPreviousArrears_SholuldThrowBusinessException()
        {
            CurrentPaymentScheduleRequest currentPaymentScheduleRequest = CurrentPaymentScheduleHelperTest.GetCurrentPaymentScheduleRequest();
            currentPaymentScheduleRequest.PreviousArrears = -1;

            CurrentPaymentScheduleResponse currentPaymentScheduleResponse =
                AmortizationScheduleHelperTest.GetCurrentPaymentScheduleAsyncResponse();

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditPaymentUsesCaseMock.Setup(mock => mock.GetCurrentPaymentSchedule(It.IsAny<CurrentPaymentScheduleRequest>(), It.IsAny<DateTime>(),
                    It.IsAny<AppParameters>()))
                .Returns(currentPaymentScheduleResponse);

            var result =
                await Assert.ThrowsAsync<BusinessException>(() => CreditPaymentService.GetCurrentPaymentScheduleAsync(currentPaymentScheduleRequest, new DateTime(2019, 10, 23)));

            Assert.Equal((int)BusinessResponse.RequestValuesInvalid, result.code);
        }

        [Fact]
        public async Task ShouldGetCurrentPaymentScheduleAsync_InvalidPreviousInterest_SholuldThrowBusinessException()
        {
            CurrentPaymentScheduleRequest currentPaymentScheduleRequest = CurrentPaymentScheduleHelperTest.GetCurrentPaymentScheduleRequest();
            currentPaymentScheduleRequest.PreviousInterest = -1;

            CurrentPaymentScheduleResponse currentPaymentScheduleResponse =
                AmortizationScheduleHelperTest.GetCurrentPaymentScheduleAsyncResponse();

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditPaymentUsesCaseMock.Setup(mock => mock.GetCurrentPaymentSchedule(It.IsAny<CurrentPaymentScheduleRequest>(), It.IsAny<DateTime>(),
                    It.IsAny<AppParameters>()))
                .Returns(currentPaymentScheduleResponse);

            var result =
                await Assert.ThrowsAsync<BusinessException>(() => CreditPaymentService.GetCurrentPaymentScheduleAsync(currentPaymentScheduleRequest, new DateTime(2019, 10, 23)));

            Assert.Equal((int)BusinessResponse.RequestValuesInvalid, result.code);
        }

        [Fact]
        public async Task ShouldGetCurrentPaymentScheduleAsync_InvalidChargeValue_SholuldThrowBusinessException()
        {
            CurrentPaymentScheduleRequest currentPaymentScheduleRequest = CurrentPaymentScheduleHelperTest.GetCurrentPaymentScheduleRequest();
            currentPaymentScheduleRequest.ChargeValue = -1;

            CurrentPaymentScheduleResponse currentPaymentScheduleResponse =
                AmortizationScheduleHelperTest.GetCurrentPaymentScheduleAsyncResponse();

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditPaymentUsesCaseMock.Setup(mock => mock.GetCurrentPaymentSchedule(It.IsAny<CurrentPaymentScheduleRequest>(), It.IsAny<DateTime>(),
                    It.IsAny<AppParameters>()))
                .Returns(currentPaymentScheduleResponse);

            var result =
                await Assert.ThrowsAsync<BusinessException>(() => CreditPaymentService.GetCurrentPaymentScheduleAsync(currentPaymentScheduleRequest, new DateTime(2019, 10, 23)));

            Assert.Equal((int)BusinessResponse.RequestValuesInvalid, result.code);
        }

        [Fact]
        public async Task ShouldGetActiveCredits()
        {
            List<CreditMaster> creditMasters = CreditMasterHelperTest.GetCreditMasterList();
            Customer customer = CustomerHelperTest.GetCustomer();
            Store store = StoreHelperTest.GetStore();

            _customerServiceMock.Setup(mock => mock.GetAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(customer);

            _storeServiceMock.Setup(mock => mock.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(store);

            _creditMasterRepositoryMock.Setup(cr => cr.GetActiveCreditsAsync(It.IsAny<Customer>()))
                .ReturnsAsync(creditMasters);

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditPaymentUsesCaseMock.Setup(item => item.GetActiveCredits(It.IsAny<List<CreditMaster>>(), It.IsAny<AppParameters>(), It.IsAny<DateTime>()))
                .Returns(new List<CreditStatus>());

            List<CreditStatus> result = await CreditPaymentService.GetActiveCreditsAsync(customer.DocumentType, customer.IdDocument, store.Id, DateTime.Now);

            Assert.NotNull(result);
            Assert.All(result, item => item.CustomerFullName = string.Empty);
        }

        [Fact]
        public async Task ShouldDetailedGetActiveCredits()
        {
            List<CreditMaster> creditMasters = CreditMasterHelperTest.GetCreditMasterList();
            Customer customer = CustomerHelperTest.GetCustomer();
            Store store = StoreHelperTest.GetStore();

            _customerServiceMock.Setup(mock => mock.GetAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(customer);

            _storeServiceMock.Setup(mock => mock.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(store);

            _creditMasterRepositoryMock.Setup(cr => cr.GetActiveCreditsAsync(It.IsAny<Customer>()))
                .ReturnsAsync(creditMasters);

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditPaymentUsesCaseMock.Setup(item => item.GetDetailedActiveCredits(It.IsAny<List<CreditMaster>>(), It.IsAny<AppParameters>(), It.IsAny<DateTime>()))
                .Returns(new List<DetailedCreditStatus>());

            List<DetailedCreditStatus> result = await CreditPaymentService.GetDetailedActiveCreditsAsync(customer.DocumentType, customer.IdDocument, store.Id, DateTime.Now);

            Assert.NotNull(result);
            Assert.All(result, item => item.CustomerFullName = string.Empty);
        }

        [Fact]
        public async Task ShouldDetailedGetActiveCreditsByCreditsMasterId()
        {
            List<CreditMaster> creditMasters = CreditMasterHelperTest.GetCreditMasterList();
            Customer customer = CustomerHelperTest.GetCustomer();
            Store store = StoreHelperTest.GetStore();
            CredinetAppSettings appsettings = CredinetAppSettingsHelperTest.GetCredinetAppSettings();
            var limit = appsettings.LimitGetCreditMasterId;

            List<Guid> ids = new List<Guid>(){
                new Guid(),
                new Guid(),
                new Guid(),
                new Guid(),
                new Guid(),
                new Guid(),
            };

            _creditMasterRepositoryMock.Setup(cr => cr.GetWithTransactionsAsync(It.IsAny<List<Guid>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(),
                It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<int>()))
                .ReturnsAsync(creditMasters);

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditPaymentUsesCaseMock.Setup(item => item.GetDetailedActiveCredits(It.IsAny<List<CreditMaster>>(), It.IsAny<AppParameters>(), It.IsAny<DateTime>()))
                .Returns(new List<DetailedCreditStatus>());

            _commonsMock.SetupGet(mock => mock.CredinetAppSettings)
            .Returns(appsettings);

            List<DetailedCreditStatus> result = await CreditPaymentService.GetDetailedActiveCreditsByCreditMasterIdAsync(ids, DateTime.Now);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task ShouldDetailedGetActiveCreditsByCreditsMasterIdLimitExceeded()
        {
            List<CreditMaster> creditMasters = CreditMasterHelperTest.GetCreditMasterList();
            Customer customer = CustomerHelperTest.GetCustomer();
            Store store = StoreHelperTest.GetStore();
            CredinetAppSettings appsettings = CredinetAppSettingsHelperTest.GetCredinetAppSettings();
            var limit = appsettings.LimitGetCreditMasterId;

            List<Guid> ids = new List<Guid>(){
                new Guid(),
                new Guid(),
                new Guid(),
                new Guid(),
                new Guid(),
                new Guid(),
                new Guid(),
                new Guid(),
                new Guid(),
                new Guid(),
                new Guid(),
                new Guid(),
                new Guid(),
                new Guid(),
            };

            _creditMasterRepositoryMock.Setup(cr => cr.GetWithTransactionsAsync(It.IsAny<List<Guid>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(),
                It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<int>()))
                .ReturnsAsync(creditMasters);

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditPaymentUsesCaseMock.Setup(item => item.GetDetailedActiveCredits(It.IsAny<List<CreditMaster>>(), It.IsAny<AppParameters>(), It.IsAny<DateTime>()))
                .Returns(new List<DetailedCreditStatus>());

            _commonsMock.SetupGet(mock => mock.CredinetAppSettings)
            .Returns(appsettings);
            try
            {
                List<DetailedCreditStatus> result = await CreditPaymentService.GetDetailedActiveCreditsByCreditMasterIdAsync(ids, DateTime.Now);

            }
            catch (BusinessException ex)
            {
                Assert.Equal((int)BusinessResponse.limitExceededOfCreditmastersId, ex.code);
            }
        }

        [Fact]
        public async Task ActiveCreditsNotify()
        {
            // Arrange
            ActiveCreditsRequest activeCreditsRequest = new ActiveCreditsRequest()
            {
                IdDocument = "545454",
                TypeDocument = "CC",
                StoreId = "f35tyg46hg4g545434rt45",
                TransactionId = "bhk767u687653u574yt4364"
            };

            List<CreditStatus> creditStatusList = new List<CreditStatus>()
            {
                new CreditStatus()
                    {
                        IdDocument = "545454",
                        TypeDocument= "CC",
                        TotalPayment = 20000
                    }
            };

            List<CreditMaster> creditMasters = CreditMasterHelperTest.GetCreditMasterList();
            Customer customer = CustomerHelperTest.GetCustomer();
            Store store = StoreHelperTest.GetStore();

            _customerServiceMock.Setup(mock => mock.GetAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(customer);

            _storeServiceMock.Setup(mock => mock.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(store);

            _creditMasterRepositoryMock.Setup(cr => cr.GetActiveCreditsAsync(It.IsAny<Customer>()))
                .ReturnsAsync(creditMasters);

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditPaymentUsesCaseMock.Setup(item => item.GetActiveCredits(
                It.IsAny<List<CreditMaster>>(), It.IsAny<AppParameters>(), It.IsAny<DateTime>()))
                .Returns(creditStatusList);

            //Act

            await CreditPaymentService.ActiveCreditsNotifyAsync(activeCreditsRequest);

            //Asser

            _creditPaymentEventsRepositoryMock.Verify(x => x.SendActiveCreditsEventsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task ActiveCreditsNotify_SendException()
        {
            // Arrange
            ActiveCreditsRequest activeCreditsRequest = new ActiveCreditsRequest()
            {
                IdDocument = "545454",
                TypeDocument = "CC",
                StoreId = "f35tyg46hg4g545434rt45",
                TransactionId = "bhk767u687653u574yt4364"
            };

            List<CreditStatus> creditStatusList = new List<CreditStatus>()
            {
                new CreditStatus()
                    {
                        IdDocument = "545454",
                        TypeDocument= "CC",
                        TotalPayment = 20000
                    }
            };

            _customerServiceMock.Setup(mock => mock.GetAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>())).ThrowsAsync(new Exception());

            _creditPaymentUsesCaseMock.Setup(item => item.GetActiveCredits(It.IsAny<List<CreditMaster>>(), It.IsAny<AppParameters>(), It.IsAny<DateTime>()))
                .Returns(creditStatusList);

            //Act
            await CreditPaymentService.ActiveCreditsNotifyAsync(activeCreditsRequest);

            //Asser
            _creditPaymentEventsRepositoryMock.Verify(x => x.SendActiveCreditsEventsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task ActiveCreditsNotifyAsync_SendBusinessException()
        {
            // Arrange
            ActiveCreditsRequest activeCreditsRequest = new ActiveCreditsRequest()
            {
                TypeDocument = "CC",
                StoreId = "f35tyg46hg4g545434rt45",
                TransactionId = "bhk767u687653u574yt4364"
            };

            //Act

            await CreditPaymentService.ActiveCreditsNotifyAsync(activeCreditsRequest);

            //Asser

            _creditPaymentEventsRepositoryMock.Verify(x => x.SendActiveCreditsEventsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task ShouldGetDataCalculateCredit()
        {
            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMaster();

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditMasterRepositoryMock.Setup(mock => mock.GetWithCurrentAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>()))
            .ReturnsAsync(creditMaster);

            decimal totalPayment = 0;

            _creditPaymentUsesCaseMock.Setup(mock => mock.GetPaymentAlternatives(It.IsAny<CreditMaster>(), It.IsAny<Credit>(), It.IsAny<AppParameters>(),
                It.IsAny<DateTime>()))
                .Returns(new PaymentAlternatives { TotalPayment = totalPayment, PaymentFees = PaymentFeesHelperTest.GetPaymentFeesResponse() });

            CalculatedQuery calculatedQuery = await CreditPaymentService.GetDataCalculateCreditAsync(Guid.NewGuid(), DateTime.Now);

            Assert.NotNull(calculatedQuery);
        }

        [Fact]
        public async Task ShouldGetCustomerPaymentHistory()
        {
            string storeId = "TestStoreId";
            string documentType = "CC";
            string idDocument = "TestIdDocument";

            Customer customer = CustomerHelperTest.GetCustomer();
            List<CreditMaster> creditMasters = CreditMasterHelperTest.GetCreditMasterList();
            Store store = StoreHelperTest.GetStore();
            List<Credit> payments =
                creditMasters
                    .Select(creditMaster => creditMaster.Current)
                    .ToList();

            List<PaymentHistoryResponse> paymentHistoryResponses = PaymentHistoryHelperTest.GetResponsesFromPayments(payments);
            List<RequestCancelPayment> requestCancelPaymentsCanceled = RequestCancelPaymentHelperTest.GetRequestCancelPaymentList();

            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            _customerServiceMock.Setup(mock => mock.GetAsync(idDocument, documentType,
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(customer);

            _storeServiceMock.Setup(mock => mock.GetAsync(storeId, It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(store);

            _creditMasterRepositoryMock.Setup(mock => mock.GetCustomerPaymentHistoryAsync(It.IsAny<Customer>(),
                    It.IsAny<Store>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<int>()))
                .ReturnsAsync(payments);

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(parameters);

            _creditPaymentUsesCaseMock.Setup(mock => mock.CreatePaymentHistory(It.IsAny<List<Credit>>(),
                    It.IsAny<List<RequestCancelPayment>>(), It.IsAny<AppParameters>()))
                .Returns(paymentHistoryResponses);

            _requestCancelPaymentRepositoryMock.Setup(mock => mock.GetByNotStatusAsync(It.IsAny<List<Guid>>(),
                    It.IsAny<RequestStatuses>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(requestCancelPaymentsCanceled);

            List<PaymentHistoryResponse> paymentHistoryResponsesResult = await CreditPaymentService.GetCustomerPaymentHistoryAsync(storeId, documentType, idDocument);

            Assert.NotNull(paymentHistoryResponsesResult);
            Assert.NotEmpty(paymentHistoryResponsesResult);
        }

        [Fact]
        public async Task GetCustomerPaymentHistory_NoPayments_ReturnsBusinessException()
        {
            string storeId = "TestStoreId";
            string documentType = "CC";
            string idDocument = "TestIdDocument";

            Customer customer = CustomerHelperTest.GetCustomer();
            Store store = StoreHelperTest.GetStore();
            List<Credit> payments = new List<Credit>();

            List<PaymentHistoryResponse> paymentHistoryResponses = PaymentHistoryHelperTest.GetResponsesFromPayments(payments);
            List<RequestCancelPayment> requestCancelPaymentsCanceled = RequestCancelPaymentHelperTest.GetRequestCancelPaymentList();

            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            _customerServiceMock.Setup(mock => mock.GetAsync(idDocument, documentType,
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(customer);

            _storeServiceMock.Setup(mock => mock.GetAsync(storeId, It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(store);

            _creditMasterRepositoryMock.Setup(mock => mock.GetCustomerPaymentHistoryAsync(It.IsAny<Customer>(),
                    It.IsAny<Store>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<int>()))
                .ReturnsAsync(payments);

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(parameters);

            _creditPaymentUsesCaseMock.Setup(mock => mock.CreatePaymentHistory(It.IsAny<List<Credit>>(),
                    It.IsAny<List<RequestCancelPayment>>(), It.IsAny<AppParameters>()))
                .Returns(paymentHistoryResponses);

            _requestCancelPaymentRepositoryMock.Setup(mock => mock.GetByNotStatusAsync(It.IsAny<List<Guid>>(),
                    It.IsAny<RequestStatuses>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(requestCancelPaymentsCanceled);

            BusinessException businessException = await Assert.ThrowsAsync<BusinessException>(() =>
                CreditPaymentService.GetCustomerPaymentHistoryAsync(storeId, documentType, idDocument));

            Assert.Equal((int)BusinessResponse.PaymentsNotFound, businessException.code);
        }
    }
}