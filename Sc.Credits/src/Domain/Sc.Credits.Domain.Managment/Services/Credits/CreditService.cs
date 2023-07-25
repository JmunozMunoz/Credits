using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Customers;
using Sc.Credits.Domain.Managment.Services.Stores;
using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Domain.Model.Call.Gateway;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Events;
using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.Domain.Model.Credits.Queries.Commands;
using Sc.Credits.Domain.Model.Credits.Queries.Reading;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Customers.Gateway;
using Sc.Credits.Domain.Model.Customers.Queries.Reading;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.Model.ReportTemplates.Gateway;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Domain.Model.Stores.Queries.Reading;
using Sc.Credits.Domain.Model.Validation.Extensions;
using Sc.Credits.Domain.Model.Validation.Validators;
using Sc.Credits.Domain.UseCase.Credits;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Managment.Services.Credits
{
    /// <summary>
    /// Credit service is an implementation of <see cref="ICreditService"/>
    /// </summary>
    public class CreditService : ICreditService
    {
        private readonly ICreditMasterRepository _creditMasterRepository;
        private readonly IRequestCancelCreditRepository _requestCancelCreditRepository;
        private readonly IRequestCancelPaymentRepository _requestCancelPaymentRepository;
        private readonly ICreditUsesCase _creditUsesCase;
        private readonly ICreditPaymentService _creditPaymentService;
        private readonly ICreditCommonsService _creditCommonsService;
        private readonly IAppParametersService _appParametersService;
        private readonly IReportTemplatesGateway _reportTemplatesGateway;
        private readonly ISignatureService _signatureService;
        private readonly ITemplatesService _templatesService;
        private readonly ICustomerService _customerService;
        private readonly IStoreService _storeService;
        private readonly IStorageService _storageService;
        private readonly IUnapprovedCreditRepository _unapprovedCreditRepository;
        private readonly ILoggerService<CreditService> _loggerService;
        private readonly IRiskLevelRepository _riskLevelRepository;
        private readonly ICreditRequestAgentAnalysisRepository _creditRequestAgentAnalysisRepository;
        private readonly ICreditRequestAgentAnalysisService _creditRequestAgentAnalysisService;
        private readonly IUdpCallHttpRepository _udpCallHttpRepository;
        private readonly CredinetAppSettings _credinetAppSettings;

        /// <summary>
        /// Creates a new instance of <see cref="CreditService"/>
        /// </summary>
        /// <param name="creditCommons"></param>
        /// <param name="creditPaymentService"></param>
        /// <param name="signatureService"></param>
        /// <param name="loggerService"></param>
        public CreditService(CreditCommons creditCommons,
            ICreditPaymentService creditPaymentService,
            ISignatureService signatureService,
            IUnapprovedCreditRepository unapprovedCreditRepository,
            IReportTemplatesGateway reportTemplatesGateway,
            ILoggerService<CreditService> loggerService,
            IRiskLevelRepository riskLevelRepository,
            ICreditRequestAgentAnalysisRepository creditRequestAgentAnalysisRepository,
            ICreditRequestAgentAnalysisService creditRequestAgentAnalysisService,
            IUdpCallHttpRepository udpCallHttpRepository)
        {
            _creditMasterRepository = creditCommons.CreditMasterRepository;
            _requestCancelCreditRepository = creditCommons.CancelCommons.RequestCancelCreditRepository;
            _requestCancelPaymentRepository = creditCommons.CancelCommons.RequestCancelPaymentRepository;
            _creditUsesCase = creditCommons.CreditUsesCase;
            _creditPaymentService = creditPaymentService;
            _creditCommonsService = creditCommons.Service;
            _appParametersService = _creditCommonsService.Commons.AppParameters;
            _signatureService = signatureService;
            _templatesService = _creditCommonsService.Commons.Templates;
            _customerService = _creditCommonsService.CustomerService;
            _storeService = _creditCommonsService.StoreService;
            _storageService = _creditCommonsService.Commons.Storage;
            _loggerService = loggerService;
            _unapprovedCreditRepository = unapprovedCreditRepository;
            _reportTemplatesGateway = reportTemplatesGateway;
            _riskLevelRepository = riskLevelRepository;
            _credinetAppSettings = _creditCommonsService.Commons.CredinetAppSettings;
            _creditRequestAgentAnalysisRepository = creditRequestAgentAnalysisRepository;
            _creditRequestAgentAnalysisService = creditRequestAgentAnalysisService;
            _udpCallHttpRepository = udpCallHttpRepository;
        }

        #region ICreditService Members

        /// <summary>
        /// <see cref="ICreditService.GetCreditDetailsAsync(RequiredInitialValuesForCreditDetail)"/>
        /// </summary>
        /// <param name="requiredValues"></param>
        /// <returns>CreditDetailResponse with all the operations made</returns>
        public async Task<CreditDetailResponse> GetCreditDetailsAsync(RequiredInitialValuesForCreditDetail requiredValues)
        {
            ValidateFieldsEmpty(requiredValues.storeId, requiredValues.typeDocument, requiredValues.idDocument, requiredValues.creditValue);

            Customer customer = await _customerService.GetAsync(requiredValues.idDocument, requiredValues.typeDocument,
                CustomerReadingFields.CreditDetails, ProfileReadingFields.MandatoryDownPayment);

            Store store = await _storeService.GetAsync(requiredValues.storeId, StoreReadingFields.CreditDetails, loadProductCategory: true);

            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            ValidateInputValues(requiredValues.creditValue, requiredValues.months, requiredValues.frequency);

            CreditDetailDomainRequest detailRequest = new CreditDetailDomainRequest(customer, store, requiredValues.creditValue, requiredValues.frequency, parameters);
            detailRequest.SetFeesByMonths(requiredValues.months);

            CreditDetailResponse creditDetailResponse = _creditUsesCase.GetCreditDetails(detailRequest);

            creditDetailResponse.CustomerAllowPhotoSignature = await CustomerAllowPhotoSignatureAsync(customer, parameters);

            return creditDetailResponse;
        }

        /// <summary>
        /// <see cref="ICreditService.GetTimeLimitInMonthsAsync(string, string, decimal, string)"/>
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="typeDocument"></param>
        /// <param name="creditValue"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public async Task<int> GetTimeLimitInMonthsAsync(string idDocument, string typeDocument, decimal creditValue, string storeId)
        {
            ValidateFieldsEmpty(storeId, typeDocument, idDocument, creditValue);

            Customer customer = await _customerService.GetActiveAsync(idDocument, typeDocument,
                CustomerReadingFields.LimitMonths);

            Store store = await _storeService.GetAsync(storeId, StoreReadingFields.LimitMonths, loadProductCategory: true);

            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            int decimalNumbersRound = parameters.DecimalNumbersRound;
            decimal partialCreditLimit = parameters.PartialCreditLimit;

            if (!IsSimulatedClient(idDocument) && parameters.StoreProfiles.Contains(store.GetStoreProfileCode))
            {
                if (creditValue > parameters.MaximumCreditValueAccordingToStoreProfile &&
                    !await ValidateACreditPaidAccordingToTime(customer, parameters) ||
                   (!await _creditMasterRepository.ValidateCustomerHistory(customer.Id) && _credinetAppSettings.CheckCustomerHistory))
                {
                    UnapprovedCredit unapprovedCredit = new UnapprovedCredit(customer.Id, storeId,
                                                                 DateTime.Now, DateTime.Now.TimeOfDay, creditValue);

                    await _unapprovedCreditRepository.AddAsync(unapprovedCredit);

                    throw new BusinessException(nameof(BusinessResponse.DeniedCredit), (int)BusinessResponse.DeniedCredit);
                }
            }

            return _creditUsesCase.GetTimeLimitInMonths(customer, creditValue, store, decimalNumbersRound, partialCreditLimit);
        }

        /// <summary>
        /// <see cref="ICreditService.CreateAsync(CreateCreditRequest)"/>
        /// </summary>
        /// <param name="createCreditRequest"></param>
        /// <returns></returns>
        public async Task<CreateCreditResponse> CreateAsync(CreateCreditRequest createCreditRequest)
        {
            ValidateRequestCreation(createCreditRequest);

            Customer customer = await _customerService.GetActiveAsync(createCreditRequest.IdDocument, createCreditRequest.TypeDocument,
              CustomerReadingFields.CreateCredit, ProfileReadingFields.MandatoryDownPayment);

            Store store = await _storeService.GetAsync(createCreditRequest.StoreId, StoreReadingFields.BusinessGroupIdentifiers,
                loadAssuranceCompany: true, loadPaymentType: true, loadProductCategory: true, loadBusinessGroup: true);

            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            await ValidateCreditToken(createCreditRequest, store, customer, parameters);

            CreateCreditDomainRequest createCreditDomainRequest = await _creditCommonsService.NewCreateCreditDomainRequestAsync(createCreditRequest,
                customer, store);

            CreateCreditTransactionResponse createCreditTransactionResponse =
                await _creditMasterRepository.ExcecuteOnTransactionAsync(async transaction =>
                {
                    CreateCreditResponse createCreditResponseOnTransaction = await _creditUsesCase.CreateAsync(createCreditDomainRequest, transaction);

                    CreditMaster creditMaster = createCreditResponseOnTransaction.CreditMaster;
                    Credit createCreditTransaction = creditMaster.Current;

                    PaymentCreditResponse paymentCreditResponseOnTransaction = null;

                    if (createCreditTransaction.HasDownPayment())
                    {
                        paymentCreditResponseOnTransaction = await _creditPaymentService.DownPaymentAsync(new PaymentCreditRequestComplete
                        {
                            BankAccount = string.Empty,
                            CalculationDate = creditMaster.GetCreditDateComplete,
                            CreditId = creditMaster.Id,
                            StoreId = createCreditRequest.StoreId,
                            TotalValuePaid = createCreditTransaction.GetTotalDownPayment,
                            UserId = createCreditRequest.UserId,
                            Location = createCreditRequest.Location,
                            UserName = createCreditRequest.UserName
                        },
                        creditMaster,
                        parameters,
                        transaction);

                        createCreditResponseOnTransaction.DownPaymentId = paymentCreditResponseOnTransaction.PaymentId;
                    }

                    return new CreateCreditTransactionResponse(createCreditResponseOnTransaction, createCreditTransaction,
                        paymentCreditResponseOnTransaction);
                });

            CreateCreditResponse createCreditResponse = createCreditTransactionResponse.CreateCreditResponse;

            await TrySendCreditCreationAsync(customer, store, createCreditTransactionResponse, createCreditResponse);
            await TrySendNotificationsCreditCreation(createCreditResponse);

            if (createCreditTransactionResponse.HasPayment)
            {
                decimal assuranceTax = parameters.AssuranceTax;
                int decimalNumbersRound = parameters.DecimalNumbersRound;

                await _creditPaymentService.PaymentCreditNotifyAsync(createCreditTransactionResponse.PaymentCreditResponse, assuranceTax,
                    decimalNumbersRound);
            }

            return createCreditResponse;
        }

        /// <summary>
        /// <see cref="ICreditService.CreditCreationNotifyAsync(CreateCreditResponse)"/>
        /// </summary>
        /// <param name="createCreditResponse"></param>
        /// <returns></returns>
        public async Task CreditCreationNotifyAsync(CreateCreditResponse createCreditResponse)
        {
            WebClient webClient = new WebClient();
            int decimalNumbersRoundPercentage = 2;

            Customer customer = await _customerService.GetActiveAsync(createCreditResponse.IdDocument, createCreditResponse.TypeDocument,
                CustomerReadingFields.CreditCreation);

            Store store = await _storeService.GetAsync(createCreditResponse.StoreId, StoreReadingFields.CreditCreation,
                loadAssuranceCompany: true, loadCity: true, loadBusinessGroup: true);

            CreditMaster creditMaster = await _creditMasterRepository.GetWithCurrentAsync(createCreditResponse.CreditId, customer, store);

            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            int decimalNumbersRound = parameters.DecimalNumbersRound;
            string cellPhone = parameters.CellPhone;
            string nit = parameters.Nit;

            Credit credit = creditMaster.Current;

            PromissoryNoteRequest promissoryNoteRequest = new PromissoryNoteRequest(
                createCreditResponse, creditMaster, _credinetAppSettings.CompanyName, credit.GetFrequency, parameters.EffectiveAnnualRate, parameters.ArrearsEffectiveAnnualRate,
                decimalNumbersRoundPercentage, decimalNumbersRound, cellPhone, nit, parameters.GovernmentAnnualEffectiveRate, parameters.AssuranceTax);

            promissoryNoteRequest.AmortizationScheduleFees =
                        _creditUsesCase.GetOriginalAmortizationScheduleHtml(promissoryNoteRequest.CreditDetails, promissoryNoteRequest.Frequency,
                                          promissoryNoteRequest.DecimalNumbersRound, promissoryNoteRequest.CreditMaster.CreateDate, out DateTime lastFeeDate);

            promissoryNoteRequest.LastFeeDate = lastFeeDate.ToShortDateString();

            byte[] promisoryNoteBytes = webClient.DownloadData(
                        await _reportTemplatesGateway.GenerateAsync(promissoryNoteRequest, _credinetAppSettings.PromissoryNoteTemplateName,
                                                                  promissoryNoteRequest.FileName));

            promissoryNoteRequest.FileName += ".pdf";

            await _storageService.UploadFileAsync(promisoryNoteBytes, _credinetAppSettings.PromissoryNotePath, promissoryNoteRequest.FileName,
                _credinetAppSettings.PdfBlobContainerName);

            creditMaster.SetPromissoryNoteFileName(promissoryNoteRequest.FileName);

            IEnumerable<Field> updateFields = CreditMasterCommandsFields.PrommisoryNoteFileNameUpdate;

            if (creditMaster.Store.GetAllowPromissoryNoteSignature)
            {
                await TrySignAsync(creditMaster, promissoryNoteRequest, promisoryNoteBytes);

                updateFields = updateFields.Union(CreditMasterCommandsFields.CertificationUpdate);
            }

            await _creditMasterRepository.UpdateAsync(creditMaster, updateFields);

            MailNotificationRequest mailNotificationRequest = _creditUsesCase.GetCreateCreditMailNotification(creditMaster.Customer, creditMaster, credit,
                createCreditResponse, creditMaster.Store, promissoryNoteRequest.FileName, decimalNumbersRound);

            await _creditCommonsService.Commons.Notification.SendMailAsync(mailNotificationRequest, "NotifyMail_CreateCredit");

            TransactionType transactionType = await _appParametersService.GetTransactionTypeAsync((int)TransactionTypes.UpdateCredit);

            await _creditCommonsService.SendEventAsync(creditMaster, creditMaster.Customer, creditMaster.Store, transactionType: transactionType);
        }

        /// <summary>
        /// <see cref="ICreditService.GetCustomerCreditLimitAsync(string, string, string)"/>
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        /// <param name="vendorId"></param>
        /// <returns></returns>
        public async Task<CustomerCreditLimitResponse> GetCustomerCreditLimitAsync(string typeDocument, string idDocument, string vendorId)
        {
            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            decimal minimumCreditValue = parameters.MinimumCreditValue;

            return await GetCustomerCreditLimitAsync(typeDocument, idDocument, vendorId, minimumCreditValue);
        }

        /// <summary>
        /// <see cref="ICreditService.GetCustomerCreditLimitAsync(string, string)"/>
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        public async Task<bool> GetCustomerCreditLimitIncreaseAsync(string typeDocument, string idDocument)
        {
            return await AllowCreditLimitIncrease(typeDocument, idDocument);
        }

        /// <summary>
        /// <see cref="ICreditService.GetCustomerCreditLimitAsync(string, string, string, decimal)"/>
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        /// <param name="vendorId"></param>
        /// <param name="creditValue"></param>
        /// <returns></returns>
        public async Task<CustomerCreditLimitResponse> GetCustomerCreditLimitAsync(string typeDocument, string idDocument, string vendorId, decimal creditValue)
        {
            Customer customer = await _customerService.GetAsync(idDocument, typeDocument,
                CustomerReadingFields.InformationCustomer);

            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            decimal partialCreditLimit = parameters.PartialCreditLimit;
            int decimalNumbersRound = parameters.DecimalNumbersRound;
            int arrearsGracePeriod = parameters.ArrearsGracePeriod;

            CustomerCreditLimitResponse customerCreditLimitResponse =
                 await _creditUsesCase.GetCustomerCreditLimitAsync(customer, partialCreditLimit, decimalNumbersRound,
                            arrearsGracePeriod, creditValue, vendorId);

            return customerCreditLimitResponse;
        }

        /// <summary>
        /// <see cref="ICreditService.AllowCreditLimitIncrease(string, string)"/>
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        public async Task<bool> AllowCreditLimitIncrease(string typeDocument, string idDocument)
        {
            Customer customer = await _customerService.GetAsync(idDocument, typeDocument,
                CustomerReadingFields.InformationCustomer);

            return customer.AllowCreditLimitIncrease();
        }

        /// <summary>
        /// <see cref="ICreditService.GetOriginalAmortizationScheduleAsync(AmortizationScheduleRequest)"/>
        /// </summary>
        /// <param name="amortizationScheduleRequest"></param>
        /// <returns></returns>
        public async Task<AmortizationScheduleResponse> GetOriginalAmortizationScheduleAsync(AmortizationScheduleRequest amortizationScheduleRequest)
        {
            ValidateInputValuesAmortizationSchedule(amortizationScheduleRequest);

            AppParameters parameters = await _appParametersService.GetAppParametersAsync();
            int decimalNumbersRound = parameters.DecimalNumbersRound;

            return _creditUsesCase.GetOriginalAmortizationSchedule(amortizationScheduleRequest, decimalNumbersRound);
        }

        /// <summary>
        ///<see cref="ICreditService.UpdateExtraFieldsAsync(UpdateCreditExtraFieldsRequest)"/>
        /// </summary>
        /// <param name="updateCreditExtraFieldsRequest"></param>
        /// <returns></returns>
        public async Task UpdateExtraFieldsAsync(UpdateCreditExtraFieldsRequest updateCreditExtraFieldsRequest)
        {
            CreditMaster creditMaster =
                await _creditMasterRepository.GetWithCurrentAsync(updateCreditExtraFieldsRequest.CreditId,
                    customerFields: CustomerReadingFields.CreditCustomerInfo, storeFields: StoreReadingFields.BasicInfo)
                ??
                throw new BusinessException(nameof(BusinessResponse.CreditsNotFound), (int)BusinessResponse.CreditsNotFound);

            _creditUsesCase.UpdateExtraFields(updateCreditExtraFieldsRequest, creditMaster);

            await _creditMasterRepository.UpdateAsync(creditMaster, CreditMasterCommandsFields.SellerInfoUpdate);

            TransactionType transactionType = await _appParametersService.GetTransactionTypeAsync((int)TransactionTypes.UpdateCredit);

            await _creditCommonsService.SendEventAsync(creditMaster, creditMaster.Customer, creditMaster.Store, transactionType: transactionType);
        }

        /// <summary>
        /// <see cref="ICreditService.UpdateChargesPaymentPlanValueAsync(Guid, decimal, bool,
        /// decimal, decimal)"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="charges"></param>
        /// <param name="hasArrearsCharge"></param>
        /// <param name="arrearsCharges"></param>
        /// <param name="updatedPaymentPlanValue"></param>
        /// <returns></returns>
        public async Task UpdateChargesPaymentPlanValueAsync(Guid id, decimal charges, bool hasArrearsCharge, decimal arrearsCharges,
            decimal updatedPaymentPlanValue)
        {
            CreditMaster creditMaster =
                await _creditMasterRepository.GetWithCurrentAsync(id, customerFields: CustomerReadingFields.CreditCustomerInfo,
                    storeFields: StoreReadingFields.BasicInfo, transactionStoreFields: StoreReadingFields.BasicInfo)
                ??
                throw new BusinessException(nameof(BusinessResponse.CreditsNotFound), (int)BusinessResponse.CreditsNotFound);

            TransactionType transactionType = await _appParametersService.GetTransactionTypeAsync((int)TransactionTypes.UpdateChargesPaymentPlan);

            creditMaster.HandleEvent(new UpdateChargesPaymentPlanMasterEvent(creditMaster, charges,
                hasArrearsCharge, arrearsCharges, updatedPaymentPlanValue, transactionType));

            await _creditMasterRepository.AddTransactionAsync(creditMaster);

            await _creditCommonsService.SendEventAsync(creditMaster, creditMaster.Customer, creditMaster.Current.Store);
        }

        /// <summary>
        /// <see cref="ICreditService.GenerateTokenWithRiskLevelCalculationAsync(GenerateTokenRequest)"/>
        /// </summary>
        /// <param name="generateTokenRequest"></param>
        /// <returns></returns>
        public async Task<TokenResponse> GenerateTokenWithRiskLevelCalculationAsync(GenerateTokenRequest generateTokenRequest)
        {
            ValidateFieldsEmpty(generateTokenRequest.StoreId, generateTokenRequest.TypeDocument, generateTokenRequest.IdDocument,
                generateTokenRequest.CreditValue);

            AppParameters parameters = await _appParametersService.GetAppParametersAsync();
            int decimalNumbersRound = parameters.DecimalNumbersRound;

            Customer customer = await _customerService.GetAsync(generateTokenRequest.IdDocument, generateTokenRequest.TypeDocument,
                CustomerReadingFields.TokenWithFirstName, ProfileReadingFields.MandatoryDownPayment);

            customer.SetCreditLimitValidation(validate: !generateTokenRequest.IsRefinancing);

            Store store = await _storeService.GetAsync(generateTokenRequest.StoreId, StoreReadingFields.Token, loadProductCategory: true);

            if (!HasRiskLevelCalculation(store))
            {
                return await SendToken(generateTokenRequest, customer, store, parameters);
            }

            #region Query Data Base

            var creditRequestAgentAnalysis =
                await _creditRequestAgentAnalysisService.GetCreditRequestAgentAnalysis(
                    customer.IdDocument,
                    CreditRequestAgentAnalysisReadingFields.ResultInfo);

            #endregion Query Data Base

            DateTime newDate = DateTime.Now.AddDays(-1);

            var agentAnalysisResult = creditRequestAgentAnalysis?.Where(x => x.TransactionDateComplete >= newDate)
                .OrderByDescending(x => x.TransactionDate)
                .ThenByDescending(x => x.TransactionTime).FirstOrDefault();

            await UpdateStatusAgentAnalysis(creditRequestAgentAnalysis);

            if (agentAnalysisResult != null)
            {
                #region validacion regla

                if (agentAnalysisResult.AgentAnalysisResultId == (int)AgentAnalysisResults.Approved)
                {
                    return await SendToken(generateTokenRequest, customer, store, parameters);
                }

                var notificationTemplateFile = GetTemplateByResult(agentAnalysisResult.AgentAnalysisResultId);
                string notificationTemplate = await _templatesService.GetAsync(notificationTemplateFile);

                SmsNotificationRequest smsNotificationRecallRequest = _creditUsesCase.SendSmsNotification(notificationTemplate, customer);
                await _creditCommonsService.Commons.Notification.SendSmsAsync(smsNotificationRecallRequest, _credinetAppSettings.SmsNotificationRequestName);

                return TokenResponse.NotGenerated();

                #endregion validacion regla
            }

            CustomerRiskLevelRequest customerRiskLevel = new CustomerRiskLevelRequest()
            {
                CustomerId = customer.Id.ToString(),
                CustomerDocumentId = customer.IdDocument,
                CreditValue = generateTokenRequest.CreditValue,
                StoreId = generateTokenRequest.StoreId,
                Source = generateTokenRequest.Source,
            };

            CustomerRiskLevel riskLevel = await _riskLevelRepository.CalculateRiskLevelAsync(customerRiskLevel);

            if (riskLevel.Level == _credinetAppSettings.CustomerRiskLevelNumber)
            {
                return await SendToken(generateTokenRequest, customer, store, parameters);
            }

            #region Save Data Base

            CreditRequestAgentAnalysis agentAnalysis = new CreditRequestAgentAnalysis(
                   customer.Id,
                   generateTokenRequest.IdDocument,
                   generateTokenRequest.CreditValue,
                   string.Join(",", riskLevel.Observations),
                   generateTokenRequest.StoreId,
                   DateTime.Now,
                   DateTime.Now.TimeOfDay,
                   (int)AgentAnalysisResults.Pending,
                   int.Parse(generateTokenRequest.Source));

            await _creditRequestAgentAnalysisRepository.AddAsync(agentAnalysis);

            #endregion Save Data Base

            MailNotificationRequest mailNotificationRequest = _creditUsesCase.GetRiskyCreditRequestNotification(
                customer, store, generateTokenRequest, riskLevel, decimalNumbersRound, agentAnalysis.Id);

            string creditTokenSmsNotificationTemplate = await _templatesService.GetAsync(_credinetAppSettings.CustomerCallNotificationTemplate);

            SmsNotificationRequest smsNotificationRequest = _creditUsesCase.SendSmsNotification(creditTokenSmsNotificationTemplate, customer);

            await _creditCommonsService.Commons.Notification.SendMailAsync(mailNotificationRequest, _credinetAppSettings.MailNotificationRequestName);
            await _creditCommonsService.Commons.Notification.SendSmsAsync(smsNotificationRequest, _credinetAppSettings.SmsNotificationRequestName);

            return TokenResponse.NotGenerated();
        }

        /// <summary>
        /// UpdateStatusAgentAnalysis
        /// </summary>
        /// <param name="creditRequestAgentAnalysis"></param>
        /// <returns></returns>
        private async Task UpdateStatusAgentAnalysis(List<CreditRequestAgentAnalysis> creditRequestAgentAnalysis)
        {
            #region Cambio de estado Automatico

            IEnumerable<Field> updateCredit = CreditRequestAgentAnalysisCommandsFields.InfoUpdate;

            DateTime datebefore = DateTime.Now.AddDays(_credinetAppSettings.DiscountedDays);
            var agentAnalysisResult = creditRequestAgentAnalysis?.Where(x => x.TransactionDateComplete < datebefore && x.AgentAnalysisResultId == _credinetAppSettings.PendingStatus)
               .OrderByDescending(x => x.TransactionDate)
               .ThenByDescending(x => x.TransactionTime).FirstOrDefault();

            if (agentAnalysisResult != null)
            {
                agentAnalysisResult.SetAnalysisResult(_credinetAppSettings.RejectDueToExpirationStatus);
                await _creditRequestAgentAnalysisRepository.UpdateAsync(agentAnalysisResult, updateCredit);
            }

            #endregion Cambio de estado Automatico
        }

        /// <summary>
        /// <see cref="ICreditService.GenerateTokenAsync(GenerateTokenRequest)"/>
        /// </summary>
        /// <param name="generateTokenRequest"></param>
        /// <returns></returns>
        public async Task<TokenResponse> GenerateTokenAsync(GenerateTokenRequest generateTokenRequest)
        {
            ValidateFieldsEmpty(generateTokenRequest.StoreId, generateTokenRequest.TypeDocument, generateTokenRequest.IdDocument,
                generateTokenRequest.CreditValue);

            Customer customer = await _customerService.GetAsync(generateTokenRequest.IdDocument, generateTokenRequest.TypeDocument,
                CustomerReadingFields.Token, ProfileReadingFields.MandatoryDownPayment);

            customer.SetCreditLimitValidation(validate: !generateTokenRequest.IsRefinancing);

            Store store = await _storeService.GetAsync(generateTokenRequest.StoreId, StoreReadingFields.Token, loadProductCategory: true);

            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            return await SendToken(generateTokenRequest, customer, store, parameters);
        }

        /// <summary>
        /// <see cref="ICreditService.TokenCallRequestAsync(CreditTokenCallRequest)"/>
        /// </summary>
        /// <param name="creditTokenCallRequest"></param>
        /// <returns></returns>
        public async Task<bool> TokenCallRequestAsync(CreditTokenCallRequest creditTokenCallRequest)
        {
            Customer customer = await _customerService.GetAsync(creditTokenCallRequest.IdDocument, creditTokenCallRequest.TypeDocument,
               CustomerReadingFields.Token, ProfileReadingFields.MandatoryDownPayment);

            if (string.IsNullOrEmpty(customer.GetMobile))
                return false;

            creditTokenCallRequest.SetMobile(customer.GetMobile);
            creditTokenCallRequest.SetCustomerId(customer.Id);

            #region Query Data Base

            var creditRequestAgentAnalysis =
                await _creditRequestAgentAnalysisService.GetCreditRequestAgentAnalysis(
                    customer.IdDocument,
                    CreditRequestAgentAnalysisReadingFields.ResultInfo);

            var agentAnalysisResult = creditRequestAgentAnalysis?.OrderByDescending(x => x.TransactionDateComplete)
                                                                 .FirstOrDefault();

            if (agentAnalysisResult == null
                || agentAnalysisResult.AgentAnalysisResultId == (int)AgentAnalysisResults.Approved)
            {
                await _creditCommonsService.TokenCallRequestAsync(creditTokenCallRequest);
                return true;
            }

            #endregion Query Data Base

            #region Asignando parametros

            creditTokenCallRequest.IdDocument = customer.IdDocument;
            string message = GetTemplateByResult(agentAnalysisResult.AgentAnalysisResultId);
            string notificationTemplate = await _templatesService.GetAsync(message);

            SmsNotificationRequest smsNotificationRecallRequest = _creditUsesCase.SendSmsNotification(notificationTemplate, customer);
            creditTokenCallRequest.Message = smsNotificationRecallRequest.Message.Replace(",", "").ToLower();

            #endregion Asignando parametros

            await _udpCallHttpRepository.CallAsync(creditTokenCallRequest);

            return true;
        }

        /// <summary>
        /// <see cref="ICreditService.GetPromissoryNoteInfoAsync(Guid, bool)"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="reprint"></param>
        /// <returns></returns>
        public async Task<PromissoryNoteInfo> GetPromissoryNoteInfoAsync(Guid id, bool reprint)
        {
            CreditMaster creditMaster =
                await _creditMasterRepository.GetWithCurrentAsync(id, fields: CreditMasterReadingFields.PromissoryNote,
                    transactionFields: CreditReadingFields.PromissoryNote, customerFields: CustomerReadingFields.PrommisoryNote,
                    storeFields: StoreReadingFields.PrommisoryNote)
                ??
                throw new BusinessException(nameof(BusinessResponse.CreditsNotFound), (int)BusinessResponse.CreditsNotFound);

            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            AssuranceCompany assuranceCompany = await _storeService.GetAssuranceCompanyAsync(creditMaster.Store.GetAssuranceCompanyId);

            creditMaster.Store.SetAssuranceCompany(assuranceCompany);

            string template = await _templatesService.GetAsync("PromissoryNotePrintTemplate.html");

            return _creditUsesCase.GetPromissoryNoteInfo(creditMaster, parameters, template, reprint);
        }

        /// <summary>
        /// <see cref="ICreditService.GetPaidCreditCertificateTemplatesAsync(List{Guid}, bool)"/>
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="reprint"></param>
        /// <returns></returns>
        public async Task<List<string>> GetPaidCreditCertificateTemplatesAsync(List<Guid> ids, bool reprint)
        {
            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            string paidCreditCertificateTemplate = await _templatesService.GetAsync(_credinetAppSettings.PaidCreditCertificatePrintTemplate);

            string cellPhone = parameters.CellPhone;

            List<CreditMaster> creditMasters =
                    await _creditMasterRepository.GetPaidCreditsForCertificateAsync(ids);

            if (creditMasters == null || !creditMasters.Any())
            {
                throw new BusinessException(nameof(BusinessResponse.CreditsNotFound), (int)BusinessResponse.CreditsNotFound);
            }
            List<string> templates = new List<string>();

            creditMasters
                   .ForEach(creditMaster =>
                   {
                       Credit credit = creditMaster.Current;
                       PaidCreditCertificateResponse paidCreditCertificateResponse = new PaidCreditCertificateResponse
                       {
                           CurrentDate = DateTime.Now,
                           FullName = creditMaster.Customer.GetFullName,
                           TypeDocument = creditMaster.Customer.DocumentType,
                           IdDocument = creditMaster.Customer.IdDocument,
                           StoreName = creditMaster.Store.StoreName,
                           CreditDate = creditMaster.GetCreditDate,
                           CreditNumber = creditMaster.GetCreditNumber,
                           TransactionDate = credit.GetTransactionDate,
                           CellPhone = cellPhone,
                           Template = paidCreditCertificateTemplate,
                       };
                       templates.Add(_creditUsesCase.GetPaidCreditCertificateTemplate(paidCreditCertificateResponse, reprint));
                   });

            return templates;
        }

        /// <summary>
        /// <see cref="ICreditService.GetCustomerCreditHistoryAsync(string, string, string)"/>
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="documentType"></param>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        public async Task<List<CreditHistoryResponse>> GetCustomerCreditHistoryAsync(string storeId, string documentType, string idDocument)
        {
            ValidateFieldsEmpty(storeId, documentType, idDocument);

            Customer customer = await _customerService.GetAsync(idDocument, documentType,
                CustomerReadingFields.CreditHistory);

            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            int maximumMonthsCreditHistory = parameters.MaximumMonthsCreditHistory;

            List<CreditMaster> creditMasters = await _creditMasterRepository.GetCustomerCreditHistoryAsync(customer, storeId,
                maximumMonthsCreditHistory);

            if (!creditMasters.Any())
            {
                throw new BusinessException(nameof(BusinessResponse.CreditsNotFound), (int)BusinessResponse.CreditsNotFound);
            }

            List<Guid> canceledCreditMasterIds =
                creditMasters
                    .Where(creditMaster => creditMaster.GetStatusId == (int)Statuses.Canceled)
                    .Select(creditMaster => creditMaster.Id)
                    .ToList();

            List<RequestCancelCredit> requestCancelCreditsCanceled = await _requestCancelCreditRepository.GetByStatusAsync(canceledCreditMasterIds,
                RequestStatuses.Cancel);

            return _creditUsesCase.CreateCreditHistory(creditMasters, requestCancelCreditsCanceled, parameters);
        }

        /// <summary>
        /// <see cref="ICreditService.CustomerAllowPhotoSignatureAsync(Customer, AppParameters)"/>
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<bool> CustomerAllowPhotoSignatureAsync(Customer customer, AppParameters parameters)
        {
            int phothoSignaturePaidCreditDays = parameters.PhothoSignaturePaidCreditDays;

            return await _creditMasterRepository.ValidateACreditPaidAccordingToTime(customer.Id,
                phothoSignaturePaidCreditDays, Statuses.Paid);
        }

        /// <summary>
        /// <see cref="ICreditService.ResendTransactionsAsync(List{Guid})"/>
        /// </summary>
        /// <param name="transactionIds"></param>
        /// <returns></returns>
        public async Task ResendTransactionsAsync(List<Guid> transactionIds)
        {
            List<Credit> transactions = await _creditMasterRepository.GetTransactionsAsync(transactionIds, CustomerReadingFields.CreditLimit,
                StoreReadingFields.BasicInfo);

            foreach (Credit transaction in transactions)
            {
                if (transaction.CreditPayment.GetTransactionTypeId == (int)TransactionTypes.CancelPayment)
                {
                    RequestCancelPayment requestCancelPayment = await _requestCancelPaymentRepository.GetByCancellationIdAsync(creditCancelId: transaction.CreditPayment.Id,
                      RequestCancelPaymentReadingFields.RequestCancelPayment, RequestStatuses.Cancel);

                    if (requestCancelPayment != null)
                    {
                        transaction.SetIdLastPaymentCancelled(requestCancelPayment.GetCreditId);
                    }


                }
                await _creditCommonsService.SendEventAsync(transaction.CreditMaster, transaction.Customer, transaction.Store, transaction);
            }

            await ResendCreditLimitEventsAsync(transactions.Select(t => t.Customer));
        }

        /// <summary>
        /// <see cref="ICreditService.ResendCreditsAsync(List{Guid})"/>
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task ResendCreditsAsync(List<Guid> ids)
        {
            List<CreditMaster> allCredits = await _creditMasterRepository.GetWithTransactionsAsync(ids, customerFields: CustomerReadingFields.CreditLimit,
                storeFields: StoreReadingFields.BasicInfo);

            List<Credit> allTransactions = allCredits.SelectMany(credit => credit.History).ToList();

            foreach (Credit transaction in allTransactions)
            {
                CreditMaster creditMaster = transaction.CreditMaster;
                await _creditCommonsService.SendEventAsync(creditMaster, creditMaster.Customer, transaction.Store, transaction);
            }

            await ResendCreditLimitEventsAsync(allCredits.Select(t => t.Customer));
        }

        /// <summary>
        /// <see cref="ICreditService.ValidateCreditToken(CreateCreditRequest)"/>
        /// </summary>
        /// <param name="createCreditRequest"></param>
        /// <returns></returns>
        public async Task<bool> ValidateCreditTokenAsync(CreateCreditRequest createCreditRequest)
        {
            ValidateRequestCreation(createCreditRequest);

            Customer customer = await _customerService.GetActiveAsync(createCreditRequest.IdDocument, createCreditRequest.TypeDocument,
              CustomerReadingFields.CreateCredit, ProfileReadingFields.MandatoryDownPayment);

            Store store = await _storeService.GetAsync(createCreditRequest.StoreId, StoreReadingFields.CreateCredit,
            loadAssuranceCompany: true, loadPaymentType: true, loadProductCategory: true);

            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            return await ValidateCreditToken(createCreditRequest, store, customer, parameters);
        }

        /// <summary>
        /// <see cref="ICreditService.GetPaidCreditDocumentAsync(List{Guid})"/>
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="reprint"></param>
        /// <returns></returns>
        public async Task<List<string>> GetPaidCreditDocumentAsync(List<Guid> ids)
        {
            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            string cellPhone = parameters.CellPhone;

            List<CreditMaster> creditMasters =
                    await _creditMasterRepository.GetPaidCreditsForCertificateAsync(ids);

            if (creditMasters == null || !creditMasters.Any())
            {
                throw new BusinessException(nameof(BusinessResponse.CreditsNotFound), (int)BusinessResponse.CreditsNotFound);
            }

            string renderName = $"{creditMasters.FirstOrDefault().Customer.IdDocument}-{_credinetAppSettings.CertificateAccountStatementReportName}";

            List<string> templates = new List<string>();
            creditMasters
                    .ForEach(creditMaster =>
                    {
                        Credit credit = creditMaster.Current;
                        ResponsePaidCreditCertification paidCreditCertificateResponse = new ResponsePaidCreditCertification
                        {
                            FormatName = _credinetAppSettings.CertificateAccountStatementName,
                            LenderCity = _credinetAppSettings.CompanyCity,
                            GenerationDate = DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy", CultureInfo.CreateSpecificCulture("es-CO")),
                            CustomerFullName = creditMaster.Customer.GetFullName,
                            CustomerDocumentType = creditMaster.Customer.CompleteDocumentType,
                            CustomerDocumentId = creditMaster.Customer.IdDocument,
                            StoreName = creditMaster.Store.StoreName,
                            CreditDate = creditMaster.GetCreditDate.ToString("dd/MM/yyyy"),
                            CreditNumber = creditMaster.GetCreditNumber.ToString(),
                            CreditPaymentDate = credit.GetTransactionDate.ToString("dd/MM/yyyy"),
                            LenderPhoneNumber = cellPhone,
                            LenderName = _credinetAppSettings.CertificateAccountStatementLenderName,
                        };
                        templates.Add(_reportTemplatesGateway.GenerateAsync(paidCreditCertificateResponse,
                                                                         _credinetAppSettings.CertificateAccountStatementReportName, renderName).Result);
                    });

            return templates;
        }

        /// <summary>
        /// <see cref="ICreditService.GetPaymentPlan(RequiredInitialValuesForCreditDetail)"/>
        /// </summary>
        /// <param name="requiredValues"></param>
        /// <returns></returns>
        public async Task<PaymentPlanResponse> GetPaymentPlan(RequiredInitialValuesForCreditDetail requiredValues)
        {
            ValidateFieldsEmpty(requiredValues.storeId, requiredValues.typeDocument, requiredValues.idDocument, requiredValues.creditValue);

            Customer customer = await _customerService.GetAsync(requiredValues.idDocument, requiredValues.typeDocument,
                CustomerReadingFields.CreditDetails, ProfileReadingFields.MandatoryDownPayment);

            Store store = await _storeService.GetAsync(requiredValues.storeId, StoreReadingFields.CreditDetails, loadProductCategory: true);

            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            ValidateInputValues(requiredValues.creditValue, requiredValues.months, requiredValues.frequency);

            CreditDetailDomainRequest detailRequest = new CreditDetailDomainRequest(customer, store, requiredValues.creditValue, requiredValues.frequency, parameters);
            detailRequest.SetFeesByMonths(requiredValues.months);

            CreditDetailResponse creditDetailResponse = _creditUsesCase.GetCreditDetails(detailRequest);

            AmortizationScheduleResponse amortizationSchedule = _creditUsesCase.GetOriginalAmortizationSchedule(
                new AmortizationScheduleRequest
                {
                    CreditValue = creditDetailResponse.CreditValue,
                    InitialDate = DateTime.Now,
                    FeeValue = creditDetailResponse.FeeCreditValue,
                    InterestRate = creditDetailResponse.InterestRate,
                    Frequency = requiredValues.frequency,
                    Fees = creditDetailResponse.Fees,
                    DownPayment = creditDetailResponse.DownPayment,
                    AssuranceValue = creditDetailResponse.AssuranceValue,
                    AssuranceFeeValue = creditDetailResponse.AssuranceFeeValue,
                    AssuranceTotalFeeValue = creditDetailResponse.AssuranceTotalFeeValue
                },
                parameters.DecimalNumbersRound);

            string amortizationScheduleUrl =
                _creditUsesCase.GetAmortizationScheduleUrl(customer, creditDetailResponse, requiredValues.frequency);

            PaymentPlanResponse paymentPlanResponse = new PaymentPlanResponse()
            {
                AmortizationSchedule = amortizationSchedule,
                AmortizationScheduleUrl = amortizationScheduleUrl,
                CustomerFirstName = customer.Name.GetFirstName,
                CustomerSecondName = customer.Name.GetSecondName,
                CustomerFullName = customer.GetFullName,
                DecimalNumbersRound = parameters.DecimalNumbersRound,
                StoreName = store.StoreName,
                CustomerEmail = customer.GetEmail,
                CustomerMobile = customer.GetMobile,
            };

            return paymentPlanResponse;
        }

        /// <summary>
        /// <see cref="ICreditService.GetRecentCreditByToken(VerifyCreditCreationRequest)"/>
        /// </summary>
        /// <param name="createCreditRequest"></param>
        /// <returns>The create credit response of a credit</returns>
        public async Task<CreateCreditResponse> GetRecentCreditByToken(VerifyCreditCreationRequest createCreditRequest)
        {
            await createCreditRequest.ValidateAndThrowsAsync<VerifyCreditCreationRequest, CreateCreditModelWhenVerifyCreditCreationValidator>(settings: _credinetAppSettings.Validation);
            CreditMaster credit = await FindCredit(createCreditRequest);
            return await CreateCreateCreditResponse(createCreditRequest, credit);
        }

        /// <summary>
        /// <see cref="ICreditService.UpdateCreditRequestByAgent(CreditRequestAnalysis)"/>
        /// </summary>
        /// <param name="creditRequestAnalysis">The credit request analysis.</param>
        /// <returns></returns>
        public async Task<bool> UpdateCreditRequestByAgent(CreditRequestAnalysis creditRequestAnalysis)
        {
            #region Query Data Base

            Customer customer = await _customerService.GetCustomerByDocument(creditRequestAnalysis.CustomerIdDocument,
                                                                             CustomerReadingFields.TokenWithFirstName, ProfileReadingFields.MandatoryDownPayment);

            IEnumerable<Field> updateCredit = CreditRequestAgentAnalysisCommandsFields.InfoUpdate;

            CreditRequestAgentAnalysis creditRequestAgentAnalysis =
              await _creditRequestAgentAnalysisService.GetCreditRequestAgentAnalysis(
                  customer.IdDocument,
                  creditRequestAnalysis.CreditValue,
                  creditRequestAnalysis.StoreId,
                  (int)AgentAnalysisResults.Pending,
                  CreditRequestAgentAnalysisReadingFields.ResultInfo);

            if (creditRequestAgentAnalysis is null)
            {
                return false;
            }

            #endregion Query Data Base

            #region Logic Implementation

            if (IsCustomerStatus(creditRequestAnalysis.CustomerStatus.ToString()))
            {
                if (creditRequestAnalysis.CustomerStatus == (int)AgentAnalysisResults.Approved)
                {
                    #region Query Data Base

                    Store store = await _storeService.GetAsync(creditRequestAnalysis.StoreId, StoreReadingFields.Token, loadProductCategory: true);
                    AppParameters parameters = await _appParametersService.GetAppParametersAsync();

                    #endregion Query Data Base

                    GenerateTokenRequest generateTokenRequest = new GenerateTokenRequest()
                    {
                        IdDocument = creditRequestAnalysis.CustomerIdDocument,
                        Frequency = creditRequestAnalysis.Frequency,
                        Months = creditRequestAnalysis.Months,
                        CreditValue = creditRequestAnalysis.CreditValue
                    };

                    await SendToken(generateTokenRequest, customer, store, parameters);
                }
                else
                {
                    var customerNotificationTemplate =
                        creditRequestAnalysis.CustomerStatus == (int)AgentAnalysisResults.RejectedOnSuspicion
                            ? _credinetAppSettings.CustomerRejectNotificationTemplate
                            : _credinetAppSettings.CustomerRejectCallNotificationTemplate;

                    string creditTokenSmsNotificationTemplate = await _templatesService.GetAsync(customerNotificationTemplate);

                    SmsNotificationRequest smsNotificationRequest = _creditUsesCase.SendSmsNotification(creditTokenSmsNotificationTemplate, customer);
                    await _creditCommonsService.Commons.Notification.SendSmsAsync(smsNotificationRequest, _credinetAppSettings.SmsNotificationRequestName);
                }

                #endregion Logic Implementation

                #region Update Data Base

                creditRequestAgentAnalysis.SetAnalysisResult(creditRequestAnalysis.CustomerStatus);
                await _creditRequestAgentAnalysisRepository.UpdateAsync(creditRequestAgentAnalysis, updateCredit);

                #endregion Update Data Base

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// <see cref="ICreditService.ExistCreditRequestById(Guid)"/>
        /// </summary>
        /// <param name="creditRequestId"></param>
        /// <returns></returns>
        public async Task<bool> ExistCreditRequestById(Guid creditRequestId)
        {
            #region Query Data Base

            CreditRequestAgentAnalysis creditRequestAgentAnalysis =
                await _creditRequestAgentAnalysisService.GetCreditRequestById(
                    creditRequestId.ToString(), CreditRequestAgentAnalysisReadingFields.ResultInfo);

            #endregion Query Data Base

            return creditRequestAgentAnalysis != null && creditRequestAgentAnalysis.AgentAnalysisResultId != _credinetAppSettings.PendingAgentAnalysisResultId;
        }

        /// <summary>
        /// <see cref="ICreditService.ResendNotificationCreditCreation(DateTime)"/>
        /// </summary>
        /// <param name="dayDate"></param>
        /// <returns></returns>
        public async Task<ResendNotificationPerDayResponse> ResendNotificationCreditCreation(ResendNotificationPerDayRequest request)
        {
            if (request.DateOperation == default || request.DateOperation >= DateTime.Now)
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);

            IEnumerable<CreditMaster> creditsMaster = await _creditMasterRepository.GetActiveWithDayDateAndPendingPromissoryNoteAsync(request.DateOperation,
                                                                                                 customerFields: CustomerReadingFields.DocumentInfo, request.LimitTransactions);

            if (creditsMaster is null || !creditsMaster.Any())
                throw new BusinessException(nameof(BusinessResponse.CreditsNotFound), (int)BusinessResponse.CreditsNotFound);

            ResendNotificationPerDayResponse response = new ResendNotificationPerDayResponse();
            foreach (CreditMaster creditMaster in creditsMaster)
            {
                try
                {
                    await ResendNotificationCreditCreation(creditMaster);
                    response.AddNotifySuccessfull(creditMaster.Id);
                }
                catch (Exception ex)
                {
                    object logDetail = new { exception = ex.ToString(), userName = request.UserName, dateOperation = request.DateOperation.ToShortDateString() };
                    await _loggerService.NotifyAsync(creditMaster.Id.ToString(),
                                                $"Exception.{nameof(ResendNotificationCreditCreation)}",
                                                logDetail, MethodBase.GetCurrentMethod());
                    response.AddNotifyUnsuccessfull(creditMaster.Id);
                }
            }
            return response;
        }

        /// <summary>
        /// <see cref="ICreditService.ResendNotificationCreditCreation(Guid)"/>
        /// </summary>
        /// <param name="creditMasterId"></param>
        /// <returns></returns>
        public async Task ResendNotificationCreditCreation(ResendNotificationRequest request)
        {
            CreditMaster creditMaster = await _creditMasterRepository.GetWithCurrentAsync(request.Id,
                    customerFields: CustomerReadingFields.DocumentInfo)
            ?? throw new BusinessException(nameof(BusinessResponse.CreditsNotFound), (int)BusinessResponse.CreditsNotFound);

            await ResendNotificationCreditCreation(creditMaster);
        }
        #endregion ICreditService Members


        #region Private
        private bool IsCustomerStatus(string statusId)
        {
            return _credinetAppSettings.CustomerStatus.Split(",").Contains(statusId);
        }

        private async Task<bool> ValidateACreditPaidAccordingToTime(Customer customer, AppParameters parameters)
        {
            int paidCreditDays = parameters.CreditDaysPaidAccordingToStoreProfile;

            return await _creditMasterRepository.ValidateACreditPaidAccordingToTime(customer.Id,
                paidCreditDays, Statuses.Paid);
        }

        /// <summary>
        /// Validate credit token
        /// </summary>
        /// <param name="createCreditRequest"></param>
        /// <param name="store"></param>
        /// <param name="customer"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private async Task<bool> ValidateCreditToken(CreateCreditRequest createCreditRequest, Store store, Customer customer, AppParameters parameters)
        {
            if (await _creditUsesCase.CustomerIsDefaulterAsync(customer, store.GetVendorId, parameters.ArrearsGracePeriod))
            {
                throw new BusinessException(nameof(BusinessResponse.CustomerIsDefaulter), (int)BusinessResponse.CustomerIsDefaulter);
            }

            if (store.CreateCreditUnautorized)
            {
                throw new BusinessException(nameof(BusinessResponse.CreateCreditUnauthorized), (int)BusinessResponse.CreateCreditUnauthorized);
            }

            await _creditCommonsService.ValidateTokenAsync(createCreditRequest, customer);

            return true;
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

        /// <summary>
        /// Validate fields empty
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        /// <param name="creditValue"></param>
        private void ValidateFieldsEmpty(string storeId, string typeDocument, string idDocument, decimal creditValue)
        {
            if (string.IsNullOrEmpty(storeId?.Trim()) || string.IsNullOrEmpty(typeDocument?.Trim()) || string.IsNullOrEmpty(idDocument?.Trim())
                || creditValue <= 0)
            {
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);
            }
        }

        /// <summary>
        /// Validate input values.
        /// </summary>
        /// <param name="creditValue"></param>
        /// <param name="months"></param>
        /// <param name="effectiveAnnualRate"></param>
        /// <param name="assurancePercentage"></param>
        private void ValidateInputValues(decimal creditValue, int months, int frequency)
        {
            if (creditValue < 0 || months < 0 || !ValidateFrequency(frequency))
            {
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);
            }
        }

        /// <summary>
        /// Validate the frequency.
        /// </summary>
        /// <param name="frequency"></param>
        /// <returns></returns>
        private bool ValidateFrequency(int frequency) =>
            Enum.IsDefined(typeof(Frequencies), frequency);

        /// <summary>
        /// Validate input values for amortization schedule.
        /// </summary>
        /// <param name="amortizationScheduleRequest"></param>
        private void ValidateInputValuesAmortizationSchedule(AmortizationScheduleRequest amortizationScheduleRequest)
        {
            if (amortizationScheduleRequest == null)
            {
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);
            }

            bool creditValueInvalid = amortizationScheduleRequest.CreditValue <= 0;
            bool feeValueInvalid = amortizationScheduleRequest.FeeValue <= 0;
            bool interestRateInvalid = amortizationScheduleRequest.InterestRate < 0;
            bool frequencyInvalid = !ValidateFrequency(amortizationScheduleRequest.Frequency);
            bool feesInvalid = amortizationScheduleRequest.Fees <= 0;
            bool downPaymentInvalid = amortizationScheduleRequest.DownPayment < 0;
            bool assuranceValueInvalid = amortizationScheduleRequest.AssuranceValue < 0;
            bool assuranceFeeValueInvalid = amortizationScheduleRequest.AssuranceFeeValue < 0;
            bool assuranceTotalFeeValueInvalid = amortizationScheduleRequest.AssuranceTotalFeeValue < 0;

            if (creditValueInvalid || feeValueInvalid || interestRateInvalid || frequencyInvalid
                || feesInvalid || downPaymentInvalid || assuranceValueInvalid || assuranceFeeValueInvalid
                || assuranceTotalFeeValueInvalid)
            {
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);
            }
        }

        /// <summary>
        /// Validate request creation.
        /// </summary>
        /// <param name="createCreditRequest"></param>
        private void ValidateRequestCreation(CreateCreditRequest createCreditRequest)
        {
            if (createCreditRequest == null)
            {
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);
            }

            ValidateFieldsEmpty(createCreditRequest.StoreId, createCreditRequest.TypeDocument, createCreditRequest.IdDocument, createCreditRequest.CreditValue);

            bool frequencyInvalid = !ValidateFrequency(createCreditRequest.Frequency);
            bool feesInvalid = createCreditRequest.Fees <= 0;
            bool sourceInvalid = !Enum.IsDefined(typeof(Sources), createCreditRequest.Source);
            bool tokenInvalid = string.IsNullOrEmpty(createCreditRequest.Token);
            bool userIdInvalid = string.IsNullOrEmpty(createCreditRequest.UserId);
            bool userNameInvalid = string.IsNullOrEmpty(createCreditRequest.UserName);
            bool authMethodInvalid = !Enum.IsDefined(typeof(AuthMethods), createCreditRequest.AuthMethod);
            bool locationInvalid = string.IsNullOrEmpty(createCreditRequest.Location);
            bool creditRiskLevelInvalid = string.IsNullOrEmpty(createCreditRequest.CreditRiskLevel);

            if (frequencyInvalid || feesInvalid || sourceInvalid || tokenInvalid
              || userIdInvalid || authMethodInvalid || locationInvalid || userNameInvalid || creditRiskLevelInvalid)
            {
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);
            }
        }

        /// <summary>
        /// Try sign the prommisory note file.
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="promissoryNoteRequest"></param>
        /// <param name="promisoryNoteFile"></param>
        /// <returns></returns>
        private async Task TrySignAsync(CreditMaster creditMaster, PromissoryNoteRequest promissoryNoteRequest, byte[] promisoryNoteFile)
        {
            try
            {
                (string Authority, string Id) = await _signatureService.SignAsync(creditMaster, promissoryNoteRequest.FileName, promisoryNoteFile);

                creditMaster.SetCertified(Authority, Id);
            }
            catch (Exception ex)
            {
                await _loggerService.NotifyAsync(creditMaster.Id.ToString(),
                    nameof(TrySignAsync),
                     new
                     {
                         ex
                     },
                     MethodBase.GetCurrentMethod());
            }
        }

        /// <summary>
        /// Resends the credit limit update event for customer list.
        /// </summary>
        /// <param name="customers"></param>
        /// <returns></returns>
        private async Task ResendCreditLimitEventsAsync(IEnumerable<Customer> customers)
        {
            foreach (Customer customer in customers.Distinct(new EntityGuidEqualityComparer<Customer>()))
            {
                await _customerService.ResendCreditLimitUpdateAsync(customer);
            }
        }

        /// <summary>
        /// Sends token to customer
        /// </summary>
        /// <param name="generateTokenRequest"></param>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <param name="tokenResponse"></param>
        /// <param name="creditDetails"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <returns></returns>
        private async Task<TokenResponse> SendToken(GenerateTokenRequest generateTokenRequest, Customer customer, Store store, AppParameters parameters)
        {
            CreditTokenRequest creditTokenRequest = new CreditTokenRequest
            {
                AdditionalData = string.Empty,
                IdDocument = generateTokenRequest.IdDocument,
                CustomerId = customer.Id
            };
            TokenResponse tokenResponse = await _creditCommonsService.GenerateTokenAsync(creditTokenRequest);

            CreditDetailDomainRequest detailRequest = new CreditDetailDomainRequest(customer,
                store, generateTokenRequest.CreditValue, generateTokenRequest.Frequency, parameters);
            detailRequest.SetFeesByMonths(generateTokenRequest.Months);
            CreditDetailResponse creditDetails = _creditUsesCase.GetCreditDetails(detailRequest);

            int decimalNumbersRound = parameters.DecimalNumbersRound;

            if (store.GetSendTokenSms && customer.GetSendTokenSms)
            {
                string creditTokenSmsNotificationTemplate;

                if (generateTokenRequest.IsRefinancing)
                {
                    creditTokenSmsNotificationTemplate = await _templatesService.GetAsync("RefinancingTokenSmsNotificationTemplate.txt");
                }
                else
                {
                    creditTokenSmsNotificationTemplate = parameters.VirtualSalesTokenSources.Split(",").Contains(generateTokenRequest.Source) ?
                        await _templatesService.GetAsync("VirtualSalesCreditTokenSmsNotificationTemplate.txt") : await _templatesService.GetAsync("CreditTokenSmsNotificationTemplate.txt");
                }

                string token = int.Parse(tokenResponse.Token.Value).ToString("00-00-00");

                SmsNotificationRequest smsNotificationRequest = _creditUsesCase.GetTokenSmsNotification(creditTokenSmsNotificationTemplate, customer,
                    store, creditDetails, generateTokenRequest.Months, generateTokenRequest.Frequency, token, decimalNumbersRound);

                await _creditCommonsService.Commons.Notification.SendSmsAsync(smsNotificationRequest, "NotifySms_CreditToken");
            }

            if (store.GetSendTokenMail && customer.GetSendTokenMail)
            {
                MailNotificationRequest mailNotificationRequest = _creditUsesCase.GetTokenMailNotification(customer, store, creditDetails,
                    generateTokenRequest.Months, generateTokenRequest.Frequency,
                    tokenResponse.Token.Value, decimalNumbersRound);

                await _creditCommonsService.Commons.Notification.SendMailAsync(mailNotificationRequest, "NotifyMail_CreditToken");
            }

            return tokenResponse;
        }

        /// <summary>
        /// Builds a create credit response object with a given params and credit
        /// </summary>
        /// <param name="createCreditRequest"></param>
        /// <param name="credit"></param>
        /// <returns></returns>
        private async Task<CreateCreditResponse> CreateCreateCreditResponse(VerifyCreditCreationRequest createCreditRequest, CreditMaster credit)
        {
            ValidateCreditExistence(credit);

            CreditDetailResponse creditDetail = await GetCreditDetailsAsync(
                                                    new RequiredInitialValuesForCreditDetail()
                                                    {
                                                        creditValue = createCreditRequest.CreditValue,
                                                        frequency = createCreditRequest.Frequency,
                                                        idDocument = createCreditRequest.IdDocument,
                                                        storeId = createCreditRequest.StoreId,
                                                        typeDocument = createCreditRequest.TypeDocument,
                                                        months = createCreditRequest.Months
                                                    });

            return new CreateCreditResponse()
            {
                IdDocument = createCreditRequest.IdDocument,
                TypeDocument = createCreditRequest.TypeDocument,
                StoreId = createCreditRequest.StoreId,
                CreditNumber = credit.GetCreditNumber,
                CreditId = credit.Id,
                DownPaymentId = null,
                EffectiveAnnualRate = credit.GetEffectiveAnnualRate,
                Fees = creditDetail.Fees,
                DownPayment = creditDetail.DownPayment,
                TotalFeeValue = creditDetail.TotalFeeValue,
                CreditValue = creditDetail.CreditValue,
                AssuranceValue = creditDetail.AssuranceValue,
                InterestRate = creditDetail.InterestRate,
                TotalInterestValue = creditDetail.TotalInterestValue,
                TotalDownPayment = creditDetail.TotalDownPayment,
                FeeCreditValue = creditDetail.FeeCreditValue,
                AssuranceFeeValue = creditDetail.AssuranceFeeValue,
                AssuranceTotalValue = creditDetail.AssuranceTotalValue,
                AssuranceTaxFeeValue = creditDetail.AssuranceTaxFeeValue,
                AssuranceTaxValue = creditDetail.AssuranceTaxValue,
                DownPaymentPercentage = creditDetail.DownPaymentPercentage,
                AssurancePercentage = creditDetail.AssurancePercentage,
                AssuranceTotalFeeValue = creditDetail.AssuranceTotalFeeValue,
                TotalPaymentValue = creditDetail.TotalPaymentValue
            };
        }

        /// <summary>
        /// Validates if the credits exists
        /// </summary>
        /// <param name="credit"></param>
        private void ValidateCreditExistence(CreditMaster credit)
        {
            if (credit == null)
                throw new BusinessException(nameof(BusinessResponse.CreditNotActive), (int)BusinessResponse.CreditNotActive);
        }

        /// <summary>
        /// Finds the credit in the database
        /// </summary>
        /// <param name="createCreditRequest"></param>
        /// <returns></returns>
        private async Task<CreditMaster> FindCredit(VerifyCreditCreationRequest createCreditRequest)
        {
            Customer customer = await _customerService.GetAsync(createCreditRequest.IdDocument, createCreditRequest.TypeDocument,
                                                            CustomerReadingFields.CreditDetails, ProfileReadingFields.MandatoryDownPayment);
            List<CreditMaster> credits = await _creditMasterRepository.GetActiveCreditsWithTokenAsync(customer, createCreditRequest.StoreId);

            return credits.Where(x => x.GetToken == createCreditRequest.Token && x.GetCreditDateComplete >= DateTime.Now.Add(-new TimeSpan(0, 0, _credinetAppSettings.TimePeriodVerifyCreationMinutes, 0)))
                          .FirstOrDefault(x => createCreditRequest.Invoice == null || createCreditRequest.Invoice == x.GetCreditInvoice);
        }

        /// <summary>
        /// Indicates if the client is simulated
        /// </summary>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        private bool IsSimulatedClient(string idDocument)
        {
            return idDocument == _credinetAppSettings.SimulatedClientIdDocument;
        }

        private string GetTemplateByResult(int agentAnalysisResultId)
        {
            switch ((AgentAnalysisResults)agentAnalysisResultId)
            {
                case AgentAnalysisResults.RejectedOnSuspicion:
                    return _credinetAppSettings.CustomerRejectNotificationTemplate;

                case AgentAnalysisResults.RejectedByPhone:
                    return _credinetAppSettings.CustomerRejectCallNotificationTemplate;

                case AgentAnalysisResults.Pending:
                default:
                    return _credinetAppSettings.CustomerCallNotificationTemplate;
            }
        }

        /// <summary>
        /// Store has risk level calculation
        /// </summary>
        /// <param name="generateTokenRequest"></param>
        /// <param name="parameters"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        private static bool HasRiskLevelCalculation(Store store)
            => store.GetHasRiskCalculation;

        private async Task ResendNotificationCreditCreation(CreditMaster creditMaster)
        {
            if (!creditMaster.IsActive())
                throw new BusinessException(nameof(BusinessResponse.CreditNotActive), (int)BusinessResponse.CreditNotActive);

            if (!string.IsNullOrEmpty(creditMaster.GetPromissoryNoteFileName))
                throw new BusinessException(nameof(BusinessResponse.FailedSignaturePromisoryNote), (int)BusinessResponse.FailedSignaturePromisoryNote);

            Store store = await _storeService.GetAsync(creditMaster.GetStoreId, StoreReadingFields.CreditDetails, loadProductCategory: true);

            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            SimulatedCreditRequest simulationRequest = new SimulatedCreditRequest(store, creditMaster.Current.GetCreditValue, creditMaster.Current.GetFrequency, parameters);
            simulationRequest.SetFees(creditMaster.Current.GetFees);
            CreditDetailResponse creditDetails = _creditUsesCase.GetCreditDetails(simulationRequest);

            CreateCreditResponse createCreditResponse = new CreateCreditResponse
            {
                CreditId = creditMaster.Id,
                CreditNumber = creditMaster.GetCreditNumber,
                CreditValue = creditMaster.Current.GetCreditValue,
                EffectiveAnnualRate = store.GetEffectiveAnnualRate,
                Fees = creditMaster.Current.GetFees,
                TotalFeeValue = creditDetails.TotalFeeValue,
                AssuranceValue = creditDetails.AssuranceValue,
                IdDocument = creditMaster.Customer.IdDocument,
                TypeDocument = creditMaster.Customer.DocumentType,
                DownPayment = creditDetails.DownPayment,
                InterestRate = creditDetails.InterestRate,
                TotalInterestValue = creditDetails.TotalInterestValue,
                TotalDownPayment = creditDetails.TotalDownPayment,
                FeeCreditValue = creditDetails.FeeCreditValue,
                AssuranceFeeValue = creditDetails.AssuranceFeeValue,
                AssuranceTotalValue = creditDetails.AssuranceTotalValue,
                AssuranceTaxValue = creditDetails.AssuranceTaxValue,
                AssuranceTaxFeeValue = creditDetails.AssuranceTaxFeeValue,
                DownPaymentPercentage = creditDetails.DownPaymentPercentage,
                AssurancePercentage = creditDetails.AssurancePercentage,
                AssuranceTotalFeeValue = creditDetails.AssuranceTotalFeeValue,
                AlternatePayment = creditMaster.Current.GetAlternatePayment,
                CreditMaster = creditMaster,
                StoreId = creditMaster.GetStoreId,
                TotalPaymentValue = creditDetails.TotalPaymentValue
            };

            await _creditCommonsService.SendCreationCommandAsync(createCreditResponse);
        }

        private async Task TrySendNotificationsCreditCreation(CreateCreditResponse createCreditResponse)
        {
            try
            {
                await _creditCommonsService.SendCreationCommandAsync(createCreditResponse);
            }
            catch (Exception ex)
            {
                object logDetailException = new { requestToSend = createCreditResponse, exception = ex };
                await _loggerService.NotifyAsync(createCreditResponse.CreditId.ToString(), nameof(TrySendNotificationsCreditCreation), logDetailException,
                    MethodBase.GetCurrentMethod());
                await DeleteCreditAsync(createCreditResponse.CreditMaster);
            }
        }

        /// <summary>
        /// Try to notify the credit creation through messaging.
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <param name="createCreditTransactionResponse"></param>
        /// <param name="createCreditResponse"></param>
        /// <returns></returns>
        private async Task TrySendCreditCreationAsync(Customer customer, Store store, CreateCreditTransactionResponse createCreditTransactionResponse,
            CreateCreditResponse createCreditResponse)
        {
            try
            {
                await _creditCommonsService.SendEventAsync(createCreditResponse.CreditMaster, customer, store,
                    createCreditTransactionResponse.CreateCreditTransaction);
            }
            catch (Exception ex)
            {
                await _loggerService.NotifyAsync(createCreditResponse.CreditId.ToString(), nameof(TrySendCreditCreationAsync), new { createCreditTransactionResponse, createCreditResponse, ex },
                    MethodBase.GetCurrentMethod());
                await DeleteCreditAsync(createCreditTransactionResponse.CreateCreditTransaction.CreditMaster);
            }
        }

        private async Task DeleteCreditAsync(CreditMaster creditMaster)
        {
            try
            {
                await _creditMasterRepository.ExcecuteOnTransactionAsync(async transaction =>
                {
                    await _creditUsesCase.DeleteAsync(creditMaster, transaction);
                });
            }
            catch
            {

            }
        }

        #endregion Private
    }
}