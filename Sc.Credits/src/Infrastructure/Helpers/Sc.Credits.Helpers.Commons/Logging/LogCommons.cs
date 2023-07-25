using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Reflection;

namespace Sc.Credits.Helpers.Commons.Logging
{
    /// <summary>
    /// The log commons functions.
    /// </summary>
    public static class LogCommons
    {
        /// <summary>
        /// The log message format.
        /// </summary>
        /// <returns></returns>
        public const string MessageTemplate = "{@data}";

        /// <summary>
        /// Formats the log event name.
        /// </summary>
        /// <param name="credinetAppSettings"></param>
        /// <param name="methodBase"></param>
        /// <param name="callerMemberName"></param>
        /// <returns></returns>
        public static string FormatEventName(CredinetAppSettings credinetAppSettings, MethodBase methodBase, string callerMemberName)
        {
            Type declaringType = methodBase.DeclaringType.DeclaringType ?? methodBase.DeclaringType;

            return $"{credinetAppSettings.DomainName}.{declaringType.Name}.{callerMemberName}";
        }

        /// <summary>
        /// Gets the formatted message.
        /// </summary>
        /// <param name="logTypeName"></param>
        /// <param name="id"></param>
        /// <param name="eventName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string GetFormattedMessage(string logTypeName, string id, string eventName, string message = null)
        {
            string basicMessage = $"[{logTypeName}]: [Id] : [{id}] - [Event] : [{eventName}]";

            return string.IsNullOrEmpty(message)
                ? basicMessage : $"{basicMessage} - [Detail] : {message}";
        }
    }
}