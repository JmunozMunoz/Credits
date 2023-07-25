using Microsoft.EntityFrameworkCore.Internal;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Customers.Gateway;
using Sc.Credits.Domain.Model.Customers.Queries;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Connection;
using Sc.Credits.DrivenAdapters.SqlServer.Dapper.Map;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.QueriesKata;
using Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Customers;
using Sc.Credits.DrivenAdapters.SqlServer.Repository.Base;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using System.Transactions;

namespace Sc.Credits.DrivenAdapters.SqlServer.Repository
{
    /// <summary>
    /// The default implementation of <see cref="ICustomerRepository"/>
    /// </summary>
    public class CustomerRepository
        : CommandRepository<Customer, CustomerFields>, ICustomerRepository
    {
        #region Properties
        private static readonly CustomerQueries _customerQueries = QueriesCatalog.Customer;
        private static readonly ProfileQueries _profileQueries = QueriesCatalog.Profile;

        private readonly CustomerMapper _customerMapper;
        private readonly ISqlDelegatedHandlers<Profile> _profileSqlDelegatedHandlers;
        #endregion

        #region Methods Public
        /// <summary>
        /// Creates new instance of <see cref="CustomerRepository"/>
        /// </summary>
        /// <param name="connectionFactory"></param>
        /// <param name="customerSqlDelegatedHandlers"></param>
        /// <param name="profileSqlDelegatedHandlers"></param>
        public CustomerRepository(ICreditsConnectionFactory connectionFactory,
            ISqlDelegatedHandlers<Customer> customerSqlDelegatedHandlers,
            ISqlDelegatedHandlers<Profile> profileSqlDelegatedHandlers)
            : base(_customerQueries, customerSqlDelegatedHandlers, connectionFactory)
        {
            _customerMapper = CustomerMapper.New();
            _profileSqlDelegatedHandlers = profileSqlDelegatedHandlers;
        }

        /// <summary>
        /// <see cref="ICustomerRepository.GetByIdAsync(object, IEnumerable{Field}, IEnumerable{Field})"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fields"></param>
        /// <param name="profileFields"></param>
        /// <returns></returns>
        public async Task<Customer> GetByIdAsync(object id, IEnumerable<Field> fields, IEnumerable<Field> profileFields = null) =>
            await ReadUsingConnectionAsync(async (connection) =>
            {
                var dynamycQuery = await EntitySqlDelegatedHandlers.QueryAsync(connection,
                        _customerQueries.ById(id, fields));

                Customer customer = _customerMapper.FromDynamicQuery(dynamycQuery);

                await TryLoadProfile(connection, customer, profileFields);

                return customer;
            });

        /// <summary>
        /// <see cref="ICustomerRepository.GetByDocumentInfoAsync(string, string,
        /// IEnumerable{Field}, IEnumerable{Field})"/>
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentType"></param>
        /// <param name="loadProfile"></param>
        /// <returns></returns>
        public async Task<Customer> GetByDocumentInfoAsync(string idDocument, string documentType, IEnumerable<Field> fields = null,
            IEnumerable<Field> profileFields = null, params object[] entitiesForMerge) =>
            await ReadUsingConnectionAsync(async (connection) =>
            {
                var dynamycQuery = await EntitySqlDelegatedHandlers.QueryAsync(connection,
                        _customerQueries.ByDocumentInfo(idDocument, documentType, fields));

                Customer customer = _customerMapper.FromDynamicQuery(dynamycQuery);

                await TryLoadProfile(connection, customer, profileFields);

                return customer;
            });

        /// <summary>
        /// Add Async
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public override async Task AddAsync(Customer entity, Transaction transaction = null) =>
            await CommandEnlistingTransactionAsync(async (connection) =>
            {
                await EntitySqlDelegatedHandlers.InsertAsync(connection, entity, TableName, entitiesForMerge: entity.Name);
            },
            transaction);

        /// <summary>
        /// Gets the customer by document.
        /// </summary>
        /// <param name="idDocument">The identifier document.</param>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        public async Task<Customer> GetCustomerByDocument(string idDocument, IEnumerable<Field> fields = null, IEnumerable<Field> profileFields = null) =>
            await ReadUsingConnectionAsync(async (Connection) => {

                IEnumerable<dynamic> dynamicQuery = await EntitySqlDelegatedHandlers.QueryAsync(Connection,
                    _customerQueries.GetCustomerByDocument(idDocument, fields));

                Customer customer = _customerMapper.FromDynamicQuery(dynamicQuery);

                await TryLoadProfile(Connection, customer, profileFields);

                return customer;
            });

        #endregion

        #region Method private
        /// <summary>
        /// Try load profile
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="customer"></param>
        /// <param name="profileFields"></param>
        /// <returns></returns>
        private async Task TryLoadProfile(DbConnection connection, Customer customer, IEnumerable<Field> profileFields)
        {
            bool loadProfile = profileFields != null && profileFields.Any();

            if (loadProfile && customer != null)
            {
                int profileId = customer.GetProfileId ?? 0;
                Profile profile = await _profileSqlDelegatedHandlers.GetSingleAsync(connection,
                    _profileQueries.ById(profileId, profileFields));

                customer.SetProfile(profile);
            }
        }

        #endregion
    }
}