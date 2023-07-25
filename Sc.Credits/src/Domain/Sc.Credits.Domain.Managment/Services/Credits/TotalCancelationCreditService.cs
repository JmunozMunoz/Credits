using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Customers;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.Domain.Model.Credits.Queries.Commands;
using Sc.Credits.Domain.Model.Credits.Queries.Reading;
using Sc.Credits.Domain.Model.Customers.Queries.Commands;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.UseCase.Credits;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Managment.Services.Credits
{
    /// <summary>
    /// Total cancelation credit service is a implementation of <see cref="ITotalCancelationCreditService"/>
    /// </summary>
    public class TotalCancelationCreditService : ITotalCancelationCreditService
    {
        private readonly ICreditMasterRepository _creditMasterRepository;
        private readonly IRequestCancelPaymentRepository _requestCancelPaymentRepository;
        private readonly IRequestCancelCreditRepository _requestCancelCreditRepository;
        private readonly ICancelUseCase _cancelUseCase;
        private readonly ICreditCommonsService _creditCommonsService;
        private readonly ICustomerService _customerService;
        private readonly IAppParametersService _appParametersService;
        private readonly CredinetAppSettings _credinetAppSettings;
        private readonly ILoggerService<TotalCancelationCreditService> _loggerService;

        /// <summary>
        /// Creat a new instance of <see cref="TotalCancelationCreditService"/>
        /// </summary>
        /// <param name="creditCommons"></param>
        /// <param name="loggerService"></param>
        public TotalCancelationCreditService(CreditCommons creditCommons, ILoggerService<TotalCancelationCreditService> loggerService)
        {
            _creditMasterRepository = creditCommons.CreditMasterRepository;
            _requestCancelCreditRepository = creditCommons.CancelCommons.RequestCancelCreditRepository;
            _cancelUseCase = creditCommons.CancelCommons.CancelUseCase;
            _requestCancelPaymentRepository = creditCommons.CancelCommons.RequestCancelPaymentRepository;
            _creditCommonsService = creditCommons.Service;
            _customerService = _creditCommonsService.CustomerService;
            _appParametersService = _creditCommonsService.Commons.AppParameters;
            _credinetAppSettings = _creditCommonsService.Commons.CredinetAppSettings;
            _loggerService = loggerService;
        }

        /// <summary>
        /// <see cref="ICancellationStrategy.ApproveCancellationAsync(UserInfo, RequestCancelCredit, CreditMaster)"/>
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="requestCancelCredit"></param>
        /// <param name="creditMaster"></param>
        /// <returns></returns>
        public async Task ApproveCancellationAsync(UserInfo userInfo, RequestCancelCredit requestCancelCredit, CreditMaster creditMaster)
        {
            try
            {
                List<RequestCancelPayment> requestCancelPaymentsCanceled =
                    await _requestCancelPaymentRepository.GetByStatusFromMasterAsync(creditMaster,
                        RequestStatuses.Cancel, RequestCancelPaymentReadingFields.CancelCredit);

                if (creditMaster.HasPayments(requestCancelPaymentsCanceled))
                {
                    throw new BusinessException(nameof(BusinessResponse.InvalidCreditCancelHasPayments), (int)BusinessResponse.InvalidCreditCancelHasPayments);
                }

                TransactionType cancelCreditTansactionType = await _appParametersService.GetTransactionTypeAsync((int)TransactionTypes.CancelCredit);

                Status canceledStatus = await _appParametersService.GetStatusAsync((int)Statuses.Canceled);

                _cancelUseCase.CancelCredit(new CancelCreditDomainRequest(creditMaster, cancelCreditTansactionType, canceledStatus, userInfo));

                await _creditMasterRepository.AddTransactionAsync(creditMaster, CreditMasterCommandsFields.StatusUpdate);

                if (creditMaster.Customer.CreditLimitIsUpdated())
                {
                    await _customerService.UpdateAsync(creditMaster.Customer, CustomerCommandsFields.UpdateCreditLimit);
                }

                requestCancelCredit.SetCanceled(userInfo.UserName, userInfo.UserId);

                await _requestCancelCreditRepository.UpdateAsync(requestCancelCredit, RequestCancelCreditCommandsFields.StatusUpdate);

                await _creditCommonsService.SendEventAsync(creditMaster, creditMaster.Customer, requestCancelCredit.Store);
            }
            catch (Exception ex)
            {
                string eventName = $"{_credinetAppSettings.DomainName}" +
                                   $".{ex.TargetSite.DeclaringType.Namespace}" +
                                   $".{ex.TargetSite.DeclaringType.Name}";

                var cancellationInformation = new
                {
                    CustomerId = creditMaster.Customer.Id,
                    StoreName = creditMaster.Store.StoreName,
                    StoreId = creditMaster.Store.Id,
                    CancellationType = requestCancelCredit.GetCancellationType,
                    ValueCancel = requestCancelCredit.GetValueCancel,
                    CreditMasterId = creditMaster.Id,
                    UserName = userInfo.UserName,
                    UserId = userInfo.UserId
                };

                _loggerService.LogError(creditMaster.Id.ToString(), eventName,
                    cancellationInformation, ex);

                await _loggerService.NotifyAsync(creditMaster.Id.ToString(),
                    eventName, cancellationInformation);
            }
        }

        public async Task<decimal> ValidateCancellationRequest(CancelCreditRequest cancelCreditRequest, CreditMaster creditMaster)
        {
            RequestCancelCredit requestCancelCreditCanceled = await _requestCancelCreditRepository.GetByStatusAsync(creditMasterId: cancelCreditRequest.CreditId,
            RequestStatuses.Cancel);

            if (requestCancelCreditCanceled != null)
            {
                throw new BusinessException(nameof(BusinessResponse.RequestCancelExists), (int)BusinessResponse.RequestCancelExists);
            }

            List<RequestCancelPayment> requestCancelPaymentsCanceled =
                    await _requestCancelPaymentRepository.GetByStatusFromMasterAsync(creditMaster,
                        RequestStatuses.Cancel, RequestCancelPaymentReadingFields.RequestCancelCredit);

            if (creditMaster.HasPayments(requestCancelPaymentsCanceled))
            {
                throw new BusinessException(nameof(BusinessResponse.InvalidCreditCancelHasPayments), (int)BusinessResponse.InvalidCreditCancelHasPayments);
            }
            return creditMaster.Current.GetBalance;
        }
    }
}