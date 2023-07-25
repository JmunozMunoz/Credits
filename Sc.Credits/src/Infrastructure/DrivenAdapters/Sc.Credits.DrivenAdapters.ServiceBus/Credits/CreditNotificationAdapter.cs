using org.reactivecommons.api;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.DrivenAdapters.ServiceBus.Common;
using Sc.Credits.DrivenAdapters.ServiceBus.Model;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.Commons.Messaging;
using Sc.Credits.Helpers.Commons.Settings;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Sc.Credits.DrivenAdapters.ServiceBus.Credits
{
    /// <summary>
    /// Credit notification adapter is an implementation of <see cref="ICreditNotificationRepository"/>.
    /// </summary>
    public class CreditNotificationAdapter
        : AsyncGatewayAdapterBase<CreditNotificationAdapter>, ICreditNotificationRepository
    {
        private readonly IDirectAsyncGateway<CreateCreditResponse> _directAsyncGatewayCreate;
        private readonly IDirectAsyncGateway<CreditEvent> _directAsyncGatewayEvent;
        private readonly ILoggerService<CreditNotificationAdapter> _loggerService;
        private readonly CredinetAppSettings _credinetAppSettings;

        /// <summary>
        /// Creates new instance of <see cref="CreditNotificationAdapter"/>.
        /// </summary>
        /// <param name="directAsyncGatewayCreate"></param>
        /// <param name="directAsyncGatewayEvent"></param>
        /// <param name="messagingLogger"></param>
        /// <param name="appSettings"></param>
        /// <param name="loggerService"></param>
        public CreditNotificationAdapter(IDirectAsyncGateway<CreateCreditResponse> directAsyncGatewayCreate,
            IDirectAsyncGateway<CreditEvent> directAsyncGatewayEvent,
            IMessagingLogger messagingLogger,
            ISettings<CredinetAppSettings> appSettings,
            ILoggerService<CreditNotificationAdapter> loggerService)
            : base(loggerService, messagingLogger)
        {
            _directAsyncGatewayCreate = directAsyncGatewayCreate;
            _directAsyncGatewayEvent = directAsyncGatewayEvent;
            _loggerService = loggerService;
            _credinetAppSettings = appSettings.Get();
        }

        /// <summary>
        /// <see cref="ICreditNotificationRepository.NotifyCreationAsync(CreateCreditResponse)"/>
        /// </summary>
        /// <param name="createCreditResponse"></param>
        /// <returns></returns>
        public async Task NotifyCreationAsync(CreateCreditResponse createCreditResponse) =>
           await HandleSendCommandAsync(_directAsyncGatewayCreate, createCreditResponse.CreditId.ToString(),
               createCreditResponse, _credinetAppSettings.CreateCreditNotificationQueue, commandName: nameof(NotifyCreationAsync),
               MethodBase.GetCurrentMethod(),
               _credinetAppSettings.MaxRetryAttemptsCreateCreditNotificationRequest);

        /// <summary>
        /// <see cref="ICreditNotificationRepository.SendEventAsync(CreditMaster, Customer, Store,
        /// Credit, TransactionType, string)"/>
        /// </summary>
        /// <returns></returns>
        public async Task SendEventAsync(CreditMaster creditMaster, Customer customer, Store store, Credit credit = null,
            TransactionType transactionType = null, string complementEventName = null)
        {
            if (TryGetCreditEvent(creditMaster, credit, customer, store, transactionType, out CreditEvent creditEvent))
            {
                string eventName = string.IsNullOrEmpty(complementEventName)
                    ?
                    $"Credit.TransactionType({creditEvent.TransactionTypeName})"
                    :
                    $"Credit.TransactionType({creditEvent.TransactionTypeName})_{complementEventName}";

                await HandleSendEventAsync(_directAsyncGatewayEvent, creditEvent.TransactionId.ToString(),
                   creditEvent, _credinetAppSettings.CreditTopicName, eventName,
                   MethodBase.GetCurrentMethod());
            }
        }

        /// <summary>
        /// Try to get the credit event, based on domain info.
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <param name="credit"></param>
        /// <param name="transactionType"></param>
        /// <returns></returns>
        private bool TryGetCreditEvent(CreditMaster creditMaster, Credit credit, Customer customer, Store store,
            TransactionType transactionType, out CreditEvent creditEvent)
        {
            try
            {
                creditEvent = new CreditEvent();

                _loggerService.Debug(credit?.Id.ToString(), credit, message: "Starts TryGetCreditEvent.", MethodBase.GetCurrentMethod());

                credit = credit ?? creditMaster.Current;

                _loggerService.Debug(credit.Id.ToString(), credit, message: "Starts CreditEvent reflection map.", MethodBase.GetCurrentMethod());

                ReflectionUtils.MapPrivateFeldsToPublic(creditMaster, creditEvent);
                ReflectionUtils.MapPrivateFeldsToPublic(credit, creditEvent);
                ReflectionUtils.MapPrivateFeldsToPublic(credit.CreditPayment, creditEvent);

                _loggerService.Debug(credit.Id.ToString(), credit, message: "Ends CreditEvent reflection map.", MethodBase.GetCurrentMethod());

                if (credit.IdLastPaymentCancelled != Guid.Empty)
                {
                    creditEvent.IdLastPaymentCancelled = credit.IdLastPaymentCancelled;
                }

                creditEvent.CustomerDocumentId = customer.IdDocument;
                creditEvent.CustomerDocumentType = customer.DocumentType;
                creditEvent.AvailableCreditLimit = customer.GetAvailableCreditLimit;
                creditEvent.StoreName = store?.StoreName;
                creditEvent.TransactionTypeId = transactionType?.Id ?? credit.CreditPayment.GetTransactionTypeId;
                creditEvent.TransactionTypeName = transactionType?.Name ?? credit.CreditPayment.GetTransactionTypeName;

                creditEvent.TransactionId = credit.Id;

                creditEvent.BusinessGroupName = store?.BusinessGroup?.Name ?? "";
                creditEvent.Nit = store?.GetNit;
                creditEvent.SourceCreditStore = creditMaster.Store?.StoreName;
                creditEvent.TransactionStore = store?.StoreName;          
                creditEvent.VendorId = store?.GetVendorId;          
            }
            catch (Exception ex)
            {
                creditEvent = null;
                _loggerService.LogError(credit?.Id.ToString(), credit, ex, MethodBase.GetCurrentMethod());

                return false;
            }

            return true;
        }
    }
}