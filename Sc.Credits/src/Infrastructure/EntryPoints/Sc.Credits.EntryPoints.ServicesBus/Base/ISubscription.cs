using System.Threading.Tasks;

namespace Sc.Credits.EntryPoints.ServicesBus.Base
{
    /// <summary>
    /// Subscription contract
    /// </summary>
    public interface ISubscription
    {
        /// <summary>
        /// Subscribe
        /// </summary>
        /// <returns></returns>
        Task SubscribeAsync();
    }
}