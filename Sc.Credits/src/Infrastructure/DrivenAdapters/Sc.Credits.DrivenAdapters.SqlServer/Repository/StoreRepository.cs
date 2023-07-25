using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Locations;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.Model.Queries.Extensions;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Domain.Model.Stores.Gateway;
using Sc.Credits.Domain.Model.Stores.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Connection;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.QueriesKata;
using Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Credits;
using Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Locations;
using Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Stores;
using Sc.Credits.DrivenAdapters.SqlServer.Repository.Base;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace Sc.Credits.DrivenAdapters.SqlServer.Repository
{
    /// <summary>
    /// The default implementation of <see cref="IStoreRepository"/>
    /// </summary>
    public class StoreRepository
        : CommandRepository<Store, StoreFields>, IStoreRepository
    {
        private static readonly StoreQueries _storeQueries = QueriesCatalog.Store;
        private static readonly AssuranceCompanyQueries _assuranceCompanyQueries = QueriesCatalog.AssuranceCompany;
        private static readonly PaymentTypeQueries _paymentTypeQueries = QueriesCatalog.PaymentType;
        private static readonly CollectTypeQueries _collectTypeQueries = QueriesCatalog.CollectType;
        private static readonly StoreIdentificationQueries _storeIdentificationQueries = QueriesCatalog.StoreIdentification;
        private static readonly BusinessGroupQueries _businessGroupQueries = QueriesCatalog.BusinessGroup;
        private static readonly StoreCategoryQueries _storeCategoryQueries = QueriesCatalog.StoreCategory;
        private static readonly CityQueries _cityQueries = QueriesCatalog.City;

        private readonly ISqlDelegatedHandlers<PaymentType> _paymentTypeSqlDelegatedHandlers;
        private readonly ISqlDelegatedHandlers<StoreCategory> _storeCategorySqlDelegatedHandlers;
        private readonly ISqlDelegatedHandlers<AssuranceCompany> _assuranceCompanySqlDelegatedHandlers;
        private readonly ISqlDelegatedHandlers<CollectType> _collectTypeSqlDelegatedHandlers;
        private readonly ISqlDelegatedHandlers<StoreIdentification> _storeIdentificationSqlDelegatedHandlers;
        private readonly ISqlDelegatedHandlers<BusinessGroup> _businessGroupSqlDelegatedHandlers;
        private readonly ISqlDelegatedHandlers<City> _citySqlDelegatedHandlers;

        /// <summary>
        /// Creates new instance of <see cref="StoreRepository"/>
        /// </summary>
        /// <param name="connectionFactory"></param>
        /// <param name="storeSqlDelegatedHandlers"></param>
        /// <param name="paymentTypeSqlDelegatedHandlers"></param>
        /// <param name="storeCategorySqlDelegatedHandlers"></param>
        /// <param name="assuranceCompanySqlDelegatedHandlers"></param>
        /// <param name="collectTypeSqlDelegatedHandlers"></param>
        /// <param name="storeIdentificationSqlDelegatedHandlers"></param>
        /// <param name="businessGroupSqlDelegatedHandlers"></param>
        public StoreRepository(ICreditsConnectionFactory connectionFactory,
            ISqlDelegatedHandlers<Store> storeSqlDelegatedHandlers,
            ISqlDelegatedHandlers<PaymentType> paymentTypeSqlDelegatedHandlers,
            ISqlDelegatedHandlers<StoreCategory> storeCategorySqlDelegatedHandlers,
            ISqlDelegatedHandlers<AssuranceCompany> assuranceCompanySqlDelegatedHandlers,
            ISqlDelegatedHandlers<CollectType> collectTypeSqlDelegatedHandlers,
            ISqlDelegatedHandlers<StoreIdentification> storeIdentificationSqlDelegatedHandlers,
            ISqlDelegatedHandlers<BusinessGroup> businessGroupSqlDelegatedHandlers,
            ISqlDelegatedHandlers<City> citySqlDelegatedHandlers)
            : base(_storeQueries, storeSqlDelegatedHandlers, connectionFactory)
        {
            _paymentTypeSqlDelegatedHandlers = paymentTypeSqlDelegatedHandlers;
            _storeCategorySqlDelegatedHandlers = storeCategorySqlDelegatedHandlers;
            _assuranceCompanySqlDelegatedHandlers = assuranceCompanySqlDelegatedHandlers;
            _collectTypeSqlDelegatedHandlers = collectTypeSqlDelegatedHandlers;
            _storeIdentificationSqlDelegatedHandlers = storeIdentificationSqlDelegatedHandlers;
            _businessGroupSqlDelegatedHandlers = businessGroupSqlDelegatedHandlers;
            _citySqlDelegatedHandlers = citySqlDelegatedHandlers;
        }

        /// <summary>
        /// <see cref="IStoreRepository.GetByIdAsync(string, IEnumerable{Field}, bool, bool, bool)"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="loadAssuranceCompany"></param>
        /// <param name="loadPaymentType"></param>
        /// <param name="loadCity"></param>
        /// <param name="loadBusinessGroup"></param>
        /// <returns></returns>
        public async Task<Store> GetByIdAsync(string id, IEnumerable<Field> fields, bool loadAssuranceCompany = false,
            bool loadPaymentType = false, bool loadProductCategory = false, bool loadCity =false, bool loadBusinessGroup = false) =>
            await ReadUsingConnectionAsync(async (connection) =>
            {
                Store store = await EntitySqlDelegatedHandlers.GetSingleAsync(connection,
                    _storeQueries.ById(id, fields));

                if (loadAssuranceCompany && store != null)
                {
                    long assuranceCompanyId = store.GetAssuranceCompanyId;
                    AssuranceCompany assuranceCompany = await GetAssuranceCompanyAsync(assuranceCompanyId,
                        connection);

                    store.SetAssuranceCompany(assuranceCompany);
                }

                if (loadPaymentType && store != null)
                {
                    int paymentTypeId = store.GetPaymentTypeId;
                    PaymentType paymentType = await _paymentTypeSqlDelegatedHandlers.GetSingleAsync(connection,
                        _paymentTypeQueries.ById(paymentTypeId));

                    store.SetPaymentType(paymentType);
                }

                if (loadProductCategory && store != null)
                {
                    int productsCategoryId = store.GetStoreCategoryId;
                    StoreCategory productsCategories = await _storeCategorySqlDelegatedHandlers.GetSingleAsync(connection,
                        _storeCategoryQueries.ById(productsCategoryId));

                    store.SetStoreCategory(productsCategories);
                }

                if (loadCity && store != null && int.TryParse(store.GetCityId, out int cityId))
                {
                    City city = await _citySqlDelegatedHandlers.GetSingleAsync(connection, _cityQueries.ById(cityId));
                    store.SetCity(city);
                }

                if (loadBusinessGroup && store != null)
                {
                    BusinessGroup businessGroup = await _businessGroupSqlDelegatedHandlers.GetSingleAsync(connection, _businessGroupQueries.ById(store.GetBusinessGroupId));
                    store.SetBusinessGroup(businessGroup);
                }
                return store;
            });

        /// <summary>
        /// Get assurance company
        /// </summary>
        /// <param name="id"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        private async Task<AssuranceCompany> GetAssuranceCompanyAsync(long id, DbConnection connection)
        {
            Func<DbConnection, Task<AssuranceCompany>> ReadFunc(long assuranceId)
            {
                return async (DbConnection conn) => await _assuranceCompanySqlDelegatedHandlers.GetSingleAsync(conn,
                    _assuranceCompanyQueries.ById(assuranceId));
            }

            return connection == null ?
                await ReadUsingConnectionAsync(ReadFunc(id))
                :
                await ReadAsync(ReadFunc(id), connection);
        }

        /// <summary>
        /// <see cref="IStoreRepository.GetAssuranceCompanyAsync(long)"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<AssuranceCompany> GetAssuranceCompanyAsync(long id) =>
            await GetAssuranceCompanyAsync(id, connection: null);

        /// <summary>
        /// <see cref="IStoreRepository.GetCollectTypeAsync(int)"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<CollectType> GetCollectTypeAsync(int id) =>
            await ReadUsingConnectionAsync(async (connection) =>
            {
                return await _collectTypeSqlDelegatedHandlers.GetSingleAsync(connection,
                    _collectTypeQueries.ById(id));
            });

        /// <summary>
        /// <see cref="IStoreRepository.GetStoreIdentificationAsync(string)"/>
        /// </summary>
        /// <param name="spec"></param>
        /// <param name="includePaths"></param>
        /// <returns></returns>
        public async Task<StoreIdentification> GetStoreIdentificationAsync(string storeId) =>
            await ReadUsingConnectionAsync(async (connection) =>
            {
                return await _storeIdentificationSqlDelegatedHandlers.GetSingleAsync(connection,
                    _storeIdentificationQueries.ByStoreId(storeId));
            });

        /// <summary>
        /// <see cref="IStoreRepository.AddStoreIdentificationAsync(StoreIdentification)"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task AddStoreIdentificationAsync(StoreIdentification entity) =>
            await CommandUsingConnectionAsync(async (connection) =>
            {
                await _storeIdentificationSqlDelegatedHandlers.InsertAsync(connection,
                    entity, Tables.Catalog.StoreIdentifications.Name);
            });

        /// <summary>
        /// <see cref="IStoreRepository.UpdateStoreIdentificationAsync(StoreIdentification)"/>
        /// </summary>
        /// <param name="entity"></param>
        public async Task UpdateStoreIdentificationAsync(StoreIdentification entity) =>
            await CommandUsingConnectionAsync(async (connection) =>
            {
                await _storeIdentificationSqlDelegatedHandlers.ExcecuteAsync(connection,
                    _storeIdentificationQueries.Update(entity,
                        _storeIdentificationQueries.Fields.GetAllFields()));
            });

        /// <summary>
        /// <see cref="IStoreRepository.GetBusinessGroupAsync(string)"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<BusinessGroup> GetBusinessGroupAsync(string id) =>
            await ReadUsingConnectionAsync(async (connection) =>
            {
                return await _businessGroupSqlDelegatedHandlers.GetSingleAsync(connection,
                    _businessGroupQueries.ById(id));
            });

        /// <summary>
        /// <see cref="IStoreRepository.AddBusinessGroupAsync(BusinessGroup)"/>
        /// </summary>
        /// <param name="businessGroup"></param>
        /// <returns></returns>
        public async Task AddBusinessGroupAsync(BusinessGroup businessGroup) =>
            await CommandUsingConnectionAsync(async (connection) =>
            {
                await _businessGroupSqlDelegatedHandlers.InsertAsync(connection, businessGroup,
                    Tables.Catalog.BusinessGroup.Name);
            });

        /// <summary>
        /// <see cref="IStoreRepository.UpdateBusinessGroupAsync(BusinessGroup)"/>
        /// </summary>
        /// <param name="businessGroup"></param>
        public async Task UpdateBusinessGroupAsync(BusinessGroup businessGroup) =>
            await CommandUsingConnectionAsync(async (connection) =>
            {
                await _businessGroupSqlDelegatedHandlers.ExcecuteAsync(connection,
                    _businessGroupQueries.Update(businessGroup, _businessGroupQueries.Fields.GetAllFields()));
            });
    }
}