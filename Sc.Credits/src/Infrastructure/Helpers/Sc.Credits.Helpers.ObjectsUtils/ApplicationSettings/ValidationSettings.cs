using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.Helpers.ObjectsUtils.ApplicationSettings
{
    public class ValidationSettings
    {
        /// <summary>
        /// Gets or sets the minimum months.
        /// </summary>
        /// <value>
        /// The minimum months.
        /// </value>
        public int MinimumMonths { get; set; }

        /// <summary>
        /// Gets or sets the minimum frequency.
        /// </summary>
        /// <value>
        /// The minimum frequency.
        /// </value>
        public int MinimumFrequency { get; set; }

        /// <summary>
        /// Gets or sets the simulation minimum credit value.
        /// </summary>
        /// <value>
        /// The simulation minimum credit value.
        /// </value>
        public int SimulationMinimumCreditValue { get; set; }

        /// <summary>
        /// Gets or sets the minimum credit value.
        /// </summary>
        /// <value>
        /// The minimum credit value.
        /// </value>
        public int MinimumCreditValue { get; set; }

        /// <summary>
        /// Gets or sets the proper user name reg ex.
        /// </summary>
        /// <value>
        /// The proper user name reg ex.
        /// </value>
        public string ProperUserNameRegEx { get; set; }

        /// <summary>
        /// Gets or sets the proper email reg ex.
        /// </summary>
        /// <value>
        /// The proper email reg ex.
        /// </value>
        public string ProperEmailRegEx { get; set; }

        /// <summary>
        /// Gets or sets the maximum character length for identifier document.
        /// </summary>
        /// <value>
        /// The maximum character length for identifier document.
        /// </value>
        public int MaximumCharacterLengthForIdDocument { get; set; }

        /// <summary>
        /// Gets or sets the identifier document reg ex.
        /// </summary>
        /// <value>
        /// The identifier document reg ex.
        /// </value>
        public string IdDocumentRegEx { get; set; }

        /// <summary>
        /// Gets or sets the store identifier reg ex.
        /// </summary>
        /// <value>
        /// The store identifier reg ex.
        /// </value>
        public string StoreIdRegEx { get; set; }

        /// <summary>
        /// Maximum Length of credit Token
        /// </summary>
        public string MaximumTokenLength { get; set; }
    }
}
