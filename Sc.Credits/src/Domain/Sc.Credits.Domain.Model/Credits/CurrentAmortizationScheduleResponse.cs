using System.Collections.Generic;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The current amortization schedule response
    /// </summary>
    public class CurrentAmortizationScheduleResponse
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
        /// Gets or sets the days since last payment
        /// </summary>
        public int DaysSinceLastPayment { get; set; }

        /// <summary>
        /// Gets or sets the current amortization schedule fees
        /// </summary>
        public List<CurrentAmortizationScheduleFee> CurrentAmortizationScheduleFees { get; set; }

        /// <summary>
        /// Gets or sets the current amortization schedule assurance fees
        /// </summary>
        public List<AmortizationScheduleAssuranceFee> CurrentAmortizationScheduleAssuranceFees { get; set; }
    }
}