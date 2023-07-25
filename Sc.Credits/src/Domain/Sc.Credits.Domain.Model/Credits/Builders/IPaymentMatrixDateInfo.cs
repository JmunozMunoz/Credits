using System;

namespace Sc.Credits.Domain.Model.Credits.Builders
{
    /// <summary>
    /// Payment matrix date info, extends <see cref="IBuilder{PaymentMatrix}"/>
    /// </summary>
    public interface IPaymentMatrixDateInfo : IBuilder<PaymentMatrix>
    {
        /// <summary>
        /// Date Info
        /// </summary>
        /// <param name="calculationDate"></param>
        /// <param name="lastPaymentDate"></param>
        /// <returns></returns>
        IPaymentMatrixCreditParametersInfo DateInfo(DateTime calculationDate, DateTime lastPaymentDate);
    }
}