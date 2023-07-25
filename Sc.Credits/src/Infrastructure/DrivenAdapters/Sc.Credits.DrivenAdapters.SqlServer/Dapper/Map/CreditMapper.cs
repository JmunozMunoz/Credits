using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Helpers.ObjectsUtils;
using System.Collections.Generic;
using System.Linq;

namespace Sc.Credits.DrivenAdapters.SqlServer.Dapper.Map
{
    /// <summary>
    /// Credit mapper
    /// </summary>
    public class CreditMapper
        : Mapper
    {
        private readonly CredinetAppSettings _credinetAppSettings;

        /// <summary>
        /// New
        /// </summary>
        /// <param name="credinetAppSettings"></param>
        /// <returns></returns>
        public static CreditMapper New(CredinetAppSettings credinetAppSettings)
            => new CreditMapper(credinetAppSettings);

        /// <summary>
        /// New credit mapper
        /// </summary>
        /// <param name="credinetAppSettings"></param>
        protected CreditMapper(CredinetAppSettings credinetAppSettings)
        {
            _credinetAppSettings = credinetAppSettings;
        }

        /// <summary>
        /// Master from dynamic query
        /// </summary>
        /// <param name="dynamicQuery"></param>
        /// <param name="loadTransaction"></param>
        /// <param name="loadCustomer"></param>
        /// <param name="loadStore"></param>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <param name="loadTransactionStore"></param>
        /// <returns></returns>
        public CreditMaster MasterFromDynamicQuery(IEnumerable<dynamic> dynamicQuery, bool loadTransaction,
            bool loadCustomer, bool loadStore, Customer customer = null, Store store = null,
            bool loadTransactionStore = false)
        {
            CreditMaster creditMaster = null;

            if (dynamicQuery is ICollection<object> rows && rows.Any())
            {
                creditMaster = MasterFromDynamicRow(rows.Single(), loadTransaction,
                    loadCustomer, loadStore, customer, store, loadTransactionStore);
            }

            return creditMaster;
        }

        /// <summary>
        /// Masters from dynamic query
        /// </summary>
        /// <param name="dynamicQuery"></param>
        /// <param name="loadTransaction"></param>
        /// <param name="loadCustomer"></param>
        /// <param name="loadStore"></param>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <param name="loadTransactionStore"></param>
        /// <returns></returns>
        public IEnumerable<CreditMaster> MastersFromDynamicQuery(IEnumerable<dynamic> dynamicQuery,
            bool loadTransaction, bool loadCustomer, bool loadStore, Customer customer = null,
            Store store = null, bool loadTransactionStore = false, bool setCreditLimit = true)
        {
            if (dynamicQuery is ICollection<object> rows && rows.Any())
            {
                foreach (object row in rows)
                {
                    yield return MasterFromDynamicRow(row, loadTransaction,
                        loadCustomer, loadStore, customer, store, loadTransactionStore, setCreditLimit);
                }
            }
        }

        /// <summary>
        /// Master from dynamic row
        /// </summary>
        /// <param name="row"></param>
        /// <param name="loadTransaction"></param>
        /// <param name="loadCustomer"></param>
        /// <param name="loadStore"></param>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <param name="loadTransactionStore"></param>
        /// <returns></returns>
        public CreditMaster MasterFromDynamicRow(object row, bool loadTransaction,
            bool loadCustomer, bool loadStore, Customer customer = null, Store store = null,
            bool loadTransactionStore = false, bool setCreditLimit = true)
        {
            CreditMaster creditMaster = null;

            if (row is IDictionary<string, object> dictionary)
            {
                ICollection<IDictionary<string, object>> dictionaries = SplitDictionary(dictionary);

                creditMaster = CreditMaster.New();

                IEnumerator<IDictionary<string, object>> enumerator = dictionaries.GetEnumerator();

                enumerator.MoveNext();

                MapEntity(creditMaster, enumerator);

                bool next = enumerator.MoveNext();

                if (loadTransaction && next)
                {
                    MapCurrent(creditMaster, enumerator, out next);
                }

                TryMapCustomer(creditMaster, loadCustomer, customer, enumerator, ref next, setCreditLimit);

                TryMapStore(creditMaster, loadStore, store, enumerator, ref next);

                TryMapStoreTransaction(creditMaster.Current, loadTransactionStore, null,
                    enumerator, ref next);

                TryMapStatus(creditMaster, enumerator, next);
            }

            return creditMaster;
        }

        /// <summary>
        /// Map transactions
        /// </summary>
        /// <param name="creditMasters"></param>
        /// <param name="dynamicQuery"></param>
        /// <param name="loadCustomer"></param>
        /// <param name="loadStore"></param>
        /// <param name="customer"></param>
        /// <returns></returns>
        public IEnumerable<CreditMaster> MapTransactions(IEnumerable<CreditMaster> creditMasters,
            IEnumerable<dynamic> dynamicQuery, bool loadCustomer, bool loadStore, Customer customer = null)
        {
            IEnumerable<Credit> transactions = TransactionsFromDynamicQuery(dynamicQuery, loadCustomer, loadStore,
                customer);

            return MapTransactions(creditMasters, transactions);
        }

        /// <summary>
        /// Map transactions
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="dynamicQuery"></param>
        /// <param name="loadCustomer"></param>
        /// <param name="loadStore"></param>
        /// <param name="customer"></param>
        /// <returns></returns>
        public CreditMaster MapTransactions(CreditMaster creditMaster, IEnumerable<dynamic> dynamicQuery,
            bool loadCustomer, bool loadStore, Customer customer = null)
        {
            if (creditMaster == null)
                return null;

            IEnumerable<Credit> transactions = TransactionsFromDynamicQuery(dynamicQuery, loadCustomer, loadStore,
                customer);

            return MapTransactions(creditMaster, transactions);
        }

        /// <summary>
        /// Map transactions
        /// </summary>
        /// <param name="creditMasters"></param>
        /// <param name="transactions"></param>
        /// <returns></returns>
        public IEnumerable<CreditMaster> MapTransactions(IEnumerable<CreditMaster> creditMasters,
            IEnumerable<Credit> transactions) =>
            creditMasters
                .Select(cm =>
                {
                    IEnumerable<Credit> masterTransactions =
                        transactions.Where(t => t.GetCreditMasterId == cm.Id);

                    return cm.SetHistory(masterTransactions);
                });

        /// <summary>
        /// Map transactions
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="transactions"></param>
        /// <returns></returns>
        public CreditMaster MapTransactions(CreditMaster creditMaster, IEnumerable<Credit> transactions) =>
            creditMaster.SetHistory(transactions);

        /// <summary>
        /// Transactions from dynamic query
        /// </summary>
        /// <param name="dynamicQuery"></param>
        /// <param name="loadCustomer"></param>
        /// <param name="loadStore"></param>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        public IEnumerable<Credit> TransactionsFromDynamicQuery(IEnumerable<dynamic> dynamicQuery, bool loadCustomer, bool loadStore,
            Customer customer = null, Store store = null)
        {
            if (dynamicQuery is ICollection<object> rows && rows.Any())
            {
                foreach (object row in rows)
                {
                    yield return TransactionFromDynamicRow(row, loadCustomer, loadStore,
                        customer, store);
                }
            }
        }

        /// <summary>
        /// Transaction from dynamic row
        /// </summary>
        /// <param name="row"></param>
        /// <param name="loadCustomer"></param>
        /// <param name="loadStore"></param>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        public Credit TransactionFromDynamicRow(object row, bool loadCustomer, bool loadStore,
            Customer customer = null, Store store = null)
        {
            Credit credit = null;

            if (row is IDictionary<string, object> dictionary)
            {
                ICollection<IDictionary<string, object>> dictionaries = SplitDictionary(dictionary);

                IEnumerator<IDictionary<string, object>> enumerator = dictionaries.GetEnumerator();

                enumerator.MoveNext();

                credit = GetTransaction(enumerator);

                bool next = enumerator.MoveNext();

                TryMapCustomer(credit, loadCustomer, customer, enumerator,
                    ref next);

                TryMapStoreTransaction(credit, loadStore, store, enumerator, ref next);
            }

            return credit;
        }

        /// <summary>
        /// Map current
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="enumerator"></param>
        /// <param name="next"></param>
        private void MapCurrent(CreditMaster creditMaster, IEnumerator<IDictionary<string, object>> enumerator,
            out bool next)
        {
            creditMaster.SetCurrent(GetTransaction(enumerator));

            next = enumerator.MoveNext();
        }

        /// <summary>
        /// Try map customer
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="load"></param>
        /// <param name="customer"></param>
        /// <param name="enumerator"></param>
        /// <param name="next"></param>
        private void TryMapCustomer(CreditMaster creditMaster, bool load, Customer customer,
            IEnumerator<IDictionary<string, object>> enumerator, ref bool next, bool setCreditLimit = true)
        {
            if (load && next)
            {
                customer = GetCustomer(enumerator);

                next = enumerator.MoveNext();
            }

            if (customer != null)
            {
                creditMaster.SetCustomer(customer, _credinetAppSettings, setCreditLimit);
            }
        }

        /// <summary>
        /// Try map customer
        /// </summary>
        /// <param name="credit"></param>
        /// <param name="load"></param>
        /// <param name="customer"></param>
        /// <param name="enumerator"></param>
        /// <param name="next"></param>
        private void TryMapCustomer(Credit credit, bool load, Customer customer,
            IEnumerator<IDictionary<string, object>> enumerator, ref bool next)
        {
            if (load && next)
            {
                customer = GetCustomer(enumerator);

                next = enumerator.MoveNext();
            }

            if (customer != null)
            {
                credit.SetCustomer(customer, _credinetAppSettings);
            }
        }

        /// <summary>
        /// Get customer
        /// </summary>
        /// <param name="enumerator"></param>
        /// <returns></returns>
        private Customer GetCustomer(IEnumerator<IDictionary<string, object>> enumerator)
        {
            Customer customer = Customer.New();
            MapEntity(customer, enumerator);
            MapEntity(customer.Name, enumerator);

            return customer;
        }

        /// <summary>
        /// Try map store
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="load"></param>
        /// <param name="store"></param>
        /// <param name="enumerator"></param>
        /// <param name="next"></param>
        private void TryMapStore(CreditMaster creditMaster, bool load, Store store,
            IEnumerator<IDictionary<string, object>> enumerator, ref bool next)
        {
            if (load && next)
            {
                store = Store.New();
                MapEntity(store, enumerator);

                next = enumerator.MoveNext();
            }

            if (store != null)
            {
                creditMaster.SetStore(store);
            }
        }

        /// <summary>
        /// Try map store transaction
        /// </summary>
        /// <param name="credit"></param>
        /// <param name="load"></param>
        /// <param name="store"></param>
        /// <param name="enumerator"></param>
        /// <param name="next"></param>
        private void TryMapStoreTransaction(Credit credit, bool load, Store store,
            IEnumerator<IDictionary<string, object>> enumerator, ref bool next)
        {
            if (load && next)
            {
                store = Store.New();
                MapEntity(store, enumerator);

                next = enumerator.MoveNext();
            }

            if (store != null)
            {
                credit.SetStore(store);
            }
        }

        /// <summary>
        /// Try map status
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="enumerator"></param>
        /// <param name="next"></param>
        private void TryMapStatus(CreditMaster creditMaster, IEnumerator<IDictionary<string, object>> enumerator,
            bool next)
        {
            Status status = null;
            if (next)
            {
                status = new Status();
                MapEntity(status, enumerator);
            }

            if (status != null)
            {
                creditMaster.SetStatus(status);
            }
        }

        /// <summary>
        /// Get transaction
        /// </summary>
        /// <param name="enumerator"></param>
        /// <returns></returns>
        private Credit GetTransaction(IEnumerator<IDictionary<string, object>> enumerator)
        {
            Credit credit = Credit.New();
            CreditPayment creditPayment = CreditPayment.New();

            MapEntity(credit, enumerator);
            MapEntity(creditPayment, enumerator);

            credit.SetCreditPayment(creditPayment);

            return credit;
        }

        /// <summary>
        /// From dynamic query
        /// </summary>
        /// <param name="dynamicQuery"></param>
        /// <returns></returns>
        public CreditRequestAgentAnalysis FromDynamicQuery(IEnumerable<dynamic> dynamicQuery)
        {
            CreditRequestAgentAnalysis customer = null;

            if (dynamicQuery is ICollection<object> rows && rows.Any())
            {
                customer = FromDynamicRow(rows.Single());
            }

            return customer;
        }

        /// <summary>
        /// From dynamic row
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public CreditRequestAgentAnalysis FromDynamicRow(object row)
        {
            CreditRequestAgentAnalysis creditRequestAgentAnalysis = null;

            if (row is IDictionary<string, object> dictionary)
            {
                ICollection<IDictionary<string, object>> dictionaries = SplitDictionary(dictionary);

                creditRequestAgentAnalysis = CreditRequestAgentAnalysis.New();

                IEnumerator<IDictionary<string, object>> enumerator = dictionaries.GetEnumerator();

                enumerator.MoveNext();

                MapEntity(creditRequestAgentAnalysis, enumerator);
            }

            return creditRequestAgentAnalysis;
        }
    }
}