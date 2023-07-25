using System;

namespace Sc.Credits.Domain.Model.Refinancings
{
    /// <summary>
    /// Calculate fees request
    /// </summary>
    public class CalculateFeesRequest : CustomerRequestBase
    {
        /// <summary>
        /// Credit ids
        /// </summary>
        public Guid[] CreditIds { get; set; }
    }
}