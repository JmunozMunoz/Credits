using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Customers;
using Sc.Credits.Domain.Managment.Services.Stores;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Stores;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Managment.Services.Credits
{
    /// <summary>
    /// Credit commons service contract
    /// </summary>
    public interface ICreditCommonsService
    {
        /// <summary>
        /// <see cref="ICommons"/>
        /// </summary>
        ICommons Commons { get; }

        /// <summary>
        /// <see cref="ICustomerService"/>
        /// </summary>
        ICustomerService CustomerService { get; }

        /// <summary>
        /// <see cref="IStoreService"/>
        /// </summary>
        IStoreService StoreService { get; }

        /// <summary>
        /// Send event
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <param name="credit"></param>
        /// <param name="transactionType"></param>
        /// <param name="complementEventName"></param>
        /// <returns></returns>
        Task SendEventAsync(CreditMaster creditMaster, Customer customer, Store store, Credit credit = null,
            TransactionType transactionType = null, string complementEventName = null);

        /// <summary>
        /// Send creation command
        /// </summary>
        /// <param name="createCreditResponse"></param>
        /// <returns></returns>
        Task SendCreationCommandAsync(CreateCreditResponse createCreditResponse);

        /// <summary>
        /// Generate token
        /// </summary>
        /// <param name="creditTokenRequest"></param>
        /// <returns></returns>
        Task<TokenResponse> GenerateTokenAsync(CreditTokenRequest creditTokenRequest);

        /// <summary>
        /// Validate token
        /// </summary>
        /// <param name="createCreditRequest"></param>
        /// <param name="customer"></param>
        /// <returns></returns>
        Task ValidateTokenAsync(CreateCreditRequest createCreditRequest, Customer customer);

        /// <summary>
        /// Token call request
        /// </summary>
        /// <param name="creditTokenRequest"></param>
        /// <returns></returns>
        Task TokenCallRequestAsync(CreditTokenCallRequest creditTokenRequest);

        /// <summary>
        /// New create credit domain request
        /// </summary>
        /// <param name="createCreditRequest"></param>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        Task<CreateCreditDomainRequest> NewCreateCreditDomainRequestAsync(CreateCreditRequest createCreditRequest, Customer customer,
            Store store);
    }
}