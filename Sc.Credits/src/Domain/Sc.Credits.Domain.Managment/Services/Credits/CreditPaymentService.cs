using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Customers;
using Sc.Credits.Domain.Managment.Services.Stores;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.Domain.Model.Credits.Queries.Reading;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Customers.Queries.Reading;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.Domain.Model.Refinancings;
using Sc.Credits.Domain.Model.Refinancings.Gateway;
using Sc.Credits.Domain.Model.Refinancings.Queries;
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
    /// Credit payment service is an implementation of <see cref="ICreditPaymentService"/>
    /// </summary>
    public class CreditPaymentService
        : ICreditPaymentService
    {
        private readonly ICreditMasterRepository _creditMasterRepository;
        private readonly IRequestCancelPaymentRepository _requestCancelPaymentRepository;
        private readonly ICreditPaymentUsesCase _creditPaymentUsesCase;
        private readonly ICreditUsesCase _creditUsesCase;
        private readonly ICreditCommonsService _creditCommonsService;
        private readonly IAppParametersService _appParametersService;
        private readonly ICustomerService _customerService;
        private readonly IStoreService _storeService;
        private readonly INotificationService _notificationService;
        private readonly ITemplatesService _templatesService;
        private readonly IRefinancingLogRepository _refinancingLogRepository;
        private readonly CredinetAppSettings _credinetAppSettings;
        private readonly ICreditPaymentEventsRepository _creditPaymentEventsRepository;
        private readonly ILoggerService<CreditPaymentService> _loggerService;


        private const string activeCreditsEventName = "Credits.ActiveCreditsEvents";
        private const string payCreditsEventName = "Credits.PayCreditsEvents";

        /// <summary>
        /// Creates a new instance of <see cref="CreditPaymentService"/>
        /// </summary>
        /// <param name="creditCommons"></param>
        public CreditPaymentService(CreditCommons creditCommons, ICreditPaymentEventsRepository creditPaymentEventsRepository,
            ILoggerService<CreditPaymentService> loggerService, IRefinancingLogRepository refinancingLogRepository)
        {
            _creditCommonsService = creditCommons.Service;
            _creditMasterRepository = creditCommons.CreditMasterRepository;
            _requestCancelPaymentRepository = creditCommons.CancelCommons.RequestCancelPaymentRepository;
            _creditPaymentUsesCase = creditCommons.PaymentCommons.CreditPaymentUsesCase;
            _creditUsesCase = creditCommons.CreditUsesCase;
            _creditCommonsService = creditCommons.Service;
            _appParametersService = _creditCommonsService.Commons.AppParameters;
            _customerService = _creditCommonsService.CustomerService;
            _storeService = _creditCommonsService.StoreService;
            _notificationService = _creditCommonsService.Commons.Notification;
            _templatesService = _creditCommonsService.Commons.Templates;
            _credinetAppSettings = _creditCommonsService.Commons.CredinetAppSettings;
            _creditPaymentEventsRepository = creditPaymentEventsRepository;
            _loggerService = loggerService;
            _refinancingLogRepository = refinancingLogRepository;
        }

        #region ICreditPaymentService Members

        /// <summary>
        /// <see cref="ICreditPaymentService.PayCreditAsync(PaymentCreditRequest, bool,
        /// AppParameters, Transaction)"/>
        /// </summary>
        /// <param name="paymentCreditRequest"></param>
        /// <param name="notify"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<PaymentCreditResponse> PayCreditAsync(PaymentCreditRequest paymentCreditRequest, bool notify,
            AppParameters parameters = null, Transaction transaction = null, bool simulation = false) =>
            await PayCreditAsync(new PaymentCreditRequestComplete(paymentCreditRequest), notify, parameters, transaction, simulation);

        /// <summary>
        /// <see cref="ICreditPaymentService.PayCreditAsync(PaymentCreditRequestComplete, bool,
        /// AppParameters, Transaction)"/>
        /// </summary>
        /// <param name="paymentCreditRequest"></param>
        /// <param name="notify"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<PaymentCreditResponse> PayCreditAsync(PaymentCreditRequestComplete paymentCreditRequest, bool notify,
                    AppParameters parameters = null, Transaction transaction = null, bool simulation = false)
        {
            if (parameters == null)
            {
                parameters = await _appParametersService.GetAppParametersAsync();
            }

            ValidatePaymentCreditRequest(paymentCreditRequest);

            Guid creditMasterId = paymentCreditRequest.CreditId;

            CreditMaster creditMaster =
                await _creditMasterRepository.GetWithCurrentAsync(creditMasterId)
                ??
                throw new BusinessException(nameof(BusinessResponse.CreditsNotFound), (int)BusinessResponse.CreditsNotFound);

            Store creditMasterStore = await _storeService.GetAsync(creditMaster.GetStoreId, StoreReadingFields.BusinessGroupIdentifiers,
                loadAssuranceCompany: true);

            creditMaster.SetStore(creditMasterStore);

            Customer customer = await _customerService.GetAsync(creditMaster.GetCustomerId, CustomerReadingFields.PayCredit,
                ProfileReadingFields.MandatoryDownPayment);

            creditMaster.SetCustomer(customer, _credinetAppSettings);


            if (creditMaster.Current.IsRefinancing(_credinetAppSettings)) 
            {

                List<RefinancingLogDetail> refinancedCredits = 
                    await _refinancingLogRepository.GetByStatusFromMasterAsync(creditMasterId, RefinancingLogDetailReadingFields.BasicInfo);
                
                paymentCreditRequest.BalanceReleaseForRefinancing = 
                            refinancedCredits.Select(x => x.GetValue.Round(parameters.DecimalNumbersRound)).Sum(); 
            }

            Store store = await _storeService.GetAsync(paymentCreditRequest.StoreId, StoreReadingFields.BusinessGroupIdentifiers,
                loadAssuranceCompany: true, loadPaymentType: true, loadBusinessGroup: true);

            PaymentCreditResponse paymentCreditResponse = await ApplyPaymentAsync(paymentCreditRequest, creditMaster, store,
                parameters, transaction: transaction, simulation: simulation);

            if (notify && !simulation)
            {
                decimal assuranceTax = parameters.AssuranceTax;
                int decimalNumbersRound = parameters.DecimalNumbersRound;

                await PaymentCreditNotifyAsync(paymentCreditResponse, assuranceTax, decimalNumbersRound);
            }

            return paymentCreditResponse;
        }

        /// <summary>
        /// <see cref="ICreditPaymentService.DownPaymentAsync(PaymentCreditRequestComplete,
        /// CreditMaster, AppParameters, Transaction)"/>
        /// </summary>
        /// <param name="paymentCreditRequest"></param>
        /// <param name="creditMaster"></param>
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async Task<PaymentCreditResponse> DownPaymentAsync(PaymentCreditRequestComplete paymentCreditRequest, CreditMaster creditMaster,
            AppParameters parameters, Transaction transaction = null)
        {
            PaymentType downPaymentType = await _appParametersService.GetPaymentTypeAsync((int)PaymentTypes.DownPayment);

            PaymentCreditResponse paymentCreditResponse = await ApplyPaymentAsync(paymentCreditRequest, creditMaster, creditMaster.Store,
                parameters, downPaymentType, transaction);

            return paymentCreditResponse;
        }

        /// <summary>
        /// Apply payment
        /// </summary>
        /// <param name="paymentCreditRequest"></param>
        /// <param name="creditMaster"></param>
        /// <param name="store"></param>
        /// <param name="parameters"></param>
        /// <param name="paymentType"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private async Task<PaymentCreditResponse> ApplyPaymentAsync(PaymentCreditRequestComplete paymentCreditRequest, CreditMaster creditMaster,
           Store store, AppParameters parameters, PaymentType paymentType = null, Transaction transaction = null, bool simulation = false)
        {
            TransactionType paymentTransactionType = await _appParametersService.GetTransactionTypeAsync((int)TransactionTypes.Payment);

            Status activeStatus = await _appParametersService.GetStatusAsync((int)Statuses.Active);
            Status paidStatus = await _appParametersService.GetStatusAsync((int)Statuses.Paid);

            PaymentDomainRequest paymentDomainRequest = new PaymentDomainRequest(paymentCreditRequest, creditMaster, store, parameters,
                    _credinetAppSettings)
                .SetMasters(paymentTransactionType, activeStatus, paidStatus);

            return await _creditPaymentUsesCase.PayAsync(paymentDomainRequest, paymentType, transaction, simulation);
        }

        /// <summary>
        /// <see cref="ICreditPaymentService.PayCreditMultipleAsync(PayCreditsRequest)"/>
        /// </summary>
        /// <param name="payCreditsRequest"></param>
        /// <returns></returns>
        public async Task<List<PaymentCreditResponse>> PayCreditMultipleAsync(PayCreditsRequest payCreditsRequest)
        {
            List<PaymentCreditResponse> paymentCreditResponses = new List<PaymentCreditResponse>();

            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            if (payCreditsRequest.CreditPaymentDetails.Count > 1)
            {
                paymentCreditResponses = await _creditMasterRepository.ExcecuteOnTransactionAsync(async (transaction) =>
                {
                    List<PaymentCreditResponse> paymentCreditResponsesSuccess = new List<PaymentCreditResponse>();

                    foreach (PaymentCreditMultipleDetail payCreditMultipleDetail in payCreditsRequest.CreditPaymentDetails)
                    {
                        paymentCreditResponsesSuccess.Add(await PayCreditAsMultipleAsync(payCreditsRequest, parameters, payCreditMultipleDetail,
                            notify: false, transaction));
                    }

                    return paymentCreditResponsesSuccess;
                });

                int decimalNumbersRound = parameters.DecimalNumbersRound;
                decimal assuranceTax = parameters.AssuranceTax;

                await MultiplePaymentCreditNotifyAsync(paymentCreditResponses, assuranceTax, decimalNumbersRound);
            }
            else if (payCreditsRequest.CreditPaymentDetails.Count == 1)
            {
                PaymentCreditMultipleDetail payCreditMultipleDetail = payCreditsRequest.CreditPaymentDetails.First();

                paymentCreditResponses = new List<PaymentCreditResponse>
                {
                    await PayCreditAsMultipleAsync(payCreditsRequest, parameters, payCreditMultipleDetail, notify: true)
                };
            }

            return paymentCreditResponses;
        }

        /// <summary>
        /// <see cref="ICreditPaymentService.PayCreditMultipleAndNotifyAsync(PayCreditsRequest)"/>
        /// </summary>
        /// <param name="paymentCreditsRequest"></param>
        /// <returns></returns>
        public async Task PayCreditMultipleAndNotifyAsync(PayCreditsRequest paymentCreditsRequest)
        {
            CreditPaymentEventResponse<List<PaymentCreditResponse>> paymentCreditsEventResponse;
            try
            {
                List<PaymentCreditResponse> creditsPaymentResponses = await PayCreditMultipleAsync(paymentCreditsRequest);

                paymentCreditsEventResponse =
                    CreditPaymentEventResponse<List<PaymentCreditResponse>>.BuildSuccessfulResponse(paymentCreditsRequest.TransactionId, creditsPaymentResponses);

                await _creditPaymentEventsRepository.SendPayCreditsEventsAsync(payCreditsEventName, paymentCreditsRequest.TransactionId,
                                                                                  paymentCreditsEventResponse);
            }
            catch (BusinessException ex)
            {
                paymentCreditsEventResponse =
                    CreditPaymentEventResponse<List<PaymentCreditResponse>>.Build(paymentCreditsRequest.TransactionId, ex.code, ex.ToString(), null);

                await _creditPaymentEventsRepository.SendPayCreditsEventsAsync(payCreditsEventName, paymentCreditsRequest.TransactionId,
                                                                                  paymentCreditsEventResponse);
            }
            catch (Exception ex)
            {
                paymentCreditsEventResponse =
                    CreditPaymentEventResponse<List<PaymentCreditResponse>>.Build(paymentCreditsRequest.TransactionId, (int)BusinessResponse.NotControlledException,
                                                                         ex.ToString(), null);

                await _creditPaymentEventsRepository.SendPayCreditsEventsAsync(payCreditsEventName, paymentCreditsRequest.TransactionId,
                                                                                  paymentCreditsEventResponse);
            }
        }

        /// <summary>
        /// <see cref="ICreditPaymentService.GetCurrentAmortizationScheduleAsync(CurrentAmortizationScheduleRequest)"/>
        /// </summary>
        /// <param name="currentAmortizationScheduleRequest"></param>
        /// <returns></returns>
        public async Task<CurrentAmortizationScheduleResponse> GetCurrentAmortizationScheduleAsync(CurrentAmortizationScheduleRequest currentAmortizationScheduleRequest)
        {
            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            DateTime calculationDate = DateTime.Today;

            ValidateInputValuesCurrentAmortizationSchedule(currentAmortizationScheduleRequest, calculationDate);

            return _creditPaymentUsesCase.GetCurrentAmortizationSchedule(currentAmortizationScheduleRequest, calculationDate, parameters);
        }

        /// <summary>
        /// <see
        /// cref="ICreditPaymentService.GetCurrentPaymentScheduleAsync(CurrentPaymentScheduleRequest, DateTime)"/>
        /// </summary>
        /// <param name="currentPaymentScheduleRequest"></param>
        /// <param name="calculationDate"></param>
        /// <returns></returns>
        public async Task<CurrentPaymentScheduleResponse> GetCurrentPaymentScheduleAsync(CurrentPaymentScheduleRequest currentPaymentScheduleRequest, DateTime calculationDate)
        {
            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            ValidateInputValuesCurrentPaymentSchedule(currentPaymentScheduleRequest, calculationDate);

            return _creditPaymentUsesCase.GetCurrentPaymentSchedule(currentPaymentScheduleRequest, calculationDate, parameters);
        }

        /// <summary>
        /// <see cref="ICreditPaymentService.GetActiveCreditsAsync(string, string, string, DateTime, bool)"/>
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        /// <param name="storeId"></param>
        /// <param name="calculationDate"></param>
        /// <returns></returns>
        public async Task<List<CreditStatus>> GetActiveCreditsAsync(string typeDocument, string idDocument, string storeId, DateTime calculationDate)
        {
            if (string.IsNullOrEmpty(storeId?.Trim()) || string.IsNullOrEmpty(typeDocument?.Trim()) || string.IsNullOrEmpty(idDocument?.Trim()))
            {
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);
            }

            Customer customer = await _customerService.GetAsync(idDocument, typeDocument,
                CustomerReadingFields.CustomerNames);

            Store store = await _storeService.GetAsync(storeId, StoreReadingFields.ActiveCredits);

            List<CreditMaster> creditMasters =
                await _creditMasterRepository.GetActiveCreditsByCollectTypeAsync(customer, store);

            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            List<CreditStatus> activeCreditsResponse =
                _creditPaymentUsesCase.GetActiveCredits(creditMasters, parameters, calculationDate);

            activeCreditsResponse.ForEach( x => { x.FirstName = customer.Name.GetFirstName; x.SecondName = customer.Name.GetSecondName; });

            return activeCreditsResponse;
        }

        /// <summary>
        /// <see cref="ICreditPaymentService.GetDetailedActiveCreditsAsync(string, string, string, DateTime)"/>
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        /// <param name="storeId"></param>
        /// <param name="calculationDate"></param>
        /// <returns></returns>
        public async Task<List<DetailedCreditStatus>> GetDetailedActiveCreditsAsync(string typeDocument, string idDocument, string storeId, DateTime calculationDate)
        {
            if (string.IsNullOrEmpty(storeId?.Trim()) || string.IsNullOrEmpty(typeDocument?.Trim()) || string.IsNullOrEmpty(idDocument?.Trim()))
            {
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);
            }

            Customer customer = await _customerService.GetAsync(idDocument, typeDocument,
                CustomerReadingFields.ActiveCredits);

            Store store = await _storeService.GetAsync(storeId, StoreReadingFields.ActiveCredits);

            List<CreditMaster> creditMasters =
                await _creditMasterRepository.GetActiveCreditsByCollectTypeAsync(customer, store);


            return await GetDetailedActiveCreditsResponse(calculationDate, creditMasters);
        }


        /// <summary>
        /// <see cref="ICreditPaymentService.GetCompleteCreditsDataAsync(string, string, string, DateTime)"/>
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        /// <param name="calculationDate"></param>
        /// <returns></returns>
        public async Task<List<DetailedCreditStatus>> GetCompleteCreditsDataAsync(string typeDocument, string idDocument, DateTime calculationDate)
        {
            if (string.IsNullOrEmpty(typeDocument?.Trim()) || string.IsNullOrEmpty(idDocument?.Trim()))
            {
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);
            }

            Customer customer = await _customerService.GetAsync(idDocument, typeDocument,
                CustomerReadingFields.ActiveCredits);

            List<CreditMaster> creditMasters =
                await _creditMasterRepository.GetActiveCreditsAsync(customer);

            return await GetCompleteCreditsDataAsyncResponse(calculationDate, creditMasters);
        }

        private async Task<List<DetailedCreditStatus>> GetCompleteCreditsDataAsyncResponse(DateTime calculationDate, List<CreditMaster> creditMasters)
        {
            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            List<DetailedCreditStatus> detailedActiveCreditsResponse =
                _creditPaymentUsesCase.GetCompleteCreditsData(creditMasters, parameters, calculationDate);  

            return detailedActiveCreditsResponse;
        }

        /// <summary>
        /// Determines if the value to pay is valid according to the business rules
        /// </summary>
        /// <param name="creditId"></param>
        /// <param name="totalValuePaid"></param>
        /// <returns></returns>
        public async Task<bool> ValidateRulesForExternalPayments(DateTime calculationDate, Guid creditId, decimal totalValuePaid)
        {
            if (creditId == null || totalValuePaid == 0)
                return false;

            CreditStatus credit = await GetActiveCredit(creditId, calculationDate);

            return totalValuePaid <= credit.TotalPayment;
        }

        public async Task<CreditStatus> GetActiveCredit(Guid creditId, DateTime calculationDate)
        {
            if (creditId == null || calculationDate == null)
            {
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);
            }

            CreditMaster creditMasters =
                await _creditMasterRepository.GetWithCurrentAsync(creditId);

            Store creditMasterStore = await _storeService.GetAsync(creditMasters.GetStoreId, StoreReadingFields.BusinessGroupIdentifiers,
                loadAssuranceCompany: true);

            creditMasters.SetStore(creditMasterStore);

            Customer customer = await _customerService.GetAsync(creditMasters.GetCustomerId, CustomerReadingFields.PayCredit,
                ProfileReadingFields.MandatoryDownPayment);

            creditMasters.SetCustomer(customer, _credinetAppSettings);

            List<CreditMaster> credit = new List<CreditMaster>
            {
                creditMasters
            };

            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            List<CreditStatus> activeCreditsResponse =
                _creditPaymentUsesCase.GetActiveCredits(credit, parameters, calculationDate);

            return activeCreditsResponse.FirstOrDefault();
        }


        /// <summary>
        /// <see cref="ICreditPaymentService.GetDetailedActiveCreditsByCreditMasterIdAsync(string, string, DateTime)(string, string, string, DateTime)"/>
        /// </summary>
        /// <param name="creditMastersId"></param>
        /// <param name="calculationDate"></param>
        /// <returns></returns>
        public async Task<List<DetailedCreditStatus>> GetDetailedActiveCreditsByCreditMasterIdAsync(List<Guid> creditMastersId, DateTime calculationDate)
        {
            if (creditMastersId.Count > _credinetAppSettings.LimitGetCreditMasterId)
            {
                throw new BusinessException(nameof(BusinessResponse.limitExceededOfCreditmastersId), (int)BusinessResponse.limitExceededOfCreditmastersId);
            }
            List<CreditMaster> creditMasters = await _creditMasterRepository.GetWithTransactionsAsync(creditMastersId,
                fields: CreditMasterReadingFields.ActiveCredits, transactionFields: CreditReadingFields.ActiveCredits, customerFields: CustomerReadingFields.ActiveCredits,
                storeFields: StoreReadingFields.ActiveCredits, (int)Statuses.Active);

            return await GetDetailedActiveCreditsResponse(calculationDate, creditMasters);
        }

        private async Task<List<DetailedCreditStatus>> GetDetailedActiveCreditsResponse(DateTime calculationDate, List<CreditMaster> creditMasters)
        {
            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            List<DetailedCreditStatus> detailedActiveCreditsResponse =
                _creditPaymentUsesCase.GetDetailedActiveCredits(creditMasters, parameters, calculationDate);

            return detailedActiveCreditsResponse;
        }


        /// <summary>
        /// <see cref="ICreditPaymentService.GetDetailedActiveCreditsAsync(string, string)" />
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        public async Task<ClientCompromise> GetDetailedActiveCreditsCompromiseAsync(string typeDocument, string idDocument)
        {
            if (string.IsNullOrEmpty(typeDocument?.Trim()) || string.IsNullOrEmpty(idDocument?.Trim()))
            {
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);
            }

            Customer customer = await _customerService.GetAsync(idDocument, typeDocument,
                CustomerReadingFields.ActiveCredits);


            List<CreditMaster> creditMasters =
                await _creditMasterRepository.GetActiveCreditsByCollectTypeAsync(customer);

            AppParameters parameters = await _appParametersService.GetAppParametersAsync();


            ClientCompromise clientCompromise =
                _creditPaymentUsesCase.GetDetailedActiveCreditsCompromise(creditMasters, parameters, customer.GetFullName);


            return clientCompromise;
        }




        /// <summary>
        /// <see cref="ICreditPaymentService.ActiveCreditsNotifyAsync(ActiveCreditsRequest)"/>
        /// </summary>
        /// <param name="activeCreditsRequest"></param>
        /// <returns></returns>
        public async Task ActiveCreditsNotifyAsync(ActiveCreditsRequest activeCreditsRequest)
        {
            CreditPaymentEventResponse<List<CreditStatus>> activeCreditsEventResponse;

            try
            {
                List<CreditStatus> creditStatusList = await GetActiveCreditsAsync(activeCreditsRequest.TypeDocument, activeCreditsRequest.IdDocument,
                                                               activeCreditsRequest.StoreId, calculationDate: DateTime.Now);

                activeCreditsEventResponse =
                    CreditPaymentEventResponse<List<CreditStatus>>.BuildSuccessfulResponse(activeCreditsRequest.TransactionId, creditStatusList);


                string serialized = JsonConvert.SerializeObject(activeCreditsEventResponse, new JsonSerializerSettings { ContractResolver = new JsonHelper() });


                await _creditPaymentEventsRepository.SendActiveCreditsEventsAsync(activeCreditsEventName, activeCreditsRequest.IdDocument,
                                                                                  JObject.Parse(serialized));
            }
            catch (BusinessException ex)
            {
                activeCreditsEventResponse =
                    CreditPaymentEventResponse<List<CreditStatus>>.Build(activeCreditsRequest.TransactionId, ex.code, ex.ToString(), null);

                await _creditPaymentEventsRepository.SendActiveCreditsEventsAsync(activeCreditsEventName, activeCreditsRequest.IdDocument,
                                                                            activeCreditsEventResponse);
            }
            catch (Exception ex)
            {
                activeCreditsEventResponse =
                    CreditPaymentEventResponse<List<CreditStatus>>.Build(activeCreditsRequest.TransactionId, (int)BusinessResponse.NotControlledException,
                                                                         ex.ToString(), null);

                await _creditPaymentEventsRepository.SendActiveCreditsEventsAsync(activeCreditsEventName, activeCreditsRequest.IdDocument,
                                                                            activeCreditsEventResponse);
            }
        }

        /// <summary>
        /// <see cref="ICreditPaymentService.GetPaymentFeesAsync(Guid)"/>
        /// </summary>
        /// <param name="creditId"></param>
        /// <returns></returns>
        public async Task<PaymentFeesResponse> GetPaymentFeesAsync(Guid creditId)
        {
            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            CreditMaster creditMaster =
                await _creditMasterRepository.GetWithCurrentAsync(creditId, fields: CreditMasterReadingFields.PaymentFees,
                    transactionFields: CreditReadingFields.PaymentFees, customerFields: CustomerReadingFields.PaymentFees,
                    storeFields: StoreReadingFields.PaymentFees)
                ??
                throw new BusinessException(nameof(BusinessResponse.CreditsNotFound), (int)BusinessResponse.CreditsNotFound);

            if (!creditMaster.IsActive())
            {
                throw new BusinessException(nameof(BusinessResponse.CreditNotActive), (int)BusinessResponse.CreditNotActive);
            }

            Credit credit = creditMaster.Current;

            return _creditPaymentUsesCase.GetPaymentFees(creditMaster, credit, parameters, DateTime.Today, out _);
        }

        /// <summary>
        /// <see cref="ICreditPaymentService.GetPaymentTemplatesAsync(List{Guid}, bool)"/>
        /// </summary>
        /// <param name="paymentsId"></param>
        /// <returns></returns>
        public async Task<List<PaymentTemplateResponse>> GetPaymentTemplatesAsync(List<Guid> paymentsId, bool reprint)
        {
            if (!paymentsId.Any())
            {
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);
            }

            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            string cellPhone = parameters.CellPhone;
            string nit = parameters.Nit;
            int decimalNumbersRound = parameters.DecimalNumbersRound;
            decimal taxValue = parameters.AssuranceTax;

            List<Credit> payments = await _creditMasterRepository.GetPaymentsAsync(paymentsId,
                fields: CreditReadingFields.PaymentTemplates, masterFields: CreditMasterReadingFields.PaymentTemplates,
                customerFields: CustomerReadingFields.PaymentTemplates,
                storeFields: StoreReadingFields.PaymentTemplates,
                masterStoreFields: StoreReadingFields.PaymentTemplates);

            string creditPaymentPrintTemplate = await _templatesService.GetAsync(_credinetAppSettings.CreditPaymentPrintTemplate);

            List<PaymentTemplateResponse> paymentTemplatesResponse = payments.Select(payment =>
            {
                decimal balance = payment.GetBalance.Round(decimalNumbersRound);
                bool isCreditActive = balance > 0;

                decimal totalBalance = balance + (isCreditActive ? payment.GetAssuranceBalance.Round(decimalNumbersRound) : 0);

                PaymentTemplateResponse paymentTemplateResponse = new PaymentTemplateResponse
                {
                    ArrearsValuePaid = payment.CreditPayment.GetArrearsValuePaid.Round(decimalNumbersRound),
                    AssuranceTax = payment.CreditPayment.GetAssuranceValuePaid - _creditPaymentUsesCase.GetValueFromTax(payment.CreditPayment.GetAssuranceValuePaid, taxValue),
                    AssuranceValuePaid = payment.CreditPayment.GetAssuranceValuePaid.Round(decimalNumbersRound),
                    AvailableCreditLimit = payment.Customer.GetAvailableCreditLimit,
                    Balance = balance,
                    TotalBalance = totalBalance,
                    CellPhone = cellPhone,
                    ChargeValuePaid = payment.CreditPayment.GetChargeValuePaid.Round(decimalNumbersRound),
                    CreditNumber = payment.GetCreditNumber,
                    CreditValuePaid = payment.CreditPayment.GetCreditValuePaid.Round(decimalNumbersRound),
                    CustomerDocumentType = payment.Customer.DocumentType,
                    CustomerFullName = payment.Customer.GetFullName,
                    CustomerIdDocument = payment.Customer.IdDocument,
                    GenerationDate = DateTime.Now,
                    InterestValuePaid = payment.CreditPayment.GetInterestValuePaid.Round(decimalNumbersRound),
                    NextDueDate = payment.IsPaid() ? null : (DateTime?)payment.CreditPayment.GetDueDate,
                    Nit = nit,
                    PaymentDate = payment.GetTransactionDateComplete,
                    PaymentNumber = payment.CreditPayment.GetPaymentNumber,
                    StoreCreditName = payment.CreditMaster.Store.StoreName,
                    StorePaymentName = payment.Store.StoreName,
                    StorePhone = payment.Store.GetPhone,
                    Template = creditPaymentPrintTemplate,
                    TotalValuePaid = payment.CreditPayment.GetTotalValuePaid.Round(decimalNumbersRound)
                };

                paymentTemplateResponse.Template = _creditPaymentUsesCase.GetPaymentTemplate(paymentTemplateResponse, decimalNumbersRound, reprint);

                return paymentTemplateResponse;
            }).ToList();

            return paymentTemplatesResponse;
        }

        /// <summary>
        /// <see cref="ICreditPaymentService.GetCustomerPaymentHistoryAsync(string, string, string)"/>
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="documentType"></param>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        public async Task<List<PaymentHistoryResponse>> GetCustomerPaymentHistoryAsync(string storeId, string documentType, string idDocument)
        {
            ValidateFieldsEmpty(storeId, documentType, idDocument);

            Customer customer = await _customerService.GetAsync(idDocument, documentType,
                CustomerReadingFields.PaymentHistory);

            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            int maximumMonthsPaymentHistory = parameters.MaximumMonthsPaymentHistory;

            Store store = await _storeService.GetAsync(storeId, StoreReadingFields.CustomerPaymentHistory);

            List<Credit> payments = await _creditMasterRepository.GetCustomerPaymentHistoryAsync(customer, store,
                masterFields: CreditMasterReadingFields.CustomerPaymentHistory,
                transactionStoreFields: StoreReadingFields.CustomerPaymentHistory, maximumMonthsPaymentHistory);

            if (!payments.Any())
            {
                throw new BusinessException(nameof(BusinessResponse.PaymentsNotFound), (int)BusinessResponse.PaymentsNotFound);
            }

            List<Guid> paymentIds =
              payments
                  .Select(payment => payment.Id)
                  .ToList();

            List<RequestCancelPayment> requestCancelPaymentsNotDismissed = await _requestCancelPaymentRepository.GetByNotStatusAsync(paymentIds,
                RequestStatuses.Dismissed, RequestCancelPaymentReadingFields.CustomerPaymentHistory,
                RequestStatusReadingFields.CustomerPaymentHistory);

            return _creditPaymentUsesCase.CreatePaymentHistory(payments, requestCancelPaymentsNotDismissed, parameters);
        }

        /// <summary>
        /// <see cref="ICreditPaymentService.PaymentCreditNotifyAsync(PaymentCreditResponse,
        /// decimal, int)"/>
        /// </summary>
        /// <param name="paymentCreditResponse"></param>
        /// <param name="assuranceTax"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <returns></returns>
        public async Task PaymentCreditNotifyAsync(PaymentCreditResponse paymentCreditResponse, decimal assuranceTax, int decimalNumbersRound)
        {
            string creditPaymentSMSTemplate = null;
            try
            {
                await _creditCommonsService.SendEventAsync(paymentCreditResponse.CreditMaster, paymentCreditResponse.CreditMaster.Customer, paymentCreditResponse.Store);

                MailNotificationRequest mailNotificationRequest = _creditPaymentUsesCase.GetPayMailNotification(paymentCreditResponse, decimalNumbersRound, assuranceTax);

                creditPaymentSMSTemplate = await _templatesService.GetAsync("CreditPaymentSMSTemplate.txt");

                SmsNotificationRequest smsNotificationRequest = _creditPaymentUsesCase.GetSmsNotification(creditPaymentSMSTemplate, paymentCreditResponse.Store,
                    paymentCreditResponse.Credit, decimalNumbersRound);

                await _notificationService.SendMailAsync(mailNotificationRequest, "PayCreditNotificationMail", handleError: true);
                await _notificationService.SendSmsAsync(smsNotificationRequest, "PayCreditNotificationSMS", handleError: true);

            }
            catch (Exception ex)
            {
                var exception = new
                {
                    _credinetAppSettings.TemplatesBlobContainerName,
                    _credinetAppSettings.StorageAccount,
                    creditPaymentSMSTemplate,
                    paymentCreditResponse,
                    exception = ex.Message,
                    exceptionData = ex.ToString()
                };

                string eventName = $"{_credinetAppSettings.DomainName}" +
                                   $".{ex.TargetSite.DeclaringType.Namespace}" +
                                   $".{ex.TargetSite.DeclaringType.Name}";

                await _loggerService.NotifyAsync(paymentCreditResponse.IdDocument, eventName, exception);
            }

        }

        /// <summary>
        /// <see cref="ICreditPaymentService.MultiplePaymentCreditNotifyAsync(List{PaymentCreditResponse}, decimal, int)"
        /// </summary>
        /// <param name="paymentCreditResponse"></param>
        /// <param name="assuranceTax"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <returns></returns>
        public async Task MultiplePaymentCreditNotifyAsync(List<PaymentCreditResponse> paymentCreditResponse, decimal assuranceTax, int decimalNumbersRound)
        {
            string creditPaymentSMSTemplate = null;
            try
            {
                foreach (var payment in paymentCreditResponse)
                {
                    await _creditCommonsService.SendEventAsync(payment.CreditMaster, payment.Credit.Customer, payment.Store);
                }

                MailNotificationRequest mailNotificationRequest = _creditPaymentUsesCase.GetMultiplePayMailNotification(paymentCreditResponse, decimalNumbersRound, assuranceTax);

                creditPaymentSMSTemplate = await _templatesService.GetAsync("CreditPaymentSMSTemplate.txt");

                SmsNotificationRequest smsNotificationRequest = _creditPaymentUsesCase.GetSmsNotification(creditPaymentSMSTemplate, paymentCreditResponse?.FirstOrDefault().Credit.Customer,
                   paymentCreditResponse.Sum(s => s.TotalValuePaid), decimalNumbersRound);

                await _notificationService.SendMailAsync(mailNotificationRequest, "MultiplePaymentsCreditNotificationMail", handleError: true);
                await _notificationService.SendSmsAsync(smsNotificationRequest, "PayCreditNotificationSMS", handleError: true);

            }
            catch (Exception ex)
            {
                var exception = new
                {
                    _credinetAppSettings.TemplatesBlobContainerName,
                    _credinetAppSettings.StorageAccount,
                    creditPaymentSMSTemplate,
                    paymentCreditResponse,
                    exception = ex.Message,
                    exceptionData = ex.ToString()
                };

                string eventName = $"{_credinetAppSettings.DomainName}" +
                                   $".{ex.TargetSite.DeclaringType.Namespace}" +
                                   $".{ex.TargetSite.DeclaringType.Name}";

                await _loggerService.NotifyAsync(paymentCreditResponse.FirstOrDefault().IdDocument, eventName, exception);
            }

        }

        /// <summary>
        /// <see cref="ICreditPaymentService.GetDataCalculateCreditAsync(Guid, DateTime)"/>
        /// </summary>
        /// <param name="creditId"></param>
        /// <param name="calculationTime"></param>
        /// <returns></returns>
        public async Task<CalculatedQuery> GetDataCalculateCreditAsync(Guid creditId, DateTime calculationTime)
        {
            CreditMaster creditMaster = await _creditMasterRepository.GetWithCurrentAsync(creditId,
                fields: CreditMasterReadingFields.PaymentFees, transactionFields: CreditReadingFields.ActiveCredits,
                customerFields: CustomerReadingFields.BasicInfo, storeFields: StoreReadingFields.CreditDetails);

            if (creditMaster != null)
            {
                AppParameters parameters = await _appParametersService.GetAppParametersAsync();

                PaymentAlternatives paymentAlternatives = _creditPaymentUsesCase.GetPaymentAlternatives(creditMaster,
                    creditMaster.Current, parameters, calculationTime);

                int arrearsGracePeriod = parameters.ArrearsGracePeriod;

                bool hasArrears =
                    _creditUsesCase.HasArrears(calculationTime, creditMaster.Current.CreditPayment.GetDueDate, arrearsGracePeriod);

                int arrearsDays = _creditUsesCase.GetArrearsDays(calculationTime, creditMaster.Current.CreditPayment.GetDueDate);

                CalculatedQuery calculatedQuery = new CalculatedQuery
                {
                    CreditId = creditMaster.Id.ToString(),
                    MinimumPayment = paymentAlternatives.MinimumPayment,
                    TotalPayment = paymentAlternatives.TotalPayment,
                    HasArrears = hasArrears,
                    ArrearsDays = arrearsDays,
                    ArrearsPayment = paymentAlternatives.PaymentFees.ArrearsPayment
                };

                return calculatedQuery;
            }
            else
            {
                return new CalculatedQuery();
            }
        }

        #endregion ICreditPaymentService Members

        #region Private

        /// <summary>
        /// Validates current payment schedule request values.
        /// </summary>
        /// <param name="currentPaymentScheduleRequest"></param>
        /// <param name="calculationDate"></param>
        private void ValidateInputValuesCurrentPaymentSchedule(CurrentPaymentScheduleRequest currentPaymentScheduleRequest, DateTime calculationDate)
        {
            ValidateInputValuesCurrentAmortizationSchedule(currentPaymentScheduleRequest, calculationDate);

            bool updatedPaymentPlanValueInvalid = currentPaymentScheduleRequest.UpdatedPaymentPlanValue < 0;

            if (updatedPaymentPlanValueInvalid)
            {
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);
            }
        }

        /// <summary>
        /// Validate input values current amortization schedule
        /// </summary>
        /// <param name="currentAmortizationScheduleRequest"></param>
        private void ValidateInputValuesCurrentAmortizationSchedule(CurrentAmortizationScheduleRequest currentAmortizationScheduleRequest, DateTime calculationDate)
        {
            if (currentAmortizationScheduleRequest == null)
            {
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);
            }

            bool creditValueInvalid = currentAmortizationScheduleRequest.CreditValue <= 0;
            bool feeValueInvalid = currentAmortizationScheduleRequest.FeeValue <= 0;
            bool interestRateInvalid = currentAmortizationScheduleRequest.InterestRate < 0;
            bool frequencyInvalid = currentAmortizationScheduleRequest.Frequency <= 0;
            bool feesInvalid = currentAmortizationScheduleRequest.Fees <= 0;
            bool downPaymentInvalid = currentAmortizationScheduleRequest.DownPayment < 0;
            bool assuranceValueInvalid = currentAmortizationScheduleRequest.AssuranceValue < 0;
            bool assuranceFeeValueInvalid = currentAmortizationScheduleRequest.AssuranceFeeValue < 0;
            bool assuranceTotalFeeValueInvalid = currentAmortizationScheduleRequest.AssuranceTotalFeeValue < 0;
            bool balanceInvalid = currentAmortizationScheduleRequest.Balance < 0;
            bool assuranceBalanceInvalid = currentAmortizationScheduleRequest.AssuranceBalance < 0;
            bool arrearsChargeInvalid = currentAmortizationScheduleRequest.ArrearsCharges < 0;
            bool chargeValueInvalid = currentAmortizationScheduleRequest.ChargeValue < 0;
            bool previousArrearsInvalid = currentAmortizationScheduleRequest.PreviousArrears < 0;
            bool previousInterestInvalid = currentAmortizationScheduleRequest.PreviousInterest < 0;
            bool datesInvalid = calculationDate < DateTime.Today;

            if (creditValueInvalid || feeValueInvalid || interestRateInvalid || frequencyInvalid
                || feesInvalid || downPaymentInvalid || assuranceValueInvalid || assuranceFeeValueInvalid
                || assuranceTotalFeeValueInvalid || balanceInvalid || assuranceBalanceInvalid || arrearsChargeInvalid
                || previousArrearsInvalid || previousInterestInvalid || chargeValueInvalid || datesInvalid)
            {
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);
            }
        }

        /// <summary>
        /// Validate payment credit request
        /// </summary>
        /// <param name="paymentCreditRequest"></param>
        private void ValidatePaymentCreditRequest(PaymentCreditRequestComplete paymentCreditRequest)
        {
            if (paymentCreditRequest == null)
            {
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);
            }

            bool creditIdInValid = paymentCreditRequest.CreditId == Guid.Empty;
            bool totalValuePaidInvalid = paymentCreditRequest.TotalValuePaid <= 0;
            bool storeIdInvalid = string.IsNullOrEmpty(paymentCreditRequest.StoreId);
            bool userIdInvalid = string.IsNullOrEmpty(paymentCreditRequest.UserId);
            bool userNameInvalid = string.IsNullOrEmpty(paymentCreditRequest.UserName);
            int paymentFutureSecondsLimit = int.Parse(_credinetAppSettings.PaymentFutureSecondsLimit);
            DateTime compareDate = DateTime.Now;
            bool calculationDateInvalid = paymentCreditRequest.CalculationDate == DateTime.MinValue ||
                paymentCreditRequest.CalculationDate > compareDate.AddSeconds(paymentFutureSecondsLimit);

            if (calculationDateInvalid)
            {
                throw new BusinessException($"Date:{compareDate}-CalculationDate:{paymentCreditRequest.CalculationDate}", (int)BusinessResponse.RequestValuesInvalid);
            }

            if (creditIdInValid || totalValuePaidInvalid || storeIdInvalid || userIdInvalid || userNameInvalid)
            {
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);
            }
        }

        /// <summary>
        /// Pay credit as multiple
        /// </summary>
        /// <param name="payCreditsRequest"></param>
        /// <param name="parameters"></param>
        /// <param name="payCreditMultipleDetail"></param>
        /// <param name="notify"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private async Task<PaymentCreditResponse> PayCreditAsMultipleAsync(PayCreditsRequest payCreditsRequest, AppParameters parameters,
            PaymentCreditMultipleDetail payCreditMultipleDetail, bool notify, Transaction transaction = null)
        {
            return await PayCreditAsync(new PaymentCreditRequestComplete
            {
                BankAccount = payCreditsRequest.BankAccount,
                CreditId = payCreditMultipleDetail.CreditId,
                Location = payCreditsRequest.Location,
                StoreId = payCreditsRequest.StoreId,
                TotalValuePaid = payCreditMultipleDetail.TotalValuePaid,
                UserId = payCreditsRequest.UserId,
                UserName = payCreditsRequest.UserName,
                CalculationDate = payCreditsRequest.CalculationDate ?? DateTime.Now,
                TransactionId = payCreditsRequest.TransactionId
            },
            notify,
            parameters,
            transaction);
        }

        /// <summary>
        /// Validate fields empty
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        private void ValidateFieldsEmpty(string storeId, string typeDocument, string idDocument)
        {
            if (string.IsNullOrEmpty(storeId?.Trim()) || string.IsNullOrEmpty(typeDocument?.Trim()) || string.IsNullOrEmpty(idDocument?.Trim()))
            {
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);
            }
        }

        #endregion Private
    }
}