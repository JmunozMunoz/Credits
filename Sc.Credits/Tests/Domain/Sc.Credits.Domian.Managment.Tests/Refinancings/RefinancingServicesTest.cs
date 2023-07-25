using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Moq;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Credits;
using Sc.Credits.Domain.Managment.Services.Customers;
using Sc.Credits.Domain.Managment.Services.Refinancings;
using Sc.Credits.Domain.Managment.Services.Stores;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.Model.Refinancings;
using Sc.Credits.Domain.Model.Refinancings.Gateway;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Domain.UseCase.Credits;
using Sc.Credits.Domain.UseCase.Refinancings;
using Sc.Credits.Helper.Test.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace Sc.Credits.Domain.Managment.Tests.Refinancings
{
    public class RefinancingServicesTest
    {
        private readonly Mock<IRefinancingRepository> _refinancingRepositoryMock = new Mock<IRefinancingRepository>();
        private readonly Mock<ICreditCommonsService> _creditCommonsServiceMock = new Mock<ICreditCommonsService>();
        private readonly Mock<ICreditMasterRepository> _creditMasterRepositoryMock = new Mock<ICreditMasterRepository>();
        private readonly Mock<IRefinancingUsesCase> _refinancingUsesCaseMock = new Mock<IRefinancingUsesCase>();
        private readonly Mock<IAppParametersService> _appParametersServiceMock = new Mock<IAppParametersService>();
        private readonly Mock<ICreditPaymentService> _creditPaymentServiceMock = new Mock<ICreditPaymentService>();
        private readonly Mock<ICreditService> _creditServiceMock = new Mock<ICreditService>();
        private readonly Mock<ICustomerService> _customerServiceMock = new Mock<ICustomerService>();
        private readonly Mock<IStoreService> _storeServiceMock = new Mock<IStoreService>();
        private readonly Mock<ICommons> _commonsMock = new Mock<ICommons>();

        public CreditCommons CreditCommons =>
            CreditCommonsHelperTest.Create(_creditMasterRepositoryMock,
                new Mock<IRequestCancelCreditRepository>(),
                new Mock<IRequestCancelPaymentRepository>(),
                new Mock<ICreditUsesCase>(),
                new Mock<ICreditPaymentUsesCase>(),
                new Mock<ICancelUseCase>(),
                _creditCommonsServiceMock);

        public IRefinancingService RefinancingService =>
            new RefinancingService(CreditCommons,
                _refinancingRepositoryMock.Object,
                _refinancingUsesCaseMock.Object,
                _creditPaymentServiceMock.Object,
                _creditServiceMock.Object);

        public RefinancingServicesTest()
        {
            _appParametersServiceMock.Setup(mock => mock.GetSettings())
                .Returns(CredinetAppSettingsHelperTest.GetCredinetAppSettings());

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _commonsMock.SetupGet(mock => mock.AppParameters)
                .Returns(_appParametersServiceMock.Object);

            _commonsMock.SetupGet(mock => mock.CredinetAppSettings)
                .Returns(_appParametersServiceMock.Object.GetSettings());

            _creditCommonsServiceMock.SetupGet(mock => mock.Commons)
                .Returns(_commonsMock.Object);

            _creditCommonsServiceMock.SetupGet(mock => mock.CustomerService)
                .Returns(_customerServiceMock.Object);

            _creditCommonsServiceMock.SetupGet(mock => mock.StoreService)
                .Returns(_storeServiceMock.Object);
        }

        [Fact]
        public async Task ShouldGetCustomerCredits()
        {
            _refinancingRepositoryMock.Setup(mock => mock.GetApplicationAsync(It.IsAny<Guid>()))
                .ReturnsAsync(RefinancingApplicationHelperTest.GetDefault());

            _customerServiceMock.Setup(mock => mock.GetActiveAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(CustomerHelperTest.GetCustomer());

            _creditMasterRepositoryMock.Setup(mock => mock.GetActiveCreditsAsync(It.IsAny<Customer>()))
                .ReturnsAsync(CreditMasterHelperTest.GetCreditMasterList());

            _refinancingUsesCaseMock.Setup(mock => mock.CustomerCredits(It.IsAny<Customer>(), It.IsAny<List<CreditMaster>>(),
                It.IsAny<RefinancingApplication>(), It.IsAny<AppParameters>()))
                .Returns(ModelHelperTest.InstanceModel<CustomerCreditsResponse>());

            CustomerCreditsResponse result = await RefinancingService.GetCustomerCreditsAsync(ModelHelperTest.InstanceModel<CustomerCreditsRequest>());

            Assert.NotNull(result);
        }

        [Fact]
        public async Task ShouldCalculateFees()
        {
            RefinancingApplication application = RefinancingApplicationHelperTest.GetDefault();

            Customer customer = CustomerHelperTest.GetCustomer();

            Store store = StoreHelperTest.GetStore();

            List<CreditMaster> activeCredits = CreditMasterHelperTest.GetCreditMasterList();

            CalculateFeesRequest calculateFeesRequest = ModelHelperTest.InstanceModel<CalculateFeesRequest>();

            calculateFeesRequest.CreditIds = activeCredits.Select(creditMaster => creditMaster.Id).ToArray();

            _refinancingRepositoryMock.Setup(mock => mock.GetApplicationAsync(It.IsAny<Guid>()))
                .ReturnsAsync(application);

            _customerServiceMock.Setup(mock => mock.GetActiveAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(customer);

            _storeServiceMock.Setup(mock => mock.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(store);

            _creditMasterRepositoryMock.Setup(mock => mock.GetWithCurrentAsync(It.IsAny<List<Guid>>(), It.IsAny<Customer>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Statuses>>(), It.IsAny<bool>()))
                .ReturnsAsync(activeCredits);

            _refinancingUsesCaseMock.Setup(mock => mock.FeesResponse(It.IsAny<Customer>(), It.IsAny<Store>(), It.IsAny<List<CreditMaster>>(),
                It.IsAny<AppParameters>(), It.IsAny<RefinancingApplication>()))
                .Returns(ModelHelperTest.InstanceModel<CalculateFeesResponse>());

            CalculateFeesResponse result = await RefinancingService.CalculateFeesAsync(calculateFeesRequest);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task ShouldCalculateFeesThrowsCreditsAreNotValidForRefinancing()
        {
            RefinancingApplication application = RefinancingApplicationHelperTest.GetDefault();

            Customer customer = CustomerHelperTest.GetCustomer();

            Store store = StoreHelperTest.GetStore();

            List<CreditMaster> activeCredits = CreditMasterHelperTest.GetCreditMasterList();

            CalculateFeesRequest calculateFeesRequest = ModelHelperTest.InstanceModel<CalculateFeesRequest>();

            calculateFeesRequest.CreditIds = new Guid[] { Guid.NewGuid() };

            _refinancingRepositoryMock.Setup(mock => mock.GetApplicationAsync(It.IsAny<Guid>()))
                .ReturnsAsync(application);

            _customerServiceMock.Setup(mock => mock.GetActiveAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(customer);

            _storeServiceMock.Setup(mock => mock.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(store);

            _creditMasterRepositoryMock.Setup(mock => mock.GetWithCurrentAsync(It.IsAny<List<Guid>>(), It.IsAny<Customer>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Statuses>>(), It.IsAny<bool>()))
                .ReturnsAsync(activeCredits);

            _refinancingUsesCaseMock.Setup(mock => mock.FeesResponse(It.IsAny<Customer>(), It.IsAny<Store>(), It.IsAny<List<CreditMaster>>(),
                It.IsAny<AppParameters>(), It.IsAny<RefinancingApplication>()))
                .Returns(ModelHelperTest.InstanceModel<CalculateFeesResponse>());

            BusinessException businessException = await Assert.ThrowsAsync<BusinessException>(() =>
                RefinancingService.CalculateFeesAsync(calculateFeesRequest));

            Assert.Equal((int)BusinessResponse.CreditsAreNotValidForRefinancing, businessException.code);
        }

        [Fact]
        public async Task ShouldCreateCreditAsync()
        {
            List<CreditMaster> refinancingCreditMasters = CreditMasterHelperTest.GetCreditMasterList();

            RefinancingCreditRequest refinancingCreditRequest = RefinancingCreditRequestHelperTest.GetDefault(refinancingCreditMasters);

            RefinancingApplication application = RefinancingApplicationHelperTest.GetDefault();

            Store store = StoreHelperTest.GetStore();

            Customer customer = CustomerHelperTest.GetCustomer();

            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            TransactionType paymentTransactionType = TransactionTypeHelperTest.GetPaymentType();

            Status activeStatus = StatusHelperTest.GetActiveStatus();

            Status paidStatus = StatusHelperTest.GetPaidStatus();

            CreateCreditRequest createCreditRequest = CreateCreditRequest.FromRefinancing(refinancingCreditRequest,
                _appParametersServiceMock.Object.GetSettings().RefinancingStoreId);

            CreateCreditDomainRequest createCreditDomainRequest = new CreateCreditDomainRequest(createCreditRequest, customer, store, parameters);

            RefinancingCreditResponse responseCallback = null;

            CreateCreditResponse createCreditResponse = CreditHelperTest.GetCreateCreditResponse();

            List<PaymentCreditResponse> paymentCreditResponses = refinancingCreditMasters.Select(creditMaster => new PaymentCreditResponse()).ToList();

            RefinancingCreditResponse refinancingCreditResponse = RefinancingCreditResponse.FromCreateCredit(createCreditResponse, paymentCreditResponses);

            _refinancingRepositoryMock.Setup(mock => mock.GetApplicationAsync(It.IsAny<Guid>()))
                .ReturnsAsync(application);

            _storeServiceMock.Setup(mock => mock.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(store);

            _customerServiceMock.Setup(mock => mock.GetActiveAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(CustomerHelperTest.GetCustomer());

            _creditMasterRepositoryMock.Setup(mock => mock.GetWithCurrentAsync(It.IsAny<List<Guid>>(), It.IsAny<Customer>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Statuses>>(), It.IsAny<bool>()))
                .ReturnsAsync(refinancingCreditMasters);

            _appParametersServiceMock.Setup(mock => mock.GetAppParametersAsync())
                .ReturnsAsync(parameters);

            _appParametersServiceMock.Setup(mock => mock.GetTransactionTypeAsync(It.IsAny<int>()))
                .ReturnsAsync(paymentTransactionType);

            _appParametersServiceMock.Setup(mock => mock.GetStatusAsync(It.Is<int>(status => status == (int)Statuses.Active)))
                .ReturnsAsync(activeStatus);

            _appParametersServiceMock.Setup(mock => mock.GetStatusAsync(It.Is<int>(status => status == (int)Statuses.Paid)))
                .ReturnsAsync(paidStatus);

            _creditMasterRepositoryMock.Setup(mock =>
                   mock.ExcecuteOnTransactionAsync(It.IsAny<Func<Transaction, Task<RefinancingCreditResponse>>>()))
                       .Callback<Func<Transaction, Task<RefinancingCreditResponse>>>(async handle =>
                       {
                           responseCallback = await handle.Invoke(Transaction.Current);
                       })
                       .ReturnsAsync(refinancingCreditResponse);

            _creditCommonsServiceMock.Setup(mock => mock.NewCreateCreditDomainRequestAsync(It.IsAny<CreateCreditRequest>(),
                    It.IsAny<Customer>(), It.IsAny<Store>()))
                .ReturnsAsync(createCreditDomainRequest);

            _refinancingUsesCaseMock.Setup(mock => mock.RefinanceAsync(It.IsAny<RefinancingDomainRequest>(), It.IsAny<Transaction>(), It.IsAny<bool>()))
                .ReturnsAsync(refinancingCreditResponse);

            await RefinancingService.CreateCreditAsync(refinancingCreditRequest);

            Assert.Equal(responseCallback, refinancingCreditResponse);

            _refinancingUsesCaseMock.Verify(mock => mock.CreateLog(It.IsAny<RefinancingDomainRequest>(), It.IsAny<RefinancingCreditResponse>()),
                Times.Once);

            _refinancingRepositoryMock.Verify(mock => mock.AddLogAsync(It.IsAny<RefinancingLog>(), It.IsAny<Transaction>()),
                Times.Once);

            _creditPaymentServiceMock.Verify(mock => mock.PaymentCreditNotifyAsync(It.IsAny<PaymentCreditResponse>(), It.IsAny<decimal>(),
                It.IsAny<int>()), Times.Exactly(3));

            _creditCommonsServiceMock.Verify(mock => mock.SendEventAsync(It.IsAny<CreditMaster>(), It.IsAny<Customer>(),
                    It.IsAny<Store>(), It.IsAny<Credit>(), It.IsAny<TransactionType>(), It.IsAny<string>()),
                Times.Once);

            _creditCommonsServiceMock.Verify(mock => mock.SendCreationCommandAsync(It.IsAny<CreateCreditResponse>()), Times.Once);
        }

        [Fact]
        public async Task ShouldGenerateToken()
        {
            _creditServiceMock.Setup(mock => mock.GenerateTokenAsync(It.IsAny<GenerateTokenRequest>()))
                .ReturnsAsync(new TokenResponse());

            TokenResponse tokenResponse = await RefinancingService.GenerateTokenAsync(ModelHelperTest.InstanceModel<GenerateTokenRequest>());

            Assert.NotNull(tokenResponse);
        }
    }
}