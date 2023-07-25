using credinet.exception.middleware.enums;
using Microsoft.AspNetCore.Mvc;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Credits;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.EntryPoints.ReactiveWeb.Controllers.Base;
using Sc.Credits.Helpers.Commons.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sc.Credits.EntryPoints.ReactiveWeb.Controllers.V1
{
    /// <summary>
    /// Credit controller
    /// </summary>
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/[controller]/[action]")]
    public class PaymentController : AppControllerBase<PaymentController>
    {
        private readonly ICreditPaymentService _creditPaymentService;

        /// <summary>
        /// New payment controller
        /// </summary>
        /// <param name="creditPaymentService"></param>
        /// <param name="commons"></param>
        /// <param name="loggerService"></param>
        public PaymentController(ICreditPaymentService creditPaymentService,
            ICommons commons,
            ILoggerService<PaymentController> loggerService)
            : base(commons.CredinetAppSettings, loggerService)
        {
            _creditPaymentService = creditPaymentService;
        }

        /// <summary>
        /// Pay credit
        /// </summary>
        /// <param name="paymentCreditRequest"></param>
        /// <param name="SCLocation"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PayCredit([FromBody] PaymentCreditRequest paymentCreditRequest, [FromHeader] string SCLocation) =>
            await HandleRequestAndLogEventAsync(async () =>
            {
                paymentCreditRequest.Location = SCLocation;

                PaymentCreditResponse paymentCreditResponse = await _creditPaymentService.PayCreditAsync(paymentCreditRequest, notify: true);

                return paymentCreditResponse;
            },
            paymentCreditRequest.CreditId.ToString(),
            notifyBusinessException: true,
            exceptionCode: BusinessResponse.FailedPayment);

        /// <summary>
        /// Pay credit bank
        /// </summary>
        /// <param name="paymentCreditRequest"></param>
        /// <param name="SCLocation"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PayCreditBank([FromBody] PaymentCreditRequestComplete paymentCreditRequest, [FromHeader] string SCLocation) =>
            await HandleRequestAndLogEventAsync(async () =>
            {
                paymentCreditRequest.Location = SCLocation;

                PaymentCreditResponse paymentCreditResponse = await _creditPaymentService.PayCreditAsync(paymentCreditRequest, notify: true);
                return paymentCreditResponse;
            },
            paymentCreditRequest.CreditId.ToString(),
            exceptionCode: BusinessResponse.FailedPayment);

        /// <summary>
        /// Add fee payment
        /// </summary>
        /// <param name="paymentCreditMultipleRequest"></param>
        /// <param name="SCLocation"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PayCreditMultiple([FromBody] PaymentCreditMultipleRequest paymentCreditMultipleRequest, [FromHeader] string SCLocation) =>
            await HandleRequestAndLogEventAsync(async () =>
            {
                paymentCreditMultipleRequest.Location = SCLocation;

                List<PaymentCreditResponse> paymentCreditResponses = await _creditPaymentService.PayCreditMultipleAsync(PayCreditsRequest.FromMultiple(paymentCreditMultipleRequest));
                return paymentCreditResponses;
            },
            logId: string.Join(",", paymentCreditMultipleRequest.CreditPaymentDetails.Select(d => d.CreditId)),
            exceptionCode: BusinessResponse.FailedPayment);

        /// <summary>
        /// Get current amortization schedule
        /// </summary>
        /// <param name="currentAmortizationScheduleRequest"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCurrentAmortizationSchedule([FromQuery] CurrentAmortizationScheduleRequest currentAmortizationScheduleRequest) =>
            await HandleRequestAsync(async () =>
                await _creditPaymentService.GetCurrentAmortizationScheduleAsync(currentAmortizationScheduleRequest),
                currentAmortizationScheduleRequest.Balance.ToString());

        /// <summary>
        /// Get current payment schedule
        /// </summary>
        /// <param name="currentPaymentScheduleRequest"></param>
        /// <param name="calculationDate"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCurrentPaymentSchedule([FromQuery] CurrentPaymentScheduleRequest currentPaymentScheduleRequest, DateTime? calculationDate = null) =>
            await HandleRequestAsync(async () =>
                await _creditPaymentService.GetCurrentPaymentScheduleAsync(currentPaymentScheduleRequest, calculationDate?.Date ?? DateTime.Today),
                currentPaymentScheduleRequest.Balance.ToString());

        /// <summary>
        /// Get payment fees
        /// </summary>
        /// <param name="creditId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetPaymentFees(Guid creditId) =>
            await HandleRequestAsync(async () => await _creditPaymentService.GetPaymentFeesAsync(creditId), creditId.ToString());

        /// <summary>
        /// Get payment templates
        /// </summary>
        /// <param name="paymentsId"></param>
        /// <param name="reprint"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetPaymentTemplates(List<Guid> paymentsId, bool reprint) =>
            await HandleRequestAsync(async () => await _creditPaymentService.GetPaymentTemplatesAsync(paymentsId, reprint),
                string.Join(",", paymentsId));

        /// <summary>
        /// Get customer payment history
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="documentType"></param>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCustomerPaymentHistory(string storeId, string documentType, string idDocument) =>
            await HandleRequestAsync(async () => await _creditPaymentService.GetCustomerPaymentHistoryAsync(storeId, documentType, idDocument),
                idDocument);

        /// <summary>
        /// Makes bussines validation for external payments
        /// </summary>
        /// <param name="calculationDate"></param>
        /// <param name="creditId"></param>
        /// <param name="totalValuePaid"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ValidateRulesForExternalPayments(DateTime calculationDate, Guid creditId, decimal totalValuePaid) =>
            await HandleRequestAsync(async () => await _creditPaymentService.ValidateRulesForExternalPayments(calculationDate, creditId, totalValuePaid)
            , creditId.ToString());


        /// <summary>
        /// A simulation of payment credit process
        /// </summary>
        /// <param name="paymentCreditRequest"></param>
        /// <param name="SCLocation"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SimulatePaymentCredit([FromBody] PaymentCreditRequest paymentCreditRequest, [FromHeader] string SCLocation) =>
            await HandleRequestAsync(async () =>
            {
                paymentCreditRequest.Location = SCLocation;

                PaymentCreditResponse paymentCreditResponse = await _creditPaymentService.PayCreditAsync(paymentCreditRequest, notify: true, simulation: true);

                return paymentCreditResponse;
            },
            paymentCreditRequest.CreditId.ToString(),
            notifyBusinessException: true,
            exceptionCode: BusinessResponse.FailedPayment);
    }
}