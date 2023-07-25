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
    public class PayCreditsCommandSubscription
        : SubscriptionBase<PayCreditsCommandSubscription>, IPayCreditsCommandSubscription
    {
        private readonly ICreditPaymentService _paymentService;
        private readonly CredinetAppSettings _credinetAppSettings;
        private readonly IDirectAsyncGateway<PayCreditsRequest> _directAsyncCreditsPayment;

        /// <summary>
        /// Pay credits command subscription
        /// </summary>
        /// <param name="paymentService"></param>
        /// <param name="directAsyncActiveCredit"></param>
        /// <param name="commons"></param>
        /// <param name="loggerService"></param>
        public PayCreditsCommandSubscription(ICreditPaymentService paymentService,
            IDirectAsyncGateway<PayCreditsRequest> directAsyncGateway,
            ICommons commons, ILoggerService<PayCreditsCommandSubscription> loggerService)
            : base(commons.CredinetAppSettings, loggerService)
        {
            _paymentService = paymentService;
            _credinetAppSettings = commons.CredinetAppSettings;
            _directAsyncCreditsPayment = directAsyncGateway;
        }

        /// <summary>
        /// <see cref="ISubscription.SubscribeAsync"/>
        /// </summary>
        /// <returns></returns>

        public async Task SubscribeAsync()
        {
            await SubscribeOnCommandAsync(_directAsyncCreditsPayment, _credinetAppSettings.PayCreditsRequestPaymentGatewayCreditsQueue,
                NotifyCreditsPaymentAsync, MethodBase.GetCurrentMethod(), _credinetAppSettings.QueueMaxConcurrentCalls);
        }

        /// <summary>
        /// Notify credits payment async
        /// </summary>
        /// <param name="activeCreditCommand"></param>
        /// <returns></returns>
        private async Task NotifyCreditsPaymentAsync(Command<PayCreditsRequest> payCreditCommand) =>
         await HandleRequestAsync(async (request) =>
         {
             await _paymentService.PayCreditMultipleAndNotifyAsync(request);
         },
         MethodBase.GetCurrentMethod(),
         payCreditCommand.data?.TransactionId,
         payCreditCommand,
         registerRequest: true);
    }
}