using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Parameters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Managment.Services.Credits
{
    /// <summary>
    /// Credit service contract
    /// </summary>
    public interface ICreditService
    {
        /// <summary>
        /// Get time limit in months
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="typeDocument"></param>
        /// <param name="creditValue"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        Task<int> GetTimeLimitInMonthsAsync(string idDocument, string typeDocument, decimal creditValue, string storeId);

        /// <summary>
        /// Gets the credit details asynchronous.
        /// </summary>
        /// <param name="requiredValues">The required values.</param>
        /// <returns></returns>
        Task<CreditDetailResponse> GetCreditDetailsAsync(RequiredInitialValuesForCreditDetail requiredValues);

        /// <summary>
        /// Create credit
        /// </summary>
        /// <param name="createCreditRequest"></param>
        /// <returns></returns>
        Task<CreateCreditResponse> CreateAsync(CreateCreditRequest createCreditRequest);

        /// <summary>
        /// Credit creation notify
        /// </summary>
        /// <param name="createCreditResponse"></param>
        /// <returns></returns>
        Task CreditCreationNotifyAsync(CreateCreditResponse createCreditResponse);

        /// <summary>
        /// Get customer credit limit
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        /// <param name="vendorId"></param>
        /// ///
        /// <returns></returns>
        Task<CustomerCreditLimitResponse> GetCustomerCreditLimitAsync(string typeDocument, string idDocument, string vendorId);

        /// <summary>
        /// Get customer credit limit increase
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        Task<bool> GetCustomerCreditLimitIncreaseAsync(string typeDocument, string idDocument);

        /// <summary>
        /// Get customer credit limit by credit value
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        /// <param name="vendorId"></param>
        /// <param name="creditValue"></param>
        /// <returns></returns>
        Task<CustomerCreditLimitResponse> GetCustomerCreditLimitAsync(string typeDocument, string idDocument, string vendorId, decimal creditValue);

        /// <summary>
        /// Get original amortization schedule
        /// </summary>
        /// <param name="amortizationScheduleRequest"></param>
        /// <returns>CreditDetailResponse with all the operations and details made</returns>
        Task<AmortizationScheduleResponse> GetOriginalAmortizationScheduleAsync(AmortizationScheduleRequest amortizationScheduleRequest);

        /// <summary>
        /// Update credit extra fields
        /// </summary>
        /// <param name="udateCreditExtraFieldsRequest"></param>
        /// <returns></returns>
        Task UpdateExtraFieldsAsync(UpdateCreditExtraFieldsRequest updateCreditExtraFieldsRequest);

        /// <summary>
        /// Update charges payment plan value
        /// </summary>
        /// <param name="id"></param>
        /// <param name="charges"></param>
        /// <param name="hasArrearsCharge"></param>
        /// <param name="arrearsCharges"></param>
        /// <param name="updatedPaymentPlanValue"></param>
        /// <returns></returns>
        Task UpdateChargesPaymentPlanValueAsync(Guid id, decimal charges, bool hasArrearsCharge, decimal arrearsCharges, decimal updatedPaymentPlanValue);

        /// <summary>
        /// Generate token
        /// </summary>
        /// <param name="generateTokenRequest"></param>
        /// <returns></returns>
        Task<TokenResponse> GenerateTokenAsync(GenerateTokenRequest generateTokenRequest);

        /// <summary>
        /// Generate token
        /// </summary>
        /// <param name="generateTokenRequest"></param>
        /// <returns></returns>
        Task<TokenResponse> GenerateTokenWithRiskLevelCalculationAsync(GenerateTokenRequest generateTokenRequest);

        /// <summary>
        /// Token call request
        /// </summary>
        /// <param name="creditTokenCallRequest"></param>
        /// <returns></returns>
        Task<bool> TokenCallRequestAsync(CreditTokenCallRequest creditTokenCallRequest);

        /// <summary>
        /// Get promissory note info
        /// </summary>
        /// <param name="id"></param>
        /// <param name="reprint"></param>
        /// <returns></returns>
        Task<PromissoryNoteInfo> GetPromissoryNoteInfoAsync(Guid id, bool reprint);

        /// <summary>
        /// Get paid credit certificate templates
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="reprint"></param>
        /// <returns></returns>
        Task<List<string>> GetPaidCreditCertificateTemplatesAsync(List<Guid> ids, bool reprint);

        /// <summary>
        /// Get customer credit history
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="documentType"></param>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        Task<List<CreditHistoryResponse>> GetCustomerCreditHistoryAsync(string storeId, string documentType, string idDocument);

        /// <summary>
        /// Customer allow photo signature
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<bool> CustomerAllowPhotoSignatureAsync(Customer customer, AppParameters parameters);

        /// <summary>
        /// Resends a credit transactions by specific ids.
        /// </summary>
        /// <param name="transactionIds"></param>
        /// <returns></returns>
        Task ResendTransactionsAsync(List<Guid> transactionIds);

        /// <summary>
        /// Resends all transactions of specific credits.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task ResendCreditsAsync(List<Guid> ids);

        /// <summary>
        /// Resends notification create credit of specific credit.
        /// </summary>
        /// <param name="creditMasterId">unique identifier of credit</param>
        /// <returns></returns>
        Task ResendNotificationCreditCreation(ResendNotificationRequest request);

        /// <summary>
        /// Resends notifications create credit by date day.
        /// </summary>
        /// <param name="dayDate"></param>
        /// <returns></returns>
        Task<ResendNotificationPerDayResponse> ResendNotificationCreditCreation(ResendNotificationPerDayRequest request);

        /// <summary>
        /// Validate credit token
        /// </summary>
        /// <param name="createCreditRequest"></param>
        /// <returns></returns>
        Task<bool> ValidateCreditTokenAsync(CreateCreditRequest createCreditRequest);

        /// <summary>
        /// Get paid credit document
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<List<string>> GetPaidCreditDocumentAsync(List<Guid> ids);

        /// <summary>
        /// Gets payment plan
        /// </summary>
        /// <param name="requiredValues"></param>
        /// <returns></returns>
        Task<PaymentPlanResponse> GetPaymentPlan(RequiredInitialValuesForCreditDetail requiredValues);

        /// <summary>
        /// Verifies if the required credit exists in the database
        /// </summary>
        /// <param name="createCreditRequest"></param>
        /// <returns></returns>
        Task<CreateCreditResponse> GetRecentCreditByToken(VerifyCreditCreationRequest createCreditRequest);

        /// <summary>
        /// Updates the credit request by agent.
        /// </summary>
        /// <param name="getServiceStatusClient">The get service status client.</param>
        /// <returns></returns>
        Task<bool> UpdateCreditRequestByAgent(CreditRequestAnalysis creditRequestAnalysis);

        /// <summary>
        /// Gets status by credit request id
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        Task<bool> ExistCreditRequestById(Guid creditRequestId);
    }
}