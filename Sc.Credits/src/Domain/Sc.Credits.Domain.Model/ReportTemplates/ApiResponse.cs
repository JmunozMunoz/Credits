using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.Domain.Model.ReportTemplates
{
    /// <summary>
    /// The api response type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Gets or sets the response data.
        /// </summary>
        public T Data { get; set; }
    }
}