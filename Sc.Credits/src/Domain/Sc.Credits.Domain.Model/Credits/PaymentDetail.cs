using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The payment detail entity
    /// </summary>
    public class PaymentDetail
    {
        /// <summary>
        /// Gets or sets the payment value
        /// </summary>
        public decimal PaymentValue { get; set; }

        /// <summary>
        /// Gets or sets the interest value payment
        /// </summary>
        public decimal InterestValuePayment { get; set; }

        /// <summary>
        /// Gets or sets the arrears value payment
        /// </summary>
        public decimal ArrearsValuePayment { get; set; }

        /// <summary>
        /// Gets or sets the assurance value payment
        /// </summary>
        public decimal AssuranceValuePayment { get; set; }

        /// <summary>
        /// Gets or sets the charges value payment
        /// </summary>
        public decimal ChargesValuePayment { get; set; }

        /// <summary>
        /// Gets or sets the credit value payment
        /// </summary>
        public decimal CreditValuePayment { get; set; }

        /// <summary>
        /// Gets or sets the arrears days
        /// </summary>
        public int ArrearsDays { get; set; }

        /// <summary>
        /// Gets or sets the interest rate
        /// </summary>
        public decimal InterestRate { get; set; }

        /// <summary>
        /// Gets or sets the last payment fee
        /// </summary>
        public int LastPaymentFee { get; set; }

        /// <summary>
        /// Gets or sets the next due date
        /// </summary>
        public DateTime NextDueDate { get; set; }

        /// <summary>
        /// Gets or sets the credit paid indicator
        /// </summary>
        public bool CreditPaid { get; set; }

        /// <summary>
        /// Gets or sets the residue
        /// </summary>
        public decimal Residue { get; set; }

        /// <summary>
        /// Gets or sets the previous arrears
        /// </summary>
        public decimal PreviousArrears { get; set; }

        /// <summary>
        /// Gets or sets the previous interest
        /// </summary>
        public decimal PreviousInterest { get; set; }

        /// <summary>
        /// Gets or sets the active fee value paid
        /// </summary>
        public decimal ActiveFeeValuePaid { get; set; }

        /// <summary>
        /// Gets or sets the last payment date
        /// </summary>
        public DateTime LastPaymentDate { get; set; }
    }
}