using System;

namespace Sc.Credits.Helpers.Commons.Messaging
{
    /// <summary>
    /// Message error log
    /// </summary>
    public class MessageErrorLog : MessageLog
    {
        /// <summary>
        /// Maximum retries
        /// </summary>
        public const int MaximumRetries = 10;

        /// <summary>
        /// Processed
        /// </summary>
        public bool Processed { get; private set; }

        /// <summary>
        /// Attempts
        /// </summary>
        public int Attempts { get; private set; }

        /// <summary>
        /// Last attempt date
        /// </summary>
        public DateTime LastAttemptDate { get; private set; }

        /// <summary>
        /// New message error log
        /// </summary>
        protected MessageErrorLog()
        {
            //Need to reflection mapping
        }

        /// <summary>
        /// New message error log
        /// </summary>
        /// <param name="name"></param>
        /// <param name="key"></param>
        /// <param name="resourceType"></param>
        /// <param name="resourceName"></param>
        /// <param name="json"></param>
        public MessageErrorLog(string name, string key, ResourceTypes resourceType, string resourceName, string json)
            : base(name, key, resourceType, resourceName, json)
        {
            Processed = false;
            Attempts = 0;
            LastAttemptDate = DateTime.Now;
        }
    }
}