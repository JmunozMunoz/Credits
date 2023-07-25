using org.reactivecommons.api;
using org.reactivecommons.api.domain;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Customers;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.EntryPoints.ServicesBus.Base;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Sc.Credits.EntryPoints.ServicesBus.Customer
{
    /// <summary>
    /// <see cref="ICustomerUpdateStudyEventSubscription"/>
    /// </summary>
    public class CustomerUpdateStudyEventSubscription
        : SubscriptionBase<CustomerUpdateStudyEventSubscription>, ICustomerUpdateStudyEventSubscription
    {
        private readonly ICustomerService _customerService;
        private readonly IDirectAsyncGateway<CustomerRequest> _directAsyncCustomerRequest;
        private readonly CredinetAppSettings _credinetAppSettings;

        /// <summary>
        /// New customer update study event subscription
        /// </summary>
        /// <param name="customerService"></param>
        /// <param name="directAsyncCustomerRequest"></param>
        /// <param name="commons"></param>
        /// <param name="loggerService"></param>
        public CustomerUpdateStudyEventSubscription(ICustomerService customerService,
            IDirectAsyncGateway<CustomerRequest> directAsyncCustomerRequest,
            ICommons commons,
            ILoggerService<CustomerUpdateStudyEventSubscription> loggerService)
            : base(commons.CredinetAppSettings, loggerService)
        {
            _customerService = customerService;
            _directAsyncCustomerRequest = directAsyncCustomerRequest;
            _credinetAppSettings = commons.CredinetAppSettings;
        }

        /// <summary>
        /// <see cref="ISubscription.SubscribeAsync"/>
        /// </summary>
        /// <returns></returns>
        public async Task SubscribeAsync()
        {
            await SubscribeOnEventAsync(_directAsyncCustomerRequest, _credinetAppSettings.CustomerUpdateStudyEventsTopic,
               _credinetAppSettings.CreditsUpdateStudyEventsSubscription, ProcessMessagesCustomerRequest,
               MethodBase.GetCurrentMethod(), _credinetAppSettings.TopicMaxConcurrentCalls);
        }

        /// <summary>
        /// Process messages customer response
        /// </summary>
        /// <param name="domainEvent"></param>
        /// <param name="eventDate"></param>
        /// <returns></returns>
        private async Task ProcessMessagesCustomerRequest(DomainEvent<CustomerRequest> domainEvent, DateTime eventDate) =>
            await HandleRequestAsync(async (request) =>
                await _customerService.CreateOrUpdateStatusAsync(request, eventDate.ToLocalTime()),
            MethodBase.GetCurrentMethod(),
            domainEvent.data?.IdDocument.ToString(),
            domainEvent);
    }
}