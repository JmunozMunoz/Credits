using org.reactivecommons.api;
using org.reactivecommons.api.domain;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Credits;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.EntryPoints.ServicesBus.Base;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Sc.Credits.EntryPoints.ServicesBus.Credits
{
    /// <summary>
    /// <see cref="ICreditQueryCommandSubscription"/>
    /// </summary>
    public class CreditQueryCommandSubscription
        : SubscriptionBase<CreditQueryCommandSubscription>, ICreditQueryCommandSubscription
    {
        private readonly IDirectAsyncGateway<CalculatedQuery> _directAsyncCreditsResponse;
        private readonly ICreditPaymentService _paymentService;
        private readonly CredinetAppSettings _credinetAppSettings;

        /// <summary>
        /// New query credit command subscription
        /// </summary>
        /// <param name="paymentService"></param>
        /// <param name="directAsyncCreditsResponse"></param>
        /// <param name="commons"></param>
        /// <param name="loggerService"></param>
        public CreditQueryCommandSubscription(ICreditPaymentService paymentService,
            IDirectAsyncGateway<CalculatedQuery> directAsyncCreditsResponse,
            ICommons commons,
            ILoggerService<CreditQueryCommandSubscription> loggerService)
            : base(commons.CredinetAppSettings, loggerService)
        {
            _directAsyncCreditsResponse = directAsyncCreditsResponse;
            _paymentService = paymentService;
            _credinetAppSettings = commons.CredinetAppSettings;
        }

        /// <summary>
        /// <see cref="ISubscription.SubscribeAsync"/>
        /// </summary>
        /// <returns></returns>
        public async Task SubscribeAsync()
        {
            await SubscribeOnResponseReplyAsync(_directAsyncCreditsResponse, "QueryDataCalculateCredit",
                _credinetAppSettings.QueryCredits, QueryDataCalculateCreditsResponseAsync,
                MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Query data calculate credits
        /// </summary>
        /// <param name="creditQuery"></param>
        /// <returns></returns>
        private async Task<CalculatedQuery> QueryDataCalculateCreditsResponseAsync(AsyncQuery<CalculatedQuery> creditQuery) =>
          await HandleRequestAsync(async (request) =>
          {
              CalculatedQuery dataResponse = await _paymentService.GetDataCalculateCreditAsync(Guid.Parse(request.CreditId), DateTime.Now);

              var response = new CalculatedQuery()
              {
                  CreditId = dataResponse.CreditId,
                  MinimumPayment = dataResponse.MinimumPayment,
                  TotalPayment = dataResponse.TotalPayment,
                  ArrearsDays = dataResponse.ArrearsDays,
                  HasArrears = dataResponse.HasArrears,
                  ArrearsPayment = dataResponse.ArrearsPayment,
                  IsCalculate = true,
                  MessageException = ""
              };

              return response;
          },
          MethodBase.GetCurrentMethod(),
          creditQuery.QueryData?.CreditId.ToString(),
          creditQuery);
    }
}