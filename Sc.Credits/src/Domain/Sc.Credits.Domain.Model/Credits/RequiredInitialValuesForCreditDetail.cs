using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.Domain.Model.Credits
{
    public class RequiredInitialValuesForCreditDetail : RequiredInitialValues
    {
        /// <summary>
        /// Gets or sets the identifier document.
        /// </summary>
        /// <value>
        /// The identifier document.
        /// </value>
        public string idDocument { get; set; }

        /// <summary>
        /// Gets or sets the type document.
        /// </summary>
        /// <value>
        /// The type document.
        /// </value>
        public string typeDocument { get; set; }
    }
}
