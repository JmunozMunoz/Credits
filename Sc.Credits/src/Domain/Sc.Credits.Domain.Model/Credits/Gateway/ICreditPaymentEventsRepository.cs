using System.Threading.Tasks;

namespace Sc.Credits.Domain.Model.Credits.Gateway
{
    /// <summary>
    /// Credit payment events repository contract
    /// </summary>
    public interface ICreditPaymentEventsRepository
    {
        /// <summary>
        /// Send active credits
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task SendActiveCreditsEventsAsync(string eventName, string id, dynamic data);

        /// <summary>
        /// Send pay credits events
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task SendPayCreditsEventsAsync(string eventName, string id, dynamic data);
    }
}