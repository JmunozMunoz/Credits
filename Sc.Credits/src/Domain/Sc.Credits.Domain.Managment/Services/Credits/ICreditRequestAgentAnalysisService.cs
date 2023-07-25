using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Managment.Services.Credits
{
    /// <summary>
    /// ICreditRequestAgentAnalysisService
    /// </summary>
    public interface ICreditRequestAgentAnalysisService
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
        Task<CreditRequestAgentAnalysis> GetCreditRequestAgentAnalysis(
           string idDocument, decimal creditValue,
           string storeId, int AgentAnalysisResultId, IEnumerable<Field> fields);

        /// <summary>
        /// Gets the credit request by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        Task<CreditRequestAgentAnalysis> GetCreditRequestById(
           string id, IEnumerable<Field> fields);
    }
}