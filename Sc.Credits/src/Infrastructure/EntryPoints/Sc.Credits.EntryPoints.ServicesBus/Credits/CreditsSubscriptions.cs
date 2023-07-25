using System.Threading.Tasks;

namespace Sc.Credits.EntryPoints.ServicesBus.Credits
{
    /// <summary>
    /// Credits subscriptions
    /// </summary>
    public class CreditsSubscriptions
    {
        private readonly ICreditCommandSubscription _creditCommandSubscription;
        private readonly ICreditCreateCommandSubscription _creditCreateCommandSubscription;
        private readonly ICreditQueryCommandSubscription _creditQueryCommandSubscription;

        /// <summary>
        /// New credits subscriptios
        /// </summary>
        /// <param name="creditCommandSubscription"></param>
        /// <param name="creditCreateCommandSubscription"></param>
        /// <param name="creditQueryCommandSubscription"></param>
        public CreditsSubscriptions(ICreditCommandSubscription creditCommandSubscription,
            ICreditCreateCommandSubscription creditCreateCommandSubscription,
            ICreditQueryCommandSubscription creditQueryCommandSubscription)
        {
            _creditCommandSubscription = creditCommandSubscription;
            _creditCreateCommandSubscription = creditCreateCommandSubscription;
            _creditQueryCommandSubscription = creditQueryCommandSubscription;
        }

        /// <summary>
        /// Subscribe all
        /// </summary>
        /// <returns></returns>
        public async Task SubscribeAllAsync()
        {
            await _creditCommandSubscription.SubscribeAsync();
            await _creditCreateCommandSubscription.SubscribeAsync();
            await _creditQueryCommandSubscription.SubscribeAsync();
        }
    }
}