using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Sc.Credits.Helpers.Commons.Logging
{
    /// <summary>
    /// The app logger contract.
    /// </summary>
    public interface IAppLogger
    {
        /// <summary>
        /// Writes a debug log.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="message"></param>
        /// <param name="methodBase"></param>
        /// <param name="callerMemberName"></param>
        /// <returns></returns>
        void Debug(string id, object data, string message, MethodBase methodBase, [CallerMemberName] string callerMemberName = null);

        /// <summary>
        /// Log information.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="methodBase"></param>
        /// <param name="callerMemberName"></param>
        /// <returns></returns>
        void LogInfo(string id, object data, MethodBase methodBase, [CallerMemberName] string callerMemberName = null);

        /// <summary>
        /// Log information.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="message"></param>
        /// <param name="methodBase"></param>
        /// <param name="callerMemberName"></param>
        /// <returns></returns>
        void LogInfo(string id, object data, string message, MethodBase methodBase, [CallerMemberName] string callerMemberName = null);

        /// <summary>
        /// Log exception as error.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="exception"></param>
        /// <param name="methodBase"></param>
        /// <param name="callerMemberName"></param>
        /// <returns></returns>
        void LogError(string id, object data, Exception exception, MethodBase methodBase, [CallerMemberName] string callerMemberName = null);

        /// <summary>
        /// Log exception as warning.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="exception"></param>
        /// <param name="methodBase"></param>
        /// <param name="callerMemberName"></param>
        /// <returns></returns>
        void LogWarning(string id, object data, Exception exception, MethodBase methodBase, [CallerMemberName] string callerMemberName = null);

        /// <summary>
        /// Log data as warning.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="methodBase"></param>
        /// <param name="callerMemberName"></param>
        /// <returns></returns>
        void LogWarning(string id, object data, MethodBase methodBase, [CallerMemberName] string callerMemberName = null);
    }
}