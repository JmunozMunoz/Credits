using Newtonsoft.Json;
using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The Credit Status Entity
    /// </summary>
    public class CreditStatus
    {
        /// <summary>
        /// Gets or sets the type document
        /// </summary>
        public string TypeDocument { get; set; }

        /// <summary>
        /// Gets or sets the IdDocument
        /// </summary>
        public string IdDocument { get; set; }

        /// <summary>
        /// Gets or sets the CustomerFullName
        /// </summary>
        [JsonIgnore]
        public string CustomerFullName { get; set; }

        /// <summary>
        /// Gets or sets the credit's Id
        /// </summary>
        public Guid CreditId { get; set; }

        /// <summary>
        /// Gets or sets the credit number
        /// </summary>
        public long CreditNumber { get; set; }

        /// <summary>
        /// Gets or sets the store's Id
        /// </summary>
        public string StoreId { get; set; }

        /// <summary>
        /// Gets or sets the credit's date of creation
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the credit's value
        /// </summary>
        public decimal CreditValue { get; set; }

        /// <summary>
        /// Gets or sets the arrears days
        /// </summary>
        public long ArrearsDays { get; set; }

        /// <summary>
        /// Gets or sets the minimum payment
        /// </summary>
        public decimal MinimumPayment { get; set; }

        /// <summary>
        /// Gets or sets the is next fee indicator
        /// </summary>
        public bool IsNextFee { get; set; }

        /// <summary>
        /// Gets or sets the total payment
        /// </summary>
        public decimal TotalPayment { get; set; }

        /// <summary>
        /// Gets or sets the fee value
        /// </summary>
        public decimal FeeValue { get; set; }

        /// <summary>
        /// Gets or sets the store's name
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// Gets or sets the current balance
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// Gets or sets the due date
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Gets or sets the updated payment plan value
        /// </summary>
        public bool UpdatedPaymentPlan { get; set; }

        /// <summary>
        /// Gets or sets the maximum residue value
        /// </summary>
        public decimal MaximumResidueValue { get; set; }

        /// <summary>
        /// Gets or sets the mobile
        /// </summary>
        [JsonIgnore]
        public string Mobile { get; set; } 

        /// <summary>
        /// Gets or sets the email
        /// </summary>
        [JsonIgnore]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the customer first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the customer second name
        /// </summary>
        public string SecondName { get; set; }
    }
}