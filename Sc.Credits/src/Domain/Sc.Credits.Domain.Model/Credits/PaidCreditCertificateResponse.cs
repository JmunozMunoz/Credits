using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The paid credit certificate response
    /// </summary>
    public class PaidCreditCertificateResponse
    {
        /// <summary>
        /// Gets or sets the current's date
        /// </summary>
        public DateTime CurrentDate { get; set; }

        /// <summary>
        /// Gets or sets the full name
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the document's type
        /// </summary>
        public string TypeDocument { get; set; }

        /// <summary>
        /// Gets or sets the document's id
        /// </summary>
        public string IdDocument { get; set; }

        /// <summary>
        /// Gets or sets the store's name
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// Gets or sets the credit date
        /// </summary>
        public DateTime CreditDate { get; set; }

        /// <summary>
        /// Gets or sets the credit number
        /// </summary>
        public long CreditNumber { get; set; }

        /// <summary>
        /// Gets or sets the transaction date
        /// </summary>
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// Gets or sets the cell phone number
        /// </summary>
        public string CellPhone { get; set; }

        /// <summary>
        /// Gets or sets the template
        /// </summary>
        public string Template { get; set; }
    }
}