using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.Domain.Model.Credits
{
    public class RequiredInitialValues
    {
        /// <summary>
        /// Gets or sets the credit value.
        /// </summary>
        /// <value>
        /// The credit value.
        /// </value>
        public decimal creditValue { get; set; }

        /// <summary>
        /// Gets or sets the months.
        /// </summary>
        /// <value>
        /// The months.
        /// </value>
        public int months { get; set; }
        /// <summary>
        /// Gets or sets the store identifier.
        /// </summary>
        /// <value>
        /// The store identifier.
        /// </value>
        public string storeId { get; set; }

        /// <summary>
        /// Gets or sets the frequency.
        /// </summary>
        /// <value>
        /// The frequency.
        /// </value>
        public int frequency { get; set; }
    }
}
