using Newtonsoft.Json;
using org.reactivecommons.api.domain;
using Sc.Credits.Helpers.Commons.Messaging.Gateway;
using System.Threading.Tasks;

namespace Sc.Credits.Helpers.Commons.Messaging
{
    /// <summary>
    /// <see cref="IMessagingLogger"/>
    /// </summary>
    public class MessagingLogger
        : IMessagingLogger
    {
        private readonly IMessageLogRepository _messageLogRepository;

        /// <summary>
        /// New messaging logger
        /// </summary>
        /// <param name="messageLogRepository"></param>
        public MessagingLogger(IMessageLogRepository messageLogRepository)
        {
            _messageLogRepository = messageLogRepository;
        }

        /// <summary>
        /// <see cref="IMessagingLogger.LogTopicErrorAsync{T}(DomainEvent{T}, string)"/>
        /// </summary>
        /// <param name="event"></param>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public async Task LogTopicErrorAsync<T>(DomainEvent<T> @event, string resourceName) =>
            await _messageLogRepository.AddErrorAsync(GetMessageError(@event, resourceName, ResourceTypes.Topic));

        /// <summary>
        /// <see cref="IMessagingLogger.LogQueueErrorAsync{T}(Command{T}, string)"/>
        /// </summary>
        /// <param name="command"></param>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public async Task LogQueueErrorAsync<T>(Command<T> command, string resourceName) =>
            await _messageLogRepository.AddErrorAsync(GetMessageError(command, resourceName, ResourceTypes.Queue));

        /// <summary>
        /// Get message error
        /// </summary>
        /// <param name="event"></param>
        /// <param name="resourceName"></param>
        /// <param name="resourceType"></param>
        /// <returns></returns>
        private MessageErrorLog GetMessageError<T>(DomainEvent<T> @event, string resourceName, ResourceTypes resourceType)
        {
            string json = JsonConvert.SerializeObject(@event);

            return new MessageErrorLog(@event.name, @event.eventId, resourceType, resourceName, json);
        }

        /// <summary>
        /// Get message error
        /// </summary>
        /// <param name="command"></param>
        /// <param name="resourceName"></param>
        /// <param name="resourceType"></param>
        /// <returns></returns>
        private MessageErrorLog GetMessageError<T>(Command<T> command, string resourceName, ResourceTypes resourceType)
        {
            string json = JsonConvert.SerializeObject(command);

            return new MessageErrorLog(command.name, command.commandId, resourceType, resourceName, json);
        }
    }
}