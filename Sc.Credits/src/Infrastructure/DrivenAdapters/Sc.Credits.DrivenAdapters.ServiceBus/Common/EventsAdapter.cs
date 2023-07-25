using org.reactivecommons.api;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.Commons.Logging.Gateway;
using Sc.Credits.Helpers.Commons.Messaging;
using Sc.Credits.Helpers.Commons.Settings;
using Sc.Credits.Helpers.ObjectsUtils;
using System.Reflection;
using System.Threading.Tasks;

namespace Sc.Credits.DrivenAdapters.ServiceBus.Common
{
    /// <summary>
    /// The events adapter is an instance of <see cref="IEventsRepository"/>
    /// </summary>
    public class EventsAdapter
        : AsyncGatewayAdapterBase<EventsAdapter>, IEventsRepository
    {
        private readonly IDirectAsyncGateway<dynamic> _directAsyncGateway;
        private readonly string _eventsTopicName;

        /// <summary>
        /// Creates new instance of <see cref="EventsAdapter"/>
        /// </summary>
        /// <param name="directAsyncGateway"></param>
        /// <param name="messagingLogger"></param>
        /// <param name="loggerServiceLazy"></param>
        /// <param name="appSettings"></param>
        public EventsAdapter(IDirectAsyncGateway<dynamic> directAsyncGateway,
            IMessagingLogger messagingLogger,
            ILoggerService<EventsAdapter> loggerService,
            ISettings<CredinetAppSettings> appSettings)
            : base(loggerService, messagingLogger)
        {
            _directAsyncGateway = directAsyncGateway;
            CredinetAppSettings credinetAppSettings = appSettings.Get();
            _eventsTopicName = credinetAppSettings.EventsTopicName;
        }

        /// <summary>
        /// <see cref="IEventsRepository.SendAsync(string, string, dynamic)"/>
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task SendAsync(string eventName, string id, dynamic data) =>
           await HandleSendEventAsync(_directAsyncGateway, id, data, _eventsTopicName, eventName,
               MethodBase.GetCurrentMethod());
    }
}