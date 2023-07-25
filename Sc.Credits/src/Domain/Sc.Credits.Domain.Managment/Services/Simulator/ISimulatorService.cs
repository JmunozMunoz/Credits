using Sc.Credits.Domain.Model.Credits;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Managment.Services.Simulator
{
    public interface ISimulatorService
    {
        /// <summary>
        /// Gets the credit details asynchronous.
        /// </summary>
        /// <param name="RequiredValues">The required values.</param>
        /// <returns>CreditDetailResponse with all the operations and details made</returns>
        Task<CreditDetailResponse> GetCreditDetailsAsync(RequiredInitialValuesForCreditSimulation RequiredValues);

        /// <summary>
        /// Gets the fee value with al the operations made
        /// </summary>
        /// <param name="requiredValues">Basic requiered values to calculate fee value</param>
        /// <returns>SimulationDetailsResponse with the needed details</returns>
        Task<SimulationDetailsResponse> GetTotalFeeValue(RequiredInitialValuesForBasicSimulation requiredValues);

        /// <summary>
        /// Independents the simulation.
        /// </summary>
        /// <param name="requiredValues">The required values.</param>
        /// <returns></returns>
        Task<SimulationDetailsResponse> IndependentSimulation(InitialValuesForIndependentSimulation requiredValues);

        /// <summary>
        /// Gets the time limit in months asynchronous.
        /// </summary>
        /// <param name="initialValues">The initial values.</param>
        /// <returns>Maximum number of months credit can be deferred</returns>
        Task<int> GetTimeLimitInMonthsAsync(LimitMonhsInitialValuesRequest initialValues);

        /// <summary>
        /// Gets the store minimum and maximum credit limit.
        /// </summary>
        /// <param name="storeId">The store identifier.</param>
        /// <returns>range of value from storecategory of store</returns>
        Task<StoreCategoryRange> GetStoreMinAndMaxCreditLimitByStoreId(string storeId);
    }
}
