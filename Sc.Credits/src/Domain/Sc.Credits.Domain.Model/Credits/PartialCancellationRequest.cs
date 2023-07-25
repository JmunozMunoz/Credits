using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The partial cancellation request
    /// </summary>
    public class PartialCancellationRequest
    {
        /// <summary>
        /// Gets or sets the credit id
        /// </summary>
        public Guid CreditId { get; set; }

        /// <summary>
        /// Gets or sets the value cancel
        /// </summary>
        public decimal ValueCancel { get; set; }
    }
}