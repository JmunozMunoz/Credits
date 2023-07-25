using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Moq;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Credits;
using Sc.Credits.Domain.Managment.Services.Customers;
using Sc.Credits.Domain.Managment.Services.Stores;
using Sc.Credits.Domain.Managment.Tests.Entities;
using Sc.Credits.Domain.Managment.Tests.Entities.Common;
using Sc.Credits.Domain.Managment.Tests.Entities.Credits;
using Sc.Credits.Domain.Model.Call.Gateway;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Customers.Gateway;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.Model.ReportTemplates.Gateway;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Domain.UseCase.Credits;
using Sc.Credits.Helper.Test.Model;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace Sc.Credits.Domain.Managment.Tests.Credits
{
    public class CreditsServiceTest
    {
        private readonly Mock<ICreditUsesCase> _creditUsesCaseMock = new Mock<ICreditUsesCase>();
        private readonly Mock<ICreditMasterRepository> _creditMasterRepositoryMock = new Mock<ICreditMasterRepository>();
        private readonly Mock<ICreditPaymentService> _creditPaymentServiceMock = new Mock<ICreditPaymentService>();
        private readonly Mock<ICreditCommonsService> _creditCommonsServiceMock = new Mock<ICreditCommonsService>();
        private readonly Mock<IAppParametersService> _appParametersServiceMock = new Mock<IAppParametersService>();
        private readonly Mock<IRequestCancelCreditRepository> _requestCancelCreditRepositoryMock = new Mock<IRequestCancelCreditRepository>();
        private readonly Mock<ISignatureService> _signatureServiceMock = new Mock<ISignatureService>();
        private readonly Mock<ICommons> _commonsMock = new Mock<ICommons>();
        private readonly Mock<IStorageService> _storageServiceMock = new Mock<IStorageService>();
        private readonly Mock<INotificationService> _notificationServiceMock = new Mock<INotificationService>();
        private readonly Mock<ITemplatesService> _templatesServiceMock = new Mock<ITemplatesService>();
        private readonly Mock<ICustomerService> _customerServiceMock = new Mock<ICustomerService>();
        private readonly Mock<IStoreService> _storeServiceMock = new Mock<IStoreService>();
        private readonly Mock<IUnapprovedCreditRepository> _unapprovedCreditRepositoryMock = new Mock<IUnapprovedCreditRepository>();
        private readonly Mock<IReportTemplatesGateway> _reportTemplatesGatewayMock = new Mock<IReportTemplatesGateway>();
        private readonly Mock<ILoggerService<CreditService>> _loggerServiceMock = new Mock<ILoggerService<CreditService>>();
        private readonly Mock<IRiskLevelRepository> _riskLevelRepository = new Mock<IRiskLevelRepository>();
        private readonly Mock<ICreditRequestAgentAnalysisService> _creditRequestAgentAnalysisService = new Mock<ICreditRequestAgentAnalysisService>();
        private readonly Mock<IUdpCallHttpRepository> _udpCallHttpRepository = new Mock<IUdpCallHttpRepository>();
        private readonly Mock<WebClient> _webClient;

        private readonly Mock<ICreditRequestAgentAnalysisRepository> _creditRequestAgentAnalysisRepository = new Mock<ICreditRequestAgentAnalysisRepository>();

        private CreditCommons CreditCommons =>
            CreditCommonsHelperTest.Create(_creditMasterRepositoryMock,
                _requestCancelCreditRepositoryMock,
                new Mock<IRequestCancelPaymentRepository>(),
                _creditUsesCaseMock,
                new Mock<ICreditPaymentUsesCase>(),
                new Mock<ICancelUseCase>(),
                _creditCommonsServiceMock);

        private ICreditService CreditService =>
            new CreditService(CreditCommons,
                _creditPaymentServiceMock.Object,
                _signatureServiceMock.Object,
                _unapprovedCreditRepositoryMock.Object,
                _reportTemplatesGatewayMock.Object,
                _loggerServiceMock.Object,
                _riskLevelRepository.Object,
                _creditRequestAgentAnalysisRepository.Object,
                _creditRequestAgentAnalysisService.Object,
                _udpCallHttpRepository.Object);

        public CreditsServiceTest()
        {
            _appParametersServiceMock.Setup(mock => mock.GetSettings())
                .Returns(new CredinetAppSettings
                {
                    PromissoryNotePath = "promissorynotes",
                    PdfBlobContainerName = "pdf",
                    ValidateTokenOnCreate = true,
                    RefinancingSourcesAllowed = "5",
                    SimulatedClientIdDocument = "0",
                    CustomerRiskLevelNumber = "1",
                });

            _webClient = new Mock<WebClient>();

            _commonsMock.SetupGet(mock => mock.AppParameters)
                .Returns(_appParametersServiceMock.Object);

            _creditCommonsServiceMock.SetupGet(mock => mock.Commons)
                .Returns(_commonsMock.Object);

            _creditCommonsServiceMock.SetupGet(mock => mock.CustomerService)
                .Returns(_customerServiceMock.Object);

            _creditCommonsServiceMock.SetupGet(mock => mock.StoreService)
                .Returns(_storeServiceMock.Object);

            _commonsMock.SetupGet(mock => mock.Storage)
                .Returns(_storageServiceMock.Object);

            _commonsMock.SetupGet(mock => mock.Notification)
                .Returns(_notificationServiceMock.Object);

            _commonsMock.SetupGet(mock => mock.Templates)
                .Returns(_templatesServiceMock.Object);

            _commonsMock.SetupGet(mock => mock.CredinetAppSettings)
                .Returns(_appParametersServiceMock.Object.GetSettings());
        }

        [Fact]
        public async Task ShouldGetTimeLimitInMonths()
        {
            Customer customer = CustomerHelperTest.GetCustomer();
            Store store = StoreHelperTest.GetStore();

            decimal creditValue = 50000M;
            int timeLimitsMonthsExpected = 1;

            _customerServiceMock.Setup(cr => cr.GetActiveAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(customer);

            _storeServiceMock.Setup(ce => ce.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(store);

            _creditMasterRepositoryMock.Setup(mock => mock.ValidateACreditPaidAccordingToTime(It.IsAny<Guid>(),
                It.IsAny<int>(), It.IsAny<Statuses>())).ReturnsAsync(true);

            _creditUsesCaseMock.Setup(ce => ce.GetTimeLimitInMonths(It.IsAny<Customer>(), It.IsAny<decimal>(), It.IsAny<Store>(), It.IsAny<int>(), It.IsAny<decimal>()))
                .Returns(timeLimitsMonthsExpected);

            _appParametersServiceMock.Setup(cr => cr.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            Assert.Equal(timeLimitsMonthsExpected, await CreditService.GetTimeLimitInMonthsAsync("1033340573", "CE", creditValue, "5d0d0e361477572ee0326f97"));
        }

        [Fact]
        public async Task ShouldGetTimeLimitInMonthsForSimulatedCustomer()
        {
            Customer customer = CustomerHelperTest.GetCustomer(idDocument: "0");
            Store store = StoreHelperTest.GetStore();

            decimal creditValue = 50000M;
            int timeLimitsMonthsExpected = 1;

            _customerServiceMock.Setup(cr => cr.GetActiveAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(customer);

            _storeServiceMock.Setup(ce => ce.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(store);

            _creditMasterRepositoryMock.Setup(mock => mock.ValidateACreditPaidAccordingToTime(It.IsAny<Guid>(),
                It.IsAny<int>(), It.IsAny<Statuses>())).ReturnsAsync(true);

            _creditUsesCaseMock.Setup(ce => ce.GetTimeLimitInMonths(It.IsAny<Customer>(), It.IsAny<decimal>(), It.IsAny<Store>(), It.IsAny<int>(), It.IsAny<decimal>()))
                .Returns(timeLimitsMonthsExpected);

            _appParametersServiceMock.Setup(cr => cr.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            Assert.Equal(timeLimitsMonthsExpected, await CreditService.GetTimeLimitInMonthsAsync("0", "CE", creditValue, "5d0d0e361477572ee0326f97"));
        }

        [Fact]
        public async Task ShouldGetCreditDetails()
        {
            Customer customer = CustomerHelperTest.GetCustomer();
            CreditDetailResponse creditDetail = CreditHelperTest.GetCreditDetails();
            Store store = StoreHelperTest.GetStore();
            int frequency = (int)Frequencies.Monthly;
            int months = 4;
            int limitInMoths = 4;
            decimal creditValue = 250000;

            RequiredInitialValuesForCreditDetail RequiredValues = new RequiredInitialValuesForCreditDetail()
            {
                creditValue = creditValue,
                months = months,
                storeId = "5d0d0e361477572ee0326f97",
                frequency = frequency,
                idDocument = "1033340573",
                typeDocument = "CE"
            };

            _customerServiceMock.Setup(cr => cr.GetAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(customer);

            _creditUsesCaseMock.Setup(ce => ce.GetCreditDetails(It.IsAny<CreditDetailDomainRequest>()))
                .Returns(creditDetail);

            _storeServiceMock.Setup(ce => ce.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(store);

            _appParametersServiceMock.Setup(cr => cr.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditUsesCaseMock.Setup(item => item.GetTimeLimitInMonths(customer, creditValue, store, 2, 300000))
                .Returns(limitInMoths);

            _creditMasterRepositoryMock.Setup(mock => mock.ValidateACreditPaidAccordingToTime(It.IsAny<Guid>(),
                    It.IsAny<int>(), It.IsAny<Statuses>()))
                .ReturnsAsync(true);

            CreditDetailResponse result = await CreditService.GetCreditDetailsAsync(RequiredValues);

            Assert.IsType<CreditDetailResponse>(result);
        }

        [Fact]
        public async Task ShouldGetCustomerCreditLimit()
        {
            Customer customer = CustomerHelperTest.GetCustomerCreditLimit();
            CustomerCreditLimitResponse customerCreditLimitResponse = CreditHelperTest.GetCustomerCreditLimit();

            _customerServiceMock.Setup(cr => cr.GetAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(customer);

            _appParametersServiceMock.Setup(cr => cr.GetAppParametersAsync())
              .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditUsesCaseMock.Setup(x => x.GetCustomerCreditLimitAsync(It.IsAny<Customer>(), It.IsAny<decimal>(), It.IsAny<int>(),
                It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<string>()))
                .ReturnsAsync(customerCreditLimitResponse);

            CustomerCreditLimitResponse customerCreditLimit = await CreditService.GetCustomerCreditLimitAsync(customer.DocumentType, customer.IdDocument, "14524");

            Assert.IsType<CustomerCreditLimitResponse>(customerCreditLimit);
            Assert.True(customerCreditLimitResponse.ValidatedMail);
            Assert.False(customerCreditLimitResponse.NewCreditButtonEnabled);
        }

        [Fact]
        public async Task ShouldGetCustomerCreditLimitCreditValue()
        {
            Customer customer = CustomerHelperTest.GetCustomerCreditLimit();
            CustomerCreditLimitResponse customerCreditLimitResponse = CreditHelperTest.GetCustomerCreditLimit();

            _customerServiceMock.Setup(cr => cr.GetAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(customer);

            _appParametersServiceMock.Setup(cr => cr.GetAppParametersAsync())
              .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditUsesCaseMock.Setup(x => x.GetCustomerCreditLimitAsync(It.IsAny<Customer>(), It.IsAny<decimal>(), It.IsAny<int>(),
                It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<string>()))
                .ReturnsAsync(customerCreditLimitResponse);

            CustomerCreditLimitResponse customerCreditLimit = await CreditService.GetCustomerCreditLimitAsync(customer.DocumentType, customer.IdDocument, "14524",
                200000);

            Assert.IsType<CustomerCreditLimitResponse>(customerCreditLimit);
            Assert.True(customerCreditLimitResponse.ValidatedMail);
            Assert.False(customerCreditLimitResponse.NewCreditButtonEnabled);
        }

        [Fact]
        public async Task ShouldCreateCreditThrowsBusinessExcepcionUnauthorizedStore_NoCredits()
        {
            _appParametersServiceMock.Setup(mock => mock.GetTransactionTypeAsync(It.IsAny<int>()))
                .ReturnsAsync(TransactionTypeHelperTest.GetCreateCreditType());

            _appParametersServiceMock.Setup(mock => mock.GetStatusAsync(It.IsAny<int>()))
                .ReturnsAsync(StatusHelperTest.GetActiveStatus());

            _storeServiceMock.Setup(ce => ce.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(StoreHelperTest.GetStoreWithStatus((int)StoreStatuses.NoCredits));

            _customerServiceMock.Setup(cr => cr.GetActiveAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(CustomerHelperTest.GetCustomer());

            _appParametersServiceMock.Setup(mock => mock.GetSourceAsync(It.IsAny<int>()))
                .ReturnsAsync(SourceHelperTest.GetCredinetSource());

            _appParametersServiceMock.Setup(mock => mock.GetAuthMethodAsync(It.IsAny<int>()))
                .ReturnsAsync(AuthMethodHelperTest.GetTokenAuthMethod());

            _appParametersServiceMock.Setup(mock => mock.GetPaymentTypeAsync(It.IsAny<int>()))
                .ReturnsAsync(PaymentTypeHelperTest.GetOrdinaryPaymentType());

            _appParametersServiceMock.Setup(cr => cr.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditUsesCaseMock.Setup(mock => mock.CustomerIsDefaulterAsync(It.IsAny<Customer>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(false);

            BusinessException exception = await Assert.ThrowsAsync<BusinessException>(() =>
                CreditService.CreateAsync(CreditHelperTest.GetCreateCreditRequest()));

            Assert.Equal((int)BusinessResponse.CreateCreditUnauthorized, exception.code);
        }

        [Fact]
        public async Task ShouldCreateCreditThrowsBusinessExcepcionUnauthorizedStore_Closed()
        {
            _appParametersServiceMock.Setup(mock => mock.GetTransactionTypeAsync(It.IsAny<int>()))
                .ReturnsAsync(TransactionTypeHelperTest.GetCreateCreditType());

            _appParametersServiceMock.Setup(mock => mock.GetStatusAsync(It.IsAny<int>()))
                .ReturnsAsync(StatusHelperTest.GetActiveStatus());

            _storeServiceMock.Setup(ce => ce.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(StoreHelperTest.GetStoreWithStatus((int)StoreStatuses.Closed));

            _customerServiceMock.Setup(cr => cr.GetActiveAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(CustomerHelperTest.GetCustomer());

            _appParametersServiceMock.Setup(mock => mock.GetSourceAsync(It.IsAny<int>()))
                .ReturnsAsync(SourceHelperTest.GetCredinetSource());

            _appParametersServiceMock.Setup(mock => mock.GetAuthMethodAsync(It.IsAny<int>()))
                .ReturnsAsync(AuthMethodHelperTest.GetTokenAuthMethod());

            _appParametersServiceMock.Setup(mock => mock.GetPaymentTypeAsync(It.IsAny<int>()))
                .ReturnsAsync(PaymentTypeHelperTest.GetOrdinaryPaymentType());

            _appParametersServiceMock.Setup(cr => cr.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditUsesCaseMock.Setup(mock => mock.CustomerIsDefaulterAsync(It.IsAny<Customer>(), It.IsAny<string>(),
                    It.IsAny<int>()))
                .ReturnsAsync(false);

            BusinessException exception = await Assert.ThrowsAsync<BusinessException>(() =>
                CreditService.CreateAsync(CreditHelperTest.GetCreateCreditRequest()));

            Assert.Equal((int)BusinessResponse.CreateCreditUnauthorized, exception.code);
        }

        [Fact]
        public async Task ShouldNotifyCreateCreditAndUploadPromissoryNote()
        {
            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMaster();

            _creditMasterRepositoryMock.Setup(mock => mock.GetWithCurrentAsync(It.IsAny<Guid>(), It.IsAny<Customer>(),
                    It.IsAny<Store>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(creditMaster);

            _appParametersServiceMock.Setup(cr => cr.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _reportTemplatesGatewayMock.Setup(mock => mock.GenerateAsync(It.IsAny<PromissoryNoteRequest>(),
                                                                         It.IsAny<string>(),
                                                                         It.IsAny<string>()))
                                                                         .ReturnsAsync("https://www.sistecredito.com/");

            await CreditService.CreditCreationNotifyAsync(CreditHelperTest.GetCreateCreditResponse());

            _creditMasterRepositoryMock.Verify(mock => mock.UpdateAsync(It.IsAny<CreditMaster>(),
                It.IsAny<IEnumerable<Field>>(), It.IsAny<Transaction>()), Times.Once);

            _creditCommonsServiceMock.Verify(mock => mock.Commons.Storage.UploadFileAsync(It.IsAny<byte[]>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>()),
                Times.Once());

            _creditCommonsServiceMock.Verify(mock => mock.Commons.Notification.SendMailAsync(It.IsAny<MailNotificationRequest>(),
                    It.IsAny<string>(), It.IsAny<bool>()),
                Times.Once());
        }

        [Fact]
        public async Task ShouldSignPromissoryNote()
        {
            string authority = "Autentic";
            string id = "TestTransactionId";

            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMasterWithStoreAllowPromissoryNoteSignature();

            _customerServiceMock.Setup(cr => cr.GetActiveAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(CustomerHelperTest.GetCustomer());

            _storeServiceMock.Setup(ce => ce.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(StoreHelperTest.GetStore());

            _signatureServiceMock.Setup(mock => mock.SignAsync(It.IsAny<CreditMaster>(), It.IsAny<string>(),
                It.IsAny<byte[]>()))
            .ReturnsAsync((authority, id));

            _reportTemplatesGatewayMock.Setup(mock => mock.GenerateAsync(It.IsAny<PromissoryNoteRequest>(),
                                                                        It.IsAny<string>(),
                                                                        It.IsAny<string>()))
                                                                        .ReturnsAsync("https://www.sistecredito.com/");

            _creditMasterRepositoryMock.Setup(mock => mock.GetWithCurrentAsync(It.IsAny<Guid>(), It.IsAny<Customer>(),
                    It.IsAny<Store>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(creditMaster);

            _appParametersServiceMock.Setup(cr => cr.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _appParametersServiceMock.Setup(mock => mock.GetTransactionTypeAsync(It.IsAny<int>()))
               .ReturnsAsync(TransactionTypeHelperTest.GetUpdateCreditType());

            await CreditService.CreditCreationNotifyAsync(CreditHelperTest.GetCreateCreditResponse());

            _creditMasterRepositoryMock.Verify(mock => mock.UpdateAsync(It.IsAny<CreditMaster>(),
                It.IsAny<IEnumerable<Field>>(), It.IsAny<Transaction>()), Times.Once);

            _creditCommonsServiceMock.Verify(mock => mock.Commons.Storage.UploadFileAsync(It.IsAny<byte[]>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>()),
                Times.Once);

            Assert.Equal(authority, creditMaster.GetCertifyingAuthority);
            Assert.Equal(id, creditMaster.GetCertifiedId);
        }

        [Fact]
        public async Task NotShouldSignPromissoryNote()
        {
            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMasterWithStoreAllowPromissoryNoteSignature();

            _customerServiceMock.Setup(cr => cr.GetActiveAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(CustomerHelperTest.GetCustomer());

            _storeServiceMock.Setup(ce => ce.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(StoreHelperTest.GetStore());

            _signatureServiceMock.Setup(mock => mock.SignAsync(It.IsAny<CreditMaster>(), It.IsAny<string>(),
                    It.IsAny<byte[]>()))
                .ReturnsAsync((null, null));

            _creditMasterRepositoryMock.Setup(mock => mock.GetWithCurrentAsync(It.IsAny<Guid>(), It.IsAny<Customer>(),
                    It.IsAny<Store>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(creditMaster);

            _appParametersServiceMock.Setup(cr => cr.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _reportTemplatesGatewayMock.Setup(mock => mock.GenerateAsync(It.IsAny<PromissoryNoteRequest>(),
                                                                        It.IsAny<string>(),
                                                                        It.IsAny<string>()))
                                                                        .ReturnsAsync("Http://Pruebas.com");

            _appParametersServiceMock.Setup(mock => mock.GetTransactionTypeAsync(It.IsAny<int>()))
               .ReturnsAsync(TransactionTypeHelperTest.GetUpdateCreditType());

            await CreditService.CreditCreationNotifyAsync(CreditHelperTest.GetCreateCreditResponse());

            _creditMasterRepositoryMock.Verify(mock => mock.UpdateAsync(It.IsAny<CreditMaster>(),
                It.IsAny<IEnumerable<Field>>(), It.IsAny<Transaction>()), Times.Once);

            _creditCommonsServiceMock.Verify(mock => mock.Commons.Storage.UploadFileAsync(It.IsAny<byte[]>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>()),
                Times.Once());

            Assert.Null(creditMaster.GetCertifyingAuthority);
            Assert.Null(creditMaster.GetCertifiedId);
        }

        [Fact]
        public async Task ShouldCreateCredit()
        {
            CreateCreditTransactionResponse createCreditTransactionResponseCallback = null;

            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMaster();
            CreateCreditResponse createCreditResponse = CreditHelperTest.GetCreateCreditResponse();
            createCreditResponse.CreditMaster = creditMaster;

            PaymentCreditResponse paymentCreditResponse = PayCreditHelperTest.GetpaymentCreditResponse();

            CreateCreditTransactionResponse createCreditTransactionResponse = new CreateCreditTransactionResponse(
                createCreditResponse, creditMaster.Current, paymentCreditResponse);

            _appParametersServiceMock.Setup(mock => mock.GetTransactionTypeAsync(It.IsAny<int>()))
                .ReturnsAsync(TransactionTypeHelperTest.GetCreateCreditType());

            _appParametersServiceMock.Setup(mock => mock.GetStatusAsync(It.IsAny<int>()))
                .ReturnsAsync(StatusHelperTest.GetActiveStatus());

            _storeServiceMock.Setup(ce => ce.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(StoreHelperTest.GetStore());

            _customerServiceMock.Setup(cr => cr.GetActiveAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(CustomerHelperTest.GetCustomer());

            _appParametersServiceMock.Setup(mock => mock.GetSourceAsync(It.IsAny<int>()))
                .ReturnsAsync(SourceHelperTest.GetCredinetSource());

            _appParametersServiceMock.Setup(mock => mock.GetAuthMethodAsync(It.IsAny<int>()))
                .ReturnsAsync(AuthMethodHelperTest.GetTokenAuthMethod());

            _appParametersServiceMock.Setup(mock => mock.GetPaymentTypeAsync(It.IsAny<int>()))
                .ReturnsAsync(PaymentTypeHelperTest.GetOrdinaryPaymentType());

            _creditMasterRepositoryMock.Setup(mock =>
                   mock.ExcecuteOnTransactionAsync(It.IsAny<Func<Transaction, Task<CreateCreditTransactionResponse>>>()))
                       .Callback<Func<Transaction, Task<CreateCreditTransactionResponse>>>(async handle =>
                       {
                           createCreditTransactionResponseCallback = await handle.Invoke(Transaction.Current);
                       })
                       .ReturnsAsync(createCreditTransactionResponse);

            _creditUsesCaseMock.Setup(mock => mock.CreateAsync(It.IsAny<CreateCreditDomainRequest>(), It.IsAny<Transaction>(), It.IsAny<bool>()))
                .ReturnsAsync(createCreditResponse);

            _appParametersServiceMock.Setup(cr => cr.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditPaymentServiceMock.Setup(item => item.DownPaymentAsync(It.IsAny<PaymentCreditRequestComplete>(), It.IsAny<CreditMaster>(),
                    It.IsAny<AppParameters>(), It.IsAny<Transaction>()))
                .ReturnsAsync(paymentCreditResponse);

            _creditUsesCaseMock.Setup(mock => mock.CustomerIsDefaulterAsync(It.IsAny<Customer>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(false);

            CreateCreditResponse createCreditResponseResult = await CreditService.CreateAsync(CreditHelperTest.GetCreateCreditRequest());

            _creditPaymentServiceMock.Verify(mock => mock.DownPaymentAsync(It.IsAny<PaymentCreditRequestComplete>(), It.IsAny<CreditMaster>(),
                    It.IsAny<AppParameters>(), It.IsAny<Transaction>()),
                Times.Once());

            Assert.NotNull(createCreditResponseResult);
            Assert.NotNull(createCreditResponseResult.CreditMaster);
            Assert.False(createCreditTransactionResponseCallback.CreateCreditResponse.AlternatePayment);
            Assert.Equal(createCreditResponseResult, createCreditTransactionResponseCallback.CreateCreditResponse);
        }

        [Fact]
        public async Task ShouldCreateCreditSendEventNotification()
        {
            CreateCreditTransactionResponse createCreditTransactionResponseCallback = null;

            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMaster();
            CreateCreditResponse createCreditResponse = CreditHelperTest.GetCreateCreditResponse();
            createCreditResponse.CreditMaster = creditMaster;

            PaymentCreditResponse paymentCreditResponse = PayCreditHelperTest.GetpaymentCreditResponse();

            CreateCreditTransactionResponse createCreditTransactionResponse = new CreateCreditTransactionResponse(
                createCreditResponse, creditMaster.Current, paymentCreditResponse);

            _appParametersServiceMock.Setup(mock => mock.GetTransactionTypeAsync(It.IsAny<int>()))
                .ReturnsAsync(TransactionTypeHelperTest.GetCreateCreditType());

            _appParametersServiceMock.Setup(mock => mock.GetStatusAsync(It.IsAny<int>()))
                .ReturnsAsync(StatusHelperTest.GetActiveStatus());

            _storeServiceMock.Setup(ce => ce.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(StoreHelperTest.GetStore());

            _customerServiceMock.Setup(cr => cr.GetActiveAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(CustomerHelperTest.GetCustomer());

            _appParametersServiceMock.Setup(mock => mock.GetSourceAsync(It.IsAny<int>()))
                .ReturnsAsync(SourceHelperTest.GetCredinetSource());

            _appParametersServiceMock.Setup(mock => mock.GetAuthMethodAsync(It.IsAny<int>()))
                .ReturnsAsync(AuthMethodHelperTest.GetTokenAuthMethod());

            _appParametersServiceMock.Setup(mock => mock.GetPaymentTypeAsync(It.IsAny<int>()))
                .ReturnsAsync(PaymentTypeHelperTest.GetOrdinaryPaymentType());

            _appParametersServiceMock.Setup(cr => cr.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditMasterRepositoryMock.Setup(mock =>
                   mock.ExcecuteOnTransactionAsync(It.IsAny<Func<Transaction, Task<CreateCreditTransactionResponse>>>()))
                       .Callback<Func<Transaction, Task<CreateCreditTransactionResponse>>>(async handle =>
                       {
                           createCreditTransactionResponseCallback = await handle.Invoke(Transaction.Current);
                       })
                       .ReturnsAsync(createCreditTransactionResponse);

            _creditUsesCaseMock.Setup(mock => mock.CreateAsync(It.IsAny<CreateCreditDomainRequest>(), It.IsAny<Transaction>(), It.IsAny<bool>()))
                .ReturnsAsync(createCreditResponse);

            _creditPaymentServiceMock.Setup(item => item.DownPaymentAsync(It.IsAny<PaymentCreditRequestComplete>(), It.IsAny<CreditMaster>(),
                    It.IsAny<AppParameters>(), It.IsAny<Transaction>()))
                .ReturnsAsync(PayCreditHelperTest.GetpaymentCreditResponse());

            _creditUsesCaseMock.Setup(mock => mock.CustomerIsDefaulterAsync(It.IsAny<Customer>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(false);

            await CreditService.CreateAsync(CreditHelperTest.GetCreateCreditRequest());

            _creditCommonsServiceMock.Verify(mock => mock.SendEventAsync(It.IsAny<CreditMaster>(), It.IsAny<Customer>(), It.IsAny<Store>(),
                It.IsAny<Credit>(), It.IsAny<TransactionType>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldCreateCreditWithCustomerNotActiveShouldGenerateBussinessExceptionCustomerNotActive()
        {
            _appParametersServiceMock.Setup(mock => mock.GetTransactionTypeAsync(It.IsAny<int>()))
                .ReturnsAsync(TransactionTypeHelperTest.GetCreateCreditType());

            _appParametersServiceMock.Setup(mock => mock.GetStatusAsync(It.IsAny<int>()))
                .ReturnsAsync(StatusHelperTest.GetActiveStatus());

            _customerServiceMock.Setup(cr => cr.GetActiveAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ThrowsAsync(new BusinessException(nameof(BusinessResponse.CustomerNotActive), (int)BusinessResponse.CustomerNotActive));

            _storeServiceMock.Setup(ce => ce.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(StoreHelperTest.GetStore());

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditUsesCaseMock.Setup(mock => mock.CustomerIsDefaulterAsync(It.IsAny<Customer>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(false);

            BusinessException exception = await Assert.ThrowsAsync<BusinessException>(() => CreditService.CreateAsync(CreditHelperTest.GetCreateCreditRequest()));

            Assert.Equal((int)BusinessResponse.CustomerNotActive, exception.code);
        }

        [Fact]
        public async Task ShouldCreateCreditWithCustomerDefaulterThrowsBusinessException()
        {
            _customerServiceMock.Setup(cr => cr.GetActiveAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(CustomerHelperTest.GetCustomerByStatus(CustomerStatuses.Approved));

            _storeServiceMock.Setup(ce => ce.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(StoreHelperTest.GetStore());

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditMasterRepositoryMock.Setup(mock => mock.IsDuplicatedAsync(It.IsAny<string>(),
                    It.IsAny<DateTime>(), It.IsAny<Guid>()))
                .ReturnsAsync(false);

            _creditUsesCaseMock.Setup(mock => mock.CustomerIsDefaulterAsync(It.IsAny<Customer>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(true);

            CreateCreditRequest createCreditRequest = CreditHelperTest.GetCreateCreditRequest();
            createCreditRequest.Source = (int)Sources.PaymentGateways;

            BusinessException exception = await Assert.ThrowsAsync<BusinessException>(() => CreditService.CreateAsync(createCreditRequest));

            Assert.Equal((int)BusinessResponse.CustomerIsDefaulter, exception.code);
        }

        [Fact]
        public async Task ShouldCreateCreditWithSourcePaymentGateway()
        {
            CreateCreditTransactionResponse createCreditTransactionResponseCallback = null;

            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMaster();
            CreateCreditResponse createCreditResponse = CreditHelperTest.GetCreateCreditResponseWithAlternatePaymentTrue();
            createCreditResponse.CreditMaster = creditMaster;

            PaymentCreditResponse paymentCreditResponse = PayCreditHelperTest.GetpaymentCreditResponse();

            CreateCreditTransactionResponse createCreditTransactionResponse = new CreateCreditTransactionResponse(
                createCreditResponse, creditMaster.Current, paymentCreditResponse);

            _appParametersServiceMock.Setup(mock => mock.GetTransactionTypeAsync(It.IsAny<int>()))
                .ReturnsAsync(TransactionTypeHelperTest.GetCreateCreditType());

            _appParametersServiceMock.Setup(mock => mock.GetStatusAsync(It.IsAny<int>()))
                .ReturnsAsync(StatusHelperTest.GetActiveStatus());

            _storeServiceMock.Setup(ce => ce.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(StoreHelperTest.GetStore());

            _customerServiceMock.Setup(cr => cr.GetActiveAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(CustomerHelperTest.GetCustomer());

            _appParametersServiceMock.Setup(mock => mock.GetSourceAsync(It.IsAny<int>()))
                .ReturnsAsync(SourceHelperTest.GetCredinetSource());

            _appParametersServiceMock.Setup(mock => mock.GetAuthMethodAsync(It.IsAny<int>()))
                .ReturnsAsync(AuthMethodHelperTest.GetTokenAuthMethod());

            _appParametersServiceMock.Setup(mock => mock.GetPaymentTypeAsync(It.IsAny<int>()))
                .ReturnsAsync(PaymentTypeHelperTest.GetOrdinaryPaymentType());

            _creditMasterRepositoryMock.Setup(mock =>
                   mock.ExcecuteOnTransactionAsync(It.IsAny<Func<Transaction, Task<CreateCreditTransactionResponse>>>()))
                       .Callback<Func<Transaction, Task<CreateCreditTransactionResponse>>>(async handle =>
                       {
                           createCreditTransactionResponseCallback = await handle.Invoke(Transaction.Current);
                       })
                       .ReturnsAsync(createCreditTransactionResponse);

            _creditUsesCaseMock.Setup(mock => mock.CreateAsync(It.IsAny<CreateCreditDomainRequest>(), It.IsAny<Transaction>(), It.IsAny<bool>()))
                .ReturnsAsync(createCreditResponse);

            _appParametersServiceMock.Setup(cr => cr.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditPaymentServiceMock.Setup(item => item.DownPaymentAsync(It.IsAny<PaymentCreditRequestComplete>(),
                    It.IsAny<CreditMaster>(), It.IsAny<AppParameters>(), It.IsAny<Transaction>()))
                .ReturnsAsync(paymentCreditResponse);

            _creditUsesCaseMock.Setup(mock => mock.CustomerIsDefaulterAsync(It.IsAny<Customer>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(false);

            CreateCreditResponse createCreditResponseResult = await CreditService.CreateAsync(CreditHelperTest.GetCreateCreditRequestSourcePaymentGateways());

            _creditPaymentServiceMock.Verify(mock => mock.DownPaymentAsync(It.IsAny<PaymentCreditRequestComplete>(), It.IsAny<CreditMaster>(), It.IsAny<AppParameters>(),
                    It.IsAny<Transaction>()),
                Times.Once());

            Assert.NotNull(createCreditResponseResult);
            Assert.NotNull(createCreditResponseResult.CreditMaster);
            Assert.True(createCreditTransactionResponseCallback.CreateCreditResponse.AlternatePayment);
            Assert.Equal(createCreditResponseResult, createCreditTransactionResponseCallback.CreateCreditResponse);
        }

        [Fact]
        public async Task ShouldUpdateExtraFields()
        {
            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMasterExtraFields();
            UpdateCreditExtraFieldsRequest updateCreditExtraFieldsRequest = CreditMasterHelperTest.GetUpdateCreditExtraFields();
            TransactionType transactionType = TransactionTypeHelperTest.GetTransactionType(TransactionTypes.UpdateCredit);

            _creditMasterRepositoryMock.Setup(mock => mock.GetWithCurrentAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(creditMaster);

            _customerServiceMock.Setup(cr => cr.GetAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(creditMaster.Customer);

            _appParametersServiceMock.Setup(cr => cr.GetTransactionTypeAsync(It.IsAny<int>()))
             .ReturnsAsync(transactionType);

            await CreditService.UpdateExtraFieldsAsync(updateCreditExtraFieldsRequest);

            _creditMasterRepositoryMock.Verify(cm => cm.UpdateAsync(It.IsAny<CreditMaster>(), It.IsAny<IEnumerable<Field>>(),
                It.IsAny<Transaction>()), Times.Once());

            _creditCommonsServiceMock.Verify(cm => cm.SendEventAsync(It.IsAny<CreditMaster>(), It.IsAny<Customer>(), It.IsAny<Store>(),
                It.IsAny<Credit>(), It.IsAny<TransactionType>(), It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public async Task ShouldGenerateTokenAndNotifySms()
        {
            CredinetAppSettings credinetAppSettings = CredinetAppSettingsHelperTest.GetCredinetAppSettings();
            _customerServiceMock.Setup(cr => cr.GetAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(CustomerHelperTest.GetCustomer());

            _creditCommonsServiceMock.Setup(mock => mock.GenerateTokenAsync(It.IsAny<CreditTokenRequest>()))
                .ReturnsAsync(new TokenResponse
                {
                    Token = new Token
                    {
                        Value = "864597",
                        RemainingSeconds = 180
                    }
                });

            _storeServiceMock.Setup(ce => ce.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(StoreHelperTest.GetStoreWithDefaultValues(credinetAppSettings));

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            await CreditService.GenerateTokenAsync(GenerateTokenHelperTest.GetDefaultResquest());

            _creditUsesCaseMock.Verify(mock => mock.GetTokenSmsNotification(It.IsAny<string>(),
                    It.IsAny<Customer>(), It.IsAny<Store>(), It.IsAny<CreditDetailResponse>(),
                    It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once);

            _creditCommonsServiceMock.Verify(mock => mock.Commons.Notification.SendSmsAsync(It.IsAny<SmsNotificationRequest>(),
                It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public async Task ShouldGenerateTokenWithoutNotifySms(bool storeSendTokenSms, bool customerSendTokenSms)
        {
            CredinetAppSettings credinetAppSettings = CredinetAppSettingsHelperTest.GetCredinetAppSettings();
            credinetAppSettings.StoreSendTokenSmsDefault = storeSendTokenSms;
            Customer customer = CustomerHelperTest.GetCustomerWithNotifications(customerSendTokenSms, sendTokenMail: true);

            _customerServiceMock.Setup(cr => cr.GetAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(customer);

            _creditCommonsServiceMock.Setup(mock => mock.GenerateTokenAsync(It.IsAny<CreditTokenRequest>()))
                .ReturnsAsync(new TokenResponse
                {
                    Token = new Token
                    {
                        Value = "864597",
                        RemainingSeconds = 180
                    }
                });

            _storeServiceMock.Setup(ce => ce.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(StoreHelperTest.GetStoreWithDefaultValues(credinetAppSettings));

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            await CreditService.GenerateTokenAsync(GenerateTokenHelperTest.GetDefaultResquest());

            _creditUsesCaseMock.Verify(mock => mock.GetTokenSmsNotification(It.IsAny<string>(),
                    It.IsAny<Customer>(), It.IsAny<Store>(), It.IsAny<CreditDetailResponse>(),
                    It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Never);

            _creditCommonsServiceMock.Verify(mock => mock.Commons.Notification.SendSmsAsync(It.IsAny<SmsNotificationRequest>(),
                It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public async Task ShouldGenerateTokenAndNotifyMail()
        {
            CredinetAppSettings credinetAppSettings = CredinetAppSettingsHelperTest.GetCredinetAppSettings();

            _customerServiceMock.Setup(cr => cr.GetAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(CustomerHelperTest.GetCustomer());

            _creditCommonsServiceMock.Setup(mock => mock.GenerateTokenAsync(It.IsAny<CreditTokenRequest>()))
                .ReturnsAsync(new TokenResponse
                {
                    Token = new Token
                    {
                        Value = "864597",
                        RemainingSeconds = 180
                    }
                });

            _storeServiceMock.Setup(ce => ce.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(StoreHelperTest.GetStoreWithDefaultValues(credinetAppSettings));

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            await CreditService.GenerateTokenAsync(GenerateTokenHelperTest.GetDefaultResquest());

            _creditUsesCaseMock.Verify(mock => mock.GetTokenMailNotification(It.IsAny<Customer>(), It.IsAny<Store>(),
                It.IsAny<CreditDetailResponse>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(),
                It.IsAny<int>()), Times.Once);

            _creditCommonsServiceMock.Verify(mock => mock.Commons.Notification.SendMailAsync(It.IsAny<MailNotificationRequest>(),
                It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public async Task ShouldGenerateTokenWithoutNotifyMail(bool storeSendTokenMail, bool customerSendTokenMail)
        {
            CredinetAppSettings credinetAppSettings = CredinetAppSettingsHelperTest.GetCredinetAppSettings();
            credinetAppSettings.StoreSendTokenMailDefault = storeSendTokenMail;
            Customer customer = CustomerHelperTest.GetCustomerWithNotifications(sendTokenSms: true, customerSendTokenMail);

            _customerServiceMock.Setup(cr => cr.GetAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(customer);

            _creditCommonsServiceMock.Setup(mock => mock.GenerateTokenAsync(It.IsAny<CreditTokenRequest>()))
                .ReturnsAsync(new TokenResponse
                {
                    Token = new Token
                    {
                        Value = "864597",
                        RemainingSeconds = 180
                    }
                });

            _storeServiceMock.Setup(ce => ce.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(StoreHelperTest.GetStoreWithDefaultValues(credinetAppSettings));

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            await CreditService.GenerateTokenAsync(GenerateTokenHelperTest.GetDefaultResquest());

            _creditUsesCaseMock.Verify(mock => mock.GetTokenMailNotification(It.IsAny<Customer>(), It.IsAny<Store>(),
                It.IsAny<CreditDetailResponse>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(),
                It.IsAny<int>()), Times.Never);

            _creditCommonsServiceMock.Verify(mock => mock.Commons.Notification.SendMailAsync(It.IsAny<MailNotificationRequest>(),
                It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public async Task ShouldCallRequestCreditToken()
        {
            _customerServiceMock.Setup(cr => cr.GetAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(CustomerHelperTest.GetCustomer());

            await CreditService.TokenCallRequestAsync(new CreditTokenCallRequest
            {
                IdDocument = "1037610106",
                TypeDocument = "CC"
            });

            _creditCommonsServiceMock.Verify(mock => mock.TokenCallRequestAsync(It.IsAny<CreditTokenCallRequest>()), Times.Once);
        }

        [Fact]
        public async Task ShouldGetPromissoryNoteInfo()
        {
            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMaster();

            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            _creditMasterRepositoryMock.Setup(mock => mock.GetWithCurrentAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>()))
               .ReturnsAsync(creditMaster);

            _customerServiceMock.Setup(cr => cr.GetAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(creditMaster.Customer);

            _storeServiceMock.Setup(mock => mock.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(creditMaster.Store);

            _storeServiceMock.Setup(mock => mock.GetAssuranceCompanyAsync(It.IsAny<long>()))
                .ReturnsAsync(new AssuranceCompany("Test"));

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(parameters);

            _creditUsesCaseMock.Setup(mock => mock.GetPromissoryNoteInfo(It.IsAny<CreditMaster>(), It.IsAny<AppParameters>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new PromissoryNoteInfo());

            PromissoryNoteInfo promissoryNoteInfo = await CreditService.GetPromissoryNoteInfoAsync(Guid.NewGuid(), false);

            Assert.NotNull(promissoryNoteInfo);
            Assert.IsType<PromissoryNoteInfo>(promissoryNoteInfo);
        }

        [Fact]
        public async Task ShouldGetOriginalAmortizationSchedule()
        {
            int frecuencyMonthly = 30;

            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            AmortizationScheduleResponse expectedAmortizationScheduleResponse = ModelHelperTest.InstanceModel<AmortizationScheduleResponse>();

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(parameters);

            _creditUsesCaseMock.Setup(mock => mock.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(), It.IsAny<int>()))
                .Returns(expectedAmortizationScheduleResponse);

            AmortizationScheduleRequest amortizationScheduleRequest = ModelHelperTest.InstanceModel<AmortizationScheduleRequest>();
            amortizationScheduleRequest.Frequency = frecuencyMonthly;

            AmortizationScheduleResponse amortizationScheduleResponse =
                await CreditService.GetOriginalAmortizationScheduleAsync(amortizationScheduleRequest);

            Assert.NotNull(amortizationScheduleResponse);
            Assert.Equal(expectedAmortizationScheduleResponse, amortizationScheduleResponse);
        }

        [Fact]
        public async Task ShouldUpdateChargesPaymentPlanValueNoHasCharges()
        {
            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMaster();

            _creditMasterRepositoryMock.Setup(mock => mock.GetWithCurrentAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(creditMaster);

            _customerServiceMock.Setup(cr => cr.GetAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(creditMaster.Customer);

            _appParametersServiceMock.Setup(mock => mock.GetTransactionTypeAsync(It.IsAny<int>()))
                .ReturnsAsync(TransactionTypeHelperTest.GetTransactionType(TransactionTypes.UpdateChargesPaymentPlan));

            await CreditService.UpdateChargesPaymentPlanValueAsync(Guid.NewGuid(), charges: 0, hasArrearsCharge: false,
                arrearsCharges: 0, updatedPaymentPlanValue: 45000);

            _creditMasterRepositoryMock.Verify(mock => mock.AddTransactionAsync(It.IsAny<CreditMaster>(), It.IsAny<IEnumerable<Field>>(),
                It.IsAny<Transaction>()), Times.Once);

            _creditCommonsServiceMock.Verify(mock => mock.SendEventAsync(It.IsAny<CreditMaster>(), It.IsAny<Customer>(), It.IsAny<Store>(),
                It.IsAny<Credit>(), It.IsAny<TransactionType>(), It.IsAny<string>()), Times.Once);

            Assert.Equal((int)TransactionTypes.UpdateChargesPaymentPlan, creditMaster.Current.CreditPayment.GetTransactionTypeId);
            Assert.Equal(0, creditMaster.Current.GetArrearsCharge);
            Assert.Equal(0, creditMaster.Current.GetChargeValue);
            Assert.False(creditMaster.Current.HasCharges());
            Assert.True(creditMaster.Current.HasUpdatedPaymentPlan());
            Assert.Equal(45000, creditMaster.Current.GetUpdatedPaymentPlanValue);
        }

        [Fact]
        public async Task ShouldUpdateChargesPaymentPlanValue()
        {
            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMaster();

            _creditMasterRepositoryMock.Setup(mock => mock.GetWithCurrentAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(creditMaster);

            _customerServiceMock.Setup(cr => cr.GetActiveAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(creditMaster.Customer);

            _appParametersServiceMock.Setup(mock => mock.GetTransactionTypeAsync(It.IsAny<int>()))
                .ReturnsAsync(TransactionTypeHelperTest.GetTransactionType(TransactionTypes.UpdateChargesPaymentPlan));

            await CreditService.UpdateChargesPaymentPlanValueAsync(Guid.NewGuid(), charges: 2000, hasArrearsCharge: true,
                arrearsCharges: 300, updatedPaymentPlanValue: 45000);

            _creditMasterRepositoryMock.Verify(mock => mock.AddTransactionAsync(It.IsAny<CreditMaster>(), It.IsAny<IEnumerable<Field>>(),
                It.IsAny<Transaction>()), Times.Once);

            _creditCommonsServiceMock.Verify(mock => mock.SendEventAsync(It.IsAny<CreditMaster>(), It.IsAny<Customer>(), It.IsAny<Store>(),
                It.IsAny<Credit>(), It.IsAny<TransactionType>(), It.IsAny<string>()), Times.Once);

            Assert.Equal((int)TransactionTypes.UpdateChargesPaymentPlan, creditMaster.Current.CreditPayment.GetTransactionTypeId);
            Assert.Equal(300, creditMaster.Current.GetArrearsCharge);
            Assert.Equal(2000, creditMaster.Current.GetChargeValue);
            Assert.True(creditMaster.Current.HasCharges());
            Assert.True(creditMaster.Current.HasUpdatedPaymentPlan());
            Assert.Equal(45000, creditMaster.Current.GetUpdatedPaymentPlanValue);
        }

        [Fact]
        public async Task ShouldGetPaidCreditCertificateAsync()
        {
            List<CreditMaster> creditMaster = CreditMasterHelperTest.GetCreditMasterPaidListTemplate();

            _appParametersServiceMock.Setup(cr => cr.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditMasterRepositoryMock.Setup(mock => mock.GetPaidCreditsForCertificateAsync(It.IsAny<List<Guid>>()))
               .ReturnsAsync(creditMaster);

            List<string> paidCreditCertificateResponse =
                await CreditService.GetPaidCreditCertificateTemplatesAsync(new List<Guid>() { Guid.NewGuid() }, false);

            Assert.NotNull(paidCreditCertificateResponse);
        }

        [Fact]
        public async Task ShouldGetPaidCreditDocumetnAsync()
        {
            List<CreditMaster> creditMaster = CreditMasterHelperTest.GetCreditMasterPaidListTemplate();

            _appParametersServiceMock.Setup(cr => cr.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditMasterRepositoryMock.Setup(mock => mock.GetPaidCreditsForCertificateAsync(It.IsAny<List<Guid>>()))
               .ReturnsAsync(creditMaster);

            List<string> paidCreditCertificateResponse =
                await CreditService.GetPaidCreditDocumentAsync(new List<Guid>() { Guid.NewGuid() });

            Assert.NotNull(paidCreditCertificateResponse);
            _reportTemplatesGatewayMock.Verify(mock => mock.GenerateAsync(It.IsAny<ResponsePaidCreditCertification>(), It.IsAny<string>(), It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task NotShouldGetPaidCreditCertificateCreditsNotFoundAsync()
        {
            _appParametersServiceMock.Setup(cr => cr.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditMasterRepositoryMock.Setup(mock => mock.GetPaidCreditsForCertificateAsync(It.IsAny<List<Guid>>()))
               .ReturnsAsync((List<CreditMaster>)null);

            BusinessException ex = await Assert.ThrowsAsync<BusinessException>(() =>
                CreditService.GetPaidCreditCertificateTemplatesAsync(new List<Guid>() { Guid.NewGuid() }, false));

            Assert.Equal(nameof(BusinessResponse.CreditsNotFound), ex.Message);
        }

        [Fact]
        public async Task ShouldGetCustomerCreditHistory()
        {
            string storeId = "TestStoreId";
            string documentType = "CC";
            string idDocument = "TestIdDocument";

            Customer customer = CustomerHelperTest.GetCustomer();
            List<CreditMaster> creditMasters = CreditMasterHelperTest.GetCreditMasterList();
            List<CreditHistoryResponse> creditHistoryResponses = CreditHistoryHelperTest.GetResponsesFromMasters(creditMasters);
            List<RequestCancelCredit> requestCancelCredits = RequestCancelCreditHelperTest.GetRequestCancelCreditList();

            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            _customerServiceMock.Setup(cr => cr.GetAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(customer);

            _creditMasterRepositoryMock.Setup(mock => mock.GetCustomerCreditHistoryAsync(It.IsAny<Customer>(),
                    It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(creditMasters);

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(parameters);

            _creditUsesCaseMock.Setup(mock => mock.CreateCreditHistory(It.IsAny<List<CreditMaster>>(),
                    It.IsAny<List<RequestCancelCredit>>(), It.IsAny<AppParameters>()))
                .Returns(creditHistoryResponses);

            _requestCancelCreditRepositoryMock.Setup(mock => mock.GetByStatusAsync(It.IsAny<List<Guid>>(),
                    It.IsAny<RequestStatuses>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(requestCancelCredits);

            List<CreditHistoryResponse> creditHistoryResponsesResult = await CreditService.GetCustomerCreditHistoryAsync(storeId, documentType, idDocument);

            Assert.NotNull(creditHistoryResponsesResult);
            Assert.NotEmpty(creditHistoryResponsesResult);
        }

        [Fact]
        public async Task ShouldCustomerAllowPhotoSignature()
        {
            Customer customer = CustomerHelperTest.GetCustomer();
            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            _creditMasterRepositoryMock.Setup(mock => mock.ValidateACreditPaidAccordingToTime(It.IsAny<Guid>(),
                    It.IsAny<int>(), It.IsAny<Statuses>()))
                .ReturnsAsync(true);

            bool allow = await CreditService.CustomerAllowPhotoSignatureAsync(customer, parameters);

            Assert.True(allow);
        }

        [Fact]
        public async Task ShouldValidateCreditToken()
        {
            _storeServiceMock.Setup(ce => ce.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(StoreHelperTest.GetStore());

            _customerServiceMock.Setup(cr => cr.GetActiveAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(CustomerHelperTest.GetCustomer());

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditUsesCaseMock.Setup(mock => mock.CustomerIsDefaulterAsync(It.IsAny<Customer>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(false);

            _creditCommonsServiceMock.Setup(mock => mock.ValidateTokenAsync(It.IsAny<CreateCreditRequest>(), It.IsAny<Customer>())).Verifiable();

            bool createCreditResponseResult = await CreditService.ValidateCreditTokenAsync(CreditHelperTest.GetCreateCreditRequest());

            Assert.True(createCreditResponseResult);
            _creditCommonsServiceMock.Verify(mock => mock.ValidateTokenAsync(It.IsAny<CreateCreditRequest>(), It.IsAny<Customer>()), Times.Exactly(1));
        }

        [Fact]
        public async Task ShouldNotValidateCreditToken()
        {
            _storeServiceMock.Setup(ce => ce.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(StoreHelperTest.GetStore());

            _customerServiceMock.Setup(cr => cr.GetActiveAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(CustomerHelperTest.GetCustomer());

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditUsesCaseMock.Setup(mock => mock.CustomerIsDefaulterAsync(It.IsAny<Customer>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(false);

            _creditCommonsServiceMock.Setup(mock => mock.ValidateTokenAsync(It.IsAny<CreateCreditRequest>(), It.IsAny<Customer>()))
                .Throws(new BusinessException(nameof(BusinessResponse.TokenIsNotValid), (int)BusinessResponse.TokenIsNotValid))
                .Verifiable();

            BusinessException exception = await Assert.ThrowsAsync<BusinessException>(() => CreditService.ValidateCreditTokenAsync(CreditHelperTest.GetCreateCreditRequest()));

            Assert.Equal((int)BusinessResponse.TokenIsNotValid, exception.code);
            _creditCommonsServiceMock.Verify(mock => mock.ValidateTokenAsync(It.IsAny<CreateCreditRequest>(), It.IsAny<Customer>()), Times.Exactly(1));
        }

        [Fact]
        public async Task ShouldNotValidateCreditTokenCauseCustomerDefaulter()
        {
            _storeServiceMock.Setup(ce => ce.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(StoreHelperTest.GetStore());

            _customerServiceMock.Setup(cr => cr.GetActiveAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(CustomerHelperTest.GetCustomer());

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditUsesCaseMock.Setup(mock => mock.CustomerIsDefaulterAsync(It.IsAny<Customer>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(true);

            _creditCommonsServiceMock.Setup(mock => mock.ValidateTokenAsync(It.IsAny<CreateCreditRequest>(), It.IsAny<Customer>())).Verifiable();

            BusinessException exception = await Assert.ThrowsAsync<BusinessException>(() => CreditService.ValidateCreditTokenAsync(CreditHelperTest.GetCreateCreditRequest()));

            Assert.Equal((int)BusinessResponse.CustomerIsDefaulter, exception.code);
            _creditCommonsServiceMock.Verify(mock => mock.ValidateTokenAsync(It.IsAny<CreateCreditRequest>(), It.IsAny<Customer>()), Times.Exactly(0));
        }

        [Fact]
        public async Task ShouldNotValidateCreditTokenCauseStoreStatus()
        {
            _storeServiceMock.Setup(ce => ce.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(StoreHelperTest.GetStoreWithStatus((int)StoreStatuses.Closed));

            _customerServiceMock.Setup(cr => cr.GetActiveAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(CustomerHelperTest.GetCustomer());

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditUsesCaseMock.Setup(mock => mock.CustomerIsDefaulterAsync(It.IsAny<Customer>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(false);

            _creditCommonsServiceMock.Setup(mock => mock.ValidateTokenAsync(It.IsAny<CreateCreditRequest>(), It.IsAny<Customer>())).Verifiable();

            BusinessException exception = await Assert.ThrowsAsync<BusinessException>(() => CreditService.ValidateCreditTokenAsync(CreditHelperTest.GetCreateCreditRequest()));

            Assert.Equal((int)BusinessResponse.CreateCreditUnauthorized, exception.code);
            _creditCommonsServiceMock.Verify(mock => mock.ValidateTokenAsync(It.IsAny<CreateCreditRequest>(), It.IsAny<Customer>()), Times.Exactly(0));
        }

        [Theory]
        [InlineData("1", "Bajo Riesgo")]
        public async Task GenerateTokenWithRiskLevelCalculationAsync(string level, string descripcion)
        {
            //Arrange
            string creditTokenSmsNotificationTemplate = "CreditTokenSmsNotificationTemplate.txt";
            Customer customer = CustomerHelperTest.GetCustomer();
            Store store = StoreHelperTest.GetStore();
            GenerateTokenRequest generateTokenRequest = new GenerateTokenRequestBuilder().Build();
            CustomerRiskLevel customerRiskLevel = new CustomerRiskLevelBuilder(level, descripcion).Build();
            MailNotificationRequest mailNotificationRequest = new MailNotificationRequestBuilder().Build();
            AppParameters appParameters = ParameterHelperTest.GetAppParameters();
            SmsNotificationRequest smsNotificationRequest = new SmsNotificationRequest();
            _customerServiceMock.Setup(mock => mock.GetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>())).ReturnsAsync(customer).Verifiable();
            _storeServiceMock.Setup(mock => mock.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(store).Verifiable();
            _riskLevelRepository.Setup(mock => mock.CalculateRiskLevelAsync(It.IsAny<CustomerRiskLevelRequest>())).ReturnsAsync(customerRiskLevel).Verifiable();
            _creditUsesCaseMock.Setup(mock => mock.GetRiskyCreditRequestNotification(It.IsAny<Customer>(), It.IsAny<Store>(), It.IsAny<GenerateTokenRequest>(), It.IsAny<CustomerRiskLevel>(), It.IsAny<int>(), It.IsAny<Guid>())).Returns(mailNotificationRequest).Verifiable();
            _creditCommonsServiceMock.Setup(mock => mock.GenerateTokenAsync(It.IsAny<CreditTokenRequest>())).ReturnsAsync(new TokenResponse
            {
                Token = new Token
                {
                    Value = "864597",
                    RemainingSeconds = 180
                }
            }).Verifiable();
            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync()).ReturnsAsync(appParameters).Verifiable();
            _templatesServiceMock.Setup(mock => mock.GetAsync(It.IsAny<string>())).ReturnsAsync(creditTokenSmsNotificationTemplate).Verifiable();
            _creditUsesCaseMock.Setup(mock => mock.GetTokenSmsNotification(It.IsAny<string>(), It.IsAny<Customer>(),
                It.IsAny<Store>(), It.IsAny<CreditDetailResponse>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>())).Returns(smsNotificationRequest).Verifiable();
            _creditCommonsServiceMock.Setup(mock => mock.Commons.Notification.SendMailAsync(It.IsAny<MailNotificationRequest>(),
                    It.IsAny<string>(), It.IsAny<bool>())).Returns(Task.CompletedTask).Verifiable();

            //Act
            TokenResponse creditDetailResponse = await CreditService.GenerateTokenWithRiskLevelCalculationAsync(generateTokenRequest);

            //Assert
            Assert.IsType<TokenResponse>(creditDetailResponse);
            _customerServiceMock.Verify(mock => mock.GetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()), Times.Once);
            _storeServiceMock.Verify(mock => mock.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
            _riskLevelRepository.Verify(mock => mock.CalculateRiskLevelAsync(It.IsAny<CustomerRiskLevelRequest>()), Times.Exactly(0));
            _creditUsesCaseMock.Verify(mock => mock.GetRiskyCreditRequestNotification(It.IsAny<Customer>(), It.IsAny<Store>(), It.IsAny<GenerateTokenRequest>(), It.IsAny<CustomerRiskLevel>(), It.IsAny<int>(), It.IsAny<Guid>()), Times.Never);
            _creditCommonsServiceMock.Verify(mock => mock.GenerateTokenAsync(It.IsAny<CreditTokenRequest>()), Times.Once);
            _appParametersServiceMock.Verify(mock => mock.GetAppParametersAsync(), Times.Once);
            _templatesServiceMock.Verify(mock => mock.GetAsync(It.IsAny<string>()), Times.Exactly(0));
            _creditUsesCaseMock.Verify(mock => mock.GetTokenSmsNotification(It.IsAny<string>(), It.IsAny<Customer>(),
               It.IsAny<Store>(), It.IsAny<CreditDetailResponse>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Exactly(0));
            _creditCommonsServiceMock.Verify(mock => mock.Commons.Notification.SendMailAsync(It.IsAny<MailNotificationRequest>(),
                    It.IsAny<string>(), It.IsAny<bool>()),
                Times.Exactly(0));
        }

        [Theory]
        [InlineData("2", "Alto Riesgo")]
        public async Task GenerateTokenWithRiskHighLevelCalculationAsync(string level, string descripcion)
        {
            //Arrange
            string creditTokenSmsNotificationTemplate = "CreditTokenSmsNotificationTemplate.txt";
            Customer customer = CustomerHelperTest.GetCustomer();
            Store store = StoreHelperTest.GetStore();
            GenerateTokenRequest generateTokenRequest = new GenerateTokenRequestBuilder().Build();
            CustomerRiskLevel customerRiskLevel = new CustomerRiskLevelBuilder(level, descripcion).Build();
            MailNotificationRequest mailNotificationRequest = new MailNotificationRequestBuilder().Build();
            AppParameters appParameters = ParameterHelperTest.GetAppParameters();
            SmsNotificationRequest smsNotificationRequest = new SmsNotificationRequest();
            _customerServiceMock.Setup(mock => mock.GetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>())).ReturnsAsync(customer).Verifiable();
            _storeServiceMock.Setup(mock => mock.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(store).Verifiable();
            _riskLevelRepository.Setup(mock => mock.CalculateRiskLevelAsync(It.IsAny<CustomerRiskLevelRequest>())).ReturnsAsync(customerRiskLevel).Verifiable();
            _creditUsesCaseMock.Setup(mock => mock.GetRiskyCreditRequestNotification(It.IsAny<Customer>(), It.IsAny<Store>(), It.IsAny<GenerateTokenRequest>(), It.IsAny<CustomerRiskLevel>(), It.IsAny<int>(), It.IsAny<Guid>())).Returns(mailNotificationRequest).Verifiable();
            _creditCommonsServiceMock.Setup(mock => mock.GenerateTokenAsync(It.IsAny<CreditTokenRequest>())).ReturnsAsync(new TokenResponse
            {
                Token = new Token
                {
                    Value = "864597",
                    RemainingSeconds = 180
                }
            }).Verifiable();
            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync()).ReturnsAsync(appParameters).Verifiable();
            _templatesServiceMock.Setup(mock => mock.GetAsync(It.IsAny<string>())).ReturnsAsync(creditTokenSmsNotificationTemplate).Verifiable();
            _creditUsesCaseMock.Setup(mock => mock.GetTokenSmsNotification(It.IsAny<string>(), It.IsAny<Customer>(),
                It.IsAny<Store>(), It.IsAny<CreditDetailResponse>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>())).Returns(smsNotificationRequest).Verifiable();
            _creditCommonsServiceMock.Setup(mock => mock.Commons.Notification.SendMailAsync(It.IsAny<MailNotificationRequest>(),
                    It.IsAny<string>(), It.IsAny<bool>())).Returns(Task.CompletedTask).Verifiable();

            //Act
            TokenResponse creditDetailResponse = await CreditService.GenerateTokenWithRiskLevelCalculationAsync(generateTokenRequest);

            //Assert
            Assert.IsType<TokenResponse>(creditDetailResponse);
            _customerServiceMock.Verify(mock => mock.GetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()), Times.Once);
            _storeServiceMock.Verify(mock => mock.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
            _riskLevelRepository.Verify(mock => mock.CalculateRiskLevelAsync(It.IsAny<CustomerRiskLevelRequest>()), Times.Exactly(0));
            _creditUsesCaseMock.Verify(mock => mock.GetRiskyCreditRequestNotification(It.IsAny<Customer>(), It.IsAny<Store>(), It.IsAny<GenerateTokenRequest>(), It.IsAny<CustomerRiskLevel>(), It.IsAny<int>(), It.IsAny<Guid>()), Times.Exactly(0));
            _creditCommonsServiceMock.Verify(mock => mock.GenerateTokenAsync(It.IsAny<CreditTokenRequest>()), Times.Exactly(1));
            _appParametersServiceMock.Verify(mock => mock.GetAppParametersAsync(), Times.Once);
            _templatesServiceMock.Verify(mock => mock.GetAsync(It.IsAny<string>()), Times.Exactly(0));
            _creditUsesCaseMock.Verify(mock => mock.GetTokenSmsNotification(It.IsAny<string>(), It.IsAny<Customer>(),
               It.IsAny<Store>(), It.IsAny<CreditDetailResponse>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Exactly(0));
            _creditCommonsServiceMock.Verify(mock => mock.Commons.Notification.SendMailAsync(It.IsAny<MailNotificationRequest>(),
                    It.IsAny<string>(), It.IsAny<bool>()), Times.Exactly(0));
        }

        [Fact]
        public async Task GetRegistrationStatusByCustomerTransaction()
        {
            //Arrange
            CreditRequestAgentAnalysis creditRequestAgentAnalysis = new CreditRequestAgentAnalysisTestBuilder().Build();
            Guid transactionId = new Guid("00BF5060-0247-42F3-BEBD-9B8AFF393A5E");

            _creditRequestAgentAnalysisService.Setup(mock => mock.GetCreditRequestById(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(creditRequestAgentAnalysis).Verifiable();

            //Act
            var response = await CreditService.ExistCreditRequestById(transactionId);

            //Assert
            _creditRequestAgentAnalysisService.Verify(mock => mock.GetCreditRequestById(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>()), Times.Once);
        }
    }
}