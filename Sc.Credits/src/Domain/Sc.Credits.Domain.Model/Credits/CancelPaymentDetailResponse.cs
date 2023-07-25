using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The cancel payment detail response entity
    /// </summary>
    public class CancelPaymentDetailResponse
    {
        /// <summary>
        /// Gets or sets the document's type
        /// </summary>
        public string TypeDocument { get; set; }

        /// <summary>
        /// Gets or sets the document's id
        /// </summary>
        public string IdDocument { get; set; }

        /// <summary>
        /// Gets or sets the payment number
        /// </summary>
        public long PaymentNumber { get; set; }

        /// <summary>
        /// Gets or sets the credit number
        /// </summary>
        public long CreditNumber { get; set; }

        /// <summary>
        /// Gets or sets the credit id
        /// </summary>
        public Guid CreditId { get; set; }

        /// <summary>
        /// Gets or sets the payment id
        /// </summary>
        public Guid PaymentId { get; set; }

        /// <summary>
        /// Gets or sets the store's id
        /// </summary>
        public string StoreId { get; set; }

        /// <summary>
        /// Gets or sets the store name
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// Gets or sets the user's id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the username
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the cancellation request date
        /// </summary>
        public DateTime CancellationRequestDate { get; set; }

        /// <summary>
        /// Gets or sets the payment value
        /// </summary>
        public decimal PaymentValue { get; set; }

        /// <summary>
        /// Gets or sets the payment date
        /// </summary>
        public DateTime PaymentDate { get; set; }

        /// <summary>
        /// Gets or sets the full name
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the reason
        /// </summary>
        public string Reason { get; set; }
    }
}