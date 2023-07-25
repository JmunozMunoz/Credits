using Newtonsoft.Json;
using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The payment history response entity
    /// </summary>
    public class PaymentHistoryResponse
    {
        /// <summary>
        /// Gets or sets the credit id
        /// </summary>
        public Guid CreditId { get; set; }

        /// <summary>
        /// Gets or sets the payment id
        /// </summary>
        public Guid PaymentId { get; set; }

        /// <summary>
        /// Gets or sets the payment number
        /// </summary>
        public long PaymentNumber { get; set; }

        /// <summary>
        /// Gets or sets the credit number
        /// </summary>
        public long CreditNumber { get; set; }

        /// <summary>
        /// Gets or sets the store's id
        /// </summary>
        public string StoreId { get; set; }

        /// <summary>
        /// Gets or sets the store's name
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// Gets or sets the creation date
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the status id
        /// </summary>
        [JsonIgnore]
        public int StatusId { get; set; }

        /// <summary>
        /// Gets or sets the value paid
        /// </summary>
        public decimal ValuePaid { get; set; }

        /// <summary>
        /// Gets or sets the cancel date
        /// </summary>
        public DateTime? CancelDate { get; set; }
    }
}