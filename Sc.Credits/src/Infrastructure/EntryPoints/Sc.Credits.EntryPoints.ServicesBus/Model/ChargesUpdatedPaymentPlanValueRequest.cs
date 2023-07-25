namespace Sc.Credits.EntryPoints.ServicesBus.Model
{
    /// <summary>
    /// Charges updated payment plan value request
    /// </summary>
    public class ChargesUpdatedPaymentPlanValueRequest
    {
        /// <summary>
        /// Gets or sets the credit id
        /// </summary>
        public string CreditId { get; set; }

        /// <summary>
        /// Gets or sets the has arrears charge
        /// </summary>

        public bool HasArrearsCharge { get; set; }

        /// <summary>
        /// Gets or sets the arrears charge
        /// </summary>
        public decimal? ArrearsCharge { get; set; }

        /// <summary>
        /// Gets or sets the charge value
        /// </summary>
        public decimal? ChargeValue { get; set; }

        /// <summary>
        /// Gets or sets the updated payment plan value
        /// </summary>
        public decimal? UpdatedPaymentPlanValue { get; set; }
    }
}