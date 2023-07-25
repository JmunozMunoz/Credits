using System.Collections.Generic;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// Payment credit request
    /// </summary>
    public class PaymentCreditMultipleRequest
    {
        /// <summary>
        /// Credit payment details
        /// </summary>
        public List<PaymentCreditMultipleDetail> CreditPaymentDetails { get; set; }

        /// <summary>
        /// Store Id
        /// </summary>
        public string StoreId { get; set; }

        /// <summary>
        /// User Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Bank Account
        /// </summary>
        public string BankAccount { get; set; }

        /// <summary>
        /// User Name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Location
        /// </summary>
        public string Location { get; set; }
    }
}