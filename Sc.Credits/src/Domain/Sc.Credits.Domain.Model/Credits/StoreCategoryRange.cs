using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// Expected store category range
    /// </summary>
    public class StoreCategoryRange
    {
        /// <summary>
        /// Gets or sets the get minimum fee value.
        /// </summary>
        /// <value>
        /// The get minimum fee value.
        /// </value>
        public decimal GetMinimumFeeValue { get; set; }

        /// <summary>
        /// Gets or sets the get maximum credit value.
        /// </summary>
        /// <value>
        /// The get maximum credit value.
        /// </value>
        public decimal GetMaximumCreditValue { get; set; }
}
}
