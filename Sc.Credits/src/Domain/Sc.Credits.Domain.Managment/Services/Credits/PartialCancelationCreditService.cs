using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.Domain.Model.Credits.Queries.Commands;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.Domain.UseCase.Credits;
using Sc.Credits.Helpers.Commons.Extensions;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Managment.Services.Credits
{
    /// <summary>
    /// Partial cancelation credit service is a implementation of <see cref="IPartialCancelationCreditService"/>
    /// </summary>
    public class PartialCancelationCreditService : IPartialCancelationCreditService
    {
        private readonly ICreditPaymentService _creditPaymentService;
        private readonly IAppParametersService _appParametersService;
        private readonly IRequestCancelCreditRepository _requestCancelCreditRepository;
        private readonly ICreditUsesCase _creditUsesCase;
        private readonly ICreditMasterRepository _creditMasterRepository;
        private readonly ICreditCommonsService _creditCommonsService;
        private readonly CredinetAppSettings _credinetAppSettings;
        private readonly ILoggerService<PartialCancelationCreditService> _loggerService;

        /// <summary>
        /// Creates a new instance of <see cref="PartialCancelationCreditService"/>
        /// </summary>
        /// <param name="creditCommons"></param>
        /// <param name="creditPaymentService"></param>
        /// <param name="loggerService"></param>
        public PartialCancelationCreditService(CreditCommons creditCommons, ICreditPaymentService creditPaymentService,
            ILoggerService<PartialCancelationCreditService> loggerService)
        {
            _creditPaymentService = creditPaymentService;
            _creditMasterRepository = creditCommons.CreditMasterRepository;
            _requestCancelCreditRepository = creditCommons.CancelCommons.RequestCancelCreditRepository;
            _creditUsesCase = creditCommons.CreditUsesCase;
            _creditCommonsService = creditCommons.Service;
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
                PaymentCreditRequest paymentCreditRequest = new PaymentCreditRequest(creditMaster.Id, requestCancelCredit.GetValueCancel.Value,
                                                                                     _credinetAppSettings.PartialCancelationStoreId,
                                                                                      userInfo.UserId, "", userInfo.UserName, "");
                _creditUsesCase.UpdateStatus((int)Statuses.Active, creditMaster);
                await _creditMasterRepository.UpdateAsync(creditMaster, CreditMasterCommandsFields.StatusUpdate);

                await _creditPaymentService.PayCreditAsync(paymentCreditRequest, true);

                requestCancelCredit.SetCanceled(userInfo.UserName, userInfo.UserId);

                await _requestCancelCreditRepository.UpdateAsync(requestCancelCredit, RequestCancelCreditCommandsFields.StatusUpdate);
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
            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            int decimalNumbersRound = parameters.DecimalNumbersRound;

            decimal valueToCancel = cancelCreditRequest.ValueCancel.Round(decimalNumbersRound);
            ValidateValueToPartialCancellation(creditMaster.Current.GetBalance, valueToCancel);

            return valueToCancel;
        }

        /// <summary>
        /// <see cref="ICancelCreditService.GetValidationToPartiallyCancelACreditAsync(PartialCancellationRequest)"/>
        /// </summary>
        /// <param name="vendorId"></param>
        /// <returns></returns>
        public async Task<bool> GetValidationToPartiallyCancelACreditAsync(PartialCancellationRequest partialCancellationRequest)
        {
            CreditMaster creditMaster = await _creditMasterRepository.GetWithCurrentAsync(id: partialCancellationRequest.CreditId)
                ??
                throw new BusinessException(nameof(BusinessResponse.CreditsNotFound), (int)BusinessResponse.CreditsNotFound);

            ValidateValueToPartialCancellation(creditMaster.Current.GetBalance, partialCancellationRequest.ValueCancel);

            return true;
        }

        /// <summary>
        /// Validate value to partial cancellation
        /// </summary>
        /// <param name="balance"></param>
        /// <param name="valueCancel"></param>
        private void ValidateValueToPartialCancellation(decimal balance, decimal valueCancel)
        {
            if (balance < valueCancel || valueCancel <= 0)
            {
                throw new BusinessException(nameof(BusinessResponse.PartialCancellationValueIsInvalid), (int)BusinessResponse.PartialCancellationValueIsInvalid);
            }
        }
    }
}