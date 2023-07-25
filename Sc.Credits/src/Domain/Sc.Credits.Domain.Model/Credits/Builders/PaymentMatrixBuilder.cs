using System;

namespace Sc.Credits.Domain.Model.Credits.Builders
{
    /// <summary>
    /// Payment matrix builder
    /// </summary>
    public class PaymentMatrixBuilder :
        IPaymentMatrixBasicInfo,
        IPaymentMatrixDateInfo,
        IPaymentMatrixCreditParametersInfo
    {
        private PaymentMatrix _paymentMatrix;

        /// <summary>
        /// Creates a new instance of <see cref="PaymentMatrixBuilder"/>
        /// </summary>
        private PaymentMatrixBuilder()
        {
        }

        /// <summary>
        /// Create payment matrix builder
        /// </summary>
        /// <returns></returns>
        public static IPaymentMatrixBasicInfo CreateBuilder()
        {
            return new PaymentMatrixBuilder();
        }

        /// <summary>
        /// <see cref="IPaymentMatrixBasicInfo.BasicInfo(AmortizationScheduleResponse, decimal, decimal)"/>
        /// </summary>
        /// <param name="amortizationSchedule"></param>
        /// <param name="balance"></param>
        /// <param name="assuranceBalance"></param>
        /// <returns></returns>
        public IPaymentMatrixDateInfo BasicInfo(AmortizationScheduleResponse amortizationSchedule,
            decimal balance, decimal assuranceBalance)
        {
            _paymentMatrix = new PaymentMatrix(amortizationSchedule, balance, assuranceBalance);
            return this;
        }

        /// <summary>
        /// <see cref="IPaymentMatrixDateInfo.DateInfo(DateTime, DateTime)"/>
        /// </summary>
        /// <param name="calculationDate"></param>
        /// <param name="lastPaymentDate"></param>
        /// <returns></returns>
        public IPaymentMatrixCreditParametersInfo DateInfo(DateTime calculationDate, DateTime lastPaymentDate)
        {
            _paymentMatrix.SetDateInfo(calculationDate, lastPaymentDate);
            return this;
        }

        /// <summary>
        /// <see cref="IPaymentMatrixCreditParametersInfo.CreditParametersInfo(decimal, decimal, int)"/>
        /// </summary>
        /// <param name="effectiveAnnualRate"></param>
        /// <param name="arrearsEffectiveAnnualRate"></param>
        /// <param name="arrearsGracePeriod"></param>
        /// <returns></returns>
        public IPaymentMatrixCreditParametersInfo CreditParametersInfo(decimal effectiveAnnualRate,
            decimal arrearsEffectiveAnnualRate, int arrearsGracePeriod)
        {
            _paymentMatrix.SetCreditParametersInfo(effectiveAnnualRate, arrearsEffectiveAnnualRate, arrearsGracePeriod);

            return this;
        }

        /// <summary>
        /// <see cref="IPaymentMatrixCreditParametersInfo.ArrearsAdjustmentDate(DateTime)"/>
        /// </summary>
        /// <param name="arrearsAdjustmentDate"></param>
        /// <returns></returns>
        public IPaymentMatrixCreditParametersInfo ArrearsAdjustmentDate(DateTime arrearsAdjustmentDate)
        {
            _paymentMatrix.SetArrearsAdjustmentDate(arrearsAdjustmentDate);

            return this;
        }

        /// <summary>
        /// Builds the payment matrix
        /// </summary>
        /// <returns></returns>
        public PaymentMatrix Build() => _paymentMatrix.CalculatePayableValues();
    }
}