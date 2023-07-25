using Newtonsoft.Json;
using Sc.Credits.Domain.Model.Stores;
using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The payment credit response entity
    /// </summary>
    public class PaymentCreditResponse
    {
        /// <summary>
        /// Gets or sets the document's type
        /// </summary>
        public string TypeDocument { get; set; }

        /// <summary>
        /// Gets or sets the document's id
        /// </summary>
        public string IdDocument { get; set; }

        /// <summary>
        /// Gets or sets the credit's id
        /// </summary>
        public Guid CreditId { get; set; }

        /// <summary>
        /// Gets or sets the payment id
        /// </summary>
        public Guid PaymentId { get; set; }

        /// <summary>
        /// Gets or sets the payment number
        /// </summary>
        public long PaymentNumber { get; set; }

        /// <summary>
        /// Gets or sets the credit value paid
        /// </summary>
        public decimal CreditValuePaid { get; set; }

        /// <summary>
        /// Gets or sets the interest value paid
        /// </summary>
        public decimal InterestValuePaid { get; set; }

        /// <summary>
        /// Gets or sets the arrears value paid
        /// </summary>
        public decimal ArrearsValuePaid { get; set; }

        /// <summary>
        /// Gets or sets the assurance value paid
        /// </summary>
        public decimal AssuranceValuePaid { get; set; }

        /// <summary>
        /// Gets or sets the charge value paid
        /// </summary>
        public decimal ChargeValuePaid { get; set; }

        /// <summary>
        /// Gets or sets the balance
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// Gets or sets the next due date
        /// </summary>
        public DateTime? NextDueDate { get; set; }

        /// <summary>
        /// Gets or sets the next minimum payment
        /// </summary>
        public decimal NextMinimumPayment { get; set; }

        /// <summary>
        /// Gets or sets the paid credit
        /// </summary>
        public bool PaidCredit { get; set; }

        /// <summary>
        /// Gets or sets the has charges indicator
        /// </summary>
        public bool HasCharges { get; set; }

        /// <summary>
        /// Gets or sets the total value paid
        /// </summary>
        [JsonIgnore]
        public decimal TotalValuePaid { get; set; }

        /// <summary>
        /// Gets or sets the balance refinanced
        /// </summary>
        [JsonIgnore]
        public decimal BalanceRefinanced { get; set; }

        /// <summary>
        /// Gets or sets the credit master
        /// </summary>

        [JsonIgnore]
        public CreditMaster CreditMaster { get; set; }

        /// <summary>
        /// Gets or sets the credit
        /// </summary>
        [JsonIgnore]
        public Credit Credit { get; set; }

        /// <summary>
        /// Gets or sets the store
        /// </summary>
        [JsonIgnore]
        public Store Store { get; set; }

    }
}