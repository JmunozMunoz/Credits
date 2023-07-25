using credinet.comun.models.Credits;
using org.reactivecommons.api;
using org.reactivecommons.api.domain;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Customers;
using Sc.Credits.EntryPoints.ServicesBus.Base;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Sc.Credits.EntryPoints.ServicesBus.Customer
{
    /// <summary>
    /// <see cref="ICustomerCreditLimitUpdateEventSubscription"/>
    /// </summary>
    public class CustomerCreditLimitUpdateEventSubscription
        : SubscriptionBase<CustomerCreditLimitUpdateEventSubscription>, ICustomerCreditLimitUpdateEventSubscription
    {
        private readonly IDirectAsyncGateway<CreditLimitResponse> _directAsyncCreditLimitResponse;
        private readonly ICustomerService _customerService;
        private readonly CredinetAppSettings _credinetAppSettings;

        /// <summary>
        /// New credit limit update event subscription
        /// </summary>
        /// <param name="customerService"></param>
        /// <param name="directAsyncCreditLimitResponse"></param>
        /// <param name="commons"></param>
        /// <param name="loggerService"></param>
        public CustomerCreditLimitUpdateEventSubscription(ICustomerService customerService,
            IDirectAsyncGateway<CreditLimitResponse> directAsyncCreditLimitResponse,
            ICommons commons,
            ILoggerService<CustomerCreditLimitUpdateEventSubscription> loggerService)
            : base(commons.CredinetAppSettings, loggerService)
        {
            _directAsyncCreditLimitResponse = directAsyncCreditLimitResponse;
            _credinetAppSettings = commons.CredinetAppSettings;
            _customerService = customerService;
        }

        /// <summary>
        /// <see cref="ISubscription.SubscribeAsync"/>
        /// </summary>
        /// <returns></returns>
        public async Task SubscribeAsync()
        {
            await SubscribeOnEventAsync(_directAsyncCreditLimitResponse, _credinetAppSettings.TopicCreditLimitUpdate, _credinetAppSettings.CreditLimitUpdateSuscription,
               ProcessCreditLimitUpdateResponse, MethodBase.GetCurrentMethod(), _credinetAppSettings.TopicMaxConcurrentCalls);
        }

        /// <summary>
        /// Process credit limit update response
        /// </summary>
        /// <param name="domainEvent"></param>
        /// <returns></returns>
        private async Task ProcessCreditLimitUpdateResponse(DomainEvent<CreditLimitResponse> domainEvent, DateTime eventDate)
        {
            Validate(domainEvent);

            if (domainEvent.name.ToLower().Trim() == "legacy.creditlimitupdate")
            {
                await HandleRequestAsync(async (request) =>
                    await _customerService.TryCreditLimitUpdateAsync(request, eventDate.ToLocalTime()),
                MethodBase.GetCurrentMethod(),
                domainEvent.data?.IdDocument.ToString(),
                domainEvent);
            }
        }
    }
}