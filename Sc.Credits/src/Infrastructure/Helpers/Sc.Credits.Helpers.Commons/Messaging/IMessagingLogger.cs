using org.reactivecommons.api.domain;
using System.Threading.Tasks;

namespace Sc.Credits.Helpers.Commons.Messaging
{
    /// <summary>
    /// The messaging logger contract.
    /// </summary>
    public interface IMessagingLogger
    {
        /// <summary>
        /// Log a topic error.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        Task LogTopicErrorAsync<T>(DomainEvent<T> @event, string resourceName);

        /// <summary>
        /// Log a queue error.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        Task LogQueueErrorAsync<T>(Command<T> @command, string resourceName);
    }
}