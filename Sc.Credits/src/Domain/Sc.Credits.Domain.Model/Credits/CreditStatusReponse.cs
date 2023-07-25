using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// Credit Status Response
    /// </summary>
    public class CreditStatusResponse
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
        /// Gets or sets the credit's id
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
        /// Gets or sets the create date
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the credit value
        /// </summary>
        public decimal CreditValue { get; set; }

        /// <summary>
        /// Gets or sets the arrears days
        /// </summary>
        public long ArrearsDays { get; set; }

        /// <summary>
        /// Gets or sets the minimum payment
        /// </summary>
        public decimal MinimumPayment { get; set; }

        /// <summary>
        /// Gets or sets the total payment
        /// </summary>
        public decimal TotalPayment { get; set; }

        /// <summary>
        /// Gets or sets the fee value
        /// </summary>
        public decimal FeeValue { get; set; }
    }
}