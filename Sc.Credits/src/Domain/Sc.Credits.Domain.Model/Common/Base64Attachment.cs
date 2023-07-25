namespace Sc.Credits.Domain.Model.Common
{
    /// <summary>
    /// Base 64 attachment entity
    /// </summary>
    public class Base64Attachment
    {
        /// <summary>
        /// Gets or sets the file's name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the file content's base64
        /// </summary>
        public string Base64FileContent { get; set; }
    }
}