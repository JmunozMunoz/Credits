using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The response paidcredit certification
    /// </summary>
    public class ResponsePaidCreditCertification
    {
        /// <summary>
        /// Gets or sets the format name.
        /// </summary>
        public string FormatName { get; set; }

        /// <summary>
        /// Gets or sets the lender city.
        /// </summary>
        public string LenderCity { get; set; }

        /// <summary>
        /// Gets or sets the generation date.
        /// </summary>
        public string GenerationDate { get; set; }

        /// <summary>
        /// Gets or sets the customer full name.
        /// </summary>
        public string CustomerFullName { get; set; }

        /// <summary>
        /// Gets or sets the customer document type.
        /// </summary>
        public string CustomerDocumentType { get; set; }

        /// <summary>
        /// Gets or sets the customer document identification.
        /// </summary>
        public string CustomerDocumentId { get; set; }

        /// <summary>
        /// Gets or sets the store name.
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// Gets or sets the credit date.
        /// </summary>
        public string CreditDate { get; set; }

        /// <summary>
        /// Gets or sets the credit number.
        /// </summary>
        public string CreditNumber { get; set; }

        /// <summary>
        /// Gets or sets the credit payment date.
        /// </summary>
        public string CreditPaymentDate { get; set; }

        /// <summary>
        /// Gets or sets the lender phone number.
        /// </summary>
        public string LenderPhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the lender name.
        /// </summary>
        public string LenderName { get; set; }

    }
}
