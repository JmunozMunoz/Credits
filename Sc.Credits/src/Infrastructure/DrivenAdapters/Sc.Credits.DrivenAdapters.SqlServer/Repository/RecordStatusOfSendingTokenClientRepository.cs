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
    public class CreditRequestAgentAnalysisRepository
        : CommandRepository<CreditRequestAgentAnalysis, CreditRequestAgentAnalysisFields>, ICreditRequestAgentAnalysisRepository
    {
        private static readonly CreditRequestAgentAnalysisQueries _creditRequestAgentAnalysisQueries = QueriesCatalog.CreditRequestAgentAnalysisQueries;

        public CreditRequestAgentAnalysisRepository(ICreditsConnectionFactory connectionFactory,
            ISqlDelegatedHandlers<CreditRequestAgentAnalysis> creditRequestAgentAnalysisSqlDelegatedHandlers)
            : base(_creditRequestAgentAnalysisQueries, creditRequestAgentAnalysisSqlDelegatedHandlers, connectionFactory)
        {
        }
    }
}