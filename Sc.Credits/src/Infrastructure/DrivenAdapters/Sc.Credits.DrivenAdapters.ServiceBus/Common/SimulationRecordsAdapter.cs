using org.reactivecommons.api;
using org.reactivecommons.api.domain;
using Sc.Credits.Domain.Model.Common.Gateway;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Helpers.Commons.Settings;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sc.Credits.DrivenAdapters.ServiceBus.Common
{
    public class SimulationRecordsAdapter : ISimulationRecordsRepository
    {
        private readonly IDirectAsyncGateway<dynamic> _directAsyncGateway;
        private readonly CredinetAppSettings _credinetAppSettings;

        public SimulationRecordsAdapter(IDirectAsyncGateway<dynamic> directAsyncGateway, ISettings<CredinetAppSettings> appSettings)
        {
            _directAsyncGateway = directAsyncGateway;
            _credinetAppSettings = appSettings.Get();
        }

        /// <summary>
        /// Saves the record request asynchronous.
        /// </summary>
        /// <param name="initialValues">The initial values.</param>
        public async Task SaveRecordRequestAsync(InitialValuesForIndependentSimulation initialValues) {
            Command<dynamic> command = new Command<dynamic>("SaveSimulationRecords", initialValues.storeId, initialValues);

            await _directAsyncGateway.SendCommand(_credinetAppSettings.SimulationRecordsQueue, command);
        }
    }
}
