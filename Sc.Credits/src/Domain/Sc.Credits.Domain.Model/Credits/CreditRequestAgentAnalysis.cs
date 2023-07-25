using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Helpers.Commons.Dapper.Annotations;
using Sc.Credits.Helpers.Commons.Domain;
using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// CreditRequestAgentAnalysis
    /// </summary>
    /// <seealso cref="Entity&lt;System.Guid&gt;" />
    /// <seealso cref="IAggregateRoot" />
    public class CreditRequestAgentAnalysis
        : Entity<Guid>, IAggregateRoot
    {
        private Guid _customerId;
        private string _customerIdDocument;
        private decimal _creditValue;
        private string _observations;
        private string _storeId;
        private DateTime _transactionDate;
        private TimeSpan _transactionTime;
        private int _sourceId;
      
        /// <summary>
        /// Gets the customer's id field
        /// </summary>
        [Write(false)]
        public Guid CustomerId => _customerId;

        /// <summary>
        /// Gets the customer document identifier.
        /// </summary>
        /// <value>
        /// The customer document identifier.
        /// </value>
        [Write(false)]
        public string CustomerIdDocument => _customerIdDocument;

        /// <summary>
        /// Gets the credit value field
        /// </summary>
        [Write(false)]
        public decimal CreditValue => _creditValue;

        /// <summary>
        /// Gets the observations.
        /// </summary>
        /// <value>
        /// The observations.
        /// </value>
        [Write(false)]
        public string Observations => _observations;

        /// <summary>
        /// Gets the store's id field
        /// </summary>
        [Write(false)]
        public string StoreId => _storeId;

        /// <summary>
        /// Gets the transaction date field
        /// </summary>
        [Write(false)]
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// Gets the transaction time field
        /// </summary>
        [Write(false)]
        public TimeSpan TransactionTime { get; set; }

        /// <summary>
        /// Gets the Transaction Date Complete
        /// </summary>
        [Write(false)]
        public DateTime TransactionDateComplete => _transactionDate + _transactionTime;

        /// <summary>
        /// Gets the agent analysis result id.
        /// </summary>
        /// <value>
        /// The transaction status.
        /// </value>
        public int AgentAnalysisResultId { get; private set; }

        /// <summary>
        /// Gets the agent analysis result
        /// </summary>
        [Write(false)]
        public AgentAnalysisResult AgentAnalysisResult { get; set; }

        /// <summary>
        /// Gets the agent analysis result
        /// </summary>
        [Write(false)]
        public Source Source { get; set; }

        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        [Write(false)]
        public int SourceId => _sourceId;

    

        /// <summary>
        /// Initializes a new instance of the <see cref="CreditRequestAgentAnalysis"/> class.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="customerIdDocument">The customer document identifier.</param>
        /// <param name="creditValue">The credit value.</param>
        /// <param name="observations">The observations.</param>
        /// <param name="storeId">The store identifier.</param>
        /// <param name="transactionDate">The transaction date.</param>
        /// <param name="transactionTime">The transaction time.</param>
        /// <param name="agentAnalysisResultId">The transaction status.</param>
        /// <param name="sourceId"></param>
        public CreditRequestAgentAnalysis(
            Guid customerId,
            string customerIdDocument,
            decimal creditValue,
            string observations,
            string storeId,
            DateTime transactionDate,
            TimeSpan transactionTime,
            int agentAnalysisResultId,
            int sourceId)
        {
            SetId(IdentityGenerator.NewSequentialGuid());
            _customerId = customerId;
            _customerIdDocument = customerIdDocument;
            _creditValue = creditValue;
            _observations = observations;
            _storeId = storeId;
            _transactionDate = transactionDate;
            _transactionTime = transactionTime;
            _sourceId = sourceId;
            SetAnalysisResult(agentAnalysisResultId);
        }

        public CreditRequestAgentAnalysis()
        {
        }

        public static CreditRequestAgentAnalysis New() =>
            new CreditRequestAgentAnalysis();

        public void SetAnalysisResult(int analysisResultId)
        {
            AgentAnalysisResultId = analysisResultId;
        }
    }
}