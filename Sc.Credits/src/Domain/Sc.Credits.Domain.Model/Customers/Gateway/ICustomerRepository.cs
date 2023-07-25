using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Domain.Model.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Model.Customers.Gateway
{
    /// <summary>
    /// The customer repository contract
    /// </summary>
    public interface ICustomerRepository
        : ICommandRepository<Customer>
    {
        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fields"></param>
        /// <param name="profileFields"></param>
        /// <returns></returns>
        Task<Customer> GetByIdAsync(object id, IEnumerable<Field> fields, IEnumerable<Field> profileFields = null);

        /// <summary>
        /// Get by document info
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentType"></param>
        /// <param name="fields"></param>
        /// <param name="profileFields"></param>
        /// <param name="entitiesForMerge"></param>
        /// <returns></returns>
        Task<Customer> GetByDocumentInfoAsync(string idDocument, string documentType, IEnumerable<Field> fields = null,
            IEnumerable<Field> profileFields = null, params object[] entitiesForMerge);

        /// <summary>
        /// Gets the customer by document.
        /// </summary>
        /// <param name="idDocument">The identifier document.</param>
        /// <param name="fields">The fields.</param>
        /// <param name="profileFields"></param>
        /// <returns></returns>
        Task<Customer> GetCustomerByDocument(string idDocument, IEnumerable<Field> fields = null, IEnumerable<Field> profileFields = null);
    }
}