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
    /// The default implementation of <see cref="IRequestCancelPaymentRepository"/>
    /// </summary>
    public class RequestCancelPaymentRepository
        : CommandRepository<RequestCancelPayment, RequestCancelPaymentFields>, IRequestCancelPaymentRepository
    {
        private static readonly RequestCancelPaymentQueries _requestCancelPaymentQueries = QueriesCatalog.RequestCancelPayment;
        private static readonly CreditMasterQueries _creditMasterQueries = QueriesCatalog.CreditMaster;
        private static readonly CreditQueries _creditQueries = QueriesCatalog.Credit;

        private readonly ISqlDelegatedHandlers<CreditMaster> _creditMasterSqlDelegatedHandlers;
        private readonly ISqlDelegatedHandlers<Credit> _creditSqlDelegatedHandlers;

        private readonly CreditMapper _creditMapper;
        private readonly RequestCancelPaymentMapper _requestCancelPaymentMapper;

        /// <summary>
        /// Creates new instance of <see cref="RequestCancelPaymentRepository"/>
        /// </summary>
        /// <param name="connectionFactory"></param>
        /// <param name="requestCancelPaymentSqlDelegatedHandlers"></param>
        /// <param name="creditMasterSqlDelegatedHandlers"></param>
        /// <param name="creditSqlDelegatedHandlers"></param>
        /// <param name="appSettings"></param>
        public RequestCancelPaymentRepository(ICreditsConnectionFactory connectionFactory,
            ISqlDelegatedHandlers<RequestCancelPayment> requestCancelPaymentSqlDelegatedHandlers,
            ISqlDelegatedHandlers<CreditMaster> creditMasterSqlDelegatedHandlers,
            ISqlDelegatedHandlers<Credit> creditSqlDelegatedHandlers,
            ISettings<CredinetAppSettings> appSettings)
            : base(_requestCancelPaymentQueries, requestCancelPaymentSqlDelegatedHandlers, connectionFactory)
        {
            _creditMapper = CreditMapper.New(appSettings.Get());
            _requestCancelPaymentMapper = RequestCancelPaymentMapper.New();

            _creditMasterSqlDelegatedHandlers = creditMasterSqlDelegatedHandlers;
            _creditSqlDelegatedHandlers = creditSqlDelegatedHandlers;
        }

        /// <summary>
        /// <see cref="IRequestCancelPaymentRepository.GetFromMastersAsync(List{Guid},
        /// IEnumerable{Field})(List{Guid}, IEnumerable{Field})"/>
        /// </summary>
        /// <param name="creditMasterIds"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public async Task<List<RequestCancelPayment>> GetFromMastersAsync(List<Guid> creditMasterIds, IEnumerable<Field> fields) =>
            await ReadUsingConnectionAsync(async (connection) =>
            {
                return await EntitySqlDelegatedHandlers.GetListAsync(connection,
                    _requestCancelPaymentQueries.FromMasters(creditMasterIds, fields));
            });

        /// <summary>
        /// <see cref="IRequestCancelPaymentRepository.GetByStatusFromMasterAsync(Guid,
        /// RequestStatuses, IEnumerable{Field})"/>
        /// </summary>
        /// <param name="creditMasterId"></param>
        /// <param name="status"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public async Task<List<RequestCancelPayment>> GetByStatusFromMasterAsync(Guid creditMasterId, RequestStatuses status,
            IEnumerable<Field> fields) =>
            await ReadUsingConnectionAsync(async (connection) =>
                await EntitySqlDelegatedHandlers.GetListAsync(connection, _requestCancelPaymentQueries.ByStatusFromMaster(creditMasterId, status, fields)));

        /// <summary>
        /// <see cref="IRequestCancelPaymentRepository.GetByStatusFromMasterAsync(CreditMaster,
        /// RequestStatuses, IEnumerable{Field}, IEnumerable{Field})"/>
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="status"></param>
        /// <param name="fields"></param>
        /// <param name="storeFields"></param>
        /// <returns></returns>
        public async Task<List<RequestCancelPayment>> GetByStatusFromMasterAsync(CreditMaster creditMaster, RequestStatuses status,
            IEnumerable<Field> fields, IEnumerable<Field> storeFields = null)
        {
            var dynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                return await EntitySqlDelegatedHandlers.QueryAsync(connection,
                    _requestCancelPaymentQueries.ByStatusFromMaster(creditMaster.Id, status, fields, storeFields));
            });

            IEnumerable<RequestCancelPayment> requestCancelPayments = _requestCancelPaymentMapper.FromDynamicQuery(dynamicQuery, loadStatus: false);

            return requestCancelPayments
                .Select(request =>
                    request
                        .SetCreditMaster(creditMaster)
                        .SetPayment(creditMaster.GetPayment(request.GetCreditId)))
                .ToList();
        }

        /// <summary>
        /// <see cref="IRequestCancelPaymentRepository.GetByStatusUntilDate(DateTime,RequestStatuses)
        /// </summary>
        /// <param name="cancellationRequestDate"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<List<RequestCancelPayment>> GetByStatusUntilDate(DateTime cancellationRequestDate, RequestStatuses status) =>
            await ReadUsingConnectionAsync(async (connection) =>
                await EntitySqlDelegatedHandlers.GetListAsync(connection, _requestCancelPaymentQueries.ByStatusAndDate(cancellationRequestDate, status)));

        /// <summary>
        /// <see cref="IRequestCancelPaymentRepository.GetUndismissedForMastersAsync(List{Guid}, IEnumerable{Field})"/>
        /// </summary>
        /// <param name="creditMasterIds"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public async Task<List<RequestCancelPayment>> GetUndismissedForMastersAsync(List<Guid> creditMasterIds, IEnumerable<Field> fields) =>
            await ReadUsingConnectionAsync(async (connection) =>
            {
                return await EntitySqlDelegatedHandlers.GetListAsync(connection,
                    _requestCancelPaymentQueries.UndismissedForMasters(creditMasterIds, fields));
            });

        /// <summary>
        /// <see cref="IRequestCancelPaymentRepository.GetUndismissedForMasterAsync(Guid, IEnumerable{Field})"/>
        /// </summary>
        /// <param name="creditMasterId"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public async Task<List<RequestCancelPayment>> GetUndismissedForMasterAsync(Guid creditMasterId, IEnumerable<Field> fields) =>
            await ReadUsingConnectionAsync(async (connection) =>
            {
                return await EntitySqlDelegatedHandlers.GetListAsync(connection,
                    _requestCancelPaymentQueries.UndismissedForMaster(creditMasterId, fields));
            });

        /// <summary>
        /// <see cref="IRequestCancelPaymentRepository.GetByVendorAsync(string, RequestStatuses,
        /// IEnumerable{Field}, IEnumerable{Field}, IEnumerable{Field}, IEnumerable{Field}, IEnumerable{Field})"/>
        /// </summary>
        /// <param name="vendorId"></param>
        /// <param name="status"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public async Task<RequestCancelPaymentPaged> GetByVendorAsync(string vendorId, int pageNumber, int valuePage, bool count, RequestStatuses status,
            IEnumerable<Field> fields, IEnumerable<Field> creditFields, IEnumerable<Field> paymentFields,
            IEnumerable<Field> customerFields, IEnumerable<Field> storeFields)
        {
            int totalRecords = 0;
            if (count)
            {
                 var countResult = ReadUsingConnectionAsync(async (connection) =>
                 {
                     return await EntitySqlDelegatedHandlers.QueryAsync(connection, _requestCancelPaymentQueries.CountByVendor(vendorId, status));
                 });

                totalRecords = countResult.Result.FirstOrDefault().CountVendor;
            }

            List<RequestCancelPayment> requestCancelPayments = await ReadUsingConnectionAsync(async (connection) =>
                 await EntitySqlDelegatedHandlers.GetListAsync(connection, _requestCancelPaymentQueries.ByVendor(vendorId, pageNumber, valuePage, status, fields)));

            var creditMasterDynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _creditMasterQueries.ByIds(ids: requestCancelPayments.Select(r => r.GetCreditMasterId).ToList(),
                        creditFields, customerFields);

                return await _creditMasterSqlDelegatedHandlers.QueryAsync(connection, query);
            });

            IEnumerable<CreditMaster> creditMasters =
                _creditMapper.MastersFromDynamicQuery(creditMasterDynamicQuery, loadTransaction: false,
                    loadCustomer: true, loadStore: true);

            var creditsDynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _creditQueries.ByIds(ids: requestCancelPayments.Select(r => r.GetCreditId).ToList(),
                        paymentFields, storeFields);

                return await _creditSqlDelegatedHandlers.QueryAsync(connection, query);
            });

            IEnumerable<Credit> payments =
                _creditMapper.TransactionsFromDynamicQuery(creditsDynamicQuery,
                    loadCustomer: false, loadStore: true);

            creditMasters = _creditMapper.MapTransactions(creditMasters, transactions: payments);

            List<RequestCancelPayment> requestCancelPayment = requestCancelPayments
                .Select(request =>
                    request.SetCreditMaster(creditMasters.First(c => c.Id == request.GetCreditMasterId)))
                .ToList();

            return new RequestCancelPaymentPaged
            {
                RequestCancelPayment = requestCancelPayment,
                TotalRecords = totalRecords
            };
        }

        /// <summary>
        /// </summary>
        /// <param name="paymentIds"></param>
        /// <param name="status"></param>
        /// <param name="fields"></param>
        /// <param name="statusFields"></param>
        /// <returns></returns>
        public async Task<List<RequestCancelPayment>> GetByNotStatusAsync(List<Guid> paymentIds, RequestStatuses status, IEnumerable<Field> fields,
            IEnumerable<Field> statusFields)
        {
            var requestCancelPaymentsDynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                SqlQuery query = _requestCancelPaymentQueries.ByNotStatus(paymentIds, status, fields, statusFields);

                return await EntitySqlDelegatedHandlers.QueryAsync(connection, query);
            });

            IEnumerable<RequestCancelPayment> requestCancelPayments =
                _requestCancelPaymentMapper.FromDynamicQuery(requestCancelPaymentsDynamicQuery, loadStatus: true);

            return requestCancelPayments.ToList();
        }

        /// <summary>
        /// <see cref="IRequestCancelPaymentRepository.GetUndismissedAsync(Guid, IEnumerable{Field})"/>
        /// </summary>
        /// <param name="creditId"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public async Task<RequestCancelPayment> GetUndismissedAsync(Guid creditId, IEnumerable<Field> fields) =>
            await ReadUsingConnectionAsync(async (connection) =>
            {
                return await EntitySqlDelegatedHandlers.GetSingleAsync(connection,
                    _requestCancelPaymentQueries.Undismissed(creditId, fields));
            });

        /// <summary>
        /// <see cref="IRequestCancelPaymentRepository.GetByCancellationIdAsync(Guid, IEnumerable{Field}, RequestStatuses)"/>
        /// </summary>
        /// <param name="creditCancelId"></param>
        /// <param name="fields"></param>
        /// <param name="requestStatuses"></param>
        /// <returns></returns>
        public async Task<RequestCancelPayment> GetByCancellationIdAsync(Guid creditCancelId, IEnumerable<Field> fields, RequestStatuses requestStatuses) =>
            await ReadUsingConnectionAsync(async (connection) =>
            {
                return await EntitySqlDelegatedHandlers.GetSingleAsync(connection,
                    _requestCancelPaymentQueries.ByCancellationId(creditCancelId, fields, requestStatuses));
            });
    }
}