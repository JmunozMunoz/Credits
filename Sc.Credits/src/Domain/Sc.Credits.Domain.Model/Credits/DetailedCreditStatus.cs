using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The detailed credit status entity
    /// </summary>
    public class DetailedCreditStatus : CreditStatus
    {
        /// <summary>
        /// Gets or sets the down payment value
        /// </summary>
        public decimal DownPayment { get; set; }

        /// <summary>
        /// Gets or sets the assurance value
        /// </summary>
        public decimal AssuranceValue { get; set; }

        /// <summary>
        /// Gets or sets the interest rate
        /// </summary>
        public decimal InterestRate { get; set; }

        /// <summary>
        /// Gets or sets the frequency
        /// </summary>
        public int Frequency { get; set; }

        /// <summary>
        /// Gets or sets the fees
        /// </summary>
        public int Fees { get; set; }

        /// <summary>
        /// Gets or sets the assurance fee value
        /// </summary>
        public decimal AssuranceFeeValue { get; set; }

        /// <summary>
        /// Gets or sets the assurance total fee value
        /// </summary>
        public decimal AssuranceTotalFeeValue { get; set; }

        /// <summary>
        /// Gets or sets the last payment date
        /// </summary>
        public DateTime LastPaymentDate { get; set; }

        /// <summary>
        /// Gets or sets the days since last payment
        /// </summary>
        public int DaysSinceLastPayment { get; set; }

        /// <summary>
        /// Gets or sets the effective annual rate
        /// </summary>
        public decimal EffectiveAnnualRate { get; set; }

        /// <summary>
        /// Gets or sets the assurance percentage
        /// </summary>
        public decimal AssurancePercentage { get; set; }

        /// <summary>
        /// Gets or sets the monthly arrears rate
        /// </summary>
        public decimal MonthlyArrearsRate { get; set; }

        /// <summary>
        /// Gets or sets the asurance balance
        /// </summary>
        public decimal AsuranceBalance { get; set; }

        /// <summary>
        /// Gets or sets the charge value
        /// </summary>
        public decimal ChargeValue { get; set; }

        /// <summary>
        /// Gets or sets the has arrears charge
        /// </summary>
        public bool HasArrearsCharge { get; set; }

        /// <summary>
        /// Gets or sets the arrears charge
        /// </summary>
        public decimal ArrearsCharge { get; set; }

        /// <summary>
        /// Gets or sets the previous interest
        /// </summary>
        public decimal PreviousInterest { get; set; }

        /// <summary>
        /// Gets or sets the previous arrears
        /// </summary>
        public decimal PreviousArrears { get; set; }

        /// <summary>
        /// Gets or sets the current effective annual rate
        /// </summary>
        public decimal CurrentEffectiveAnnualRate { get; set; }

        /// <summary>
        /// Gets or sets the current interest rate
        /// </summary>
        public decimal CurrentInterestRate { get; set; }

        /// <summary>
        /// Gets or sets the total fee value
        /// </summary>
        public decimal TotalFeeValue { get; set; }

        /// <summary>
        /// Gets or sets the updated payment plan value
        /// </summary>
        public decimal UpdatedPaymentPlanValue { get; set; }

        /// <summary>
        /// Gets or sets the arrears fees
        /// </summary>
        public int ArrearsFees { get; set; }

        /// <summary>
        /// Gets or sets the arrears payment
        /// </summary>
        public decimal ArrearsPayment { get; set; }


        public CurrentAmortizationScheduleResponse AmortizationSchedule { get; set; }
    }
}