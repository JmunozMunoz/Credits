using Sc.Credits.Domain.Model.Common;

namespace Sc.Credits.DrivenAdapters.ServiceBus.Model
{
    /// <summary>
    /// Token generate response
    /// </summary>
    public class TokenGenerateResponse
    {
        /// <summary>
        /// Gets or sets the Data
        /// </summary>
        public TokenResponse Data { get; set; }
    }
}