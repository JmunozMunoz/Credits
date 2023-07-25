using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.Domain.Model.Credits.Queries;
using Sc.Credits.Domain.Model.Credits.Queries.Commands;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.DrivenAdapters.SqlServer.Connection;
using Sc.Credits.DrivenAdapters.SqlServer.Dapper.Map;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.QueriesKata;
using Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Credits;
using Sc.Credits.DrivenAdapters.SqlServer.Repository.Base;
using Sc.Credits.Helpers.Commons.Settings;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Sc.Credits.DrivenAdapters.SqlServer.Repository
{
    /// <summary>
    /// The default implementation of <see cref="ICreditMasterRepository"/>
    /// </summary>
    public class CreditMasterRepository
        : TransactionRepository<CreditMaster, CreditMasterFields>, ICreditMasterRepository
    {
        private static readonly CreditMasterQueries _creditMasterQueries = QueriesCatalog.CreditMaster;
        private static readonly CreditQueries _creditQueries = QueriesCatalog.Credit;

        private readonly CreditMapper _creditMapper;

        private readonly ISqlDelegatedHandlers<Credit> _creditSqlDelegatedHandlers;
        private readonly ISqlDelegatedHandlers<TransactionReference> _transactionReferenceSqlDelegatedHandlers;
        private readonly CredinetAppSettings _credinetAppSettings;

        /// <summary>
        /// Creates new instance of <see cref="CreditMasterRepository"/>
        /// </summary>
        /// <param name="connectionFactory"></param>
        /// <param name="creditMasterSqlDelegatedHandlers"></param>
        /// <param name="creditSqlDelegatedHandlers"></param>
        /// <param name="transactionReferenceSqlDelegatedHandlers"></param>
        /// <param name="appSettings"></param>
        public CreditMasterRepository(ICreditsConnectionFactory connectionFactory,
            ISqlDelegatedHandlers<CreditMaster> creditMasterSqlDelegatedHandlers,
            ISqlDelegatedHandlers<Credit> creditSqlDelegatedHandlers,
            ISqlDelegatedHandlers<TransactionReference> transactionReferenceSqlDelegatedHandlers,
            ISettings<CredinetAppSettings> appSettings)
            : base(_creditMasterQueries, creditMasterSqlDelegatedHandlers, connectionFactory)
        {
            _credinetAppSettings = appSettings.Get();
            _creditMapper = CreditMapper.New(_credinetAppSettings);

            _creditSqlDelegatedHandlers = creditSqlDelegatedHandlers;
            _transactionReferenceSqlDelegatedHandlers = transactionReferenceSqlDelegatedHandlers;
        }

        /// <summary>
        /// Adds new credit.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async new Task AddAsync(CreditMaster entity, Transaction transaction = null)
        {
            try
            {
                await CommandEnlistingTransactionAsync(async (connection) =>
                {
                    await EntitySqlDelegatedHandlers.InsertAsync(connection, entity, _creditMasterQueries.Table.Name);

                    await _creditSqlDelegatedHandlers.InsertAsync(connection, entity.Current, _creditQueries.Table.Name,
                        entitiesForMerge: entity.Current.CreditPayment);
                },
                transaction);
            }
            catch (SqlException ex)
            {
                HandleCreditUniqueIndexException(ex);
                throw;
            }
        }

        /// <summary>
        /// delete credit.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async new Task<bool> DeleteAsync(CreditMaster entity, Transaction transaction = null)
        {
            bool result = false;

            await CommandEnlistingTransactionAsync(async (connection) =>
            {
                //Delete as table Credit
                result = await _creditSqlDelegatedHandlers.DeleteAsync(connection, _creditQueries.Table.Name, _creditQueries.Table.Fields.CreditMasterId.Name, entity.Id.ToString());

                //Delete as table CreditMaster
                result = await EntitySqlDelegatedHandlers.DeleteAsync(connection, _creditMasterQueries.Table.Name, _creditMasterQueries.Table.Fields.Id.Name, entity.Id.ToString());
            },
            transaction);

            return result;
        }

        /// <summary>
        /// <see cref="ICreditMasterRepository.AddTransactionAsync(CreditMaster, IEnumerable{Field}, Transaction)"/>
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="additionalMasterUpdateFields"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async Task AddTransactionAsync(CreditMaster creditMaster, IEnumerable<Field> additionalMasterUpdateFields = null,
            Transaction transaction = null)
        {
            try
            {
                if (transaction != null)
                {
                    await ExecuteTransactionAsync(creditMaster, additionalMasterUpdateFields, transaction);
                }
                else
                {
                    await ExcecuteOnTransactionAsync(async newTransaction =>
                    {
                        await ExecuteTransactionAsync(creditMaster, additionalMasterUpdateFields, newTransaction);
                    });
                }
            }
            catch (SqlException ex)
            {
                HandlePaymentUniqueIndexException(ex);
                throw;
            }
        }

        /// <summary>
        /// <see cref="ICreditMasterRepository.GetWithCurrentAsync(Guid, IEnumerable{Field},
        /// IEnumerable{Field}, IEnumerable{Field}, IEnumerable{Field}, IEnumerable{Field})"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fields"></param>
        /// <param name="transactionFields"></param>
        /// <param name="customerFields"></param>
        /// <param name="storeFields"></param>
        /// <param name="transactionStoreFields"></param>
        /// <returns></returns>
        public async Task<CreditMaster> GetWithCurrentAsync(Guid id, IEnumerable<Field> fields = null,
            IEnumerable<Field> transactionFields = null, IEnumerable<Field> customerFields = null, IEnumerable<Field> storeFields = null,
            IEnumerable<Field> transactionStoreFields = null)
        {
            var dynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _creditMasterQueries.WithCurrent(id, fields, transactionFields, customerFields,
                    storeFields, transactionStoreFields);

                return await EntitySqlDelegatedHandlers.QueryAsync(connection, query);
            });

            return _creditMapper.MasterFromDynamicQuery(dynamicQuery, loadTransaction: true,
                loadCustomer: customerFields != null, loadStore: storeFields != null,
                loadTransactionStore: transactionStoreFields != null);
        }

        /// <summary>
        /// <see cref="ICreditMasterRepository.GetWithCurrentAsync(Guid, Customer, Store, IEnumerable{Field})"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        public async Task<CreditMaster> GetWithCurrentAsync(Guid id, Customer customer, Store store,
                IEnumerable<Field> transactionStoreFields = null)
        {
            var dynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _creditMasterQueries.WithCurrent(id, transactionStoreFields: transactionStoreFields);

                return await EntitySqlDelegatedHandlers.QueryAsync(connection, query);
            });

            return _creditMapper.MasterFromDynamicQuery(dynamicQuery, loadTransaction: true,
                loadCustomer: false, loadStore: false, customer, store,
                loadTransactionStore: transactionStoreFields != null);
        }

        /// <summary>
        /// <see cref="ICreditMasterRepository.GetWithCurrentAsync(List{Guid}, Customer,
        /// IEnumerable{Field}, IEnumerable{Field}, IEnumerable{Statuses})"/>
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="customer"></param>
        /// <param name="storeFields"></param>
        /// <param name="transactionStoreFields"></param>
        /// <param name="statuses"></param>
        /// <returns></returns>
        public async Task<List<CreditMaster>> GetWithCurrentAsync(List<Guid> ids, Customer customer,
            IEnumerable<Field> storeFields = null, IEnumerable<Field> transactionStoreFields = null,
            IEnumerable<Statuses> statuses = null, bool setCreditLimit = true)
        {
            var dynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _creditMasterQueries.WithCurrent(ids, customer.Id, storeFields,
                    transactionStoreFields, statuses);

                return await EntitySqlDelegatedHandlers.QueryAsync(connection, query);
            });

            return _creditMapper.MastersFromDynamicQuery(dynamicQuery, loadTransaction: true,
                loadCustomer: false, loadStore: storeFields != null, customer,
                loadTransactionStore: transactionStoreFields != null, setCreditLimit : setCreditLimit).ToList();
        }

        /// <summary>
        /// <see cref="ICreditMasterRepository.GetWithTransactionsAsync(Guid, IEnumerable{Field},
        /// IEnumerable{Field}, IEnumerable{Field}, IEnumerable{Field}, IEnumerable{Field})"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fields"></param>
        /// <param name="transactionFields"></param>
        /// <param name="customerFields"></param>
        /// <param name="storeFields"></param>
        /// <param name="transactionStoreFields"></param>
        /// <returns></returns>
        public async Task<CreditMaster> GetWithTransactionsAsync(Guid id, IEnumerable<Field> fields = null,
            IEnumerable<Field> transactionFields = null, IEnumerable<Field> customerFields = null,
            IEnumerable<Field> storeFields = null, IEnumerable<Field> transactionStoreFields = null)
        {
            var creditMasterDynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _creditMasterQueries.ById(id, fields, customerFields, storeFields);

                return await EntitySqlDelegatedHandlers.QueryAsync(connection, query);
            });

            CreditMaster creditMaster =
                _creditMapper.MasterFromDynamicQuery(creditMasterDynamicQuery, loadTransaction: false,
                    loadCustomer: true, loadStore: storeFields != null, loadTransactionStore: transactionStoreFields != null);

            if (creditMaster == null)
            {
                return null;
            }

            var creditsDynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _creditQueries.ByMaster(creditMasterId: id, transactionFields, transactionStoreFields);

                return await _creditSqlDelegatedHandlers.QueryAsync(connection, query);
            });

            return _creditMapper.MapTransactions(creditMaster, creditsDynamicQuery, loadCustomer: false,
                    loadStore: transactionStoreFields != null);
        }

        /// <summary>
        /// <see cref="ICreditMasterRepository.GetWithTransactionsAsync(Customer,
        /// IEnumerable{Statuses},IEnumerable{Field}, IEnumerable{Field}) "/>
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="statuses"></param>
        /// <param name="storeFields"></param>
        /// <param name="transactionStoreFields"></param>
        /// <returns></returns>
        public async Task<List<CreditMaster>> GetWithTransactionsAsync(Customer customer, IEnumerable<Statuses> statuses = null,
            IEnumerable<Field> storeFields = null, IEnumerable<Field> transactionStoreFields = null)
        {
            var creditMasterDynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _creditMasterQueries.ByCustomer(customer.Id, statuses, storeFields);

                return await EntitySqlDelegatedHandlers.QueryAsync(connection, query);
            });

            IEnumerable<CreditMaster> creditMasters =
                _creditMapper.MastersFromDynamicQuery(creditMasterDynamicQuery, loadTransaction: false,
                    loadCustomer: false, loadStore: storeFields != null, customer);

            List<Guid> ids = creditMasters.Select(cm => cm.Id).ToList();

            var creditsDynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _creditQueries.ByMasters(creditMasterIds: ids, storeFields: transactionStoreFields);

                return await _creditSqlDelegatedHandlers.QueryAsync(connection, query);
            });

            return _creditMapper.MapTransactions(creditMasters, creditsDynamicQuery, loadCustomer: false,
                    loadStore: transactionStoreFields != null)
                .ToList();
        }

        /// <summary>
        /// <see cref="ICreditMasterRepository.GetWithTransactionsAsync(List{Guid},
        /// IEnumerable{Field}, IEnumerable{Field}, IEnumerable{Field}, IEnumerable{Field}, IEnumerable{Field})"/>
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="fields"></param>
        /// <param name="transactionFields"></param>
        /// <param name="customerFields"></param>
        /// <param name="storeFields"></param>
        /// <param name="transactionStoreFields"></param>
        /// <returns></returns>
        public async Task<List<CreditMaster>> GetWithTransactionsAsync(List<Guid> ids, IEnumerable<Field> fields = null,
            IEnumerable<Field> transactionFields = null, IEnumerable<Field> customerFields = null,
            IEnumerable<Field> storeFields = null, int statusId = 0)
        {
            var creditMasterDynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _creditMasterQueries.ByIds(ids, fields, customerFields, storeFields,statusId);

                return await EntitySqlDelegatedHandlers.QueryAsync(connection, query);
            });

            IEnumerable<CreditMaster> creditMasters =
                _creditMapper.MastersFromDynamicQuery(creditMasterDynamicQuery, loadTransaction: false,
                    loadCustomer: true, loadStore: storeFields != null);

            var creditsDynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _creditQueries.ByMasters(creditMasterIds: ids, fields: transactionFields, customerFields, storeFields);

                return await _creditSqlDelegatedHandlers.QueryAsync(connection, query);
            });

            return _creditMapper.MapTransactions(creditMasters, creditsDynamicQuery, loadCustomer: true,
                    loadStore: true)
                .ToList();
        }

        /// <summary>
        /// <see cref="ICreditMasterRepository.GetWithTransactionsByCreditIdAsync(Guid,
        /// IEnumerable{Field}, IEnumerable{Field}, IEnumerable{Field}, IEnumerable{Field}, IEnumerable{Field})"/>
        /// </summary>
        /// <param name="creditId"></param>
        /// <param name="fields"></param>
        /// <param name="transactionFields"></param>
        /// <param name="customerFields"></param>
        /// <param name="storeFields"></param>
        /// <param name="transactionStoreFields"></param>
        /// <returns></returns>
        public async Task<CreditMaster> GetWithTransactionsByCreditIdAsync(Guid creditId, IEnumerable<Field> fields = null,
            IEnumerable<Field> transactionFields = null, IEnumerable<Field> customerFields = null,
            IEnumerable<Field> storeFields = null, IEnumerable<Field> transactionStoreFields = null)
        {
            var creditMasterDynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _creditMasterQueries.ByCreditId(creditId, fields, customerFields, storeFields);

                return await EntitySqlDelegatedHandlers.QueryAsync(connection, query);
            });

            CreditMaster creditMaster =
               _creditMapper.MasterFromDynamicQuery(creditMasterDynamicQuery, loadTransaction: false,
                    loadCustomer: customerFields != null, loadStore: storeFields != null);

            if (creditMaster == null)
            {
                return null;
            }

            var creditsDynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _creditQueries.ByMaster(creditMaster.Id, storeFields: transactionStoreFields);

                return await _creditSqlDelegatedHandlers.QueryAsync(connection, query);
            });

            return _creditMapper.MapTransactions(creditMaster, creditsDynamicQuery, loadCustomer: false,
                loadStore: transactionStoreFields != null);
        }

        /// <summary>
        /// <see cref="ICreditMasterRepository.GetActiveCreditsByCollectTypeAsync(Customer, Store, CredinetAppSettings)"/>
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        public async Task<List<CreditMaster>> GetActiveCreditsByCollectTypeAsync(Customer customer, Store store)
        {
            var dynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _creditMasterQueries.ActiveByCollectType(customer.Id, store.GetCollectTypeId,
                    store.GetVendorId, store.GetBusinessGroupId, includedStoreIds: _credinetAppSettings.ActiveCreditsIncludedStoreIds?.Split(','));

                return await EntitySqlDelegatedHandlers.QueryAsync(connection, query);
            });

            return _creditMapper.MastersFromDynamicQuery(dynamicQuery, loadTransaction: true,
                    loadCustomer: false, loadStore: true, customer)
                .ToList();
        }

        /// <summary>
        /// <see cref = "ICreditMasterRepository.GetActiveCreditsByCollectTypeAsync(Customer)" />
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>

        public async Task<List<CreditMaster>> GetActiveCreditsByCollectTypeAsync(Customer customer)
        {
            var dynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _creditMasterQueries.ActiveByCollectType(customer.Id, includedStoreIds: _credinetAppSettings.ActiveCreditsIncludedStoreIds?.Split(','));

                return await EntitySqlDelegatedHandlers.QueryAsync(connection, query);
            });

            return _creditMapper.MastersFromDynamicQuery(dynamicQuery, loadTransaction: true,
                    loadCustomer: false, loadStore: true, customer)
                .ToList();
        }


        /// <summary>
        /// <see cref="ICreditMasterRepository.GetActiveCreditsAsync(Customer)"/>
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public async Task<List<CreditMaster>> GetActiveCreditsAsync(Customer customer)
        {
            var dynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _creditMasterQueries.Actives(customer.Id);

                return await EntitySqlDelegatedHandlers.QueryAsync(connection, query);
            });

            return _creditMapper.MastersFromDynamicQuery(dynamicQuery, loadTransaction: true,
                    loadCustomer: false, loadStore: true, customer)
                .ToList();
        }

        /// <summary>
        /// <see cref="ICreditMasterRepository.GetActiveAndCancelRequestCreditsAsync(Customer)"/>
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public async Task<List<CreditMaster>> GetActiveAndCancelRequestCreditsAsync(Customer customer)
        {
            var dynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _creditMasterQueries.ActivesAndCancelRequest(customer.Id);

                return await EntitySqlDelegatedHandlers.QueryAsync(connection, query);
            });

            return _creditMapper.MastersFromDynamicQuery(dynamicQuery, loadTransaction: true,
                    loadCustomer: false, loadStore: true, customer)
                .ToList();
        }

        /// <summary>
        /// <see cref="ICreditMasterRepository.GetActiveCreditsWithTokenAsync(Customer, string)"/>
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public async Task<List<CreditMaster>> GetActiveCreditsWithTokenAsync(Customer customer, string storeId)
        {
            var dynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _creditMasterQueries.ActivesRequestInStore(customer.Id, storeId);

                return await EntitySqlDelegatedHandlers.QueryAsync(connection, query);
            });

            return _creditMapper.MastersFromDynamicQuery(dynamicQuery, loadTransaction: true,
                    loadCustomer: false, loadStore: true, customer)
                .ToList();
        }

        /// <summary>
        /// <see cref="ICreditMasterRepository.GetPaidCreditsForCertificateAsync(List{Guid})"/>
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<List<CreditMaster>> GetPaidCreditsForCertificateAsync(List<Guid> ids)
        {
            var dynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _creditMasterQueries.PaidForCertificate(ids);

                return await EntitySqlDelegatedHandlers.QueryAsync(connection, query);
            });

            return _creditMapper.MastersFromDynamicQuery(dynamicQuery, loadTransaction: true,
                    loadCustomer: true, loadStore: true)
                .ToList();
        }

        /// <summary>
        /// <see cref="ICreditMasterRepository.GetCustomerCreditHistoryAsync(Customer, string, int)"/>
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="storeId"></param>
        /// <param name="maximumMonthsCreditHistory"></param>
        /// <returns></returns>
        public async Task<List<CreditMaster>> GetCustomerCreditHistoryAsync(Customer customer, string storeId, int maximumMonthsCreditHistory)
        {
            var dynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _creditMasterQueries.CustomerHistory(customer.Id, storeId, maximumMonthsCreditHistory);

                return await EntitySqlDelegatedHandlers.QueryAsync(connection, query);
            });

            return _creditMapper.MastersFromDynamicQuery(dynamicQuery, loadTransaction: true,
                    loadCustomer: false, loadStore: true, customer)
                .ToList();
        }

        /// <summary>
        /// <see cref="ICreditMasterRepository.GetActiveAndPendingCancellationCreditsAsync(Customer,
        /// string, IEnumerable{Field}, IEnumerable{Field})"/>
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="vendorId"></param>
        /// <param name="transactionFields"></param>
        /// <param name="storeFields"></param>
        /// <returns></returns>
        public async Task<List<CreditMaster>> GetActiveAndPendingCancellationCreditsAsync(Customer customer, string vendorId,
            IEnumerable<Field> transactionFields, IEnumerable<Field> storeFields)
        {
            var creditMasterDynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _creditMasterQueries.ActiveAndPendingCancellationCredits(customer.Id, vendorId, storeFields);

                return await EntitySqlDelegatedHandlers.QueryAsync(connection, query);
            });

            IEnumerable<CreditMaster> creditMasters =
                _creditMapper.MastersFromDynamicQuery(creditMasterDynamicQuery, loadTransaction: false,
                    loadCustomer: false, loadStore: true, customer);

            List<Guid> ids = creditMasters.Select(cm => cm.Id).ToList();

            var creditsDynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _creditQueries.ByMasters(creditMasterIds: ids, fields: transactionFields, storeFields: storeFields);

                return await _creditSqlDelegatedHandlers.QueryAsync(connection, query);
            });

            return _creditMapper.MapTransactions(creditMasters, creditsDynamicQuery, loadCustomer: false,
                    loadStore: storeFields != null, customer)
                .ToList();
        }

        /// <summary>
        /// <see cref="ICreditMasterRepository.GetActiveAndPendingCancellationPaymentsAsync(Customer)"/>
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public async Task<List<CreditMaster>> GetActiveAndPendingCancellationPaymentsAsync(Customer customer)
        {
            var creditMasterDynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _creditMasterQueries.ActiveAndPendingCancellationPayments(customer.Id);

                return await EntitySqlDelegatedHandlers.QueryAsync(connection, query);
            });

            IEnumerable<CreditMaster> creditMasters =
                _creditMapper.MastersFromDynamicQuery(creditMasterDynamicQuery, loadTransaction: false,
                    loadCustomer: false, loadStore: true, customer);

            List<Guid> ids = creditMasters.Select(cm => cm.Id).ToList();

            var creditsDynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _creditQueries.ActiveAndPendingCancellationPayments(creditMasterIds: ids);

                return await _creditSqlDelegatedHandlers.QueryAsync(connection, query);
            });

            return _creditMapper.MapTransactions(creditMasters, creditsDynamicQuery, loadCustomer: false,
                    loadStore: true, customer)
                .ToList();
        }

        /// <summary>
        /// <see cref="ICreditMasterRepository.GetCustomerPaymentHistoryAsync(Customer, Store,
        /// IEnumerable{Field}, IEnumerable{Field}, int)"/>
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <param name="masterFields"></param>
        /// <param name="transactionStoreFields"></param>
        /// <param name="maximumMonthsPaymentHistory"></param>
        /// <returns></returns>
        public async Task<List<Credit>> GetCustomerPaymentHistoryAsync(Customer customer, Store store, IEnumerable<Field> masterFields,
            IEnumerable<Field> transactionStoreFields, int maximumMonthsPaymentHistory)
        {
            var creditsDynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _creditQueries.CustomerPaymentHistory(store, customer.Id, transactionStoreFields, maximumMonthsPaymentHistory);

                return await _creditSqlDelegatedHandlers.QueryAsync(connection, query);
            });

            IEnumerable<Credit> payments =
                _creditMapper.TransactionsFromDynamicQuery(creditsDynamicQuery, loadCustomer: false, loadStore: true,
                    customer);

            List<Guid> ids = payments.Select(c => c.Id).ToList();

            var creditMasterDynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _creditMasterQueries.CustomerPaymentHistory(paymentIds: ids, fields: masterFields);

                return await EntitySqlDelegatedHandlers.QueryAsync(connection, query);
            });

            IEnumerable<CreditMaster> creditMasters =
                _creditMapper.MastersFromDynamicQuery(creditMasterDynamicQuery, loadTransaction: false,
                    loadCustomer: false, loadStore: false, customer);

            return _creditMapper.MapTransactions(creditMasters, payments)
                .SelectMany(cm => cm.History)
                .ToList();
        }

        /// <summary>
        /// <see cref="ICreditMasterRepository.GetPaymentsAsync(List{Guid}, IEnumerable{Field},
        /// IEnumerable{Field}, IEnumerable{Field}, IEnumerable{Field}, IEnumerable{Field})"/>
        /// </summary>
        /// <param name="paymentsId"></param>
        /// <param name="fields"></param>
        /// <param name="masterFields"></param>
        /// <param name="customerFields"></param>
        /// <param name="storeFields"></param>
        /// <param name="masterStoreFields"></param>
        /// <returns></returns>
        public async Task<List<Credit>> GetPaymentsAsync(List<Guid> paymentsId, IEnumerable<Field> fields, IEnumerable<Field> masterFields,
            IEnumerable<Field> customerFields, IEnumerable<Field> storeFields, IEnumerable<Field> masterStoreFields)
        {
            var creditsDynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _creditQueries.Payments(paymentsId, fields, customerFields,
                    storeFields);

                return await _creditSqlDelegatedHandlers.QueryAsync(connection, query);
            });

            IEnumerable<Credit> payments =
                _creditMapper.TransactionsFromDynamicQuery(creditsDynamicQuery, loadCustomer: true, loadStore: true);

            List<Guid> ids = payments.Select(c => c.Id).ToList();

            var creditMasterDynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _creditMasterQueries.CustomerPaymentHistory(paymentIds: ids, fields: masterFields,
                    storeFields: masterStoreFields);

                return await EntitySqlDelegatedHandlers.QueryAsync(connection, query);
            });

            IEnumerable<CreditMaster> creditMasters =
                _creditMapper.MastersFromDynamicQuery(creditMasterDynamicQuery, loadTransaction: false,
                    loadCustomer: false, loadStore: true);

            return _creditMapper.MapTransactions(creditMasters, payments)
                .SelectMany(cm => cm.History)
                .ToList();
        }

        /// <summary>
        /// <see cref="ICreditMasterRepository.GetTransactionsAsync(List{Guid}, IEnumerable{Field},
        /// IEnumerable{Field}, IEnumerable{Field})"/>
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="fields"></param>
        /// <param name="masterFields"></param>
        /// <param name="customerFields"></param>
        /// <param name="storeFields"></param>
        /// <param name="masterStoreFields"></param>
        /// <returns></returns>
        public async Task<List<Credit>> GetTransactionsAsync(List<Guid> ids, IEnumerable<Field> customerFields, IEnumerable<Field> storeFields)
        {
            var creditsDynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _creditQueries.Transactions(ids, customerFields, storeFields);

                return await _creditSqlDelegatedHandlers.QueryAsync(connection, query);
            });

            IEnumerable<Credit> transactions =
                _creditMapper.TransactionsFromDynamicQuery(creditsDynamicQuery, loadCustomer: true, loadStore: true);

            List<Guid> creditMasterIds = transactions.Select(t => t.GetCreditMasterId).ToList();
            var creditMasterDynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _creditMasterQueries.ByIds(ids: creditMasterIds, customerFields: customerFields, storeFields: storeFields);

                return await EntitySqlDelegatedHandlers.QueryAsync(connection, query);
            });

            IEnumerable<CreditMaster> creditMasters =
                _creditMapper.MastersFromDynamicQuery(creditMasterDynamicQuery, loadTransaction: false,
                    loadCustomer: true, loadStore: true);

            return _creditMapper.MapTransactions(creditMasters, transactions)
                .SelectMany(cm => cm.History)
                .ToList();
        }

        /// <summary>
        /// <see cref="ICreditMasterRepository.IsDuplicatedAsync(string, DateTime, Guid)"/>
        /// </summary>
        /// <param name="token"></param>
        /// <param name="date"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public async Task<bool> IsDuplicatedAsync(string token, DateTime date, Guid customerId) =>
            await ReadUsingConnectionAsync(async (connection) =>
            {
                return await EntitySqlDelegatedHandlers.AnyAsync(connection,
                    _creditMasterQueries.Duplicated(token, date, customerId));
            });

        /// <summary>
        /// <see cref="ICreditMasterRepository.ValidateACreditPaidAccordingToTime(Guid, int, Statuses)"/>
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="paidCreditDays"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<bool> ValidateACreditPaidAccordingToTime(Guid customerId, int paidCreditDays, Statuses status) =>
            await ReadUsingConnectionAsync(async (connection) =>
            {
                return await EntitySqlDelegatedHandlers.AnyAsync(connection,
                    _creditMasterQueries.ValidateACreditPaidAccordingToTime(customerId, paidCreditDays, status));
            });

        /// <summary>
        /// <see cref="ICreditMasterRepository.ValidateCustomerHistory(Guid)"/>
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public async Task<bool> ValidateCustomerHistory(Guid customerId) =>
            await ReadUsingConnectionAsync(async (connection) =>
            {
                return await EntitySqlDelegatedHandlers.AnyAsync(connection,
                    _creditMasterQueries.ValidateCustomerHistory(customerId));
            });

        /// <summary>
        /// <see cref="ICreditMasterRepository.GetActiveWithDayDateAndPendingPromissoryNoteAsync(DateTime, IEnumerable{Field})"/>
        /// </summary>
        /// <param name="date"></param>
        /// <param name="customerFields"></param>
        /// <param name="top">limit of credits per transaction</param>
        /// <returns></returns>
        public async Task<List<CreditMaster>> GetActiveWithDayDateAndPendingPromissoryNoteAsync(DateTime date, IEnumerable<Field> customerFields,int top)
        {
            var creditMasterDynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _creditMasterQueries.ActiveAndPendingPromissoryFile(date, customerFields,top);

                return await EntitySqlDelegatedHandlers.QueryAsync(connection, query);
            });

            IEnumerable<CreditMaster> creditMasters =
                _creditMapper.MastersFromDynamicQuery(creditMasterDynamicQuery, loadTransaction: false,
                    loadCustomer: true, loadStore: false);

            List<Guid> ids = creditMasters.Select(cm => cm.Id).ToList();

            var creditsDynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _creditQueries.ByMasters(creditMasterIds: ids);

                return await _creditSqlDelegatedHandlers.QueryAsync(connection, query);
            });

            return _creditMapper.MapTransactions(creditMasters, creditsDynamicQuery, loadCustomer: true, loadStore: false)
                .ToList();
        }

        /// <summary>
        /// Execute Transaction
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="additionalMasterUpdateFields"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private async Task ExecuteTransactionAsync(CreditMaster creditMaster, IEnumerable<Field> additionalMasterUpdateFields, Transaction transaction) =>
            await CommandEnlistingTransactionAsync(async (connection) =>
            {
                await _creditSqlDelegatedHandlers.InsertAsync(connection, creditMaster.Current,
                    _creditQueries.Table.Name, entitiesForMerge: creditMaster.Current.CreditPayment);

                await EntitySqlDelegatedHandlers.ExcecuteAsync(connection,
                    _creditMasterQueries.Update(creditMaster,
                        (additionalMasterUpdateFields ?? Enumerable.Empty<Field>())
                            .Union(CreditMasterCommandsFields.NewTransactionUpdate)));

                await AddTransactionReferenceAsync(connection, creditMaster.Current.TransactionReference);
            },
            transaction);

        /// <summary>
        /// Add transaction reference
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transactionReferences"></param>
        /// <returns></returns>
        private async Task AddTransactionReferenceAsync(DbConnection connection, TransactionReference transactionReferences)
        {
            if (transactionReferences != null)
            {
                await _transactionReferenceSqlDelegatedHandlers.InsertAsync(connection, transactionReferences, Tables.Catalog.TransactionReferences.Name);
            }
        }

        /// <summary>
        /// Handle payment unique index exception
        /// </summary>
        /// <param name="sqlException"></param>
        private void HandlePaymentUniqueIndexException(SqlException sqlException)
        {
            if (sqlException.Number == 2627 || sqlException.Number == 2601)
            {
                const string UNIQUE_PAYMENT_INDEX_NAME = @"IX_Credits_CreditMasterId_TotalValuePaid_TransactionDate_TransactionHour_TransactionMinute";
                const string UNIQUE_PAYMENTNUMBER = @"IX_Credits_PaymentNumber_StoreId";
                if (sqlException.Message.Contains(UNIQUE_PAYMENT_INDEX_NAME))
                {                    
                    throw new BusinessException(nameof(BusinessResponse.DuplicatedPayment), (int)BusinessResponse.DuplicatedPayment);                    
                }
                if (sqlException.Message.Contains(UNIQUE_PAYMENTNUMBER))
                {
                    throw new BusinessException(nameof(BusinessResponse.DuplicatedPaymentNumber), (int)BusinessResponse.DuplicatedPaymentNumber);
                }
            }

        }

        /// <summary>
        /// Handle credit unique index exception
        /// </summary>
        /// <param name="sqlException"></param>
        private void HandleCreditUniqueIndexException(SqlException sqlException)
        {
            if (sqlException.Number == 2627 || sqlException.Number == 2601)
            {
                const string UNIQUE_CREDIT_INDEX_NAME = @"IX_CreditsMaster_CustomerId_StoreId_CreditDate_Month_Hour_Minute";
                if (sqlException.Message.Contains(UNIQUE_CREDIT_INDEX_NAME))
                {
                    throw new BusinessException(nameof(BusinessResponse.DuplicatedCredit), (int)BusinessResponse.DuplicatedCredit);
                }

                const string UNIQUE_CREDIT_NUMBER_INDEX_NAME = @"IX_CreditsMaster_CreditNumber_StoreId";
                if (sqlException.Message.Contains(UNIQUE_CREDIT_NUMBER_INDEX_NAME))
                {
                    throw new BusinessException(nameof(BusinessResponse.InvalidStoreSequence), (int)BusinessResponse.InvalidStoreSequence);
                }
            }
        }
    }
}