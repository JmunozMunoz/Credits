using System;

namespace Sc.Credits.Domain.Model.Refinancings
{
    /// <summary>
    /// Customer request base
    /// </summary>
    public class CustomerRequestBase
    {
        /// <summary>
        /// Application id
        /// </summary>
        public Guid ApplicationId { get; set; }

        /// <summary>
        /// Document type
        /// </summary>
        public string DocumentType { get; set; }

        /// <summary>
        /// Id document
        /// </summary>
        public string IdDocument { get; set; }
    }
}