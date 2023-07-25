using Sc.Credits.Domain.Model.Credits.Queries;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.Model.Stores.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using SqlKata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Credits
{
    /// <summary>
    /// Request cancel payment queries
    /// </summary>
    internal class RequestCancelPaymentQueries
        : CommandQueries<RequestCancelPaymentFields>
    {
        private static readonly StoreFields _storeFields = Tables.Catalog.Stores.Fields;
        private static readonly BusinessGroupFields _businessGroupFields = Tables.Catalog.BusinessGroup.Fields;
        private static readonly RequestStatusFields _requestStatusFields = Tables.Catalog.RequestStatus.Fields;

        /// <summary>
        /// New request cancel payments
        /// </summary>
        public RequestCancelPaymentQueries()
            : base(Tables.Catalog.RequestCancelPayments)
        {
        }

        /// <summary>
        /// From masters
        /// </summary>
        /// <param name="creditMasterIds"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public SqlQuery FromMasters(List<Guid> creditMasterIds, IEnumerable<Field> fields)
        {
            Query query =
                Query
                    .WhereIn(Fields.CreditMasterId.Name, creditMasterIds)
                    .Select(fields
                        .Select(f => f.Name)
                        .ToArray());

            return ToReadQuery(query);
        }

        /// <summary>
        /// By status from master
        /// </summary>
        /// <param name="creditMasterId"></param>
        /// <param name="status"></param>
        /// <param name="fields"></param>
        /// <param name="storeFields"></param>
        /// <returns></returns>
        public SqlQuery ByStatusFromMaster(Guid creditMasterId, RequestStatuses status, IEnumerable<Field> fields,
            IEnumerable<Field> storeFields = null)
        {
            Query query =
                Query
                    .Where(Fields.CreditMasterId.NameWithAlias, creditMasterId)
                    .Where(Fields.RequestStatusId.NameWithAlias, (int)status)
                    .When(storeFields != null,
                        q =>
                            q.Join(Tables.Catalog.Stores.Name,
                                Fields.StoreId.NameWithAlias,
                                _storeFields.Id.NameWithAlias)
                            .Join(Tables.Catalog.BusinessGroup.Name,
                                  _businessGroupFields.Id.NameWithAlias,
                                  _storeFields.BusinessGroupId.NameWithAlias)
                           )
                    .Select(fields
                        .Union(storeFields ?? Enumerable.Empty<Field>())
                            .Select(f => f.NameWithAlias)
                            .ToArray());

            return ToReadQuery(query);
        }

        /// <summary>
        /// By status and date
        /// </summary>
        /// <param name="cancellationRequestDate"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public SqlQuery ByStatusAndDate(DateTime cancellationRequestDate, RequestStatuses status)
        {
            Query queryToGetCreditMasterIds =
                Query
                    .Where(Fields.RequestStatusId.NameWithAlias, (int)status)
                    .WhereDate(Fields.Date.NameWithAlias, "<=", cancellationRequestDate)
                    .Select(Fields.CreditMasterId.NameWithAlias);

            Query query =
                Query
                     .Where(Fields.RequestStatusId.NameWithAlias, (int)status)
                     .WhereIn(Fields.CreditMasterId.NameWithAlias, queryToGetCreditMasterIds);

            return ToReadQuery(query);
        }

        /// <summary>
        /// By not status
        /// </summary>
        /// <param name="paymentIds"></param>
        /// <param name="status"></param>
        /// <param name="fields"></param>
        /// <param name="statusFields"></param>
        /// <returns></returns>
        public SqlQuery ByNotStatus(List<Guid> paymentIds, RequestStatuses status, IEnumerable<Field> fields, IEnumerable<Field> statusFields)
        {
            Query query =
                Query
                    .WhereIn(Fields.CreditId.Name, paymentIds)
                    .WhereNot(Fields.RequestStatusId.Name, (int)status)
                    .Join(Tables.Catalog.RequestStatus.Name,
                        Fields.RequestStatusId.NameWithAlias,
                        _requestStatusFields.Id.NameWithAlias)
                    .Select(fields
                        .Union(statusFields)
                        .Select(f => f.NameWithAlias)
                        .ToArray());

            return ToReadQuery(query);
        }

        /// <summary>
        /// Undismissed for masters
        /// </summary>
        /// <param name="creditMasterIds"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public SqlQuery UndismissedForMasters(List<Guid> creditMasterIds, IEnumerable<Field> fields)
        {
            Query query =
                Query
                    .WhereIn(Fields.CreditMasterId.Name, creditMasterIds)
                    .Where(q =>
                        q.Where(Fields.RequestStatusId.Name, (int)RequestStatuses.Pending)
                        .Or()
                        .Where(Fields.RequestStatusId.Name, (int)RequestStatuses.Cancel))
                    .Select(fields
                        .Select(f => f.Name)
                        .ToArray());

            return ToReadQuery(query);
        }

        /// <summary>
        /// Undismissed for master
        /// </summary>
        /// <param name="creditMasterId"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public SqlQuery UndismissedForMaster(Guid creditMasterId, IEnumerable<Field> fields)
        {
            Query query =
                Query
                    .Where(Fields.CreditMasterId.Name, creditMasterId)
                    .Where(q =>
                        q.Where(Fields.RequestStatusId.Name, (int)RequestStatuses.Pending)
                        .Or()
                        .Where(Fields.RequestStatusId.Name, (int)RequestStatuses.Cancel))
                    .Select(fields
                        .Select(f => f.Name)
                        .ToArray());

            return ToReadQuery(query);
        }

        /// <summary>
        /// Undismissed
        /// </summary>
        /// <param name="creditId"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public SqlQuery Undismissed(Guid creditId, IEnumerable<Field> fields)
        {
            Query query =
                Query
                    .Where(Fields.CreditId.Name, creditId)
                    .Where(q =>
                        q.Where(Fields.RequestStatusId.Name, (int)RequestStatuses.Pending)
                        .Or()
                        .Where(Fields.RequestStatusId.Name, (int)RequestStatuses.Cancel))
                    .Select(fields
                        .Select(f => f.Name)
                        .ToArray());

            return ToReadQuery(query);
        }

        /// <summary>
        /// By cancellation id
        /// </summary>
        /// <param name="creditCancelId"></param>
        /// <param name="fields"></param>
        /// <param name="requestStatuses"></param>
        /// <returns></returns>
        public SqlQuery ByCancellationId(Guid creditCancelId, IEnumerable<Field> fields, RequestStatuses requestStatuses)
        {
            Query query =
                Query
                    .Where(Fields.CreditCancelId.Name, creditCancelId)
                    .Where(Fields.RequestStatusId.Name, (int)requestStatuses)
                    .Select(fields
                        .Select(f => f.Name)
                        .ToArray())
                    .Take(1);

            return ToReadQuery(query);
        }

        /// <summary>
        /// By vendor
        /// </summary>
        /// <param name="vendorId"></param>
        /// <param name="status"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public SqlQuery ByVendor(string vendorId, int pageNumber, int valuePage, RequestStatuses status, IEnumerable<Field> fields)
        {
            int currentRegistry = 0;

            if (pageNumber != 1)
            {
                currentRegistry = (pageNumber - 1) * valuePage;
            }

            Query query =
                Query
                    .Where(Fields.RequestStatusId.NameWithAlias, (int)status)
                    .Where(_storeFields.VendorId.NameWithAlias, vendorId)
                    .Join(Tables.Catalog.Stores.Name,
                        Fields.StoreId.NameWithAlias,
                        _storeFields.Id.NameWithAlias)
                    .Skip(currentRegistry)
                    .Take(valuePage)
                    .Select(fields
                        .Select(f => f.NameWithAlias)
                        .ToArray());

            return ToReadQuery(query);
        }

        /// <summary>
        /// CountByVendor
        /// </summary>
        /// <param name="vendorId"></param>
        /// <param name="status"></param>
        /// <param name="fields"></param>
        /// <returns></returns>

        public SqlQuery CountByVendor(string vendorId, RequestStatuses status)
        {
            Query query =
                Query
                    .Where(Fields.RequestStatusId.NameWithAlias, (int)status)
                    .Where(_storeFields.VendorId.NameWithAlias, vendorId)
                    .Join(Tables.Catalog.Stores.Name,
                        Fields.StoreId.NameWithAlias,
                        _storeFields.Id.NameWithAlias)
                    .SelectRaw("count(1) as CountVendor");

            return ToReadQuery(query);
        }
    }
}