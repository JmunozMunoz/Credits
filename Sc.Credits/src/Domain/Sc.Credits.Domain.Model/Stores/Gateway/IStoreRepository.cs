using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Domain.Model.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Model.Stores.Gateway
{
    /// <summary>
    /// The store repository contract
    /// </summary>
    public interface IStoreRepository
        : ICommandRepository<Store>
    {
        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fields"></param>
        /// <param name="loadAssuranceCompany"></param>
        /// <param name="loadPaymentType"></param>
        /// <param name="loadProductCategory"></param>
        /// <param name="loadCity"></param>
        /// <param name="loadBusinessGroup"></param>
        /// <returns></returns>
        Task<Store> GetByIdAsync(string id, IEnumerable<Field> fields, bool loadAssuranceCompany = false,
            bool loadPaymentType = false, bool loadProductCategory = false, bool loadCity=false, bool loadBusinessGroup = false);

        /// <summary>
        /// Get assurance company
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<AssuranceCompany> GetAssuranceCompanyAsync(long id);

        /// <summary>
        /// Get collect type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<CollectType> GetCollectTypeAsync(int id);

        /// <summary>
        /// Get store identification
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        Task<StoreIdentification> GetStoreIdentificationAsync(string storeId);

        /// <summary>
        /// Add store identification
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task AddStoreIdentificationAsync(StoreIdentification entity);

        /// <summary>
        /// Update store identification
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task UpdateStoreIdentificationAsync(StoreIdentification entity);

        /// <summary>
        /// Get business group
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<BusinessGroup> GetBusinessGroupAsync(string id);

        /// <summary>
        /// Add business group
        /// </summary>
        /// <param name="businessGroup"></param>
        /// <returns></returns>
        Task AddBusinessGroupAsync(BusinessGroup businessGroup);

        /// <summary>
        /// Update business group
        /// </summary>
        /// <param name="businessGroup"></param>
        /// <returns></returns>
        Task UpdateBusinessGroupAsync(BusinessGroup businessGroup);
    }
}