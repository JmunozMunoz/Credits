using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The credit token request entity
    /// </summary>
    public class CreditTokenRequest
    {
        /// <summary>
        /// Gets or sets the token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the customer's id
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the document's id
        /// </summary>
        public string IdDocument { get; set; }

        /// <summary>
        /// Gets or sets the additional data
        /// </summary>
        public string AdditionalData { get; set; }
    }
}