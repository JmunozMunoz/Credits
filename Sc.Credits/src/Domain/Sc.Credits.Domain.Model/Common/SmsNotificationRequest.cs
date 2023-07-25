using System.Collections.Generic;

namespace Sc.Credits.Domain.Model.Common
{
    /// <summary>
    /// The sms notification request entity
    /// </summary>
    public class SmsNotificationRequest
    {
        /// <summary>
        /// Gets or sets the mobile's number
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// Gets or sets the message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the url template's values
        /// </summary>
        public List<UrlTemplateValue> UrlTemplateValues { get; set; }
    }
}