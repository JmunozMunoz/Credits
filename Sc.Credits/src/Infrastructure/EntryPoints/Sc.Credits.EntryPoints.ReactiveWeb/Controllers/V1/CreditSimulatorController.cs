using Microsoft.AspNetCore.Mvc;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Simulator;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.EntryPoints.ReactiveWeb.Controllers.Base;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sc.Credits.EntryPoints.ReactiveWeb.Controllers.V1
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/[controller]/[action]")]
    public class CreditSimulatorController : AppControllerBase<CreditSimulatorController>
    {
        private readonly ISimulatorService _simulatorService;
        private readonly CredinetAppSettings _credinetAppSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreditSimulatorController"/> class.
        /// </summary>
        /// <param name="creditService">The credit service.</param>
        /// <param name="commons">The commons.</param>
        /// <param name="loggerService">The logger service.</param>
        public CreditSimulatorController(ISimulatorService creditService,
                                ICommons commons,
                                ILoggerService<CreditSimulatorController> loggerService)
    : base(commons.CredinetAppSettings, loggerService)
        {
            _simulatorService = creditService;
            _credinetAppSettings = commons.CredinetAppSettings;
        }

        /// <summary>
        /// Gets the credit details.
        /// </summary>
        /// <param name="creditValue">The credit value.</param>
        /// <param name="frequency">The frequency.</param>
        /// <param name="months">The months.</param>
        /// <param name="storeId">The store identifier.</param>
        /// <returns>IActionResult with the credit operations applied</returns>
        [HttpGet]
        public async Task<IActionResult> GetCreditDetails(decimal creditValue, int frequency, int months, string storeId) =>
    await HandleRequestAsync(async () => await _simulatorService.GetCreditDetailsAsync(new RequiredInitialValuesForCreditSimulation() { creditValue = creditValue, months = months, storeId = storeId, frequency = frequency }),
        storeId);

        /// <summary>
        /// Gets the needed data for a simulations
        /// </summary>
        /// <param name="creditValue">The credit value.</param>
        /// <param name="storeId">The store identifier.</param>
        /// <returns>IActionResult with the operations applied</returns>
        [HttpGet]
        public async Task<IActionResult> GetTotalFeeValue(decimal creditValue, string storeId) =>
    await HandleRequestAsync(async () => await _simulatorService.GetTotalFeeValue(new RequiredInitialValuesForBasicSimulation() { creditValue = creditValue, storeId = storeId }),
        storeId);

        /// <summary>
        /// Independents the simulation.
        /// </summary>
        /// <param name="initialValues">The initial values.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> IndependentSimulation([FromBody] InitialValuesForIndependentSimulation initialValues) =>
            await HandleRequestAsync(async () => await _simulatorService.IndependentSimulation(initialValues),
            initialValues.userEmail);

        /// <summary>
        /// Gets the time limit in months.
        /// </summary>
        /// <param name="creditValue">The credit value.</param>
        /// <param name="storeId">The store identifier.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetTimeLimitInMonthsByValueAndStore(decimal creditValue, string storeId) =>
                await HandleRequestAsync(async () =>
        new
        {
            months = await _simulatorService.GetTimeLimitInMonthsAsync(new LimitMonhsInitialValuesRequest() { creditValue = creditValue, storeId = storeId })
        },
        string.IsNullOrEmpty(storeId) ? storeId : "Generic Store");

        /// <summary>
        /// Gets the store minimum and maximum credit limit.
        /// </summary>
        /// <param name="storeId">The store identifier.</param>
        /// <returns>range of value from storecategory of store</returns>
        [HttpGet]
        public async Task<IActionResult> GetStoreMinAndMaxCreditLimitByStoreId(string storeId) =>
             await HandleRequestAsync(async () => await _simulatorService.GetStoreMinAndMaxCreditLimitByStoreId(storeId),
        storeId);
    }
}