namespace Sc.Credits.Helper.Test.Model
{
    using Domain.Model.Parameters;
    using System.Collections.Generic;

    /// <summary>
    /// Parameter Helper Test
    /// </summary>
    public static class ParameterHelperTest
    {
        /// <summary>
        /// Get app parameters
        /// </summary>
        /// <returns></returns>
        public static AppParameters GetAppParameters() =>
            new AppParameters(GetParameters());

        /// <summary>
        /// Get Parameters
        /// </summary>
        /// <returns></returns>
        public static List<Parameter> GetParameters()
        {
            return new List<Parameter>
            {
                new Parameter("AssuranceTax", "0.19").SetId(1),
                new Parameter("DecimalNumbersRound", "2").SetId(2),
                new Parameter("PartialCreditLimit", "300000").SetId(3),
                new Parameter("MinimumCreditValue", "50000").SetId(4),
                new Parameter("ArrearsGracePeriod", "3").SetId(5),
                new Parameter("ArrearsEffectiveAnnualRate", "0.2832").SetId(6),
                new Parameter("MaximumResidueValue", "2000").SetId(7),
                new Parameter("MaximumDaysRequestCancellationPayments", "30").SetId(8),
                new Parameter("MaximumPaymentAdjustmentResidue", "100").SetId(9),
                new Parameter("CellPhone", "33333333333").SetId(10),
                new Parameter("Nit", "811007832").SetId(11),
                new Parameter("HaveAssurance", "true").SetId(12),
                new Parameter("HaveAssuranceTax", "true").SetId(13),
                new Parameter("MaximumMonthsCreditHistory", "6").SetId(15),
                new Parameter("MaximumMonthsPaymentHistory", "6").SetId(16),
                new Parameter("PhothoSignaturePaidCreditDays", "60").SetId(17),
                new Parameter("InterestRateDecimalNumbers", "6").SetId(18),
                new Parameter("ArrearsAdjustmentDate", "2010-08-31").SetId(19),
                new Parameter("DaysElapsedToRejectARequestCancelCredit", "3").SetId(20),
                new Parameter("DaysElapsedToRejectARequestCancelPayment", "3").SetId(21),
                new Parameter("MaximumCreditValueAccordingToStoreProfile", "200000").SetId(22),
                new Parameter("StoreProfiles", "8,9").SetId(23),
                new Parameter("CreditDaysPaidAccordingToStoreProfile", "30").SetId(24),
                new Parameter("EffectiveAnnualRate", "0.2832").SetId(25),
                new Parameter("Default", "Default").SetId(default),
                new Parameter("VirtualSalesTokenSources", "3,4,6").SetId(31),
        };
        }

        /// <summary>
        /// Get app parameters custom
        /// </summary>
        /// <param name="assuranceTax"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <param name="partialCreditLimit"></param>
        /// <param name="minimumCreditValue"></param>
        /// <param name="arrearsGracePeriod"></param>
        /// <param name="arrearsEffectiveAnnualRate"></param>
        /// <returns></returns>
        public static AppParameters GetAppParametersCustom(decimal assuranceTax, int decimalNumbersRound, decimal partialCreditLimit,
            decimal minimumCreditValue, int arrearsGracePeriod, decimal arrearsEffectiveAnnualRate)
            => new AppParameters(GetParametersCustom(assuranceTax, decimalNumbersRound, partialCreditLimit,
                minimumCreditValue, arrearsGracePeriod, arrearsEffectiveAnnualRate));

        /// <summary>
        /// Get Parameters Custom
        /// </summary>
        /// <param name="assuranceTax"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <param name="partialCreditLimit"></param>
        /// <param name="minimumCreditValue"></param>
        /// <param name="arrearsGracePeriod"></param>
        /// <param name="arrearsEffectiveAnnualRate"></param>
        /// <returns></returns>
        public static List<Parameter> GetParametersCustom(decimal assuranceTax, int decimalNumbersRound, decimal partialCreditLimit,
            decimal minimumCreditValue, int arrearsGracePeriod, decimal arrearsEffectiveAnnualRate)
        {
            return new List<Parameter>
            {
                new Parameter("AssuranceTax", assuranceTax.ToString()),
                new Parameter("DecimalNumbersRound", decimalNumbersRound.ToString()),
                new Parameter("PartialCreditLimit", partialCreditLimit.ToString()),
                new Parameter("MinimumCreditValue", minimumCreditValue.ToString()),
                new Parameter("ArrearsGracePeriod", arrearsGracePeriod.ToString()),
                new Parameter("ArrearsEffectiveAnnualRate", arrearsEffectiveAnnualRate.ToString()),
                new Parameter("MaximumResidueValue", "2000"),
                new Parameter("InterestRateDecimalNumbers", "6")
            };
        }
    }
}