using Newtonsoft.Json;

namespace Sc.Credits.Domain.Model.Common
{
    /// <summary>
    /// Oauth request autentic entity
    /// </summary>
    public class OAuthRequestAutentic
    {
        /// <summary>
        /// Gets or sets the audience
        /// </summary>
        [JsonProperty(propertyName: "audience")]
        public string Audience { get; set; }

        /// <summary>
        /// Gets or sets the grant type
        /// </summary>
        [JsonProperty(propertyName: "grant_type")]
        public string GrantType { get; set; }

        /// <summary>
        /// Gets or sets the client's id
        /// </summary>
        [JsonProperty(propertyName: "client_id")]
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client's secret
        /// </summary>
        [JsonProperty(propertyName: "client_secret")]
        public string ClientSecret { get; set; }
    }
}