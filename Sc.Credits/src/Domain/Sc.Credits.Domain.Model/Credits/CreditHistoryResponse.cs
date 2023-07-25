using Newtonsoft.Json;
using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The credit history response entity
    /// </summary>
    public class CreditHistoryResponse
    {
        /// <summary>
        /// Gets or sets the credit id
        /// </summary>
        public Guid CreditId { get; set; }

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
        /// Gets or sets the credit's date
        /// </summary>
        public DateTime CreditDate { get; set; }

        /// <summary>
        /// Gets or sets the status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the status' id
        /// </summary>
        [JsonIgnore]
        public int StatusId { get; set; }

        /// <summary>
        /// Gets or sets the credit's value
        /// </summary>
        public decimal CreditValue { get; set; }

        /// <summary>
        /// Gets or sets the arrears days
        /// </summary>
        public int ArrearsDays { get; set; }

        /// <summary>
        /// Gets or sets the cancel date
        /// </summary>
        public DateTime? CancelDate { get; set; }
    }
}