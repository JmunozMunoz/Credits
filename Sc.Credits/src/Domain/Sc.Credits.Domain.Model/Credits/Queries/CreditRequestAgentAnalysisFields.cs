using Sc.Credits.Domain.Model.Queries;
using System.Reflection;

namespace Sc.Credits.Domain.Model.Credits.Queries
{
    public class CreditRequestAgentAnalysisFields
        : EntityFields
    {
        public CreditRequestAgentAnalysisFields(string alias)
            : base(alias)
        {
        }

        /// <summary>
        /// Gets the customer identifier.
        /// </summary>
        /// <value>
        /// The customer identifier.
        /// </value>
        public Field CustomerId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the customer document identifier.
        /// </summary>
        /// <value>
        /// The customer document identifier.
        /// </value>
        public Field CustomerIdDocument => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the credit value.
        /// </summary>
        /// <value>
        /// The credit value.
        /// </value>
        public Field CreditValue => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the observations.
        /// </summary>
        /// <value>
        /// The observations.
        /// </value>
        public Field Observations => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the store identifier.
        /// </summary>
        /// <value>
        /// The store identifier.
        /// </value>
        public Field StoreId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the transaction date.
        /// </summary>
        /// <value>
        /// The transaction date.
        /// </value>
        public Field TransactionDate => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the transaction time.
        /// </summary>
        /// <value>
        /// The transaction time.
        /// </value>
        public Field TransactionTime => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the agent analysis result id
        /// </summary>
        /// <value>
        /// The transaction status identifier.
        /// </value>
        public Field AgentAnalysisResultId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public Field SourceId => GetField(MethodBase.GetCurrentMethod().Name);
    }
}