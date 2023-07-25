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
using Sc.Credits.Domain.Model.Customers.Queries.Reading;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Parameters;
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

namespace Sc.Credits.Domain.Managment.Services.Credits
{
    /// <summary>
    /// Cancel credit service is an implementation of <see cref="ICancelCreditService"/>
    /// </summary>
    public class CancelCreditService
        : ICancelCreditService
    {
        private readonly ICancelCreditContextService _cancelCreditContextService;

        private readonly ICreditMasterRepository _creditMasterRepository;
        private readonly IRequestCancelCreditRepository _requestCancelCreditRepository;
        private readonly IRequestCancelPaymentRepository _requestCancelPaymentRepository;
        private readonly ICancelUseCase _cancelUseCase;
        private readonly ICreditUsesCase _creditUsesCase;
        private readonly ICreditCommonsService _creditCommonsService;
        private readonly ICustomerService _customerService;
        private readonly IStoreService _storeService;
        private readonly IAppParametersService _appParametersService;
        private readonly CredinetAppSettings _credinetAppSettings;
        private readonly ILoggerService<CancelCreditService> _loggerService;
        private readonly ITemplatesService _templatesService;

        /// <summary>
        /// Creates a new instance of <see cref="CancelCreditService"/>
        /// </summary>
        /// <param name="creditCommons"></param>
        public CancelCreditService(CreditCommons creditCommons, ILoggerService<CancelCreditService> loggerService, ICancelCreditContextService cancelCreditContextService)
        {
            _cancelCreditContextService = cancelCreditContextService;
            _creditMasterRepository = creditCommons.CreditMasterRepository;
            _requestCancelCreditRepository = creditCommons.CancelCommons.RequestCancelCreditRepository;
            _requestCancelPaymentRepository = creditCommons.CancelCommons.RequestCancelPaymentRepository;
            _cancelUseCase = creditCommons.CancelCommons.CancelUseCase;
            _creditUsesCase = creditCommons.CreditUsesCase;
            _creditCommonsService = creditCommons.Service;
            _customerService = _creditCommonsService.CustomerService;
            _storeService = _creditCommonsService.StoreService;
            _appParametersService = _creditCommonsService.Commons.AppParameters;
            _credinetAppSettings = _creditCommonsService.Commons.CredinetAppSettings;
            _loggerService = loggerService;
            _templatesService = _creditCommonsService.Commons.Templates;
        }

        /// <summary>
        /// <see cref="ICancelCreditService.GetActiveAndPendingCancellationAsync(string, string, string)"/>
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        /// <param name="vendorId"></param>
        /// <returns></returns>
        public async Task<List<ActivePendingCancellationCreditResponse>> GetActiveAndPendingCancellationAsync(string typeDocument, string idDocument,
            string vendorId)
        {
            if (string.IsNullOrWhiteSpace(vendorId?.Trim()) || string.IsNullOrWhiteSpace(typeDocument?.Trim()) || string.IsNullOrWhiteSpace(idDocument?.Trim()))
            {
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);
            }

            Customer customer = await _customerService.GetAsync(idDocument, typeDocument,
                CustomerReadingFields.ActiveAndPendingCancellationCredits);

            List<CreditMaster> activeAndPendingCancelationCredits =
                await _creditMasterRepository.GetActiveAndPendingCancellationCreditsAsync(customer, vendorId,
                    CreditReadingFields.ActiveAndPendingCancellationCredits, StoreReadingFields.ActiveAndPendingCancellationCredits);

            if (!activeAndPendingCancelationCredits.Any())
            {
                throw new BusinessException(nameof(BusinessResponse.CreditsNotFound), (int)BusinessResponse.CreditsNotFound);
            }

            List<RequestCancelPayment> requestCancelPaymentsCanceled =
                await _requestCancelPaymentRepository.GetFromMastersAsync(creditMasterIds:
                    activeAndPendingCancelationCredits
                        .Select(item => item.Id)
                        .ToList(),
                    RequestCancelPaymentReadingFields.ActiveAndPendingCancellationCredits);

            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            List<ActivePendingCancellationCreditResponse> activePendingCancellationCreditResponses =
                _cancelUseCase
                    .GetActiveAndPendingCancellationCredits(new ActiveAndPendingCancellationCreditsRequest(activeAndPendingCancelationCredits,
                        requestCancelPaymentsCanceled),
                    parameters);

            return activePendingCancellationCreditResponses;
        }

        /// <summary>
        /// <see cref="ICancelCreditService.RequestAsync(CancelCreditRequest)"/>
        /// </summary>
        /// <param name="cancelCreditRequest"></param>
        /// <returns></returns>
        public async Task RequestAsync(CancelCreditRequest cancelCreditRequest)
        {
            if (cancelCreditRequest == null || cancelCreditRequest.CreditId == Guid.Empty
                 || string.IsNullOrEmpty(Enum.GetName(typeof(CancellationTypes), cancelCreditRequest.CancellationType)))
            {
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);
            }

            Store store = await _storeService.GetAsync(cancelCreditRequest.StoreId, StoreReadingFields.CreditsInCancellationRequest);

            CreditMaster creditMaster = await _creditMasterRepository.GetWithTransactionsAsync(id: cancelCreditRequest.CreditId,
                fields: CreditMasterReadingFields.RequestCancelCredit, transactionFields: CreditReadingFields.RequestCancelCredit,
                customerFields: CustomerReadingFields.CreditLimit, storeFields: StoreReadingFields.CreditsInCancellationRequest, transactionStoreFields: StoreReadingFields.BasicInfo)
                ??
                throw new BusinessException(nameof(BusinessResponse.CreditsNotFound), (int)BusinessResponse.CreditsNotFound);

            if (store.GetVendorId != creditMaster.Store.GetVendorId)
            {
                throw new BusinessException(nameof(BusinessResponse.invalidVendor), (int)BusinessResponse.invalidVendor);
            }

            RequestCancelCredit requestCancelCredit = await _requestCancelCreditRepository.GetByStatusAsync(creditMasterId: cancelCreditRequest.CreditId,
                RequestStatuses.Pending);

            if (requestCancelCredit != null)
            {
                throw new BusinessException(nameof(BusinessResponse.RequestCancelExists), (int)BusinessResponse.RequestCancelExists);
            }

            decimal valueToPartialCancellation = await _cancelCreditContextService.ValidateCancellationRequest(cancelCreditRequest, creditMaster);

            requestCancelCredit = new RequestCancelCredit(creditMaster.Id, cancelCreditRequest.UserName, store.Id, cancelCreditRequest.Reason,
                (int)RequestStatuses.Pending, cancelCreditRequest.UserId, cancelCreditRequest.CancellationType, valueToPartialCancellation);

            await _requestCancelCreditRepository.AddAsync(requestCancelCredit);

            _creditUsesCase.UpdateStatus((int)Statuses.CancelRequest, creditMaster);

            await _creditMasterRepository.UpdateAsync(creditMaster, CreditMasterCommandsFields.StatusUpdate);

            MailNotificationRequest mailNotificationRequest = _cancelUseCase.GetMailNotification(message.RequestCredit, "Credito", creditMaster.Store, creditMaster);
            await _creditCommonsService.Commons.Notification.SendMailAsync(mailNotificationRequest, "RequestCancellationCreditNotificationMail", handleError: true);

            string notificationTemplate = await _templatesService.GetAsync("CreditCancellationRequest.txt");
            SmsNotificationRequest smsNotificationRecallRequest = _cancelUseCase.GetSmsNotification(notificationTemplate, creditMaster.Store, creditMaster);
            await _creditCommonsService.Commons.Notification.SendSmsAsync(smsNotificationRecallRequest, _credinetAppSettings.SmsNotificationRequestName);
        }

        /// <summary>
        /// <see cref="ICancelCreditService.GetPendingsAsync(string,int,int)"/>
        /// </summary>
        /// <param name="vendorId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="valuePage"></param>
        /// <returns></returns>
        public async Task<CancelCreditDetailResponsePaged> GetPendingsAsync(string vendorId, int pageNumber, int valuePage, bool count)
        {
            if (string.IsNullOrEmpty(vendorId?.Trim()))
            {
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);
            }

            RequestCancelCreditPaged pendings = await _requestCancelCreditRepository.GetByVendorAsync(vendorId, pageNumber, valuePage, count,
                RequestStatuses.Pending, RequestCancelCreditReadingFields.Pending, CreditMasterReadingFields.PendingCancellationCredits,
                    CreditReadingFields.PendingCancellationCredits, CustomerReadingFields.PendingCancellationCredits, StoreReadingFields.BasicInfo);

            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            int decimalNumbersRound = parameters.DecimalNumbersRound;


            return new CancelCreditDetailResponsePaged
            {
                CreditDetailResponses = pendings.RequestCancelCredit.Select(CreateCancelCreditDetailResponse(decimalNumbersRound)).ToList(),
                TotalRecords = pendings.TotalRecords
            };
        }

        /// <summary>
        /// Create cancel credit detail response.
        /// </summary>
        /// <param name="decimalNumbersRound"></param>
        /// <returns></returns>
        private Func<RequestCancelCredit, CancelCreditDetailResponse> CreateCancelCreditDetailResponse(int decimalNumbersRound)
        {
            return request =>
            {
                CreditMaster creditMaster = request.CreditMaster;
                Credit credit = request.CreditMaster.Current;

                return new CancelCreditDetailResponse
                {
                    IdDocument = creditMaster.Customer.IdDocument,
                    TypeDocument = creditMaster.Customer.DocumentType,
                    CreditId = creditMaster.Id,
                    CreditNumber = creditMaster.GetCreditNumber,
                    StoreId = creditMaster.Store.Id,
                    StoreName = creditMaster.Store.StoreName,
                    UserName = request.GetUserName,
                    CancellationRequestDate = request.GetDate,
                    CreateDate = creditMaster.GetCreditDate,
                    Reason = request.GetReason,
                    Balance = credit.GetBalance.Round(decimalNumbersRound),
                    FullName = creditMaster.Customer.GetFullName,
                    IdRequestCancelCredit = request.Id,
                    Payments = new List<CancelCreditPaymentResponse>(),
                    UserId = request.GetUserId,
                    CancellationType = Enum.GetName(typeof(CancellationTypes), request.GetCancellationType),
                    ValueCancel = request.GetValueCancel,
                };
            };
        }

        /// <summary>
        /// <see cref="ICancelCreditService.CancelAsync(CancelCredit)"/>
        /// </summary>
        /// <param name="cancelCredit"></param>
        /// <returns></returns>
        public async Task CancelAsync(CancelCredit cancelCredit)
        {
            if (cancelCredit == null || cancelCredit.CreditId == Guid.Empty)
            {
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);
            }

            RequestCancelCredit requestCancelCredit = await _requestCancelCreditRepository.GetByStatusAsync(creditMasterId: cancelCredit.CreditId,
                RequestStatuses.Pending, StoreReadingFields.VendorIdentifier, BusinessGroupReadingFields.PayCredit)
                ??
                throw new BusinessException(nameof(BusinessResponse.RequestCancelCreditNotFound), (int)BusinessResponse.RequestCancelCreditNotFound);

            CreditMaster creditMaster = await _creditMasterRepository.GetWithTransactionsAsync(id: cancelCredit.CreditId,
                storeFields: StoreReadingFields.BasicInfo, customerFields: CustomerReadingFields.CreditLimit)
                ??
                throw new BusinessException(nameof(BusinessResponse.CreditsNotFound), (int)BusinessResponse.CreditsNotFound);

            await _cancelCreditContextService.ApproveCancellationAsync(new UserInfo(cancelCredit.UserName, cancelCredit.UserId), requestCancelCredit, creditMaster);

            MailNotificationRequest mailNotificationRequest = _cancelUseCase.GetMailNotification(message.ApprovalCredit, "Credito", creditMaster.Store, creditMaster);
            await _creditCommonsService.Commons.Notification.SendMailAsync(mailNotificationRequest, "ApprovalCancellationCreditNotificationMail", handleError: true);

            string notificationTemplate = await _templatesService.GetAsync("ApprovalCreditCancellation.txt");
            SmsNotificationRequest smsNotificationRecallRequest = _cancelUseCase.GetSmsNotification(notificationTemplate, creditMaster.Store, creditMaster);
            await _creditCommonsService.Commons.Notification.SendSmsAsync(smsNotificationRecallRequest, _credinetAppSettings.SmsNotificationRequestName);
        }

        /// <summary>
        /// <see cref="ICancelCreditService.RejectAsync(CancelCredit)"/>
        /// </summary>
        /// <param name="creditId"></param>
        /// <returns></returns>
        public async Task RejectAsync(CancelCredit cancelCredit)
        {
            if (cancelCredit == null || cancelCredit.CreditId == Guid.Empty)
            {
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);
            }

            RequestCancelCredit requestCancelCredit =
                 await _requestCancelCreditRepository.GetByStatusAsync(creditMasterId: cancelCredit.CreditId,
                     RequestStatuses.Pending)
                     ??
                     throw new BusinessException(nameof(BusinessResponse.RequestCancelCreditNotFound), (int)BusinessResponse.RequestCancelCreditNotFound);

            CreditMaster creditMaster = await _creditMasterRepository.GetWithTransactionsAsync(id: cancelCredit.CreditId,
                fields: CreditMasterReadingFields.CancelCreditReject, storeFields: StoreReadingFields.BasicInfo, customerFields: CustomerReadingFields.CreditLimit)
                   ??
                   throw new BusinessException(nameof(BusinessResponse.CreditsNotFound), (int)BusinessResponse.CreditsNotFound);

            await RejectCancelCreditAsync(requestCancelCredit, creditMaster, new UserInfo(cancelCredit.UserName, cancelCredit.UserId));

            MailNotificationRequest mailNotificationRequest = _cancelUseCase.GetMailNotification(message.RejectCredit, "Credito", creditMaster.Store, creditMaster);
            await _creditCommonsService.Commons.Notification.SendMailAsync(mailNotificationRequest, "RejectCancellationCreditNotificationMail", handleError: true);

            string notificationTemplate = await _templatesService.GetAsync("RejectCreditCancellation.txt");
            SmsNotificationRequest smsNotificationRecallRequest = _cancelUseCase.GetSmsNotification(notificationTemplate, creditMaster.Store, creditMaster);
            await _creditCommonsService.Commons.Notification.SendSmsAsync(smsNotificationRecallRequest, _credinetAppSettings.SmsNotificationRequestName);
        }

        /// <summary>
        /// <see cref="ICancelCreditService.RejectUnprocessedRequestAsync()"/>
        /// </summary>
        /// <returns></returns>
        public async Task RejectUnprocessedRequestAsync()
        {
            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            DateTime dateToRejectCancellation = DateTime.Today.AddDays(-(parameters.DaysElapsedToRejectARequestCancelCredit));

            List<RequestCancelCredit> requestCancelCreditsToReject =
                await _requestCancelCreditRepository.GetByStatusUntilDateAsync(cancellationRequestDate: dateToRejectCancellation,
                    RequestStatuses.Pending);

            if (requestCancelCreditsToReject.Any())
            {
                UserInfo userInfo = new UserInfo(_credinetAppSettings.UserToRejectCancellationRequests,
                                                 _credinetAppSettings.UserIdToRejectCancellationRequests);
                foreach (RequestCancelCredit requestCancelCredit in requestCancelCreditsToReject)
                {
                    try
                    {
                        CreditMaster creditMaster =
                                    await _creditMasterRepository.GetByIdAsync(requestCancelCredit.GetCreditMasterId,
                                          CreditMasterReadingFields.CancelCreditReject);
                        await RejectCancelCreditAsync(requestCancelCredit, creditMaster, userInfo);
                    }
                    catch (Exception ex)
                    {
                        string eventName = $"{_credinetAppSettings.DomainName}" +
                                           $".{ex.TargetSite.DeclaringType.Namespace}" +
                                           $".{ex.TargetSite.DeclaringType.Name}";

                        object logDetails = new { exception = ex.ToString(), body = requestCancelCredit };

                        _loggerService.LogError(requestCancelCredit.GetCreditMasterId.ToString(), eventName,
                            requestCancelCredit, ex);

                        await _loggerService.NotifyAsync(requestCancelCredit.GetCreditMasterId.ToString(),
                            eventName, logDetails);
                    }
                }
            }
        }

        /// <summary>
        /// Rejects credit cancellation requests
        /// </summary>
        /// <param name="requestCancelCredit"></param>
        /// <param name="creditMaster"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        private async Task RejectCancelCreditAsync(RequestCancelCredit requestCancelCredit, CreditMaster creditMaster, UserInfo userInfo)
        {
            await _creditMasterRepository.ExcecuteOnTransactionAsync(async (transaction) =>
            {
                _creditUsesCase.UpdateStatus((int)Statuses.Active, creditMaster);

                await _creditMasterRepository.UpdateAsync(creditMaster, CreditMasterCommandsFields.StatusUpdate);

                _cancelUseCase.UpdateStatusRequestCreditCancel(RequestStatuses.Dismissed, requestCancelCredit, userInfo);

                await _requestCancelCreditRepository.UpdateAsync(requestCancelCredit, RequestCancelCreditCommandsFields.StatusUpdate);
            });
        }
    }
}