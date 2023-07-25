using Microsoft.AspNetCore.Mvc;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Refinancings;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Refinancings;
using Sc.Credits.EntryPoints.ReactiveWeb.Controllers.Base;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.ObjectsUtils;
using System.Threading.Tasks;

namespace Sc.Credits.EntryPoints.ReactiveWeb.Controllers.V1
{
    /// <summary>
    /// Refinancing controller
    /// </summary>
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/[controller]/[action]")]
    public class RefinancingController : AppControllerBase<RefinancingController>
    {
        private readonly IRefinancingService _refinancingService;
        private readonly CredinetAppSettings _credinetAppSettings;

        /// <summary>
        /// New refinancing controller
        /// </summary>
        /// <param name="refinancingService"></param>
        /// <param name="commons"></param>
        /// <param name="loggerService"></param>
        public RefinancingController(IRefinancingService refinancingService,
            ICommons commons,
            ILoggerService<RefinancingController> loggerService)
            : base(commons.CredinetAppSettings, loggerService)
        {
            _refinancingService = refinancingService;
            _credinetAppSettings = commons.CredinetAppSettings;
        }

        /// <summary>
        /// Get customer credits
        /// </summary>
        /// <param name="customerCreditsRequest"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCustomerCredits([FromQuery] CustomerCreditsRequest customerCreditsRequest) =>
            await HandleRequestAsync(async () => await _refinancingService.GetCustomerCreditsAsync(customerCreditsRequest),
                customerCreditsRequest?.IdDocument);

        /// <summary>
        /// Calculate fees
        /// </summary>
        /// <param name="calculateFeesRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CalculateFees([FromBody] CalculateFeesRequest calculateFeesRequest) =>
            await HandleRequestAsync(async () => await _refinancingService.CalculateFeesAsync(calculateFeesRequest),
                calculateFeesRequest?.IdDocument);

        /// <summary>
        /// Create credit
        /// </summary>
        /// <param name="refinancingCreditRequest"></param>
        /// <param name="SCLocation"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateCredit([FromBody] RefinancingCreditRequest refinancingCreditRequest, [FromHeader] string SCLocation) =>
                await HandleRequestAndLogEventAsync(async () =>
                {
                    refinancingCreditRequest.Location = SCLocation;
                    return await _refinancingService.CreateCreditAsync(refinancingCreditRequest);
                },
                refinancingCreditRequest?.IdDocument);

        /// <summary>
        /// Get refinancing token
        /// </summary>
        /// <param name="generateTokenRequest"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetRefinancingToken([FromQuery] GenerateTokenRequest generateTokenRequest) =>
            await HandleRequestAsync(async () =>
            {
                TokenResponse tokenResponse = await _refinancingService.GenerateTokenAsync(generateTokenRequest);
                tokenResponse.SetupVisibility(_credinetAppSettings);
                return tokenResponse;
            },
            generateTokenRequest?.IdDocument);
    }
}