namespace Sc.Credits.Domain.Model.Common
{
    /// <summary>
    /// Blob storage attachment
    /// </summary>
    public class BlobStorageAttachment
    {
        /// <summary>
        /// Gets or sets the file name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the path
        /// </summary>

        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the blob container name
        /// </summary>
        public string BlobContainerName { get; set; }
    }
}