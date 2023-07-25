using Sc.Credits.Domain.Model.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.Domain.Managment.Tests.Entities.Common
{
    public class Base64AttachmentBuilder
    {
        #region Properties
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName { get; set; }
        /// <summary>
        /// Gets or sets the content of the base64 file.
        /// </summary>
        /// <value>
        /// The content of the base64 file.
        /// </value>
        public string Base64FileContent { get; set; }
        #endregion

        #region Inicialization
        private const string _fileName = "Content Test";
        private const string _base64FileContent = "Prueba content";
        #endregion

        public Base64AttachmentBuilder()
        {
            FileName = _fileName;
            Base64FileContent = _base64FileContent;
        }
        /// <summary>
        /// Withes the name of the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public Base64AttachmentBuilder WithFileName(string fileName)
        {
            this.FileName = fileName;
            return this;
        }
        /// <summary>
        /// Withes the content of the base64 file.
        /// </summary>
        /// <param name="base64FileContent">Content of the base64 file.</param>
        /// <returns></returns>
        public Base64AttachmentBuilder WithBase64FileContent(string base64FileContent)
        {
            this.Base64FileContent = base64FileContent;
            return this;
        }
        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns></returns>
        public List<Base64Attachment> Build()
        {
            return new List<Base64Attachment>
            {
                new Base64Attachment()
                {
                    FileName = FileName,
                    Base64FileContent = Base64FileContent
                }
            };
        }
    }
}
