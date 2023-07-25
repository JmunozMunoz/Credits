using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// Credit ScCode Request
    /// </summary>
    public class CreditScCodeRequest
    {
        /// <summary>
        /// Gets or sets the credit's code on legacy system
        /// </summary>
        public string ScCode { get; set; }

        /// <summary>
        /// Gets or sets the credit's id
        /// </summary>
        public Guid CreditId { get; set; }
    }
}