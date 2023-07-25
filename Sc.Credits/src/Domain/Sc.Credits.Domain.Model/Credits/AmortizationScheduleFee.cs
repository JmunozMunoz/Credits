using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The current amortization schedule fee entity
    /// </summary>
    public class AmortizationScheduleFee
    {
        /// <summary>
        /// Gets or sets the fee
        /// </summary>
        public int Fee { get; set; }

        /// <summary>
        /// Gets or sets the fee date
        /// </summary>
        public DateTime FeeDate { get; set; }

        /// <summary>
        /// Gets or sets the balance
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// Gets or sets the fee value
        /// </summary>
        public decimal FeeValue { get; set; }

        /// <summary>
        /// Gets or sets the interest value
        /// </summary>
        public decimal InterestValue { get; set; }

        /// <summary>
        /// Gets or sets the credit value payment
        /// </summary>
        public decimal CreditValuePayment { get; set; }

        /// <summary>
        /// Gets or sets the final balance
        /// </summary>
        public decimal FinalBalance { get; set; }

        /// <summary>
        /// Gets or sets the total fee value
        /// </summary>
        public decimal TotalFeeValue { get; set; }
    }
}