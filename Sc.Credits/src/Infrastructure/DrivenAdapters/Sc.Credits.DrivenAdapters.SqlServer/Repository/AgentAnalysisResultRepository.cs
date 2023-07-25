using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.Domain.Model.Credits.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Connection;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.QueriesKata;
using Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Credits;
using Sc.Credits.DrivenAdapters.SqlServer.Repository.Base;

namespace Sc.Credits.DrivenAdapters.SqlServer.Repository
{
    public class AgentAnalysisResultRepository
        : CommandRepository<AgentAnalysisResult, AgentAnalysisResultFields>, IAgentAnalysisResultRepository
    {
        private static readonly AgentAnalysisResultQueries _agentAnalysisResultQueries = QueriesCatalog.AgentAnalysisResultQueries;

        public AgentAnalysisResultRepository(ICreditsConnectionFactory connectionFactory,
            ISqlDelegatedHandlers<AgentAnalysisResult> AgentAnalysisResultSqlDelegatedHandlers)
            : base(_agentAnalysisResultQueries, AgentAnalysisResultSqlDelegatedHandlers, connectionFactory)
        {
        }
    }
}