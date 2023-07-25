using credinet.comun.models.Credits;
using credinet.comun.models.Study;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Queries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace Sc.Credits.Domain.Managment.Services.Customers
{
    /// <summary>
    /// Customer service contract
    /// </summary>
    public interface ICustomerService
    {
        /// <summary>
        /// Create or update
        /// </summary>
        /// <param name="createCustomerRequest"></param>
        /// <param name="eventDate"></param>
        /// <returns></returns>
        Task CreateOrUpdateAsync(CustomerRequest createCustomerRequest, DateTime eventDate);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="fields"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task UpdateAsync(Customer customer, IEnumerable<Field> fields, Transaction transaction = null);

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fields"></param>
        /// <param name="profileFields"></param>
        /// <returns></returns>
        Task<Customer> GetAsync(Guid id, IEnumerable<Field> fields,
            IEnumerable<Field> profileFields = null);

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentType"></param>
        /// <param name="fields"></param>
        /// <param name="profileFields"></param>
        /// <returns></returns>
        Task<Customer> GetAsync(string idDocument, string documentType, IEnumerable<Field> fields,
            IEnumerable<Field> profileFields = null);

        /// <summary>
        /// Get active
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentType"></param>
        /// <param name="fields"></param>
        /// <param name="profileFields"></param>
        /// <returns></returns>
        Task<Customer> GetActiveAsync(string idDocument, string documentType, IEnumerable<Field> fields,
            IEnumerable<Field> profileFields = null);

        /// <summary>
        /// Try send credit limit update
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        Task TrySendCreditLimitUpdateAsync(Customer customer);

        /// <summary>
        /// Resends a credit limit update event.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        Task ResendCreditLimitUpdateAsync(Customer customer);

        /// <summary>
        /// Try update credit limit
        /// </summary>
        /// <param name="creditLimitResponse"></param>
        /// <param name="eventDate"></param>
        /// <returns></returns>
        Task TryCreditLimitUpdateAsync(CreditLimitResponse creditLimitResponse, DateTime eventDate);

        /// <summary>
        /// Update status
        /// </summary>
        /// <param name="studyResponse"></param>
        /// <returns></returns>
        Task UpdateStatusAsync(StudyResponse studyResponse);

        /// <summary>
        /// Update mail
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentType"></param>
        /// <param name="email"></param>
        /// <param name="validatedMail"></param>
        /// <returns></returns>
        Task UpdateMailAsync(string idDocument, string documentType, string email, bool validatedMail);

        /// <summary>
        /// Update mobile
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentType"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        Task UpdateMobileAsync(string idDocument, string documentType, string mobile);

        /// <summary>
        /// Create or update status
        /// </summary>
        /// <param name="request"></param>
        /// <param name="eventDate"></param>
        /// <returns></returns>
        Task CreateOrUpdateStatusAsync(CustomerRequest request, DateTime eventDate);

        /// <summary>
        /// Reject credit limit
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentType"></param>
        /// <param name="userName"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> RejectCreditLimit(string idDocument, string documentType, string userName, string userId);

        /// <summary>
        /// Gets the customer by document.
        /// </summary>
        /// <param name="idDocument">The identifier document.</param>
        /// <returns></returns>
        Task<Customer> GetCustomerByDocument(string idDocument, IEnumerable<Field> fields, IEnumerable<Field> profile);
    }
}