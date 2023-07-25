using Sc.Credits.Domain.Model.Queries;
using System.Collections.Generic;

namespace Sc.Credits.Domain.Model.Credits.Queries.Commands
{
    public class CreditRequestAgentAnalysisCommandsFields
    {
        private static readonly CreditRequestAgentAnalysisFields _creditRequestAgentAnalysisFields = Tables.Catalog.CreditRequestAgentAnalyses.Fields;

        public static IEnumerable<Field> InfoUpdate =>
            new List<Field>()
           {
                _creditRequestAgentAnalysisFields.AgentAnalysisResultId
           };
    }
}
