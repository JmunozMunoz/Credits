using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The current amortization schedule request
    /// </summary>
    public class CurrentAmortizationScheduleRequest
    {
        /// <summary>
        /// Gets or sets the credit value
        /// </summary>
        public decimal CreditValue { get; set; }

        /// <summary>
        /// Gets or sets the initial date
        /// </summary>

        public DateTime InitialDate { get; set; }

        /// <summary>
        /// Gets or sets the fee value
        /// </summary>
        public decimal FeeValue { get; set; }

        /// <summary>
        /// Gets or sets interest rate
        /// </summary>
        public decimal InterestRate { get; set; }

        /// <summary>
        /// Gets or sets frequency
        /// </summary>
        public int Frequency { get; set; }

        /// <summary>
        /// Gets or sets the fees
        /// </summary>
        public int Fees { get; set; }

        /// <summary>
        /// Gets or sets the down payment
        /// </summary>
        public decimal DownPayment { get; set; }

        /// <summary>
        /// Gets or sets the assurance value
        /// </summary>
        public decimal AssuranceValue { get; set; }

        /// <summary>
        /// Gets or sets assurance fee value
        /// </summary>
        public decimal AssuranceFeeValue { get; set; }

        /// <summary>
        /// Gets or sets assurance total fee value
        /// </summary>
        public decimal AssuranceTotalFeeValue { get; set; }

        /// <summary>
        /// Gets or sets last payment date
        /// </summary>
        public DateTime LastPaymentDate { get; set; }

        /// <summary>
        /// Gets or sets balance
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// Gets or sets the assurance balance
        /// </summary>
        public decimal AssuranceBalance { get; set; }

        /// <summary>
        /// Gets or sets the has arrears charge indicator
        /// </summary>
        public bool HasArrearsCharge { get; set; }

        /// <summary>
        /// Gets or sets the arrears charges
        /// </summary>
        public decimal ArrearsCharges { get; set; }

        /// <summary>
        /// Gets or sets the current effective annual rate
        /// </summary>
        public decimal CurrentEffectiveAnnualRate { get; set; }

        /// <summary>
        /// Gets or sets the charge value
        /// </summary>
        public decimal ChargeValue { get; set; }

        /// <summary>
        /// Gets or sets the previous interest
        /// </summary>
        public decimal PreviousInterest { get; set; }

        /// <summary>
        /// Gets or sets the previous arrears
        /// </summary>
        public decimal PreviousArrears { get; set; }
    }
}