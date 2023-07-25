using Sc.Credits.Domain.Model.Credits;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.Domain.Managment.Tests.Entities.Credits
{
    public class CreditRequestAgentAnalysisTestBuilder
    {
		private DateTime TransactionDate { get; set; }
		private TimeSpan TransactionTime { get; set; }
		private int AgentAnalysisResultId { get; set; }
		private AgentAnalysisResult AgentAnalysisResult { get; set; }
		
		private Source Source { get; set; }

		public DateTime _transactionDate = DateTime.Now;
		public TimeSpan _transactionTime = DateTime.Now.TimeOfDay;
		public int _agentAnalysisResultId= 1;
		public AgentAnalysisResult _agentAnalysisResult;
		
		public Source _source;
		public CreditRequestAgentAnalysisTestBuilder() {
			TransactionDate = _transactionDate;
			TransactionTime	= _transactionTime;
			AgentAnalysisResultId = _agentAnalysisResultId;
			AgentAnalysisResult = _agentAnalysisResult;
			Source= _source;

		}

		public CreditRequestAgentAnalysisTestBuilder WithTransactionDate(DateTime transactionDate)
		{
			_transactionDate = transactionDate;
			return this;
		}

		public CreditRequestAgentAnalysisTestBuilder WithTransactionTime(TimeSpan transactionTime)
		{
			_transactionTime = transactionTime;
			return this;
		}

		public CreditRequestAgentAnalysisTestBuilder WithAgentAnalysisResultId(int agentAnalysisResultId)
		{
			_agentAnalysisResultId = agentAnalysisResultId;
			return this;
		}

		public CreditRequestAgentAnalysisTestBuilder WithAgentAnalysisResult(AgentAnalysisResult agentAnalysisResult)
		{
			_agentAnalysisResult = agentAnalysisResult;
			return this;
		}
		
		public CreditRequestAgentAnalysisTestBuilder WithSource(Source source)
		{
			_source = source;
			return this;
		}

		public CreditRequestAgentAnalysis Build()
		{
			return new CreditRequestAgentAnalysis
			{
				TransactionDate = TransactionDate,
				TransactionTime = TransactionTime,
				AgentAnalysisResult = AgentAnalysisResult,
				Source = Source
			};
		}
	}
}

