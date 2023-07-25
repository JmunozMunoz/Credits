using System.Threading.Tasks;

namespace Sc.Credits.EntryPoints.ServicesBus.Credits
{
    /// <summary>
    /// Credits payments subscriptions
    /// </summary>
    public class CreditsPaymentsSubscription
    {
        private readonly IActiveCreditsCommandSubscription _activeCreditsCommandSubscription;
        private readonly IPayCreditsCommandSubscription _payCreditsCommandSubscription;

        /// <summary>
        /// Credits payments subscriptions
        /// </summary>
        /// <param name="activeCreditsCommandSubscription"></param>
        public CreditsPaymentsSubscription(IActiveCreditsCommandSubscription activeCreditsCommandSubscription,
               IPayCreditsCommandSubscription payCreditsCommandSubscription)
        {
            _activeCreditsCommandSubscription = activeCreditsCommandSubscription;
            _payCreditsCommandSubscription = payCreditsCommandSubscription;
        }

        /// <summary>
        /// Subscribe all async
        /// </summary>
        /// <returns></returns>
        public async Task SubscribeAllAsync()
        {
            await _activeCreditsCommandSubscription.SubscribeAsync();
            await _payCreditsCommandSubscription.SubscribeAsync();
        }
    }
}