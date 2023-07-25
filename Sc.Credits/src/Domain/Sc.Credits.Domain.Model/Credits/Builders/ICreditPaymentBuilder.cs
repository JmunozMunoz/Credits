using System;

namespace Sc.Credits.Domain.Model.Credits.Builders
{
    /// <summary>
    /// Credit payment builder contract, extends <see cref="IBuilder{CreditMaster}"/>
    /// </summary>
    public interface ICreditPaymentBuilder : IBuilder<CreditMaster>
    {
        /// <summary>
        /// Initial payment
        /// </summary>
        /// <param name="transactionType"></param>
        /// <param name="paymentType"></param>
        /// <param name="dueDate"></param>
        /// <param name="calculationDate"></param>
        /// <returns></returns>
        ICreditPaymentBuilder InitialPayment(TransactionType transactionType, PaymentType paymentType, DateTime dueDate, DateTime calculationDate);

        /// <summary>
        /// Set assurance values
        /// </summary>
        /// <param name="assurancePercentage"></param>
        /// <param name="assuranceValue"></param>
        /// <param name="assuranceFee"></param>
        /// <param name="assuranceTotalFeeValue"></param>
        /// <param name="assuranceTotalValue"></param>
        ICreditCompleteBuilder AssuranceValues(decimal assurancePercentage, decimal assuranceValue, decimal assuranceFee, decimal assuranceTotalFeeValue,
            decimal assuranceTotalValue);
    }
}