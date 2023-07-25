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
    /// <see cref="ICreditCustomerCommandSubscription"/>
    /// </summary>
    public class CreditCustomerCommandSubscription
        : SubscriptionBase<CreditCustomerCommandSubscription>, ICreditCustomerCommandSubscription
    {
        private readonly IDirectAsyncGateway<CreditCustomerMigrationHistoryRequest> _directAsyncCreditCustomerMigrationHistory;
        private readonly ICreditCustomerService _creditCustomerService;
        private readonly CredinetAppSettings _credinetAppSettings;

        /// <summary>
        /// New credit customer command subscription
        /// </summary>
        /// <param name="directAsyncCreditCustomerMigrationHistory"></param>
        /// <param name="creditCustomerService"></param>
        /// <param name="commons"></param>
        /// <param name="loggerService"></param>
        public CreditCustomerCommandSubscription(IDirectAsyncGateway<CreditCustomerMigrationHistoryRequest> directAsyncCreditCustomerMigrationHistory,
            ICreditCustomerService creditCustomerService,
            ICommons commons,
            ILoggerService<CreditCustomerCommandSubscription> loggerService)
            : base(commons.CredinetAppSettings, loggerService)
        {
            _directAsyncCreditCustomerMigrationHistory = directAsyncCreditCustomerMigrationHistory;
            _creditCustomerService = creditCustomerService;
            _credinetAppSettings = commons.CredinetAppSettings;
        }

        /// <summary>
        /// <see cref="ISubscription.SubscribeAsync"/>
        /// </summary>
        /// <returns></returns>
        public async Task SubscribeAsync()
        {
            await SubscribeOnCommandAsync(_directAsyncCreditCustomerMigrationHistory, _credinetAppSettings.CreditCustomerMigrationHistoryQueue,
                SendCustomerMigrationHistory, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Send customer migration history
        /// </summary>
        /// <param name="theEvent"></param>
        /// <returns></returns>
        private async Task SendCustomerMigrationHistory(Command<CreditCustomerMigrationHistoryRequest> creditCustomerMigrationHistoryCommand) =>
            await HandleRequestAsync(async (request) =>
                {
                    await _creditCustomerService.SendMigrationHistoryAsync(request);
                },
            MethodBase.GetCurrentMethod(),
            creditCustomerMigrationHistoryCommand.data?.IdDocument.ToString(),
            creditCustomerMigrationHistoryCommand);
    }
}