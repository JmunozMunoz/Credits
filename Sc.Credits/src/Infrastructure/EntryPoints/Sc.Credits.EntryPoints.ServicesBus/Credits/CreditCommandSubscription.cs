using org.reactivecommons.api;
using org.reactivecommons.api.domain;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Credits;
using Sc.Credits.EntryPoints.ServicesBus.Base;
using Sc.Credits.EntryPoints.ServicesBus.Model;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Sc.Credits.EntryPoints.ServicesBus.Credits
{
    /// <summary>
    /// <see cref="ICreditCommandSubscription"/>
    /// </summary>
    public class CreditCommandSubscription
        : SubscriptionBase<CreditCommandSubscription>, ICreditCommandSubscription
    {
        private readonly IDirectAsyncGateway<ChargesUpdatedPaymentPlanValueRequest> _directAsyncChargesUpdatedPaymentPlan;
        private readonly ICreditService _creditService;
        private readonly CredinetAppSettings _credinetAppSettings;

        /// <summary>
        /// New credit command subscription
        /// </summary>
        /// <param name="creditService"></param>
        /// <param name="directAsyncChargesUpdatedPaymentPlan"></param>
        /// <param name="commons"></param>
        /// <param name="loggerService"></param>
        public CreditCommandSubscription(ICreditService creditService,
            IDirectAsyncGateway<ChargesUpdatedPaymentPlanValueRequest> directAsyncChargesUpdatedPaymentPlan,
            ICommons commons,
            ILoggerService<CreditCommandSubscription> loggerService)
            : base(commons.CredinetAppSettings, loggerService)
        {
            _directAsyncChargesUpdatedPaymentPlan = directAsyncChargesUpdatedPaymentPlan;
            _creditService = creditService;
            _credinetAppSettings = commons.CredinetAppSettings;
        }

        /// <summary>
        /// <see cref="ISubscription.SubscribeAsync"/>
        /// </summary>
        /// <returns></returns>
        public async Task SubscribeAsync()
        {
            await SubscribeOnCommandAsync(_directAsyncChargesUpdatedPaymentPlan, _credinetAppSettings.CreditExtraValuesQueue,
                UpdateChargesPaymentPlanAsync, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Update charges and payment plan value
        /// </summary>
        /// <param name="updateChargesPaymentPlanCommand"></param>
        /// <returns></returns>
        private async Task UpdateChargesPaymentPlanAsync(Command<ChargesUpdatedPaymentPlanValueRequest> updateChargesPaymentPlanCommand) =>
          await HandleRequestAsync(async (request) =>
              {
                  await _creditService.UpdateChargesPaymentPlanValueAsync(new Guid(request.CreditId), request.ChargeValue ?? 0,
                      request.HasArrearsCharge, request.ArrearsCharge ?? 0, request.UpdatedPaymentPlanValue ?? 0);
              },
          MethodBase.GetCurrentMethod(),
          updateChargesPaymentPlanCommand.data?.CreditId?.ToString(),
          updateChargesPaymentPlanCommand);
    }
}