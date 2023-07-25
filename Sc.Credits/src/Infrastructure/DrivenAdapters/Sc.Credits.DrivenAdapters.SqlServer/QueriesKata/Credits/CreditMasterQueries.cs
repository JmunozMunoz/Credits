using Sc.Credits.Domain.Model.Credits.Queries;
using Sc.Credits.Domain.Model.Credits.Queries.Reading;
using Sc.Credits.Domain.Model.Customers.Queries;
using Sc.Credits.Domain.Model.Customers.Queries.Reading;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.Model.Queries.Extensions;
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
    /// Credit master queries
    /// </summary>
    internal class CreditMasterQueries
        : CommandQueries<CreditMasterFields>
    {
        private readonly CreditFields _creditFields = Tables.Catalog.Credits.Fields;
        private readonly CustomerFields _customerFields = Tables.Catalog.Customers.Fields;
        private readonly StoreFields _storeFields = Tables.Catalog.Stores.Fields;
        private readonly StatusFields _statusFields = Tables.Catalog.Status.Fields;

        /// <summary>
        /// Credit master queries
        /// </summary>
        public CreditMasterQueries()
            : base(Tables.Catalog.CreditsMaster)
        {
        }

        /// <summary>
        /// By id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fields"></param>
        /// <param name="customerFields"></param>
        /// <param name="storeFields"></param>
        public SqlQuery ById(Guid id, IEnumerable<Field> fields = null, IEnumerable<Field> customerFields = null,
            IEnumerable<Field> storeFields = null)
        {
            bool loadStore = storeFields != null && storeFields.Any();

            Query query =
                Query
                    .Where(Fields.Id.NameWithAlias, id)
                    .Join(Tables.Catalog.Customers.Name,
                        Fields.CustomerId.NameWithAlias,
                        _customerFields.Id.NameWithAlias)
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
        /// With current
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fields"></param>
        /// <param name="transactionFields"></param>
        /// <param name="customerFields"></param>
        /// <param name="storeFields"></param>
        /// <param name="transactionStoreFields"></param>
        /// <returns></returns>
        public SqlQuery WithCurrent(Guid id, IEnumerable<Field> fields = null,
            IEnumerable<Field> transactionFields = null, IEnumerable<Field> customerFields = null,
            IEnumerable<Field> storeFields = null, IEnumerable<Field> transactionStoreFields = null)
        {
            Query query =
                WithCurrentQuery(fields, transactionFields, customerFields, storeFields, transactionStoreFields)
                    .Where(Fields.Id.NameWithAlias, id);

            return ToReadQuery(query);
        }

        /// <summary>
        /// With current by statuses
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="customerId"></param>
        /// <param name="fields"></param>
        /// <param name="transactionFields"></param>
        /// <param name="storeFields"></param>
        /// <param name="transactionStoreFields"></param>
        /// <param name="statuses"></param>
        /// <returns></returns>
        public SqlQuery WithCurrent(List<Guid> ids, Guid customerId, IEnumerable<Field> storeFields = null,
            IEnumerable<Field> transactionStoreFields = null, IEnumerable<Statuses> statuses = null)
        {
            Query query =
                WithCurrentQuery(storeFields: storeFields, transactionStoreFields: transactionStoreFields)
                    .WhereIn(Fields.Id.NameWithAlias, ids)
                    .WhereIn(Fields.StatusId.NameWithAlias, statuses.Select(status => (int)status))
                    .Where(Fields.CustomerId.NameWithAlias, customerId);

            return ToReadQuery(query);
        }

        /// <summary>
        /// With current query
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="transactionFields"></param>
        /// <param name="customerFields"></param>
        /// <param name="storeFields"></param>
        /// <param name="transactionStoreFields"></param>
        /// <returns></returns>
        private Query WithCurrentQuery(IEnumerable<Field> fields = null, IEnumerable<Field> transactionFields = null,
            IEnumerable<Field> customerFields = null, IEnumerable<Field> storeFields = null,
            IEnumerable<Field> transactionStoreFields = null)
        {
            bool loadCustomer = customerFields != null && customerFields.Any();
            bool loadTransactionStore = transactionStoreFields != null && transactionStoreFields.Any();
            bool loadStore = storeFields != null && storeFields.Any();

            TableWithAlias<StoreFields> transactionStoreAliasedTable
                = Tables.Catalog.Stores.WithAlias(suffix: "Credit");

            Query query =
                Query
                    .Join(Tables.Catalog.Credits.Name,
                        Fields.LastId.NameWithAlias,
                        _creditFields.Id.NameWithAlias)
                    .When(loadCustomer,
                        q => q.Join(Tables.Catalog.Customers.Name,
                            Fields.CustomerId.NameWithAlias,
                            _customerFields.Id.NameWithAlias))
                    .When(loadStore,
                        q => q.Join(Tables.Catalog.Stores.Name,
                            Fields.StoreId.NameWithAlias,
                            _storeFields.Id.NameWithAlias))
                    .When(loadTransactionStore,
                        q => q.Join(transactionStoreAliasedTable.NameAs,
                            _creditFields.StoreId.NameWithAlias,
                            transactionStoreAliasedTable.Fields.Id.NameWithAlias))
                    .Select((fields ?? Fields.GetAllFields())
                        .Union(transactionFields ?? _creditFields.GetAllFields())
                        .Union(customerFields ?? Enumerable.Empty<Field>())
                        .Union(storeFields ?? Enumerable.Empty<Field>())
                        .Union(transactionStoreFields?.Rename(transactionStoreAliasedTable.Alias) ?? Enumerable.Empty<Field>())
                            .Select(f => f.NameWithAlias)
                            .ToArray());

            return query;
        }

        /// <summary>
        /// By ids
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="status"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public SqlQuery ByIds(List<Guid> ids, IEnumerable<Field> fields = null, IEnumerable<Field> customerFields = null,
            IEnumerable<Field> storeFields = null, int statusId = 0)
        {

            bool loadStatus = statusId > 0;

            Query query =
                Query
                    .WhereIn(Fields.Id.NameWithAlias, ids)
                    .When(loadStatus, q => q.Where(Fields.StatusId.NameWithAlias, statusId))
                    .Join(Tables.Catalog.Credits.Name,
                        Fields.LastId.NameWithAlias,
                        _creditFields.Id.NameWithAlias)
                    .Join(Tables.Catalog.Customers.Name,
                        Fields.CustomerId.NameWithAlias,
                        _customerFields.Id.NameWithAlias)
                    .Join(Tables.Catalog.Stores.Name,
                        Fields.StoreId.NameWithAlias,
                        _storeFields.Id.NameWithAlias)
                    .Select((fields ?? Fields.GetAllFields())
                        .Union(customerFields ?? Enumerable.Empty<Field>())
                        .Union(storeFields ?? Enumerable.Empty<Field>())
                            .Select(f => f.NameWithAlias)
                            .ToArray());

            return ToReadQuery(query);
        }

        /// <summary>
        /// By credit id
        /// </summary>
        /// <param name="creditId"></param>
        /// <param name="fields"></param>
        /// <param name="customerFields"></param>
        /// <param name="storeFields"></param>
        public SqlQuery ByCreditId(Guid creditId, IEnumerable<Field> fields = null,
            IEnumerable<Field> customerFields = null, IEnumerable<Field> storeFields = null)
        {
            bool loadCustomer = customerFields != null && customerFields.Any();
            bool loadStore = storeFields != null && storeFields.Any();

            Query query =
                Query
                    .Where(_creditFields.Id.NameWithAlias, creditId)
                    .Join(Tables.Catalog.Credits.Name,
                        Fields.Id.NameWithAlias,
                        _creditFields.CreditMasterId.NameWithAlias)
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
        /// By customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="statuses"></param>
        /// <param name="storeFields"></param>
        /// <returns></returns>
        public SqlQuery ByCustomer(Guid customerId, IEnumerable<Statuses> statuses, IEnumerable<Field> storeFields)
        {
            bool loadStore = storeFields != null && storeFields.Any();
            bool filterStatuses = statuses != null && statuses.Any();

            Query query =
                Query
                    .Where(Fields.CustomerId.NameWithAlias, customerId)
                    .When(filterStatuses,
                        q =>
                            q.WhereIn(Fields.StatusId.NameWithAlias, statuses.Select(status => (int)status)))
                    .Join(Tables.Catalog.Credits.Name,
                        Fields.LastId.NameWithAlias,
                        _creditFields.Id.NameWithAlias)
                    .When(loadStore,
                        q => q.Join(Tables.Catalog.Stores.Name,
                            Fields.StoreId.NameWithAlias,
                            _storeFields.Id.NameWithAlias))
                    .Select(Fields.GetAllFields()
                        .Union(storeFields ?? Enumerable.Empty<Field>())
                            .Select(f => f.NameWithAlias)
                            .ToArray());

            return ToReadQuery(query);
        }

        /// <summary>
        /// Duplicated
        /// </summary>
        /// <param name="token"></param>
        /// <param name="date"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public SqlQuery Duplicated(string token, DateTime date, Guid customerId)
        {
            Query query =
                Query
                    .Where(Fields.Token.Name, token)
                    .Where(Fields.CreditDate.Name, date)
                    .Where(Fields.CustomerId.Name, customerId)
                    .Select(Fields.Id.Name);

            return ToReadQuery(query);
        }

        /// <summary>
        /// Customer photo signature allowed
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="paidCreditDays"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public SqlQuery ValidateACreditPaidAccordingToTime(Guid customerId, int paidCreditDays, Statuses status)
        {
            DateTime limitDate = DateTime.Today.AddDays(-paidCreditDays);
            int statusId = (int)status;

            Query query =
                Query
                    .Where(Fields.CustomerId.Name, customerId)
                    .Where(Fields.CreditDate.Name, "<=", limitDate)
                    .Where(Fields.StatusId.Name, statusId)
                    .Select(Fields.Id.Name);

            return ToReadQuery(query);
        }

        /// <summary>
        /// Validate customer history
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public SqlQuery ValidateCustomerHistory(Guid customerId)
        {
            Query query =
                Query
                    .Where(Fields.CustomerId.Name, customerId)
                    .Select(Fields.Id.Name);

            return ToReadQuery(query);
        }

        /// <summary>
        /// Paid for certificate
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public SqlQuery PaidForCertificate(List<Guid> ids)
        {
            Query query =
                Query
                    .WhereIn(Fields.Id.NameWithAlias, ids)
                    .Where(Fields.StatusId.NameWithAlias, (int)Statuses.Paid)
                    .Join(Tables.Catalog.Credits.Name,
                        Fields.LastId.NameWithAlias,
                        _creditFields.Id.NameWithAlias)
                    .Join(Tables.Catalog.Customers.Name,
                        Fields.CustomerId.NameWithAlias,
                        _customerFields.Id.NameWithAlias)
                    .Join(Tables.Catalog.Stores.Name,
                        Fields.StoreId.NameWithAlias,
                        _storeFields.Id.NameWithAlias)
                    .Select(CreditMasterReadingFields.PaidCreditCertificate
                        .Union(CreditReadingFields.PaidCreditCertificate)
                        .Union(CustomerReadingFields.PaidCreditCertificate)
                        .Union(StoreReadingFields.PaidCreditCertificate)
                            .Select(f => f.NameWithAlias)
                            .ToArray());

            return ToReadQuery(query);
        }

        /// <summary>
        /// Customer history
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="storeId"></param>
        /// <param name="maximumMonthsCreditHistory"></param>
        /// <returns></returns>
        public SqlQuery CustomerHistory(Guid customerId, string storeId, int maximumMonthsCreditHistory)
        {
            DateTime limitDate = DateTime.Today.AddMonths(-maximumMonthsCreditHistory);
            bool hasTimeFilter = maximumMonthsCreditHistory > 0;

            var queryToGetVendorId =
                new Query(Tables.Catalog.Stores.Name)
                    .Where(_storeFields.Id.NameWithAlias, storeId).Select(_storeFields.VendorId.NameWithAlias);

            Query query =
                new Query(Tables.Catalog.Stores.Name)
                    .Where(Fields.CustomerId.NameWithAlias, customerId)
                    .When(hasTimeFilter, q => q.Where(Fields.CreditDate.NameWithAlias, ">=", limitDate))
                    .Join(queryToGetVendorId.As("storeVendorId"), join => join.On($"storeVendorId.{_storeFields.VendorId.Name}", _storeFields.VendorId.NameWithAlias))
                    .Join(Tables.Catalog.CreditsMaster.Name, Fields.StoreId.NameWithAlias, _storeFields.Id.NameWithAlias)
                    .Join(Tables.Catalog.Credits.Name,
                        Fields.LastId.NameWithAlias,
                        _creditFields.Id.NameWithAlias)
                    .Join(Tables.Catalog.Status.Name,
                        Fields.StatusId.NameWithAlias,
                        _statusFields.Id.NameWithAlias)
                    .Select(CreditMasterReadingFields.CustomerCreditHistory
                        .Union(CreditReadingFields.CustomerCreditHistory)
                        .Union(StoreReadingFields.CustomerCreditHistory)
                        .Union(_statusFields.GetAllFields())
                            .Select(f => f.NameWithAlias)
                            .ToArray());

            return ToReadQuery(query);
        }

        /// <summary>
        /// Active by collect type
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="collectTypeId"></param>
        /// <param name="vendorId"></param>
        /// <param name="businessGroupId"></param>
        /// <param name="includedStoreIds"></param>
        public SqlQuery ActiveByCollectType(Guid customerId, int collectTypeId, string vendorId, string businessGroupId,
            params string[] includedStoreIds)
        {
            CollectTypes collectType = (CollectTypes)collectTypeId;

            includedStoreIds =
                includedStoreIds?
                    .Where(storeId =>
                        !string.IsNullOrEmpty(storeId))
                    .ToArray();

            bool includeStoreIds = collectTypeId != (int)CollectTypes.All && includedStoreIds != null && includedStoreIds.Any();

            Query query =
                Query
                    .Where(Fields.CustomerId.NameWithAlias, customerId)
                    .Where(Fields.StatusId.NameWithAlias, (int)Statuses.Active)
                    .Where(qBusinessGroupAndVendor =>
                        qBusinessGroupAndVendor
                            .When(collectType == CollectTypes.Ordinary,
                                q =>
                                    q.When(!string.IsNullOrEmpty(businessGroupId),
                                        qVendorOrBusinessGroup =>
                                            qVendorOrBusinessGroup
                                                .Where(_storeFields.BusinessGroupId.NameWithAlias, businessGroupId)
                                                .OrWhere(_storeFields.VendorId.NameWithAlias, vendorId),
                                        qVendor =>
                                            qVendor.Where(_storeFields.VendorId.NameWithAlias, vendorId)))
                            .When(collectType == CollectTypes.AlternatePayment,
                                q =>
                                    q.When(!string.IsNullOrEmpty(businessGroupId),
                                        qVendorOrBusinessGroup =>
                                            qVendorOrBusinessGroup
                                                .Where(_storeFields.BusinessGroupId.NameWithAlias, businessGroupId)
                                                .OrWhere(_storeFields.VendorId.NameWithAlias, vendorId)
                                                .OrWhereTrue(_creditFields.AlternatePayment.NameWithAlias),
                                        qFinal =>
                                            qFinal
                                                .Where(_storeFields.VendorId.NameWithAlias, vendorId)
                                                .OrWhereTrue(_creditFields.AlternatePayment.NameWithAlias)))
                            .When(includeStoreIds,
                                q =>
                                    q
                                     .OrWhereIn(_storeFields.Id.NameWithAlias, includedStoreIds)))
                    .Join(Tables.Catalog.Credits.Name,
                        Fields.LastId.NameWithAlias,
                        _creditFields.Id.NameWithAlias)
                    .Join(Tables.Catalog.Stores.Name,
                        Fields.StoreId.NameWithAlias,
                        _storeFields.Id.NameWithAlias)
                    .Select(CreditMasterReadingFields.ActiveCredits
                        .Union(CreditReadingFields.ActiveCredits)
                        .Union(StoreReadingFields.ActiveCredits)
                            .Select(f => f.NameWithAlias)
                            .ToArray());

            return ToReadQuery(query);
        }


        /// <summary>
        /// Actives credits
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="includedStoreIds"></param>
        /// <returns></returns>
        public SqlQuery ActiveByCollectType(Guid customerId,
            params string[] includedStoreIds)
        {


            includedStoreIds =
                includedStoreIds?
                    .Where(storeId =>
                        !string.IsNullOrEmpty(storeId))
                    .ToArray();


            Query query =
                Query
                    .Where(Fields.CustomerId.NameWithAlias, customerId)
                    .Where(Fields.StatusId.NameWithAlias, (int)Statuses.Active)
                    .Where(qBusinessGroupAndVendor =>
                        qBusinessGroupAndVendor
                          )
                    .Join(Tables.Catalog.Credits.Name,
                        Fields.LastId.NameWithAlias,
                        _creditFields.Id.NameWithAlias)
                    .Join(Tables.Catalog.Stores.Name,
                        Fields.StoreId.NameWithAlias,
                        _storeFields.Id.NameWithAlias)
                    .Select(CreditMasterReadingFields.ActiveCredits
                        .Union(CreditReadingFields.ActiveCredits)
                        .Union(StoreReadingFields.ActiveCredits)
                            .Select(f => f.NameWithAlias)
                            .ToArray());

            return ToReadQuery(query);
        }


        /// <summary>
        /// Actives
        /// </summary>
        /// <param name="customerId"></param>
        public SqlQuery Actives(Guid customerId)
        {
            Query query =
                   Query
                       .Where(Fields.CustomerId.NameWithAlias, customerId)
                       .Where(Fields.StatusId.NameWithAlias, (int)Statuses.Active)
                       .WhereNot(_creditFields.SourceId.NameWithAlias, (int)Sources.Refinancing)
                        .Join(Tables.Catalog.Credits.Name,
                            Fields.LastId.NameWithAlias,
                            _creditFields.Id.NameWithAlias)
                        .Join(Tables.Catalog.Stores.Name,
                            Fields.StoreId.NameWithAlias,
                            _storeFields.Id.NameWithAlias)
                        .Select(CreditMasterReadingFields.ActiveCredits
                            .Union(CreditReadingFields.ActiveCredits)
                            .Union(StoreReadingFields.ActiveCredits)
                                .Select(f => f.NameWithAlias)
                                .ToArray());

            return ToReadQuery(query);
        }

        /// <summary>
        /// Actives and cancel request
        /// </summary>
        /// <param name="customerId"></param>
        public SqlQuery ActivesAndCancelRequest(Guid customerId)
        {
            Query query =
                   Query
                       .Where(Fields.CustomerId.NameWithAlias, customerId)
                       .WhereIn(Fields.StatusId.NameWithAlias, new[] { (int)Statuses.Active, (int)Statuses.CancelRequest })
                        .Join(Tables.Catalog.Credits.Name,
                            Fields.LastId.NameWithAlias,
                            _creditFields.Id.NameWithAlias)
                        .Join(Tables.Catalog.Stores.Name,
                            Fields.StoreId.NameWithAlias,
                            _storeFields.Id.NameWithAlias)
                        .Select(CreditMasterReadingFields.ActiveCredits
                            .Union(CreditReadingFields.ActiveCredits)
                            .Union(StoreReadingFields.ActiveCredits)
                                .Select(f => f.NameWithAlias)
                                .ToArray());

            return ToReadQuery(query);
        }

        /// <summary>
        /// Actives and cancel request
        /// </summary>
        /// <param name="customerId"></param>
        public SqlQuery ActivesRequestInStore(Guid customerId, string storeId)
        {
            Query query =
                   Query
                       .Where(Fields.CustomerId.NameWithAlias, customerId)
                       .Where(Fields.StatusId.NameWithAlias, (int)Statuses.Active)
                       .Where(Fields.StoreId.NameWithAlias, storeId)
                        .Join(Tables.Catalog.Credits.Name,
                            Fields.LastId.NameWithAlias,
                            _creditFields.Id.NameWithAlias)
                        .Join(Tables.Catalog.Stores.Name,
                            Fields.StoreId.NameWithAlias,
                            _storeFields.Id.NameWithAlias)
                        .Select(CreditMasterReadingFields.ActiveCreditsWithToken
                            .Union(CreditReadingFields.ActiveCredits)
                            .Union(StoreReadingFields.ActiveCredits)
                                .Select(f => f.NameWithAlias)
                                .ToArray());

            return ToReadQuery(query);
        }

        /// <summary>
        /// Customer payment history
        /// </summary>
        /// <param name="paymentIds"></param>
        /// <param name="fields"></param>
        /// <param name="storeFields"></param>
        /// <returns></returns>
        public SqlQuery CustomerPaymentHistory(List<Guid> paymentIds, IEnumerable<Field> fields,
            IEnumerable<Field> storeFields = null)
        {
            Query query =
                Query
                    .WhereIn(_creditFields.Id.NameWithAlias, paymentIds)
                    .Join(Tables.Catalog.Credits.Name,
                        Fields.Id.NameWithAlias,
                        _creditFields.CreditMasterId.NameWithAlias)
                    .Join(Tables.Catalog.Stores.Name,
                        Fields.StoreId.NameWithAlias,
                        _storeFields.Id.NameWithAlias)
                    .Select(fields
                        .Union(storeFields ?? Enumerable.Empty<Field>())
                            .Select(f => f.NameWithAlias)
                            .ToArray())
                        .Distinct();

            return ToReadQuery(query);
        }

        /// <summary>
        /// Active and pending cancellation credits
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="vendorId"></param>
        /// <param name="storeFields"></param>
        /// <returns></returns>
        public SqlQuery ActiveAndPendingCancellationCredits(Guid customerId, string vendorId,
            IEnumerable<Field> storeFields)
        {
            Query query =
               Query
                   .Where(Fields.CustomerId.NameWithAlias, customerId)
                   .Where(_storeFields.VendorId.NameWithAlias, vendorId)
                   .Where(q =>
                        q.Where(Fields.StatusId.NameWithAlias, (int)Statuses.Active)
                            .OrWhere(Fields.StatusId.NameWithAlias, (int)Statuses.CancelRequest))
                    .Join(Tables.Catalog.Stores.Name,
                        Fields.StoreId.NameWithAlias,
                        _storeFields.Id.NameWithAlias)
                    .Join(Tables.Catalog.Status.Name,
                        Fields.StatusId.NameWithAlias,
                        _statusFields.Id.NameWithAlias)
                    .Select(CreditMasterReadingFields.ActiveAndPendingCancellationCredits
                        .Union(storeFields)
                        .Union(_statusFields.GetAllFields())
                            .Select(f => f.NameWithAlias)
                            .ToArray());

            return ToReadQuery(query);
        }

        /// <summary>
        /// Active and pending cancellation payments
        /// </summary>
        /// <param name="customerId"></param>
        public SqlQuery ActiveAndPendingCancellationPayments(Guid customerId)
        {
            Query query =
               Query
                   .Where(Fields.CustomerId.NameWithAlias, customerId)
                   .Where(q =>
                        q.Where(Fields.StatusId.NameWithAlias, (int)Statuses.Active)
                            .OrWhere(Fields.StatusId.NameWithAlias, (int)Statuses.Paid))
                    .Join(Tables.Catalog.Stores.Name,
                        Fields.StoreId.NameWithAlias,
                        _storeFields.Id.NameWithAlias)
                    .Join(Tables.Catalog.Status.Name,
                        Fields.StatusId.NameWithAlias,
                        _statusFields.Id.NameWithAlias)
                    .Select(CreditMasterReadingFields.ActiveAndPendingCancellationPayments
                        .Union(StoreReadingFields.ActiveAndPendingCancellationPayments)
                        .Union(_statusFields.GetAllFields())
                            .Select(f => f.NameWithAlias)
                            .ToArray());

            return ToReadQuery(query);
        }

        /// <summary>
        /// Active and pending promissory file per day
        /// </summary>
        /// <param name="date"></param>
        /// <param name="customerFields"></param>
        /// <param name="top">limit of credits per transaction</param>
        public SqlQuery ActiveAndPendingPromissoryFile(DateTime date,
            IEnumerable<Field> customerFields,int top = 200)
        {
            bool loadCustomer = customerFields != null && customerFields.Any();

            Query query =
               Query
                   .WhereDatePart(DateQueryCatalog.PER_YEAR, Fields.CreateDate.NameWithAlias, date.Year)
                   .WhereDatePart(DateQueryCatalog.PER_MONTH, Fields.CreateDate.NameWithAlias, date.Month)
                   .WhereDatePart(DateQueryCatalog.PER_DAY, Fields.CreateDate.NameWithAlias, date.Day)
                   .Where(Fields.PromissoryNoteFileName.NameWithAlias, null)
                   .Join(Tables.Catalog.Credits.Name,
                        Fields.LastId.NameWithAlias,
                        _creditFields.Id.NameWithAlias)
                    .When(loadCustomer,
                        q => q.Join(Tables.Catalog.Customers.Name,
                        Fields.CustomerId.NameWithAlias,
                        _customerFields.Id.NameWithAlias))
                   .Limit(top)
                   .Select(CreditMasterReadingFields.PendingSendNotification
                        .Union(customerFields ?? Enumerable.Empty<Field>())
                            .Select(f => f.NameWithAlias)
                            .ToArray());

            return ToReadQuery(query);
        }
    }
}