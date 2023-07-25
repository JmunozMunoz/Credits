using org.reactivecommons.api;
using org.reactivecommons.api.domain;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Stores;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.EntryPoints.ServicesBus.Base;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.ObjectsUtils;
using System.Reflection;
using System.Threading.Tasks;

namespace Sc.Credits.EntryPoints.ServicesBus.Store
{
    /// <summary>
    /// <see cref="IStoreCommandSubscription"/>
    /// </summary>
    public class StoreCommandSubscription
        : SubscriptionBase<StoreCommandSubscription>, IStoreCommandSubscription
    {
        private readonly IDirectAsyncGateway<StoreRequest> _directAsyncStore;
        private readonly IStoreService _storeService;
        private readonly CredinetAppSettings _credinetAppSettings;

        /// <summary>
        /// New store command subscription
        /// </summary>
        /// <param name="storeService"></param>
        /// <param name="directAsync"></param>
        /// <param name="commons"></param>
        /// <param name="loggerService"></param>
        public StoreCommandSubscription(IStoreService storeService,
            IDirectAsyncGateway<StoreRequest> directAsync,
            ICommons commons,
            ILoggerService<StoreCommandSubscription> loggerService)
            : base(commons.CredinetAppSettings, loggerService)
        {
            _directAsyncStore = directAsync;
            _storeService = storeService;
            _credinetAppSettings = commons.CredinetAppSettings;
        }

        /// <summary>
        /// <see cref="ISubscription.SubscribeAsync"/>
        /// </summary>
        /// <returns></returns>
        public async Task SubscribeAsync()
        {
            await SubscribeOnCommandAsync(_directAsyncStore, _credinetAppSettings.StoreQueueNameSubscription, ProcessMessagesStore, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Process Messages Store
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private async Task ProcessMessagesStore(Command<StoreRequest> command) =>
            await HandleRequestAsync(async (request) =>
            {
                await _storeService.CreateOrUpdateAsync(request);
            },
            MethodBase.GetCurrentMethod(),
            command.data?.Id.ToString(),
            command);
    }
}