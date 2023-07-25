using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The promissory note payment plan entity
    /// </summary>
    public class PromissoryNotePaymentPlan
    {
        /// <summary>
        /// Gets or sets the fee
        /// </summary>
        public int Fee { get; set; }

        /// <summary>
        /// Gets or sets the due date
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Gets or sets the balance
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// Gets or sets the fee value
        /// </summary>
        public decimal FeeValue { get; set; }

        /// <summary>
        /// Gets or sets the credit value payment
        /// </summary>
        public decimal CreditValuePayment { get; set; }

        /// <summary>
        /// Gets or sets the interest value
        /// </summary>
        public decimal InterestValue { get; set; }

        /// <summary>
        /// Gets or sets the assurance fee value
        /// </summary>
        public decimal AssuranceFeeValue { get; set; }

        /// <summary>
        /// Gets or sets the assurance tax value
        /// </summary>
        public decimal AssuranceTaxValue { get; set; }

        /// <summary>
        /// Gets or sets the assurance total fee value
        /// </summary>
        public decimal AssuranceTotalFeeValue { get; set; }

        /// <summary>
        /// Gets or sets the total fee value
        /// </summary>
        public decimal TotalFeeValue { get; set; }
    }
}