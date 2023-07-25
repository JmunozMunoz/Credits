using Sc.Credits.Domain.Model.Credits.Queries;
using Sc.Credits.Domain.Model.Credits.Queries.Reading;
using Sc.Credits.Domain.Model.Customers.Queries;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.Model.Queries.Extensions;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Domain.Model.Stores.Queries;
using Sc.Credits.Domain.Model.Stores.Queries.Reading;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using SqlKata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Credits
{
    /// <summary>
    /// Credit queries
    /// </summary>
    internal class CreditQueries
        : ReadQueries<CreditFields>
    {
        private readonly CreditMasterFields _creditMasterFields = Tables.Catalog.CreditsMaster.Fields;
        private readonly CustomerFields _customerFields = Tables.Catalog.Customers.Fields;
        private readonly StoreFields _storeFields = Tables.Catalog.Stores.Fields;

        /// <summary>
        /// New credit queries
        /// </summary>
        public CreditQueries()
            : base(Tables.Catalog.Credits)
        {
        }

        /// <summary>
        /// By ids
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="fields"></param>
        /// <param name="storeFields"></param>
        /// <returns></returns>
        public SqlQuery ByIds(List<Guid> ids, IEnumerable<Field> fields,
            IEnumerable<Field> storeFields)
        {
            Query query =
                Query
                    .WhereIn(Fields.Id.NameWithAlias, ids)
                    .Join(Tables.Catalog.Stores.Name,
                        Fields.StoreId.NameWithAlias,
                        _storeFields.Id.NameWithAlias)
                    .Select(fields
                        .Union(storeFields)
                        .Select(f => f.NameWithAlias)
                        .ToArray());

            return ToReadQuery(query);
        }

        /// <summary>
        /// Customer payment history
        /// </summary>
        /// <param name="store"></param>
        /// <param name="customerId"></param>
        /// <param name="storeFields"></param>
        /// <param name="maximumMonthsPaymentHistory"></param>
        /// <returns></returns>
        public SqlQuery CustomerPaymentHistory(Store store, Guid customerId, IEnumerable<Field> storeFields,
            int maximumMonthsPaymentHistory)
        {
            DateTime limitDate = DateTime.Today.AddMonths(-maximumMonthsPaymentHistory);

            bool hasBusinessGroupId = !string.IsNullOrEmpty(store.GetBusinessGroupId);
            bool hasTimeFilter = maximumMonthsPaymentHistory > 0;

            TableWithAlias<StoreFields> masterStoreAliasedTable
                = Tables.Catalog.Stores.WithAlias(suffix: "CreditMaster");

            Query query =
                Query.Where(Fields.CustomerId.NameWithAlias, customerId)
                     .Where(Fields.TransactionTypeId.NameWithAlias, (int)TransactionTypes.Payment)
                     .When(hasTimeFilter, q => q.Where(Fields.TransactionDate.NameWithAlias, ">=", limitDate))
                     .Where(q =>
                        q.Where(Fields.StoreId.NameWithAlias, store.Id)
                         .When(hasBusinessGroupId,
                            q2 => q2.OrWhere(masterStoreAliasedTable.Fields.BusinessGroupId.NameWithAlias, store.GetBusinessGroupId),
                            q2 => q2.OrWhere(masterStoreAliasedTable.Fields.VendorId.NameWithAlias, store.GetVendorId))
                        )
                    .Join(Tables.Catalog.CreditsMaster.Name,
                        Fields.CreditMasterId.NameWithAlias,
                        _creditMasterFields.Id.NameWithAlias)
                    .Join(masterStoreAliasedTable.NameAs,
                        _creditMasterFields.StoreId.NameWithAlias,
                        masterStoreAliasedTable.Fields.Id.NameWithAlias)
                    .Join(Tables.Catalog.Stores.Name,
                        Fields.StoreId.NameWithAlias,
                        _storeFields.Id.NameWithAlias)
                    .Select(CreditReadingFields.CustomerPaymentHistory
                        .Union(storeFields)
                            .Select(f => f.NameWithAlias)
                            .ToArray());

            return ToReadQuery(query);
        }

        /// <summary>
        /// By master
        /// </summary>
        /// <param name="creditMasterId"></param>
        /// <param name="fields"></param>
        /// <param name="storeFields"></param>
        public SqlQuery ByMaster(Guid creditMasterId, IEnumerable<Field> fields = null, IEnumerable<Field> storeFields = null)
        {
            bool loadStore = storeFields != null && storeFields.Any();

            Query query =
                Query
                    .Where(Fields.CreditMasterId.NameWithAlias, creditMasterId)
                    .When(loadStore,
                        q => q.Join(Tables.Catalog.Stores.Name,
                            Fields.StoreId.NameWithAlias,
                            _storeFields.Id.NameWithAlias))
                    .Select((fields ?? Fields.GetAllFields())
                        .Union(storeFields ?? Enumerable.Empty<Field>())
                        .Select(f => f.NameWithAlias)
                        .ToArray());

            return ToReadQuery(query);
        }

        /// <summary>
        /// By masters
        /// </summary>
        /// <param name="creditMasterIds"></param>
        /// <param name="fields"></param>
        /// <param name="customerFields"></param>
        /// <param name="storeFields"></param>
        /// <returns></returns>
        public SqlQuery ByMasters(List<Guid> creditMasterIds, IEnumerable<Field> fields = null,
            IEnumerable<Field> customerFields = null, IEnumerable<Field> storeFields = null)
        {
            bool loadCustomer = customerFields != null && customerFields.Any();
            bool loadStore = storeFields != null && storeFields.Any();

            Query query =
                Query
                    .WhereIn(Fields.CreditMasterId.NameWithAlias, creditMasterIds)
                    .When(loadCustomer,
                        q => q.Join(Tables.Catalog.Customers.Name,
                            Fields.CustomerId.NameWithAlias,
                            _customerFields.Id.NameWithAlias))
                    .When(loadStore,
                        q => q.Join(Tables.Catalog.Stores.Name,
                            Fields.StoreId.NameWithAlias,
                            _storeFields.Id.NameWithAlias))
                    .Select((fields ?? Fields.GetAllFields())
                        .Union(customerFields ?? Enumerable.Empty<Field>())
                        .Union(storeFields ?? Enumerable.Empty<Field>())
                        .Select(f => f.NameWithAlias)
                        .ToArray());

            return ToReadQuery(query);
        }

        /// <summary>
        /// Active and pending cancellation payments
        /// </summary>
        /// <param name="creditMasterIds"></param>
        public SqlQuery ActiveAndPendingCancellationPayments(List<Guid> creditMasterIds)
        {
            Query query =
                Query
                    .WhereIn(Fields.CreditMasterId.Name, creditMasterIds)
                    .Join(Tables.Catalog.Stores.Name,
                        Fields.StoreId.NameWithAlias,
                        _storeFields.Id.NameWithAlias)
                    .Select(CreditReadingFields.ActiveAndPendingCancellationPayments
                        .Union(StoreReadingFields.ActiveAndPendingCancellationPayments)
                            .Select(f => f.NameWithAlias)
                            .ToArray());

            return ToReadQuery(query);
        }

        /// <summary>
        /// Payments
        /// </summary>
        /// <param name="paymentIds"></param>
        /// <param name="fields"></param>
        /// <param name="customerFields"></param>
        /// <param name="storeFields"></param>
        /// <param name="masterStoreFields"></param>
        public SqlQuery Payments(List<Guid> paymentIds, IEnumerable<Field> fields, IEnumerable<Field> customerFields,
            IEnumerable<Field> storeFields) =>
            Transactions(ids: paymentIds, customerFields, storeFields, fields, new TransactionTypes[] { TransactionTypes.Payment });

        /// <summary>
        /// Payments
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="fields"></param>
        /// <param name="customerFields"></param>
        /// <param name="storeFields"></param>
        /// <param name="masterStoreFields"></param>
        public SqlQuery Transactions(List<Guid> ids, IEnumerable<Field> customerFields, IEnumerable<Field> storeFields,
            IEnumerable<Field> fields = null, TransactionTypes[] transactionTypes = null)
        {
            bool filterByTransactionType = transactionTypes != null && transactionTypes.Any();

            Query query =
                Query
                    .WhereIn(Fields.Id.NameWithAlias, ids)
                    .When(filterByTransactionType,
                        q =>
                            q.WhereIn(Fields.TransactionTypeId.NameWithAlias, transactionTypes.Select(transactionType => (int)transactionType)))
                    .Join(Tables.Catalog.CreditsMaster.Name,
                        Fields.CreditMasterId.NameWithAlias,
                        _creditMasterFields.Id.NameWithAlias)
                    .Join(Tables.Catalog.Customers.Name,
                        Fields.CustomerId.NameWithAlias,
                        _customerFields.Id.NameWithAlias)
                    .Join(Tables.Catalog.Stores.Name,
                        Fields.StoreId.NameWithAlias,
                        _storeFields.Id.NameWithAlias)
                    .Select((fields ?? Fields.GetAllFields())
                        .Union(customerFields)
                        .Union(storeFields)
                            .Select(f => f.NameWithAlias)
                            .ToArray());

            return ToReadQuery(query);
        }
    }
}