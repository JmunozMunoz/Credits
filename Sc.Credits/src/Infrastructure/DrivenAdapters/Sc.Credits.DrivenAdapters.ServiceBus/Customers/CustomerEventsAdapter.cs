using credinet.comun.models.Credits;
using org.reactivecommons.api;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Customers.Gateway;
using Sc.Credits.DrivenAdapters.ServiceBus.Common;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.Commons.Messaging;
using Sc.Credits.Helpers.Commons.Settings;
using Sc.Credits.Helpers.ObjectsUtils;
using System.Reflection;
using System.Threading.Tasks;

namespace Sc.Credits.DrivenAdapters.ServiceBus.Customers
{
    /// <summary>
    /// The customer events adapter is an implementation of <see cref="ICustomerEventsRepository"/>
    /// </summary>
    public class CustomerEventsAdapter
        : AsyncGatewayAdapterBase<CustomerEventsAdapter>, ICustomerEventsRepository
    {
        private readonly IDirectAsyncGateway<CreditLimitResponse> _directAsyncGatewayCreditLimitResponse;
        private readonly CredinetAppSettings _credinetAppSettings;

        /// <summary>
        /// Creates new instance of <see cref="CustomerEventsAdapter"/>
        /// </summary>
        /// <param name="directAsyncGatewayCreditLimitResponse"></param>
        /// <param name="appSettings"></param>
        /// <param name="loggerService"></param>
        /// <param name="messagingLogger"></param>
        public CustomerEventsAdapter(IDirectAsyncGateway<CreditLimitResponse> directAsyncGatewayCreditLimitResponse,
            ISettings<CredinetAppSettings> appSettings,
            ILoggerService<CustomerEventsAdapter> loggerService,
            IMessagingLogger messagingLogger)
            : base(loggerService, messagingLogger)
        {
            _directAsyncGatewayCreditLimitResponse = directAsyncGatewayCreditLimitResponse;
            _credinetAppSettings = appSettings.Get();
        }

        /// <summary>
        /// <see cref="ICustomerEventsRepository.NotifyCreditLimitUpdate(Customer)"/>
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public async Task NotifyCreditLimitUpdateAsync(Customer customer)
        {
            string eventName = "Credits.CreditLimitUpdate";

            CreditLimitResponse creditLimitResponse = new CreditLimitResponse
            {
                AvailableCreditLimit = customer.GetAvailableCreditLimit,
                CreditLimit = customer.GetCreditLimit,
                IdDocument = customer.IdDocument,
                TypeDocument = customer.DocumentType
            };

            await HandleSendEventAsync(_directAsyncGatewayCreditLimitResponse, customer.IdDocument,
                creditLimitResponse, _credinetAppSettings.TopicCreditLimitUpdate, eventName,
                MethodBase.GetCurrentMethod());
        }
    }
}