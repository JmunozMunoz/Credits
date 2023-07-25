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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc.Credits.DrivenAdapters.SqlServer.Repository
{
    public class RefinancingLogRepository
        : CommandRepository<RefinancingLog, RefinancingLogFields>, IRefinancingLogRepository
    {
        private static readonly RefinancingLogQueries _refinancingLogQueries = QueriesCatalog.RefinancingLog;
        private readonly RefinancingLogMapper _refinancingLogMapper;

        public RefinancingLogRepository(ICreditsConnectionFactory connectionFactory,
            ISqlDelegatedHandlers<RefinancingLog> entitySqlDelegatedHandlers)
            : base(_refinancingLogQueries, entitySqlDelegatedHandlers, connectionFactory)
        {
            _refinancingLogMapper = RefinancingLogMapper.New();
        }

        public async Task<List<RefinancingLogDetail>> GetByStatusFromMasterAsync(Guid refinancingLogId,
            IEnumerable<Field> fields, IEnumerable<Field> storeFields = null)
        {
            var dynamicQuery = await ReadUsingConnectionAsync(async (connection) =>
            {
                return await EntitySqlDelegatedHandlers.QueryAsync(connection,
                    _refinancingLogQueries.ByStatusFromMaster(refinancingLogId, fields, storeFields));
            });

            List<RefinancingLogDetail> requestCancelPayments = 
                _refinancingLogMapper.MastersFromDynamicQuery(dynamicQuery).ToList();

            return requestCancelPayments;
        }
    }
}
