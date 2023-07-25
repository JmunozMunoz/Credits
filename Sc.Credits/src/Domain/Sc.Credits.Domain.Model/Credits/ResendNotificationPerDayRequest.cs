using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// Request of send notification to create credit per day specific
    /// </summary>
    public class ResendNotificationPerDayRequest
    {
        /// <summary>
        /// date on which all missing notifications will be executed
        /// </summary>
        public DateTime DateOperation { get; set; }

        /// <summary>
        /// limit of number of transactions that will be executed
        /// </summary>
        public int LimitTransactions { get; set; }

        /// <summary>
        /// User name who sent the request
        /// </summary>
        public string UserName { get; set; }
    }
}
