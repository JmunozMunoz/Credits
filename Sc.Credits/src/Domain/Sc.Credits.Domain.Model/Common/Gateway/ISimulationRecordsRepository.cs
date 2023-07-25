using Sc.Credits.Domain.Model.Credits;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Model.Common.Gateway
{
    /// <summary>
    /// Records of Simulation Contract
    /// </summary>
    public interface ISimulationRecordsRepository
    {
        /// <summary>
        /// Saves the record request asynchronous.
        /// </summary>
        /// <param name="initialValues">The initial values.</param>
        /// <returns></returns>
        Task SaveRecordRequestAsync(InitialValuesForIndependentSimulation initialValues);
    }
}
