using Newtonsoft.Json;
using System;

namespace Sc.Credits.Domain.Model.Refinancings
{
    /// <summary>
    /// Refinancing credit request
    /// </summary>
    public class RefinancingCreditRequest : CustomerRequestBase
    {
        /// <summary>
        /// Reference text
        /// </summary>
        public string ReferenceText { get; set; }

        /// <summary>
        /// Reference code
        /// </summary>
        public string ReferenceCode { get; set; }

        /// <summary>
        /// Fees
        /// </summary>
        public int Fees { get; set; }

        /// <summary>
        /// Source
        /// </summary>
        public int Source { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Auth method
        /// </summary>
        public int AuthMethod { get; set; }

        /// <summary>
        /// User id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Credit ids
        /// </summary>
        public Guid[] CreditIds { get; set; }

        /// <summary>
        /// Location
        /// </summary>
        [JsonIgnore]
        public string Location { get; set; }
    }
}