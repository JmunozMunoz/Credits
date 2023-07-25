namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The payment alternatives entity
    /// </summary>
    public class PaymentAlternatives
    {
        /// <summary>
        /// Gets or sets the minimum payment
        /// </summary>
        public decimal MinimumPayment { get; set; }

        /// <summary>
        /// Gets or sets the total payment
        /// </summary>
        public decimal TotalPayment { get; set; }

        /// <summary>
        /// Gets or sets the payment fees
        /// </summary>
        public PaymentFeesResponse PaymentFees { get; set; }
    }
}