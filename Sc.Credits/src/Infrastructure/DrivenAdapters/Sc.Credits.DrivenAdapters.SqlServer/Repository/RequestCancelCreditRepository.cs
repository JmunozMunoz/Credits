using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.Domain.Model.Credits.Queries;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Queries;
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
using System.Linq;
using System.Threading.Tasks;

namespace Sc.Credits.DrivenAdapters.SqlServer.Repository
{
    /// <summary>
    /// The default implementation of <see cref="IRequestCancelCreditRepository"/>
    /// </summary>
    public class RequestCancelCreditRepository
        : CommandRepository<RequestCancelCredit, RequestCancelCreditFields>, IRequestCancelCreditRepository
    {
        private static readonly RequestCancelCreditQueries _requestCancelCreditQueries = QueriesCatalog.RequestCancelCredit;
        private static readonly CreditMasterQueries _creditMasterQueries = QueriesCatalog.CreditMaster;
        private static readonly CreditQueries _creditQueries = QueriesCatalog.Credit;

        private readonly CreditMapper _creditMapper;
        private readonly RequestCancelCreditMapper _requestCancelCreditMapper;

        private readonly ISqlDelegatedHandlers<CreditMaster> _creditMasterSqlDelegatedHandlers;
        private readonly ISqlDelegatedHandlers<Credit> _creditSqlDelegatedHandlers;

        /// <summary>
        /// Creates new instance of <see cref="RequestCancelCreditRepository"/>
        /// </summary>
        /// <param name="connectionFactory"></param>
        /// <param name="requestCancelCreditSqlDelegatedHandlers"></param>
        /// <param name="creditMasterSqlDelegatedHandlers"></param>
        /// <param name="creditSqlDelegatedHandlers"></param>
        /// <param name="appSettings"></param>
        public RequestCancelCreditRepository(ICreditsConnectionFactory connectionFactory,
            ISqlDelegatedHandlers<RequestCancelCredit> requestCancelCreditSqlDelegatedHandlers,
            ISqlDelegatedHandlers<CreditMaster> creditMasterSqlDelegatedHandlers,
            ISqlDelegatedHandlers<Credit> creditSqlDelegatedHandlers,
            ISettings<CredinetAppSettings> appSettings)
            : base(_requestCancelCreditQueries, requestCancelCreditSqlDelegatedHandlers, connectionFactory)
        {
            _creditMapper = CreditMapper.New(appSettings.Get());
            _requestCancelCreditMapper = RequestCancelCreditMapper.New();

            _creditMasterSqlDelegatedHandlers = creditMasterSqlDelegatedHandlers;
            _creditSqlDelegatedHandlers = creditSqlDelegatedHandlers;
        }

        /// <summary>
        /// <see cref="IRequestCancelCreditRepository.GetByStatusAsync(Guid, RequestStatuses, IEnumerable{Field}, IEnumerable{Field})"/>
        /// </summary>
        /// <param name="creditMasterId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<RequestCancelCredit> GetByStatusAsync(Guid creditMasterId, RequestStatuses status,
            IEnumerable<Field> storeFields = null, IEnumerable<Field> businessGroupFields = null)
        {
            var dynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                return await EntitySqlDelegatedHandlers.QueryAsync(connection,
                    _requestCancelCreditQueries.ByStatus(creditMasterId, status, storeFields, businessGroupFields));
            });

            return _requestCancelCreditMapper.FromDynamicQuery(dynamicQuery);
        }

        /// <summary>
        /// <see cref="IRequestCancelCreditRepository.GetByStatusAsync(List{Guid}, RequestStatuses, IEnumerable{Field})"/>
        /// </summary>
        /// <param name="creditMasterIds"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<List<RequestCancelCredit>> GetByStatusAsync(List<Guid> creditMasterIds, RequestStatuses status,
            IEnumerable<Field> storeFields = null) =>
            await ReadUsingConnectionAsync(async (connection) =>
            {
                return await EntitySqlDelegatedHandlers.GetListAsync(connection,
                    _requestCancelCreditQueries.ByStatus(creditMasterIds, status, storeFields));
            });

        /// <summary>
        /// <see cref="IRequestCancelCreditRepository.GetByStatusUntilDateAsync(DateTime, RequestStatuses)"/>
        /// </summary>
        /// <param name="cancellationRequestDate"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<List<RequestCancelCredit>> GetByStatusUntilDateAsync(DateTime cancellationRequestDate,
            RequestStatuses status) =>
            await ReadUsingConnectionAsync(async (connection) =>
            {
                return await EntitySqlDelegatedHandlers.GetListAsync(connection,
                    _requestCancelCreditQueries.ByStatusAndDate(cancellationRequestDate, status));
            });

        /// <summary>
        /// <see cref="IRequestCancelCreditRepository.GetByVendorAsync(string, RequestStatuses)"/>
        /// </summary>
        /// <param name="vendorId"></param>
        /// <param name="status"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public async Task<RequestCancelCreditPaged> GetByVendorAsync(string vendorId, int pageNumber, int valuePage, bool count, RequestStatuses status,
            IEnumerable<Field> fields, IEnumerable<Field> creditFields, IEnumerable<Field> paymentFields,
            IEnumerable<Field> customerFields, IEnumerable<Field> storeFields)
        {
            int totalRecords = 0;
            if (count)
            {

                var countResult =  ReadUsingConnectionAsync(async (connection) =>
                {
                    return await EntitySqlDelegatedHandlers.QueryAsync(connection,
                                                                  _requestCancelCreditQueries.CountByVendor(vendorId, status));
                });

                totalRecords = countResult.Result.FirstOrDefault().CountVendor;
            }

            List<RequestCancelCredit> requestCancelCredits = await ReadUsingConnectionAsync(async (connection) =>
                 await EntitySqlDelegatedHandlers.GetListAsync(connection, _requestCancelCreditQueries.ByVendor(vendorId, pageNumber, valuePage, status, fields)));

            var creditMasterDynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _creditMasterQueries.ByIds(ids: requestCancelCredits.Select(r => r.GetCreditMasterId).ToList(),
                        creditFields, customerFields, storeFields);

                return await _creditMasterSqlDelegatedHandlers.QueryAsync(connection, query);
            });

            IEnumerable<CreditMaster> creditMasters =
                _creditMapper.MastersFromDynamicQuery(creditMasterDynamicQuery, loadTransaction: false,
                    loadCustomer: true, loadStore: true);

            var creditsDynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _creditQueries.ByMasters(creditMasterIds: creditMasters.Select(r => r.Id).ToList(),
                        fields: paymentFields);

                return await _creditSqlDelegatedHandlers.QueryAsync(connection, query);
            });

            IEnumerable<Credit> payments =
                _creditMapper.TransactionsFromDynamicQuery(creditsDynamicQuery,
                    loadCustomer: false, loadStore: false);

            creditMasters = _creditMapper.MapTransactions(creditMasters, transactions: payments);

            List<RequestCancelCredit> requestCancelCredit = requestCancelCredits
                .Select(request =>
                    request.SetCreditMaster(creditMasters.First(c => c.Id == request.GetCreditMasterId)))
                .ToList();

            return new RequestCancelCreditPaged
            {
                RequestCancelCredit = requestCancelCredit,
                TotalRecords = totalRecords
        };
    }
}
}