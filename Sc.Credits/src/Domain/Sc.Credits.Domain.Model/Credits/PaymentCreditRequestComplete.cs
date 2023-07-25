using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The payment credit request complete entity
    /// </summary>
    public class PaymentCreditRequestComplete : PaymentCreditRequest
    {
        /// <summary>
        /// Creates a new instance of <see cref="PaymentCreditRequestComplete"/>
        /// </summary>
        public PaymentCreditRequestComplete()
        {
        }

        /// <summary>
        /// Creeates a new instance of <see cref="PaymentCreditRequestComplete"/>
        /// </summary>
        /// <param name="paymentCreditRequest"></param>
        public PaymentCreditRequestComplete(PaymentCreditRequest paymentCreditRequest)
        {
            CreditId = paymentCreditRequest.CreditId;
            TotalValuePaid = paymentCreditRequest.TotalValuePaid;
            StoreId = paymentCreditRequest.StoreId;
            UserId = paymentCreditRequest.UserId;
            BankAccount = paymentCreditRequest.BankAccount;
            UserName = paymentCreditRequest.UserName;
            Location = paymentCreditRequest.Location;
            CalculationDate = DateTime.Now;
        }

        /// <summary>
        /// Gets or sets the calculation date
        /// </summary>
        public DateTime CalculationDate { get; set; }

        /// <summary>
        /// Gets or sets the balance release for refinancing
        /// </summary>
        public decimal BalanceReleaseForRefinancing { get; set; }
    }
}