using Sc.Credits.Helpers.Commons.Domain;
using System;

namespace Sc.Credits.Helpers.Commons.Messaging
{
    /// <summary>
    /// Message log
    /// </summary>
    public class MessageLog
    {
        public Guid Id { get; protected set; }

        /// <summary>
        /// Date
        /// </summary>
        public DateTime Date { get; protected set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Log key
        /// </summary>
        public string Key { get; protected set; }

        /// <summary>
        /// Resource name
        /// </summary>
        public string ResourceName { get; protected set; }

        /// <summary>
        /// Resource type
        /// </summary>
        public ResourceTypes ResourceType { get; protected set; }

        /// <summary>
        /// Json
        /// </summary>
        public string Json { get; protected set; }

        /// <summary>
        /// New message log
        /// </summary>
        protected MessageLog()
        {
            //Need to reflection mapping
        }

        /// <summary>
        /// New message log
        /// </summary>
        /// <param name="name"></param>
        /// <param name="key"></param>
        /// <param name="resourceType"></param>
        /// <param name="resourceName"></param>
        /// <param name="json"></param>
        public MessageLog(string name, string key, ResourceTypes resourceType, string resourceName, string json)
        {
            Id = IdentityGenerator.NewSequentialGuid();
            Date = DateTime.Now;
            Name = name;
            Key = key;
            ResourceType = resourceType;
            ResourceName = resourceName;
            Json = json;
        }
    }
}