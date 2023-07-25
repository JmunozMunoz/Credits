using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Customers;
using Sc.Credits.Domain.Managment.Services.Stores;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Common.Gateway;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.Domain.Model.Stores;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Managment.Services.Credits
{
    /// <summary>
    /// Credit commons service is an implementation of <see cref="ICreditCommonsService"/>
    /// </summary>
    public class CreditCommonsService
        : ICreditCommonsService
    {
        private readonly ICreditNotificationRepository _creditNotificationRepository;
        private readonly ITokenRepository _tokenRepository;

        /// <summary>
        /// <see cref="ICreditCommonsService.Commons"/>
        /// </summary>
        public ICommons Commons { get; }

        /// <summary>
        /// <see cref="ICreditCommonsService.CustomerService"/>
        /// </summary>
        public ICustomerService CustomerService { get; }

        /// <summary>
        /// <see cref="ICreditCommonsService.StoreService"/>
        /// </summary>
        public IStoreService StoreService { get; }

        /// <summary>
        /// Creates a new instance of <see cref="CreditCommonsService"/>
        /// </summary>
        /// <param name="commons"></param>
        /// <param name="customerService"></param>
        /// <param name="creditNotificationRepository"></param>
        /// <param name="tokenRepository"></param>
        /// <param name="promissoryNotePdf"></param>
        /// <param name="storeService"></param>
        public CreditCommonsService(ICommons commons,
            ICustomerService customerService,
            ICreditNotificationRepository creditNotificationRepository,
            ITokenRepository tokenRepository,
            IStoreService storeService)
        {
            Commons = commons;
            CustomerService = customerService;
            StoreService = storeService;
            _creditNotificationRepository = creditNotificationRepository;
            _tokenRepository = tokenRepository;
        }

        /// <summary>
        /// <see cref="ICreditCommonsService.SendEventAsync(CreditMaster, Customer, Store, Credit,
        /// TransactionType, string)"/>
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <param name="credit"></param>
        /// <param name="transactionType"></param>
        /// <param name="complementEventName"></param>
        /// <returns></returns>
        public async Task SendEventAsync(CreditMaster creditMaster, Customer customer, Store store, Credit credit = null,
            TransactionType transactionType = null, string complementEventName = null)
        {
            await CustomerService.TrySendCreditLimitUpdateAsync(customer);

            await _creditNotificationRepository.SendEventAsync(creditMaster, customer, store, credit, transactionType, complementEventName);
        }

        /// <summary>
        /// <see cref="ICreditCommonsService.SendCreationCommandAsync(CreateCreditResponse)"/>
        /// </summary>
        /// <param name="createCreditResponse"></param>
        /// <returns></returns>
        public async Task SendCreationCommandAsync(CreateCreditResponse createCreditResponse) =>
            await _creditNotificationRepository.NotifyCreationAsync(createCreditResponse);

        /// <summary>
        /// <see cref="ICreditCommonsService.GenerateTokenAsync(CreditTokenRequest)"/>
        /// </summary>
        /// <param name="creditTokenRequest"></param>
        /// <returns></returns>
        public async Task<TokenResponse> GenerateTokenAsync(CreditTokenRequest creditTokenRequest) =>
            await _tokenRepository.GenerateCreditTokenAsync(creditTokenRequest);

        /// <summary>
        /// <see cref="ICreditCommonsService.ValidateTokenAsync(CreateCreditRequest, Customer)"/>
        /// </summary>
        /// <param name="createCreditRequest"></param>
        /// <param name="customer"></param>
        /// <returns></returns>
        public async Task ValidateTokenAsync(CreateCreditRequest createCreditRequest, Customer customer)
        {
            CreditTokenRequest creditTokenRequest = new CreditTokenRequest
            {
                AdditionalData = createCreditRequest.AuthMethod == (int)AuthMethods.Token
                                        ?
                                        string.Empty
                                        :
                                        $"{createCreditRequest.Fees}|{createCreditRequest.StoreId}",
                IdDocument = createCreditRequest.IdDocument,
                Token = createCreditRequest.Token,
                CustomerId = customer.Id
            };

            if (Commons.CredinetAppSettings.ValidateTokenOnCreate && !await IsValidTokenAsync(creditTokenRequest))
            {
                throw new BusinessException(nameof(BusinessResponse.TokenIsNotValid), (int)BusinessResponse.TokenIsNotValid);
            }
        }

        /// <summary>
        /// Is valid token
        /// </summary>
        /// <param name="creditTokenRequest"></param>
        /// <returns></returns>
        private async Task<bool> IsValidTokenAsync(CreditTokenRequest creditTokenRequest) =>
           await _tokenRepository.IsValidCreditTokenAsync(creditTokenRequest);

        /// <summary>
        /// <see cref="ICreditCommonsService.TokenCallRequestAsync(CreditTokenCallRequest)"/>
        /// </summary>
        /// <param name="creditTokenRequest"></param>
        /// <returns></returns>
        public async Task TokenCallRequestAsync(CreditTokenCallRequest creditTokenRequest) =>
           await _tokenRepository.CreditTokenCallRequestAsync(creditTokenRequest);

        /// <summary>
        /// <see cref="ICreditCommonsService.NewCreateCreditDomainRequestAsync(CreateCreditRequest,
        /// Customer, Store)"/>
        /// </summary>
        /// <param name="createCreditRequest"></param>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        public async Task<CreateCreditDomainRequest> NewCreateCreditDomainRequestAsync(CreateCreditRequest createCreditRequest, Customer customer,
            Store store)
        {
            TransactionType createCreditTransactionType = await Commons.AppParameters.GetTransactionTypeAsync((int)TransactionTypes.CreateCredit);

            PaymentType ordinaryPaymentType = await Commons.AppParameters.GetPaymentTypeAsync((int)PaymentTypes.Ordinary);

            Status status = await Commons.AppParameters.GetStatusAsync((int)Statuses.Active);

            Source source = await Commons.AppParameters.GetSourceAsync(createCreditRequest.Source);

            AuthMethod authMethod = await Commons.AppParameters.GetAuthMethodAsync(createCreditRequest.AuthMethod);

            AppParameters appParameters = await Commons.AppParameters.GetAppParametersAsync();

            return new CreateCreditDomainRequest(createCreditRequest, customer, store, appParameters)
                .SetCreateCreditTransactionType(createCreditTransactionType)
                .SetOrdinaryPaymentType(ordinaryPaymentType)
                .SetSeedMasters(status, source, authMethod);
        }
    }
}