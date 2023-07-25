namespace Sc.Credits.Domain.Model.Common
{
    using System.Collections.Generic;

    /// <summary>
    /// Mail notification request
    /// </summary>
    public class MailNotificationRequest
    {
        /// <summary>
        /// Gets or sets the recipients
        /// </summary>
        public List<string> Recipients { get; set; }

        /// <summary>
        /// Gets or sets the sender's email
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Gets or sets the sender's name
        /// </summary>
        public string FromName { get; set; }

        /// <summary>
        /// Gets or sets the subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the content
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the base 64 attachments
        /// </summary>
        public List<Base64Attachment> Base64Attachments { get; set; }

        /// <summary>
        /// Gets or sets the blob storage attachments
        /// </summary>
        public List<BlobStorageAttachment> BlobStorageAttachments { get; set; }

        /// <summary>
        /// Gets or sets the template's info
        /// </summary>
        public TemplateInfo TemplateInfo { get; set; }
    }
}