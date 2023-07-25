using Newtonsoft.Json;

namespace Sc.Credits.Domain.Model.Common
{
    /// <summary>
    /// The response autentic entity
    /// </summary>
    public class ResponseAutentic
    {
        /// <summary>
        /// Transaction id
        /// </summary>
        [JsonProperty(propertyName: "transactionId")]
        public string TransactionId { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        [JsonProperty(propertyName: "type")]
        public string Type { get; set; }

        /// <summary>
        /// Operation code
        /// </summary>
        [JsonProperty(propertyName: "operationCode")]
        public int OperationCode { get; set; }

        /// <summary>
        /// Operation msg
        /// </summary>
        [JsonProperty(propertyName: "operationMsg")]
        public string OperationMsg { get; set; }

        /// <summary>
        /// Process date
        /// </summary>
        [JsonProperty(propertyName: "processedDate")]
        public string ProcessedDate { get; set; }

        /// <summary>
        /// Url documents
        /// </summary>
        [JsonProperty(propertyName: "urlDocuments")]
        public string UrlDocuments { get; set; }

        /// <summary>
        /// Account
        /// </summary>
        [JsonProperty(propertyName: "account")]
        public string Account { get; set; }
    }
}