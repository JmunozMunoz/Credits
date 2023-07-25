using System.Collections.Generic;

namespace Sc.Credits.Domain.Model.Customers
{
    /// <summary>
    /// The customer risk level entity
    /// </summary>
    public class CustomerRiskLevel
    {
        /// <summary>
        /// Gets or sets the level
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the observations
        /// </summary>
        public List<string> Observations { get; set; }
    }
}