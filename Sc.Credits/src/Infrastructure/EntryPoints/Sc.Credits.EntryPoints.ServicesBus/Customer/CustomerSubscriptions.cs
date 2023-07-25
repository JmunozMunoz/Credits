using System.Threading.Tasks;

namespace Sc.Credits.EntryPoints.ServicesBus.Customer
{
    /// <summary>
    /// Customer subscriptions
    /// </summary>
    public class CustomerSubscriptions
    {
        private readonly ICustomerUpdateEventSubscription _customerUpdateEventSuscription;
        private readonly ICustomerStatusUpdateEventSubscription _customerUpdateStatusEventSuscription;
        private readonly ICustomerUpdateMailMobileEventSubscription _customerUpdateMailMobileEventSuscription;
        private readonly ICustomerUpdateMigrationEventSubscription _customerUpdateMigrationEventSuscription;
        private readonly ICustomerUpdateStudyEventSubscription _customerUpdateStudyEventSuscription;
        private readonly ICustomerCreditLimitUpdateEventSubscription _customerCreditLimitUpdateEventSuscription;

        /// <summary>
        /// New customer subscriptions
        /// </summary>
        /// <param name="customerUpdateEventSuscription"></param>
        /// <param name="customerUpdateStatusEventSuscription"></param>
        /// <param name="customerUpdateMailMobileEventSuscription"></param>
        /// <param name="customerUpdateMigrationEventSuscription"></param>
        /// <param name="customerUpdateStudyEventSuscription"></param>
        /// <param name="customerCreditLimitUpdateEventSuscription"></param>
        public CustomerSubscriptions(ICustomerUpdateEventSubscription customerUpdateEventSuscription,
            ICustomerStatusUpdateEventSubscription customerUpdateStatusEventSuscription,
            ICustomerUpdateMailMobileEventSubscription customerUpdateMailMobileEventSuscription,
            ICustomerUpdateMigrationEventSubscription customerUpdateMigrationEventSuscription,
            ICustomerUpdateStudyEventSubscription customerUpdateStudyEventSuscription,
            ICustomerCreditLimitUpdateEventSubscription customerCreditLimitUpdateEventSuscription)
        {
            _customerUpdateEventSuscription = customerUpdateEventSuscription;
            _customerUpdateStatusEventSuscription = customerUpdateStatusEventSuscription;
            _customerUpdateMailMobileEventSuscription = customerUpdateMailMobileEventSuscription;
            _customerUpdateMigrationEventSuscription = customerUpdateMigrationEventSuscription;
            _customerUpdateStudyEventSuscription = customerUpdateStudyEventSuscription;
            _customerCreditLimitUpdateEventSuscription = customerCreditLimitUpdateEventSuscription;
        }

        /// <summary>
        /// Subscribe all
        /// </summary>
        /// <returns></returns>
        public async Task SubscribeAllAsync()
        {
            await _customerUpdateEventSuscription.SubscribeAsync();
            await _customerUpdateStatusEventSuscription.SubscribeAsync();
            await _customerUpdateMailMobileEventSuscription.SubscribeAsync();
            await _customerUpdateMigrationEventSuscription.SubscribeAsync();
            await _customerUpdateStudyEventSuscription.SubscribeAsync();
            await _customerCreditLimitUpdateEventSuscription.SubscribeAsync();
        }
    }
}