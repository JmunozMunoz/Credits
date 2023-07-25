using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Stores;
using Sc.Credits.Domain.Model.Common.Gateway;
using Sc.Credits.Domain.Model.Credits;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Managment.Services.Credits
{
    public class SimulatorCommonService : ISimulatorCommonService
    {
        private readonly ISimulationRecordsRepository _recordsRepository;

        /// <summary>
        /// <see cref="ICreditCommonsService.Commons"/>
        /// </summary>
        public ICommons Commons { get; }
        /// <summary>
        /// <see cref="ICreditCommonsService.StoreService"/>
        /// </summary>
        public IStoreService StoreService { get; }

        public SimulatorCommonService(ICommons commons,
    IStoreService storeService,
    ISimulationRecordsRepository recordsRepository)
        {
            Commons = commons;
            StoreService = storeService;
            _recordsRepository = recordsRepository;
        }

        /// <summary>
        /// Saves the record request asynchronous.
        /// </summary>
        /// <param name="initialValues">The initial values.</param>
        public async Task SaveRecordRequestAsync(InitialValuesForIndependentSimulation initialValues) {
            await _recordsRepository.SaveRecordRequestAsync(initialValues);
        }
    }
}
