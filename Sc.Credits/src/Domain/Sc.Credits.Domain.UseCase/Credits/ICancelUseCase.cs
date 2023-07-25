using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Helpers.ObjectsUtils;
using System.Collections.Generic;

namespace Sc.Credits.Domain.UseCase.Credits
{
    /// <summary>
    /// Cancel credit uses case contract
    /// </summary>
    public interface ICancelUseCase
    {
        /// <summary>
        /// Get active and pending cancellation credits
        /// </summary>
        /// <param name="activeAndPendingCancellationCreditsRequest"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        List<ActivePendingCancellationCreditResponse> GetActiveAndPendingCancellationCredits(ActiveAndPendingCancellationCreditsRequest activeAndPendingCancellationCreditsRequest,
            AppParameters parameters);

        /// <summary>
        /// Get active and pending cancellation payments
        /// </summary>
        /// <param name="activeAndPendingCancellationPaymentsRequest"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        List<ActivePendingCancellationPaymentResponse> GetActiveAndPendingCancellationPayments(ActiveAndPendingCancellationPaymentsRequest activeAndPendingCancellationPaymentsRequest,
            AppParameters parameters);

        /// <summary>
        /// Cancel credit
        /// </summary>
        /// <param name="cancelCreditDomainRequest"></param>
        void CancelCredit(CancelCreditDomainRequest cancelCreditDomainRequest);

        /// <summary>
        /// Cancel payment
        /// </summary>
        /// <param name="cancelPaymentDomainRequest"></param>
        /// <param name="balanceReleaseForRefinancing"></param>
        /// <param name="isRefinancing"></param>
        void CancelPayment(CancelPaymentDomainRequest cancelPaymentDomainRequest, decimal balanceReleaseForRefinancing, bool isRefinancing);

        /// <summary>
        /// Update status request credit cancel
        /// </summary>
        /// <param name="requestStatus"></param>
        /// <param name="requestCancelCredit"></param>
        /// <param name="userInfo"></param>
        void UpdateStatusRequestCreditCancel(RequestStatuses requestStatus, RequestCancelCredit requestCancelCredit, UserInfo userInfo);

        /// <summary>
        /// Update status request payment cancel
        /// </summary>
        /// <param name="requestStatus"></param>
        /// <param name="requestCancelPayment"></param>
        /// <param name="userInfo"></param>
        void UpdateStatusRequestPaymentCancel(RequestStatuses requestStatus, RequestCancelPayment requestCancelPayment, UserInfo userInfo);

        /// <summary>
        /// Get Sms Notification
        /// </summary>
        /// <param name="template"></param>
        /// <param name="store"></param>
        /// <param name="creditMaster"></param>
        /// <returns></returns>
        SmsNotificationRequest GetSmsNotification(string template, Store store, CreditMaster creditMaster);

        /// <summary>
        /// Get Sms Notification
        /// </summary>
        /// <param name="template"></param>
        /// <param name="reason"></param>
        /// <param name="store"></param>
        /// <param name="creditMaster"></param>
        /// <returns></returns>
        MailNotificationRequest GetMailNotification(message template, string reason, Store store, CreditMaster creditMaster);
    }
}