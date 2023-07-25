using System;
using System.Collections.Generic;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The cancel credit detail response entity
    /// </summary>
    public class CancelCreditDetailResponse : CancelCreditResponse
    {
        /// <summary>
        /// Gets or sets the store's name
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// Gets or sets the cancellation request date
        /// </summary>
        public DateTime CancellationRequestDate { get; set; }

        /// <summary>
        /// Gets or sets the create date
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the reason
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets the balance
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// Gets or sets the payments
        /// </summary>
        public List<CancelCreditPaymentResponse> Payments { get; set; }

    }

    /// <summary>
    /// The cancel credit payment response entity
    /// </summary>
    public class CancelCreditPaymentResponse
    {
        /// <summary>
        /// Gets or sets the payment's number
        /// </summary>
        public int PaymentNumber { get; set; }

        /// <summary>
        /// Gets or sets the payment's value
        /// </summary>
        public int PaymentValue { get; set; }

        /// <summary>
        /// Gets or sets the payment's date
        /// </summary>
        public DateTime PaymentDate { get; set; }
    }
}