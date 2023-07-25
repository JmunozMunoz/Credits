using System.Threading.Tasks;

namespace Sc.Credits.Helpers.Commons.Messaging.Gateway
{
    /// <summary>
    /// Message log repository
    /// </summary>
    public interface IMessageLogRepository
    {
        /// <summary>
        /// Add error
        /// </summary>
        /// <param name="messageErrorLog"></param>
        Task AddErrorAsync(MessageErrorLog messageErrorLog);
    }
}