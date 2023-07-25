namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// CreditDetail class
    /// </summary>
    public class CreditDetailResponse
    {
        /// <summary>
        /// Gets or sets the down payment
        /// </summary>
        public decimal DownPayment { get; set; }

        /// <summary>
        /// Gets or sets the total fee value
        /// </summary>
        public decimal TotalFeeValue { get; set; }

        /// <summary>
        /// Gets or sets the credit's value
        /// </summary>
        public decimal CreditValue { get; set; }

        /// <summary>
        /// Gets or sets the fees
        /// </summary>
        public int Fees { get; set; }

        /// <summary>
        /// Gets or sets the assurance value
        /// </summary>
        public decimal AssuranceValue { get; set; }

        /// <summary>
        /// Gets or sets the interest rate
        /// </summary>
        public decimal InterestRate { get; set; }

        /// <summary>
        /// Gets or sets the total interest value
        /// </summary>
        public decimal TotalInterestValue { get; set; }

        /// <summary>
        /// Gets or sets the total down payment
        /// </summary>
        public decimal TotalDownPayment { get; set; }

        /// <summary>
        /// Gets or sets the fee credit value
        /// </summary>
        public decimal FeeCreditValue { get; set; }

        /// <summary>
        /// Gets or sets the assurance fee value
        /// </summary>
        public decimal AssuranceFeeValue { get; set; }

        /// <summary>
        /// Gets or sets the assurance total value
        /// </summary>
        public decimal AssuranceTotalValue { get; set; }

        /// <summary>
        /// Gets or sets the assurance tax fee value
        /// </summary>
        public decimal AssuranceTaxFeeValue { get; set; }

        /// <summary>
        /// Gets or sets the assurance tax value
        /// </summary>
        public decimal AssuranceTaxValue { get; set; }

        /// <summary>
        /// Gets or sets the down payment percentage
        /// </summary>
        public decimal DownPaymentPercentage { get; set; }

        /// <summary>
        /// Gets or sets the assurance percentage
        /// </summary>
        public decimal AssurancePercentage { get; set; }

        /// <summary>
        /// Gets or sets the assurance total fee value
        /// </summary>
        public decimal AssuranceTotalFeeValue { get; set; }

        /// <summary>
        /// Gets or sets the total payment value
        /// </summary>
        public decimal TotalPaymentValue { get; set; }

        /// <summary>
        /// Gets or sets the customer allow photo signature
        /// </summary>
        public bool CustomerAllowPhotoSignature { get; set; }
    }
}