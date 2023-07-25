using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The active and pending cancellation payment response entity
    /// </summary>
    public class ActivePendingCancellationPaymentResponse
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
        /// Gets or sets the
        /// </summary>
        public Guid CreditId { get; set; }

        /// <summary>
        /// Gets or sets the credit number
        /// </summary>
        public long CreditNumber { get; set; }

        /// <summary>
        /// Gets or sets the payment's id
        /// </summary>
        public Guid PaymentId { get; set; }

        /// <summary>
        /// Gets or sets the payment number
        /// </summary>
        public long PaymentNumber { get; set; }

        /// <summary>
        /// Gets or sets the payment's date
        /// </summary>
        public DateTime PaymentDate { get; set; }

        /// <summary>
        /// Gets or sets the store's id
        /// </summary>
        public string StoreId { get; set; }

        /// <summary>
        /// Gets or sets the store's name
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// Gets or sets the value paid
        /// </summary>
        public decimal ValuePaid { get; set; }

        /// <summary>
        /// Gets or sets the has cancel request
        /// </summary>
        public bool HasCancelRequest { get; set; }

        /// <summary>
        /// Gets or sets the cancelable
        /// </summary>
        public bool Cancelable { get; set; }
    }
}