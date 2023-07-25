using Newtonsoft.Json;
using System.Collections.Generic;

namespace Sc.Credits.Domain.Model.Common
{
    /// <summary>
    /// The attachment pdf entity
    /// </summary>
    public class AttachmentPdf
    {
        /// <summary>
        /// Gets or sets the file's content
        /// </summary>
        [JsonProperty(propertyName: "fileContent")]
        public byte[] FileContent { get; set; }

        /// <summary>
        /// Gets or sets the file's name
        /// </summary>
        [JsonProperty(propertyName: "fileName")]
        public string FileName { get; set; }
    }

    /// <summary>
    /// The metadata entity
    /// </summary>
    public class Metadata
    {
        /// <summary>
        /// Gets or sets the names
        /// </summary>
        [JsonProperty(propertyName: "names")]
        public string Names { get; set; }

        /// <summary>
        /// Gets or sets the last names
        /// </summary>
        [JsonProperty(propertyName: "lastNames")]
        public string LastNames { get; set; }

        /// <summary>
        /// Gets or sets the doc's id
        /// </summary>
        [JsonProperty(propertyName: "docId")]
        public string DocId { get; set; }

        /// <summary>
        /// Gets or sets the secure key
        /// </summary>
        [JsonProperty(propertyName: "secureKey")]
        public string SecureKey { get; set; }
    }

    /// <summary>
    /// The request autentic entity
    /// </summary>
    public class RequestAutentic
    {
        /// <summary>
        /// Gets or sets the metadata
        /// </summary>
        [JsonProperty(propertyName: "metadata")]
        public Metadata Metadata { get; set; }

        /// <summary>
        /// Gets or sets the files
        /// </summary>
        [JsonProperty(propertyName: "files")]
        public IEnumerable<AttachmentPdf> Files { get; set; }
    }
}