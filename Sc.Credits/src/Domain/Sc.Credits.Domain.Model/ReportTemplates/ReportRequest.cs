using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.Domain.Model.ReportTemplates
{
    /// <summary>
    /// The report request type.
    /// </summary>
    public class ReportRequest
    {
        /// <summary>
        /// Gets or sets the consumer application name.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets the report name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the report file render name.
        /// </summary>
        public string RenderName { get; set; }

        /// <summary>
        /// Gets or sets the report request data.
        /// </summary>
        public IEnumerable<RequestData> Data { get; set; }
    }
}