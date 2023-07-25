using System;

namespace Sc.Credits.Domain.Model.Credits.Builders
{
    /// <summary>
    /// Payment matrix credit parameters info contract, extends <see cref="IBuilder{PaymentMatrix}"/>
    /// </summary>
    public interface IPaymentMatrixCreditParametersInfo : IBuilder<PaymentMatrix>
    {
        /// <summary>
        /// Credit Parameters Info
        /// </summary>
        /// <param name="effectiveAnnualRate"></param>
        /// <param name="arrearsEffectiveAnnualRate"></param>
        /// <param name="arrearsGracePeriod"></param>
        /// <returns></returns>
        IPaymentMatrixCreditParametersInfo CreditParametersInfo(decimal effectiveAnnualRate, decimal arrearsEffectiveAnnualRate, int arrearsGracePeriod);

        /// <summary>
        /// Arrears Adjustment Date
        /// </summary>
        /// <param name="arrearsAdjustmentDate"></param>
        /// <returns></returns>
        IPaymentMatrixCreditParametersInfo ArrearsAdjustmentDate(DateTime arrearsAdjustmentDate);
    }
}