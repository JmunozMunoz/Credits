using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Parameters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using System.Data;

namespace Sc.Credits.Domain.UseCase.Credits
{
    /// <summary>
    /// Credit uses case contract
    /// </summary>
    public interface ICreditUsesCase
    {
        /// <summary>
        /// Get credit details
        /// </summary>
        /// <param name="creditDetailDomainRequest"></param>
        /// <returns></returns>
        CreditDetailResponse GetCreditDetails(CreditDetailDomainRequest creditDetailDomainRequest);

        /// <summary>
        /// Gets the credit details.
        /// </summary>
        /// <param name="simulatedCreditDetailRequest">The credit detail domain request.</param>
        /// <returns>CreditDetailResponse with operations made</returns>
        CreditDetailResponse GetCreditDetails(SimulatedCreditRequest simulatedCreditDetailRequest);

        /// <summary>
        /// Get time limit in months
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="creditValue"></param>
        /// <param name="store"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <param name="partialCreditLimit"></param>
        /// <returns></returns>
        int GetTimeLimitInMonths(Customer customer, decimal creditValue, Store store, int decimalNumbersRound,
            decimal partialCreditLimit);

        /// <summary>
        /// Get time limit in months when it is independent of a customer
        /// </summary>
        /// <param name="creditValue"></param>
        /// <param name="store"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <param name="partialCreditLimit"></param>
        /// <returns>number of months that a credit can be deferred</returns>
        int GetTimeLimitInMonths(decimal creditValue, Store store, int decimalNumbersRound);

        /// <summary>
        /// Gets the store minimum and maximum credit limit by store identifier.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="decimalNumbersRound">The decimal numbers round.</param>
        /// <returns>range of value from storecategory of store</returns>
        StoreCategoryRange GetStoreMinAndMaxCreditLimitByStoreId(Store store, int decimalNumbersRound);

        /// <summary>
        /// Create credit
        /// </summary>
        /// <param name="createCreditDomainRequest"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<CreateCreditResponse> CreateAsync(CreateCreditDomainRequest createCreditDomainRequest, Transaction transaction = null, bool setCreditLimit = true);

        /// <summary>
        /// Delete Credit
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="column"></param>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <returns>is Delete</returns>
        Task<bool> DeleteAsync(CreditMaster creditMaster, Transaction transaction = null);

        /// <summary>
        /// Get credit limit client
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="partialCreditLimit"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <param name="arrearsGracePeriod"></param>
        /// <param name="minimumCreditValue"></param>
        /// <param name="vendorId"></param>
        /// <returns></returns>
        Task<CustomerCreditLimitResponse> GetCustomerCreditLimitAsync(Customer customer, decimal partialCreditLimit, int decimalNumbersRound,
            int arrearsGracePeriod, decimal minimumCreditValue, string vendorId);

        /// <summary>
        /// Get original amortization schedule
        /// </summary>
        /// <param name="amortizationScheduleRequest"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <returns></returns>
        AmortizationScheduleResponse GetOriginalAmortizationSchedule(AmortizationScheduleRequest amortizationScheduleRequest, int decimalNumbersRound);

        /// <summary>
        /// Update Extra Fields
        /// </summary>
        /// <param name="udateCreditExtraFieldsRequest"></param>
        /// <param name="creditMaster"></param>
        /// <returns></returns>
        void UpdateExtraFields(UpdateCreditExtraFieldsRequest updateCreditExtraFieldsRequest, CreditMaster creditMaster);

        /// <summary>
        /// Update ScCode
        /// </summary>
        /// <param name="scCode"></param>
        /// <param name="creditMaster"></param>
        void UpdateScCode(string scCode, CreditMaster creditMaster);

        /// <summary>
        /// Get token Sms notification
        /// </summary>
        /// <param name="template"></param>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <param name="creditDetails"></param>
        /// <param name="months"></param>
        /// <param name="frequency"></param>
        /// <param name="token"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <returns></returns>
        SmsNotificationRequest GetTokenSmsNotification(string template, Customer customer, Store store, CreditDetailResponse creditDetails, int months, int frequency,
            string token, int decimalNumbersRound);

        /// <summary>
        /// Sends the SMS notification.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="customer">The customer.</param>
        /// <returns></returns>
        SmsNotificationRequest SendSmsNotification(string template, Customer customer);

        /// <summary>
        /// Get mail notification
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <param name="creditDetails"></param>
        /// <param name="months"></param>
        /// <param name="frequency"></param>
        /// <param name="token"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <returns></returns>
        MailNotificationRequest GetTokenMailNotification(Customer customer, Store store, CreditDetailResponse creditDetails, int months, int frequency,
            string token, int decimalNumbersRound);

        /// <summary>
        /// Get create credit mail notification
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="creditMaster"></param>
        /// <param name="credit"></param>
        /// <param name="createCreditResponse"></param>
        /// <param name="store"></param>
        /// <param name="promissoryNoteFileName"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <returns></returns>
        MailNotificationRequest GetCreateCreditMailNotification(Customer customer, CreditMaster creditMaster, Credit credit, CreateCreditResponse createCreditResponse,
            Store store, string promissoryNoteFileName, int decimalNumbersRound);

        /// <summary>
        /// Get valid effective annual rate
        /// </summary>
        /// <param name="effectiveAnualRate1"></param>
        /// <param name="effectiveAnualRate2"></param>
        /// <returns></returns>
        decimal GetValidEffectiveAnnualRate(decimal effectiveAnualRate1, decimal effectiveAnualRate2);

        /// <summary>
        /// Get Interest Rate
        /// </summary>
        /// <param name="effectiveAnnualRate"></param>
        /// <param name="frequency"></param>
        /// <returns></returns>
        decimal GetInterestRate(decimal effectiveAnnualRate, int frequency);

        /// <summary>
        /// Update Status
        /// </summary>
        /// <param name="newStatus"></param>
        /// <param name="creditMaster"></param>
        void UpdateStatus(int newStatus, CreditMaster creditMaster);

        /// <summary>
        /// Get promissory note info
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="parameters"></param>
        /// <param name="template"></param>
        /// <param name="reprint"></param>
        /// <returns></returns>
        PromissoryNoteInfo GetPromissoryNoteInfo(CreditMaster creditMaster, AppParameters parameters,
            string template, bool reprint);

        /// <summary>
        /// Has arrears
        /// </summary>
        /// <param name="calculationDate"></param>
        /// <param name="dueDate"></param>
        /// <param name="graceDays"></param>
        /// <returns></returns>
        bool HasArrears(DateTime calculationDate, DateTime dueDate, int graceDays);

        /// <summary>
        /// Get Arrears Days
        /// </summary>
        /// <param name="calculationDate"></param>
        /// <param name="dueDate"></param>
        /// <returns></returns>
        int GetArrearsDays(DateTime calculationDate, DateTime dueDate);

        /// <summary>
        /// Customer is allowed to create credits
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="vendorId"></param>
        /// <param name="arrearsGracePeriod"></param>
        /// <returns></returns>
        Task<bool> CustomerIsDefaulterAsync(Customer customer, string vendorId, int arrearsGracePeriod);

        /// <summary>
        /// Get Paid Credit Certificate Template
        /// </summary>
        /// <param name="paidCreditCertificateResponse"></param>
        /// <param name="reprint"></param>
        /// <returns></returns>
        string GetPaidCreditCertificateTemplate(PaidCreditCertificateResponse paidCreditCertificateResponse, bool reprint);

        /// <summary>
        /// Get credit history
        /// </summary>
        /// <param name="creditMasters"></param>
        /// <param name="requestCancelCreditsCanceled"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        List<CreditHistoryResponse> CreateCreditHistory(List<CreditMaster> creditMasters, List<RequestCancelCredit> requestCancelCreditsCanceled,
            AppParameters parameters);

        /// <summary>
        /// Get next fee date by frequency
        /// </summary>
        /// <param name="fee"></param>
        /// <param name="frequency"></param>
        /// <param name="firstFeeDate"></param>
        /// <param name="previousFeeDate"></param>
        /// <returns></returns>
        DateTime GetNextFeeDateByFrequency(int fee, Frequencies frequency, DateTime firstFeeDate, DateTime? previousFeeDate = null);

        /// <summary>
        /// Get calculate assurance to balance
        /// </summary>
        /// <param name="creditValuePayment"></param>
        /// <param name="assurancePercentage"></param>
        /// <param name="assuranceTax"></param>
        /// <returns></returns>
        decimal CalculateAssuranceToBalance(decimal creditValuePayment, decimal assurancePercentage, decimal assuranceTax);

        /// <summary>
        /// Gets the amortization schedule's url
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="creditDetails"></param>
        /// <param name="frequency"></param>
        /// <returns></returns>
        string GetAmortizationScheduleUrl(Customer customer, CreditDetailResponse creditDetails, int frequency);

        /// <summary>
        /// Gets the risky credit request notification
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <param name="generateTokenRequest"></param>
        /// <param name="riskLevel"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        MailNotificationRequest GetRiskyCreditRequestNotification(
            Customer customer, Store store, GenerateTokenRequest generateTokenRequest, CustomerRiskLevel riskLevel, int decimalNumbersRound, Guid transactionId);


        /// <summary>
        /// Get original amortization schedule html
        /// </summary>
        /// <param name="creditDetails"></param>
        /// <param name="frequency"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <param name="createDate"></param>
        /// <param name="lastFeeDate"></param>
        /// <returns></returns>
        string GetOriginalAmortizationScheduleHtml(CreditDetailResponse creditDetails, int frequency, int decimalNumbersRound,
                                                           DateTime createDate, out DateTime lastFeeDate);
    }
}