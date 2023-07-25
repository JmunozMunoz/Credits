using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Parameters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace Sc.Credits.Domain.Managment.Services.Credits
{
    /// <summary>
    /// Credit payment service contract
    /// </summary>
    public interface ICreditPaymentService
    {
        /// <summary>
        /// Pay credit
        /// </summary>
        /// <param name="paymentCreditRequest"></param>
        /// <param name="notify"></param>
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<PaymentCreditResponse> PayCreditAsync(PaymentCreditRequest paymentCreditRequest, bool notify, AppParameters parameters = null,
           Transaction transaction = null, bool simulation = false);

        /// <summary>
        /// Pay credit
        /// </summary>
        /// <param name="paymentCreditRequest"></param>
        /// <param name="notify"></param>
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<PaymentCreditResponse> PayCreditAsync(PaymentCreditRequestComplete paymentCreditRequest, bool notify,
            AppParameters parameters = null, Transaction transaction = null, bool simulation = false);

        /// <summary>
        /// Down payment
        /// </summary>
        /// <param name="paymentCreditRequest"></param>
        /// <param name="creditMaster"></param>
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<PaymentCreditResponse> DownPaymentAsync(PaymentCreditRequestComplete paymentCreditRequest, CreditMaster creditMaster, AppParameters parameters,
            Transaction transaction = null);

        /// <summary>
        /// Pay credit multiple
        /// </summary>
        /// <param name="payCreditsRequest"></param>
        /// <returns></returns>
        Task<List<PaymentCreditResponse>> PayCreditMultipleAsync(PayCreditsRequest payCreditsRequest);

        /// <summary>
        /// Pay credit multiple and notify
        /// </summary>
        /// <param name="paymentCreditsRequest"></param>
        /// <returns></returns>
        Task PayCreditMultipleAndNotifyAsync(PayCreditsRequest paymentCreditsRequest);

        /// <summary>
        /// Get Current Amortization Schedule
        /// </summary>
        /// <param name="currentAmortizationScheduleRequest"></param>
        /// <returns></returns>
        Task<CurrentAmortizationScheduleResponse> GetCurrentAmortizationScheduleAsync(CurrentAmortizationScheduleRequest currentAmortizationScheduleRequest);

        /// <summary>
        /// Get current payment schedule
        /// </summary>
        /// <param name="currentPaymentScheduleRequest"></param>
        /// <param name="calculationDate"></param>
        /// <returns></returns>
        Task<CurrentPaymentScheduleResponse> GetCurrentPaymentScheduleAsync(CurrentPaymentScheduleRequest currentPaymentScheduleRequest, DateTime calculationDate);

        /// <summary>
        /// Get Active Credits
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        /// <param name="storeId"></param>
        /// <param name="calculationDate"></param>
        Task<List<CreditStatus>> GetActiveCreditsAsync(string typeDocument, string idDocument, string storeId, DateTime calculationDate);

        /// <summary>
        /// Get Detailed Active Credits
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        /// <param name="storeId"></param>
        /// <param name="calculationDate"></param>
        Task<List<DetailedCreditStatus>> GetDetailedActiveCreditsAsync(string typeDocument, string idDocument, string storeId, DateTime calculationDate);/// <summary>


        /// <summary>
        /// Get Complete Credits Data Async
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        /// <param name="calculationDate"></param>
        Task<List<DetailedCreditStatus>> GetCompleteCreditsDataAsync(string typeDocument, string idDocument, DateTime calculationDate);/// <summary>


        /// Get Detailed Active Credits by CreditMaster Id
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        /// <param name="storeId"></param>
        /// <param name="calculationDate"></param>
        Task<List<DetailedCreditStatus>> GetDetailedActiveCreditsByCreditMasterIdAsync(List<Guid> creditMastersId, DateTime calculationDate);

        /// <summary>
        /// Get Detailed Active Credits compromise
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        Task<ClientCompromise> GetDetailedActiveCreditsCompromiseAsync(string typeDocument, string idDocument);

        /// <summary>
        /// Active credits notify
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        /// <param name="storeId"></param>
        /// <param name="calculationDate"></param>
        /// <returns></returns>
        Task ActiveCreditsNotifyAsync(ActiveCreditsRequest activeCreditsRequest);

        /// <summary>
        /// Get payment Fees
        /// </summary>
        /// <param name="creditId"></param>
        /// <returns></returns>
        Task<PaymentFeesResponse> GetPaymentFeesAsync(Guid creditId);

        /// <summary>
        /// Get Payment Templates for print.
        /// </summary>
        /// <param name="paymentsId"></param>
        /// <returns></returns>
        Task<List<PaymentTemplateResponse>> GetPaymentTemplatesAsync(List<Guid> paymentsId, bool reprint);

        /// <summary>
        /// Get customer payment history
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="documentType"></param>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        Task<List<PaymentHistoryResponse>> GetCustomerPaymentHistoryAsync(string storeId, string documentType, string idDocument);

        /// <summary>
        /// Payment credit notify
        /// </summary>
        /// <param name="paymentCreditResponse"></param>
        /// <param name="assuranceTax"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <returns></returns>
        Task PaymentCreditNotifyAsync(PaymentCreditResponse paymentCreditResponse, decimal assuranceTax, int decimalNumbersRound);

        /// <summary>
        /// Get data calculate credit
        /// </summary>
        /// <param name="creditId"></param>
        /// <param name="calculationTime"></param>
        /// <returns></returns>
        Task<CalculatedQuery> GetDataCalculateCreditAsync(Guid creditId, DateTime calculationTime);

        /// <summary>
        /// Payment credit notify
        /// </summary>
        /// <param name="paymentCreditResponse"></param>
        /// <param name="assuranceTax"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <returns></returns>
        Task MultiplePaymentCreditNotifyAsync(List<PaymentCreditResponse> paymentCreditResponse, decimal assuranceTax, int decimalNumbersRound);

        /// <summary>
        /// Get Active Credits
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        /// <param name="storeId"></param>
        /// <param name="calculationDate"></param>
        Task<CreditStatus> GetActiveCredit(Guid creditId, DateTime calculationDate);

        /// <summary>
        /// Validate the bussines rules for make an external payment
        /// </summary>
        /// <param name="calculationDate"></param>
        /// <param name="creditId"></param>
        /// <param name="totalValuePaid"></param>
        /// <returns></returns>
        Task<bool> ValidateRulesForExternalPayments(DateTime calculationDate, Guid creditId, decimal totalValuePaid);
    }
}