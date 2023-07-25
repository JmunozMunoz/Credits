using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Builders;
using Sc.Credits.Helper.Test.Model;
using System;
using System.Collections.Generic;
using Xunit;

namespace Sc.Credits.Domain.UseCase.Tests.Credits
{
    public class PaymentMatrixTest
    {
        public static IEnumerable<object[]> DataForPayableValues =>
        new List<object[]>
        {
            new object[] { new DateTime(2020, 07, 01), new DateTime(2020, 02, 07), 609243M, 65000M, 55334.10M, 5706.40M, 34535.0M},
            new object[] { new DateTime(2020, 05, 30), new DateTime(2020, 02, 07), 609243M, 60927M, 45991.89M, 1609.51M, 10154.0M},
            new object[] { new DateTime(2020, 11, 01), new DateTime(2020, 07, 05), 520000M, 60927M, 17332.42M, 28075.04M, 60927.0M},
            new object[] { new DateTime(2020, 03, 07), new DateTime(2020, 02, 07), 708000M, 85000M, 14354.59M, 0M, 13919.0M},
        };

        [Theory]
        [MemberData(nameof(DataForPayableValues))]
        public void ShouldGetPayableValues(DateTime calculationDate, DateTime lastPaymentDate, decimal balance, decimal assuranceBalance, decimal PayableInterest, decimal PayableArrears, decimal PayableAssurance)
        {
            CreditsUsesCaseTest creditsUsesCaseTest = new CreditsUsesCaseTest();

            decimal creditValue = 768000;
            DateTime initialDate = new DateTime(2020, 1, 31);
            decimal feeValue = 94316.0M;
            decimal interestRate = 0.0204985209860811M;
            decimal assuranceValue = 76800;
            decimal assuranceFeeValue = 8533.0M;
            decimal assuranceTotalFeeValue = 10154.0M;

            AmortizationScheduleResponse amortizationSchedule = creditsUsesCaseTest.CreditUsesCase.GetOriginalAmortizationSchedule(
                AmortizationScheduleHelperTest.GetAmortizationScheduleRequest(creditValue, initialDate, feeValue, interestRate,
                    frequency: 30, fees: 9, downPayment: 0, assuranceValue, assuranceFeeValue, assuranceTotalFeeValue),
                    decimalNumbersRound: 0);

            PaymentMatrix paymentMatrix = PaymentMatrixBuilder.CreateBuilder()
                   .BasicInfo(amortizationSchedule, balance, assuranceBalance)
                   .DateInfo(calculationDate, lastPaymentDate)
                   .CreditParametersInfo(0.2757M, 0.283243M, 3)
                   .ArrearsAdjustmentDate(new DateTime(2020, 05, 05))
                   .Build();

            // Assert
            Assert.Equal(PayableInterest, Math.Round(paymentMatrix.PayableInterest, 2));
            Assert.Equal(PayableArrears, Math.Round(paymentMatrix.PayableArrears, 2));
            Assert.Equal(PayableAssurance, Math.Round(paymentMatrix.PayableAssurance, 2));
        }
    }
}