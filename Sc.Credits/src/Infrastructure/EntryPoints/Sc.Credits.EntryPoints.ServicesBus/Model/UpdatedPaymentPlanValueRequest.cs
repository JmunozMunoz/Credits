namespace Sc.Credits.EntryPoints.ServicesBus.Model
{
    /// <summary>
    /// Updated payment plan value request
    /// </summary>
    public class UpdatedPaymentPlanValueRequest
    {
        /// <summary>
        /// Gets or sets the credit id
        /// </summary>
        public string CreditId { get; set; }

        /// <summary>
        /// Gets or sets the updated payment plan value
        /// </summary>
        public decimal? UpdatedPaymentPlanValue { get; set; }
    }
}