using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.Domain.Model.Credits
{
    public class VerifyCreditCreationRequest
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
        /// Gets or sets the credit's value
        /// </summary>
        public decimal CreditValue { get; set; }

        /// <summary>
        /// Gets or sets the frequency
        /// </summary>
        public int Frequency { get; set; }

        /// <summary>
        /// Gets or sets the token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the store's id
        /// </summary>
        public string StoreId { get; set; }

        /// <summary>
        /// Gets or sets the months.
        /// </summary>
        /// <value>
        /// The months.
        /// </value>
        public int Months { get; set; }

        /// <summary>
        /// Gets or sets the invoice.
        /// </summary>
        public string Invoice { get; set; }
    }
}
