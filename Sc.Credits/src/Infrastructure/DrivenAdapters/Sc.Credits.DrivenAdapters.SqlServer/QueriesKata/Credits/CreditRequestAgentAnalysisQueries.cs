using Sc.Credits.Domain.Model.Credits.Queries;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.Model.Queries.Extensions;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using SqlKata;
using System.Collections.Generic;
using System.Linq;

namespace Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Credits
{
    /// <summary>
    /// CreditRequestAgentAnalysisQueries
    /// </summary>
    /// <seealso cref="Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.CommandQueries&lt;Sc.Credits.Domain.Model.Credits.Queries.CreditRequestAgentAnalysisFields&gt;" />
    internal class CreditRequestAgentAnalysisQueries
        : CommandQueries<CreditRequestAgentAnalysisFields>
    {
        public CreditRequestAgentAnalysisQueries()
            : base(Tables.Catalog.CreditRequestAgentAnalyses)
        { }

        /// <summary>
        /// Gets the credit request agent analysis.
        /// </summary>
        /// <param name="idDocument">The identifier document.</param>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        public SqlQuery GetCreditRequestAgentAnalysis(string idDocument, IEnumerable<Field> fields)
        {
            Query query = Query.Where(Fields.CustomerIdDocument.Name, idDocument)
                    .Select((fields ?? Fields.GetAllFields()).Select(f => f.Name).ToArray());

            return ToReadQuery(query);
        }

        /// <summary>
        /// Gets the credit request agent analysis.
        /// </summary>
        /// <param name="idDocument">The identifier document.</param>
        /// <param name="creditValue">The credit value.</param>
        /// <param name="storeId">The store identifier.</param>
        /// <param name="AgentAnalysisResultId">The agent analysis result identifier.</param>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        public SqlQuery GetCreditRequestAgentAnalysis(string idDocument, decimal creditValue, string storeId, int AgentAnalysisResultId,
            IEnumerable<Field> fields)
        {
            Query query =
                BasicInfoQueryFilter(idDocument, creditValue, storeId)
                    .Where(Fields.AgentAnalysisResultId.Name, AgentAnalysisResultId)
                    .Select((fields ?? Fields.GetAllFields()).Select(f => f.Name).ToArray());

            return ToReadQuery(query);
        }
        /// <summary>
        /// Get credit by transaction id
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public SqlQuery GetCreditByCustomerTransaction(string transactionId,
           IEnumerable<Field> fields)
        {
            Query query = BasicInfoQueryFilterByTransactionId(transactionId)
                    .Select((fields ?? Fields.GetAllFields()).Select(f => f.Name).ToArray());

            return ToReadQuery(query);
        }

        /// <summary>
        /// Basics the information query filter.
        /// </summary>
        /// <param name="idDocument">The identifier document.</param>
        /// <param name="creditValue">The credit value.</param>
        /// <param name="storeId">The store identifier.</param>
        /// <returns></returns>
        private Query BasicInfoQueryFilter(string idDocument, decimal creditValue, string storeId)
        {
            return Query.Where(Fields.CustomerIdDocument.Name, idDocument)
                         .Where(Fields.CreditValue.Name, creditValue)
                         .Where(Fields.StoreId.Name, storeId);
        }
        /// <summary>
        /// Basic query by transactionId
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        private Query BasicInfoQueryFilterByTransactionId(string transactionId)
        {
            return Query.Where(Fields.Id.Name, transactionId);
        }
    }
}