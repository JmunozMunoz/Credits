using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// Initial values to calculate limit months
    /// </summary>
    /// <seealso cref="Sc.Credits.Domain.Model.Credits.RequiredInitialValues" />
    public class LimitMonhsInitialValuesRequest
    {
        /// <summary>
        /// Gets or sets the credit value.
        /// </summary>
        /// <value>
        /// The credit value.
        /// </value>
        public decimal creditValue { get; set; }

        /// <summary>
        /// Gets or sets the store identifier.
        /// </summary>
        /// <value>
        /// The store identifier.
        /// </value>
        public string storeId { get; set; }

    }
}
