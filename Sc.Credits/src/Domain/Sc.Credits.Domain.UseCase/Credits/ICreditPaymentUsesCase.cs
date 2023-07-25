using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.Domain.Model.Stores;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace Sc.Credits.Domain.UseCase.Credits
{
    /// <summary>
    /// Credit payment uses case contract
    /// </summary>
    public interface ICreditPaymentUsesCase
    {
        /// <summary>
        /// Pay
        /// </summary>
        /// <param name="paymentDomainRequest"></param>
        /// <param name="paymentType"></param>
        /// <param name="transaction"></param>
        /// <param name="simulation"></param>
        /// <returns></returns>
        Task<PaymentCreditResponse> PayAsync(PaymentDomainRequest paymentDomainRequest, PaymentType paymentType = null,
            Transaction transaction = null, bool simulation = false, bool setCreditLimit = true);

        /// <summary>
        /// Get payment fees
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="credit"></param>
        /// <param name="parameters"></param>
        /// <param name="calculationDate"></param>
        /// <param name="totalPayment"></param>
        /// <returns></returns>
        PaymentFeesResponse GetPaymentFees(CreditMaster creditMaster, Credit credit, AppParameters parameters, DateTime calculationDate,
            out decimal totalPayment);

        /// <summary>
        /// Gets the minimum payment
        /// </summary>
        /// <param name="paymentFee"></param>
        /// <param name="arrearsFees"></param>
        /// <param name="totalPayment"></param>
        /// <param name="updatedPaymentPlanValue"></param>
        /// <param name="hasUpdatedPaymentPlan"></param>
        /// <param name="maximumUpdatedPaymentPlanGapRate"></param>
        /// <returns></returns>
        decimal GetMinimumPayment(List<PaymentFee> paymentFee, int arrearsFees, decimal totalPayment, decimal updatedPaymentPlanValue,
            bool hasUpdatedPaymentPlan, decimal maximumUpdatedPaymentPlanGapRate);

        /// <summary>
        /// Assurance payment
        /// </summary>
        /// <param name="paymentMatrix"></param>
        /// <param name="pendingPaymentValue"></param>
        /// <param name="amortizationScheduleResponse"></param>
        /// <param name="creditValueBalance"></param>
        /// <param name="maximumPaymentAdjustmentResidue"></param>
        /// <param name="calculationDate"></param>
        /// <param name="paymentValue"></param>
        /// <param name="interestValuePayment"></param>
        /// <param name="arrearsValuePayment"></param>
        /// <param name="chargeValue"></param>
        /// <param name="arrearsGracePeriod"></param>
        /// <param name="totalPayment"></param>
        /// <returns></returns>
        decimal GetAssuranceOrdinaryPayment(PaymentMatrix paymentMatrix, decimal pendingPaymentValue, AmortizationScheduleResponse amortizationScheduleResponse, decimal creditValueBalance, decimal maximumPaymentAdjustmentResidue,
            DateTime calculationDate, decimal paymentValue, decimal interestValuePayment, decimal arrearsValuePayment, decimal chargeValue, int arrearsGracePeriod, decimal totalPayment);

        /// <summary>
        /// Get interest payment
        /// </summary>
        /// <param name="paymentMatrix"></param>
        /// <param name="previousInterest"></param>
        /// <returns></returns>
        decimal GetInterestPayment(PaymentMatrix paymentMatrix, decimal previousInterest);

        /// <summary>
        /// Get Current Amortization Schedule
        /// </summary>
        /// <param name="currentAmortizationScheduleRequest"></param>
        /// <param name="calculationDate"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        CurrentAmortizationScheduleResponse GetCurrentAmortizationSchedule(CurrentAmortizationScheduleRequest currentAmortizationScheduleRequest,
            DateTime calculationDate, AppParameters parameters);

        /// <summary>
        /// Get current payment schedule
        /// </summary>
        /// <param name="currentPaymentScheduleRequest"></param>
        /// <param name="calculationDate"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        CurrentPaymentScheduleResponse GetCurrentPaymentSchedule(CurrentPaymentScheduleRequest currentPaymentScheduleRequest,
            DateTime calculationDate, AppParameters parameters);

        /// <summary>
        /// Get Arrears Payment
        /// </summary>
        /// <param name="paymentMatrix"></param>
        /// <param name="arrearsCharges"></param>
        /// <param name="hasArrearsCharge"></param>
        /// <param name="previousArrears"></param>
        /// <returns></returns>
        decimal GetArrearsPayment(PaymentMatrix paymentMatrix, decimal arrearsCharges, bool hasArrearsCharge, decimal previousArrears);

        /// <summary>
        /// Get full payment
        /// </summary>
        /// <param name="creditValueBalance"></param>
        /// <param name="interestPaymentValue"></param>
        /// <param name="arrearsPayment"></param>
        /// <param name="chargesPayment"></param>
        /// <param name="payableAssurance"></param>
        /// <param name="amortizationSchedule"></param>
        /// <param name="calculationDate"></param>
        /// <returns></returns>
        decimal GetFullPayment(decimal creditValueBalance, decimal interestPaymentValue, decimal arrearsPayment, decimal chargesPayment,
            decimal payableAssurance, AmortizationScheduleResponse amortizationSchedule, DateTime calculationDate);

        /// <summary>
        /// Get first due fee
        /// </summary>
        /// <param name="lastPaymentFee"></param>
        /// <param name="totalFees"></param>
        /// <returns></returns>
        int GetFirstDueFee(int lastPaymentFee, int totalFees);

        /// <summary>
        /// Get last payment fee
        /// </summary>
        /// <param name="amortizationSchedule"></param>
        /// <param name="creditValueBalance"></param>
        /// <param name="nextDueDate"></param>
        /// <returns></returns>
        int GetLastPaymentFee(AmortizationScheduleResponse amortizationSchedule, decimal creditValueBalance, out DateTime nextDueDate);

        /// <summary>
        /// Get Last Due Fee
        /// </summary>
        /// <param name="calculationDate"></param>
        /// <param name="amortizationScheduleResponse"></param>
        /// <returns></returns>
        int GetLastDueFee(DateTime calculationDate, AmortizationScheduleResponse amortizationScheduleResponse);

        /// <summary>
        /// Get value from tax
        /// </summary>
        /// <param name="totalValue"></param>
        /// <param name="taxValue"></param>
        /// <returns></returns>
        decimal GetValueFromTax(decimal totalValue, decimal taxValue);

        /// <summary>
        /// Due credit balance
        /// </summary>
        /// <param name="paymentMatrix"></param>
        /// <param name="lastDueFee"></param>
        /// <param name="firstDueFee"></param>
        /// <returns></returns>
        decimal GetDueCreditBalance(PaymentMatrix paymentMatrix, int lastDueFee, int firstDueFee);

        /// <summary>
        /// Get payment alternatives (minimum, total and payment by fees)
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="credit"></param>
        /// <param name="parameters"></param>
        /// <param name="calculationDate"></param>
        /// <returns></returns>
        PaymentAlternatives GetPaymentAlternatives(CreditMaster creditMaster, Credit credit, AppParameters parameters, DateTime calculationDate);

        /// <summary>
        /// Get active credits
        /// </summary>
        /// <param name="creditMasters"></param>
        /// <param name="parameters"></param>
        /// <param name="calculationDate"></param>
        /// <returns></returns>
        List<CreditStatus> GetActiveCredits(List<CreditMaster> creditMasters, AppParameters parameters, DateTime calculationDate);

        /// <summary>
        /// Get detailed active credits
        /// </summary>
        /// <param name="creditMasters"></param>
        /// <param name="parameters"></param>
        /// <param name="calculationDate"></param>
        /// <returns></returns>
        List<DetailedCreditStatus> GetDetailedActiveCredits(List<CreditMaster> creditMasters, AppParameters parameters, DateTime calculationDate);

        /// <summary>
        /// Get Complete Credits Data
        /// </summary>
        /// <param name="creditMasters"></param>
        /// <param name="parameters"></param>
        /// <param name="calculationDate"></param>
        /// <returns></returns>
        List<DetailedCreditStatus> GetCompleteCreditsData(List<CreditMaster> creditMasters, AppParameters parameters, DateTime calculationDate);
        /// <summary>
        /// Get active credits for the commitment module
        /// </summary>
        /// <param name="creditMasters"></param>
        /// <param name="parameters"></param>
        /// <param name="ClientName"></param>
        /// <returns></returns>
        ClientCompromise GetDetailedActiveCreditsCompromise(List<CreditMaster> creditMasters, AppParameters parameters, string ClientName);

        /// <summary>
        /// Get Pay Mail Notification
        /// </summary>
        /// <param name="paymentCreditResponse"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <param name="taxValue"></param>
        /// <returns></returns>
        MailNotificationRequest GetPayMailNotification(PaymentCreditResponse paymentCreditResponse, int decimalNumbersRound, decimal taxValue);
        
        /// <summary>
        /// Get Pay Mail Notification
        /// </summary>
        /// <param name="paymentCreditResponse"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <param name="taxValue"></param>
        /// <returns></returns>
        MailNotificationRequest GetMultiplePayMailNotification(List<PaymentCreditResponse> paymentCreditResponse, int decimalNumbersRound, decimal taxValue);

        /// <summary>
        /// Get Sms Notification
        /// </summary>
        /// <param name="template"></param>
        /// <param name="store"></param>
        /// <param name="credit"></param>
        /// <param name="decimalNumberRound"></param>
        /// <returns></returns>
        SmsNotificationRequest GetSmsNotification(string template, Store store, Credit credit, int decimalNumberRound);

        /// <summary>
        /// Get Sms Notification
        /// </summary>
        /// <param name="template"></param>
        /// <param name="customer"></param>
        /// <param name="total"></param>
        /// <param name="decimalNumberRound"></param>
        /// <returns></returns>
        SmsNotificationRequest GetSmsNotification(string template, Customer customer, decimal total, int decimalNumberRound);

        /// <summary>
        /// Get status credit
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        CreditStatusResponse GetStatusCredit(CreditMaster creditMaster, AppParameters parameters);

        /// <summary>
        /// Get last payment fee from payment matrix
        /// </summary>
        /// <param name="paymentMatrix"></param>
        /// <param name="pendingPaymentValue"></param>
        /// <returns></returns>
        int GetLastPaymentFeeFromPaymentMatrix(PaymentMatrix paymentMatrix, decimal pendingPaymentValue);

        /// <summary>
        /// Get Payment Template
        /// </summary>
        /// <param name="paymentTemplateResponse"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <param name="reprint"></param>
        /// <returns></returns>
        string GetPaymentTemplate(PaymentTemplateResponse paymentTemplateResponse, int decimalNumbersRound, bool reprint);

        /// <summary>
        /// Create payment history
        /// </summary>
        /// <param name="payments"></param>
        /// <param name="requestCancelPaymentsNotDismissed"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        List<PaymentHistoryResponse> CreatePaymentHistory(List<Credit> payments, List<RequestCancelPayment> requestCancelPaymentsNotDismissed,
            AppParameters parameters);
    }
}