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
    /// Request cancel credit queries
    /// </summary>
    internal class RequestCancelCreditQueries
        : CommandQueries<RequestCancelCreditFields>
    {
        private static readonly StoreFields _storeFields = Tables.Catalog.Stores.Fields;
        private static readonly BusinessGroupFields _businessGroupFields = Tables.Catalog.BusinessGroup.Fields;

        /// <summary>
        /// New request cancel credit
        /// </summary>
        public RequestCancelCreditQueries()
            : base(Tables.Catalog.RequestCancelCredits)
        {
        }

        /// <summary>
        /// By status
        /// </summary>
        /// <param name="creditMasterId"></param>
        /// <param name="status"></param>
        /// <param name="storeFields"></param>
        /// <returns></returns>
        public SqlQuery ByStatus(Guid creditMasterId, RequestStatuses status, IEnumerable<Field> storeFields = null, 
                                IEnumerable<Field> businessGroupFields = null)
        {
            bool loadStore = storeFields != null && storeFields.Any();

            Query query =
                Query
                    .Where(Fields.CreditMasterId.NameWithAlias, creditMasterId)
                    .Where(Fields.RequestStatusId.NameWithAlias, (int)status)
                    .When(loadStore,
                        q => q.Join(Tables.Catalog.Stores.Name,
                            Fields.StoreId.NameWithAlias,
                            _storeFields.Id.NameWithAlias)
                           .When(businessGroupFields!=null,
                        bg => bg.Join(Tables.Catalog.BusinessGroup.Name,
                                  _businessGroupFields.Id.NameWithAlias,
                                  _storeFields.BusinessGroupId.NameWithAlias)))
                    .Select(new List<Field>()
                        {
                            Fields.Id,
                            Fields.ValueCancel,
                            Fields.CancellationType
                        }
                        .Union(storeFields ?? Enumerable.Empty<Field>())
                        .Union(businessGroupFields ?? Enumerable.Empty<Field>())
                        .Select(f => f.NameWithAlias)
                        .ToArray());

            return ToReadQuery(query);
        }

        /// <summary>
        /// By status
        /// </summary>
        /// <param name="creditMasterIds"></param>
        /// <param name="status"></param>
        /// <param name="storeFields"></param>
        /// <returns></returns>
        public SqlQuery ByStatus(List<Guid> creditMasterIds, RequestStatuses status, IEnumerable<Field> storeFields = null)
        {
            bool loadStore = storeFields != null && storeFields.Any();

            Query query =
                Query
                    .WhereIn(Fields.CreditMasterId.Name, creditMasterIds)
                    .Where(Fields.RequestStatusId.Name, (int)status)
                    .When(loadStore,
                        q => q.Join(Tables.Catalog.Stores.Name,
                            Fields.StoreId.Name,
                            _storeFields.Id.Name))
                    .Select(new List<Field>()
                        {
                            Fields.Id,
                            Fields.CreditMasterId,
                            Fields.ProcessDate
                        }
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
            Query query =
                Query
                    .Where(Fields.RequestStatusId.NameWithAlias, (int)status)
                    .WhereDate(Fields.Date.NameWithAlias, "<=", cancellationRequestDate);

            return ToReadQuery(query);
        }

        /// <summary>
        /// By vendor
        /// </summary>
        /// <param name="vendorId"></param>
        /// <param name="status"></param>
        /// <param name="pageNumber"></param>
        /// <param name="valuePage"></param>
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