using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Moq;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Customers.Gateway;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.Model.Sequences.Gateway;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Domain.UseCase.Credits;
using Sc.Credits.Helper.Test.Model;
using Sc.Credits.Helpers.Commons.Settings;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;
using Sc.Credits.Helpers.Commons.Extensions;

namespace Sc.Credits.Domain.UseCase.Tests.Credits
{
    public class CreditsUsesCaseTest
    {
        private readonly Mock<ICreditMasterRepository> _creditMasterRepositoryMock = new Mock<ICreditMasterRepository>();
        private readonly Mock<ISequenceRepository> _sequenceRepositoryMock = new Mock<ISequenceRepository>();
        private readonly Mock<ICustomerRepository> _customerRepositoryMock = new Mock<ICustomerRepository>();
        private readonly Mock<ISettings<CredinetAppSettings>> _appSettingsMock = new Mock<ISettings<CredinetAppSettings>>();
        private readonly Mock<ICreditOperationsUseCase> _creditsOperationsMock = new Mock<ICreditOperationsUseCase>();

        public ICreditUsesCase CreditUsesCase
        {
            get
            {
                return new CreditUsesCase(_creditMasterRepositoryMock.Object,
                    _sequenceRepositoryMock.Object,
                    _customerRepositoryMock.Object,
                    _appSettingsMock.Object,
                    _creditsOperationsMock.Object);
            }
        }

        public CreditsUsesCaseTest()
        {
            _appSettingsMock.Setup(mock => mock.Get())
               .Returns(new CredinetAppSettings
               {
                   PromissoryNotePath = "promissorynotes",
                   PdfBlobContainerName = "pdf",
                   CreateCreditNotificationTemplateId = "454545232155",
                   CreditTokenNotificationTemplateId = "54545455687",
                   AmortizationScheduleUrlTemplate = "https://paymentplan.z20.web.core.windows.net/?creditValue={creditValue}&interestRate={interestRate}&frequency={frequency}&fees={fees}&assuranceValue={assuranceValue}&downPayment={downPayment}&initialDate={initialDate}&feeValue={feeValue}&assuranceFeeValue={assuranceFeeValue}&assuranceTotalFeeValue={assuranceTotalFeeValue}&client={customer.FullName}",
                   ValidateTokenOnCreate = true,
                   RefinancingSourcesAllowed = "5"
               });
        }

        [Fact]
        public void ShouldCalculateTimeLimitsMonthsNotMandatoryDownPayment()
        {
            Customer customer = CustomerHelperTest.GetCustomer();
            Store store = StoreHelperTest.GetStoreCustomWithCustomStoreCategory("5686528", 40000M, 0.10M, "Test Store", false, AssuranceTypes.TypeA, "vendorId", 0.283200M, "Default", 3, 8, 30000M, 300000M, 100000);
            int timeLimitsMonthsExpected = 1;

            Assert.Equal(timeLimitsMonthsExpected, CreditUsesCase.GetTimeLimitInMonths(customer, 50000M, store, 2, 300000));
        }

        [Fact]
        public void ShouldCalculateTimeLimitsMonthsWithMandatoryDownPayment()
        {
            Customer customer = CustomerHelperTest.GetCustomer();
            Store store = StoreHelperTest.GetStoreCustomWithCustomStoreCategory("525215512", 30000M, 0.10M, "Test Store", false, AssuranceTypes.TypeA, "vendorId", 0.283200M, "Default", 3, 8, 20000M, 300000M, 30000);
            int timeLimitsMonthsExpected = 6;

            Assert.Equal(timeLimitsMonthsExpected,
                CreditUsesCase.GetTimeLimitInMonths(customer, 200000M, store, 2, 300000));
        }

        [Fact]
        public void NotShouldCalculateTimeLimitsMonthsWithMandatoryDownPaymentEqualCapital()
        {
            Customer customer = CustomerHelperTest.GetCustomerCustom(300000M, 250000M, 0);
            Store store = StoreHelperTest.GetStoreCustomWithCustomStoreCategory("54125215", 21000M, 0.10M, "Test Store", false, AssuranceTypes.TypeA, "vendorId", 0.283200M, "Default", 3, 8, 30000M, 300000M, 100000);
            Assert.Throws<BusinessException>(() =>
                CreditUsesCase.GetTimeLimitInMonths(customer, 20000M, store, 2, 300000));
        }

        [Fact]
        public void NotShouldCalculateTimeLimitsMonthsWithMandatoryDownPaymentFromStore()
        {
            Customer customer = CustomerHelperTest.GetCustomerCustom(300000M, 250000M, DownPayments.Store);
            Store store = StoreHelperTest.GetStoreCustomWithCustomStoreCategory("45545452", 40000M, 0.10M, "Test Store", true, AssuranceTypes.TypeA, "vendorId", 0.283200M, "Default", 3, 8, 30000M, 300000M, 100000);
            int timeLimitsMonthsExpected = 1;

            Assert.Equal(timeLimitsMonthsExpected,
                CreditUsesCase.GetTimeLimitInMonths(customer, 50000M, store, 2, 300000));
        }

        [Fact]
        public void NotShouldCalculateTimeLimitsMonthsAvailableCreditLimit()
        {
            Customer customer = CustomerHelperTest.GetCustomerCustom(300000M, 25000M, DownPayments.Never);
            Store store = StoreHelperTest.GetStoreCustomWithCustomStoreCategory("47556325", 50000M, 0.10M, "tertg435tfgbb345", false, AssuranceTypes.TypeA, "vendorId", 0.283200M, "Default", 4, 8, 60000M, 300000M, 100000);

            Assert.Throws<BusinessException>(() =>
                CreditUsesCase.GetTimeLimitInMonths(customer, 50000M, store, 2, 300000));
        }

        [Fact]
        public void NotShouldCalculateCuotaMinorThatMinimalFeeStore()
        {
            Customer customer = CustomerHelperTest.GetCustomer();
            Store store = StoreHelperTest.GetStoreCustomWithCustomStoreCategory("985256545", 30000M, 0.10M, "Test Store", true, AssuranceTypes.TypeA, "vendorId", 0.283200M, "Default", 3, 8, 30000M, 300000M, 100000);
            Assert.Throws<BusinessException>(() => CreditUsesCase.GetTimeLimitInMonths(customer, 20000M, store, 2, 300000));
        }

        [Fact]
        public void ShouldCalculateGetOriginalAmortizationSchedule()
        {
            decimal creditValue = 100000;
            decimal interestRate = 0.0209971486861902M;
            int frequency = 30;
            int fees = 15;
            decimal assuranceValue = 10000;
            decimal downPayment = 10000;
            int decimalNumbersRound = 2;
            DateTime initialDate = new DateTime(2019, 01, 31);
            decimal feeValue = 7057;
            decimal assuranceFeeValue = 625;
            decimal assuranceTotalFeeValue = 743.75M;

            AmortizationScheduleResponse amortizationScheduleResponse =
                CreditUsesCase.GetOriginalAmortizationSchedule(
                    AmortizationScheduleHelperTest.GetAmortizationScheduleRequest(creditValue, initialDate, feeValue, interestRate, frequency, fees,
                        downPayment, assuranceValue, assuranceFeeValue, assuranceTotalFeeValue),
                    decimalNumbersRound);

            Assert.True(amortizationScheduleResponse.AmortizationScheduleFees.Any());
            Assert.Equal(fees + 1, amortizationScheduleResponse.AmortizationScheduleFees.Count);

            var fiveFee = amortizationScheduleResponse.AmortizationScheduleFees.First(item => item.Fee == 4);

            Assert.Equal(74170.46M, fiveFee.Balance);
            Assert.Equal(5499.63M, fiveFee.CreditValuePayment);
            Assert.Equal(68670.83M, fiveFee.FinalBalance);

            var lastFee = amortizationScheduleResponse.AmortizationScheduleFees.Last();

            Assert.Equal(6905.91M, lastFee.Balance);
            Assert.Equal(6905.91M, lastFee.CreditValuePayment);
            Assert.Equal(0, lastFee.FinalBalance);
        }

        [Fact]
        public void ShouldCalculateGetOriginalAmortizationScheduleAssuranceWithoutDownPayment()
        {
            decimal creditValue = 100000;
            decimal interestRate = 0.0209971486861902M;
            int frequency = 30;
            int fees = 9;
            decimal assuranceValue = 10000;
            decimal downPayment = 0;
            int decimalNumbersRound = 2;
            DateTime initialDate = new DateTime(2019, 01, 31);
            decimal feeValue = 12310;
            decimal assuranceFeeValue = 1111.11M;
            decimal assuranceTotalFeeValue = 1322.22M;

            AmortizationScheduleResponse amortizationScheduleResponse =
                CreditUsesCase.GetOriginalAmortizationSchedule(
                    AmortizationScheduleHelperTest.GetAmortizationScheduleRequest(creditValue, initialDate, feeValue, interestRate, frequency, fees,
                        downPayment, assuranceValue, assuranceFeeValue, assuranceTotalFeeValue),
                    decimalNumbersRound);

            Assert.True(amortizationScheduleResponse.AmortizationScheduleAssuranceFees.Any());
            Assert.Equal(fees + 1, amortizationScheduleResponse.AmortizationScheduleAssuranceFees.Count);

            var fiveFee = amortizationScheduleResponse.AmortizationScheduleAssuranceFees.First(item => item.Fee == 4);

            Assert.Equal(6666.67M, fiveFee.Balance);
            Assert.Equal(5555.56M, fiveFee.FinalBalance);

            var lastFee = amortizationScheduleResponse.AmortizationScheduleAssuranceFees.Last();

            Assert.Equal(1111.12M, lastFee.Balance);
            Assert.Equal(1111.12M, lastFee.AssuranceFeeValue);
            Assert.Equal(0, lastFee.FinalBalance);
        }

        [Fact]
        public void ShouldCalculateGetOriginalAmortizationScheduleAssuranceWithDownPayment()
        {
            decimal creditValue = 100000;
            decimal interestRate = 0.0209971486861902M;
            int frequency = 30;
            int fees = 11;
            decimal assuranceValue = 10000;
            decimal downPayment = 10000;
            int decimalNumbersRound = 2;
            DateTime initialDate = new DateTime(2019, 01, 31);
            decimal feeValue = 9248;
            decimal assuranceFeeValue = 833.33M;
            decimal assuranceTotalFeeValue = 991.66M;

            AmortizationScheduleResponse amortizationScheduleResponse =
                CreditUsesCase.GetOriginalAmortizationSchedule(
                    AmortizationScheduleHelperTest.GetAmortizationScheduleRequest(creditValue, initialDate, feeValue, interestRate, frequency, fees,
                        downPayment, assuranceValue, assuranceFeeValue, assuranceTotalFeeValue),
                    decimalNumbersRound);

            Assert.True(amortizationScheduleResponse.AmortizationScheduleAssuranceFees.Any());
            Assert.Equal(fees + 1, amortizationScheduleResponse.AmortizationScheduleAssuranceFees.Count);

            var fiveFee = amortizationScheduleResponse.AmortizationScheduleAssuranceFees.First(item => item.Fee == 4);

            Assert.Equal(6666.68M, fiveFee.Balance);
            Assert.Equal(5833.35M, fiveFee.FinalBalance);

            var lastFee = amortizationScheduleResponse.AmortizationScheduleAssuranceFees.Last();

            Assert.Equal(833.37M, lastFee.Balance);
            Assert.Equal(833.37M, lastFee.AssuranceFeeValue);
            Assert.Equal(0, lastFee.FinalBalance);
        }

        [Fact]
        public async Task ShouldNewCreditButtonEnabledTestFalse()
        {
            Customer customer = CustomerHelperTest.GetCustomerCreditLimit();

            List<CreditMaster> creditMasterComposites = CreditMasterHelperTest.GetCreditMasterList();
            _creditMasterRepositoryMock.Setup(item => item.GetActiveAndCancelRequestCreditsAsync(It.IsAny<Customer>()))
                .ReturnsAsync(creditMasterComposites);

            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            decimal partialCreditLimit = parameters.PartialCreditLimit;
            int decimalNumbersRound = parameters.DecimalNumbersRound;
            int arrearsGracePeriod = parameters.ArrearsGracePeriod;
            decimal minimumCreditValue = parameters.MinimumCreditValue;

            CustomerCreditLimitResponse customerCreditLimitResponse =
                await CreditUsesCase.GetCustomerCreditLimitAsync(customer, partialCreditLimit, decimalNumbersRound,
                    arrearsGracePeriod, minimumCreditValue, "14524");

            Assert.NotNull(customerCreditLimitResponse);
            Assert.IsType<CustomerCreditLimitResponse>(customerCreditLimitResponse);
            Assert.True(customerCreditLimitResponse.ValidatedMail);
            Assert.False(customerCreditLimitResponse.NewCreditButtonEnabled);
        }

        [Fact]
        public void CreditLimitIncreaseValidateWithValidClientReturnTrue()
        {
            Customer customer = CustomerHelperTest.GetCustomerCreditLimit();
            bool CreditLimitIncrease = customer.AllowCreditLimitIncrease();
            Assert.True(CreditLimitIncrease);
        }

        [Fact]
        public void CreditLimitIncreaseValidateWithDeniedClientReturnFalse()
        {
            Customer customer = CustomerHelperTest.GetCustomerByStatus(CustomerStatuses.Denied);
            bool CreditLimitIncrease = customer.AllowCreditLimitIncrease();
            Assert.False(CreditLimitIncrease);
        }

        [Fact]
        public async Task ShouldNewCreditButtonEnabledTestTrue()
        {
            Customer customer = CustomerHelperTest.GetCustomerCreditLimitTrue();
            List<CreditMaster> creditMasters = CreditMasterHelperTest.GetCreditMasterList();

            _creditMasterRepositoryMock.Setup(item => item.GetActiveAndCancelRequestCreditsAsync(It.IsAny<Customer>()))
                .ReturnsAsync(creditMasters);

            AppParameters parameters = ParameterHelperTest.GetAppParametersCustom(0.19M, 2, 300000, 50000, 0, 0.2832M);

            decimal partialCreditLimit = parameters.PartialCreditLimit;
            int decimalNumbersRound = parameters.DecimalNumbersRound;
            int arrearsGracePeriod = parameters.ArrearsGracePeriod;
            decimal minimumCreditValue = parameters.MinimumCreditValue;

            CustomerCreditLimitResponse customerCreditLimitResponse =
                await CreditUsesCase.GetCustomerCreditLimitAsync(customer, partialCreditLimit, decimalNumbersRound, arrearsGracePeriod,
                    minimumCreditValue, "14524");

            Assert.NotNull(customerCreditLimitResponse);
            Assert.IsType<CustomerCreditLimitResponse>(customerCreditLimitResponse);
            Assert.True(customerCreditLimitResponse.ValidatedMail);
            Assert.True(customerCreditLimitResponse.NewCreditButtonEnabled);
        }

        [Fact]
        public void ShouldUpdateExtraFields()
        {
            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMasterExtraFields();
            UpdateCreditExtraFieldsRequest updateCreditExtraFieldsRequest = CreditMasterHelperTest.GetUpdateCreditExtraFields();

            CreditUsesCase.UpdateExtraFields(updateCreditExtraFieldsRequest, creditMaster);

            Assert.IsType<CreditMaster>(creditMaster);
            Assert.Equal(creditMaster.GetCreditSeller, updateCreditExtraFieldsRequest.Seller);
            Assert.Equal(creditMaster.GetCreditProducts, updateCreditExtraFieldsRequest.Products);
        }

        [Fact]
        public async Task ShouldAddCredit()
        {
            CreditDetailResponse detailResponse = CreditDetailResponseHelperTest.GetCreditDetailResponse_BasicData();

            _creditsOperationsMock.Setup(process => process.CreateCreditDetails(It.IsAny<GeneralCreditDetailDomainRequest>())).Returns(detailResponse);

            CreateCreditDomainRequest createCreditDomainRequest = CreateCreditDomainRequestHelperTest.GetDefault(CreditHelperTest.GetCreateCreditRequest(),
                                                                                     SourceHelperTest.GetCredinetSource(),
                                                                                     AuthMethodHelperTest.GetTokenAuthMethod(),
                                                                                     ParameterHelperTest.GetAppParameters());

            CreateCreditResponse createCreditResponse =
                 await CreditUsesCase.CreateAsync(createCreditDomainRequest);

            _creditMasterRepositoryMock.Verify(mock => mock.AddAsync(It.IsAny<CreditMaster>(), It.IsAny<Transaction>()),
                Times.Once);

            Assert.Equal(createCreditDomainRequest.CreateCreditRequest.Invoice, createCreditResponse.CreditMaster.GetCreditInvoice);
            Assert.NotNull(createCreditResponse);
        }

        [Fact]
        public async Task ShouldCustomerCreditLimitDecrease()
        {
            CreditDetailResponse detailResponse = CreditDetailResponseHelperTest.GetCreditDetailResponse_BasicData();

            _creditsOperationsMock.Setup(process => process.CreateCreditDetails(It.IsAny<GeneralCreditDetailDomainRequest>())).Returns(detailResponse);

            CreateCreditResponse createCreditResponse = await CreditUsesCase.CreateAsync(
                CreateCreditDomainRequestHelperTest.GetDefault(CreditHelperTest.GetCreateCreditRequest(),
                    SourceHelperTest.GetCredinetSource(), AuthMethodHelperTest.GetTokenAuthMethod(), ParameterHelperTest.GetAppParameters()));

            _customerRepositoryMock.Verify(mock => mock.UpdateAsync(It.IsAny<Customer>(), It.IsAny<IEnumerable<Field>>(),
                It.IsAny<Transaction>()), Times.Once());

            Assert.Equal(250000, createCreditResponse.CreditMaster.Customer.GetAvailableCreditLimit);
        }

        [Fact]
        public async Task ShouldAddCreditWithSourcePaymentGateways()
        {
            CreditDetailResponse detailResponse = CreditDetailResponseHelperTest.GetCreditDetailResponse_BasicData();

            _creditsOperationsMock.Setup(process => process.CreateCreditDetails(It.IsAny<GeneralCreditDetailDomainRequest>())).Returns(detailResponse);

            var createCreditDomainRequest = CreateCreditDomainRequestHelperTest.GetDefault(CreditHelperTest.GetCreateCreditRequestSourcePaymentGateways(),
                    SourceHelperTest.GetCredinetSourcePaymentGateways(), AuthMethodHelperTest.GetTokenAuthMethod(), ParameterHelperTest.GetAppParametersCustom(0.19m, 0, 300000, 50000, 3, 0.2832m));

            CreateCreditResponse createCreditResponse =
                await CreditUsesCase.CreateAsync(createCreditDomainRequest);

            _creditMasterRepositoryMock.Verify(mock => mock.AddAsync(It.IsAny<CreditMaster>(), It.IsAny<Transaction>()),
                Times.Once());

            Assert.NotNull(createCreditResponse);
            Assert.Equal(250001m, createCreditResponse.CreditValue);
            Assert.True(createCreditResponse.AlternatePayment);
        }

        [Fact]
        public void ShouldGetTokenSmsNotification()
        {
            string template = "Token sms notification test";
            SmsNotificationRequest smsNotificationRequest = CreditUsesCase.GetTokenSmsNotification(template, CustomerHelperTest.GetCustomer(),
                StoreHelperTest.GetStore(), CreditHelperTest.GetCreditDetails(), 6, 14, "486258", 2);

            Assert.NotNull(smsNotificationRequest);
            Assert.Equal("3205875958", smsNotificationRequest.Mobile);
            Assert.False(string.IsNullOrEmpty(smsNotificationRequest.Message));
            Assert.Single(smsNotificationRequest.UrlTemplateValues);
        }

        [Fact]
        public void ShouldGetTokenMailNotification()
        {
            MailNotificationRequest mailNotificationRequest = CreditUsesCase.GetTokenMailNotification(CustomerHelperTest.GetCustomer(), StoreHelperTest.GetStore(),
                CreditHelperTest.GetCreditDetails(), 6, 14, "486258", 2);

            Assert.NotNull(mailNotificationRequest);
            Assert.True(mailNotificationRequest.Recipients.Any());
            Assert.NotNull(mailNotificationRequest.TemplateInfo);
            Assert.True(mailNotificationRequest.TemplateInfo.TemplateValues.Any());
        }

        [Fact]
        public void ShouldGetCreateCreditMailNotification()
        {
            MailNotificationRequest mailNotificationRequest = CreditUsesCase.GetCreateCreditMailNotification(CustomerHelperTest.GetCustomer(),
                CreditMasterHelperTest.GetCreditMaster(), CreditMasterHelperTest.GetCreditMaster().Current,
                CreditHelperTest.GetCreateCreditResponse(), StoreHelperTest.GetStore(), "1002512.pdf", 2);

            Assert.NotNull(mailNotificationRequest);
            Assert.True(mailNotificationRequest.Recipients.Any());
            Assert.NotNull(mailNotificationRequest.TemplateInfo);
            Assert.True(mailNotificationRequest.TemplateInfo.TemplateValues.Any());
            Assert.Single(mailNotificationRequest.BlobStorageAttachments);
        }

        [Fact]
        public void ShouldGetPromissoryNoteInfo()
        {
            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMaster();

            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            string template = "<div>Plantilla impresión pagaré.</div>";

            PromissoryNoteInfo promissoryNoteInfo = CreditUsesCase.GetPromissoryNoteInfo(creditMaster, parameters, template, false);

            Assert.NotNull(promissoryNoteInfo);
            Assert.IsType<PromissoryNoteInfo>(promissoryNoteInfo);
        }

        [Fact]
        public void ShouldHasArrears()
        {
            DateTime calculationDate = new DateTime(2019, 09, 15);
            DateTime expirationDate = new DateTime(2019, 09, 12);
            int dayGrace = 2;

            Assert.True(CreditUsesCase.HasArrears(calculationDate, expirationDate, dayGrace));
        }

        [Fact]
        public void ShouldHasArrearsLastPaymentHiger()
        {
            DateTime calculationDate = new DateTime(2019, 09, 15);
            DateTime expirationDate = new DateTime(2019, 09, 11);
            int dayGrace = 2;

            Assert.True(CreditUsesCase.HasArrears(calculationDate, expirationDate, dayGrace));
        }

        [Fact]
        public void ShouldNotHasArrears()
        {
            DateTime calculationDate = new DateTime(2019, 09, 12);
            DateTime expirationDate = new DateTime(2019, 09, 12);
            int dayGrace = 2;

            Assert.False(CreditUsesCase.HasArrears(calculationDate, expirationDate, dayGrace));
        }

        [Fact]
        public void ShouldNotHasArrearsPaymentHiger()
        {
            DateTime calculationDate = new DateTime(2019, 09, 12);
            DateTime expirationDate = new DateTime(2019, 09, 11);
            int dayGrace = 2;

            Assert.False(CreditUsesCase.HasArrears(calculationDate, expirationDate, dayGrace));
        }

        [Fact]
        public void ShouldGetArrearsDays()
        {
            int expectedArrearsDay = 47;
            DateTime dueDate = new DateTime(2019, 09, 13);
            DateTime calculationDate = new DateTime(2019, 10, 30);

            Assert.Equal(expectedArrearsDay, CreditUsesCase.GetArrearsDays(calculationDate, dueDate));
        }

        [Fact]
        public void ShouldGetArrearsDaysHasNotArrears()
        {
            int expectedArrearsDay = 0;
            DateTime calculationDate = new DateTime(2019, 08, 30);
            DateTime dueDate = new DateTime(2019, 09, 30);

            Assert.Equal(expectedArrearsDay, CreditUsesCase.GetArrearsDays(calculationDate, dueDate));
        }

        [Fact]
        public void ShouldGetZeroArrearsDays()
        {
            int expectedArrearsDay = 0;
            DateTime dueDate = new DateTime(2019, 09, 13);
            DateTime calculationDate = new DateTime(2019, 09, 13);

            Assert.Equal(expectedArrearsDay, CreditUsesCase.GetArrearsDays(calculationDate, dueDate));
        }

        [Fact]
        public void ShouldGetPaidCreditCertificateTemplateWithoutReprint()
        {
            string template = CreditUsesCase.GetPaidCreditCertificateTemplate(
                PaidCreditCertificateTemplateHelperTest.GetGetPaidCreditCertificateTemplate(), false);

            Assert.NotNull(template);
        }

        [Fact]
        public void ShouldGetPaidCreditCertificateTemplateWithReprint()
        {
            string template = CreditUsesCase.GetPaidCreditCertificateTemplate(
                PaidCreditCertificateTemplateHelperTest.GetGetPaidCreditCertificateTemplate(), true);

            Assert.NotNull(template);
        }

        [Fact]
        public void ShouldGetCreditHistory()
        {
            List<CreditMaster> creditMasters = CreditMasterHelperTest.GetCreditMasterList();
            List<RequestCancelCredit> requestCancelCredits = RequestCancelCreditHelperTest.GetRequestCancelCreditList();
            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            List<CreditHistoryResponse> creditHistoryResponses = CreditUsesCase.CreateCreditHistory(creditMasters, requestCancelCredits, parameters);

            Assert.NotNull(creditHistoryResponses);
            Assert.NotEmpty(creditHistoryResponses);
        }

        [Fact]
        public void ShouldGetCreditHistoryWithCanceledCredits()
        {
            List<CreditMaster> creditMasters = CreditMasterHelperTest.GetCreditMasterList();
            List<CreditMaster> canceledCreditMasters = CreditMasterHelperTest.GetCreditMasterCanceledList();
            creditMasters.AddRange(canceledCreditMasters);

            List<RequestCancelCredit> requestCancelCredits = RequestCancelCreditHelperTest.GetRequestCancelCreditsFromCreditMasters(canceledCreditMasters);
            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            List<CreditHistoryResponse> creditHistoryResponses = CreditUsesCase.CreateCreditHistory(creditMasters, requestCancelCredits, parameters);

            List<CreditHistoryResponse> canceledHistoryResponses =
                creditHistoryResponses
                    .Where(history =>
                        history.StatusId == (int)Statuses.Canceled && history.CancelDate != null)
                    .ToList();

            Assert.NotNull(creditHistoryResponses);
            Assert.NotEmpty(creditHistoryResponses);
            Assert.Equal(requestCancelCredits.Count, canceledHistoryResponses.Count);
        }

        [Fact]
        public async Task ShuldCreateCreditWithTokenAlreadyUsedThrowsBusinessException()
        {
            _creditMasterRepositoryMock.Setup(mock => mock.IsDuplicatedAsync(It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<Guid>()))
                .ReturnsAsync(true);

            BusinessException exception = await Assert.ThrowsAsync<BusinessException>(() =>
                CreditUsesCase.CreateAsync(CreateCreditDomainRequestHelperTest.GetDefault(CreditHelperTest.GetCreateCreditRequest(),
                    SourceHelperTest.GetCredinetSource(), AuthMethodHelperTest.GetTokenAuthMethod(), ParameterHelperTest.GetAppParameters())));
            Assert.Equal((int)BusinessResponse.TokenAlreadyUsed, exception.code);
        }

        [Fact]
        public void ShouldCalculateTimeLimitsMonthsWithRegularFeesNumber()
        {
            int regularFees = 3;
            Customer customer = CustomerHelperTest.GetCustomer();
            Store store = StoreHelperTest.GetStoreCustomWithCustomStoreCategory("5686528", 40000M, 0.10M, "Test Store", false, AssuranceTypes.TypeA, "vendorId", 0.283200M, "Default", regularFees, 8, 20000M, 300000M, 100000);

            Assert.Equal(regularFees, CreditUsesCase.GetTimeLimitInMonths(customer, 100000M, store, 2, 300000));
        }

        [Fact]
        public void ShouldCalculateTimeLimitsMonthsWithCuotoffFeesValue()
        {
            int cutoffValue = 100000;
            decimal creditValue = 500000M;
            Customer customer = CustomerHelperTest.GetCustomer();
            Store store = StoreHelperTest.GetStoreCustomWithCustomStoreCategory("5686528", 40000M, 0.10M, "Test Store", false, AssuranceTypes.TypeA, "vendorId", 0.283200M, "Default", 2, 8, 50000M, 3000000M, cutoffValue);

            Assert.Equal(5, CreditUsesCase.GetTimeLimitInMonths(customer, creditValue, store, 2, 300000));
        }

        [Fact]
        public void ShouldCalculateTimeLimitsMonthsWithMaximumFeesNumber()
        {
            int maximumFees = 8;
            Customer customer = CustomerHelperTest.GetCustomer();
            Store store = StoreHelperTest.GetStoreCustomWithCustomStoreCategory("5686528", 40000M, 0.10M, "Test Store", false, AssuranceTypes.TypeA, "vendorId", 0.283200M, "Default", 3, maximumFees, 30000M, 30000000M, 10000);

            Assert.Equal(maximumFees, CreditUsesCase.GetTimeLimitInMonths(customer, 500000M, store, 2, 300000));
        }

        [Fact]
        public void ShouldNotCalculateTimeLimitsMonthsCauseMaximumCreditValue()
        {
            Customer customer = CustomerHelperTest.GetCustomer();
            Store store = StoreHelperTest.GetStoreCustomWithCustomStoreCategory("5686528", 40000M, 0.10M, "Test Store", false, AssuranceTypes.TypeA, "vendorId", 0.283200M, "Default", 3, 8, 30000M, 300000M, 100000);

            BusinessException exception = Assert.Throws<BusinessException>(() => CreditUsesCase.GetTimeLimitInMonths(customer, 500000M, store, 2, 300000));
            Assert.Equal((int)BusinessResponse.InvalidAmountCredit, exception.code);
        }

        [Fact]
        public void ShouldNotCalculateTimeLimitsMonthsCauseMinimumCreditValue()
        {
            Customer customer = CustomerHelperTest.GetCustomer();
            Store store = StoreHelperTest.GetStoreCustomWithCustomStoreCategory("5686528", 40000M, 0.10M, "Test Store", false, AssuranceTypes.TypeA, "vendorId", 0.283200M, "Default", 3, 8, 300000M, 3000000M, 100000);

            BusinessException exception = Assert.Throws<BusinessException>(() => CreditUsesCase.GetTimeLimitInMonths(customer, 30000M, store, 2, 300000));
            Assert.Equal((int)BusinessResponse.InvalidAmountCredit, exception.code);
        }

        [Fact]
        public async Task ShouldReponseCustomerIsDefaulterAsync()
        {
            Customer customer = CustomerHelperTest.GetCustomer();
            List<CreditMaster> creditMasterComposites = CreditMasterHelperTest.GetCreditMasterWithRequestCancelList();

            creditMasterComposites.ForEach(credit =>
                 {
                     if (credit.GetStatusId == (int)Statuses.CancelRequest)
                     {
                         CreditMasterHelperTest.AddTransactionWithDueDate(credit, TransactionTypes.Payment, credit.Store, Statuses.Active, new DateTime(2021, 01, 10), new DateTime(2021, 01, 20));
                     }
                     else
                     {
                         CreditMasterHelperTest.AddTransaction(credit, TransactionTypes.Payment, credit.Store, Statuses.Active, new DateTime(2021, 05, 20));
                     }
                 }
            );

            _creditMasterRepositoryMock.Setup(item => item.GetActiveAndCancelRequestCreditsAsync(It.IsAny<Customer>()))
                .ReturnsAsync(creditMasterComposites).Verifiable();

            var response = await CreditUsesCase.CustomerIsDefaulterAsync(customer, "1231243", 6);

            Assert.True(response);
            _creditMasterRepositoryMock.Verify(mock => mock.GetActiveAndCancelRequestCreditsAsync(It.IsAny<Customer>()), Times.Once());
        }

        [Fact]
        public async void ShouldReponseCustomerIsNotDefaulter()
        {
            Customer customer = CustomerHelperTest.GetCustomer();
            List<CreditMaster> creditMasterComposites = CreditMasterHelperTest.GetCreditMasterWithRequestCancelList();

            creditMasterComposites.ForEach(credit =>
            CreditMasterHelperTest.AddTransaction(credit, TransactionTypes.Payment, credit.Store, Statuses.Active, new DateTime(2021, 05, 20))
            );

            _creditMasterRepositoryMock.Setup(item => item.GetActiveAndCancelRequestCreditsAsync(It.IsAny<Customer>()))
                .ReturnsAsync(creditMasterComposites).Verifiable();

            var response = await CreditUsesCase.CustomerIsDefaulterAsync(customer, "1231243", 6);

            Assert.False(response);
            _creditMasterRepositoryMock.Verify(mock => mock.GetActiveAndCancelRequestCreditsAsync(It.IsAny<Customer>()), Times.Once());
        }


        /// <summary>
        /// Gets the credit details when storeis null then expect business exception store not found.
        /// </summary>
        [Fact]
        public async Task GetCreditDetailsWhenStoreisNullThenExpectBusinessExceptionStoreNotFound()
        {
            //Arrange
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            Store store = null;
            decimal creditValue = 50000;
            int frequency = (int)Frequencies.Monthly;

            SimulatedCreditRequest creditDetailDomainRequest = new SimulatedCreditRequest(store, creditValue, frequency, parameters);

            //Act
            //Assert
            var result = Assert.Throws<BusinessException>(() => CreditUsesCase.GetCreditDetails(creditDetailDomainRequest));

            Assert.Equal(result.Message, BusinessResponse.StoreNotFound.ToString());
        }

        /// <summary>
        /// Gets the credit details when credit value less than minimum fee value then expect business exception.
        /// </summary>
        [Fact]
        public async Task GetCreditDetailsWhenCreditValueLessThanMinimumFeeValueThenExpectBusinessException()
        {
            //Arrange
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            Store store = StoreHelperTest.GetStoreCustomWithCustomStoreCategory("99656586", 20000, 0.10M, "Store Test", true, AssuranceTypes.TypeA, "vendorId", effectiveAnualRate: 0, "Default", 4, 8, 60000M, 300000M, 100000);
            decimal creditValue = 50000;
            int frequency = (int)Frequencies.Monthly;

            SimulatedCreditRequest creditDetailDomainRequest = new SimulatedCreditRequest(store, creditValue, frequency, parameters);



            //Act
            //Assert
            var result = Assert.Throws<BusinessException>(() => CreditUsesCase.GetCreditDetails(creditDetailDomainRequest));
            Assert.Equal(result.Message, 60000M.Round(creditDetailDomainRequest.DecimalNumbersRound).ToString() + "-" + 300000M.Round(creditDetailDomainRequest.DecimalNumbersRound).ToString());
        }

        /// <summary>
        /// Gets the credit details when months greater than limit months then expect business exception months number not valid.
        /// </summary>
        [Fact]
        public async Task GetCreditDetailsWhenMonthsGreaterThanLimitMonthsThenExpectBusinessExceptionMonthsNumberNotValid()
        {
            //Arrange
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            Store store = StoreHelperTest.GetStoreCustomWithCustomStoreCategory("99656586", 20000, 0.10M, "Store Test", true, AssuranceTypes.TypeA, "vendorId", effectiveAnualRate: 0, "Default", 4, 8, 6000M, 60000M, 10000);
            decimal creditValue = 50000;
            int frequency = (int)Frequencies.Monthly;

            SimulatedCreditRequest creditDetailDomainRequest = new SimulatedCreditRequest(store, creditValue, frequency, parameters);
            creditDetailDomainRequest.SetFees(18);



            //Act
            //Assert
            var result = Assert.Throws<BusinessException>(() => CreditUsesCase.GetCreditDetails(creditDetailDomainRequest));

            Assert.Equal(result.Message, BusinessResponse.MonthsNumberNotValid.ToString());
        }

        /// <summary>
        /// Gets the credit details when all data is right then expect credit detail response.
        /// </summary>
        [Fact]
        public async Task GetCreditDetailsWhenAllDataIsRightThenExpectCreditDetailResponse()
        {
            //Arrange
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            Store store = StoreHelperTest.GetStoreCustomWithCustomStoreCategory("99656586", 20000, 0.10M, "Store Test", true, AssuranceTypes.TypeA, "vendorId", effectiveAnualRate: 0, "Default", 4, 8, 6000M, 60000M, 10000);
            decimal creditValue = 50000;
            int frequency = (int)Frequencies.Monthly;
            int months = 4;
            CreditDetailResponse creditDetail = CreditHelperTest.GetCreditDetails();

            SimulatedCreditRequest creditDetailDomainRequest = new SimulatedCreditRequest(store, creditValue, frequency, parameters);
            creditDetailDomainRequest.SetFees(months);

            _creditsOperationsMock.Setup(simulator => simulator.CreateCreditDetails(It.Is<GeneralCreditDetailDomainRequest>(Simulation =>
                                                                                                                             Simulation.Store == store &&
                                                                                                                             Simulation.CreditValue == creditValue &&
                                                                                                                             Simulation.Frequency == (Frequencies)frequency &&
                                                                                                                             Simulation.Fees == months))).Returns(creditDetail).Verifiable();

            //Act            
            var result = CreditUsesCase.GetCreditDetails(creditDetailDomainRequest);

            //Assert
            _creditsOperationsMock.VerifyAll();
            Assert.Equal(creditDetail, result);
        }

        /// <summary>
        /// Gets the store limit in months when credit value less than minimum fee value then expect business exception.
        /// </summary>
        [Fact]
        public async Task GetStoreLimitInMonthsWhenCreditValueLessThanMinimumFeeValueThenExpectBusinessException()
        {
            //Arrange
            decimal minimumValue = 60000M;
            decimal maximumCreditValue = 70000M;
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            Store store = StoreHelperTest.GetStoreCustomWithCustomStoreCategory("99656586", 20000, 0.10M, "Store Test", true, AssuranceTypes.TypeA, "vendorId", effectiveAnualRate: 0, "Default", 4, 8, minimumValue, maximumCreditValue, 10000);
            decimal creditValue = 50000;
            int frequency = (int)Frequencies.Monthly;

            GeneralCreditDetailDomainRequest creditDetailDomainRequest = new GeneralCreditDetailDomainRequest(store, creditValue, frequency, parameters);

            //Act            
            //Assert
            var result = Assert.Throws<BusinessException>(() => CreditUsesCase.GetTimeLimitInMonths(creditValue, store, creditDetailDomainRequest.DecimalNumbersRound));
            Assert.Equal(result.Message, minimumValue.Round(creditDetailDomainRequest.DecimalNumbersRound).ToString() + "-" + maximumCreditValue.Round(creditDetailDomainRequest.DecimalNumbersRound).ToString());
        }

        [Fact]
        public async Task GetStoreLimitInMonthsWhen()
        {
            //Arrange
            int expectedMonths = 5;
            decimal minimumValue = 6000M;
            decimal maximumCreditValue = 70000M;
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            Store store = StoreHelperTest.GetStoreCustomWithCustomStoreCategory("99656586", 20000, 0.10M, "Store Test", true, AssuranceTypes.TypeA, "vendorId", effectiveAnualRate: 0, "Default", 4, 8, minimumValue, maximumCreditValue, 10000);
            decimal creditValue = 50000;
            int frequency = (int)Frequencies.Monthly;

            GeneralCreditDetailDomainRequest creditDetailDomainRequest = new GeneralCreditDetailDomainRequest(store, creditValue, frequency, parameters);

            //Act
            var result = CreditUsesCase.GetTimeLimitInMonths(creditValue, store, creditDetailDomainRequest.DecimalNumbersRound);
            //Assert            
            Assert.Equal(expectedMonths, result);
        }

        [Fact]
        public async Task DeleteCreditValidateWithValidCreditReturnFalse()
        {
            //Arrange
            CreditMaster creditMasterComposites = CreditMasterHelperTest.GetCreditMaster();

            bool result = await CreditUsesCase.DeleteAsync(creditMasterComposites);

            //Act
            _creditMasterRepositoryMock.Verify(mock => mock.DeleteAsync(It.IsAny<CreditMaster>(), It.IsAny<Transaction>()),
                Times.Once);

            //Assert            
            Assert.False(result);
        }



        [Fact]
        public async Task DeleteCreditValidateWithValidCreditReturnTrue()
        {
            //Arrange
            CreditMaster creditMasterComposites = CreditMasterHelperTest.GetCreditMaster();
            _creditMasterRepositoryMock.Setup(mock => mock.DeleteAsync(creditMasterComposites, It.IsAny<Transaction>()));

            bool result = await CreditUsesCase.DeleteAsync(creditMasterComposites);

            //Act
            _creditMasterRepositoryMock.Verify(mock => mock.DeleteAsync(It.IsAny<CreditMaster>(), It.IsAny<Transaction>()),
                Times.Once);

            //Assert            
            Assert.True(result);
        }
    }
}