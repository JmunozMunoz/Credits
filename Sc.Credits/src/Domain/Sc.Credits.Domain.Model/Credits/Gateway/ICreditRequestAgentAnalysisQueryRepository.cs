using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Domain.Model.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Model.Credits.Gateway
{
    public interface ICreditRequestAgentAnalysisQueryRepository
        : ICommandRepository<CreditRequestAgentAnalysis>
    {
        /// <summary>
        /// Gets the credit request agent analysis.
        /// </summary>
        /// <param name="idDocument">The identifier document.</param>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        Task<List<CreditRequestAgentAnalysis>> GetCreditRequestAgentAnalysis(string idDocument, IEnumerable<Field> fields);

        /// <summary>
        /// Gets the credit request agent analysis.
        /// </summary>
        /// <param name="idDocument">The identifier document.</param>
        /// <param name="creditValue">The credit value.</param>
        /// <param name="storeId">The store identifier.</param>
        /// <param name="AgentAnalysisResultId">The agent analysis result identifier.</param>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        Task<CreditRequestAgentAnalysis> GetCreditRequestAgentAnalysis(string idDocument, decimal creditValue, string storeId,
            int AgentAnalysisResultId, IEnumerable<Field> fields);

        /// <summary>
        /// Get Credit by transaction
        /// </summary>
        /// <param name="transactionId">transaction identifier</param>
        /// <param name="fields"></param>
        /// <returns></returns>
        Task<CreditRequestAgentAnalysis> GetCreditByCustomerTransaction(
            string transactionId,
            IEnumerable<Field> fields);
    }
}