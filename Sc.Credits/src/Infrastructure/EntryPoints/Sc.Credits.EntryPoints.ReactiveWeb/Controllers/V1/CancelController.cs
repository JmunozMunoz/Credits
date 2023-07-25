using Microsoft.AspNetCore.Mvc;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Credits;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.EntryPoints.ReactiveWeb.Controllers.Base;
using Sc.Credits.Helpers.Commons.Logging;
using System;
using System.Threading.Tasks;

namespace Sc.Credits.EntryPoints.ReactiveWeb.Controllers.V1
{
    /// <summary>
    /// Cancel controller
    /// </summary>
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/[controller]/[action]")]
    public class CancelController : AppControllerBase<CancelController>
    {
        private readonly ICancelCreditService _cancelCreditService;
        private readonly ICancelPaymentService _cancelPaymentService;
        private readonly IPartialCancelationCreditService _partialCancelationCreditService;

        /// <summary>
        /// Cancel controller
        /// </summary>
        /// <param name="cancelCreditService"></param>
        /// <param name="cancelPaymentService"></param>
        /// <param name="commons"></param>
        /// <param name="partialCancelationCreditService"></param>
        /// <param name="loggerService"></param>
        public CancelController(ICancelCreditService cancelCreditService,
            ICancelPaymentService cancelPaymentService,
            ICommons commons,
            IPartialCancelationCreditService partialCancelationCreditService,
            ILoggerService<CancelController> loggerService)
            : base(commons.CredinetAppSettings, loggerService)
        {
            _cancelCreditService = cancelCreditService;
            _cancelPaymentService = cancelPaymentService;
            _partialCancelationCreditService = partialCancelationCreditService;
        }

        /// <summary>
        /// Get active and pending cancellations credits
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        /// <param name="vendorId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetActiveAndPendingCancellationCredits(string typeDocument, string idDocument, string vendorId) =>
            await HandleRequestAsync(async () =>
                    await _cancelCreditService.GetActiveAndPendingCancellationAsync(typeDocument, idDocument, vendorId),
                idDocument);

        /// <summary>
        /// Cancel credit
        /// </summary>
        /// <param name="cancelCredit"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CancelCredit([FromBody] CancelCredit cancelCredit) =>
            await HandleRequestAndLogEventAsync(async () =>
            {
                await _cancelCreditService.CancelAsync(cancelCredit);
                return true;
            },
            cancelCredit.CreditId.ToString());

        /// <summary>
        /// Get list credits pending cancellation
        /// </summary>
        /// <param name="vendorId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="valuePage"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetListCreditsPendingCancellation(string vendorId, int pageNumber, int valuePage, bool count = true) =>
            await HandleRequestAsync(async () => await _cancelCreditService.GetPendingsAsync(vendorId, pageNumber, valuePage, count), vendorId);

        /// <summary>
        /// Cancel payments
        /// </summary>
        /// <param name="cancelPayments"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CancelPayments([FromBody] CancelPayments cancelPayments) =>
            await HandleRequestAndLogEventAsync(async () =>
            {
                await _cancelPaymentService.CancelAsync(cancelPayments);
                return true;
            },
            cancelPayments.CreditId.ToString());

        /// <summary>
        /// Get list payments pending cancellation
        /// </summary>
        /// <param name="vendorId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="valuePage"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetListPaymentsPendingCancellation(string vendorId, int pageNumber, int valuePage, bool count = true) =>
            await HandleRequestAsync(async () => await _cancelPaymentService.GetPendingsAsync(vendorId, pageNumber, valuePage, count), vendorId);

        /// <summary>
        /// Request cancel payment
        /// </summary>
        /// <param name="cancelPaymentRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RequestCancelPayment([FromBody] CancelPaymentRequest cancelPaymentRequest) =>
                await HandleRequestAndLogEventAsync(async () =>
                {
                    await _cancelPaymentService.RequestAsync(cancelPaymentRequest);
                    return true;
                },
                cancelPaymentRequest.PaymentId.ToString());

        /// <summary>
        /// Request cancel credit
        /// </summary>
        /// <param name="cancelCreditRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RequestCancelCredit([FromBody] CancelCreditRequest cancelCreditRequest) =>
                await HandleRequestAndLogEventAsync(async () =>
                {
                    await _cancelCreditService.RequestAsync(cancelCreditRequest);
                    return true;
                },
                cancelCreditRequest.CreditId.ToString());

        /// <summary>
        /// Reject request cancel payment
        /// </summary>
        /// <param name="cancelPaymentsRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RejectRequestCancelPayments([FromBody] CancelPayments cancelPaymentsRequest) =>
                await HandleRequestAndLogEventAsync(async () =>
                {
                    await _cancelPaymentService.RejectAsync(cancelPaymentsRequest);
                    return true;
                },
                cancelPaymentsRequest.CreditId.ToString());

        /// <summary>
        /// Reject request cancel credit
        /// </summary>
        /// <param name="cancelCreditRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RejectRequestCancelCredit([FromBody] CancelCredit cancelCreditRequest) =>
                await HandleRequestAndLogEventAsync(async () =>
                {
                    await _cancelCreditService.RejectAsync(cancelCreditRequest);
                    return true;
                },
                cancelCreditRequest.CreditId.ToString());

        /// <summary>
        /// Get active and pending cancellation payments
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        /// <param name="storeId"></param>
        [HttpGet]
        public async Task<IActionResult> GetActiveAndPendingCancellationPayments(string typeDocument, string idDocument, string storeId) =>
            await HandleRequestAsync(async () =>
                await _cancelPaymentService.GetActiveAndPendingCancellationAsync(typeDocument, idDocument, storeId), idDocument);

        /// <summary>
        /// Validation to partially cancel a credit
        /// </summary>
        /// <param name="partialCancellationRequest"></param>
        [HttpPost]
        public async Task<IActionResult> ValidationToPartiallyCancelACredit([FromBody] PartialCancellationRequest partialCancellationRequest) =>
            await HandleRequestAsync(async () =>
                    await _partialCancelationCreditService.GetValidationToPartiallyCancelACreditAsync(partialCancellationRequest), partialCancellationRequest.CreditId.ToString());
    }
}