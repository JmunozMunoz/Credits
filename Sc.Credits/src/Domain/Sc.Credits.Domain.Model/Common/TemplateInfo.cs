using System.Collections.Generic;

namespace Sc.Credits.Domain.Model.Common
{
    /// <summary>
    /// The template info entity
    /// </summary>
    public class TemplateInfo
    {
        /// <summary>
        /// Gets or sets the template's id
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// Gets or sets the template's values
        /// </summary>
        public List<TemplateValue> TemplateValues { get; set; }
    }
}