using Sc.Credits.Domain.Model.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.Domain.Managment.Tests.Entities.Common
{
    public class BlobStorageAttachmentBuilder
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
        /// Gets or sets the path.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        public string Path { get; set; }
        /// <summary>
        /// Gets or sets the name of the BLOB container.
        /// </summary>
        /// <value>
        /// The name of the BLOB container.
        /// </value>
        public string BlobContainerName { get; set; }
        #endregion

        #region Inicialization
        private const string _fileName = "prueba content";
        private const string _path = "prueba content";
        private const string _blobContainerName = "prueba content";
        #endregion		
        /// <summary>
        /// Initializes a new instance of the <see cref="BlobStorageAttachmentBuilder"/> class.
        /// </summary>
        public BlobStorageAttachmentBuilder()
        {
            FileName = _fileName;
            Path = _path;
            BlobContainerName = _blobContainerName;
        }
        /// <summary>
        /// Withes the name of the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public BlobStorageAttachmentBuilder WithFileName(string fileName)
        {
            this.FileName = fileName;
            return this;
        }
        /// <summary>
        /// Withes the path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public BlobStorageAttachmentBuilder WithPath(string path)
        {
            this.Path = path;
            return this;
        }
        /// <summary>
        /// Withes the name of the BLOB container.
        /// </summary>
        /// <param name="blobContainerName">Name of the BLOB container.</param>
        /// <returns></returns>
        public BlobStorageAttachmentBuilder WithBlobContainerName(string blobContainerName)
        {
            this.BlobContainerName = blobContainerName;
            return this;
        }
        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns></returns>
        public List<BlobStorageAttachment> Build()
        {
            return new List<BlobStorageAttachment>
            {
                new BlobStorageAttachment()
                {
                    FileName = FileName,
                    Path = Path,
                    BlobContainerName = BlobContainerName
                }
            };
        }
    }
}
