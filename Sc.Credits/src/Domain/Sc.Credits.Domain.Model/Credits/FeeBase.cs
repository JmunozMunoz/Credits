using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The fee base entity
    /// </summary>
    public class FeeBase
    {
        /// <summary>
        /// Gets or sets the fee number
        /// </summary>
        public int FeeNumber { get; set; }

        /// <summary>
        /// Gets or sets the fee date
        /// </summary>
        public DateTime FeeDate { get; set; }
    }
}