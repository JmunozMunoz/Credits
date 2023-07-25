namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The calculated query entity
    /// </summary>
    public class CalculatedQuery
    {
        /// <summary>
        /// Gets or sets the credit's id
        /// </summary>
        public string CreditId { get; set; }

        /// <summary>
        /// Gets or sets the minimum payment
        /// </summary>
        public decimal? MinimumPayment { get; set; }

        /// <summary>
        /// Gets or sets the total payment
        /// </summary>
        public decimal? TotalPayment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that has arrears
        /// </summary>
        public bool HasArrears { get; set; }

        /// <summary>
        /// Gets or sets the arrears days
        /// </summary>
        public int ArrearsDays { get; set; }

        /// <summary>
        /// Gets or sets a value indicating values are calculated
        /// </summary>
        public bool IsCalculate { get; set; }

        /// <summary>
        /// Gets or sets the arrears payment
        /// </summary>
        public decimal ArrearsPayment { get; set; }

        /// <summary>
        /// Gets or sets the exception's message
        /// </summary>
        public string MessageException { get; set; }

        
    }
}