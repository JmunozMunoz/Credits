using Microsoft.Extensions.Logging;
using org.reactivecommons.api;
using Sc.Credits.Helpers.Commons.Extensions;
using Sc.Credits.Helpers.Commons.Messaging;
using Sc.Credits.Helpers.Commons.Settings;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Sc.Credits.Helpers.Commons.Logging
{
    /// <summary>
    /// The logger service is an implementation of <see cref="ILoggerService{T}"/>
    /// </summary>
    public class LoggerService<T>
        : ILoggerService<T>
    {
        private readonly IDirectAsyncGateway<dynamic> _eventsAsyncGateway;
        private readonly IMessagingLogger _messagingLogger;
        private readonly ILogger<T> _logger;
        private readonly CredinetAppSettings _credinetAppSettings;

        /// <summary>
        /// Creates new instance of <see cref="LoggerService{T}"/>
        /// </summary>
        /// <param name="eventsAsyncGateway"></param>
        /// <param name="logger"></param>
        /// <param name="messagingLogger"></param>
        /// <param name="appSettings"></param>
        public LoggerService(IDirectAsyncGateway<dynamic> eventsAsyncGateway,
            ILogger<T> logger,
            IMessagingLogger messagingLogger,
            ISettings<CredinetAppSettings> appSettings)
        {
            _eventsAsyncGateway = eventsAsyncGateway;
            _messagingLogger = messagingLogger;
            _logger = logger;
            _credinetAppSettings = appSettings.Get();
        }

        /// <summary>
        /// Gets the log event name.
        /// </summary>
        /// <param name="methodBase"></param>
        /// <param name="callerMemberName"></param>
        /// <returns></returns>
        private string GetLogEventName(MethodBase methodBase, string callerMemberName) =>
            LogCommons.FormatEventName(_credinetAppSettings, methodBase, callerMemberName);

        /// <summary>
        /// <see cref="IAppLogger.Debug(string, object, string, MethodBase, string)"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="message"></param>
        /// <param name="methodBase"></param>
        /// <param name="callerMemberName"></param>
        /// <returns></returns>
        public void Debug(string id, object data, string message, MethodBase methodBase, [CallerMemberName] string callerMemberName = null)
        {
            string eventName = GetLogEventName(methodBase, callerMemberName);
            message = LogCommons.GetFormattedMessage(nameof(Debug), id, eventName, message);

            data = new { id, eventName, message, data };

            _logger.LogDebug(message: LogCommons.MessageTemplate, data);
        }

        /// <summary>
        /// <see cref="IAppLogger.LogInfo(string, object, MethodBase, string)"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="methodBase"></param>
        /// <param name="callerMemberName"></param>
        public void LogInfo(string id, object data, MethodBase methodBase, [CallerMemberName] string callerMemberName = null) =>
            LogInfo(id, GetLogEventName(methodBase, callerMemberName), data);

        /// <summary>
        /// <see cref="IAppLogger.LogInfo(string, object, string, MethodBase, string)"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="message"></param>
        /// <param name="methodBase"></param>
        /// <param name="callerMemberName"></param>
        public void LogInfo(string id, object data, string message, MethodBase methodBase, [CallerMemberName] string callerMemberName = null) =>
            LogInfo(id, GetLogEventName(methodBase, callerMemberName), data, message);

        /// <summary>
        /// <see cref="IEventsLogger.LogInfo(string, string, object, string)"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="eventName"></param>
        /// <param name="data"></param>
        /// <param name="message"></param>
        public void LogInfo(string id, string eventName, object data, string message = null)
        {
            message = LogCommons.GetFormattedMessage(nameof(LogInfo), id, eventName, message);

            data = new { id, eventName, message, data };

            _logger.LogInformation(message: LogCommons.MessageTemplate, data);
        }

        /// <summary>
        /// <see cref="IAppLogger.LogError(string, object, Exception, MethodBase, string)"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="exception"></param>
        /// <param name="methodBase"></param>
        /// <param name="callerMemberName"></param>
        public void LogError(string id, object data, Exception exception, MethodBase methodBase,
            [CallerMemberName] string callerMemberName = null) =>
            LogError(id, GetLogEventName(methodBase, callerMemberName), data, exception);

        /// <summary>
        /// <see cref="IEventsLogger.LogError(string, string, object, Exception)"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="eventName"></param>
        /// <param name="data"></param>
        /// <param name="exception"></param>
        public void LogError(string id, string eventName, object data, Exception exception)
        {
            string message = LogCommons.GetFormattedMessage(nameof(LogError), id, eventName, exception.Message);
            string code = exception.GetType().GetProperty("code")?.GetValue(exception)?.ToString() ?? string.Empty;
            string type = exception.GetType().Name;

            data = new { id, eventName, message, code, exception, type, data };

            _logger.LogError(exception, LogCommons.MessageTemplate, data);
        }

        /// <summary>
        /// <see cref="IAppLogger.LogWarning(string, object, Exception, MethodBase, string)"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="exception"></param>
        /// <param name="methodBase"></param>
        /// <param name="callerMemberName"></param>
        public void LogWarning(string id, object data, Exception exception, MethodBase methodBase,
            [CallerMemberName] string callerMemberName = null) =>
            LogWarning(GetLogEventName(methodBase, callerMemberName), id, data, exception);

        /// <summary>
        /// <see cref="IAppLogger.LogWarning(string, object, Exception, MethodBase, string)"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="methodBase"></param>
        /// <param name="callerMemberName"></param>
        public void LogWarning(string id, object data, MethodBase methodBase,
            [CallerMemberName] string callerMemberName = null) =>
            LogWarning(GetLogEventName(methodBase, callerMemberName), id, data);

        /// <summary>
        /// <see cref="IEventsLogger.LogWarning(string, string, object, Exception)"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="eventName"></param>
        /// <param name="data"></param>
        /// <param name="exception"></param>
        public void LogWarning(string id, string eventName, object data, Exception exception = null)
        {
            string message = LogCommons.GetFormattedMessage(nameof(LogWarning), id, eventName, exception?.Message ?? "Warning");
            string code = exception?.GetType().GetProperty("id")?.GetValue(exception)?.ToString() ?? string.Empty;
            string type = exception?.GetType().Name;

            data = new { id, eventName, data, message, code, exception, type };

            _logger.LogWarning(exception, message: LogCommons.MessageTemplate, data);
        }

        /// <summary>
        /// <see cref="IEventsLogger.NotifyAsync(string, string, object, MethodBase, string)"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="eventName"></param>
        /// <param name="data"></param>
        /// <param name="methodBase"></param>
        /// <param name="callerMemberName"></param>
        public async Task NotifyAsync(string id, string eventName, object data, MethodBase methodBase = null, [CallerMemberName] string callerMemberName = null) =>
            await _eventsAsyncGateway.HandleSendEventAsync(this, _messagingLogger, id, data, _credinetAppSettings.EventsTopicName, eventName,
                methodBase ?? MethodBase.GetCurrentMethod(), callerMemberName);

        /// <summary>
        /// <see cref="IEventsLogger.NotifyAsync(string, object, MethodBase, string)"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="methodBase"></param>
        /// <param name="callerMemberName"></param>
        public async Task NotifyAsync(string id, object data, MethodBase methodBase, [CallerMemberName] string callerMemberName = null) =>
            await NotifyAsync(id, GetLogEventName(methodBase, callerMemberName), data, methodBase, callerMemberName);
    }
}