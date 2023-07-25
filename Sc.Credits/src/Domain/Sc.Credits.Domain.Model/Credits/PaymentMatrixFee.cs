namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The payment matrix fee entity
    /// </summary>
    public class PaymentMatrixFee : FeeBase
    {
        /// <summary>
        /// Gets or sets the credit value payment
        /// </summary>
        public decimal CreditValuePayment { get; set; }

        /// <summary>
        /// Gets or sets the assurance value payment
        /// </summary>
        public decimal AssuranceValuePayment { get; set; }

        /// <summary>
        /// Gets or sets the credit value not due
        /// </summary>
        public decimal CreditValueNotDue { get; set; }

        /// <summary>
        /// Gets or sets the interest days
        /// </summary>
        public int InterestDays { get; set; }

        /// <summary>
        /// Gets or sets the interest payment
        /// </summary>
        public decimal InterestPayment { get; set; }

        /// <summary>
        /// Gets or sets the arrears days
        /// </summary>
        public int ArrearsDays { get; set; }

        /// <summary>
        /// Gets or sets the arrears payment
        /// </summary>
        public decimal ArrearsPayment { get; set; }
    }
}