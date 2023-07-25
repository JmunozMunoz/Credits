using Microsoft.AspNetCore.Mvc;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Credits;
using Sc.Credits.Domain.Managment.Services.Customers;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Customers.Queries.Reading;
using Sc.Credits.EntryPoints.ReactiveWeb.Controllers.Base;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sc.Credits.EntryPoints.ReactiveWeb.Controllers.V2
{
    /// <summary>
    /// Credit controller
    /// </summary>
    [Produces("application/json")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class CreditController : AppControllerBase<CreditController>
    {
        private readonly ICreditService _creditService;
        private readonly ICustomerService _customerService;
        private readonly ICreditPaymentService _creditPaymentService;
        private readonly CredinetAppSettings _credinetAppSettings;

        /// <summary>
        /// New credit controller
        /// </summary>
        /// <param name="creditService"></param>
        /// <param name="customerService"></param>
        /// <param name="creditPaymentService"></param>
        /// <param name="commons"></param>
        /// <param name="loggerService"></param>
        public CreditController(ICreditService creditService,
            ICustomerService customerService,
            ICreditPaymentService creditPaymentService,
            ICommons commons,
            ILoggerService<CreditController> loggerService)
            : base(commons.CredinetAppSettings, loggerService)
        {
            _creditService = creditService;
            _customerService = customerService;
            _creditPaymentService = creditPaymentService;
            _credinetAppSettings = commons.CredinetAppSettings;
        }

        /// <summary>
        /// Get time limit in months
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="typeDocument"></param>
        /// <param name="creditValue"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetTimeLimitInMonths(string idDocument, string typeDocument, decimal creditValue, string storeId) =>
            await HandleRequestAsync(async () =>
                new
                {
                    months = await _creditService.GetTimeLimitInMonthsAsync(idDocument, typeDocument, creditValue, storeId)
                },
                idDocument);

        /// <summary>
        /// Get credit details
        /// </summary>
        /// <param name="creditValue"></param>
        /// <param name="frequency"></param>
        /// <param name="months"></param>
        /// <param name="storeId"></param>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCreditDetails(decimal creditValue, int frequency, int months, string storeId, string typeDocument, string idDocument) =>
            await HandleRequestAsync(async () => await _creditService.GetCreditDetailsAsync(
                new RequiredInitialValuesForCreditDetail() { creditValue = creditValue, frequency = frequency, storeId = storeId, months = months, typeDocument = typeDocument, idDocument = idDocument }),
                idDocument);

        /// <summary>
        /// Get customer credit limit
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        /// <param name="vendorId"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("GetCreditLimitClient")]
        public async Task<IActionResult> GetCustomerCreditLimit(string typeDocument, string idDocument, string vendorId) =>
            await HandleRequestAsync(async () => await _creditService.GetCustomerCreditLimitAsync(typeDocument, idDocument, vendorId),
                idDocument);

        /// <summary>
        /// Get customer credit limit validate
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        /// <param name="vendorId"></param>
        /// <param name="creditValue"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCustomerCreditLimitValidate(string typeDocument, string idDocument, string vendorId, decimal creditValue) =>
            await HandleRequestAsync(async () => await _creditService.GetCustomerCreditLimitAsync(typeDocument, idDocument, vendorId, creditValue),
                idDocument);

        /// <summary>
        /// Create credit
        /// </summary>
        /// <param name="createCreditRequest"></param>
        /// <param name="SCLocation"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCreditRequest createCreditRequest, [FromHeader] string SCLocation) =>
            await HandleRequestAndLogEventAsync(async () =>
            {
                createCreditRequest.Location = SCLocation;

                return await _creditService.CreateAsync(createCreditRequest);
            },
            createCreditRequest.IdDocument);

        /// <summary>
        /// Get original amortization schedule
        /// </summary>
        /// <param name="amortizationScheduleRequest"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetOriginalAmortizationSchedule([FromQuery] AmortizationScheduleRequest amortizationScheduleRequest) =>
            await HandleRequestAsync(async () => await _creditService.GetOriginalAmortizationScheduleAsync(amortizationScheduleRequest), amortizationScheduleRequest.CreditValue.ToString());

        /// <summary>
        /// Update credit extra fields
        /// </summary>
        /// <param name="udateCreditExtraFieldsRequest"></param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<IActionResult> UpdateCreditExtraFields([FromBody] UpdateCreditExtraFieldsRequest udateCreditExtraFieldsRequest) =>
            await HandleRequestAndLogEventAsync(async () =>
            {
                await _creditService.UpdateExtraFieldsAsync(udateCreditExtraFieldsRequest);
                return true;
            },
                udateCreditExtraFieldsRequest.CreditId.ToString());

        /// <summary>
        /// Generate token
        /// </summary>
        /// <param name="generateTokenRequest"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCreditToken([FromQuery] GenerateTokenRequest generateTokenRequest) =>
            await HandleRequestAsync(async () =>
            {
                TokenResponse tokenResponse = await _creditService.GenerateTokenAsync(generateTokenRequest);
                tokenResponse.SetupVisibility(_credinetAppSettings);
                return tokenResponse;
            },
            generateTokenRequest?.IdDocument);

        /// <summary>
        /// Credit token call request
        /// </summary>
        /// <param name="creditTokenCallRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreditTokenCallRequest([FromBody] CreditTokenCallRequest creditTokenCallRequest) =>
            await HandleRequestAsync(async () => await _creditService.TokenCallRequestAsync(creditTokenCallRequest), creditTokenCallRequest.IdDocument);

        /// <summary>
        /// Get customer by idDocument and typeDocument
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCustomerByIdDocument(string typeDocument, string idDocument) =>
            await HandleRequestAsync(async () =>
            {
                await _customerService.GetActiveAsync(idDocument, typeDocument, CustomerReadingFields.CreditLimit);

                return true;
            },
            idDocument);

        /// <summary>
        /// Get active credits
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetActiveCredits(string typeDocument, string idDocument, string storeId) =>
            await HandleRequestAsync(async () =>
                    await _creditPaymentService.GetActiveCreditsAsync(typeDocument, idDocument, storeId, DateTime.Today),
                idDocument);

        /// <summary>
        /// Get List Credit Customer
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        /// <param name="storeId"></param>
        /// <param name="calculationDate"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetListCreditCustomer(string typeDocument, string idDocument, string storeId, DateTime calculationDate) =>
            await HandleRequestAsync(async () =>
                    await _creditPaymentService.GetActiveCreditsAsync(typeDocument, idDocument, storeId, calculationDate),
                idDocument);

        /// <summary>
        /// Get promissory note info
        /// </summary>
        /// <param name="creditId"></param>
        /// <param name="reprint"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetPromissoryNoteInfo(Guid creditId, bool reprint) =>
            await HandleRequestAsync(async () => await _creditService.GetPromissoryNoteInfoAsync(creditId, reprint), creditId.ToString());

        /// <summary>
        /// Get Paid Credit Certificate
        /// </summary>
        /// <param name="creditIds"></param>
        /// <param name="reprint"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetPaidCreditCertificateTemplate(List<Guid> creditIds, bool reprint) =>
            await HandleRequestAsync(async () => await _creditService.GetPaidCreditCertificateTemplatesAsync(creditIds, reprint),
                string.Join(",", creditIds));

        /// <summary>
        /// Get customer credit history
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="documentType"></param>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCustomerCreditHistory(string storeId, string documentType, string idDocument) =>
            await HandleRequestAsync(async () => await _creditService.GetCustomerCreditHistoryAsync(storeId, documentType, idDocument), idDocument);

        /// <summary>
        /// Reject credit limit
        /// </summary>
        /// <param name="rejectCreditLimitRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RejectCreditLimit([FromBody] RejectCreditLimitRequest rejectCreditLimitRequest) =>
            await HandleRequestAndLogEventAsync(async () =>
            {
                return await _customerService.RejectCreditLimit(rejectCreditLimitRequest.IdDocument,
                    rejectCreditLimitRequest.DocumentType, rejectCreditLimitRequest.UserName, rejectCreditLimitRequest.UserId);
            },
            rejectCreditLimitRequest.IdDocument);

        /// <summary>
        /// Resends specific credit transactions.
        /// </summary>
        /// <param name="transactionIds"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ResendTransactions(List<Guid> transactionIds) =>
            await HandleRequestAndLogEventAsync(async () =>
            {
                await _creditService.ResendTransactionsAsync(transactionIds);

                return true;
            },
            string.Join(",", transactionIds));

        /// <summary>
        /// Resends all transactions of specific credits.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ResendCredits(List<Guid> ids) =>
            await HandleRequestAndLogEventAsync(async () =>
            {
                await _creditService.ResendCreditsAsync(ids);

                return true;
            },
            string.Join(",", ids));

        /// <summary>
        /// Resends the notifications of specific credits when promissory is not found
        /// </summary>
        /// <param name="request"></param>
        /// <returns>true, if send successful the notification of credit; otherwise, false</returns>
        [HttpPost]
        public async Task<IActionResult> ResendNotificationCredit([FromBody] ResendNotificationRequest request) =>
            await HandleRequestAndLogEventAsync(async () =>
            {
                await _creditService.ResendNotificationCreditCreation(request);
                return true;
            },
            nameof(ResendNotificationCredit));

        /// <summary>
        /// Resends the notifications of all credits per day when promissory not found
        /// </summary>
        /// <param name="request"></param>
        /// <returns>true, if send successful the notification of credit; otherwise, false</returns>
        [HttpPost]
        public async Task<IActionResult> ResendNotificationCreditsByDay([FromBody] ResendNotificationPerDayRequest request) =>
            await HandleRequestAndLogEventAsync(async () =>
            {
                return await _creditService.ResendNotificationCreditCreation(request);
            },
            nameof(ResendNotificationCreditsByDay));

        /// <summary>
        /// Get Detailed Active Credits
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        /// <param name="storeId"></param>
        /// <param name="calculationDate"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetDetailedActiveCredits(string typeDocument, string idDocument, string storeId, DateTime calculationDate) =>
            await HandleRequestAsync(async () =>
                    await _creditPaymentService.GetDetailedActiveCreditsAsync(typeDocument, idDocument, storeId, calculationDate),
                idDocument);

        /// <summary>
        /// Get Detailed Active Credits for the commitment module
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetDetailedActiveCreditsCompromise(string typeDocument, string idDocument) =>
            await HandleRequestAsync(async () =>
                    await _creditPaymentService.GetDetailedActiveCreditsCompromiseAsync(typeDocument, idDocument),
                idDocument);

        /// <summary>
        /// Validate credit token
        /// </summary>
        /// <param name="createCreditRequest"></param>
        /// <param name="SCLocation"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ValidateCreditToken([FromBody] CreateCreditRequest createCreditRequest, [FromHeader] string SCLocation) =>
            await HandleRequestAndLogEventAsync(async () =>
            {
                createCreditRequest.Location = SCLocation;

                return await _creditService.ValidateCreditTokenAsync(createCreditRequest);
            },
            string.Join(",", createCreditRequest.IdDocument, createCreditRequest.Token));

        /// <summary>
        /// Get paid credit document
        /// </summary>
        /// <param name="creditIds"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetPaidCreditDocumentAsync(List<Guid> creditIds) =>
            await HandleRequestAsync(async () => await _creditService.GetPaidCreditDocumentAsync(creditIds),
                string.Join(",", creditIds));

        /// <summary>
        /// Get payment plan
        /// </summary>
        /// <param name="creditValue"></param>
        /// <param name="frequency"></param>
        /// <param name="months"></param>
        /// <param name="storeId"></param>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetPaymentPlan(decimal creditValue, int frequency, int months, string storeId, string typeDocument, string idDocument) =>
            await HandleRequestAsync(async () => await _creditService.GetPaymentPlan(
                new RequiredInitialValuesForCreditDetail() { creditValue = creditValue, frequency = frequency, storeId = storeId, months = months, typeDocument = typeDocument, idDocument = idDocument }),
                idDocument);
    }
}