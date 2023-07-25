using credinet.exception.middleware.models;
using org.reactivecommons.api;
using org.reactivecommons.api.domain;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Sc.Credits.EntryPoints.ServicesBus.Base
{
    /// <summary>
    /// Subscription base
    /// </summary>
    public class SubscriptionBase<T>
    {
        private readonly ILoggerService<T> _loggerService;
        private readonly CredinetAppSettings _credinetAppSettings;

        /// <summary>
        /// New subscription base
        /// </summary>
        /// <param name="commons"></param>
        /// <param name="loggerService"></param>
        public SubscriptionBase(CredinetAppSettings credinetAppSettings,
            ILoggerService<T> loggerService)
        {
            _loggerService = loggerService;
            _credinetAppSettings = credinetAppSettings;
        }

        /// <summary>
        /// Subscribe on event
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="directAsyncGateway"></param>
        /// <param name="targetName"></param>
        /// <param name="subscriptionName"></param>
        /// <param name="handler"></param>
        /// <param name="methodBase"></param>
        /// <param name="maxConcurrentCalls"></param>
        /// <param name="callerMemberName"></param>
        /// <returns></returns>
        public async Task SubscribeOnEventAsync<TEntity>(IDirectAsyncGateway<TEntity> directAsyncGateway,
            string targetName, string subscriptionName, Func<DomainEvent<TEntity>, Task> handler, MethodBase methodBase, int maxConcurrentCalls = 1,
            [CallerMemberName] string callerMemberName = null)
        {
            string eventName = GetEventName(methodBase, callerMemberName);
            try
            {
                _loggerService.LogInfo(id: eventName, GetEventName(methodBase, callerMemberName), data: null);

                await directAsyncGateway.SuscripcionEvent(targetName, subscriptionName, handler, maxConcurrentCalls);
            }
            catch (Exception ex)
            {
                await LogAndNotifyAsync(ex, methodBase, targetName, new { targetName, subscriptionName }, callerMemberName);
            }
        }

        /// <summary>
        /// Subscribe on event
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="directAsyncGateway"></param>
        /// <param name="targetName"></param>
        /// <param name="subscriptionName"></param>
        /// <param name="handler"></param>
        /// <param name="methodBase"></param>
        /// <param name="maxConcurrentCalls"></param>
        /// <param name="callerMemberName"></param>
        /// <returns></returns>
        public async Task SubscribeOnEventAsync<TEntity>(IDirectAsyncGateway<TEntity> directAsyncGateway,
            string targetName, string subscriptionName, Func<DomainEvent<TEntity>, DateTime, Task> handler, MethodBase methodBase, int maxConcurrentCalls = 1,
            [CallerMemberName] string callerMemberName = null)
        {
            string eventName = GetEventName(methodBase, callerMemberName);
            try
            {
                _loggerService.LogInfo(id: eventName, eventName, data: null);

                await directAsyncGateway.SuscripcionEvent(targetName, subscriptionName, handler, maxConcurrentCalls);
            }
            catch (Exception ex)
            {
                await LogAndNotifyAsync(ex, methodBase, targetName, new { targetName, subscriptionName }, callerMemberName);
            }
        }

        /// <summary>
        /// Subscribe on command
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="directAsyncGateway"></param>
        /// <param name="targetName"></param>
        /// <param name="handler"></param>
        /// <param name="methodBase"></param>
        /// <param name="maxConcurrentCalls"></param>
        /// <param name="callerMemberName"></param>
        /// <returns></returns>
        public async Task SubscribeOnCommandAsync<TEntity>(IDirectAsyncGateway<TEntity> directAsyncGateway,
            string targetName, Func<Command<TEntity>, Task> handler, MethodBase methodBase, int maxConcurrentCalls = 1,
            [CallerMemberName] string callerMemberName = null)
        {
            string eventName = GetEventName(methodBase, callerMemberName);
            try
            {
                _loggerService.LogInfo(id: eventName, eventName, data: null);

                await directAsyncGateway.SuscripcionCommand(targetName, handler, maxConcurrentCalls);
            }
            catch (Exception ex)
            {
                await LogAndNotifyAsync(ex, methodBase, targetName, new { targetName }, callerMemberName);
            }
        }

        /// <summary>
        /// Subscribe on response reply
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="directAsyncGateway"></param>
        /// <param name="resource"></param>
        /// <param name="targetName"></param>
        /// <param name="handler"></param>
        /// <param name="methodBase"></param>
        /// <param name="maxConcurrentSessions"></param>
        /// <param name="callerMemberName"></param>
        /// <returns></returns>
        public async Task SubscribeOnResponseReplyAsync<TEntity, TResult>(IDirectAsyncGateway<TEntity> directAsyncGateway,
            string resource, string targetName, Func<AsyncQuery<TEntity>, Task<TResult>> handler, MethodBase methodBase, int maxConcurrentSessions = 1,
            [CallerMemberName] string callerMemberName = null)
        {
            string eventName = GetEventName(methodBase, callerMemberName);
            try
            {
                _loggerService.LogInfo(id: eventName, eventName, data: null);

                await directAsyncGateway.ResponseReply(resource, targetName, handler, maxConcurrentSessions);
            }
            catch (Exception ex)
            {
                await LogAndNotifyAsync(ex, methodBase, targetName, new { targetName }, callerMemberName);
            }
        }

        /// <summary>
        /// Handle request command
        /// </summary>
        /// <param name="serviceHandler"></param>
        /// <param name="methodBase"></param>
        /// <param name="logId"></param>
        /// <param name="request"></param>
        /// <param name="notifyBusinessException"></param>
        /// <param name="callerMemberName"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public async Task HandleRequestAsync<TEntity>(Func<TEntity, Task> serviceHandler, MethodBase methodBase, string logId, Command<TEntity> request,
            bool notifyBusinessException = false, [CallerMemberName] string callerMemberName = null, bool registerRequest=false) =>
            await HandleAsync(async () =>
                {
                    Validate(request);
                    return await InvokeAsync(async (entity) =>
                        {
                            await serviceHandler(entity);
                            return true;
                        },
                        request.data);
                },
                methodBase,
                logId,
                request,
                callerMemberName,
                notifyBusinessException,
                registerRequest: registerRequest);

        /// <summary>
        /// Handle request domain event
        /// </summary>
        /// <param name="serviceHandler"></param>
        /// <param name="methodBase"></param>
        /// <param name="logId"></param>
        /// <param name="request"></param>
        /// <param name="notifyBusinessException"></param>
        /// <param name="callerMemberName"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public async Task HandleRequestAsync<TEntity>(Func<TEntity, Task> serviceHandler, MethodBase methodBase, string logId, DomainEvent<TEntity> request,
            bool notifyBusinessException = false, [CallerMemberName] string callerMemberName = null,bool registerRequest = false) =>
            await HandleAsync(async () =>
            {
                Validate(request);
                return await InvokeAsync(async (entity) =>
                    {
                        await serviceHandler(entity);
                        return true;
                    },
                    request.data);
            },
            methodBase,
            logId,
            request,
            callerMemberName,
            notifyBusinessException,
            registerRequest);

        /// <summary>
        /// Handle request request reply
        /// </summary>
        /// <param name="serviceHandler"></param>
        /// <param name="methodBase"></param>
        /// <param name="logId"></param>
        /// <param name="request"></param>
        /// <param name="notifyBusinessException"></param>
        /// <param name="callerMemberName"></param>
        /// <returns></returns>
        public async Task<TResult> HandleRequestAsync<TEntity, TResult>(Func<TEntity, Task<TResult>> serviceHandler, MethodBase methodBase, string logId,
            AsyncQuery<TEntity> request, bool notifyBusinessException = false, [CallerMemberName] string callerMemberName = null) =>
            await HandleAsync(async () =>
            {
                Validate(request);
                return await InvokeAsync(serviceHandler, request.QueryData);
            },
            methodBase,
            logId,
            request,
            callerMemberName,
            notifyBusinessException);

        #region Private

        /// <summary>
        /// Get event name
        /// </summary>
        /// <param name="ex"></param>
        private string GetEventName(MethodBase methodBase, string callerMemberName, Exception ex = null)
        {
            string defaultEventName = $"{_credinetAppSettings.DomainName}.{methodBase.DeclaringType.DeclaringType.Name}.{callerMemberName}";

            return ex != null ?
                $"Exception.{defaultEventName}.{ex.GetType().Name}"
                :
                defaultEventName;
        }

        /// <summary>
        /// Handle request
        /// </summary>
        /// <param name="serviceHandler"></param>
        /// <param name="methodBase"></param>
        /// <param name="logId"></param>
        /// <param name="request"></param>
        /// <param name="notifyBusinessException"></param>
        /// <param name="callerMemberName"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        private async Task<TResult> HandleAsync<TResult, TRequest>(Func<Task<TResult>> serviceHandler, MethodBase methodBase,
            string logId, TRequest request, string callerMemberName, bool notifyBusinessException = false, bool registerRequest=false)
        {
            try
            {
                _loggerService.LogInfo(logId, GetEventName(methodBase, callerMemberName), data: GetLogDetails(request));

                if (registerRequest)
                {
                    await _loggerService.NotifyAsync(logId, GetEventName(methodBase, callerMemberName), data: GetLogDetails(request));
                }

                return await serviceHandler();
            }
            catch (BusinessException be)
            {
                LogBusinessException(be, methodBase, logId, request, callerMemberName);

                if (notifyBusinessException)
                {
                    await _loggerService.NotifyAsync(logId, GetEventName(methodBase, callerMemberName, be),
                        data: GetLogDetails(request, be));
                }
                throw;
            }
            catch (Exception ex)
            {
                await LogAndNotifyAsync(ex, methodBase, logId, request, callerMemberName);
                throw;
            }
        }

        /// <summary>
        /// Invoke
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="handler"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        private async Task<TResult> InvokeAsync<TEntity, TResult>(Func<TEntity, Task<TResult>> handler, TEntity entity)
        {
            return await handler(entity);
        }

        /// <summary>
        /// Validate
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="command"></param>
        public void Validate<TEntity>(Command<TEntity> command)
        {
            if (command == null || command.data == null)
            {
                throw new ArgumentNullException(nameof(command));
            }
        }

        /// <summary>
        /// Validate
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="event"></param>
        public void Validate<TEntity>(DomainEvent<TEntity> @event)
        {
            if (@event == null || @event.data == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }
        }

        /// <summary>
        /// Validate
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="query"></param>
        public void Validate<TEntity>(AsyncQuery<TEntity> query)
        {
            if (query == null || query.QueryData == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
        }

        /// <summary>
        /// Log and notify
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="methodBase"></param>
        /// <param name="logId"></param>
        /// <param name="request"></param>
        /// <param name="callerMemberName"></param>
        /// <returns></returns>
        private async Task LogAndNotifyAsync(Exception ex, MethodBase methodBase, string logId, dynamic request,
            string callerMemberName)
        {
            object logDetails = GetLogDetails(request, ex);

            string eventName = GetEventName(methodBase, callerMemberName, ex);

            _loggerService.LogError(logId, eventName, logDetails, ex);

            await _loggerService.NotifyAsync(logId, eventName, logDetails);
        }

        /// <summary>
        /// Log business exception
        /// </summary>
        /// <param name="be"></param>
        /// <param name="logId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private void LogBusinessException(BusinessException be, MethodBase methodBase, string logId, dynamic request,
            string callerMemberName)
        {
            object logDetails = GetLogDetails(request, be);

            string eventName = GetEventName(methodBase, callerMemberName, be);

            _loggerService.LogWarning(logId, eventName, logDetails, be);
        }

        /// <summary>
        /// Get log details
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private object GetLogDetails(dynamic request, Exception ex = null) =>
            ex == null ?
            request
            :
            new
            {
                exception = ex.ToString(),
                request
            };

        #endregion Private
    }
}