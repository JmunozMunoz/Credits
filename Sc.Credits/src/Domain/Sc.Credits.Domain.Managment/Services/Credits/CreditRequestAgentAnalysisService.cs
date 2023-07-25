using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.Domain.Model.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Managment.Services.Credits
{
    /// <summary>
    /// CreditRequestAgentAnalysisService
    /// </summary>
    /// <seealso cref="Sc.Credits.Domain.Managment.Services.Credits.ICreditRequestAgentAnalysisService" />
    public class CreditRequestAgentAnalysisService :
        ICreditRequestAgentAnalysisService
    {
        private readonly ICreditRequestAgentAnalysisQueryRepository _creditRequestAgentAnalysisQueryRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreditRequestAgentAnalysisService"/> class.
        /// </summary>
        /// <param name="creditRequestAgentAnalysisQueryRepository">The credit request agent analysis query repository.</param>
        public CreditRequestAgentAnalysisService(ICreditRequestAgentAnalysisQueryRepository creditRequestAgentAnalysisQueryRepository)
        {
            _creditRequestAgentAnalysisQueryRepository = creditRequestAgentAnalysisQueryRepository;
        }

        /// <summary>
        /// <see cref="ICreditRequestAgentAnalysisService.GetCreditRequestAgentAnalysis(string, IEnumerable{Field})"/>
        /// </summary>
        /// <param name="idDocument">The identifier document.</param>
        /// <param name="fields">The fields.</param>
        /// <param name="profileFields">The profile fields.</param>
        /// <returns></returns>
        public async Task<List<CreditRequestAgentAnalysis>> GetCreditRequestAgentAnalysis(
            string idDocument,
            IEnumerable<Field> fields)
        {
            var creditRequestAgentAnalysis =
                await _creditRequestAgentAnalysisQueryRepository.GetCreditRequestAgentAnalysis(idDocument, fields);

            return creditRequestAgentAnalysis;
        }

        /// <summary>
        /// <see cref="ICreditRequestAgentAnalysisService.GetCreditRequestAgentAnalysis(string, decimal, string, int, IEnumerable{Field})"/>
        /// </summary>
        /// <param name="idDocument">The identifier document.</param>
        /// <param name="creditValue">The credit value.</param>
        /// <param name="storeId">The store identifier.</param>
        /// <param name="AgentAnalysisResultId">The agent analysis result identifier.</param>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        public async Task<CreditRequestAgentAnalysis> GetCreditRequestAgentAnalysis(
           string idDocument, decimal creditValue,
           string storeId, int AgentAnalysisResultId, IEnumerable<Field> fields)
        {
            CreditRequestAgentAnalysis creditRequestAgentAnalysis =
                await _creditRequestAgentAnalysisQueryRepository
                    .GetCreditRequestAgentAnalysis(
                        idDocument, creditValue, storeId, AgentAnalysisResultId, fields);

            return creditRequestAgentAnalysis;
        }

        /// <summary>
        /// <see cref="ICreditRequestAgentAnalysisService.GetCreditRequestById(string, IEnumerable{Field})"/>
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="creditValue"></param>
        /// <param name="storeId"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public async Task<CreditRequestAgentAnalysis> GetCreditRequestById(
           string id, IEnumerable<Field> fields)
        {
            CreditRequestAgentAnalysis creditRequestAgentAnalysis =
                 await _creditRequestAgentAnalysisQueryRepository
                     .GetCreditByCustomerTransaction(
                        id, fields);

            return creditRequestAgentAnalysis;
        }
    }
}