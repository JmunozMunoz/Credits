using org.reactivecommons.api;
using Sc.Credits.Helpers.Commons.Extensions;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.Commons.Messaging;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Sc.Credits.DrivenAdapters.ServiceBus.Common
{
    /// <summary>
    /// The async gateway adapter base.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncGatewayAdapterBase<T>
    {
        private readonly ILoggerService<T> _loggerService;
        private readonly IMessagingLogger _messagingLogger;

        /// <summary>
        /// Creates new instance of <see cref="AsyncGatewayAdapterBase{T}"/>
        /// </summary>
        /// <param name="loggerService"></param>
        /// <param name="messagingLogger"></param>
        protected AsyncGatewayAdapterBase(ILoggerService<T> loggerService,
            IMessagingLogger messagingLogger)
        {
            _loggerService = loggerService;
            _messagingLogger = messagingLogger;
        }

        /// <summary>
        /// Handle the send command.
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="directAsyncGateway"></param>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="queue"></param>
        /// <param name="commandName"></param>
        /// <param name="methodBase"></param>
        /// <param name="callerMemberName"></param>
        /// <returns></returns>
        public async Task HandleSendCommandAsync<TData>(IDirectAsyncGateway<TData> directAsyncGateway, string id, TData data, string queue, string commandName,
            MethodBase methodBase,int retryAttempt, [CallerMemberName] string callerMemberName = null) =>
            await directAsyncGateway.HandleSendCommandAsync(_loggerService, _messagingLogger, id, data, queue, commandName, methodBase, retryAttempt, callerMemberName);

        /// <summary>
        /// Handle the send event.
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="directAsyncGateway"></param>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="topic"></param>
        /// <param name="eventName"></param>
        /// <param name="methodBase"></param>
        /// <param name="callerMemberName"></param>
        /// <returns></returns>
        public async Task HandleSendEventAsync<TData>(IDirectAsyncGateway<TData> directAsyncGateway, string id, TData data, string topic, string eventName,
            MethodBase methodBase, [CallerMemberName] string callerMemberName = null) =>
            await directAsyncGateway.HandleSendEventAsync(_loggerService, _messagingLogger, id, data, topic, eventName, methodBase, callerMemberName);
    }
}