using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Customers;
using Sc.Credits.Domain.Managment.Services.Stores;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.Domain.Model.Credits.Queries.Commands;
using Sc.Credits.Domain.Model.Credits.Queries.Reading;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Customers.Queries.Commands;
using Sc.Credits.Domain.Model.Customers.Queries.Reading;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.Domain.Model.Refinancings;
using Sc.Credits.Domain.Model.Refinancings.Gateway;
using Sc.Credits.Domain.Model.Refinancings.Queries.Reading;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Domain.Model.Stores.Queries.Reading;
using Sc.Credits.Domain.UseCase.Credits;
using Sc.Credits.Helpers.Commons.Extensions;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Sc.Credits.Domain.Managment.Services.Credits
{
    /// <summary>
    /// Cancel payment service is an implementation of <see cref="ICancelPaymentService"/>
    /// </summary>
    public class CancelPaymentService
        : ICancelPaymentService
    {
        private readonly ICreditMasterRepository _creditMasterRepository;
        private readonly IRequestCancelPaymentRepository _requestCancelPaymentRepository;
        private readonly ICancelUseCase _cancelUseCase;
        private readonly ICreditCommonsService _creditCommonsService;
        private readonly ICustomerService _customerService;
        private readonly IStoreService _storeService;
        private readonly IAppParametersService _appParametersService;
        private readonly CredinetAppSettings _credinetAppSettings;
        private readonly ILoggerService<CancelPaymentService> _loggerService;
        private readonly IRefinancingLogRepository _refinancingLogRepository;
        private readonly ITemplatesService _templatesService;

        /// <summary>
        /// Creates a new instance of <see cref="CancelPaymentService"/>
        /// </summary>
        /// <param name="creditCommons"></param>
        public CancelPaymentService(CreditCommons creditCommons,
                ILoggerService<CancelPaymentService> loggerService, 
                IRefinancingLogRepository refinancingLogRepository)
        {
            _creditMasterRepository = creditCommons.CreditMasterRepository;
            _requestCancelPaymentRepository = creditCommons.CancelCommons.RequestCancelPaymentRepository;
            _cancelUseCase = creditCommons.CancelCommons.CancelUseCase;
            _creditCommonsService = creditCommons.Service;
            _customerService = _creditCommonsService.CustomerService;
            _storeService = _creditCommonsService.StoreService;
            _appParametersService = _creditCommonsService.Commons.AppParameters;
            _credinetAppSettings = _creditCommonsService.Commons.CredinetAppSettings;
            _loggerService = loggerService;
            _refinancingLogRepository = refinancingLogRepository;
            _templatesService = _creditCommonsService.Commons.Templates;
        }

        /// <summary>
        /// <see cref="ICancelPaymentService.GetActiveAndPendingCancellationAsync(string, string, string)"/>
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public async Task<List<ActivePendingCancellationPaymentResponse>> GetActiveAndPendingCancellationAsync(string typeDocument, string idDocument,
            string storeId)
        {
            if (string.IsNullOrEmpty(storeId?.Trim()) || string.IsNullOrEmpty(typeDocument?.Trim()) || string.IsNullOrEmpty(idDocument?.Trim()))
            {
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);
            }

            Customer customer = await _customerService.GetAsync(idDocument, typeDocument, CustomerReadingFields.DocumentInfo);

            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            List<CreditMaster> customerActiveAndPendingCancelationCreditMasters =
                await _creditMasterRepository.GetActiveAndPendingCancellationPaymentsAsync(customer);

            if (!customerActiveAndPendingCancelationCreditMasters.Any())
            {
                throw new BusinessException(nameof(BusinessResponse.PaymentsNotFound), (int)BusinessResponse.PaymentsNotFound);
            }

            List<RequestCancelPayment> allRequestCancelPaymentsUndismissed =
                await _requestCancelPaymentRepository.GetUndismissedForMastersAsync(creditMasterIds:
                    customerActiveAndPendingCancelationCreditMasters.Select(item => item.Id).ToList(),
                    RequestCancelPaymentReadingFields.ActiveAndPendingCancellationPayments);

            List<ActivePendingCancellationPaymentResponse> activePendingCancellationPaymentResponses =
                _cancelUseCase.GetActiveAndPendingCancellationPayments(
                    new ActiveAndPendingCancellationPaymentsRequest(customerActiveAndPendingCancelationCreditMasters, allRequestCancelPaymentsUndismissed,
                        storeId),
                    parameters);

            if (activePendingCancellationPaymentResponses == null || !activePendingCancellationPaymentResponses.Any())
            {
                throw new BusinessException(nameof(BusinessResponse.PaymentsNotFound), (int)BusinessResponse.PaymentsNotFound);
            }

            return activePendingCancellationPaymentResponses;
        }

        /// <summary>
        /// <see cref="ICancelPaymentService.RequestAsync(CancelPaymentRequest)"/>
        /// </summary>
        /// <param name="cancelPaymentRequest"></param>
        /// <returns></returns>
        public async Task RequestAsync(CancelPaymentRequest cancelPaymentRequest)
        {
            if (cancelPaymentRequest == null || cancelPaymentRequest.PaymentId == Guid.Empty)
            {
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);
            }

            Store store = await _storeService.GetAsync(cancelPaymentRequest.StoreId, StoreReadingFields.BasicInfo);

            CreditMaster creditMaster = await _creditMasterRepository.GetWithTransactionsByCreditIdAsync(creditId: cancelPaymentRequest.PaymentId,
                fields: CreditMasterReadingFields.RequestCancelPayment, transactionFields: CreditReadingFields.RequestCancelPayment, customerFields: CustomerReadingFields.CreditLimit)
                ??
                throw new BusinessException(nameof(BusinessResponse.CreditsNotFound), (int)BusinessResponse.CreditsNotFound);

            Credit payment = creditMaster.GetPayment(cancelPaymentRequest.PaymentId)
                ??
                throw new BusinessException(nameof(BusinessResponse.PaymentsNotFound), (int)BusinessResponse.PaymentsNotFound);

            if (store.Id != payment.GetStoreId)
            {
                throw new BusinessException(nameof(BusinessResponse.invalidStoreId), (int)BusinessResponse.invalidStoreId);
            }

            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            int maximumDaysRequestCancellationPayments = parameters.MaximumDaysRequestCancellationPayments;

            if (!payment.AllowsRequestCancellation(maximumDaysRequestCancellationPayments))
            {
                throw new BusinessException(nameof(BusinessResponse.CancellationRequestNotAllowed), (int)BusinessResponse.CancellationRequestNotAllowed);
            }

            RequestCancelPayment requestCancelPayment = await _requestCancelPaymentRepository.GetUndismissedAsync(creditId: cancelPaymentRequest.PaymentId,
                RequestCancelPaymentReadingFields.RequestCancelPayment);

            if (requestCancelPayment != null)
            {
                throw new BusinessException(nameof(BusinessResponse.RequestCancelExists), (int)BusinessResponse.RequestCancelExists);
            }

            List<RequestCancelPayment> allRequestCancelPayments =
                await _requestCancelPaymentRepository.GetUndismissedForMasterAsync(creditMasterId: creditMaster.Id,
                    RequestCancelPaymentReadingFields.RequestCancelPayment);

            Credit cancelablePayment = creditMaster.GetCancelablePayment(allRequestCancelPayments);

            if (cancelablePayment == null || cancelablePayment.Id != payment.Id)
            {
                throw new BusinessException(nameof(BusinessResponse.PaymentIsNotCancelable), (int)BusinessResponse.PaymentIsNotCancelable);
            }

            requestCancelPayment = new RequestCancelPayment(payment.Id, payment.GetCreditMasterId, cancelPaymentRequest.UserName, store.Id,
                cancelPaymentRequest.Reason, (int)RequestStatuses.Pending, cancelPaymentRequest.UserId);

            await _requestCancelPaymentRepository.AddAsync(requestCancelPayment);

            MailNotificationRequest mailNotificationRequest = _cancelUseCase.GetMailNotification(message.RequestPayment, "Recibo", store, creditMaster);
            await _creditCommonsService.Commons.Notification.SendMailAsync(mailNotificationRequest, "RequestPaymentNotificationMail", handleError: true);

            string notificationTemplate = await _templatesService.GetAsync("PaymentCancellationRequest.txt");
            SmsNotificationRequest smsNotificationRecallRequest = _cancelUseCase.GetSmsNotification(notificationTemplate, store, creditMaster);
            await _creditCommonsService.Commons.Notification.SendSmsAsync(smsNotificationRecallRequest, _credinetAppSettings.SmsNotificationRequestName);
        }

        /// <summary>
        /// <see cref="ICancelPaymentService.GetPendingsAsync(string,int,int)"/>
        /// </summary>
        /// <param name="vendorId"></param>
        /// <returns></returns>
        public async Task<CancelPaymentDetailResponsePaged> GetPendingsAsync(string vendorId, int pageNumber, int valuePage, bool count)
        {
            if (string.IsNullOrEmpty(vendorId?.Trim()))
            {
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);
            }

            RequestCancelPaymentPaged pendings = await _requestCancelPaymentRepository.GetByVendorAsync(vendorId, pageNumber, valuePage, count,
                RequestStatuses.Pending, RequestCancelPaymentReadingFields.Pending, CreditMasterReadingFields.PendingCancellationPayments,
                    CreditReadingFields.PendingCancellationPayments, CustomerReadingFields.PendingCancellationPayments, StoreReadingFields.BasicInfo);

            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            int decimalNumbersRound = parameters.DecimalNumbersRound;

            return new CancelPaymentDetailResponsePaged
            {
                PaymentDetailResponses = pendings.RequestCancelPayment.Select(CreateCancelPaymentDetailResponse(decimalNumbersRound)).ToList(),
                TotalRecords = pendings.TotalRecords
            };
        }

        /// <summary>
        /// Create cancel payment detail response
        /// </summary>
        /// <param name="decimalNumbersRound"></param>
        /// <returns></returns>
        private Func<RequestCancelPayment, CancelPaymentDetailResponse> CreateCancelPaymentDetailResponse(int decimalNumbersRound)
        {
            return request =>
            {
                CreditMaster creditMaster = request.CreditMaster;
                Credit payment = creditMaster.GetPayment(request.GetCreditId);

                return new CancelPaymentDetailResponse
                {
                    CancellationRequestDate = request.GetDate,
                    CreditNumber = creditMaster.GetCreditNumber,
                    IdDocument = creditMaster.Customer.IdDocument,
                    TypeDocument = creditMaster.Customer.DocumentType,
                    PaymentDate = payment.GetTransactionDate,
                    PaymentId = payment.Id,
                    CreditId = creditMaster.Id,
                    PaymentNumber = payment.CreditPayment.GetPaymentNumber,
                    PaymentValue = payment.CreditPayment.GetTotalValuePaid.Round(decimalNumbersRound),
                    StoreId = payment.Store.Id,
                    StoreName = payment.Store.StoreName,
                    UserName = request.GetUserName,
                    UserId = request.GetUserId,
                    FullName = creditMaster.Customer.GetFullName,
                    Reason = request.GetReason
                };
            };
        }

        /// <summary>
        /// <see cref="ICancelPaymentService.CancelAsync(CancelPayments)"/>
        /// </summary>
        /// <param name="cancelPayment"></param>
        /// <returns></returns>
        public async Task CancelAsync(CancelPayments cancelPayments)
        {
            if (cancelPayments == null || cancelPayments.CreditId == Guid.Empty)
            {
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);
            }

            CreditMaster creditMaster = await _creditMasterRepository.GetWithTransactionsAsync(id: cancelPayments.CreditId,
                customerFields: CustomerReadingFields.CreditLimit, storeFields: StoreReadingFields.BasicInfo,
                transactionStoreFields: StoreReadingFields.BasicInfo)
                ??
                throw new BusinessException(nameof(BusinessResponse.CreditsNotFound), (int)BusinessResponse.CreditsNotFound);

            List<RequestCancelPayment> requestCancelPayments = await _requestCancelPaymentRepository.GetByStatusFromMasterAsync(creditMaster,
                RequestStatuses.Pending, RequestCancelPaymentReadingFields.CancelPayment, StoreReadingFields.BusinessGroupIdentifiers.Union(BusinessGroupReadingFields.PayCredit));

            if (!requestCancelPayments.Any())
            {
                throw new BusinessException(nameof(BusinessResponse.RequestCancelPaymentNotFound), (int)BusinessResponse.RequestCancelPaymentNotFound);
            }

            Store store = requestCancelPayments.FirstOrDefault().Store;

            List<RequestCancelPayment> requestCancelPaymentsCanceleds = await _requestCancelPaymentRepository.GetByStatusFromMasterAsync(creditMaster,
                RequestStatuses.Cancel, RequestCancelPaymentReadingFields.CancelPayment);

            List<RequestCancelPayment> allRequestCancels = requestCancelPayments.Union(requestCancelPaymentsCanceleds).ToList();

            List<RequestCancelPayment> successRequestCanceleds = await ApplyCancelPayments(creditMaster, requestCancelPayments, allRequestCancels,
                new UserInfo(cancelPayments.UserName, cancelPayments.UserId));

            if (!successRequestCanceleds.Any())
                throw new BusinessException(nameof(BusinessResponse.NoPaymentsWasCanceled), (int)BusinessResponse.NoPaymentsWasCanceled);

            foreach (RequestCancelPayment requestCancelPayment in successRequestCanceleds)
            {
                requestCancelPayment.Store.BusinessGroup = store.BusinessGroup;
                requestCancelPayment.CanceledPayment.SetIdLastPaymentCancelled(requestCancelPayment.GetCreditId);
                await _creditCommonsService.SendEventAsync(requestCancelPayment.CreditMaster, requestCancelPayment.CreditMaster.Customer, requestCancelPayment.Store,
                        requestCancelPayment.CanceledPayment);
            }

            await _creditCommonsService.SendEventAsync(creditMaster, creditMaster.Customer, store);

            MailNotificationRequest mailNotificationRequest = _cancelUseCase.GetMailNotification(message.ApprovalPayment, "Recibo", store, creditMaster);
            await _creditCommonsService.Commons.Notification.SendMailAsync(mailNotificationRequest, "ApprovalPaymentNotificationMail", handleError: true);

            string notificationTemplate = await _templatesService.GetAsync("ApprovalPaymentCancellation.txt");
            SmsNotificationRequest smsNotificationRecallRequest = _cancelUseCase.GetSmsNotification(notificationTemplate, store, creditMaster);
            await _creditCommonsService.Commons.Notification.SendSmsAsync(smsNotificationRecallRequest, _credinetAppSettings.SmsNotificationRequestName);
        }

        /// <summary>
        /// Apply cancel payments
        /// </summary>
        /// <param name="requestCancelPayments"></param>
        /// <param name="allRequestCancels"></param>
        /// <param name="creditMaster"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        private async Task<List<RequestCancelPayment>> ApplyCancelPayments(CreditMaster creditMaster, List<RequestCancelPayment> requestCancelPayments,
            List<RequestCancelPayment> allRequestCancels, UserInfo userInfo)
        {
            TransactionType cancelTransactionType = await _appParametersService.GetTransactionTypeAsync((int)TransactionTypes.CancelPayment);

            Status activeStatus = await _appParametersService.GetStatusAsync((int)Statuses.Active);

            return await _creditMasterRepository.ExcecuteOnTransactionAsync(
                CancelTransactionFunc(new CancelPaymentsTransactionRequest(creditMaster,
                        requestCancelPayments, allRequestCancels, userInfo),
                    cancelTransactionType, activeStatus));
        }

        /// <summary>
        /// <see cref="ICancelPaymentService.CancelTransactionFunc(CancelPaymentsTransactionRequest,
        /// TransactionType, Status)"/>
        /// </summary>
        /// <param name="cancelPaymentsTransactionRequest"></param>
        /// <param name="cancelTransactionType"></param>
        /// <param name="activeStatus"></param>
        /// <returns></returns>
        public Func<Transaction, Task<List<RequestCancelPayment>>> CancelTransactionFunc(CancelPaymentsTransactionRequest cancelPaymentsTransactionRequest,
            TransactionType cancelTransactionType, Status activeStatus)
        {
            List<RequestCancelPayment> requestCancelPayments = cancelPaymentsTransactionRequest.RequestCancelPayments;

            return async (transaction) =>
            {
                List<RequestCancelPayment> requestCanceleds = new List<RequestCancelPayment>();

                foreach (RequestCancelPayment requestCancelPayment in requestCancelPayments.OrderByDescending(request => request.Payment.GetTransactionDateComplete))
                {
                    bool thereAreSubsequentPayments = cancelPaymentsTransactionRequest.ThereAreSubsequentPayments(requestCancelPayment);

                    if (thereAreSubsequentPayments)
                    {
                        if (!requestCanceleds.Any())
                        {
                            throw new BusinessException(nameof(BusinessResponse.ThereAreSubsequentPayments), (int)BusinessResponse.ThereAreSubsequentPayments);
                        }
                        break;
                    }

                    await CancelPaymentAsync(new CancelPaymentDomainRequest(requestCancelPayment.CreditMaster,
                            requestCancelPayment.Payment, cancelTransactionType, activeStatus, cancelPaymentsTransactionRequest.UserInfo),
                        requestCancelPayment,
                        transaction);

                    requestCanceleds.Add(requestCancelPayment);
                }

                if (requestCanceleds.Any())
                {
                    Credit lastPaymentWithoutCancel = cancelPaymentsTransactionRequest.GetLastPaymentWithoutCancel(requestCanceleds);

                    TransactionType transactionTypeCancelPaymentUpdate = await _appParametersService.GetTransactionTypeAsync((int)TransactionTypes.CancelPaymentUpdate);

                    cancelPaymentsTransactionRequest.CreditMaster.SetLastRecord(lastPaymentWithoutCancel, transactionTypeCancelPaymentUpdate);

                    await _creditMasterRepository.AddTransactionAsync(cancelPaymentsTransactionRequest.CreditMaster,
                        CreditMasterCommandsFields.StatusUpdate, transaction);
                }

                return requestCanceleds;
            };
        }

        /// <summary>
        /// Cancel payment
        /// </summary>
        /// <param name="cancelPaymentDomainRequest"></param>
        /// <param name="requestCancelPayment"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private async Task CancelPaymentAsync(CancelPaymentDomainRequest cancelPaymentDomainRequest, RequestCancelPayment requestCancelPayment,
            Transaction transaction)
        {
            decimal balanceReleaseForRefinancing = 0;
            bool isRefinancing = RefinancingParams.IsAllowedSource(_credinetAppSettings, cancelPaymentDomainRequest.CancelPayment.GetSourceId);

            if (cancelPaymentDomainRequest.CancelPayment.IsPaid() && isRefinancing)
            {
                var parameters = _appParametersService.GetAppParametersAsync().Result;

                List<RefinancingLogDetail> refinancedCredits =
                    await _refinancingLogRepository.GetByStatusFromMasterAsync(cancelPaymentDomainRequest.CreditMaster.Id, RefinancingLogDetailReadingFields.BasicInfo);

                balanceReleaseForRefinancing =
                            refinancedCredits.Select(x => x.GetBalance.Round(parameters.DecimalNumbersRound)).Sum();

            }
            _cancelUseCase.CancelPayment(cancelPaymentDomainRequest, balanceReleaseForRefinancing, isRefinancing);

            await _creditMasterRepository.AddTransactionAsync(cancelPaymentDomainRequest.CreditMaster, CreditMasterCommandsFields.StatusUpdate,
                transaction);

            Customer customer = cancelPaymentDomainRequest.CreditMaster.Customer;

            if (customer.CreditLimitIsUpdated())
            {
                await _customerService.UpdateAsync(customer, CustomerCommandsFields.UpdateCreditLimit, transaction);
            }

            requestCancelPayment.SetCanceled(cancelPaymentDomainRequest.CreditMaster.Current, cancelPaymentDomainRequest.UserInfo.UserName,
                cancelPaymentDomainRequest.UserInfo.UserId);

            await _requestCancelPaymentRepository.UpdateAsync(requestCancelPayment, RequestCancelPaymentCommandsFields.CancelPayment,
                transaction);
        }

        /// <summary>
        /// <see cref="ICancelPaymentService.RejectAsync(CancelPayments)"/>
        /// </summary>
        /// <param name="cancelPaymentRequest"></param>
        /// <returns></returns>
        public async Task RejectAsync(CancelPayments cancelPaymentRequest)
         {
            if (cancelPaymentRequest == null || cancelPaymentRequest.CreditId == Guid.Empty)
            {
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);
            }

            List<RequestCancelPayment> requestCancelPayments = await _requestCancelPaymentRepository.GetByStatusFromMasterAsync(creditMasterId:
                cancelPaymentRequest.CreditId, RequestStatuses.Pending, RequestCancelPaymentReadingFields.Pending);

            if (!requestCancelPayments.Any())
            {
                throw new BusinessException(nameof(BusinessResponse.RequestCancelPaymentNotFound), (int)BusinessResponse.RequestCancelPaymentNotFound);
            }

            await RejectCancelPaymentsAsync(new UserInfo(cancelPaymentRequest.UserName, cancelPaymentRequest.UserId),
                requestCancelPayments);

            CreditMaster creditMaster = await _creditMasterRepository.GetWithTransactionsAsync(id: cancelPaymentRequest.CreditId,
              fields: CreditMasterReadingFields.CancelCreditReject, storeFields: StoreReadingFields.BasicInfo, customerFields: CustomerReadingFields.CreditLimit)
                 ??
                 throw new BusinessException(nameof(BusinessResponse.CreditsNotFound), (int)BusinessResponse.CreditsNotFound);

            MailNotificationRequest mailNotificationRequest = _cancelUseCase.GetMailNotification(message.RejectPayment, "Recibo", creditMaster.Store, creditMaster);
            await _creditCommonsService.Commons.Notification.SendMailAsync(mailNotificationRequest, "RejectPaymentNotificationMail", handleError: true);

            string notificationTemplate = await _templatesService.GetAsync("RejectPaymentCancellation.txt");
            SmsNotificationRequest smsNotificationRecallRequest = _cancelUseCase.GetSmsNotification(notificationTemplate, creditMaster.Store, creditMaster);
            await _creditCommonsService.Commons.Notification.SendSmsAsync(smsNotificationRecallRequest, _credinetAppSettings.SmsNotificationRequestName);
        }

        /// <summary>
        /// <see cref="ICancelPaymentService.RejectUnprocessedRequestAsync()"/>
        /// </summary>
        /// <returns></returns>
        public async Task RejectUnprocessedRequestAsync()
        {
            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            DateTime dateToRejectCancellation = DateTime.Today.AddDays(-(parameters.DaysElapsedToRejectARequestCancelPayment));

            List<RequestCancelPayment> requestCancelPayments = await _requestCancelPaymentRepository.GetByStatusUntilDate(dateToRejectCancellation,
                RequestStatuses.Pending);

            if (requestCancelPayments.Any())
            {
                IEnumerable<List<RequestCancelPayment>> paymentCancellationRequestsGroupedByCreditMasterId =
                                                            requestCancelPayments.GroupBy(x => x.GetCreditMasterId)
                                                                                 .Select(x => x.ToList());

                UserInfo userInfo = new UserInfo(_credinetAppSettings.UserToRejectCancellationRequests,
                                 _credinetAppSettings.UserIdToRejectCancellationRequests);

                foreach (List<RequestCancelPayment> creditMasterRequestCancelPayments in paymentCancellationRequestsGroupedByCreditMasterId)
                {
                    try
                    {
                        await RejectCancelPaymentsAsync(userInfo, creditMasterRequestCancelPayments);
                    }
                    catch (Exception ex)
                    {
                        string eventName = $"{_credinetAppSettings.DomainName}" +
                                           $".{ex.TargetSite.DeclaringType.Namespace}" +
                                           $".{ex.TargetSite.DeclaringType.Name}";

                        object logDetails = new { exception = ex.ToString(), body = creditMasterRequestCancelPayments.ToList() };

                        string idToLogError = creditMasterRequestCancelPayments.FirstOrDefault().GetCreditMasterId.ToString();

                        _loggerService.LogError(idToLogError, eventName, creditMasterRequestCancelPayments, ex);

                        await _loggerService.NotifyAsync(idToLogError, eventName, logDetails);
                    }
                }
            }
        }

        /// <summary>
        /// Rejects payment cancellation requests
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="requestCancelPayments"></param>
        /// <returns></returns>
        private async Task RejectCancelPaymentsAsync(UserInfo userInfo, List<RequestCancelPayment> requestCancelPayments)
        {
            await _creditMasterRepository.ExcecuteOnTransactionAsync(async (transaction) =>
            {
                foreach (RequestCancelPayment requestCancelPayment in requestCancelPayments)
                {
                    _cancelUseCase.UpdateStatusRequestPaymentCancel(RequestStatuses.Dismissed, requestCancelPayment,
                        userInfo);

                    await _requestCancelPaymentRepository.UpdateAsync(requestCancelPayment, RequestCancelPaymentCommandsFields.StatusUpdate,
                        transaction);
                }
            });
        }
    }
}