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
    /// <see cref="ICustomerUpdateMailMobileEventSubscription"/>
    /// </summary>
    public class CustomerUpdateMailMobileEventSubscription
        : SubscriptionBase<CustomerUpdateMailMobileEventSubscription>, ICustomerUpdateMailMobileEventSubscription
    {
        private readonly ICustomerService _customerService;
        private readonly IDirectAsyncGateway<CustomerRequest> _directAsyncCustomerRequest;
        private readonly CredinetAppSettings _credinetAppSettings;

        /// <summary>
        /// New customer update mail mobile event subscription
        /// </summary>
        /// <param name="customerService"></param>
        /// <param name="directAsyncCustomerRequest"></param>
        /// <param name="commons"></param>
        /// <param name="loggerService"></param>
        public CustomerUpdateMailMobileEventSubscription(ICustomerService customerService,
            IDirectAsyncGateway<CustomerRequest> directAsyncCustomerRequest,
            ICommons commons,
            ILoggerService<CustomerUpdateMailMobileEventSubscription> loggerService)
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
            await SubscribeOnEventAsync(_directAsyncCustomerRequest, _credinetAppSettings.CustomerUpdateMailMobileTopic,
                _credinetAppSettings.CreditsMailMobileSubscription, ProcessMessagesCustomerRequest,
                MethodBase.GetCurrentMethod(), _credinetAppSettings.TopicMaxConcurrentCalls);
        }

        /// <summary>
        /// Process messages customer request
        /// </summary>
        /// <param name="domainEvent"></param>
        /// <param name="eventDate"></param>
        /// <returns></returns>
        private async Task ProcessMessagesCustomerRequest(DomainEvent<CustomerRequest> domainEvent, DateTime eventDate) =>
            await HandleRequestAsync(async (request) =>
                {
                    if (domainEvent.name.ToLower().EndsWith(".changemail"))
                    {
                        await _customerService.UpdateMailAsync(request.IdDocument, request.TypeDocument, request.Email, request.VerifiedEmail ?? false);
                    }

                    if (domainEvent.name.ToLower().EndsWith(".changemobile"))
                    {
                        await _customerService.UpdateMobileAsync(request.IdDocument, request.TypeDocument, request.MovileNumber);
                    }
                },
            MethodBase.GetCurrentMethod(),
            domainEvent.data?.IdDocument.ToString(),
            domainEvent);
    }
}