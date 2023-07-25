using System.Threading.Tasks;

namespace Sc.Credits.Helpers.Commons.Logging.Gateway
{
    /// <summary>
    /// Events Repository
    /// </summary>
    public interface IEventsRepository
    {
        /// <summary>
        /// Send
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task SendAsync(string eventName, string id, dynamic data);
    }
}