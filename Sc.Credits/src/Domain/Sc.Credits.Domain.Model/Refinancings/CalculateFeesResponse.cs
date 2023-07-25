using System;
using System.Collections.Generic;

namespace Sc.Credits.Domain.Model.Refinancings
{
    /// <summary>
    /// Calculate fees response
    /// </summary>
    public class CalculateFeesResponse
    {
        /// <summary>
        /// Credit value
        /// </summary>
        public decimal CreditValue { get; set; }

        /// <summary>
        /// Fees
        /// </summary>
        public List<FeeResponse> Fees { get; set; }

        /// <summary>
        /// Next payment date
        /// </summary>
        public DateTime NextPaymentDate { get; set; }
    }
}