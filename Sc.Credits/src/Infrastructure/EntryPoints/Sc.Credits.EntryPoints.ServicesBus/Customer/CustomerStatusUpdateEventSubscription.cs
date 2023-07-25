using credinet.comun.models.Study;
using org.reactivecommons.api;
using org.reactivecommons.api.domain;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Customers;
using Sc.Credits.EntryPoints.ServicesBus.Base;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.ObjectsUtils;
using System.Reflection;
using System.Threading.Tasks;

namespace Sc.Credits.EntryPoints.ServicesBus.Customer
{
    /// <summary>
    /// <see cref="ICustomerStatusUpdateEventSubscription"/>
    /// </summary>
    public class CustomerStatusUpdateEventSubscription
        : SubscriptionBase<CustomerStatusUpdateEventSubscription>, ICustomerStatusUpdateEventSubscription
    {
        private readonly ICustomerService _customerService;
        private readonly IDirectAsyncGateway<StudyResponse> _directAsyncStudyRespose;
        private readonly CredinetAppSettings _credinetAppSettings;

        /// <summary>
        /// New customer status update event subscription
        /// </summary>
        /// <param name="customerService"></param>
        /// <param name="directAsyncStudyRespose"></param>
        /// <param name="commons"></param>
        /// <param name="loggerService"></param>
        public CustomerStatusUpdateEventSubscription(ICustomerService customerService,
            IDirectAsyncGateway<StudyResponse> directAsyncStudyRespose,
            ICommons commons,
            ILoggerService<CustomerStatusUpdateEventSubscription> loggerService)
            : base(commons.CredinetAppSettings, loggerService)
        {
            _customerService = customerService;
            _directAsyncStudyRespose = directAsyncStudyRespose;
            _credinetAppSettings = commons.CredinetAppSettings;
        }

        /// <summary>
        /// <see cref="ISubscription.SubscribeAsync"/>
        /// </summary>
        /// <returns></returns>
        public async Task SubscribeAsync()
        {
            await SubscribeOnEventAsync(_directAsyncStudyRespose, _credinetAppSettings.CustomerStatusUpdateLegacyTopic,
                _credinetAppSettings.CreditsStatusUpdateLegacySubscription, ProcessMessagesStatusUpdate,
                MethodBase.GetCurrentMethod(), _credinetAppSettings.TopicMaxConcurrentCalls);
        }

        /// <summary>
        /// Process messages status update
        /// </summary>
        /// <param name="domainEvent"></param>
        /// <returns></returns>
        protected async Task ProcessMessagesStatusUpdate(DomainEvent<StudyResponse> domainEvent) =>
            await HandleRequestAsync(async (request) =>
                await _customerService.UpdateStatusAsync(request),
            MethodBase.GetCurrentMethod(),
            domainEvent.data?.IdDocument.ToString(),
            domainEvent);
    }
}