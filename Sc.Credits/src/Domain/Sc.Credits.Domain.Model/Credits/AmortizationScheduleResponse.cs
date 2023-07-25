using System.Collections.Generic;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The amortization schedule response entity
    /// </summary>
    public class AmortizationScheduleResponse
    {
        /// <summary>
        /// Gets or sets the credit value
        /// </summary>
        public decimal CreditValue { get; set; }

        /// <summary>
        /// Gets or sets the down payment
        /// </summary>
        public decimal DownPayment { get; set; }

        /// <summary>
        /// Gets or sets the assurance value
        /// </summary>
        public decimal AssuranceValue { get; set; }

        /// <summary>
        /// Gets or sets the amortization schedule fees
        /// </summary>
        public List<AmortizationScheduleFee> AmortizationScheduleFees { get; set; }

        /// <summary>
        /// Gets or sets the
        /// </summary>
        public List<AmortizationScheduleAssuranceFee> AmortizationScheduleAssuranceFees { get; set; }
    }
}