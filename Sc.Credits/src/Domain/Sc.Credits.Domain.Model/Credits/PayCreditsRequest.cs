using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The pay credits request type.
    /// </summary>
    public class PayCreditsRequest : PaymentCreditMultipleRequest
    {
        /// <summary>
        /// Gets or sets the correlated transaction id.
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// Gets or sets the payment calculation date.
        /// </summary>
        public DateTime? CalculationDate { get; set; }

        /// <summary>
        /// Creates a <see cref="PayCreditsRequest"/> from multiple request.
        /// </summary>
        /// <param name="paymentCreditMultipleRequest"></param>
        /// <returns></returns>
        public static PayCreditsRequest FromMultiple(PaymentCreditMultipleRequest paymentCreditMultipleRequest) =>
            new PayCreditsRequest
            {
                CalculationDate = DateTime.Now,
                BankAccount = paymentCreditMultipleRequest.BankAccount,
                CreditPaymentDetails = paymentCreditMultipleRequest.CreditPaymentDetails,
                Location = paymentCreditMultipleRequest.Location,
                StoreId = paymentCreditMultipleRequest.StoreId,
                UserId = paymentCreditMultipleRequest.UserId,
                UserName = paymentCreditMultipleRequest.UserName
            };
    }
}