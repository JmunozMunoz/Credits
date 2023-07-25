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
    /// <see cref="ICreditCreateCommandSubscription"/>
    /// </summary>
    public class CreditCreateCommandSubscription
        : SubscriptionBase<CreditCreateCommandSubscription>, ICreditCreateCommandSubscription
    {
        private readonly IDirectAsyncGateway<CreateCreditResponse> _directAsyncCreateCredit;
        private readonly ICreditService _creditService;
        private readonly CredinetAppSettings _credinetAppSettings;

        /// <summary>
        /// New create credit command subscription
        /// </summary>
        /// <param name="creditService"></param>
        /// <param name="directAsyncCreateCredit"></param>
        /// <param name="commons"></param>
        /// <param name="loggerService"></param>
        public CreditCreateCommandSubscription(ICreditService creditService,
            IDirectAsyncGateway<CreateCreditResponse> directAsyncCreateCredit,
            ICommons commons,
            ILoggerService<CreditCreateCommandSubscription> loggerService)
            : base(commons.CredinetAppSettings, loggerService)
        {
            _directAsyncCreateCredit = directAsyncCreateCredit;
            _creditService = creditService;
            _credinetAppSettings = commons.CredinetAppSettings;
        }

        /// <summary>
        /// <see cref="ISubscription.SubscribeAsync"/>
        /// </summary>
        /// <returns></returns>
        public async Task SubscribeAsync()
        {
            await SubscribeOnCommandAsync(_directAsyncCreateCredit, _credinetAppSettings.CreateCreditNotificationQueue,
                NotifyCreditCreationAsync, MethodBase.GetCurrentMethod(), _credinetAppSettings.QueueMaxConcurrentCalls);
        }

        /// <summary>
        /// Notify credit creation
        /// </summary>
        /// <param name="createCreditCommand"></param>
        /// <returns></returns>
        private async Task NotifyCreditCreationAsync(Command<CreateCreditResponse> createCreditCommand) =>
          await HandleRequestAsync(async (request) =>
              {
                  await _creditService.CreditCreationNotifyAsync(request);
              },
          MethodBase.GetCurrentMethod(),
          createCreditCommand.data?.CreditId.ToString(),
          createCreditCommand,
          notifyBusinessException: true,
          registerRequest: true);
    }
}