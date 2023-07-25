using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.Model.Stores;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Managment.Services.Stores
{
    /// <summary>
    /// Store service contract
    /// </summary>
    public interface IStoreService
    {
        /// <summary>
        /// Create or update
        /// </summary>
        /// <param name="storeRequest"></param>
        /// <returns></returns>
        Task CreateOrUpdateAsync(StoreRequest storeRequest);

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fields"></param>
        /// <param name="loadAssuranceCompany"></param>
        /// <param name="loadPaymentType"></param>
        /// <param name="loadProductCategory"></param>
        /// <param name="loadCity"></param>
        /// <param name="loadBusinessGroup"></param>
        /// <returns></returns>
        Task<Store> GetAsync(string id, IEnumerable<Field> fields, bool loadAssuranceCompany = false,
            bool loadPaymentType = false, bool loadProductCategory = false, bool loadCity = false, bool loadBusinessGroup = false);

        /// <summary>
        /// Get assurance company
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<AssuranceCompany> GetAssuranceCompanyAsync(long id);
    }
}