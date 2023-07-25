using System.Collections.Generic;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The current payment schedule response
    /// </summary>
    public class CurrentPaymentScheduleResponse
    {
        /// <summary>
        /// Gets or sets the
        /// </summary>
        public int DaysSinceLastPayment { get; set; }

        /// <summary>
        /// Gets or sets the
        /// </summary>
        public int PendingFees { get; set; }

        /// <summary>
        /// Gets or sets the payment fees
        /// </summary>
        public List<PaymentFee> PaymentFees { get; set; }

        /// <summary>
        /// Gets or sets the
        /// </summary>
        public decimal MinimumPayment { get; set; }

        /// <summary>
        /// Gets or sets the
        /// </summary>
        public decimal TotalPayment { get; set; }

        /// <summary>
        /// Gets or sets the payment credit schedule fees
        /// </summary>
        public List<CurrentAmortizationScheduleFee> PaymentCreditScheduleFees { get; set; }

        /// <summary>
        /// Gets or sets the payment assurance schedule fees
        /// </summary>
        public List<AmortizationScheduleAssuranceFee> PaymentAssuranceScheduleFees { get; set; }
    }
}