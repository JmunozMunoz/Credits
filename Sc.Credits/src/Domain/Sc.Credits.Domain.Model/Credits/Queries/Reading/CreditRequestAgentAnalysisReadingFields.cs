using Sc.Credits.Domain.Model.Queries;
using System.Collections.Generic;

namespace Sc.Credits.Domain.Model.Credits.Queries.Reading
{
    /// <summary>
    /// CreditRequestAgentAnalysisReadingFields
    /// </summary>
    public class CreditRequestAgentAnalysisReadingFields
    {
        private static readonly CreditRequestAgentAnalysisFields _creditRequestAgentAnalysisFields = Tables.Catalog.CreditRequestAgentAnalyses.Fields;

        /// <summary>
        /// Gets the result information.
        /// </summary>
        /// <value>
        /// The result information.
        /// </value>
        public static IEnumerable<Field> ResultInfo =>
            new List<Field>()
            {
                _creditRequestAgentAnalysisFields.Id,
                _creditRequestAgentAnalysisFields.AgentAnalysisResultId,
                _creditRequestAgentAnalysisFields.TransactionDate,
                _creditRequestAgentAnalysisFields.TransactionTime
            };
    }
}