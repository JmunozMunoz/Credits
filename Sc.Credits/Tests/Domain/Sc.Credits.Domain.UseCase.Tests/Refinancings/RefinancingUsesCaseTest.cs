using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Moq;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.Domain.Model.Refinancings;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Domain.UseCase.Credits;
using Sc.Credits.Domain.UseCase.Refinancings;
using Sc.Credits.Helper.Test.Model;
using Sc.Credits.Helpers.Commons.Settings;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace Sc.Credits.Domain.UseCase.Tests.Refinancings
{
    public class RefinancingUsesCaseTest
    {
        private readonly Mock<ICreditUsesCase> _creditUsesCaseMock = new Mock<ICreditUsesCase>();
        private readonly Mock<ICreditPaymentUsesCase> _creditPaymentUsesCaseMock = new Mock<ICreditPaymentUsesCase>();
        private readonly Mock<ISettings<CredinetAppSettings>> _appSettingsMock = new Mock<ISettings<CredinetAppSettings>>();

        public IRefinancingUsesCase RefinancingUsesCase =>
            new RefinancingUsesCase(_creditUsesCaseMock.Object,
                _creditPaymentUsesCaseMock.Object,
                _appSettingsMock.Object);

        public RefinancingUsesCaseTest()
        {
            _appSettingsMock.Setup(mock => mock.Get())
                .Returns(new CredinetAppSettings
                {
                    RefinancingFeesAllowed = "1,2,4",
                    RefinancingSourcesAllowed = "5"
                });
        }

        [Theory]
        [InlineData(-1, true)]
        [InlineData(int.MinValue, true)]
        [InlineData(1, false)]
        [InlineData(int.MaxValue, false)]
        [InlineData(int.MinValue, false)]
        public void ShouldCustomerCreditsThrowsCreditsNotFoundException(int arrearsDays, bool hasArrears)
        {
            Customer customer = CustomerHelperTest.GetCustomer();

            List<CreditMaster> activeCredits = CreditMasterHelperTest.GetCreditMasterList(customer.Id);

            RefinancingApplication refinancingApplication = RefinancingApplicationHelperTest.GetDefault();

            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            _creditUsesCaseMock.Setup(mock => mock.HasArrears(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(hasArrears);

            _creditUsesCaseMock.Setup(mock => mock.GetArrearsDays(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(arrearsDays);

            _creditPaymentUsesCaseMock.Setup(mock => mock.GetPaymentAlternatives(It.IsAny<CreditMaster>(), It.IsAny<Credit>(), It.IsAny<AppParameters>(),
                It.IsAny<DateTime>()))
                .Returns(new PaymentAlternatives { TotalPayment = 0 });

            BusinessException exception = Assert.Throws<BusinessException>(() => RefinancingUsesCase.CustomerCredits(customer, activeCredits,
                refinancingApplication, parameters));

            Assert.Equal((int)BusinessResponse.CreditsNotFound, exception.code);
        }

        [Theory]
        [InlineData(1, false)]
        [InlineData(30, false)]
        [InlineData(int.MaxValue, false)]
        [InlineData(1, true)]
        [InlineData(30, true)]
        [InlineData(int.MaxValue, true)]
        public void ShouldCustomerCreditsResturnsSuccesfully(int arrearsDays, bool allowRefinancingCredits)
        {
            Customer customer = CustomerHelperTest.GetCustomer();

            List<CreditMaster> activeCredits = CreditMasterHelperTest.GetCreditMasterList(customer.Id);

            RefinancingApplication refinancingApplication = RefinancingApplicationHelperTest.GetDefault(allowRefinancingCredits);

            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            _creditUsesCaseMock.Setup(mock => mock.HasArrears(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(true);

            _creditUsesCaseMock.Setup(mock => mock.GetArrearsDays(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(arrearsDays);

            _creditPaymentUsesCaseMock.Setup(mock => mock.GetPaymentAlternatives(It.IsAny<CreditMaster>(), It.IsAny<Credit>(), It.IsAny<AppParameters>(),
                It.IsAny<DateTime>()))
                .Returns(new PaymentAlternatives { TotalPayment = 0 });

            CustomerCreditsResponse result = RefinancingUsesCase.CustomerCredits(customer, activeCredits,
                refinancingApplication, parameters);

            Assert.NotNull(result);
            Assert.True(result.Details.Any());
            Assert.Equal(6, result.Details.Count);
        }

        [Theory]
        [InlineData(-1, true, true)]
        [InlineData(int.MinValue, true, true)]
        [InlineData(1, false, true)]
        [InlineData(int.MaxValue, false, true)]
        [InlineData(int.MinValue, false, true)]
        [InlineData(-1, true, false)]
        [InlineData(int.MinValue, true, false)]
        [InlineData(1, false, false)]
        [InlineData(int.MaxValue, false, false)]
        [InlineData(int.MinValue, false, false)]
        public void ShouldFeeResponseThrowsCreditIsNotRefinancingAllowedException(int arrearsDays, bool hasArrears, bool allowRefinancingCredits)
        {
            Customer customer = CustomerHelperTest.GetCustomer();

            List<CreditMaster> refinancingCreditMasters = CreditMasterHelperTest.GetCreditMasterList(customer.Id);

            Store store = StoreHelperTest.GetStore();

            RefinancingApplication refinancingApplication = RefinancingApplicationHelperTest.GetDefault(allowRefinancingCredits);

            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            _creditUsesCaseMock.Setup(mock => mock.HasArrears(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(hasArrears);

            _creditUsesCaseMock.Setup(mock => mock.GetArrearsDays(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(arrearsDays);

            _creditPaymentUsesCaseMock.Setup(mock => mock.GetPaymentAlternatives(It.IsAny<CreditMaster>(), It.IsAny<Credit>(), It.IsAny<AppParameters>(),
                It.IsAny<DateTime>()))
                .Returns(new PaymentAlternatives { TotalPayment = 0 });

            BusinessException exception = Assert.Throws<BusinessException>(() => RefinancingUsesCase.FeesResponse(customer, store, refinancingCreditMasters,
                parameters, refinancingApplication));

            Assert.Equal((int)BusinessResponse.CreditsAreNotValidForRefinancing, exception.code);
        }

        [Fact]
        public void ShouldFeeResponseThrowsRefinancingValueShouldBeGreaterThanZeroException()
        {
            Customer customer = CustomerHelperTest.GetCustomer();

            List<CreditMaster> refinancingCreditMasters = CreditMasterHelperTest.GetCreditMasterList(customer.Id);

            Store store = StoreHelperTest.GetStore();

            RefinancingApplication refinancingApplication = RefinancingApplicationHelperTest.GetDefault();

            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            _creditUsesCaseMock.Setup(mock => mock.HasArrears(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(true);

            _creditUsesCaseMock.Setup(mock => mock.GetArrearsDays(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(10);

            _creditPaymentUsesCaseMock.Setup(mock => mock.GetPaymentAlternatives(It.IsAny<CreditMaster>(), It.IsAny<Credit>(), It.IsAny<AppParameters>(),
                It.IsAny<DateTime>()))
                .Returns(new PaymentAlternatives { TotalPayment = 0 });

            BusinessException exception = Assert.Throws<BusinessException>(() => RefinancingUsesCase.FeesResponse(customer, store, refinancingCreditMasters,
                parameters, refinancingApplication));

            Assert.Equal((int)BusinessResponse.RefinancingValueShouldBeGreaterThanZero, exception.code);
        }

        [Theory]
        [InlineData(1, false)]
        [InlineData(30, false)]
        [InlineData(int.MaxValue, false)]
        [InlineData(1, true)]
        [InlineData(30, true)]
        [InlineData(int.MaxValue, true)]
        public void ShouldFeeResponseSuccesfully(int arrearsDays, bool allowRefinancingCredits)
        {
            Customer customer = CustomerHelperTest.GetCustomer();

            Store store = StoreHelperTest.GetStore();

            List<CreditMaster> refinancingCreditMasters = CreditMasterHelperTest.GetCreditMasterList(customer.Id);

            RefinancingApplication refinancingApplication = RefinancingApplicationHelperTest.GetDefault(allowRefinancingCredits);

            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            decimal totalPayment = 80000;

            _creditUsesCaseMock.Setup(mock => mock.HasArrears(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(true);

            _creditUsesCaseMock.Setup(mock => mock.GetArrearsDays(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(arrearsDays);

            _creditPaymentUsesCaseMock.Setup(mock => mock.GetPaymentAlternatives(It.IsAny<CreditMaster>(),
                It.IsAny<Credit>(), It.IsAny<AppParameters>(), It.IsAny<DateTime>()))
                .Returns(new PaymentAlternatives { MinimumPayment = 20000, TotalPayment = totalPayment });

            _creditUsesCaseMock.Setup(mock => mock.GetTimeLimitInMonths(It.IsAny<Customer>(), It.IsAny<decimal>(), It.IsAny<Store>(),
                It.IsAny<int>(), It.IsAny<decimal>()))
                .Returns(6);

            _creditUsesCaseMock.Setup(mock => mock.GetCreditDetails(It.IsAny<CreditDetailDomainRequest>()))
                .Returns<CreditDetailDomainRequest>((request) => new CreditDetailResponse
                {
                    TotalFeeValue = totalPayment / request.Fees
                });


            CalculateFeesResponse result = RefinancingUsesCase.FeesResponse(customer, store, refinancingCreditMasters,
                parameters, refinancingApplication);

            Assert.NotNull(result);
            Assert.True(result.Fees.Any());
            Assert.Equal(3, result.Fees.Count);
            Assert.Equal(480000, result.CreditValue);
        }

        [Fact]
        public async Task ShouldRefinance()
        {
            List<CreditMaster> refinancingCreditMasters = CreditMasterHelperTest.GetCreditMasterList();

            RefinancingCreditRequest refinancingCreditRequest = RefinancingCreditRequestHelperTest.GetDefault(refinancingCreditMasters);

            Customer customer = CustomerHelperTest.GetCustomer();

            Store store = StoreHelperTest.GetStore();

            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            CreateCreditRequest createCreditRequest = CreateCreditRequest.FromRefinancing(refinancingCreditRequest,
                CredinetAppSettingsHelperTest.GetCredinetAppSettings().RefinancingStoreId);

            CreateCreditDomainRequest createCreditDomainRequest = new CreateCreditDomainRequest(createCreditRequest, customer, store, parameters);

            RefinancingApplication application = RefinancingApplicationHelperTest.GetDefault();

            RefinancingDomainRequest refinancingDomainRequest = new RefinancingDomainRequest(refinancingCreditRequest,
                    createCreditDomainRequest, customer, store, parameters)
                .SetRefinancingParams(refinancingCreditMasters, application, CredinetAppSettingsHelperTest.GetCredinetAppSettings());

            decimal totalPayment = 80000;

            PaymentCreditResponse paymentCreditResponse = new PaymentCreditResponse
            {
                TotalValuePaid = totalPayment
            };

            CreateCreditResponse createCreditResponse = new CreateCreditResponse();

            _creditPaymentUsesCaseMock.Setup(mock => mock.GetPaymentAlternatives(It.IsAny<CreditMaster>(),
                It.IsAny<Credit>(), It.IsAny<AppParameters>(), It.IsAny<DateTime>()))
                .Returns(new PaymentAlternatives { MinimumPayment = 20000, TotalPayment = totalPayment });

            _creditUsesCaseMock.Setup(mock => mock.HasArrears(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(true);

            _creditUsesCaseMock.Setup(mock => mock.GetArrearsDays(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(30);

            _creditPaymentUsesCaseMock.Setup(mock => mock.PayAsync(It.IsAny<PaymentDomainRequest>(), It.IsAny<PaymentType>(),
                    It.IsAny<Transaction>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(paymentCreditResponse);

            _creditUsesCaseMock.Setup(mock => mock.CreateAsync(It.IsAny<CreateCreditDomainRequest>(), It.IsAny<Transaction>(), It.IsAny<bool>()))
                .ReturnsAsync(createCreditResponse);

            RefinancingCreditResponse response = await RefinancingUsesCase.RefinanceAsync(refinancingDomainRequest, Transaction.Current);

            Assert.NotNull(response);
            Assert.Equal(3, response.PaymentCreditResponses.Count);

            _creditPaymentUsesCaseMock.Verify(mock => mock.PayAsync(It.IsAny<PaymentDomainRequest>(), It.IsAny<PaymentType>(),
                    It.IsAny<Transaction>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Exactly(3));
        }

        [Fact]
        public void ShouldCreateLog()
        {
            List<CreditMaster> refinancingCreditMasters = CreditMasterHelperTest.GetCreditMasterList();

            RefinancingCreditRequest refinancingCreditRequest = RefinancingCreditRequestHelperTest.GetDefault(refinancingCreditMasters);

            CreateCreditRequest createCreditRequest = CreateCreditRequest.FromRefinancing(refinancingCreditRequest,
                CredinetAppSettingsHelperTest.GetCredinetAppSettings().RefinancingStoreId);

            Customer customer = CustomerHelperTest.GetCustomer();

            Store store = StoreHelperTest.GetStore();

            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            CreateCreditDomainRequest createCreditDomainRequest = new CreateCreditDomainRequest(createCreditRequest, customer, store, parameters);

            RefinancingApplication application = RefinancingApplicationHelperTest.GetDefault();

            RefinancingDomainRequest refinancingDomainRequest = new RefinancingDomainRequest(refinancingCreditRequest,
                    createCreditDomainRequest, customer, store, parameters)
                .SetRefinancingParams(refinancingCreditMasters, application, CredinetAppSettingsHelperTest.GetCredinetAppSettings());

            List<PaymentCreditResponse> paymentCreditResponses =
                refinancingCreditMasters.Select(creditMaster =>
                    new PaymentCreditResponse
                    {
                        TotalValuePaid = 80000,
                        CreditMaster = creditMaster,
                        Credit = creditMaster.Current
                    })
                .ToList();

            CreateCreditResponse createCreditResponse = new CreateCreditResponse();

            RefinancingCreditResponse refinancingCreditResponse = RefinancingCreditResponse.FromCreateCredit(createCreditResponse, paymentCreditResponses);

            RefinancingLog log = RefinancingUsesCase.CreateLog(refinancingDomainRequest, refinancingCreditResponse);

            Assert.NotNull(log);
            Assert.Equal(3, log.Details.Count);
        }
    }
}