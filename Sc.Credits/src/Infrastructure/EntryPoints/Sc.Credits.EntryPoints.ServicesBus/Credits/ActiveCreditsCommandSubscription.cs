using org.reactivecommons.api;
using org.reactivecommons.api.domain;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Credits;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.EntryPoints.ServicesBus.Base;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.ObjectsUtils;
using System.Reflection;
using System.Threading.Tasks;

namespace Sc.Credits.EntryPoints.ServicesBus.Credits
{
    /// <summary>
    /// <see cref="IActiveCreditsCommandSubscription"/>
    /// </summary>
    public class ActiveCreditsCommandSubscription
        : SubscriptionBase<ActiveCreditsCommandSubscription>, IActiveCreditsCommandSubscription
    {
        private readonly ICreditPaymentService _paymentService;
        private readonly CredinetAppSettings _credinetAppSettings;
        private readonly IDirectAsyncGateway<ActiveCreditsRequest> _directAsyncActiveCredit;

        /// <summary>
        /// Active credits command subscription
        /// </summary>
        /// <param name="paymentService"></param>
        /// <param name="directAsyncActiveCredit"></param>
        /// <param name="commons"></param>
        /// <param name="loggerService"></param>
        public ActiveCreditsCommandSubscription(ICreditPaymentService paymentService,
            IDirectAsyncGateway<ActiveCreditsRequest> directAsyncActiveCredit,
            ICommons commons,
            ILoggerService<ActiveCreditsCommandSubscription> loggerService)
            : base(commons.CredinetAppSettings, loggerService)
        {
            _paymentService = paymentService;
            _credinetAppSettings = commons.CredinetAppSettings;
            _directAsyncActiveCredit = directAsyncActiveCredit;
        }

        /// <summary>
        /// <see cref="ISubscription.SubscribeAsync"/>
        /// </summary>
        /// <returns></returns>
        public async Task SubscribeAsync()
        {
            await SubscribeOnCommandAsync(_directAsyncActiveCredit, _credinetAppSettings.ActiveCreditsRequestPaymentGatewayCreditsQueue,
                NotifyActiveCreditsAsync, MethodBase.GetCurrentMethod(), _credinetAppSettings.QueueMaxConcurrentCalls);
        }

        /// <summary>
        /// Notify active credits async
        /// </summary>
        /// <param name="activeCreditCommand"></param>
        /// <returns></returns>
        private async Task NotifyActiveCreditsAsync(Command<ActiveCreditsRequest> activeCreditCommand) =>
         await HandleRequestAsync(async (request) =>
         {
             await _paymentService.ActiveCreditsNotifyAsync(request);
         },
         MethodBase.GetCurrentMethod(),
         activeCreditCommand.data?.TransactionId,
         activeCreditCommand,
         registerRequest: true);
    }
}