using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// Request of send notification to create credit by identifier
    /// </summary>
    public class ResendNotificationRequest
    {
        /// <summary>
        /// Unique Identifier of credit master
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// User name who sent the request
        /// </summary>
        public string UserName { get; set; }
    }
}
