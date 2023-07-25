using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Events;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Helpers.Commons.Extensions;
using Sc.Credits.Helpers.Commons.Settings;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sc.Credits.Domain.UseCase.Credits
{
    /// <summary>
    /// Cancel use case is an implementation of <see cref="ICancelUseCase"/>
    /// </summary>
    public class CancelUseCase : ICancelUseCase
    {
        public readonly CredinetAppSettings _credinetAppSettings;

        /// <summary>
        /// Creates a new instance of <see cref="CancelUseCase"/>
        /// </summary>
        /// <param name="appSettings"></param>
        public CancelUseCase(ISettings<CredinetAppSettings> appSettings)
        {
            _credinetAppSettings = appSettings.Get();
        }

        /// <summary>
        /// <see cref="ICancelUseCase.GetActiveAndPendingCancellationCredits(ActiveAndPendingCancellationCreditsRequest, AppParameters)"/>
        /// </summary>
        /// <param name="activeAndPendingCancellationCreditsRequest"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public List<ActivePendingCancellationCreditResponse> GetActiveAndPendingCancellationCredits(ActiveAndPendingCancellationCreditsRequest activeAndPendingCancellationCreditsRequest,
            AppParameters parameters) =>
            CreateActiveCredits(activeAndPendingCancellationCreditsRequest, parameters).ToList();

        /// <summary>
        /// Create active credits.
        /// </summary>
        /// <param name="activeAndPendingCancellationCreditsRequest"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private IEnumerable<ActivePendingCancellationCreditResponse> CreateActiveCredits(ActiveAndPendingCancellationCreditsRequest activeAndPendingCancellationCreditsRequest,
            AppParameters parameters)
        {
            int decimalNumbersRound = parameters.DecimalNumbersRound;

            List<CreditMaster> activeCredits = activeAndPendingCancellationCreditsRequest.CustomerActiveAndPendingCancelationCreditMasters;

            foreach (var activeCredit in activeCredits.OrderByDescending(item => item.GetCreditDateComplete))
            {
                CreditMaster creditMaster = activeCredit;
                Credit credit = activeCredit.Current;

                yield return new ActivePendingCancellationCreditResponse
                {
                    TypeDocument = creditMaster.Customer.DocumentType,
                    IdDocument = creditMaster.Customer.IdDocument,
                    CreditId = creditMaster.Id,
                    CreditNumber = creditMaster.GetCreditNumber,
                    StoreId = creditMaster.Store.Id,
                    StoreName = creditMaster.Store.StoreName,
                    CreateDate = creditMaster.GetCreditDate,
                    CreditValue = credit.GetCreditValue.Round(decimalNumbersRound),
                    StatusId = creditMaster.Status.Id,
                    StatusName = creditMaster.Status.Name,
                    HasPayments = activeAndPendingCancellationCreditsRequest.CreditHasPayments(creditMaster),
                    Balance = credit.GetBalance
                };
            }
        }

        /// <summary>
        /// <see cref="ICancelUseCase.GetActiveAndPendingCancellationPayments(ActiveAndPendingCancellationPaymentsRequest, AppParameters)"/>
        /// </summary>
        /// <param name="activeAndPendingCancellationPaymentsRequest"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public List<ActivePendingCancellationPaymentResponse> GetActiveAndPendingCancellationPayments(ActiveAndPendingCancellationPaymentsRequest activeAndPendingCancellationPaymentsRequest,
            AppParameters parameters) =>
            CreateActivePayments(activeAndPendingCancellationPaymentsRequest, parameters).ToList();

        /// <summary>
        /// Create Active Payments
        /// </summary>
        /// <param name="activeAndPendingCancellationPaymentsRequest"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private IEnumerable<ActivePendingCancellationPaymentResponse> CreateActivePayments(ActiveAndPendingCancellationPaymentsRequest activeAndPendingCancellationPaymentsRequest,
            AppParameters parameters)
        {
            int maximumDaysRequestCancellationPayments = parameters.MaximumDaysRequestCancellationPayments;
            int decimalNumbersRound = parameters.DecimalNumbersRound;

            List<Credit> paymentTransactions = activeAndPendingCancellationPaymentsRequest.GetPaymentTransactions(maximumDaysRequestCancellationPayments);

            foreach (Credit paymentTransaction in paymentTransactions.OrderByDescending(item => item.GetTransactionDateComplete))
            {
                yield return new ActivePendingCancellationPaymentResponse
                {
                    TypeDocument = paymentTransaction.Customer.DocumentType,
                    IdDocument = paymentTransaction.Customer.IdDocument,
                    CreditId = paymentTransaction.GetCreditMasterId,
                    CreditNumber = paymentTransaction.GetCreditNumber,
                    StoreId = paymentTransaction.GetStoreId,
                    StoreName = paymentTransaction.Store.StoreName,
                    PaymentDate = paymentTransaction.CreditPayment.GetLastPaymentDate,
                    PaymentNumber = paymentTransaction.CreditPayment.GetPaymentNumber,
                    ValuePaid = paymentTransaction.CreditPayment.GetTotalValuePaid.Round(decimalNumbersRound),
                    HasCancelRequest = activeAndPendingCancellationPaymentsRequest.PaymentHasCancelRequest(paymentTransaction),
                    PaymentId = paymentTransaction.Id,
                    Cancelable = activeAndPendingCancellationPaymentsRequest.IsCancellablePayment(paymentTransaction)
                };
            }
        }

        /// <summary>
        /// <see cref="ICancelUseCase.CancelCredit(CancelCreditDomainRequest)"/>
        /// </summary>
        /// <param name="cancelCreditDomainRequest"></param>
        public void CancelCredit(CancelCreditDomainRequest cancelCreditDomainRequest) =>
            cancelCreditDomainRequest.CreditMaster.HandleEvent(new CancelCreditMasterEvent(cancelCreditDomainRequest, _credinetAppSettings));

        /// <summary>
        /// <see cref="ICancelUseCase.CancelPayment(CancelPaymentDomainRequest, decimal, bool)"/>
        /// </summary>
        /// <param name="cancelPaymentDomainRequest"></param>
        /// <param name="balanceReleaseForRefinancing"></param>
        /// <param name="isRefinancing"></param>
        public void CancelPayment(CancelPaymentDomainRequest cancelPaymentDomainRequest, decimal balanceReleaseForRefinancing, bool isRefinancing) =>
            cancelPaymentDomainRequest.CreditMaster.HandleEvent(new CancelPaymentMasterEvent(cancelPaymentDomainRequest, 
                                                                    _credinetAppSettings, balanceReleaseForRefinancing, isRefinancing),
                                                                   cancelPaymentDomainRequest.CancelPayment);

        /// <summary>
        /// <see cref="ICancelUseCase.UpdateStatusRequestCreditCancel(RequestStatuses, RequestCancelCredit, UserInfo)"/>
        /// </summary>
        /// <param name="requestStatus"></param>
        /// <param name="requestCancelCredit"></param>
        /// <param name="userInfo"></param>

        public void UpdateStatusRequestCreditCancel(RequestStatuses requestStatus, RequestCancelCredit requestCancelCredit, UserInfo userInfo) =>
            requestCancelCredit.UpdateStatus((int)requestStatus, userInfo.UserName, userInfo.UserId);

        /// <summary>
        /// <see cref="ICancelUseCase.UpdateStatusRequestPaymentCancel(RequestStatuses, RequestCancelPayment, UserInfo)"/>
        /// </summary>
        /// <param name="requestStatus"></param>
        /// <param name="requestCancelPayment"></param>
        /// <param name="userInfo"></param>
        public void UpdateStatusRequestPaymentCancel(RequestStatuses requestStatus, RequestCancelPayment requestCancelPayment, UserInfo userInfo) =>
            requestCancelPayment.UpdateStatus((int)requestStatus, userInfo.UserName, userInfo.UserId);

        /// <summary>
        /// <see cref="ICancelUseCase.GetSmsNotification(string, Store, CreditMaster)"/>
        /// </summary>
        /// <param name="template"></param>
        /// <param name="store"></param>
        /// <param name="creditMaster"></param>
        public SmsNotificationRequest GetSmsNotification(string template, Store store, CreditMaster creditMaster)
        {
            string message = template;
            message = message.Replace("{creditNumber}", creditMaster.GetCreditNumber.ToString());
            message = message.Replace("{storeName}", store.StoreName);
            message = message.Replace("{date}", DateTime.Now.ToString());

            SmsNotificationRequest smsNotificationRequest = new SmsNotificationRequest
            {
                Message = message,
                Mobile = creditMaster.Customer.GetMobile
            };

            return smsNotificationRequest;
        }

        /// <summary>
        /// <see cref="ICancelUseCase.GetMailNotification(message, string, Store, CreditMaster)"/>
        /// </summary>
        /// <param name="template"></param>
        /// <param name="reason"></param>
        /// <param name="store"></param>
        /// <param name="creditMaster"></param>
        /// <returns></returns>
        public MailNotificationRequest GetMailNotification(message template, string reason, Store store, CreditMaster creditMaster)
        {
            List<TemplateValue> templateValues = new List<TemplateValue>
            {
                new TemplateValue
                {
                    Key = "{{Fullname}}",
                    Value = creditMaster.Customer.GetFullName
                },
                new TemplateValue
                {
                    Key = "{{Message}}",
                    Value = CancellationMessages.GetMessage(template,store.StoreName,creditMaster.GetCreditNumber.ToString())
                },
                new TemplateValue
                {
                    Key = "{{Email}}",
                    Value = creditMaster.Customer.GetEmail
                }
            };

            MailNotificationRequest mailNotificationRequest = new MailNotificationRequest
            {
                TemplateInfo = new TemplateInfo
                {
                    TemplateId = _credinetAppSettings.TemplateCancellation,
                    TemplateValues = templateValues
                },
                Recipients = new List<string> { creditMaster.Customer.GetEmail },
                Subject = $"Proceso de anulación de { reason }"
            };

            return mailNotificationRequest;
        }
    }
}