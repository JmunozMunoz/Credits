using Microsoft.Azure.ServiceBus;
using org.reactivecommons.api;
using org.reactivecommons.api.domain;
using Polly;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.Commons.Messaging;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Sc.Credits.Helpers.Commons.Extensions
{
    /// <summary>
    /// The async gateway logging extensions.
    /// </summary>
    public static class AsyncGatewayLoggingExtensions
    {
        /// <summary>
        /// Handle the send command.
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TLogger"></typeparam>
        /// <param name="directAsyncGateway"></param>
        /// <param name="loggerService"></param>
        /// <param name="messagingLogger"></param>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="queue"></param>
        /// <param name="commandName"></param>
        /// <param name="methodBase"></param>
        /// <param name="callerMemberName"></param>
        /// <returns></returns>
        public static async Task HandleSendCommandAsync<TData, TLogger>(this IDirectAsyncGateway<TData> directAsyncGateway, ILoggerService<TLogger> loggerService,
            IMessagingLogger messagingLogger, string id, TData data, string queue, string commandName, MethodBase methodBase, int retryAttempt,
            [CallerMemberName] string callerMemberName = null)
        {
            Command<TData> command = new Command<TData>(commandName, id, data);

            try
            {
                loggerService.Debug(id, data, "Starting send command.", methodBase, callerMemberName);

                var retryPolicy = Policy.Handle<ServiceBusException>()
                                    .WaitAndRetryAsync(retryAttempt, sleepDuration => TimeSpan.FromSeconds(Math.Pow(2, sleepDuration)));

                await retryPolicy.ExecuteAsync(async () =>
                {
                    try
                    {
                        await directAsyncGateway.SendCommand(queue, command);
                    }
                    catch (ServiceBusException ex)
                    {
                        await loggerService.NotifyAsync(id, commandName, command, methodBase, callerMemberName);
                        throw;
                    }
                });

                loggerService.Debug(id, data, "Successful sended command.", methodBase, callerMemberName);
            }
            catch (Exception ex)
            {
                loggerService.Debug(id, data, "Failed sending command.", methodBase, callerMemberName);

                await HandleSendCommandFaultsAsync(loggerService, messagingLogger, command, queue, ex, methodBase, callerMemberName);
            }
        }

        /// <summary>
        /// Handle the send event.
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TLogger"></typeparam>
        /// <param name="directAsyncGateway"></param>
        /// <param name="loggerService"></param>
        /// <param name="messagingLogger"></param>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="topic"></param>
        /// <param name="eventName"></param>
        /// <param name="methodBase"></param>
        /// <param name="callerMemberName"></param>
        /// <returns></returns>
        public static async Task HandleSendEventAsync<TData, TLogger>(this IDirectAsyncGateway<TData> directAsyncGateway, ILoggerService<TLogger> loggerService,
            IMessagingLogger messagingLogger, string id, TData data, string topic, string eventName, MethodBase methodBase,
            [CallerMemberName] string callerMemberName = null)
        {
            DomainEvent<TData> @event = new DomainEvent<TData>(eventName, id, data);

            try
            {
                loggerService.Debug(id, data, "Starting send event.", methodBase, callerMemberName);

                await directAsyncGateway.SendEvent(topic, @event);

                loggerService.Debug(id, data, "Successful sended event.", methodBase, callerMemberName);
            }
            catch (Exception ex)
            {
                loggerService.Debug(id, data, "Failed sending event.", methodBase, callerMemberName);

                await HandleSendEventFaultsAsync(loggerService, messagingLogger, @event, topic, ex, methodBase, callerMemberName);
            }
        }

        /// <summary>
        /// Handle the send command faults.
        /// </summary>
        /// <param name="loggerService"></param>
        /// <param name="messagingLogger"></param>
        /// <param name="command"></param>
        /// <param name="queue"></param>
        /// <param name="exception"></param>
        /// <param name="methodBase"></param>
        /// <param name="callerMemberName"></param>
        /// <returns></returns>
        private static async Task HandleSendCommandFaultsAsync<TData, TLogger>(ILoggerService<TLogger> loggerService, IMessagingLogger messagingLogger, Command<TData> command,
            string queue, Exception exception, MethodBase methodBase, string callerMemberName)
        {
            string id = command.commandId;

            loggerService.LogError(id, command, exception, methodBase, callerMemberName);

            try
            {
                loggerService.Debug(id, command, "Starting send command log to retry.", methodBase, callerMemberName);

                await messagingLogger.LogQueueErrorAsync(command, queue);

                loggerService.Debug(id, command, "Successful sended command log to retry.", methodBase, callerMemberName);
            }
            catch (Exception ex)
            {
                loggerService.Debug(id, command, "Failed sending command to retries log.", methodBase, callerMemberName);

                loggerService.LogError(id, command, ex, methodBase, callerMemberName);
            }
        }

        /// <summary>
        /// Handle the send event faults.
        /// </summary>
        /// <param name="loggerService"></param>
        /// <param name="messagingLogger"></param>
        /// <param name="event"></param>
        /// <param name="topic"></param>
        /// <param name="exception"></param>
        /// <param name="methodBase"></param>
        /// <param name="callerMemberName"></param>
        /// <returns></returns>
        private static async Task HandleSendEventFaultsAsync<TData, TLogger>(ILoggerService<TLogger> loggerService, IMessagingLogger messagingLogger, DomainEvent<TData> @event,
            string topic, Exception exception, MethodBase methodBase, string callerMemberName)
        {
            string id = @event.eventId;

            loggerService.LogError(id, @event, exception, methodBase, callerMemberName);

            try
            {
                loggerService.Debug(id, @event, "Starting send event log to retry.", methodBase, callerMemberName);

                await messagingLogger.LogTopicErrorAsync(@event, topic);

                loggerService.Debug(id, @event, "Successful sended event log to retry.", methodBase, callerMemberName);
            }
            catch (Exception ex)
            {
                loggerService.Debug(id, @event, "Failed sending event to retries log.", methodBase);

                loggerService.LogError(id, @event, ex, methodBase, callerMemberName);
            }
        }
    }
}