using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Moq;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Builders;
using Sc.Credits.Domain.Model.Credits.Gateway;
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
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace Sc.Credits.Domain.UseCase.Tests.Credits
{
    public class CreditPaymentUsesCaseTest
    {
        private readonly Mock<ICreditUsesCase> _creditUseCaseMock = new Mock<ICreditUsesCase>();
        private readonly Mock<ICreditMasterRepository> _creditMasterRepositoryMock = new Mock<ICreditMasterRepository>();
        private readonly Mock<ICustomerRepository> _customerRepositoryMock = new Mock<ICustomerRepository>();
        private readonly Mock<ISequenceRepository> _sequenceRepositoryMock = new Mock<ISequenceRepository>();
        private readonly Mock<ISettings<CredinetAppSettings>> _appSettingsMock = new Mock<ISettings<CredinetAppSettings>>();

        public ICreditPaymentUsesCase CreditPaymentUsesCase=>
            new CreditPaymentUsesCase(_creditUseCaseMock.Object,
                _creditMasterRepositoryMock.Object,
                _customerRepositoryMock.Object,
                _sequenceRepositoryMock.Object,
                _appSettingsMock.Object);


        public CreditPaymentUsesCaseTest()
        {
            _appSettingsMock.Setup(mock => mock.Get())
                .Returns(CredinetAppSettingsHelperTest.GetCredinetAppSettings());
        }

        [Fact]
        public void ShouldCalculateInterestPaymentValue()
        {
            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            decimal creditValue = 768000;
            DateTime initialDate = new DateTime(2020, 1, 31);
            decimal feeValue = 94316.0M;
            decimal interestRate = 0.0204985209860811M;
            int frequency = 30;
            int fees = 9;
            decimal downPayment = 0;
            decimal assuranceValue = 76800;
            decimal assuranceFeeValue = 8533.0M;
            decimal assuranceTotalFeeValue = 10154.0M;
            int decimalNumbersRound = 0;

            DateTime calculationDate = new DateTime(2020, 05, 30);
            DateTime lastPaymentDate = new DateTime(2020, 02, 07);
            decimal balance = 609243M;
            decimal assuranceBalance = 60927M;

            decimal previousInterest = 0;
            decimal expectedInterestPaymentValue = 45991.89M;

            AmortizationScheduleResponse amortizationSchedule = creditsUsesCaseTest.CreditUsesCase.GetOriginalAmortizationSchedule(
                    AmortizationScheduleHelperTest.GetAmortizationScheduleRequest(creditValue, initialDate, feeValue, interestRate, frequency, fees,
                        downPayment, assuranceValue, assuranceFeeValue, assuranceTotalFeeValue),
                    decimalNumbersRound);

            PaymentMatrix paymentMatrix = PaymentMatrixBuilder.CreateBuilder()
               .BasicInfo(amortizationSchedule, balance, assuranceBalance)
               .DateInfo(calculationDate, lastPaymentDate)
               .CreditParametersInfo(0.2757M, 0.283243M, 3)
               .Build();

            decimal actualInterestPaymentValue = CreditPaymentUsesCase.GetInterestPayment(paymentMatrix, previousInterest);

            Assert.Equal(expectedInterestPaymentValue, Math.Round(actualInterestPaymentValue, 2));
        }

        [Fact]
        public void ShouldCalculateInterestPaymentValue_PreviousInterestGreaterThanPayableInterest()
        {
            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            decimal creditValue = 768000;
            DateTime initialDate = new DateTime(2020, 1, 31);
            decimal feeValue = 94316.0M;
            decimal interestRate = 0.0204985209860811M;
            int frequency = 30;
            int fees = 9;
            decimal downPayment = 0;
            decimal assuranceValue = 76800;
            decimal assuranceFeeValue = 8533.0M;
            decimal assuranceTotalFeeValue = 10154.0M;
            int decimalNumbersRound = 0;

            DateTime calculationDate = new DateTime(2020, 05, 30);
            DateTime lastPaymentDate = new DateTime(2020, 02, 07);
            decimal balance = 609243M;
            decimal assuranceBalance = 60927M;

            decimal previousInterest = 50000;

            AmortizationScheduleResponse amortizationSchedule = creditsUsesCaseTest.CreditUsesCase.GetOriginalAmortizationSchedule(
                    AmortizationScheduleHelperTest.GetAmortizationScheduleRequest(creditValue, initialDate, feeValue, interestRate, frequency, fees,
                        downPayment, assuranceValue, assuranceFeeValue, assuranceTotalFeeValue),
                    decimalNumbersRound);

            PaymentMatrix paymentMatrix = PaymentMatrixBuilder.CreateBuilder()
               .BasicInfo(amortizationSchedule, balance, assuranceBalance)
               .DateInfo(calculationDate, lastPaymentDate)
               .CreditParametersInfo(0.2757M, 0.283243M, 3)
               .Build();

            decimal actualInterestPaymentValue = CreditPaymentUsesCase.GetInterestPayment(paymentMatrix, previousInterest);

            Assert.Equal(0M, Math.Round(actualInterestPaymentValue, 2));
        }

        [Fact]
        public void ShouldGetMinimumPayment()
        {
            PaymentFeesResponse paymentFeesHelperTest = PaymentFeesHelperTest.GetPaymentFeesResponse();
            decimal minimumPayment = CreditPaymentUsesCase.GetMinimumPayment(
                paymentFee: paymentFeesHelperTest.PaymentFees,
                arrearsFees: paymentFeesHelperTest.ArrearsFees,
                totalPayment: 216240.24M,
                updatedPaymentPlanValue: 0.0M,
                hasUpdatedPaymentPlan: false,
                maximumUpdatedPaymentPlanGapRate: 0.2M
            );

            Assert.Equal(expected: 112430.24M, minimumPayment);
        }

        [Fact]
        public void ShouldGetMinimumPayment_NoArrearsFees()
        {
            PaymentFeesResponse paymentFeesResponse = PaymentFeesHelperTest.GetPaymentFeesResponse();
            paymentFeesResponse.ArrearsFees = 0;

            decimal minimumPayment = CreditPaymentUsesCase.GetMinimumPayment(
                paymentFee: paymentFeesResponse.PaymentFees,
                arrearsFees: paymentFeesResponse.ArrearsFees,
                totalPayment: 216240.24M,
                updatedPaymentPlanValue: 0.0M,
                hasUpdatedPaymentPlan: false,
                maximumUpdatedPaymentPlanGapRate: 0.2M
            );

            Assert.Equal(expected: 52430.24M, minimumPayment);
        }

        [Fact]
        public void ShouldGetMinimumPaymentWithUpdatedPaymentPlan()
        {
            decimal updatedPaymentPlanValue = 32000;
            PaymentFeesResponse paymentFeesHelperTest = PaymentFeesHelperTest.GetPaymentFeesResponse();

            decimal minimumPayment = CreditPaymentUsesCase.GetMinimumPayment(
                paymentFee: paymentFeesHelperTest.PaymentFees,
                arrearsFees: paymentFeesHelperTest.ArrearsFees,
                totalPayment: 216240.24M,
                updatedPaymentPlanValue,
                hasUpdatedPaymentPlan: true,
                maximumUpdatedPaymentPlanGapRate: 0.2M
            );

            Assert.Equal(expected: updatedPaymentPlanValue, minimumPayment);
        }

        [Fact]
        public void GetMinimumPayment_WithUpdatedPaymentPlanAndGapPercentageExcceded_ShouldBeTotalPayment()
        {
            decimal updatedPaymentPlanValue = 200000;
            decimal totalPayment = 216240.24M;
            PaymentFeesResponse paymentFeesHelperTest = PaymentFeesHelperTest.GetPaymentFeesResponse();

            decimal minimumPayment = CreditPaymentUsesCase.GetMinimumPayment(
                paymentFee: paymentFeesHelperTest.PaymentFees,
                arrearsFees: paymentFeesHelperTest.ArrearsFees,
                totalPayment,
                updatedPaymentPlanValue,
                hasUpdatedPaymentPlan: true,
                maximumUpdatedPaymentPlanGapRate: 0.1M
            );

            Assert.Equal(expected: totalPayment, minimumPayment);
        }

        [Fact]
        public void ShouldGetMinimumPaymentWithUpdatedPaymentPlan_UpdatedPaymentPlanExceedsTotalPayment()
        {
            decimal totalPayment = 216240.24M;
            PaymentFeesResponse paymentFeesHelperTest = PaymentFeesHelperTest.GetPaymentFeesResponse();

            decimal minimumPayment = CreditPaymentUsesCase.GetMinimumPayment(
                paymentFee: paymentFeesHelperTest.PaymentFees,
                arrearsFees: paymentFeesHelperTest.ArrearsFees,
                totalPayment,
                updatedPaymentPlanValue: 220000.0M,
                hasUpdatedPaymentPlan: true,
                maximumUpdatedPaymentPlanGapRate: 0.2M
            );

            Assert.Equal(expected: totalPayment, minimumPayment);
        }

        [Fact]
        public void ShouldGetAssuranceOrdinaryPaymentWithEstimatePendingPaymentSum()
        {
            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            decimal creditValue = 200000;
            decimal creditValueBalance = 100000;
            DateTime initialDate = new DateTime(2019, 5, 21);
            decimal maximumPaymentAdjustmentResidue = 100;
            decimal feeValue = 26117.0M;
            decimal interestRate = 0.0343660831319166M;
            int frequency = 30;
            int fees = 8;
            decimal downPayment = 20000;
            decimal assuranceValue = 20000;
            decimal assuranceFeeValue = 2222.0M;
            decimal assuranceTotalFeeValue = 2644.0M;
            int decimalNumbersRound = 1;
            decimal assuranceBalance = 13000;
            decimal pendingPaymentValue = 2000;
            decimal paymentValue = 3000;
            decimal interestValuePayment = 1000;
            decimal arrearsValuePayment = 0;
            decimal chargeValue = 0;
            int arrearsGracePeriod = 3;
            decimal totalPayment = 200000;

            AmortizationScheduleResponse amortizationSchedule = creditsUsesCaseTest.CreditUsesCase.GetOriginalAmortizationSchedule(
                AmortizationScheduleHelperTest.GetAmortizationScheduleRequest(creditValue, initialDate, feeValue, interestRate, frequency, fees,
                    downPayment, assuranceValue, assuranceFeeValue, assuranceTotalFeeValue),
                decimalNumbersRound);

            PaymentMatrix paymentMatrix = PaymentMatrixBuilder.CreateBuilder()
                .BasicInfo(amortizationSchedule, creditValueBalance, assuranceBalance)
                .DateInfo(lastPaymentDate: new DateTime(), calculationDate: new DateTime())
                .CreditParametersInfo(effectiveAnnualRate: 0M, arrearsEffectiveAnnualRate: 0M, arrearsGracePeriod: 3)
                .Build();

            decimal assurancePayment = CreditPaymentUsesCase.GetAssuranceOrdinaryPayment(paymentMatrix, pendingPaymentValue, amortizationSchedule,
                creditValueBalance, maximumPaymentAdjustmentResidue, DateTime.Now, paymentValue, interestValuePayment, arrearsValuePayment,
                chargeValue, arrearsGracePeriod, totalPayment);

            Assert.Equal(2000, assurancePayment);
        }

        [Fact]
        public void ShouldGetAssuranceOrdinaryPaymentWithAssuranceNextPaymentFeeSum()
        {
            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            decimal creditValue = 200000;
            decimal creditValueBalance = 100000;
            DateTime initialDate = new DateTime(2019, 5, 21);
            decimal maximumPaymentAdjustmentResidue = 100;
            decimal feeValue = 26117.0M;
            decimal interestRate = 0.0343660831319166M;
            int frequency = 30;
            int fees = 8;
            decimal downPayment = 20000;
            decimal assuranceValue = 20000;
            decimal assuranceFeeValue = 2222.0M;
            decimal assuranceTotalFeeValue = 2644.0M;
            int decimalNumbersRound = 1;
            decimal assuranceBalance = 12000;
            decimal pendingPaymentValue = 20000;
            decimal paymentValue = 25000;
            decimal interestValuePayment = 5000;
            decimal arrearsValuePayment = 0;
            decimal chargeValue = 0;
            int arrearsGracePeriod = 3;
            decimal totalPayment = 200000;

            AmortizationScheduleResponse amortizationSchedule = creditsUsesCaseTest.CreditUsesCase.GetOriginalAmortizationSchedule(
                AmortizationScheduleHelperTest.GetAmortizationScheduleRequest(creditValue, initialDate, feeValue, interestRate, frequency, fees,
                    downPayment, assuranceValue, assuranceFeeValue, assuranceTotalFeeValue),
                decimalNumbersRound);

            PaymentMatrix paymentMatrix = PaymentMatrixBuilder.CreateBuilder()
                .BasicInfo(amortizationSchedule, creditValueBalance, assuranceBalance)
                .DateInfo(lastPaymentDate: new DateTime(), calculationDate: new DateTime())
                .CreditParametersInfo(effectiveAnnualRate: 0M, arrearsEffectiveAnnualRate: 0M, arrearsGracePeriod: 3)
                .Build();

            decimal assurancePayment = CreditPaymentUsesCase.GetAssuranceOrdinaryPayment(paymentMatrix, pendingPaymentValue, amortizationSchedule,
                creditValueBalance, maximumPaymentAdjustmentResidue, DateTime.Now, paymentValue, interestValuePayment, arrearsValuePayment,
                chargeValue, arrearsGracePeriod, totalPayment);

            Assert.Equal(4066.0M, assurancePayment);
        }

        [Fact]
        public void ShouldGetAssuranceOrdinaryPaymentWithOnlyLastPendingFeePayment()
        {
            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            decimal creditValue = 200000;
            decimal creditValueBalance = 100000;
            DateTime initialDate = new DateTime(2019, 5, 21);
            decimal maximumPaymentAdjustmentResidue = 100;
            decimal feeValue = 26117.0M;
            decimal interestRate = 0.0343660831319166M;
            int frequency = 30;
            int fees = 8;
            decimal downPayment = 20000;
            decimal assuranceValue = 20000;
            decimal assuranceFeeValue = 2222.0M;
            decimal assuranceTotalFeeValue = 2644.0M;
            int decimalNumbersRound = 1;
            decimal assuranceBalance = 12000;
            decimal pendingPaymentValue = 1422.0M;
            decimal paymentValue = 2000;
            decimal interestValuePayment = 1000;
            decimal arrearsValuePayment = 0;
            decimal chargeValue = 0;
            int arrearsGracePeriod = 3;
            decimal totalPayment = 200000;

            AmortizationScheduleResponse amortizationSchedule = creditsUsesCaseTest.CreditUsesCase.GetOriginalAmortizationSchedule(
                AmortizationScheduleHelperTest.GetAmortizationScheduleRequest(creditValue, initialDate, feeValue, interestRate, frequency, fees,
                    downPayment, assuranceValue, assuranceFeeValue, assuranceTotalFeeValue),
                decimalNumbersRound);

            PaymentMatrix paymentMatrix = PaymentMatrixBuilder.CreateBuilder()
                .BasicInfo(amortizationSchedule, creditValueBalance, assuranceBalance)
                .DateInfo(lastPaymentDate: new DateTime(), calculationDate: new DateTime())
                .CreditParametersInfo(effectiveAnnualRate: 0M, arrearsEffectiveAnnualRate: 0M, arrearsGracePeriod: 3)
                .Build();

            decimal assurancePayment = CreditPaymentUsesCase.GetAssuranceOrdinaryPayment(paymentMatrix, pendingPaymentValue, amortizationSchedule,
                creditValueBalance, maximumPaymentAdjustmentResidue, DateTime.Now, paymentValue, interestValuePayment, arrearsValuePayment,
                chargeValue, arrearsGracePeriod, totalPayment);

            Assert.Equal(1422.0M, assurancePayment);
        }

        [Fact]
        public void ShouldGetAssuranceOrdinaryPaymentWithEstimatePendingPaymentMinorToLastPendingFee()
        {
            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            decimal creditValue = 200000;
            decimal creditValueBalance = 100000;
            DateTime initialDate = new DateTime(2019, 5, 21);
            decimal maximumPaymentAdjustmentResidue = 100;
            decimal feeValue = 26117.0M;
            decimal interestRate = 0.0343660831319166M;
            int frequency = 30;
            int fees = 8;
            decimal downPayment = 20000;
            decimal assuranceValue = 20000;
            decimal assuranceFeeValue = 2222.0M;
            decimal assuranceTotalFeeValue = 2644.0M;
            int decimalNumbersRound = 1;
            decimal assuranceBalance = 12000;
            decimal pendingPaymentValue = 1300.0M;
            decimal paymentValue = 2000;
            decimal interestValuePayment = 1000;
            decimal arrearsValuePayment = 0;
            decimal chargeValue = 0;
            int arrearsGracePeriod = 3;
            decimal totalPayment = 200000;

            AmortizationScheduleResponse amortizationSchedule = creditsUsesCaseTest.CreditUsesCase.GetOriginalAmortizationSchedule(
                AmortizationScheduleHelperTest.GetAmortizationScheduleRequest(creditValue, initialDate, feeValue, interestRate, frequency, fees,
                    downPayment, assuranceValue, assuranceFeeValue, assuranceTotalFeeValue),
                decimalNumbersRound);

            PaymentMatrix paymentMatrix = PaymentMatrixBuilder.CreateBuilder()
                .BasicInfo(amortizationSchedule, creditValueBalance, assuranceBalance)
                .DateInfo(lastPaymentDate: new DateTime(), calculationDate: new DateTime())
                .CreditParametersInfo(effectiveAnnualRate: 0M, arrearsEffectiveAnnualRate: 0M, arrearsGracePeriod: 3)
                .Build();

            decimal assurancePayment = CreditPaymentUsesCase.GetAssuranceOrdinaryPayment(paymentMatrix, pendingPaymentValue, amortizationSchedule,
                creditValueBalance, maximumPaymentAdjustmentResidue, DateTime.Now, paymentValue, interestValuePayment, arrearsValuePayment,
                chargeValue, arrearsGracePeriod, totalPayment);

            Assert.Equal(1300, assurancePayment);
        }

        [Fact]
        public void ShouldGetAssuranceOrdinaryPaymentWithTwoPartialPaymentFees()
        {
            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            decimal creditValue = 200000;
            decimal creditValueBalance = 100000;
            DateTime initialDate = new DateTime(2019, 5, 21);
            decimal maximumPaymentAdjustmentResidue = 100;
            decimal feeValue = 26117.0M;
            decimal interestRate = 0.0343660831319166M;
            int frequency = 30;
            int fees = 8;
            decimal downPayment = 20000;
            decimal assuranceValue = 20000;
            decimal assuranceFeeValue = 2222.0M;
            decimal assuranceTotalFeeValue = 2644.0M;
            int decimalNumbersRound = 1;
            decimal assuranceBalance = 12000;
            decimal pendingPaymentValue = 7851;
            decimal paymentValue = 9000;
            decimal interestValuePayment = 2000;
            decimal arrearsValuePayment = 0;
            decimal chargeValue = 0;
            int arrearsGracePeriod = 3;
            decimal totalPayment = 200000;

            AmortizationScheduleResponse amortizationSchedule = creditsUsesCaseTest.CreditUsesCase.GetOriginalAmortizationSchedule(
                AmortizationScheduleHelperTest.GetAmortizationScheduleRequest(creditValue, initialDate, feeValue, interestRate, frequency, fees,
                    downPayment, assuranceValue, assuranceFeeValue, assuranceTotalFeeValue),
                decimalNumbersRound);

            PaymentMatrix paymentMatrix = PaymentMatrixBuilder.CreateBuilder()
                .BasicInfo(amortizationSchedule, creditValueBalance, assuranceBalance)
                .DateInfo(lastPaymentDate: new DateTime(), calculationDate: new DateTime())
                .CreditParametersInfo(effectiveAnnualRate: 0M, arrearsEffectiveAnnualRate: 0M, arrearsGracePeriod: 3)
                .Build();

            decimal assurancePayment = CreditPaymentUsesCase.GetAssuranceOrdinaryPayment(paymentMatrix, pendingPaymentValue, amortizationSchedule,
                creditValueBalance, maximumPaymentAdjustmentResidue, DateTime.Now, paymentValue, interestValuePayment, arrearsValuePayment,
                chargeValue, arrearsGracePeriod, totalPayment);

            Assert.Equal(3921.8M, assurancePayment);
        }

        [Fact]
        public void ShouldGetAssuranceOrdinaryPaymentWithCompleteThreeFeesPayment()
        {
            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            decimal creditValue = 200000;
            decimal creditValueBalance = 100000;
            DateTime initialDate = new DateTime(2019, 5, 21);
            decimal maximumPaymentAdjustmentResidue = 100;
            decimal feeValue = 26117.0M;
            decimal interestRate = 0.0343660831319166M;
            int frequency = 30;
            int fees = 8;
            decimal downPayment = 20000;
            decimal assuranceValue = 20000;
            decimal assuranceFeeValue = 2222.0M;
            decimal assuranceTotalFeeValue = 2644.0M;
            int decimalNumbersRound = 1;
            decimal assuranceBalance = 12000;
            decimal pendingPaymentValue = 57054M;
            decimal paymentValue = 60000;
            decimal interestValuePayment = 1000;
            decimal arrearsValuePayment = 0;
            decimal chargeValue = 0;
            int arrearsGracePeriod = 3;
            decimal totalPayment = 200000;

            AmortizationScheduleResponse amortizationSchedule = creditsUsesCaseTest.CreditUsesCase.GetOriginalAmortizationSchedule(
                AmortizationScheduleHelperTest.GetAmortizationScheduleRequest(creditValue, initialDate, feeValue, interestRate, frequency, fees,
                    downPayment, assuranceValue, assuranceFeeValue, assuranceTotalFeeValue),
                decimalNumbersRound);

            PaymentMatrix paymentMatrix = PaymentMatrixBuilder.CreateBuilder()
                .BasicInfo(amortizationSchedule, creditValueBalance, assuranceBalance)
                .DateInfo(lastPaymentDate: new DateTime(), calculationDate: new DateTime())
                .CreditParametersInfo(effectiveAnnualRate: 0M, arrearsEffectiveAnnualRate: 0M, arrearsGracePeriod: 3)
                .Build();

            decimal assurancePayment = CreditPaymentUsesCase.GetAssuranceOrdinaryPayment(paymentMatrix, pendingPaymentValue, amortizationSchedule,
                creditValueBalance, maximumPaymentAdjustmentResidue, DateTime.Now, paymentValue, interestValuePayment, arrearsValuePayment,
                chargeValue, arrearsGracePeriod, totalPayment);

            Assert.Equal(6710M, assurancePayment);
        }

        [Fact]
        public void ShouldGetAssuranceOrdinaryPaymentWithAssuranceBalancePayment()
        {
            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            decimal creditValue = 200000;
            decimal creditValueBalance = 100000;
            DateTime initialDate = new DateTime(2019, 5, 21);
            decimal maximumPaymentAdjustmentResidue = 100;
            decimal feeValue = 26117.0M;
            decimal interestRate = 0.0343660831319166M;
            int frequency = 30;
            int fees = 8;
            decimal downPayment = 20000;
            decimal assuranceValue = 20000;
            decimal assuranceFeeValue = 2222.0M;
            decimal assuranceTotalFeeValue = 2644.0M;
            int decimalNumbersRound = 1;
            decimal assuranceBalance = 12000;
            decimal pendingPaymentValue = 120000;
            decimal paymentValue = 125000;
            decimal interestValuePayment = 1000;
            decimal arrearsValuePayment = 0;
            decimal chargeValue = 0;
            int arrearsGracePeriod = 3;
            decimal totalPayment = 200000;

            AmortizationScheduleResponse amortizationSchedule = creditsUsesCaseTest.CreditUsesCase.GetOriginalAmortizationSchedule(
                AmortizationScheduleHelperTest.GetAmortizationScheduleRequest(creditValue, initialDate, feeValue, interestRate, frequency, fees,
                    downPayment, assuranceValue, assuranceFeeValue, assuranceTotalFeeValue),
                decimalNumbersRound);

            PaymentMatrix paymentMatrix = PaymentMatrixBuilder.CreateBuilder()
                .BasicInfo(amortizationSchedule, creditValueBalance, assuranceBalance)
                .DateInfo(lastPaymentDate: new DateTime(), calculationDate: new DateTime())
                .CreditParametersInfo(effectiveAnnualRate: 0M, arrearsEffectiveAnnualRate: 0M, arrearsGracePeriod: 3)
                .Build();

            decimal assurancePayment = CreditPaymentUsesCase.GetAssuranceOrdinaryPayment(paymentMatrix, pendingPaymentValue, amortizationSchedule,
                creditValueBalance, maximumPaymentAdjustmentResidue, DateTime.Now, paymentValue, interestValuePayment, arrearsValuePayment,
                chargeValue, arrearsGracePeriod, totalPayment);

            Assert.Equal(12000, assurancePayment);
        }

        [Fact]
        public void ShouldGetAssuranceOrdinaryPaymentWithTotalPayment()
        {
            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            decimal creditValue = 200000;
            decimal creditValueBalance = 100000;
            DateTime initialDate = new DateTime(2018, 5, 21);
            decimal maximumPaymentAdjustmentResidue = 100;
            decimal feeValue = 26117.0M;
            decimal interestRate = 0.0343660831319166M;
            int frequency = 30;
            int fees = 8;
            decimal downPayment = 20000;
            decimal assuranceValue = 20000;
            decimal assuranceFeeValue = 2222.0M;
            decimal assuranceTotalFeeValue = 2644.0M;
            int decimalNumbersRound = 1;
            decimal assuranceBalance = 12000;
            decimal pendingPaymentValue = 120000;
            decimal paymentValue = 122000.0M;
            decimal interestValuePayment = 10000;
            decimal arrearsValuePayment = 0;
            decimal chargeValue = 0;
            int arrearsGracePeriod = 3;
            decimal totalPayment = 200000;

            AmortizationScheduleResponse amortizationSchedule = creditsUsesCaseTest.CreditUsesCase.GetOriginalAmortizationSchedule(
                AmortizationScheduleHelperTest.GetAmortizationScheduleRequest(creditValue, initialDate, feeValue, interestRate, frequency, fees,
                    downPayment, assuranceValue, assuranceFeeValue, assuranceTotalFeeValue),
                decimalNumbersRound);

            PaymentMatrix paymentMatrix = PaymentMatrixBuilder.CreateBuilder()
                .BasicInfo(amortizationSchedule, creditValueBalance, assuranceBalance)
                .DateInfo(lastPaymentDate: new DateTime(), calculationDate: new DateTime())
                .CreditParametersInfo(effectiveAnnualRate: 0M, arrearsEffectiveAnnualRate: 0M, arrearsGracePeriod: 3)
                .Build();

            decimal assurancePayment = CreditPaymentUsesCase.GetAssuranceOrdinaryPayment(paymentMatrix, pendingPaymentValue, amortizationSchedule,
                creditValueBalance, maximumPaymentAdjustmentResidue, DateTime.Now, paymentValue, interestValuePayment, arrearsValuePayment,
                chargeValue, arrearsGracePeriod, totalPayment);

            Assert.Equal(12000, assurancePayment);
        }

        [Fact]
        public void ShouldGetArrearsPaymentWithoutCharges()
        {
            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            decimal creditValue = 768000;
            DateTime initialDate = new DateTime(2020, 1, 31);
            decimal feeValue = 94316.0M;
            decimal interestRate = 0.0204985209860811M;
            int frequency = 30;
            int fees = 9;
            decimal downPayment = 0;
            decimal assuranceValue = 76800;
            decimal assuranceFeeValue = 8533.0M;
            decimal assuranceTotalFeeValue = 10154.0M;
            int decimalNumbersRound = 0;

            DateTime calculationDate = new DateTime(2020, 05, 30);
            DateTime lastPaymentDate = new DateTime(2020, 02, 07);
            decimal balance = 609243M;
            decimal assuranceBalance = 60927M;

            AmortizationScheduleResponse amortizationSchedule = creditsUsesCaseTest.CreditUsesCase.GetOriginalAmortizationSchedule(
                    AmortizationScheduleHelperTest.GetAmortizationScheduleRequest(creditValue, initialDate, feeValue, interestRate, frequency, fees,
                        downPayment, assuranceValue, assuranceFeeValue, assuranceTotalFeeValue),
                    decimalNumbersRound);

            PaymentMatrix paymentMatrix = PaymentMatrixBuilder.CreateBuilder()
               .BasicInfo(amortizationSchedule, balance, assuranceBalance)
               .DateInfo(calculationDate, lastPaymentDate)
               .CreditParametersInfo(0.2757M, 0.283243M, 3)
               .Build();

            decimal arrearsPayment = CreditPaymentUsesCase.GetArrearsPayment(paymentMatrix, arrearsCharges: 32000, hasArrearsCharge: false, previousArrears: 0);

            Assert.Equal(1931.41M, Math.Round(arrearsPayment, 2));
        }

        [Fact]
        public void ShouldGetArrearsPaymentWithoutCharges_PreviousArrearsGreaterThanPayableArrears()
        {
            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            decimal creditValue = 768000;
            DateTime initialDate = new DateTime(2020, 1, 31);
            decimal feeValue = 94316.0M;
            decimal interestRate = 0.0204985209860811M;
            int frequency = 30;
            int fees = 9;
            decimal downPayment = 0;
            decimal assuranceValue = 76800;
            decimal assuranceFeeValue = 8533.0M;
            decimal assuranceTotalFeeValue = 10154.0M;
            int decimalNumbersRound = 0;

            DateTime calculationDate = new DateTime(2020, 05, 30);
            DateTime lastPaymentDate = new DateTime(2020, 02, 07);
            decimal balance = 609243M;
            decimal assuranceBalance = 60927M;

            AmortizationScheduleResponse amortizationSchedule = creditsUsesCaseTest.CreditUsesCase.GetOriginalAmortizationSchedule(
                    AmortizationScheduleHelperTest.GetAmortizationScheduleRequest(creditValue, initialDate, feeValue, interestRate, frequency, fees,
                        downPayment, assuranceValue, assuranceFeeValue, assuranceTotalFeeValue),
                    decimalNumbersRound);

            PaymentMatrix paymentMatrix = PaymentMatrixBuilder.CreateBuilder()
               .BasicInfo(amortizationSchedule, balance, assuranceBalance)
               .DateInfo(calculationDate, lastPaymentDate)
               .CreditParametersInfo(0.2757M, 0.283243M, 3)
               .Build();

            decimal arrearsPayment = CreditPaymentUsesCase.GetArrearsPayment(paymentMatrix, arrearsCharges: 32000, hasArrearsCharge: false, previousArrears: 2000);

            Assert.Equal(0M, Math.Round(arrearsPayment, 2));
        }

        [Fact]
        public void ShouldGetArrearsPaymentWithCharges()
        {
            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            decimal creditValue = 768000;
            DateTime initialDate = new DateTime(2020, 1, 31);
            decimal feeValue = 94316.0M;
            decimal interestRate = 0.0204985209860811M;
            int frequency = 30;
            int fees = 9;
            decimal downPayment = 0;
            decimal assuranceValue = 76800;
            decimal assuranceFeeValue = 8533.0M;
            decimal assuranceTotalFeeValue = 10154.0M;
            int decimalNumbersRound = 0;

            DateTime calculationDate = new DateTime(2020, 05, 30);
            DateTime lastPaymentDate = new DateTime(2020, 02, 07);
            decimal balance = 609243M;
            decimal assuranceBalance = 60927M;

            AmortizationScheduleResponse amortizationSchedule = creditsUsesCaseTest.CreditUsesCase.GetOriginalAmortizationSchedule(
                    AmortizationScheduleHelperTest.GetAmortizationScheduleRequest(creditValue, initialDate, feeValue, interestRate, frequency, fees,
                        downPayment, assuranceValue, assuranceFeeValue, assuranceTotalFeeValue),
                    decimalNumbersRound);

            PaymentMatrix paymentMatrix = PaymentMatrixBuilder.CreateBuilder()
               .BasicInfo(amortizationSchedule, balance, assuranceBalance)
               .DateInfo(calculationDate, lastPaymentDate)
               .CreditParametersInfo(0.2757M, 0.283243M, 3)
               .Build();

            decimal arrearsPayment = CreditPaymentUsesCase.GetArrearsPayment(paymentMatrix, arrearsCharges: 0, hasArrearsCharge: true, previousArrears: 0);

            Assert.Equal(0, Math.Round(arrearsPayment, 2));
        }

        [Fact]
        public void ShouldGetFullPayment()
        {
            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            decimal creditValueBalance = 61238;
            decimal interestPaymentValue = 1285.41M;
            decimal arrearsPayment = 0;
            decimal chargesPayment = 0;
            decimal payableAssurance = 1983.33M;

            DateTime initialDate = new DateTime(2019, 5, 21);
            decimal feeValue = 16121;
            decimal interestRate = 0.0209905179088468M;
            int frequency = 30;
            int fees = 6;
            decimal downPayment = 10000;
            decimal assuranceValue = 10000;
            decimal assuranceFeeValue = 1666.67M;
            decimal assuranceTotalFeeValue = 1983.33M;
            int decimalNumbersRound = 2;

            AmortizationScheduleResponse amortizationSchedule = creditsUsesCaseTest.CreditUsesCase.GetOriginalAmortizationSchedule(
                AmortizationScheduleHelperTest.GetAmortizationScheduleRequest(creditValueBalance, initialDate, feeValue, interestRate, frequency, fees,
                    downPayment, assuranceValue, assuranceFeeValue, assuranceTotalFeeValue),
                decimalNumbersRound);

            decimal fullPayment = CreditPaymentUsesCase.GetFullPayment(creditValueBalance, interestPaymentValue, arrearsPayment, chargesPayment,
                payableAssurance, amortizationSchedule, calculationDate: DateTime.Today);

            decimal expectedFullPayment = creditValueBalance + interestPaymentValue + arrearsPayment + chargesPayment + payableAssurance;

            Assert.Equal(expectedFullPayment, fullPayment);
        }

        [Fact]
        public void ShouldGetFirstDueFee()
        {
            int lastPaymentFee = 5;
            int totalFees = 8;

            int firstDueFee = CreditPaymentUsesCase.GetFirstDueFee(lastPaymentFee, totalFees);

            Assert.Equal(6, firstDueFee);
        }

        [Fact]
        public void ShouldGetFirstDueFeeLastFee()
        {
            int lastPaymentFee = 8;
            int totalFees = 8;

            int firstDueFee = CreditPaymentUsesCase.GetFirstDueFee(lastPaymentFee, totalFees);

            Assert.Equal(0, firstDueFee);
        }

        [Fact]
        public void ShouldGetLastPaymentFee()
        {
            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            decimal creditValue = 100000;
            DateTime initialDate = new DateTime(2019, 5, 21);
            decimal feeValue = 16121;
            decimal interestRate = 0.0209905179088468M;
            int frequency = 30;
            int fees = 6;
            decimal downPayment = 10000;
            decimal assuranceValue = 10000;
            decimal assuranceFeeValue = 1666.67M;
            decimal assuranceTotalFeeValue = 1983.33M;
            int decimalNumbersRound = 2;

            AmortizationScheduleResponse amortizationSchedule = creditsUsesCaseTest.CreditUsesCase.GetOriginalAmortizationSchedule(
                AmortizationScheduleHelperTest.GetAmortizationScheduleRequest(creditValue, initialDate, feeValue, interestRate, frequency, fees,
                    downPayment, assuranceValue, assuranceFeeValue, assuranceTotalFeeValue),
                decimalNumbersRound);

            decimal creditValueBalance = 75768.15M;

            int lastPaymentFee = CreditPaymentUsesCase.GetLastPaymentFee(amortizationSchedule, creditValueBalance, out DateTime nextDueDate);

            Assert.Equal(1, lastPaymentFee);
            Assert.Equal(new DateTime(2019, 7, 21), nextDueDate);
        }

        [Fact]
        public void ShouldGetLastPaymentFeePartialPay()
        {
            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            decimal creditValue = 100000;
            DateTime initialDate = new DateTime(2019, 5, 21);
            decimal feeValue = 16121;
            decimal interestRate = 0.0209905179088468M;
            int frequency = 30;
            int fees = 6;
            decimal downPayment = 10000;
            decimal assuranceValue = 10000;
            decimal assuranceFeeValue = 1666.67M;
            decimal assuranceTotalFeeValue = 1983.33M;
            int decimalNumbersRound = 2;

            AmortizationScheduleResponse amortizationSchedule = creditsUsesCaseTest.CreditUsesCase.GetOriginalAmortizationSchedule(
                AmortizationScheduleHelperTest.GetAmortizationScheduleRequest(creditValue, initialDate, feeValue, interestRate, frequency, fees,
                    downPayment, assuranceValue, assuranceFeeValue, assuranceTotalFeeValue),
                decimalNumbersRound);

            decimal creditValueBalance = 70000;

            int lastPaymentFee = CreditPaymentUsesCase.GetLastPaymentFee(amortizationSchedule, creditValueBalance, out DateTime nextDueDate);

            Assert.Equal(1, lastPaymentFee);
            Assert.Equal(new DateTime(2019, 7, 21), nextDueDate);
        }

        [Fact]
        public void ShouldGetLastPaymentFeeDownPayment()
        {
            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            decimal creditValue = 100000;
            DateTime initialDate = new DateTime(2019, 5, 21);
            decimal feeValue = 16121;
            decimal interestRate = 0.0209905179088468M;
            int frequency = 30;
            int fees = 6;
            decimal downPayment = 10000;
            decimal assuranceValue = 10000;
            decimal assuranceFeeValue = 1666.67M;
            decimal assuranceTotalFeeValue = 1983.33M;
            int decimalNumbersRound = 2;

            AmortizationScheduleResponse amortizationSchedule = creditsUsesCaseTest.CreditUsesCase.GetOriginalAmortizationSchedule(
                AmortizationScheduleHelperTest.GetAmortizationScheduleRequest(creditValue, initialDate, feeValue, interestRate, frequency, fees,
                    downPayment, assuranceValue, assuranceFeeValue, assuranceTotalFeeValue),
                decimalNumbersRound);

            decimal creditValueBalance = 90000;

            int lastPaymentFee = CreditPaymentUsesCase.GetLastPaymentFee(amortizationSchedule, creditValueBalance, out DateTime nextDueDate);

            Assert.Equal(0, lastPaymentFee);
            Assert.Equal(new DateTime(2019, 6, 21), nextDueDate);
        }

        [Fact]
        public void ShouldGet4AsLastDueFeeWithFrecuency30()
        {
            int expected = 4;
            DateTime calculationDate = new DateTime(2020, 04, 30);

            CreditsUsesCaseTest creditUsesCaseTest = new CreditsUsesCaseTest();
            ICreditUsesCase creditUsesCase = creditUsesCaseTest.CreditUsesCase;

            decimal creditValue = 50000;
            decimal interestRate = 0.0209971486861902M;
            int frequency = 30;
            int fees = 10;
            decimal assuranceValue = 5000;
            decimal downPayment = 0;
            int decimalNumbersRound = 2;
            DateTime initialDate = new DateTime(2019, 12, 31);
            decimal feeValue = 5000;
            decimal assuranceFeeValue = 250;
            decimal assuranceTotalFeeValue = 5250;

            AmortizationScheduleResponse amortizationScheduleResponse = creditUsesCase.GetOriginalAmortizationSchedule(
                AmortizationScheduleHelperTest.GetAmortizationScheduleRequest(creditValue, initialDate, feeValue, interestRate, frequency, fees,
                    downPayment, assuranceValue, assuranceFeeValue, assuranceTotalFeeValue),
                decimalNumbersRound);

            Assert.Equal(expected, CreditPaymentUsesCase.GetLastDueFee(calculationDate, amortizationScheduleResponse));
        }

        [Fact]
        public void ShouldGet9AsLastDueFeeWithFrecuency30()
        {
            int expected = 8;
            DateTime calculationDate = new DateTime(2019, 09, 16);

            CreditsUsesCaseTest creditUsesCaseTest = new CreditsUsesCaseTest();
            ICreditUsesCase creditUsesCase = creditUsesCaseTest.CreditUsesCase;

            decimal creditValue = 50000;
            decimal interestRate = 0.0209971486861902M;
            int frequency = 30;
            int fees = 10;
            decimal assuranceValue = 5000;
            decimal downPayment = 0;
            int decimalNumbersRound = 2;
            DateTime initialDate = new DateTime(2019, 01, 01);
            decimal feeValue = 5000;
            decimal assuranceFeeValue = 250;
            decimal assuranceTotalFeeValue = 5250;

            AmortizationScheduleResponse amortizationScheduleResponse = creditUsesCase.GetOriginalAmortizationSchedule(
                AmortizationScheduleHelperTest.GetAmortizationScheduleRequest(creditValue, initialDate, feeValue, interestRate, frequency, fees,
                    downPayment, assuranceValue, assuranceFeeValue, assuranceTotalFeeValue),
                decimalNumbersRound);

            Assert.Equal(expected, CreditPaymentUsesCase.GetLastDueFee(calculationDate, amortizationScheduleResponse));
        }

        [Fact]
        public void ShouldGet9AsLastDueFeeWithFrecuency14()
        {
            int expected = 8;
            DateTime calculationDate = new DateTime(2020, 04, 30);

            CreditsUsesCaseTest creditUsesCaseTest = new CreditsUsesCaseTest();
            ICreditUsesCase creditUsesCase = creditUsesCaseTest.CreditUsesCase;

            decimal creditValue = 50000;
            decimal interestRate = 0.0209971486861902M;
            int frequency = 14;
            int fees = 10;
            decimal assuranceValue = 5000;
            decimal downPayment = 0;
            int decimalNumbersRound = 2;
            DateTime initialDate = new DateTime(2019, 12, 31);
            decimal feeValue = 5000;
            decimal assuranceFeeValue = 250;
            decimal assuranceTotalFeeValue = 5250;

            AmortizationScheduleResponse amortizationScheduleResponse = creditUsesCase.GetOriginalAmortizationSchedule(
                AmortizationScheduleHelperTest.GetAmortizationScheduleRequest(creditValue, initialDate, feeValue, interestRate, frequency, fees,
                    downPayment, assuranceValue, assuranceFeeValue, assuranceTotalFeeValue),
                decimalNumbersRound);

            int lastDueFee = CreditPaymentUsesCase.GetLastDueFee(calculationDate, amortizationScheduleResponse);

            Assert.Equal(expected, lastDueFee);
        }

        [Fact]
        public void ShouldGetDueCreditBalance()
        {
            decimal balance = 5275.75M;
            decimal assuranceBalance = 600M;

            CreditsUsesCaseTest creditUsesCaseTest = new CreditsUsesCaseTest();
            ICreditUsesCase creditUsesCase = creditUsesCaseTest.CreditUsesCase;

            decimal creditValue = 100000;
            decimal interestRate = 0.0209971486861902M;
            int frequency = 30;
            int fees = 15;
            decimal assuranceValue = 10000;
            decimal downPayment = 10000;
            int decimalNumbersRound = 2;
            DateTime initialDate = new DateTime(2019, 01, 01);
            decimal feeValue = 7057;
            decimal assuranceFeeValue = 625;
            decimal assuranceTotalFeeValue = 743.75M;

            AmortizationScheduleResponse amortizationScheduleResponse = creditUsesCase.GetOriginalAmortizationSchedule(
                AmortizationScheduleHelperTest.GetAmortizationScheduleRequest(creditValue, initialDate, feeValue, interestRate, frequency, fees,
                    downPayment, assuranceValue, assuranceFeeValue, assuranceTotalFeeValue),
                decimalNumbersRound);

            PaymentMatrix paymentMatrix = PaymentMatrixBuilder.CreateBuilder()
               .BasicInfo(amortizationScheduleResponse, balance, assuranceBalance)
               .DateInfo(calculationDate: new DateTime(), lastPaymentDate: new DateTime())
               .CreditParametersInfo(effectiveAnnualRate: 0M, arrearsEffectiveAnnualRate: 0M, 3)
               .Build();

            Assert.Equal(balance, CreditPaymentUsesCase.GetDueCreditBalance(paymentMatrix, 1, 15));
        }

        [Fact]
        public void GetActiveCreditsAsyncReturnListCreditStatusSuccesfully()
        {
            List<CreditMaster> creditMasters = CreditMasterHelperTest.GetCreditMasterList();
            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            decimal creditValue = 100000;
            decimal interestRate = 0.0209971486861902M;
            int frequency = 30;
            int fees = 10;
            decimal assuranceValue = 10000;
            decimal downPayment = 10000;
            int decimalNumbersRound = 2;
            DateTime initialDate = new DateTime(2019, 01, 01);
            decimal feeValue = 7057;
            decimal assuranceFeeValue = 625;
            decimal assuranceTotalFeeValue = 743.75M;

            AmortizationScheduleResponse amortizationScheduleResponse = creditsUsesCaseTest.CreditUsesCase.GetOriginalAmortizationSchedule(
                AmortizationScheduleHelperTest.GetAmortizationScheduleRequest(creditValue, initialDate, feeValue, interestRate, frequency, fees,
                    downPayment, assuranceValue, assuranceFeeValue, assuranceTotalFeeValue),
                decimalNumbersRound);

            _creditUseCaseMock.Setup(item => item.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(), It.IsAny<int>()))
                .Returns(amortizationScheduleResponse);

            var activeCreditsResponse = CreditPaymentUsesCase.GetActiveCredits(creditMasters, parameters, DateTime.Today);

            Assert.NotNull(creditMasters);
            Assert.Equal(creditMasters.Count, activeCreditsResponse.Count);
            Assert.IsType<List<CreditStatus>>(activeCreditsResponse);
        }

        [Fact]
        public void GetDetailedActiveCreditsAsyncReturnListCreditStatusSuccesfully()
        {
            List<CreditMaster> creditMasters = CreditMasterHelperTest.GetCreditMasterList();
            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            decimal creditValue = 100000;
            decimal interestRate = 0.0209971486861902M;
            int frequency = 30;
            int fees = 10;
            decimal assuranceValue = 10000;
            decimal downPayment = 10000;
            int decimalNumbersRound = 2;
            DateTime initialDate = new DateTime(2019, 01, 01);
            decimal feeValue = 7057;
            decimal assuranceFeeValue = 625;
            decimal assuranceTotalFeeValue = 743.75M;

            AmortizationScheduleResponse amortizationScheduleResponse = creditsUsesCaseTest.CreditUsesCase.GetOriginalAmortizationSchedule(
                AmortizationScheduleHelperTest.GetAmortizationScheduleRequest(creditValue, initialDate, feeValue, interestRate, frequency, fees,
                    downPayment, assuranceValue, assuranceFeeValue, assuranceTotalFeeValue),
                decimalNumbersRound);

            _creditUseCaseMock.Setup(item => item.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(), It.IsAny<int>()))
                .Returns(amortizationScheduleResponse);

            var activeCreditsResponse = CreditPaymentUsesCase.GetDetailedActiveCredits(creditMasters, parameters, DateTime.Today);

            Assert.NotNull(creditMasters);
            Assert.Equal(creditMasters.Count, activeCreditsResponse.Count);
            Assert.IsType<List<DetailedCreditStatus>>(activeCreditsResponse);
        }

        [Fact]
        public void GetActiveCreditsAsyncWithoutCreditsMasterListReturnBussinessException()
        {
            List<CreditMaster> creditMasters = new List<CreditMaster>();
            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            Assert.Throws<BusinessException>(() => CreditPaymentUsesCase.GetActiveCredits(creditMasters, parameters, DateTime.Today));
        }

        [Fact]
        public void GetDetailedActiveCreditsByCreditMasterIdAsyncReturnListCreditStatusSuccesfully()
        {
            List<CreditMaster> creditMasters = CreditMasterHelperTest.GetCreditMasterList();
            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            decimal creditValue = 100000;
            decimal interestRate = 0.0209971486861902M;
            int frequency = 30;
            int fees = 10;
            decimal assuranceValue = 10000;
            decimal downPayment = 10000;
            int decimalNumbersRound = 2;
            DateTime initialDate = new DateTime(2019, 01, 01);
            decimal feeValue = 7057;
            decimal assuranceFeeValue = 625;
            decimal assuranceTotalFeeValue = 743.75M;

            AmortizationScheduleResponse amortizationScheduleResponse = creditsUsesCaseTest.CreditUsesCase.GetOriginalAmortizationSchedule(
                AmortizationScheduleHelperTest.GetAmortizationScheduleRequest(creditValue, initialDate, feeValue, interestRate, frequency, fees,
                    downPayment, assuranceValue, assuranceFeeValue, assuranceTotalFeeValue),
                decimalNumbersRound);

            _creditUseCaseMock.Setup(item => item.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(), It.IsAny<int>()))
                .Returns(amortizationScheduleResponse);

            var activeCreditsResponse = CreditPaymentUsesCase.GetDetailedActiveCredits(creditMasters, parameters, DateTime.Today);

            Assert.NotNull(creditMasters);
            Assert.Equal(creditMasters.Count, activeCreditsResponse.Count);
            Assert.IsType<List<DetailedCreditStatus>>(activeCreditsResponse);
        }

        [Fact]
        public void GetActiveCreditsByCreditMasterIdAsyncWithoutCreditsMasterListReturnBussinessException()
        {
            List<CreditMaster> creditMasters = new List<CreditMaster>();
            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            Assert.Throws<BusinessException>(() => CreditPaymentUsesCase.GetActiveCredits(creditMasters, parameters, DateTime.Today));
        }


        [Fact]
        public void GetDetailedActiveCreditsAsyncWithoutCreditsMasterListReturnBussinessException()
        {
            List<CreditMaster> creditMasters = new List<CreditMaster>();
            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            Assert.Throws<BusinessException>(() => CreditPaymentUsesCase.GetDetailedActiveCredits(creditMasters, parameters, DateTime.Today));
        }

        [Fact]
        public void ShouldGetStatusCredits()
        {
            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            decimal creditValue = 100000;
            decimal interestRate = 0.0209971486861902M;
            int frequency = 30;
            int fees = 8;
            decimal assuranceValue = 10000;
            decimal downPayment = 10000;
            int decimalNumbersRound = 2;
            DateTime initialDate = new DateTime(2019, 01, 01);
            decimal feeValue = 7057;
            decimal assuranceFeeValue = 625;
            decimal assuranceTotalFeeValue = 743.75M;

            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMaster();
            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            AmortizationScheduleResponse amortizationScheduleResponse = creditsUsesCaseTest.CreditUsesCase.GetOriginalAmortizationSchedule(
                AmortizationScheduleHelperTest.GetAmortizationScheduleRequest(creditValue, initialDate, feeValue, interestRate, frequency, fees,
                    downPayment, assuranceValue, assuranceFeeValue, assuranceTotalFeeValue),
                decimalNumbersRound);

            _creditUseCaseMock.Setup(item => item.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(), It.IsAny<int>()))
                .Returns(amortizationScheduleResponse);

            CreditStatusResponse result = CreditPaymentUsesCase.GetStatusCredit(creditMaster, parameters);

            Assert.NotNull(result);
            Assert.Equal(creditMaster.GetCreditNumber, result.CreditNumber);
            Assert.IsType<CreditStatusResponse>(result);
        }

        [Fact]
        public void ShouldPay()
        {
            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMasterNoDownPayment(new DateTime(2019, 6, 22), new DateTime(2019, 7, 22));
            Credit credit = creditMaster.Current;
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            Store store = StoreHelperTest.GetStore();
            decimal totalValuePaid = 80000;
            string userId = "kajs5145d5a78we";
            string bankAccount = string.Empty;
            string location = "1,1";
            string storeId = "5sd45sd87a5w95a6";
            string userName = "Test user";
            Status activeStatus = StatusHelperTest.GetActiveStatus();
            string expectedTransactionId = "156et4g564teyg";

            PaymentCreditRequest paymentCreditRequest = new PaymentCreditRequest
            {
                BankAccount = bankAccount,
                CreditId = creditMaster.Id,
                Location = location,
                StoreId = storeId,
                TotalValuePaid = totalValuePaid,
                UserId = userId,
                UserName = userName
            };

            PaymentCreditRequestComplete paymentCreditRequestComplete = new PaymentCreditRequestComplete(paymentCreditRequest)
            {
                CalculationDate = new DateTime(2019, 9, 22),
                TransactionId = expectedTransactionId
            };

            _creditUseCaseMock.Setup(mock => mock.GetValidEffectiveAnnualRate(It.IsAny<decimal>(), It.IsAny<decimal>()))
                .Returns(creditMaster.GetEffectiveAnnualRate);

            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            _creditUseCaseMock.Setup(mock => mock.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(), It.IsAny<int>()))
                .Returns(creditsUsesCaseTest.CreditUsesCase
                    .GetOriginalAmortizationSchedule(AmortizationScheduleRequest.FromCredit(credit, creditMaster.GetCreditDate), 2));

            _creditUseCaseMock.Setup(mock => mock.GetInterestRate(It.IsAny<decimal>(), It.IsAny<int>()))
                .Returns(credit.GetInterestRate);

            _creditUseCaseMock.Setup(mock => mock.HasArrears(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(true);

            _creditUseCaseMock.Setup(mock => mock.GetArrearsDays(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(62);

            CreditPaymentUsesCase.PayAsync(new PaymentDomainRequest(paymentCreditRequestComplete, creditMaster, store, parameters,
                    CredinetAppSettingsHelperTest.GetCredinetAppSettings())
                .SetMasters(TransactionTypeHelperTest.GetPaymentType(), StatusHelperTest.GetActiveStatus(), StatusHelperTest.GetPaidStatus()),
                    PaymentTypeHelperTest.GetOrdinaryPaymentType());

            Credit newCredit = creditMaster.Current;

            Assert.Equal(activeStatus.Id, creditMaster.GetStatusId);
            Assert.Equal(80000M, newCredit.CreditPayment.GetTotalValuePaid);
            Assert.Equal(10858.87M, newCredit.CreditPayment.GetInterestValuePaid);
            Assert.Equal(1713.42M, newCredit.CreditPayment.GetArrearsValuePaid);
            Assert.Equal(8925M, newCredit.CreditPayment.GetAssuranceValuePaid);
            Assert.Equal(58502.71M, newCredit.CreditPayment.GetCreditValuePaid);
            Assert.Equal(2, newCredit.CreditPayment.GetLastFee);
            Assert.Equal(new DateTime(2019, 9, 22), newCredit.CreditPayment.GetDueDate);
            Assert.Equal(expectedTransactionId, newCredit.TransactionReference.TransactionId);
            Assert.Equal(newCredit.Id, newCredit.TransactionReference.CreditId);

            _creditMasterRepositoryMock.Verify(mock => mock.AddTransactionAsync(It.IsAny<CreditMaster>(), It.IsAny<IEnumerable<Field>>(),
                     It.IsAny<Transaction>()), Times.Once);
        }

        [Fact]
        public void ShouldAddPaymentWithPaymentTypeDownPayment()
        {
            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMasterDateTimeNow();
            Credit credit = creditMaster.Current;
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            Store store = StoreHelperTest.GetStore();
            decimal totalValuePaid = 20000;
            string userId = "kajs5145d5a78we";
            string bankAccount = string.Empty;
            string location = "1,1";
            string storeId = "5sd45sd87a5w95a6";
            string userName = "Test user";
            decimal interestRate = 0.0209971486861902M;
            Status activeStatus = StatusHelperTest.GetActiveStatus();

            PaymentCreditRequest paymentCreditRequest = new PaymentCreditRequest
            {
                BankAccount = bankAccount,
                CreditId = creditMaster.Id,
                Location = location,
                StoreId = storeId,
                TotalValuePaid = totalValuePaid,
                UserId = userId,
                UserName = userName
            };

            PaymentCreditRequestComplete paymentCreditRequestComplete = new PaymentCreditRequestComplete(paymentCreditRequest)
            {
                CalculationDate = new DateTime(2019, 9, 22)
            };

            _creditUseCaseMock.Setup(mock => mock.GetValidEffectiveAnnualRate(It.IsAny<decimal>(), It.IsAny<decimal>()))
                .Returns(interestRate);

            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            _creditUseCaseMock.Setup(mock => mock.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(), It.IsAny<int>()))
                .Returns(creditsUsesCaseTest.CreditUsesCase
                    .GetOriginalAmortizationSchedule(AmortizationScheduleRequest.FromCredit(credit, creditMaster.GetCreditDate), 2));

            _creditUseCaseMock.Setup(mock => mock.GetInterestRate(It.IsAny<decimal>(), It.IsAny<int>()))
                .Returns(0.00695062642333455M);

            CreditPaymentUsesCase.PayAsync(new PaymentDomainRequest(paymentCreditRequestComplete, creditMaster, store, parameters,
                    CredinetAppSettingsHelperTest.GetCredinetAppSettings())
                .SetMasters(TransactionTypeHelperTest.GetPaymentType(), StatusHelperTest.GetActiveStatus(), StatusHelperTest.GetPaidStatus()),
                    PaymentTypeHelperTest.GetDownPaymentPaymentType());

            Credit newCredit = creditMaster.Current;

            Assert.Equal(activeStatus.Id, creditMaster.GetStatusId);
            Assert.Equal(20000M, newCredit.CreditPayment.GetTotalValuePaid);
            Assert.Equal(0M, newCredit.CreditPayment.GetInterestValuePaid);
            Assert.Equal(0M, newCredit.CreditPayment.GetArrearsValuePaid);
            Assert.Equal(2644, newCredit.CreditPayment.GetAssuranceValuePaid);
            Assert.Equal(17356M, newCredit.CreditPayment.GetCreditValuePaid);
            Assert.Equal(0, newCredit.CreditPayment.GetLastFee);
            Assert.Equal(DateTime.Today.AddMonths(1), newCredit.CreditPayment.GetDueDate);
            Assert.Null(newCredit.TransactionReference);

            _creditMasterRepositoryMock.Verify(mock => mock.AddTransactionAsync(It.IsAny<CreditMaster>(), It.IsAny<IEnumerable<Field>>(),
                It.IsAny<Transaction>()), Times.Once);
        }

        [Fact]
        public void ShouldAddPaymentWithTotalPayment()
        {
            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMasterDateTimeNow();
            Credit credit = creditMaster.Current;
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            Store store = StoreHelperTest.GetStore();
            decimal totalValuePaid = 200000;
            string userId = "kajs5145d5a78we";
            string bankAccount = string.Empty;
            string location = "1,1";
            string storeId = "5sd45sd87a5w95a6";
            string userName = "Test user";
            decimal interestRate = 0.0209971486861902M;
            Status paidStatus = StatusHelperTest.GetPaidStatus();

            CredinetAppSettings credinetAppSettings = CredinetAppSettingsHelperTest.GetCredinetAppSettings();

            PaymentCreditRequest paymentCreditRequest = new PaymentCreditRequest
            {
                BankAccount = bankAccount,
                CreditId = creditMaster.Id,
                Location = location,
                StoreId = storeId,
                TotalValuePaid = totalValuePaid,
                UserId = userId,
                UserName = userName
            };

            PaymentCreditRequestComplete paymentCreditRequestComplete = new PaymentCreditRequestComplete(paymentCreditRequest)
            {
                CalculationDate = new DateTime(2019, 9, 22)
            };

            _creditUseCaseMock.Setup(mock => mock.GetValidEffectiveAnnualRate(It.IsAny<decimal>(), It.IsAny<decimal>()))
                .Returns(interestRate);

            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            _creditUseCaseMock.Setup(mock => mock.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(), It.IsAny<int>()))
                .Returns(creditsUsesCaseTest.CreditUsesCase
                    .GetOriginalAmortizationSchedule(AmortizationScheduleRequest.FromCredit(credit, creditMaster.GetCreditDate), 2));

            _creditUseCaseMock.Setup(mock => mock.GetInterestRate(It.IsAny<decimal>(), It.IsAny<int>()))
                .Returns(0.00695062642333455M);

            CreditPaymentUsesCase.PayAsync(new PaymentDomainRequest(paymentCreditRequestComplete, creditMaster, store, parameters,
                    CredinetAppSettingsHelperTest.GetCredinetAppSettings())
                .SetMasters(TransactionTypeHelperTest.GetPaymentType(), StatusHelperTest.GetActiveStatus(), StatusHelperTest.GetPaidStatus()),
                    PaymentTypeHelperTest.GetOrdinaryPaymentType());

            Credit newCredit = creditMaster.Current;

            Assert.Equal(paidStatus.Id, creditMaster.GetStatusId);
            Assert.Equal(200000M, newCredit.CreditPayment.GetTotalValuePaid);
            Assert.Equal(0M, newCredit.CreditPayment.GetInterestValuePaid);
            Assert.Equal(0M, newCredit.CreditPayment.GetArrearsValuePaid);
            Assert.Equal(0M, newCredit.CreditPayment.GetAssuranceValuePaid);
            Assert.Equal(200000M, newCredit.CreditPayment.GetCreditValuePaid);
            Assert.Equal(8, newCredit.CreditPayment.GetLastFee);
            Assert.Equal(DateTime.Today, newCredit.CreditPayment.GetDueDate);
            Assert.Null(newCredit.TransactionReference);

            _creditMasterRepositoryMock.Verify(mock => mock.AddTransactionAsync(It.IsAny<CreditMaster>(), It.IsAny<IEnumerable<Field>>(),
                It.IsAny<Transaction>()), Times.Once);
        }

        [Fact]
        public void ShouldAddPaymentWithPaymentTypeCreditValueOnly()
        {
            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMaster(new DateTime(2019, 6, 22), new DateTime(2019, 7, 22));
            Credit credit = creditMaster.Current;
            Store store = StoreHelperTest.GetStore();
            decimal totalValuePaid = 80000;
            string userId = "kajs5145d5a78we";
            string bankAccount = string.Empty;
            string location = "1,1";
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            string storeId = "5sd45sd87a5w95a6";
            string userName = "Test user";
            decimal interestRate = 0.0209971486861902M;
            Status activeStatus = StatusHelperTest.GetActiveStatus();

            PaymentCreditRequest paymentCreditRequest = new PaymentCreditRequest
            {
                BankAccount = bankAccount,
                CreditId = creditMaster.Id,
                Location = location,
                StoreId = storeId,
                TotalValuePaid = totalValuePaid,
                UserId = userId,
                UserName = userName
            };

            PaymentCreditRequestComplete paymentCreditRequestComplete = new PaymentCreditRequestComplete(paymentCreditRequest)
            {
                CalculationDate = new DateTime(2019, 9, 22)
            };

            _creditUseCaseMock.Setup(mock => mock.GetValidEffectiveAnnualRate(It.IsAny<decimal>(), It.IsAny<decimal>()))
                .Returns(interestRate);

            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            _creditUseCaseMock.Setup(mock => mock.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(), It.IsAny<int>()))
                .Returns(creditsUsesCaseTest.CreditUsesCase
                    .GetOriginalAmortizationSchedule(AmortizationScheduleRequest.FromCredit(credit, creditMaster.GetCreditDate), 2));

            _creditUseCaseMock.Setup(mock => mock.GetInterestRate(It.IsAny<decimal>(), It.IsAny<int>()))
                .Returns(0.00695062642333455M);

            CreditPaymentUsesCase.PayAsync(new PaymentDomainRequest(paymentCreditRequestComplete, creditMaster, store, parameters,
                    CredinetAppSettingsHelperTest.GetCredinetAppSettings())
                .SetMasters(TransactionTypeHelperTest.GetPaymentType(), StatusHelperTest.GetActiveStatus(), StatusHelperTest.GetPaidStatus()),
                    PaymentTypeHelperTest.GetOrdinaryPaymentType());

            Credit newCredit = creditMaster.Current;

            Assert.Equal(activeStatus.Id, creditMaster.GetStatusId);
            Assert.Equal(80000M, newCredit.CreditPayment.GetTotalValuePaid);
            Assert.Equal(831.08M, newCredit.CreditPayment.GetInterestValuePaid);
            Assert.Equal(0M, newCredit.CreditPayment.GetArrearsValuePaid);
            Assert.Equal(7932, newCredit.CreditPayment.GetAssuranceValuePaid);
            Assert.Equal(71236.92M, newCredit.CreditPayment.GetCreditValuePaid);
            Assert.Equal(2, newCredit.CreditPayment.GetLastFee);
            Assert.Equal(new DateTime(2019, 9, 22), newCredit.CreditPayment.GetDueDate);

            _creditMasterRepositoryMock.Verify(mock => mock.AddTransactionAsync(It.IsAny<CreditMaster>(), It.IsAny<IEnumerable<Field>>(),
                It.IsAny<Transaction>()), Times.Once);
        }

        [Fact]
        public async Task ShouldNotBeAuthorizedToPayCredit_ClosedStore()
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

            Store store = StoreHelperTest.GetStoreWithStatus((int)StoreStatuses.Closed);

            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            PaymentDomainRequest paymentDomainRequest = new PaymentDomainRequest(paymentCreditRequest,
                creditMaster, store, parameters, CredinetAppSettingsHelperTest.GetCredinetAppSettings());

            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            _creditUseCaseMock.Setup(mock => mock.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(), It.IsAny<int>()))
                .Returns(creditsUsesCaseTest.CreditUsesCase.GetOriginalAmortizationSchedule(AmortizationScheduleRequest.FromCredit(creditMaster.Current, creditMaster.GetCreditDate, 0), 2));

            BusinessException businessException = await Assert.ThrowsAsync<BusinessException>(() =>
               CreditPaymentUsesCase.PayAsync(paymentDomainRequest));

            Assert.Equal((int)BusinessResponse.PaymentUnauthorized, businessException.code);
        }

        [Fact]
        public async Task ShouldNotBeAuthorizedToPayCredit_NoPaymentStore()
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

            Store store = StoreHelperTest.GetStoreWithStatus((int)StoreStatuses.NoPayments);

            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            PaymentDomainRequest paymentDomainRequest = new PaymentDomainRequest(paymentCreditRequest,
                creditMaster, store, parameters, CredinetAppSettingsHelperTest.GetCredinetAppSettings());

            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            _creditUseCaseMock.Setup(mock => mock.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(), It.IsAny<int>()))
                .Returns(creditsUsesCaseTest.CreditUsesCase.GetOriginalAmortizationSchedule(AmortizationScheduleRequest.FromCredit(creditMaster.Current, creditMaster.GetCreditDate, 0), 2));

            BusinessException businessException = await Assert.ThrowsAsync<BusinessException>(() =>
               CreditPaymentUsesCase.PayAsync(paymentDomainRequest));

            Assert.Equal((int)BusinessResponse.PaymentUnauthorized, businessException.code);
        }

        [Fact]
        public async Task ShouldNotPayCredit_PaymentExceedsMaximumPayment()
        {
            PaymentCreditRequestComplete paymentCreditRequest = new PaymentCreditRequestComplete
            {
                BankAccount = string.Empty,
                CalculationDate = new DateTime(2019, 9, 23),
                CreditId = Guid.Parse("a8876e4b-23be-c51f-89bc-08d73fb1ffef"),
                Location = "1,1",
                StoreId = "5c8964fbfe8e3e43d48bf393",
                TotalValuePaid = 201000,
                UserId = "50BC0535-9E3A-43BC-B451-33C8DE162FD0",
                UserName = "Test User"
            };

            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMaster();

            Store store = StoreHelperTest.GetStore();

            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            PaymentType paymentType = PaymentTypeHelperTest.GetOrdinaryPaymentType();

            PaymentDomainRequest paymentDomainRequest = new PaymentDomainRequest(paymentCreditRequest,
                    creditMaster, store, parameters, CredinetAppSettingsHelperTest.GetCredinetAppSettings())
                .SetMasters(TransactionTypeHelperTest.GetPaymentType(), StatusHelperTest.GetActiveStatus(),
                    StatusHelperTest.GetPaidStatus());

            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            _creditUseCaseMock.Setup(mock => mock.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(), It.IsAny<int>()))
                .Returns(creditsUsesCaseTest.CreditUsesCase.GetOriginalAmortizationSchedule(AmortizationScheduleRequest.FromCredit(creditMaster.Current, creditMaster.GetCreditDate, 0), 2));

            BusinessException businessException = await Assert.ThrowsAsync<BusinessException>(() =>
               CreditPaymentUsesCase.PayAsync(paymentDomainRequest, paymentType));

            Assert.Equal((int)BusinessResponse.PaymentExceedsTotalPayment, businessException.code);
        }

        [Fact]
        public void ShouldReturnPaymentFees_beforeDueDate()
        {
            DateTime calculationDate = new DateTime(2019, 12, 4);

            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMasterNoDownPayment(
                creditDate: calculationDate.AddMonths(-3).AddDays(-7),
                dueDate: calculationDate.AddMonths(-2).AddDays(-7)
            );

            Enumerable.Range(1, 3).ToList().ForEach(_ =>
                CreditMasterHelperTest.AddTransaction(creditMaster, TransactionTypes.Payment, creditMaster.Store, Statuses.Active, new DateTime(2019, 11, 28))
            );
            Credit credit = creditMaster.Current;

            decimal effectiveAnnualRate = 0.2757M;
            decimal interestRate = 0.0204985209860811M;

            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            int decimalNumbersRound = parameters.DecimalNumbersRound;

            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            _creditUseCaseMock.Setup(mock => mock.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(), It.IsAny<int>()))
                .Returns(creditsUsesCaseTest.CreditUsesCase.GetOriginalAmortizationSchedule(AmortizationScheduleRequest.FromCredit(credit, creditMaster.GetCreditDate, interestRate), decimalNumbersRound));

            _creditUseCaseMock.Setup(mock => mock.GetValidEffectiveAnnualRate(It.IsAny<decimal>(), It.IsAny<decimal>()))
                .Returns(effectiveAnnualRate);

            _creditUseCaseMock.Setup(mock => mock.GetInterestRate(It.IsAny<decimal>(), It.IsAny<int>()))
                .Returns(interestRate);

            _creditUseCaseMock.Setup(mock => mock.HasArrears(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(false);

            PaymentFeesResponse result = CreditPaymentUsesCase.GetPaymentFees(creditMaster, credit, parameters, calculationDate, out decimal totalPayment);

            Assert.Equal(5, result.PendingFees);
            Assert.Equal(17316.93M, result.PaymentFees[0].Payment);
            Assert.Equal(103488.05M, result.PaymentFees[3].Payment);
            Assert.Equal(Math.Round(totalPayment, decimalNumbersRound), result.PaymentFees[4].Payment);
            Assert.IsType<PaymentFeesResponse>(result);
        }

        [Fact]
        public void ShouldReturnPaymentFees_beforeDueDate_minimumPaymentgreaterThanTotalFeeValue()
        {
            DateTime calculationDate = new DateTime(2019, 12, 4);

            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMasterNoDownPayment(
                creditDate: calculationDate.AddMonths(-3).AddDays(-7),
                dueDate: calculationDate.AddMonths(-2).AddDays(-7)
            );

            CreditMasterHelperTest.UpdateChargesPaymentPlan(creditMaster, chargesValue: 20000, hasArrearsCharge: false,
              arrearsCharges: 0, updatedPaymentPlanValue: 0, TransactionTypes.UpdateChargesPaymentPlan);

            Enumerable.Range(1, 3).ToList().ForEach(_ =>
                CreditMasterHelperTest.AddTransaction(creditMaster, TransactionTypes.Payment, creditMaster.Store, Statuses.Active, new DateTime(2019, 11, 28))
            );
            Credit credit = creditMaster.Current;

            decimal effectiveAnnualRate = 0.2757M;
            decimal interestRate = 0.0204985209860811M;

            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            int decimalNumbersRound = parameters.DecimalNumbersRound;

            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            _creditUseCaseMock.Setup(mock => mock.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(), It.IsAny<int>()))
                .Returns(creditsUsesCaseTest.CreditUsesCase.GetOriginalAmortizationSchedule(AmortizationScheduleRequest.FromCredit(credit, creditMaster.GetCreditDate, interestRate), decimalNumbersRound));

            _creditUseCaseMock.Setup(mock => mock.GetValidEffectiveAnnualRate(It.IsAny<decimal>(), It.IsAny<decimal>()))
                .Returns(effectiveAnnualRate);

            _creditUseCaseMock.Setup(mock => mock.GetInterestRate(It.IsAny<decimal>(), It.IsAny<int>()))
                .Returns(interestRate);

            _creditUseCaseMock.Setup(mock => mock.HasArrears(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(false);

            PaymentFeesResponse result = CreditPaymentUsesCase.GetPaymentFees(creditMaster, credit, parameters, calculationDate, out decimal totalPayment);

            decimal expectedOneFeeValue = 37316.93M;

            Assert.Equal(expectedOneFeeValue, result.PaymentFees[0].Payment);
            Assert.Equal(123488.05M, result.PaymentFees[3].Payment);
            Assert.Equal(Math.Round(totalPayment, decimalNumbersRound), result.PaymentFees[4].Payment);
            Assert.IsType<PaymentFeesResponse>(result);
        }

        [Fact]
        public void ShouldReturnPaymentFees_dueDateDay()
        {
            DateTime calculationDate = new DateTime(2019, 12, 4);

            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMasterNoDownPayment(
                creditDate: calculationDate.AddMonths(-4).AddDays(0),
                dueDate: calculationDate.AddMonths(-3).AddDays(0)
            );

            Enumerable.Range(1, 3).ToList().ForEach(_ =>
                CreditMasterHelperTest.AddTransaction(creditMaster, TransactionTypes.Payment, creditMaster.Store, Statuses.Active, new DateTime(2019, 11, 28))
            );
            Credit credit = creditMaster.Current;

            decimal effectiveAnnualRate = 0.2757M;
            decimal interestRate = 0.0204985209860811M;

            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            int decimalNumbersRound = parameters.DecimalNumbersRound;

            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            _creditUseCaseMock.Setup(mock => mock.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(), It.IsAny<int>()))
                .Returns(creditsUsesCaseTest.CreditUsesCase.GetOriginalAmortizationSchedule(AmortizationScheduleRequest.FromCredit(credit, creditMaster.GetCreditDate, interestRate), decimalNumbersRound));

            _creditUseCaseMock.Setup(mock => mock.GetValidEffectiveAnnualRate(It.IsAny<decimal>(), It.IsAny<decimal>()))
                .Returns(effectiveAnnualRate);

            _creditUseCaseMock.Setup(mock => mock.GetInterestRate(It.IsAny<decimal>(), It.IsAny<int>()))
                .Returns(interestRate);

            _creditUseCaseMock.Setup(mock => mock.HasArrears(It.IsAny<DateTime>(), It.IsAny<DateTime>(), 0))
                .Returns(false);

            PaymentFeesResponse result = CreditPaymentUsesCase.GetPaymentFees(creditMaster, credit, parameters, calculationDate, out decimal totalPayment);

            decimal expectedOneFeeValue = 17316.93M;

            Assert.Equal(5, result.PendingFees);
            Assert.Equal(expectedOneFeeValue, result.PaymentFees[0].Payment);
            Assert.Equal(103488.05M, result.PaymentFees[3].Payment);
            Assert.Equal(Math.Round(totalPayment, decimalNumbersRound), result.PaymentFees[4].Payment);
            Assert.IsType<PaymentFeesResponse>(result);
        }

        [Fact]
        public void ShouldReturnPaymentFees_graceDays()
        {
            DateTime calculationDate = new DateTime(2019, 12, 4);

            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMasterNoDownPayment(
                creditDate: calculationDate.AddMonths(-4).AddDays(-2),
                dueDate: calculationDate.AddMonths(-3).AddDays(-2)
            );

            Enumerable.Range(1, 3).ToList().ForEach(_ =>
                CreditMasterHelperTest.AddTransaction(creditMaster, TransactionTypes.Payment, creditMaster.Store, Statuses.Active, new DateTime(2019, 11, 28))
            );
            Credit credit = creditMaster.Current;

            decimal effectiveAnnualRate = 0.2757M;
            decimal interestRate = 0.0204985209860811M;

            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            int decimalNumbersRound = parameters.DecimalNumbersRound;

            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            _creditUseCaseMock.Setup(mock => mock.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(), It.IsAny<int>()))
                .Returns(creditsUsesCaseTest.CreditUsesCase.GetOriginalAmortizationSchedule(AmortizationScheduleRequest.FromCredit(credit, creditMaster.GetCreditDate, interestRate), decimalNumbersRound));

            _creditUseCaseMock.Setup(mock => mock.GetValidEffectiveAnnualRate(It.IsAny<decimal>(), It.IsAny<decimal>()))
                .Returns(effectiveAnnualRate);

            _creditUseCaseMock.Setup(mock => mock.GetInterestRate(It.IsAny<decimal>(), It.IsAny<int>()))
                .Returns(interestRate);

            _creditUseCaseMock.Setup(mock => mock.HasArrears(It.IsAny<DateTime>(), It.IsAny<DateTime>(), 0))
                .Returns(true);

            PaymentFeesResponse result = CreditPaymentUsesCase.GetPaymentFees(creditMaster, credit, parameters, calculationDate, out decimal totalPayment);

            decimal feeValue = 27361.0M;
            decimal assuranceFeeValue = 2975.0M;
            decimal totalFeeValue = feeValue + assuranceFeeValue;

            decimal expectedOneFeeValue = totalFeeValue;

            Assert.Equal(5, result.PendingFees);
            Assert.NotEqual(expectedOneFeeValue, result.PaymentFees[0].Payment);
            Assert.Equal(Math.Round(totalPayment, decimalNumbersRound), result.PaymentFees[4].Payment);
            Assert.IsType<PaymentFeesResponse>(result);
        }

        [Fact]
        public void ShouldReturnPaymentFees_Charges()
        {
            decimal chargesValue = 18000M;
            decimal arrearsCharge = 20000M;

            DateTime calculationDate = new DateTime(2019, 12, 4);

            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMasterNoDownPayment(
                creditDate: calculationDate.AddMonths(-3).AddDays(-7),
                dueDate: calculationDate.AddMonths(-2).AddDays(-7)
            );

            Enumerable.Range(1, 3).ToList().ForEach(_ =>
                CreditMasterHelperTest.AddTransaction(creditMaster, TransactionTypes.Payment, creditMaster.Store, Statuses.Active, new DateTime(2019, 12, 28))
            );
            CreditMasterHelperTest.UpdateChargesPaymentPlan(creditMaster, chargesValue, hasArrearsCharge: true,
                arrearsCharge, updatedPaymentPlanValue: 0, TransactionTypes.UpdateChargesPaymentPlan);
            Credit credit = creditMaster.Current;

            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            int decimalNumbersRound = parameters.DecimalNumbersRound;

            decimal interestRate = 0.0204985209860811M;
            _creditUseCaseMock.Setup(mock => mock.GetValidEffectiveAnnualRate(It.IsAny<decimal>(), It.IsAny<decimal>()))
                .Returns(0.2757M);

            _creditUseCaseMock.Setup(mock => mock.GetInterestRate(It.IsAny<decimal>(), It.IsAny<int>()))
                .Returns(interestRate);

            _creditUseCaseMock.Setup(mock => mock.HasArrears(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(true);

            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();
            _creditUseCaseMock.Setup(mock => mock.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(), It.IsAny<int>()))
                .Returns(creditsUsesCaseTest.CreditUsesCase.GetOriginalAmortizationSchedule(
                    AmortizationScheduleRequest.FromCredit(credit, creditMaster.GetCreditDate, interestRate),
                 decimalNumbersRound));

            PaymentFeesResponse result = CreditPaymentUsesCase.GetPaymentFees(creditMaster, credit, parameters, calculationDate.AddMonths(2), out decimal totalPayment);

            decimal expectedOneFeeValue = 57290.93M;

            Assert.Equal(5, result.PendingFees);
            Assert.Equal(expectedOneFeeValue, result.PaymentFees[0].Payment);
            Assert.Equal(143462.05M, result.PaymentFees[3].Payment);
            Assert.Equal(Math.Round(totalPayment, decimalNumbersRound), result.PaymentFees[4].Payment);
        }

        [Fact]
        public void ShouldCalculatePaymentMatrix_CreditCreated()
        {
            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMaster(new DateTime(2019, 6, 22), new DateTime(2019, 7, 22));
            Credit credit = creditMaster.Current;
            decimal interestRate = 0.0209971486861902M;

            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            AmortizationScheduleResponse amortizationSchedule = creditsUsesCaseTest.CreditUsesCase.GetOriginalAmortizationSchedule(
                AmortizationScheduleRequest.FromCredit(credit, creditMaster.GetCreditDate, interestRate, downPayment: 0),
                2);

            decimal balance = credit.GetCreditValue;
            decimal assuranceBalance = credit.GetAssuranceValue;

            PaymentMatrix result = PaymentMatrixBuilder.CreateBuilder()
                .BasicInfo(amortizationSchedule, balance, assuranceBalance)
                .DateInfo(lastPaymentDate: new DateTime(), calculationDate: new DateTime())
                .CreditParametersInfo(effectiveAnnualRate: 0M, arrearsEffectiveAnnualRate: 0M, arrearsGracePeriod: 3)
                .Build();

            Assert.NotNull(result);
            Assert.Equal(amortizationSchedule.AmortizationScheduleFees.Last().CreditValuePayment, result.Fees.Last().CreditValuePayment);
            Assert.Equal(amortizationSchedule.AmortizationScheduleAssuranceFees.Last().AssurancePaymentValue, result.Fees.Last().AssuranceValuePayment);
            Assert.Equal(amortizationSchedule.AmortizationScheduleFees.First().CreditValuePayment, result.Fees.First().CreditValuePayment);
            Assert.Equal(amortizationSchedule.AmortizationScheduleAssuranceFees.First().AssurancePaymentValue, result.Fees.First().AssuranceValuePayment);
            Assert.IsType<PaymentMatrix>(result);
        }

        [Fact]
        public void ShouldCalculatePaymentMatrix_CreditPaid()
        {
            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMaster(new DateTime(2019, 6, 22), new DateTime(2019, 7, 22));
            Credit credit = creditMaster.Current;
            decimal interestRate = 0.0209971486861902M;

            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            AmortizationScheduleResponse amortizationSchedule = creditsUsesCaseTest.CreditUsesCase.GetOriginalAmortizationSchedule(
                AmortizationScheduleRequest.FromCredit(credit, creditMaster.GetCreditDate, interestRate),
                2);

            decimal balance = 0;
            decimal assuranceBalance = 0;

            PaymentMatrix result = PaymentMatrixBuilder.CreateBuilder()
                .BasicInfo(amortizationSchedule, balance, assuranceBalance)
                .DateInfo(calculationDate: new DateTime(), lastPaymentDate: new DateTime())
                .CreditParametersInfo(effectiveAnnualRate: 0M, arrearsEffectiveAnnualRate: 0M, arrearsGracePeriod: 3)
                .Build();

            Assert.NotNull(result);
            Assert.Equal(0, result.Fees.Last().CreditValuePayment);
            Assert.Equal(0, result.Fees.Last().AssuranceValuePayment);
            Assert.Equal(0, result.Fees.First().CreditValuePayment);
            Assert.Equal(0, result.Fees.First().AssuranceValuePayment);
            Assert.IsType<PaymentMatrix>(result);
        }

        [Fact]
        public void ShouldCalculatePaymentMatrix()
        {
            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMaster(new DateTime(2019, 6, 22), new DateTime(2019, 7, 22));
            Credit credit = creditMaster.Current;
            decimal interestRate = 0.0343660831319166M;

            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            AmortizationScheduleResponse amortizationSchedule = creditsUsesCaseTest.CreditUsesCase.GetOriginalAmortizationSchedule(
                AmortizationScheduleRequest.FromCredit(credit, creditMaster.GetCreditDate, interestRate, downPayment: 0),
                2);

            decimal balance = 100000;
            decimal assuranceBalance = 0;

            PaymentMatrix result = PaymentMatrixBuilder.CreateBuilder()
                .BasicInfo(amortizationSchedule, balance, assuranceBalance)
                .DateInfo(lastPaymentDate: new DateTime(), calculationDate: new DateTime())
                .CreditParametersInfo(effectiveAnnualRate: 0M, arrearsEffectiveAnnualRate: 0M, arrearsGracePeriod: 3)
                .Build();

            Assert.NotNull(result);
            Assert.Equal(0M, result.Fees.ToList()[4].CreditValuePayment);
            Assert.Equal(3063.46M, result.Fees.ToList()[5].CreditValuePayment);
            Assert.Equal(22785.67M, result.Fees.ToList()[6].CreditValuePayment);
            Assert.IsType<PaymentMatrix>(result);
        }

        [Fact]
        public void ShouldCalculatePaymentMatrixAssurance()
        {
            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMaster(new DateTime(2019, 6, 22), new DateTime(2019, 7, 22));
            Credit credit = creditMaster.Current;
            decimal interestRate = 0.0343660831319166M;

            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            AmortizationScheduleResponse amortizationSchedule = creditsUsesCaseTest.CreditUsesCase.GetOriginalAmortizationSchedule(
                AmortizationScheduleRequest.FromCredit(credit, creditMaster.GetCreditDate, interestRate, downPayment: 0),
                2);

            decimal balance = 100000;
            decimal assuranceBalance = 10000;

            PaymentMatrix result = PaymentMatrixBuilder.CreateBuilder()
                .BasicInfo(amortizationSchedule, balance, assuranceBalance)
                .DateInfo(lastPaymentDate: new DateTime(), calculationDate: new DateTime())
                .CreditParametersInfo(effectiveAnnualRate: 0M, arrearsEffectiveAnnualRate: 0M, arrearsGracePeriod: 3)
                .Build();

            Assert.NotNull(result);
            Assert.Equal(0M, result.Fees.ToList()[5].AssuranceValuePayment);
            Assert.Equal(2488.0M, result.Fees.ToList()[6].AssuranceValuePayment);
            Assert.Equal(credit.GetAssuranceTotalFeeValue, result.Fees.ToList()[7].AssuranceValuePayment);
            Assert.IsType<PaymentMatrix>(result);
        }

        [Fact]
        public void ShouldGetPaymentTemplateSuccessfully()
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

            int decimalNumbersRound = 0;
            PaymentTemplateResponse paymentTemplateResponse = PaymentHelperTest.GetPaymentTemplateResponseHelperTest(decimalNumbersRound);

            string templateExpected = "<style>\r\n  * {\r\n    font-family: Arial, Helvetica, sans-serif;\r\n  }\r\n\r\n  @page {\r\n    margin: 0;\r\n  }\r\n\r\n  @media print {\r\n    @media (min-width: 83mm) and (max-width: 216mm) {\r\n      .img-container {\r\n        width: 300px;\r\n      }\r\n\r\n      .div-header {\r\n        display: flex;\r\n        flex-direction: row;\r\n        height: 120px;\r\n      }\r\n\r\n      .container-x {\r\n        width: 140mm;\r\n        height: 216mm;\r\n        margin: 0 auto;\r\n        margin-bottom: 140mm;\r\n      }\r\n\r\n      .div-logo-siste {\r\n        width: 55%;\r\n      }\r\n\r\n      .lbl-credit-value {\r\n        font-weight: bold;\r\n        font-size: 18pt;\r\n      }\r\n\r\n      .div-credit-value {\r\n        padding-top: 0px;\r\n      }\r\n\r\n      .div-payment-details {\r\n        display: flex;\r\n        flex-direction: row;\r\n      }\r\n\r\n      .personal-data {\r\n        width: 30%;\r\n        margin-left: 15%;\r\n        font-size: 9pt;\r\n      }\r\n\r\n      .lbl-title-payment-data {\r\n        font-weight: bold;\r\n        margin-bottom: 12pt;\r\n      }\r\n\r\n      .footer {\r\n        color: white;\r\n        background-color: #666666;\r\n        text-align: center;\r\n      }\r\n\r\n      p {\r\n        margin: 1mm;\r\n      }\r\n\r\n      .divisor {\r\n        color: #666666;\r\n        height: 76px;\r\n        border: 1px dashed;\r\n        margin-left: 10px;\r\n        margin-right: 70px;\r\n      }\r\n\r\n      .divisor-details-v {\r\n        color: #666666;\r\n        height: 76px;\r\n      }\r\n\r\n      .payment-date {\r\n        font-size: 9pt;\r\n      }\r\n\r\n      .payment-warehouse-name {\r\n        font-size: 9pt;\r\n      }\r\n\r\n      .warehouse-phone {\r\n        font-size: 9pt;\r\n      }\r\n\r\n      .credit-code {\r\n        font-size: 9pt;\r\n      }\r\n\r\n      .cash-receipt {\r\n        font-size: 9pt;\r\n      }\r\n    }\r\n  }\r\n\r\n  .img-container {\r\n    width: 300px;\r\n  }\r\n\r\n  p {\r\n    margin: 1mm;\r\n  }\r\n\r\n  .div-header {\r\n    display: flex;\r\n    flex-direction: row;\r\n    height: 100px;\r\n  }\r\n\r\n  .container-x {\r\n    height: 140mm;\r\n    width: 216mm;\r\n    margin: 0 auto;\r\n    margin-bottom: 140mm;\r\n  }\r\n\r\n  .div-logo-siste {\r\n    width: 52%;\r\n  }\r\n\r\n  .print-date {\r\n    padding-bottom: 29px;\r\n    margin-right: 6%;\r\n    font-size: 8pt;\r\n    text-align: right;\r\n  }\r\n\r\n  .lbl-credit-value {\r\n    font-weight: bold;\r\n    font-size: 18pt;\r\n  }\r\n\r\n  .div-credit-value {\r\n    width: 50%;\r\n    padding-top: 2px;\r\n  }\r\n\r\n  .div-payment-details {\r\n    display: flex;\r\n    flex-direction: row;\r\n  }\r\n\r\n  .personal-data {\r\n    width: 40%;\r\n    margin-left: 2%;\r\n    font-size: 9pt;\r\n  }\r\n\r\n  .lbl-title-payment-data {\r\n    font-weight: bold;\r\n    margin-bottom: 12pt;\r\n  }\r\n\r\n  .footer {\r\n    color: white;\r\n    background-color: #666666;\r\n    text-align: center;\r\n    font-size: 9pt;\r\n    padding: 1%;\r\n  }\r\n\r\n  .divisor {\r\n    color: #666666;\r\n    height: 76px;\r\n    border: 0.5px dashed;\r\n    margin-left: 14px;\r\n    margin-right: 35px;\r\n  }\r\n\r\n  .divisor-details-v {\r\n    color: #747070;\r\n    height: 109px;\r\n    border: 1px solid;\r\n  }\r\n\r\n  .div-fill-fields {\r\n    padding-left: 15px;\r\n    margin-bottom: 20px;\r\n  }\r\n\r\n  .full-name-footer {\r\n    margin-right: 70px;\r\n  }\r\n\r\n  .type-document-footer {\r\n    padding-right: 115px;\r\n  }\r\n\r\n  .payment-date {\r\n    font-size: 9pt;\r\n  }\r\n\r\n  .payment-warehouse-name {\r\n    font-size: 9pt;\r\n  }\r\n\r\n  .purchase-warehouse-name {\r\n    font-size: 9pt;\r\n  }\r\n\r\n  .warehouse-phone {\r\n    font-size: 9pt;\r\n  }\r\n\r\n  .credit-code {\r\n    font-size: 9pt;\r\n  }\r\n\r\n  .cash-receipt {\r\n    font-size: 9pt;\r\n    width: 46%;\r\n  }\r\n\r\n  .container-details {\r\n    display: flex;\r\n    flex-direction: row;\r\n    background-color: #e7e7e7;\r\n  }\r\n\r\n  .block-1 {\r\n    margin-top: 1%;\r\n    width: 40%;\r\n    margin-left: 2%;\r\n    font-size: 9pt;\r\n  }\r\n\r\n  .block-2 {\r\n    width: 432px;\r\n    margin-right: 2%;\r\n    font-size: 9pt;\r\n  }\r\n\r\n  .icon-balance {\r\n    background-image: url(../../../../assets/img/saldo_credito.svg);\r\n    background-repeat: round;\r\n    background-size: cover;\r\n    width: 45px;\r\n    height: 40px;\r\n    margin-right: 5px;\r\n  }\r\n\r\n  .icon-date {\r\n    background-image: url(../../../../assets/img/dia_proximo_pago.svg);\r\n    background-repeat: round;\r\n    background-size: cover;\r\n    width: 45px;\r\n    height: 40px;\r\n    margin-right: 5px;\r\n  }\r\n\r\n  .footer-greetings {\r\n    padding-top: 15px;\r\n    text-align: center;\r\n    padding-bottom: 15px;\r\n    font-style: italic;\r\n  }\r\n\r\n  .subblock-2 {\r\n    padding-left: 10px;\r\n    padding-top: 20px;\r\n    padding-right: 10px;\r\n    display: flex;\r\n    flex-direction: row;\r\n    padding-bottom: 5px;\r\n  }\r\n\r\n  .subblock-3 {\r\n    display: flex;\r\n    flex-direction: row;\r\n    padding-left: 25px;\r\n    font-weight: bold;\r\n    font-size: 14pt;\r\n  }\r\n\r\n  .title-thanks-payment{\r\n    font-size: 12pt;\r\n  }\r\n\r\n  .text-thanks-payment{\r\n    font-size: 9pt;\r\n  }\r\n\r\n  .available-amount {\r\n    font-weight: 100;\r\n  }\r\n\r\n  .divisor-details-h {\r\n    width: 390px;\r\n    border: 1px solid black;\r\n  }\r\n\r\n  .header-detail {\r\n    line-height: 51px;\r\n    background-color: #BBBBBB;\r\n    height: 49px;\r\n    font-size: 18pt;\r\n    font-weight: 600;\r\n    color: white;\r\n    text-align: center;\r\n    margin: 0%;\r\n    margin-top: 1%;\r\n  }\r\n\r\n  .div-contact {\r\n    display: none;\r\n  }\r\n\r\n  .credit-value-tirilla {\r\n    display: none;\r\n  }\r\n\r\n  .date-tirilla {\r\n    display: none;\r\n  }\r\n\r\n  .div-info-payment{\r\n    display: flex;\r\n  }\r\n\r\n  .div-reference-code {\r\n    display: flex;\r\n    flex-direction: row;\r\n  }\r\n\r\n  @media print {\r\n    @page{\r\n    margin-top: 2%;\r\n    }\r\n    body{\r\n      margin: 0;\r\n    }\r\n    @media (max-width: 77mm) {\r\n\r\n      .container-x{\r\n        display: contents;\r\n      }\r\n\r\n      .div-logo-siste{\r\n        margin-left: 15%;\r\n        margin-bottom: 0;\r\n        height: 0;\r\n        padding-top: 1%;\r\n      }\r\n\r\n      .img-container{\r\n        width: 130%;\r\n      }\r\n\r\n      .div-header {\r\n        flex-direction: column;\r\n        text-align: center;\r\n        height: 7%;\r\n      }\r\n\r\n      .div-contact {\r\n        margin-top: 2%;\r\n        margin-bottom: 10%;\r\n        display: block;\r\n        text-align: center;\r\n        width: 100%;\r\n      }\r\n\r\n      .div-payment-details {\r\n        flex-direction: column;\r\n      }\r\n\r\n      .lbl-title-payment-data {\r\n        margin-bottom: 0;\r\n      }\r\n\r\n      .container-details {\r\n        margin-top: 2%;\r\n        flex-direction: column;\r\n        background: white;\r\n      }\r\n\r\n      .div-reference-code {\r\n        flex-direction: column;\r\n      }\r\n\r\n      .div-credit-value {\r\n        display: none;\r\n      }\r\n\r\n      .personal-data{\r\n        margin-left: 4%;\r\n        width: 100%;\r\n      }\r\n\r\n      .credit-value-tirilla {\r\n        display: block;\r\n        text-align: center;\r\n        width: 100%;\r\n        margin-top: 6%;\r\n        margin-bottom: 6%;\r\n      }\r\n\r\n      .lbl-credit-value {\r\n        margin: 0 auto;\r\n      }\r\n\r\n      .divisor {\r\n        margin-top: 2%;\r\n        margin-bottom: 2%;\r\n        margin-left: 0;\r\n        height: 0;\r\n        width: 100%;\r\n        border: none;\r\n        border-top: 3px dashed;\r\n      }\r\n\r\n      .header-detail {\r\n        line-height: 23px;\r\n        background-color: white;\r\n        height: 30px;\r\n        width: 100%;\r\n        font-size: 15px;\r\n        border-bottom: 2px solid;\r\n        border-top: 2px solid;\r\n        color: black;\r\n      }\r\n\r\n      .store-data{\r\n        margin-left: 4%;\r\n      }\r\n\r\n      .subblock-2{\r\n        margin-left: 5%;\r\n        flex-direction: column;\r\n        width: 80%;\r\n      }\r\n\r\n      .div-info-payment{\r\n        margin-bottom: 4%;\r\n      }\r\n\r\n      .block-1 {\r\n        margin-bottom: 2%;\r\n        margin-left: 3%;\r\n        width: 100%;\r\n      }\r\n\r\n      .block-2 {\r\n        width: 100%;\r\n        background: rgba(212, 212, 212, 0.986);\r\n      }\r\n\r\n      .icon-balance{\r\n        margin-right: 10%;\r\n      }\r\n\r\n      .icon-date{\r\n        margin-right: 10%;\r\n      }\r\n\r\n      .divisor-details-h {\r\n        width: 80%;\r\n      }\r\n\r\n      .divisor-details-v {\r\n        display: none;\r\n      }\r\n\r\n      .footer-greetings {\r\n        margin-top: 4%;\r\n        width: 100%;\r\n      }\r\n\r\n      .title-thanks-payment {\r\n        margin-bottom: 4%;\r\n      }\r\n\r\n      .text-thanks-payment {\r\n        margin: 0 auto;\r\n        width: 70%;\r\n      }\r\n\r\n      .date-tirilla {\r\n        margin-top: 2%;\r\n        display: block;\r\n      }\r\n\r\n      .print-date {\r\n        text-align: center;\r\n        margin-bottom: 70%;\r\n      }\r\n\r\n      .footer {\r\n        display: none;\r\n      }\r\n      .subblock-3{\r\n        margin: 0 auto;\r\n        width: 65%;\r\n        padding: 0;\r\n        text-align: center;\r\n        margin-top: 3%;\r\n        font-size: 12pt;\r\n        margin-bottom: 2%;\r\n      }\r\n    }\r\n  }\r\n</style>\r\n\r\n<div class=\"container-x\">\r\n  <div class=\"div-header\">\r\n    <div class=\"div-logo-siste\">\r\n      <img class=\"img-container\" src=\"../../../../assets/img/Impresiones_LogoSiste.png\" />\r\n    </div>\r\n    <div class=\"div-credit-value\">\r\n      <p class=\"print-date\">\r\n        Fecha de impresión: <span>" + new DateTime(2020, 1, 20, 10, 55, 48) + "</span>\r\n      </p>\r\n      <p class=\"lbl-credit-value\">Valor cancelado: $13,170</p>\r\n    </div>\r\n  </div>\r\n  <div class=\"div-contact\">\r\n    <p>NIT. 854689741</p>\r\n    <p>Contáctanos: 3208899898</p>\r\n  </div>\r\n  <div class=\"div-payment-details\">\r\n    <div class=\"personal-data\">\r\n      <p class=\"lbl-title-payment-data\">\r\n        Estos son los datos de tu pago:\r\n      </p>\r\n      <p class=\"full-name\">Nombre completo: <span>SANTIAGO SERNA HIGUITA</span></p>\r\n      <p class=\"documentType\">CC: 1152686129</p>\r\n    </div>\r\n    <hr class=\"divisor\" />\r\n    <div class=\"store-data\">\r\n      <p class=\"payment-date\">Fecha del pago: <span>" + new DateTime(2020, 1, 15).ToShortDateString() + "</span></p>\r\n      <p class=\"payment-warehouse-name\">\r\n        Almacén donde realizaste el pago: <span>Tienda Meli</span>\r\n      </p>\r\n      <p class=\"purchase-warehouse-name\">\r\n        Almacén donde hiciste la compra: <span>Tienda Meli</span>\r\n      </p>\r\n      <p class=\"warehouse-phone\">Teléfono almacén: <span>2585858</span></p>\r\n      <div class=\"div-reference-code\">\r\n        <p class=\"cash-receipt\">Recibo de caja: <span>444</span></p>\r\n        <p class=\"credit-code\">Código del crédito: <span>221</span></p>\r\n      </div>\r\n    </div>\r\n  </div>\r\n  <div class=\"credit-value-tirilla\">\r\n    <p class=\"lbl-credit-value\">Valor cancelado: <br>$13,170</p>\r\n  </div>\r\n  <div class=\"header-detail\">\r\n    <p>DETALLES DEL PAGO</p>\r\n  </div>\r\n  <div class=\"container-details\">\r\n    <div class=\"block-1\">\r\n      <p>Valor cancelado capital: <span>$9,600</span></p>\r\n      <p>Valor cancelado financiación: <span>$0</span></p>\r\n      <p>Valor cancelado mora: <span>$0</span></p>\r\n      <p>Valor cancelado aval: <span>$3,570</span></p>\r\n      <p>Valor cancelado cargos: <span>$0</span></p>\r\n      <p>Valor IVA aval: <span>$570</span></p>\r\n    </div>\r\n    <hr class=\"divisor-details-v\" />\r\n    <div class=\"block-2\">\r\n      <div class=\"subblock-2\">\r\n        <div class=\"div-info-payment\">\r\n          <i class=\"icon-balance\" id=\"icon-compromise\"></i>\r\n          <div style=\"font-weight: bold;\">\r\n            <p>Saldo crédito</p>\r\n            <span style=\"margin-left: 7%;\">$110,400</span>\r\n          </div>\r\n        </div>\r\n        <div class=\"div-info-payment\">\r\n          <i class=\"icon-date\" id=\"icon-compromise\"></i>\r\n          <div>\r\n            <p style=\"font-weight: bold;\">Día de tu próximo pago</p>\r\n            <p style=\"margin-left: 4%;\">" + new DateTime(2020, 2, 15).ToShortDateString() + "</p>\r\n          </div>\r\n        </div>\r\n      </div>\r\n      <hr class=\"divisor-details-h\" />\r\n      <div class=\"subblock-3\">\r\n        <p>CUPO DISPONIBLE: <span class=\"available-amount\"> $45,730,620</span></p>\r\n      </div>\r\n    </div>\r\n  </div>\r\n  <div class=\"footer-greetings\">\r\n    <p class=\"title-thanks-payment\"><b>¡Gracias por tu pago!</b></p>\r\n    <p class=\"text-thanks-payment\">\r\n      Recuerda que tener tus créditos al día te permite disfrutar de tu cupo\r\n      disponible en nuestros almacenes aliados.\r\n    </p>\r\n  </div>\r\n  <div class=\"date-tirilla\">\r\n    <p class=\"print-date\">\r\n      Fecha de impresión: <span>" + new DateTime(2020, 1, 20, 10, 55, 48) + "</span>\r\n    </p>\r\n  </div>\r\n  <div class=\"footer\">\r\n    Si tienes alguna duda, comunícate con nosotros al 3208899898  - <b>SISTECRÉDITO S.A.S. NIT. 854689741</b>\r\n  </div>\r\n</div>\r\n";

            string template = CreditPaymentUsesCase.GetPaymentTemplate(paymentTemplateResponse, decimalNumbersRound, false);

            Assert.NotNull(template);
            Assert.Equal(templateExpected, template);
        }

        [Fact]
        public void ShouldGetPayMailNotification()
        {
            string[] expectedTemplateKeys =
                new string[]
                {
                    "{{customer.FullName}}",
                    "{{customer.Email}}",
                    "{{payValueLetter}}",
                    "{{payCreditResponse.PaymentId}}",
                    "{{valuePaidToCapital}}",
                    "{{valuePaidToInterest}}",
                    "{{valuePaidToArrears}}",
                    "{{valuePaidToAssurance}}",
                    "{{valuePaidToCharges}}",
                    "{{valuePaidToAssuranceTax}}",
                    "{{AvalCompany}}",
                    "{{payValue}}",
                    "{{date}}",
                    "{{store.StoreName}}",
                    "{{TypId}}",
                    "{{DocumentId}}",
                    "{{paymentNumber}}",
                    "{{nextDueDate}}",
                    "{{creditNumber}}",
                };

            MailNotificationRequest paymentMailNotificationRequest = GetPayMailNotification();

            Assert.NotNull(paymentMailNotificationRequest);
            Assert.Equal(expectedTemplateKeys.Count(), paymentMailNotificationRequest.TemplateInfo.TemplateValues.Count);
            Assert.True(paymentMailNotificationRequest.TemplateInfo.TemplateValues.All(templateValue => expectedTemplateKeys.Contains(templateValue.Key)));
        }

        [Fact]
        public void ShouldGetPayMailNotificationWithNextDueDate()
        {
            MailNotificationRequest paymentMailNotificationRequest = GetPayMailNotification(DateTime.Today.AddDays(1));

            Assert.NotNull(paymentMailNotificationRequest);
        }

        [Fact]
        public void ShouldGetPayMailNotificationWithInmediateDueDate()
        {
            MailNotificationRequest paymentMailNotificationRequest = GetPayMailNotification(DateTime.Today.AddDays(-1));

            Assert.NotNull(paymentMailNotificationRequest);
        }

        private MailNotificationRequest GetPayMailNotification(DateTime? nextDueDate = null)
        {
            PaymentCreditResponse paymentCreditResponse = ModelHelperTest.InstanceModel<PaymentCreditResponse>();

            paymentCreditResponse.Store = StoreHelperTest.GetStore();
            paymentCreditResponse.Store.AssuranceCompany = new AssuranceCompany("Test company");
            paymentCreditResponse.CreditMaster = CreditMasterHelperTest.GetCreditMaster();
            paymentCreditResponse.Credit = paymentCreditResponse.CreditMaster.Current;
            paymentCreditResponse.NextDueDate = nextDueDate;

            return CreditPaymentUsesCase.GetPayMailNotification(paymentCreditResponse, decimalNumbersRound: 2, taxValue: 0.19M);
        }

        [Fact]
        public void ShouldGetSmsNotification()
        {
            string testTemplate = "Test template";

            SmsNotificationRequest smsNotificationRequest = CreditPaymentUsesCase.GetSmsNotification(testTemplate, StoreHelperTest.GetStore(),
                CreditMasterHelperTest.GetCreditMaster().Current, decimalNumberRound: 2);

            Assert.NotNull(smsNotificationRequest);
        }

        [Fact]
        public void ShouldGetPaymentHistory()
        {
            List<CreditMaster> creditMasters = CreditMasterHelperTest.GetCreditMasterList();

            creditMasters.ForEach(creditMaster =>
                Enumerable.Range(1, 5).ToList().ForEach(_ =>
                    CreditMasterHelperTest.AddTransaction(creditMaster, TransactionTypes.Payment, creditMaster.Store, Statuses.Active, DateTime.Now)));

            List<Credit> payments =
                creditMasters
                    .SelectMany(creditMaster =>
                        creditMaster.History
                            .Where(credit =>
                                credit.CreditPayment.GetTransactionTypeId == (int)TransactionTypes.Payment))
                    .ToList();

            List<RequestCancelPayment> requestCancelPayments = RequestCancelPaymentHelperTest.GetRequestCancelPaymentList();

            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            List<PaymentHistoryResponse> paymentHistoryResponses = CreditPaymentUsesCase.CreatePaymentHistory(payments, requestCancelPayments, parameters);

            Assert.NotNull(paymentHistoryResponses);
            Assert.NotEmpty(paymentHistoryResponses);
        }

        [Fact]
        public void ShouldGetPaymentHistoryWithCanceledPayments()
        {
            List<CreditMaster> creditMasters = CreditMasterHelperTest.GetCreditMasterList();

            creditMasters.ForEach(creditMaster =>
                Enumerable.Range(1, 5).ToList().ForEach(_ =>
                    CreditMasterHelperTest.AddTransaction(creditMaster, TransactionTypes.Payment, creditMaster.Store, Statuses.Active, DateTime.Now)));

            List<Credit> payments =
                creditMasters
                    .SelectMany(creditMaster =>
                        creditMaster.History
                            .Where(credit =>
                                credit.CreditPayment.GetTransactionTypeId == (int)TransactionTypes.Payment))
                    .ToList();

            creditMasters.ForEach(creditMaster => creditMaster.History.ForEach(credit =>
            {
                credit.SetCreditMaster(creditMaster);
                credit.SetStore(creditMaster.Store);
            }));

            List<RequestCancelPayment> requestCancelPayments = RequestCancelPaymentHelperTest.GetRequestCancelPaymentsFromPayments(payments);

            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            List<PaymentHistoryResponse> paymentHistoryResponses = CreditPaymentUsesCase.CreatePaymentHistory(payments, requestCancelPayments, parameters);

            List<PaymentHistoryResponse> canceledHistoryResponses =
                paymentHistoryResponses
                    .Where(history =>
                        history.StatusId == (int)RequestStatuses.Cancel && history.CancelDate != null)
                    .ToList();

            Assert.NotNull(paymentHistoryResponses);
            Assert.NotEmpty(paymentHistoryResponses);
            Assert.Equal(requestCancelPayments.Count, canceledHistoryResponses.Count);
        }

        [Fact]
        public void GetCurrentAmortizationSchedule_ArrearGraceDays_ShouldNotBeLate()
        {
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            var currentAmortizationScheduleRequest = CurrentAmortizationScheduleHelperTest.GetCurrentAmortizationScheduleRequest();
            var amortizationScheduleResponse = AmortizationScheduleHelperTest.GetAmortizationScheduleResponse();
            DateTime calculationDate = new DateTime(2021, month: 01, day: 09);

            _creditUseCaseMock.Setup(creditUseCase => creditUseCase.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(),
                It.IsAny<int>())).Returns(amortizationScheduleResponse);

            var result = CreditPaymentUsesCase.GetCurrentAmortizationSchedule(currentAmortizationScheduleRequest, calculationDate, parameters);

            Assert.All(result.CurrentAmortizationScheduleFees.Where(fee => fee.FeeStatus == FeeStatuses.PAID),
                paidFee => Assert.Equal(0, paidFee.Balance));
            Assert.Equal(1, result.CurrentAmortizationScheduleFees.Count(fee => fee.FeeStatus == FeeStatuses.LATE));
            Assert.Equal(FeeStatuses.LATE, result.CurrentAmortizationScheduleFees[7].FeeStatus);
        }

        [Fact]
        public void GetCurrentAmortizationSchedule_With_ArrearsCharges_Charges()
        {
            //Arrange
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            var currentAmortizationScheduleRequest = CurrentAmortizationScheduleHelperTest.GetCurrentAmortizationSchedule_WithArrears();
            var amortizationScheduleResponse = AmortizationScheduleHelperTest.GetAmortizationScheduleResponse();
            DateTime calculationDate = new DateTime(2021, month: 01, day: 10);

            var expectedChargeValue = currentAmortizationScheduleRequest.ArrearsCharges + currentAmortizationScheduleRequest.ChargeValue;

            _creditUseCaseMock.Setup(creditUseCase => creditUseCase.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(),
                                                             It.IsAny<int>())).Returns(amortizationScheduleResponse);

            //Act

            var result = CreditPaymentUsesCase.GetCurrentAmortizationSchedule(currentAmortizationScheduleRequest, calculationDate, parameters);
            var feeToValidate = result.CurrentAmortizationScheduleFees.FirstOrDefault(value => value.Balance > 0);

            //Assert
            Assert.Equal(expectedChargeValue, feeToValidate.ChargeValue);
            Assert.Equal(FeeStatuses.LATE, feeToValidate.FeeStatus);
            Assert.Equal(0, feeToValidate.ArrearPaymentValue);
        }

        [Fact]
        public void GetCurrentAmortizationSchedule_PreviousInterest_PreviousArrears()
        {
            //Arrange
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            var currentAmortizationScheduleRequest = CurrentAmortizationScheduleHelperTest.GetCurrentAmortizationSchedule_PreviousInterest_PreviousArrears();
            var amortizationScheduleResponse = AmortizationScheduleHelperTest.GetAmortizationScheduleResponse();
            DateTime calculationDate = new DateTime(2021, month: 02, day: 28);

            _creditUseCaseMock.Setup(creditUseCase => creditUseCase.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(),
             It.IsAny<int>())).Returns(amortizationScheduleResponse);

            _creditUseCaseMock.SetupSequence(creditUseCase => creditUseCase.GetInterestRate(It.IsAny<decimal>(),
                                                                            It.IsAny<int>())).Returns(0.052835M)
                                                                                             .Returns(0.009284M);
            //Act

            var result = CreditPaymentUsesCase.GetCurrentAmortizationSchedule(currentAmortizationScheduleRequest, calculationDate, parameters);
            var validateInterestValue = result.CurrentAmortizationScheduleFees.FirstOrDefault(value => value.InterestValue > 0);
            var validateArrearPaymentValue = result.CurrentAmortizationScheduleFees.FirstOrDefault(value => value.ArrearPaymentValue > 0);

            //Assert
            Assert.Equal(11781.32M, validateInterestValue.InterestValue);
            Assert.Equal(3130.90M, validateArrearPaymentValue.ArrearPaymentValue);
        }

        [Fact]
        public void GetCurrentAmortizationSchedule_WithInterestValue()
        {
            //Arrange
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            var currentAmortizationScheduleRequest = CurrentAmortizationScheduleHelperTest.GetCurrentAmortizationScheduleRequest();
            var amortizationScheduleResponse = AmortizationScheduleHelperTest.GetAmortizationScheduleResponse();
            DateTime calculationDate = new DateTime(2021, month: 01, day: 05);

            _creditUseCaseMock.Setup(creditUseCase => creditUseCase.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(),
             It.IsAny<int>())).Returns(amortizationScheduleResponse);

            _creditUseCaseMock.SetupSequence(creditUseCase => creditUseCase.GetInterestRate(It.IsAny<decimal>(),
                                                                            It.IsAny<int>())).Returns(0.052835M)
                                                                                             .Returns(0.009284M);
            //Act

            var result = CreditPaymentUsesCase.GetCurrentAmortizationSchedule(currentAmortizationScheduleRequest, calculationDate, parameters);
            var feeToValidate = result.CurrentAmortizationScheduleFees.FirstOrDefault(value => value.InterestDays > 0);
            var arrearValueValidate = result.CurrentAmortizationScheduleFees.FirstOrDefault(value => value.FeeStatus == FeeStatuses.LATE);

            //Assert
            Assert.Equal(78, feeToValidate.InterestDays);
            Assert.Equal(87730.30M, feeToValidate.InterestValue);
            Assert.Null(arrearValueValidate);
        }

        [Fact]
        public void GetCurrentAmortizationSchedule_ValidateLastPaymentDate()
        {
            //Arrange
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            var currentAmortizationScheduleRequest = CurrentAmortizationScheduleHelperTest.GetCurrentAmortizationScheduleRequest();
            currentAmortizationScheduleRequest.LastPaymentDate = new DateTime(2021, month: 02, day: 15);
            var amortizationScheduleResponse = AmortizationScheduleHelperTest.GetAmortizationScheduleResponse();
            DateTime calculationDate = new DateTime(2021, month: 02, day: 15);

            _creditUseCaseMock.Setup(creditUseCase => creditUseCase.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(),
             It.IsAny<int>())).Returns(amortizationScheduleResponse);

            _creditUseCaseMock.SetupSequence(creditUseCase => creditUseCase.GetInterestRate(It.IsAny<decimal>(),
                                                                            It.IsAny<int>())).Returns(0.0M)
                                                                                             .Returns(0.0M)
                                                                                             .Returns(0.0M)
                                                                                             .Returns(0.001321M);
            //Act

            var result = CreditPaymentUsesCase.GetCurrentAmortizationSchedule(currentAmortizationScheduleRequest, calculationDate, parameters);
            var feeToValidate = result.CurrentAmortizationScheduleFees.FirstOrDefault(value => value.InterestDays > 0);

            //Assert
            Assert.Equal(2, feeToValidate.InterestDays);
            Assert.Equal(1211.19M, feeToValidate.InterestValue);
        }

        [Fact]
        public void GetCurrentAmortizationSchedule_ValidateArrearsDays()
        {
            //Arrange
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            var currentAmortizationScheduleRequest = CurrentAmortizationScheduleHelperTest.GetCurrentAmortizationScheduleRequest();

            var amortizationScheduleResponse = AmortizationScheduleHelperTest.GetAmortizationScheduleResponse();
            DateTime calculationDate = new DateTime(2021, month: 04, day: 30);

            _creditUseCaseMock.Setup(creditUseCase => creditUseCase.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(),
             It.IsAny<int>())).Returns(amortizationScheduleResponse);

            //Act

            var result = CreditPaymentUsesCase.GetCurrentAmortizationSchedule(currentAmortizationScheduleRequest, calculationDate, parameters);
            var feeToValidate = result.CurrentAmortizationScheduleFees.LastOrDefault();

            //Assert

            Assert.Equal(44, feeToValidate.ArrearDays);
            Assert.Equal(14, feeToValidate.InterestDays);
        }

        [Fact]
        public void GetCurrentAmortizationSchedule_JustPendigLastFee()
        {
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            var currentAmortizationScheduleRequest = CurrentAmortizationScheduleHelperTest.GetCurrentAmortizationSchedule_JustLastFeePending_Request();
            var amortizationScheduleResponse = AmortizationScheduleHelperTest.GetAmortizationScheduleResponse();
            DateTime calculationDate = new DateTime(2021, month: 03, day: 03);

            _creditUseCaseMock.Setup(creditUseCase => creditUseCase.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(),
                It.IsAny<int>())).Returns(amortizationScheduleResponse);

            var result = CreditPaymentUsesCase.GetCurrentAmortizationSchedule(currentAmortizationScheduleRequest, calculationDate, parameters);

            Assert.All(result.CurrentAmortizationScheduleFees.Where(fee => fee.FeeStatus == FeeStatuses.PAID),
                paidFee => Assert.Equal(0, paidFee.Balance));
            Assert.Equal(1, result.CurrentAmortizationScheduleFees.Count(fee => fee.FeeStatus == FeeStatuses.CURRENT));
            Assert.Equal(result.CurrentAmortizationScheduleFees.Count - 1, result.CurrentAmortizationScheduleFees.Count(fee => fee.FeeStatus == FeeStatuses.PAID));
            Assert.Equal(FeeStatuses.CURRENT, result.CurrentAmortizationScheduleFees[result.CurrentAmortizationScheduleFees.Count - 1].FeeStatus);
        }

        [Fact]
        public void GetCurrentAmortizationSchedule_JustFirstFee_IsPay()
        {
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            var currentAmortizationScheduleRequest = CurrentAmortizationScheduleHelperTest.GetCurrentAmortizationSchedule_FirstFeePay_Request();
            var amortizationScheduleResponse = AmortizationScheduleHelperTest.GetAmortizationScheduleResponse();
            DateTime calculationDate = new DateTime(2020, month: 10, day: 13);

            _creditUseCaseMock.Setup(creditUseCase => creditUseCase.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(),
                It.IsAny<int>())).Returns(amortizationScheduleResponse);

            var result = CreditPaymentUsesCase.GetCurrentAmortizationSchedule(currentAmortizationScheduleRequest, calculationDate, parameters);

            Assert.All(result.CurrentAmortizationScheduleFees.Where(fee => fee.FeeStatus == FeeStatuses.PAID),
                paidFee => Assert.Equal(0, paidFee.Balance));
            Assert.Equal(11, result.CurrentAmortizationScheduleFees.Count(fee => fee.FeeStatus == FeeStatuses.CURRENT));
            Assert.Equal(FeeStatuses.PAID, result.CurrentAmortizationScheduleFees[0].FeeStatus);
            Assert.Equal(FeeStatuses.PAID, result.CurrentAmortizationScheduleFees[1].FeeStatus);
        }

        [Fact]
        public void GetCurrentPaymentSchedule_ArrearGraceDays_ShouldNotBeLate()
        {
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            var currentPaymentScheduleRequest = CurrentPaymentScheduleHelperTest.GetCurrentPaymentScheduleRequest();
            var amortizationScheduleResponse = AmortizationScheduleHelperTest.GetAmortizationScheduleResponse();
            DateTime calculationDate = new DateTime(2021, month: 01, day: 09);

            _creditUseCaseMock.Setup(creditUseCase => creditUseCase.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(),
                It.IsAny<int>())).Returns(amortizationScheduleResponse);

            var result = CreditPaymentUsesCase.GetCurrentPaymentSchedule(currentPaymentScheduleRequest, calculationDate, parameters);

            Assert.All(result.PaymentCreditScheduleFees.Where(fee => fee.FeeStatus == FeeStatuses.PAID),
                paidFee => Assert.Equal(0, paidFee.Balance));
            Assert.Equal(1, result.PaymentCreditScheduleFees.Count(fee => fee.FeeStatus == FeeStatuses.LATE));
            Assert.Equal(FeeStatuses.LATE, result.PaymentCreditScheduleFees[0].FeeStatus);
        }

        [Fact]
        public void GetCurrentPaymentSchedule_PreviousInterest_PreviousArrears()
        {
            //Arrange
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            var currentPaymentScheduleRequest = CurrentPaymentScheduleHelperTest.GetCurrentPaymentSchedule_PreviousInterest_PreviousArrears();
            var amortizationScheduleResponse = AmortizationScheduleHelperTest.GetAmortizationScheduleResponse();
            DateTime calculationDate = new DateTime(2021, month: 02, day: 28);

            _creditUseCaseMock.Setup(creditUseCase => creditUseCase.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(),
             It.IsAny<int>())).Returns(amortizationScheduleResponse);

            _creditUseCaseMock.SetupSequence(creditUseCase => creditUseCase.GetInterestRate(It.IsAny<decimal>(),
                                                                            It.IsAny<int>())).Returns(0.052835M)
                                                                                             .Returns(0.009284M);
            //Act

            var result = CreditPaymentUsesCase.GetCurrentPaymentSchedule(currentPaymentScheduleRequest, calculationDate, parameters);
            var validateInterestValue = result.PaymentCreditScheduleFees.FirstOrDefault(value => value.InterestValue > 0);
            var validateArrearPaymentValue = result.PaymentCreditScheduleFees.FirstOrDefault(value => value.ArrearPaymentValue > 0);

            //Assert
            Assert.Equal(11781.32M, validateInterestValue.InterestValue);
            Assert.Equal(3130.90M, validateArrearPaymentValue.ArrearPaymentValue);
        }

        [Fact]
        public void GetCurrentPaymentSchedule_With_ArrearsCharges_Charges()
        {
            //Arrange
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            var currentPaymentScheduleRequest = CurrentPaymentScheduleHelperTest.GetCurrentPaymentSchedule_WithArrears();
            var amortizationScheduleResponse = AmortizationScheduleHelperTest.GetAmortizationScheduleResponse();
            DateTime calculationDate = new DateTime(2021, month: 01, day: 10);

            var expectedChargeValue = currentPaymentScheduleRequest.ArrearsCharges + currentPaymentScheduleRequest.ChargeValue;

            _creditUseCaseMock.Setup(creditUseCase => creditUseCase.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(),
                                                             It.IsAny<int>())).Returns(amortizationScheduleResponse);

            //Act

            var result = CreditPaymentUsesCase.GetCurrentPaymentSchedule(currentPaymentScheduleRequest, calculationDate, parameters);
            var feeToValidate = result.PaymentCreditScheduleFees.FirstOrDefault(value => value.Balance > 0);

            //Assert
            Assert.Equal(expectedChargeValue, feeToValidate.ChargeValue);
            Assert.Equal(FeeStatuses.LATE, feeToValidate.FeeStatus);
            Assert.Equal(0, feeToValidate.ArrearPaymentValue);
        }

        [Fact]
        public void GetCurrentPaymentSchedule_With_Charges()
        {
            //Arrange
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            var currentPaymentScheduleRequest = CurrentPaymentScheduleHelperTest.GetCurrentPaymentSchedule_WithCharges();
            var amortizationScheduleResponse = AmortizationScheduleHelperTest.GetAmortizationScheduleResponse();
            DateTime calculationDate = new DateTime(2021, month: 01, day: 10);

            var expectedChargeValue = currentPaymentScheduleRequest.ChargeValue;

            _creditUseCaseMock.Setup(creditUseCase => creditUseCase.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(),
                                                             It.IsAny<int>())).Returns(amortizationScheduleResponse);

            //Act
            var result = CreditPaymentUsesCase.GetCurrentPaymentSchedule(currentPaymentScheduleRequest, calculationDate, parameters);
            var feeToValidate = result.PaymentCreditScheduleFees.FirstOrDefault(value => value.Balance > 0);

            //Assert
            Assert.Equal(expectedChargeValue, feeToValidate.ChargeValue);
            Assert.Equal(FeeStatuses.LATE, feeToValidate.FeeStatus);
            Assert.NotEqual(0, feeToValidate.ArrearPaymentValue);
        }

        [Fact]
        public void GetCurrentPaymentSchedule_WithInterestValue()
        {
            //Arrange
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            var currentPaymentScheduleRequest = CurrentPaymentScheduleHelperTest.GetCurrentPaymentScheduleRequest();
            var amortizationScheduleResponse = AmortizationScheduleHelperTest.GetAmortizationScheduleResponse();
            DateTime calculationDate = new DateTime(2021, month: 01, day: 05);

            _creditUseCaseMock.Setup(creditUseCase => creditUseCase.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(),
             It.IsAny<int>())).Returns(amortizationScheduleResponse);

            _creditUseCaseMock.SetupSequence(creditUseCase => creditUseCase.GetInterestRate(It.IsAny<decimal>(),
                                                                            It.IsAny<int>())).Returns(0.052835M)
                                                                                             .Returns(0.009284M);
            //Act

            CurrentPaymentScheduleResponse result = CreditPaymentUsesCase.GetCurrentPaymentSchedule(currentPaymentScheduleRequest, calculationDate, parameters);
            var feeToValidate = result.PaymentCreditScheduleFees.FirstOrDefault(value => value.InterestDays > 0);
            var arrearValueValidate = result.PaymentCreditScheduleFees.FirstOrDefault(value => value.FeeStatus == FeeStatuses.LATE);

            //Assert

            Assert.Equal(77, feeToValidate.InterestDays);
            Assert.Equal(233569.32M, result.MinimumPayment);
            Assert.Equal(1747035.32M, result.TotalPayment);
            Assert.Equal(87730.30M, feeToValidate.InterestValue);
            Assert.Null(arrearValueValidate);
        }

        [Fact]
        public void GetCurrentPaymentSchedule_ValidateLastPaymentDate()
        {
            //Arrange
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            CurrentPaymentScheduleRequest currentAmortizationScheduleRequest = CurrentPaymentScheduleHelperTest.GetCurrentPaymentScheduleRequest();
            currentAmortizationScheduleRequest.LastPaymentDate = new DateTime(2021, month: 02, day: 15);
            var amortizationScheduleResponse = AmortizationScheduleHelperTest.GetAmortizationScheduleResponse();
            DateTime calculationDate = new DateTime(2021, month: 02, day: 17);

            _creditUseCaseMock.Setup(creditUseCase => creditUseCase.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(),
             It.IsAny<int>())).Returns(amortizationScheduleResponse);

            _creditUseCaseMock.SetupSequence(creditUseCase => creditUseCase.GetInterestRate(It.IsAny<decimal>(),
                                                                            It.IsAny<int>())).Returns(0.0M)
                                                                                             .Returns(0.0M)
                                                                                             .Returns(0.0M)
                                                                                             .Returns(0.001321M);
            //Act

            var result = CreditPaymentUsesCase.GetCurrentPaymentSchedule(currentAmortizationScheduleRequest, calculationDate, parameters);
            var feeToValidate = result.PaymentCreditScheduleFees.FirstOrDefault(value => value.InterestDays > 0);

            //Assert
            Assert.Equal(2, feeToValidate.InterestDays);
            Assert.Equal(1211.19M, feeToValidate.InterestValue);
        }

        [Fact]
        public void GetCurrentPaymentSchedule_ValidateArrearsDays()
        {
            //Arrange
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            var currentPaymentScheduleRequest = CurrentPaymentScheduleHelperTest.GetCurrentPaymentScheduleRequest();

            var amortizationScheduleResponse = AmortizationScheduleHelperTest.GetAmortizationScheduleResponse();
            DateTime calculationDate = new DateTime(2021, month: 04, day: 30);

            _creditUseCaseMock.Setup(creditUseCase => creditUseCase.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(),
             It.IsAny<int>())).Returns(amortizationScheduleResponse);

            //Act

            var result = CreditPaymentUsesCase.GetCurrentPaymentSchedule(currentPaymentScheduleRequest, calculationDate, parameters);
            var feeToValidate = result.PaymentCreditScheduleFees.LastOrDefault();

            //Assert

            Assert.Equal(44, feeToValidate.ArrearDays);
            Assert.Equal(14, feeToValidate.InterestDays);
        }

        [Fact]
        public void GetCurrentPaymentSchedule_JustPendigLastFee()
        {
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            var currentPaymentScheduleRequest = CurrentPaymentScheduleHelperTest.GetCurrentPaymentSchedule_JustLastFeePending_Request();
            var amortizationScheduleResponse = AmortizationScheduleHelperTest.GetAmortizationScheduleResponse();
            DateTime calculationDate = new DateTime(2021, month: 03, day: 03);

            _creditUseCaseMock.Setup(creditUseCase => creditUseCase.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(),
                It.IsAny<int>())).Returns(amortizationScheduleResponse);

            var result = CreditPaymentUsesCase.GetCurrentPaymentSchedule(currentPaymentScheduleRequest, calculationDate, parameters);

            Assert.All(result.PaymentCreditScheduleFees.Where(fee => fee.FeeStatus == FeeStatuses.PAID),
                paidFee => Assert.Equal(0, paidFee.Balance));
            Assert.Equal(1, result.PaymentCreditScheduleFees.Count(fee => fee.FeeStatus == FeeStatuses.CURRENT));
            Assert.Equal(result.PaymentCreditScheduleFees.Count - 1, result.PaymentCreditScheduleFees.Count(fee => fee.FeeStatus == FeeStatuses.PAID));
            Assert.Equal(FeeStatuses.CURRENT, result.PaymentCreditScheduleFees[result.PaymentCreditScheduleFees.Count - 1].FeeStatus);
        }

        [Fact]
        public void GetCurrentPaymentSchedule_JustFirstFee_IsPay()
        {
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            var currentPaymentScheduleRequest = CurrentPaymentScheduleHelperTest.GetCurrentPaymentSchedule_FirstFeePay_Request();
            var amortizationScheduleResponse = AmortizationScheduleHelperTest.GetAmortizationScheduleResponse();
            DateTime calculationDate = new DateTime(2020, month: 10, day: 13);

            _creditUseCaseMock.Setup(creditUseCase => creditUseCase.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(),
                It.IsAny<int>())).Returns(amortizationScheduleResponse);

            var result = CreditPaymentUsesCase.GetCurrentPaymentSchedule(currentPaymentScheduleRequest, calculationDate, parameters);

            Assert.Equal(11, result.PaymentCreditScheduleFees.Count);
            Assert.Equal(11, result.PendingFees);
            Assert.All(result.PaymentCreditScheduleFees,
                Fee => Assert.Equal(FeeStatuses.CURRENT, Fee.FeeStatus));
        }

        [Fact]
        public void GetCurrentPaymentSchedule_PendingFees_Quantity()
        {
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            var currentPaymentScheduleRequest = CurrentPaymentScheduleHelperTest.GetCurrentPaymentSchedule_PendingFeesQuantity();
            var amortizationScheduleResponse = AmortizationScheduleHelperTest.GetAmortizationScheduleResponse();
            DateTime calculationDate = new DateTime(2021, month: 01, day: 18);

            _creditUseCaseMock.Setup(creditUseCase => creditUseCase.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(),
                It.IsAny<int>())).Returns(amortizationScheduleResponse);

            var result = CreditPaymentUsesCase.GetCurrentPaymentSchedule(currentPaymentScheduleRequest, calculationDate, parameters);

            Assert.Equal(5, result.PaymentCreditScheduleFees.Count);
            Assert.Equal(5, result.PendingFees);
            Assert.Equal(5, result.PaymentAssuranceScheduleFees.Count);
            Assert.All(result.PaymentCreditScheduleFees,
                Fee => Assert.Equal(FeeStatuses.CURRENT, Fee.FeeStatus));
        }

        [Fact]
        public void GetCurrentPaymentSchedule_calculationDate_After_LastFeeDate()
        {
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            var currentPaymentScheduleRequest = CurrentPaymentScheduleHelperTest.GetCurrentPaymentSchedule_calculationDate_After_lastFeeDate();
            var amortizationScheduleResponse = AmortizationScheduleHelperTest.GetAmortizationScheduleResponse();
            DateTime calculationDate = new DateTime(2021, month: 04, day: 13);

            _creditUseCaseMock.Setup(creditUseCase => creditUseCase.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(),
                It.IsAny<int>())).Returns(amortizationScheduleResponse);

            var result = CreditPaymentUsesCase.GetCurrentPaymentSchedule(currentPaymentScheduleRequest, calculationDate, parameters);

            Assert.Equal(5, result.PaymentCreditScheduleFees.Count);
            Assert.Equal(5, result.PendingFees);
            Assert.All(result.PaymentCreditScheduleFees,
                Fee => Assert.Equal(FeeStatuses.LATE, Fee.FeeStatus));
            Assert.Equal(27, result.PaymentCreditScheduleFees.LastOrDefault().ArrearDays);
            Assert.Equal(6554.56M, result.PaymentCreditScheduleFees.LastOrDefault().ArrearPaymentValue);
            Assert.Equal(1887786.13M, result.TotalPayment);
            Assert.Equal(1887786.13M, result.PaymentFees.LastOrDefault().Payment);
            Assert.Equal(1887786.13M, result.MinimumPayment);
        }

        [Fact]
        public void GetCurrentPaymentSchedule_calculationDate_Before_LastPayment()
        {
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            var currentPaymentScheduleRequest = CurrentPaymentScheduleHelperTest.GetCurrentPaymentSchedule_calculationDate_Before_LastPayment();
            var amortizationScheduleResponse = AmortizationScheduleHelperTest.GetAmortizationScheduleResponse();
            DateTime calculationDate = new DateTime(2021, month: 01, day: 18);

            _creditUseCaseMock.Setup(creditUseCase => creditUseCase.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(),
                It.IsAny<int>())).Returns(amortizationScheduleResponse);

            var result = CreditPaymentUsesCase.GetCurrentPaymentSchedule(currentPaymentScheduleRequest, calculationDate, parameters);

            Assert.Equal(3, result.PaymentCreditScheduleFees.Count);
            Assert.Equal(3, result.PendingFees);
            Assert.All(result.PaymentCreditScheduleFees,
                Fee => Assert.Equal(FeeStatuses.CURRENT, Fee.FeeStatus));
            Assert.All(result.PaymentCreditScheduleFees, Fee => Assert.Equal(0, Fee.InterestDays));
            Assert.All(result.PaymentCreditScheduleFees, Fee => Assert.Equal(0, Fee.InterestValue));
        }

        [Fact]
        public void GetCurrentPaymentSchedule_Payments_ShouldBe_FirstAndLastFee()
        {
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            var currentPaymentScheduleRequest = CurrentPaymentScheduleHelperTest.GetCurrentPaymentSchedule_Payments_ShouldBe_FirstAndLastFee();
            var amortizationScheduleResponse = AmortizationScheduleHelperTest.GetAmortizationScheduleResponse();
            DateTime calculationDate = new DateTime(2021, month: 02, day: 18);

            _creditUseCaseMock.Setup(creditUseCase => creditUseCase.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(),
                It.IsAny<int>())).Returns(amortizationScheduleResponse);

            var result = CreditPaymentUsesCase.GetCurrentPaymentSchedule(currentPaymentScheduleRequest, calculationDate, parameters);

            Assert.Equal(339195.85M, result.MinimumPayment);
            Assert.Equal(339195.85M, result.PaymentFees.FirstOrDefault().Payment);
            Assert.Equal(988239.85M, result.TotalPayment);
            Assert.Equal(988239.85M, result.PaymentFees.LastOrDefault().Payment);
        }

        [Fact]
        public void GetCurrentPaymentSchedule_Minimun_Payment_With_Arrear_ShouldBe_FirstFee()
        {
            AppParameters parameters = ParameterHelperTest.GetAppParameters();
            var currentPaymentScheduleRequest = CurrentPaymentScheduleHelperTest.GetCurrentPaymentSchedule_Minimun_Payment_With_Arrear_ShouldBe_FirstFee();
            var amortizationScheduleResponse = AmortizationScheduleHelperTest.GetAmortizationScheduleResponse();
            DateTime calculationDate = new DateTime(2020, month: 11, day: 18);

            _creditUseCaseMock.Setup(creditUseCase => creditUseCase.GetOriginalAmortizationSchedule(It.IsAny<AmortizationScheduleRequest>(),
                It.IsAny<int>())).Returns(amortizationScheduleResponse);

            var result = CreditPaymentUsesCase.GetCurrentPaymentSchedule(currentPaymentScheduleRequest, calculationDate, parameters);

            Assert.Equal(417323.18M, result.MinimumPayment);
            Assert.Equal(417323.18M, result.PaymentFees.FirstOrDefault().Payment);
        }
    }
}