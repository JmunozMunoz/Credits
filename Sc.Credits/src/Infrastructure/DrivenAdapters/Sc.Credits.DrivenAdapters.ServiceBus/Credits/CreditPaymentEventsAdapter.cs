using org.reactivecommons.api;
using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.DrivenAdapters.ServiceBus.Common;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.Commons.Messaging;
using Sc.Credits.Helpers.Commons.Settings;
using Sc.Credits.Helpers.ObjectsUtils;
using System.Reflection;
using System.Threading.Tasks;

namespace Sc.Credits.DrivenAdapters.ServiceBus.Credits
{
    /// <summary>
    /// The credit payment events adapter is an implementation of <see cref="ICreditPaymentEventsRepository"/>
    /// </summary>
    public class CreditPaymentEventsAdapter
        : AsyncGatewayAdapterBase<CreditPaymentEventsAdapter>, ICreditPaymentEventsRepository
    {
        private readonly IDirectAsyncGateway<dynamic> _directAsyncGateway;
        private readonly CredinetAppSettings _credinetAppSettings;

        /// <summary>
        /// Creates new instance of <see cref="CreditPaymentEventsAdapter"/>
        /// </summary>
        /// <param name="directAsyncGateway"></param>
        /// <param name="appSettings"></param>
        public CreditPaymentEventsAdapter(IDirectAsyncGateway<dynamic> directAsyncGateway,
            IMessagingLogger messagingLogger,
            ISettings<CredinetAppSettings> appSettings,
            ILoggerService<CreditPaymentEventsAdapter> loggerService)
            : base(loggerService, messagingLogger)
        {
            _directAsyncGateway = directAsyncGateway;
            _credinetAppSettings = appSettings.Get();
        }

        /// <summary>
        /// <see cref="ICreditPaymentEventsRepository.SendActiveCreditsEventsAsync(string, string, dynamic)"/>
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task SendActiveCreditsEventsAsync(string eventName, string id, dynamic data) =>
            await HandleSendEventAsync(_directAsyncGateway, id, data, _credinetAppSettings.ActiveCreditsEventsCreditsTopic, eventName,
               MethodBase.GetCurrentMethod());

        /// <summary>
        /// <see cref="ICreditPaymentEventsRepository.SendPayCreditsEventsAsync(string, string, dynamic)"/>
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task SendPayCreditsEventsAsync(string eventName, string id, dynamic data) =>
            await HandleSendEventAsync(_directAsyncGateway, id, data, _credinetAppSettings.PayCreditsEventsCreditsTopic, eventName,
               MethodBase.GetCurrentMethod());
    }
}