using Newtonsoft.Json;

namespace Sc.Credits.Domain.Model.Common
{
    /// <summary>
    /// The oauth response autentic entity
    /// </summary>
    public class OAuthResponseAutentic
    {
        /// <summary>
        /// Gets or sets the access token
        /// </summary>
        [JsonProperty(propertyName: "access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the scope
        /// </summary>
        [JsonProperty(propertyName: "scope")]
        public string Scope { get; set; }

        /// <summary>
        /// Gets or sets the expires in
        /// </summary>
        [JsonProperty(propertyName: "expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Gets or sets the token's type
        /// </summary>
        [JsonProperty(propertyName: "token_type")]
        public string TokenType { get; set; }
    }
}