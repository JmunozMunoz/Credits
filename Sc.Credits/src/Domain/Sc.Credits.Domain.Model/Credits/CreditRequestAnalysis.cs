namespace Sc.Credits.Domain.Model.Credits
{
    public class CreditRequestAnalysis
    {
        /// <summary>
        /// Gets or sets the customer id document.
        /// </summary>
        /// <value>
        /// The customer document identifier.
        /// </value>
        public string CustomerIdDocument { get; set; }

        /// <summary>
        /// Gets or sets the customer status.
        /// </summary>
        /// <value>
        /// The customer status.
        /// </value>
        public int CustomerStatus { get; set; }

        /// <summary>
        /// Gets or sets the credit value.
        /// </summary>
        /// <value>
        /// The credit value.
        /// </value>
        public decimal CreditValue { get; set; }

        /// <summary>
        /// Gets or sets the observations.
        /// </summary>
        /// <value>
        /// The observations.
        /// </value>
        public string Observations { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the frequency.
        /// </summary>
        /// <value>
        /// The frequency.
        /// </value>
        public int Frequency { get; set; }

        /// <summary>
        /// Gets or sets the months.
        /// </summary>
        /// <value>
        /// The months.
        /// </value>
        public int Months { get; set; }

        /// <summary>
        /// Gets or sets the store identifier.
        /// </summary>
        /// <value>
        /// The store identifier.
        /// </value>
        public string StoreId { get; set; }
    }
}