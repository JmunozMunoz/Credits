using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Helper.Test.Model;
using Xunit;

namespace Sc.Credits.Domain.UseCase.Tests.Credits
{
    public class CreditModelsTest
    {
        [Fact]
        public void TestActivePendingCancellationCreditResponse()
        {
            Assert.True(ModelHelperTest.TestModel<ActivePendingCancellationCreditResponse>());
        }

        [Fact]
        public void TestActivePendingCancellationPaymentResponse()
        {
            Assert.True(ModelHelperTest.TestModel<ActivePendingCancellationPaymentResponse>());
        }

        [Fact]
        public void TestAmortizationScheduleResponse()
        {
            Assert.True(ModelHelperTest.TestModel<AmortizationScheduleResponse>());
        }

        [Fact]
        public void TestCalculatedQuery()
        {
            Assert.True(ModelHelperTest.TestModel<CalculatedQuery>());
        }

        [Fact]
        public void TestCancelCreditDetailResponse()
        {
            Assert.True(ModelHelperTest.TestModel<CancelCreditDetailResponse>());
        }

        [Fact]
        public void TestCancelCreditPaymentResponse()
        {
            Assert.True(ModelHelperTest.TestModel<CancelCreditPaymentResponse>());
        }

        [Fact]
        public void TestCancelCreditResponse()
        {
            Assert.True(ModelHelperTest.TestModel<CancelCreditResponse>());
        }

        [Fact]
        public void TestCancelPaymentDetailResponse()
        {
            Assert.True(ModelHelperTest.TestModel<CancelPaymentDetailResponse>());
        }

        [Fact]
        public void TestCancelPaymentResponse()
        {
            Assert.True(ModelHelperTest.TestModel<CancelPaymentResponse>());
        }

        [Fact]
        public void TestCreateCreditResponse()
        {
            CreateCreditResponse createCreditResponse = ModelHelperTest.InstanceModel<CreateCreditResponse>();
            createCreditResponse.CreditMaster = CreditMasterHelperTest.GetCreditMaster();
            Assert.True(ModelHelperTest.ValidateModel(createCreditResponse));
        }

        [Fact]
        public void TestCustomerCreditLimitResponse()
        {
            Assert.True(ModelHelperTest.TestModel<CustomerCreditLimitResponse>());
        }

        [Fact]
        public void TestCreditScCodeRequest()
        {
            Assert.True(ModelHelperTest.TestModel<CreditScCodeRequest>());
        }

        [Fact]
        public void TestCreditStatus()
        {
            Assert.True(ModelHelperTest.TestModel<CreditStatus>());
        }

        [Fact]
        public void TestCreditStatusResponse()
        {
            Assert.True(ModelHelperTest.TestModel<CreditStatusResponse>());
        }

        [Fact]
        public void TestCreditTokenRequest()
        {
            Assert.True(ModelHelperTest.TestModel<CreditTokenRequest>());
        }

        [Fact]
        public void TestCurrentAmortizationScheduleFee()
        {
            Assert.True(ModelHelperTest.TestModel<CurrentAmortizationScheduleFee>());
        }

        [Fact]
        public void TestpaymentCreditMultipleRequest()
        {
            Assert.True(ModelHelperTest.TestModel<PaymentCreditMultipleRequest>());
        }

        [Fact]
        public void TestpaymentCreditResponse()
        {
            PaymentCreditResponse paymentCreditResponse = ModelHelperTest.InstanceModel<PaymentCreditResponse>();
            paymentCreditResponse.CreditMaster = CreditMasterHelperTest.GetCreditMaster();
            paymentCreditResponse.Credit = paymentCreditResponse.CreditMaster.Current;
            paymentCreditResponse.Store = StoreHelperTest.GetStore();
            Assert.True(ModelHelperTest.ValidateModel(paymentCreditResponse));
        }

        [Fact]
        public void TestRequestStatus()
        {
            Assert.NotNull(new RequestStatus("Test").SetId(1));
        }
    }
}