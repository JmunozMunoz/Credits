using Sc.Credits.Domain.Model.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.Domain.Managment.Tests.Entities.Common
{
    public class MailNotificationRequestBuilder
    {
        #region Properties
        /// <summary>
        /// Gets or sets the recipients.
        /// </summary>
        /// <value>
        /// The recipients.
        /// </value>
        public List<string> Recipients { get; set; }

        /// <summary>
        /// Gets or sets from.
        /// </summary>
        /// <value>
        /// From.
        /// </value>
        public string From { get; set; }

        /// <summary>
        /// Gets or sets from name.
        /// </summary>
        /// <value>
        /// From name.
        /// </value>
        public string FromName { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>
        /// The subject.
        /// </value>
        public string Subject { get; set; }
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public string Content { get; set; }
        /// <summary>
        /// Gets or sets the base64 attachments.
        /// </summary>
        /// <value>
        /// The base64 attachments.
        /// </value>
        public List<Base64Attachment> Base64Attachments { get; set; }
        /// <summary>
        /// Gets or sets the BLOB storage attachments.
        /// </summary>
        /// <value>
        /// The BLOB storage attachments.
        /// </value>
        public List<BlobStorageAttachment> BlobStorageAttachments { get; set; }
        /// <summary>
        /// Gets or sets the template information.
        /// </summary>
        /// <value>
        /// The template information.
        /// </value>
        public TemplateInfo TemplateInfo { get; set; }
        #endregion

        #region Inicialization
        private readonly List<string> _recipients = new List<string>() { };
        private const string _from = "noreply@sistecredito.co";
        private const string _fromName = "";
        private const string _subject = "Prueba";
        private const string _content = "Contenido de prueba";
        private readonly List<Base64Attachment> _base64Attachments = new Base64AttachmentBuilder().Build();
        private readonly List<BlobStorageAttachment> _blobStorageAttachments = new BlobStorageAttachmentBuilder().Build();
        private readonly TemplateInfo _templateInfo = new TemplateInfoBuilder().Build();
        #endregion

        public MailNotificationRequestBuilder()
        {
            Recipients = _recipients;
            From = _from;
            FromName = _fromName;
            Subject = _subject;
            Content = _content;
            Base64Attachments = _base64Attachments;
            BlobStorageAttachments = _blobStorageAttachments;
            TemplateInfo = _templateInfo;
        }

        public MailNotificationRequestBuilder WithRecipients(List<string> recipients)
        {
            this.Recipients = recipients;
            return this;
        }

        public MailNotificationRequestBuilder WithFrom(string from)
        {
            this.From = from;
            return this;
        }

        public MailNotificationRequestBuilder WithFromName(string fromName)
        {
            this.FromName = fromName;
            return this;
        }

        public MailNotificationRequestBuilder WithSubject(string subject)
        {
            this.Subject = subject;
            return this;
        }

        public MailNotificationRequestBuilder WithContent(string content)
        {
            this.Content = content;
            return this;
        }

        public MailNotificationRequestBuilder WithBase64Attachments(List<Base64Attachment> base64Attachments)
        {
            this.Base64Attachments = base64Attachments;
            return this;
        }

        public MailNotificationRequestBuilder WithBlobStorageAttachments(List<BlobStorageAttachment> blobStorageAttachments)
        {
            this.BlobStorageAttachments = blobStorageAttachments;
            return this;
        }

        public MailNotificationRequestBuilder WithTemplateInfo(TemplateInfo templateInfo)
        {
            this.TemplateInfo = templateInfo;
            return this;
        }

        public MailNotificationRequest Build()
        {
            return new MailNotificationRequest
            {
                Recipients = Recipients,
                From = From,
                FromName = FromName,
                Subject = Subject,
                Content = Content,
                Base64Attachments = Base64Attachments,
                BlobStorageAttachments = BlobStorageAttachments,
                TemplateInfo = TemplateInfo
            };
        }
    }
}
