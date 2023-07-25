using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The current payment schedule request
    /// </summary>
    public class CurrentPaymentScheduleRequest : CurrentAmortizationScheduleRequest
    {
        /// <summary>
        /// Gets or sets the updated payment plan value
        /// </summary>
        public decimal UpdatedPaymentPlanValue { get; set; }
    }
}