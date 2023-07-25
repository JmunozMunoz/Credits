using Sc.Credits.Domain.Model.Customers;
using System.Collections.Generic;

namespace Sc.Credits.Domain.Managment.Tests.Entities
{
	public class CustomerRiskLevelBuilder
	{
		public string Level {get; set;}
		public string Description {get; set;}
		public List<string> Observations { get; set; }

		private const string _level = "";
		private const string _description = "";
		private readonly List<string> _observations = new List<string>();

		public CustomerRiskLevelBuilder(string level, string descripcion) { 
			Level = level;
			Description = descripcion;
			Observations = _observations;
		}

		public CustomerRiskLevelBuilder WithLevel(string level)
		{
			this.Level = level;
			return this;
		}

		public CustomerRiskLevelBuilder WithDescription(string description)
		{
			this.Description = description;
			return this;
		}

		public CustomerRiskLevelBuilder WithObservations(List<string> observations)
		{
			this.Observations = observations;
			return this;
		}

		public CustomerRiskLevel Build()
		{
			return new CustomerRiskLevel
			{
				Level = Level,
				Description = Description,
				Observations = Observations
			};
		}
	}
}
