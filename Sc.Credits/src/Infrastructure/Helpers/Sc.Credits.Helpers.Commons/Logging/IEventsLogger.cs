using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Sc.Credits.Helpers.Commons.Logging
{
    /// <summary>
    /// The events logger contract.
    /// </summary>
    public interface IEventsLogger
    {
        /// <summary>
        /// Log information.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="eventName"></param>
        /// <param name="data"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        void LogInfo(string id, string eventName, object data, string message = null);

        /// <summary>
        /// Log exception as error.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="eventName"></param>
        /// <param name="data"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        void LogError(string id, string eventName, object data, Exception exception);

        /// <summary>
        /// Log exception as warning.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="eventName"></param>
        /// <param name="data"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        void LogWarning(string id, string eventName, object data, Exception exception = null);

        /// <summary>
        /// Notify a log event.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="eventName"></param>
        /// <param name="data"></param>
        /// <param name="methodBase"></param>
        /// <param name="callerMemberName"></param>
        Task NotifyAsync(string id, string eventName, object data, MethodBase methodBase = null, [CallerMemberName] string callerMemberName = null);

        /// <summary>
        /// Notify a log event.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="methodBase"></param>
        /// <param name="callerMemberName"></param>
        Task NotifyAsync(string id, object data, MethodBase methodBase, [CallerMemberName] string callerMemberName = null);
    }
}