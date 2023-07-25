using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Credits;
using Sc.Credits.Domain.Managment.Services.Customers;
using Sc.Credits.Domain.Managment.Services.Stores;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Customers.Queries.Reading;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.Domain.Model.Refinancings;
using Sc.Credits.Domain.Model.Refinancings.Gateway;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Domain.Model.Stores.Queries.Reading;
using Sc.Credits.Domain.UseCase.Refinancings;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Sc.Credits.Domain.Managment.Services.Refinancings
{
    /// <summary>
    /// Refinancing service is an implementation of <see cref="IRefinancingService"/>
    /// </summary>
    public class RefinancingService
        : IRefinancingService
    {
        private readonly IRefinancingRepository _refinancingRepository;
        private readonly ICreditCommonsService _creditCommonsService;
        private readonly ICreditMasterRepository _creditMasterRepository;
        private readonly IRefinancingUsesCase _refinancingUsesCase;
        private readonly IAppParametersService _appParametersService;
        private readonly ICreditPaymentService _creditPaymentService;
        private readonly ICreditService _creditService;
        private readonly ICustomerService _customerService;
        private readonly IStoreService _storeService;
        private readonly CredinetAppSettings _credinetAppSettings;

        /// <summary>
        /// Creates a new instance of <see cref="RefinancingService"/>
        /// </summary>
        /// <param name="creditCommons"></param>
        /// <param name="refinancingSimpleRepository"></param>
        /// <param name="refinancingUsesCase"></param>
        /// <param name="creditPaymentService"></param>
        /// <param name="creditService"></param>
        public RefinancingService(CreditCommons creditCommons,
            IRefinancingRepository refinancingSimpleRepository,
            IRefinancingUsesCase refinancingUsesCase,
            ICreditPaymentService creditPaymentService,
            ICreditService creditService)
        {
            _refinancingRepository = refinancingSimpleRepository;
            _creditCommonsService = creditCommons.Service;
            _creditMasterRepository = creditCommons.CreditMasterRepository;
            _refinancingUsesCase = refinancingUsesCase;
            _appParametersService = _creditCommonsService.Commons.AppParameters;
            _creditPaymentService = creditPaymentService;
            _creditService = creditService;
            _customerService = _creditCommonsService.CustomerService;
            _storeService = _creditCommonsService.StoreService;
            _credinetAppSettings = _creditCommonsService.Commons.CredinetAppSettings;
        }

        #region IRefinancingService Members

        /// <summary>
        /// <see cref="IRefinancingService.GetCustomerCreditsAsync(CustomerCreditsRequest)"/>
        /// </summary>
        /// <param name="customerCreditsRequest"></param>
        /// <returns></returns>
        public async Task<CustomerCreditsResponse> GetCustomerCreditsAsync(CustomerCreditsRequest customerCreditsRequest)
        {
            ValidateCustomerCreditsRequest(customerCreditsRequest);

            RefinancingApplication application = await GetApplicationAsync(customerCreditsRequest.ApplicationId);

            Customer customer = await _customerService.GetActiveAsync(customerCreditsRequest.IdDocument,
                customerCreditsRequest.DocumentType, CustomerReadingFields.Refinancing);

            List<CreditMaster> activeCredits = await _creditMasterRepository.GetActiveCreditsAsync(customer);

            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            return _refinancingUsesCase.CustomerCredits(customer, activeCredits, application, parameters);
        }

        /// <summary>
        /// <see cref="IRefinancingService.CalculateFeesAsync(CalculateFeesRequest)"/>
        /// </summary>
        /// <param name="calculateFeesRequest"></param>
        /// <returns></returns>
        public async Task<CalculateFeesResponse> CalculateFeesAsync(CalculateFeesRequest calculateFeesRequest)
        {
            ValidateCalculateFess(calculateFeesRequest);

            RefinancingApplication application = await GetApplicationAsync(calculateFeesRequest.ApplicationId);

            Store store = await _storeService.GetAsync(RefinancingParams.StoreId(_credinetAppSettings),
                StoreReadingFields.CreditDetails, loadProductCategory: true);

            Customer customer = await _customerService.GetActiveAsync(calculateFeesRequest.IdDocument,
                calculateFeesRequest.DocumentType, CustomerReadingFields.PaymentFees, ProfileReadingFields.MandatoryDownPayment);

            List<CreditMaster> refinancingCreditMasters = await GetRefinancingCreditsAsync(calculateFeesRequest.CreditIds,
                customer);

            ValidateCreditsCount(calculateFeesRequest.CreditIds, refinancingCreditMasters);

            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            return _refinancingUsesCase.FeesResponse(customer, store, refinancingCreditMasters, parameters, application);
        }

        /// <summary>
        /// <see cref="IRefinancingService.GetCustomerCreditsAsync(CustomerCreditsRequest)"/>
        /// </summary>
        /// <param name="refinancingCreditRequest"></param>
        /// <returns></returns>
        public async Task<RefinancingCreditResponse> CreateCreditAsync(RefinancingCreditRequest refinancingCreditRequest)
        {
            ValidateRefinancingCredit(refinancingCreditRequest);

            RefinancingApplication application = await GetApplicationAsync(refinancingCreditRequest.ApplicationId);

            Store store = await _storeService.GetAsync(RefinancingParams.StoreId(_credinetAppSettings),
                StoreReadingFields.BusinessGroupIdentifiers, loadAssuranceCompany: true, loadPaymentType: true, loadProductCategory: true, loadBusinessGroup: true);

            Customer customer = await _customerService.GetActiveAsync(refinancingCreditRequest.IdDocument,
                refinancingCreditRequest.DocumentType, CustomerReadingFields.CreateCredit, ProfileReadingFields.MandatoryDownPayment);

            List<CreditMaster> refinancingCreditMasters = await GetRefinancingCreditsAsync(refinancingCreditRequest.CreditIds,
                customer, false);

            ValidateCreditsCount(refinancingCreditRequest.CreditIds, refinancingCreditMasters);

            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            TransactionType paymentTransactionType = await _appParametersService.GetTransactionTypeAsync((int)TransactionTypes.Payment);

            Status activeStatus = await _appParametersService.GetStatusAsync((int)Statuses.Active);
            Status paidStatus = await _appParametersService.GetStatusAsync((int)Statuses.Paid);
           
            CreateCreditRequest createCreditRequest = CreateCreditRequest.FromRefinancing(refinancingCreditRequest,
                RefinancingParams.StoreId(_credinetAppSettings));

            createCreditRequest.CreditRiskLevel = parameters.DefaultRiskLevel;

            await _creditCommonsService.ValidateTokenAsync(createCreditRequest, customer);

            CreateCreditDomainRequest createCreditDomainRequest = await _creditCommonsService.NewCreateCreditDomainRequestAsync(createCreditRequest,
                customer, store);

            RefinancingDomainRequest refinancingDomainRequest = new RefinancingDomainRequest(refinancingCreditRequest,
                    createCreditDomainRequest, customer, store, parameters)
                .SetRefinancingParams(refinancingCreditMasters, application, _credinetAppSettings, false)
                .SetPaymentParams(paymentTransactionType, activeStatus, paidStatus);

            RefinancingCreditResponse response = await _creditMasterRepository.ExcecuteOnTransactionAsync(async transaction =>
            {
                RefinancingCreditResponse refinancingCreditResponse = await _refinancingUsesCase.RefinanceAsync(refinancingDomainRequest, transaction, false);

                await SaveLogAsync(refinancingDomainRequest, refinancingCreditResponse, transaction);

                return refinancingCreditResponse;
            });

            await NotifyRefinancingAsync(response, customer, store, parameters);

            return response;
        }

        /// <summary>
        /// <see cref="IRefinancingService.GenerateTokenAsync(GenerateTokenRequest)"/>
        /// </summary>
        /// <param name="generateTokenRequest"></param>
        /// <returns></returns>
        public async Task<TokenResponse> GenerateTokenAsync(GenerateTokenRequest generateTokenRequest) =>
            await _creditService.GenerateTokenAsync(generateTokenRequest.SetRefinancing(_credinetAppSettings));

        #endregion IRefinancingService Members

        #region Private

        /// <summary>
        /// Validate customer credit request
        /// </summary>
        /// <param name="customerCreditsRequest"></param>
        private void ValidateCustomerCreditsRequest(CustomerCreditsRequest customerCreditsRequest)
        {
            if (customerCreditsRequest == null
                || customerCreditsRequest.ApplicationId == Guid.Empty
                || string.IsNullOrEmpty(customerCreditsRequest.DocumentType)
                || string.IsNullOrEmpty(customerCreditsRequest.IdDocument))
            {
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);
            }
        }

        /// <summary>
        /// Get application
        /// </summary>
        /// <param name="applicationId"></param>
        private async Task<RefinancingApplication> GetApplicationAsync(Guid applicationId)
        {
            RefinancingApplication refinancingApplication = await _refinancingRepository.GetApplicationAsync(applicationId);

            if (refinancingApplication == null)
            {
                throw new BusinessException(nameof(BusinessResponse.RefinancingApplicationInvalid), (int)BusinessResponse.RefinancingApplicationInvalid);
            }

            return refinancingApplication;
        }

        /// <summary>
        /// Validate calculate fees
        /// </summary>
        /// <param name="calculateFeesRequest"></param>
        private void ValidateCalculateFess(CalculateFeesRequest calculateFeesRequest)
        {
            if (calculateFeesRequest == null
             || calculateFeesRequest.ApplicationId == Guid.Empty
             || string.IsNullOrEmpty(calculateFeesRequest.DocumentType)
             || string.IsNullOrEmpty(calculateFeesRequest.IdDocument)
             || calculateFeesRequest.CreditIds == null
             || !calculateFeesRequest.CreditIds.Any())
            {
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);
            }
        }

        /// <summary>
        /// Get refinancing credits
        /// </summary>
        /// <param name="creditIds"></param>
        /// <param name="customer"></param>
        /// <returns></returns>
        private async Task<List<CreditMaster>> GetRefinancingCreditsAsync(Guid[] creditIds, Customer customer, bool setCreditLimit = true) =>
            await _creditMasterRepository.GetWithCurrentAsync(ids: creditIds.ToList(), customer,
                storeFields: StoreReadingFields.Refinancing, transactionStoreFields: StoreReadingFields.Refinancing,
                statuses: new[] { Statuses.Active }, setCreditLimit);

        /// <summary>
        /// Validate refinancing credit
        /// </summary>
        /// <param name="refinancingCreditRequest"></param>
        private void ValidateRefinancingCredit(RefinancingCreditRequest refinancingCreditRequest)
        {
            if (refinancingCreditRequest == null
             || refinancingCreditRequest.ApplicationId == Guid.Empty
             || string.IsNullOrEmpty(refinancingCreditRequest.DocumentType)
             || string.IsNullOrEmpty(refinancingCreditRequest.IdDocument)
             || refinancingCreditRequest.CreditIds == null
             || !refinancingCreditRequest.CreditIds.Any()
             || string.IsNullOrEmpty(refinancingCreditRequest.ReferenceText)
             || string.IsNullOrEmpty(refinancingCreditRequest.ReferenceCode)
             || refinancingCreditRequest.Fees <= 0
             || refinancingCreditRequest.Source <= 0
             || string.IsNullOrEmpty(refinancingCreditRequest.Token)
             || refinancingCreditRequest.AuthMethod <= 0
             || string.IsNullOrEmpty(refinancingCreditRequest.UserId)
             || string.IsNullOrEmpty(refinancingCreditRequest.UserName))
            {
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);
            }

            RefinancingParams.ValidateSource(_credinetAppSettings, refinancingCreditRequest.Source);
        }

        /// <summary>
        /// Validate credits count
        /// </summary>
        /// <param name="creditIds"></param>
        /// <param name="refinancingCreditMasters"></param>
        private void ValidateCreditsCount(Guid[] creditIds, List<CreditMaster> refinancingCreditMasters)
        {
            if (refinancingCreditMasters.Count != creditIds.Count())
            {
                throw new BusinessException(nameof(BusinessResponse.CreditsAreNotValidForRefinancing),
                    (int)BusinessResponse.CreditsAreNotValidForRefinancing);
            }
        }

        /// <summary>
        /// Save log
        /// </summary>
        /// <param name="refinancingDomainRequest"></param>
        /// <param name="refinancingCreditResponse"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private async Task SaveLogAsync(RefinancingDomainRequest refinancingDomainRequest,
            RefinancingCreditResponse refinancingCreditResponse, Transaction transaction)
        {
            RefinancingLog log = _refinancingUsesCase.CreateLog(refinancingDomainRequest, refinancingCreditResponse);

            await _refinancingRepository.AddLogAsync(log, transaction);
        }

        /// <summary>
        /// Notify refinancing
        /// </summary>
        /// <param name="response"></param>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private async Task NotifyRefinancingAsync(RefinancingCreditResponse response, Customer customer, Store store, AppParameters parameters)
        {
            await NotifyPaymentsAsync(response.PaymentCreditResponses, parameters);

            customer.SetCreditLimitUpdated(true);

            await _creditCommonsService.SendEventAsync(response.CreateCreditResponse.CreditMaster, customer, store);

            await _creditCommonsService.SendCreationCommandAsync(response.CreateCreditResponse);
        }

        /// <summary>
        /// Notify payments
        /// </summary>
        /// <param name="paymentCreditResponses"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private async Task NotifyPaymentsAsync(List<PaymentCreditResponse> paymentCreditResponses, AppParameters parameters)
        {
            foreach (PaymentCreditResponse paymentCreditResponse in paymentCreditResponses)
            {
                await _creditPaymentService.PaymentCreditNotifyAsync(paymentCreditResponse, parameters.AssuranceTax,
                    parameters.DecimalNumbersRound);
            }
        }

        #endregion Private
    }
}