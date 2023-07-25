using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.Model.Refinancings;
using Sc.Credits.Domain.Model.Refinancings.Gateway;
using Sc.Credits.Domain.Model.Refinancings.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Connection;
using Sc.Credits.DrivenAdapters.SqlServer.Dapper.Map;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.QueriesKata;
using Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Refinancings;
using Sc.Credits.DrivenAdapters.SqlServer.Repository.Base;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using System.Transactions;

namespace Sc.Credits.DrivenAdapters.SqlServer.Repository
{
    /// <summary>
    /// <see cref="IRefinancingRepository"/>
    /// </summary>
    public class RefinancingRepository
        : SqlRepository, IRefinancingRepository
    {
        private static readonly RefinancingApplicationQueries _refinancingApplicationQueries = QueriesCatalog.RefinancingApplication;


        private readonly ISqlDelegatedHandlers<RefinancingApplication> _refinancingApplicationSqlDelegatedHandlers;
        private readonly ISqlDelegatedHandlers<RefinancingLog> _refinancingLogSqlDelegatedHandlers;
        private readonly ISqlDelegatedHandlers<RefinancingLogDetail> _refinancingLogDetailSqlDelegatedHandlers;

        /// <summary>
        /// New refinancing simple repository
        /// </summary>
        /// <param name="connectionFactory"></param>
        /// <param name="refinancingApplicationSqlDelegatedHandlers"></param>
        /// <param name="refinancingLogSqlDelegatedHandlers"></param>
        public RefinancingRepository(ICreditsConnectionFactory connectionFactory,
            ISqlDelegatedHandlers<RefinancingApplication> refinancingApplicationSqlDelegatedHandlers,
            ISqlDelegatedHandlers<RefinancingLog> refinancingLogSqlDelegatedHandlers,
            ISqlDelegatedHandlers<RefinancingLogDetail> refinancingLogDetailSqlDelegatedHandlers)
            : base(connectionFactory)
        {
            _refinancingApplicationSqlDelegatedHandlers = refinancingApplicationSqlDelegatedHandlers;
            _refinancingLogSqlDelegatedHandlers = refinancingLogSqlDelegatedHandlers;
            _refinancingLogDetailSqlDelegatedHandlers = refinancingLogDetailSqlDelegatedHandlers;
        }

        /// <summary>
        /// <see cref="IRefinancingRepository.GetApplicationAsync(Guid)"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RefinancingApplication> GetApplicationAsync(Guid id)
        {
            RefinancingApplication refinancingApplication = null;

            using (DbConnection connection = ConnectionFactory.Create())
            {
                await connection.OpenAsync();

                refinancingApplication = await _refinancingApplicationSqlDelegatedHandlers.GetSingleAsync(connection,
                    _refinancingApplicationQueries.ById(id));
            }

            return refinancingApplication;
        }

        /// <summary>
        /// <see cref="IRefinancingRepository.AddLogAsync(RefinancingLog, Transaction)"/>
        /// </summary>
        /// <param name="log"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async Task AddLogAsync(RefinancingLog log, Transaction transaction)
        {
            using (DbConnection connection = ConnectionFactory.Create())
            {
                await connection.OpenAsync();

                if (transaction != null)
                {
                    connection.EnlistTransaction(transaction);
                }

                await _refinancingLogSqlDelegatedHandlers.InsertAsync(connection, log, Tables.Catalog.RefinancingLogs.Name);

                foreach (RefinancingLogDetail detail in log.Details)
                {
                    await _refinancingLogDetailSqlDelegatedHandlers.InsertAsync(connection, detail,
                        Tables.Catalog.RefinancingLogDetails.Name);
                }
            }
        }


     


    }
}