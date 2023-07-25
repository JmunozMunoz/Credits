using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// Amortization schedule assurance fee entity
    /// </summary>
    public class AmortizationScheduleAssuranceFee
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
        /// Gets or sets the assurance fee value
        /// </summary>
        public decimal AssuranceFeeValue { get; set; }

        /// <summary>
        /// Gets or sets the assurance tax value
        /// </summary>
        public decimal AssuranceTaxValue { get; set; }

        /// <summary>
        /// Gets or sets the assurance payment value
        /// </summary>
        public decimal AssurancePaymentValue { get; set; }

        /// <summary>
        /// Gets or sets the final balance
        /// </summary>
        public decimal FinalBalance { get; set; }
    }
}