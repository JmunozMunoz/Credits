using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.Domain.Model.Credits.Queries;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Connection;
using Sc.Credits.DrivenAdapters.SqlServer.Dapper.Map;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.QueriesKata;
using Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Credits;
using Sc.Credits.DrivenAdapters.SqlServer.Repository.Base;
using Sc.Credits.Helpers.ObjectsUtils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sc.Credits.DrivenAdapters.SqlServer.Repository
{
    public class CreditRequestAgentAnalysisQueryRepository
        : CommandRepository<CreditRequestAgentAnalysis, CreditRequestAgentAnalysisFields>, ICreditRequestAgentAnalysisQueryRepository
    {
        #region Properties

        private static readonly CreditRequestAgentAnalysisQueries _customerQueries = QueriesCatalog.CreditRequestAgentAnalysisQueries;
        private readonly CredinetAppSettings _credinetAppSettings;

        private readonly CreditMapper _creditMapper;
        private readonly ISqlDelegatedHandlers<Profile> _profileSqlDelegatedHandlers;

        #endregion Properties

        /// <summary>
        /// Creates new instance of <see cref="CustomerRepository"/>
        /// </summary>
        /// <param name="connectionFactory"></param>
        /// <param name="customerSqlDelegatedHandlers"></param>
        /// <param name="profileSqlDelegatedHandlers"></param>
        public CreditRequestAgentAnalysisQueryRepository(ICreditsConnectionFactory connectionFactory,
            ISqlDelegatedHandlers<CreditRequestAgentAnalysis> customerSqlDelegatedHandlers)
            : base(_customerQueries, customerSqlDelegatedHandlers, connectionFactory)
        {
            _creditMapper = CreditMapper.New(_credinetAppSettings);
        }

        /// <summary>
        /// Gets the credit request agent analysis.
        /// </summary>
        /// <param name="idDocument">The identifier document.</param>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        public async Task<List<CreditRequestAgentAnalysis>> GetCreditRequestAgentAnalysis(string idDocument, IEnumerable<Field> fields) =>
        await ReadUsingConnectionAsync(async (Connection) =>
        {
            var creditRequestAgentAnalysis = await EntitySqlDelegatedHandlers.GetListAsync(Connection,
                    _customerQueries.GetCreditRequestAgentAnalysis(idDocument, fields));

            return creditRequestAgentAnalysis;
        });

        /// <summary>
        /// Gets the credit request agent analysis.
        /// </summary>
        /// <param name="idDocument">The identifier document.</param>
        /// <param name="creditValue">The credit value.</param>
        /// <param name="storeId">The store identifier.</param>
        /// <param name="AgentAnalysisResultId">The agent analysis result identifier.</param>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        public async Task<CreditRequestAgentAnalysis> GetCreditRequestAgentAnalysis(string idDocument, decimal creditValue, string storeId,
            int AgentAnalysisResultId, IEnumerable<Field> fields) =>
        await ReadUsingConnectionAsync(async (Connection) =>
        {
            var creditRequestAgentAnalysis = await EntitySqlDelegatedHandlers.GetSingleAsync(Connection,
                    _customerQueries.GetCreditRequestAgentAnalysis(idDocument, creditValue, storeId, AgentAnalysisResultId, fields));

            return creditRequestAgentAnalysis;
        });

        public async Task<CreditRequestAgentAnalysis> GetCreditByCustomerTransaction(
            string transactionId,
            IEnumerable<Field> fields) =>
        await ReadUsingConnectionAsync(async (Connection) =>
        {
            var creditRequestAgentAnalysis = await EntitySqlDelegatedHandlers.GetSingleAsync(Connection,
                  _customerQueries.GetCreditByCustomerTransaction(transactionId, fields));

             return creditRequestAgentAnalysis;
        });
    }
}