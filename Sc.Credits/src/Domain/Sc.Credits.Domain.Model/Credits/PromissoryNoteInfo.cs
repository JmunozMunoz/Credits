using System;
using System.Collections.Generic;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The promissory note info entity
    /// </summary>
    public class PromissoryNoteInfo
    {
        /// <summary>
        /// Gets or sets the credit's value
        /// </summary>
        public decimal CreditValue { get; set; }

        /// <summary>
        /// Gets or sets the customer full name
        /// </summary>
        public string CustomerFullName { get; set; }

        /// <summary>
        /// Gets or sets the document's id
        /// </summary>
        public string CustomerIdDocument { get; set; }

        /// <summary>
        /// Gets or sets the document's type
        /// </summary>
        public string CustomerDocumentType { get; set; }

        /// <summary>
        /// Gets or sets the credit date
        /// </summary>
        public DateTime CreditDate { get; set; }

        /// <summary>
        /// Gets or sets the store's name
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// Gets or sets the store's phone
        /// </summary>
        public string StorePhone { get; set; }

        /// <summary>
        /// Gets or sets the credit number
        /// </summary>
        public long CreditNumber { get; set; }

        /// <summary>
        /// Gets or sets the invoice
        /// </summary>
        public string Invoice { get; set; }

        /// <summary>
        /// Gets or sets the assurance company
        /// </summary>
        public string AssuranceCompany { get; set; }

        /// <summary>
        /// Gets or sets the letters credit value
        /// </summary>
        public string LettersCreditValue { get; set; }

        /// <summary>
        /// Gets or sets the effective annual rate
        /// </summary>
        public decimal EffectiveAnnualRate { get; set; }

        /// <summary>
        /// Gets or sets the interest rate
        /// </summary>
        public decimal InterestRate { get; set; }

        /// <summary>
        /// Gets or sets the down payment
        /// </summary>
        public decimal DownPayment { get; set; }

        /// <summary>
        /// Gets or sets the cell phone number
        /// </summary>
        public string CellPhone { get; set; }

        /// <summary>
        /// Gets or sets the nit
        /// </summary>
        public string Nit { get; set; }

        /// <summary>
        /// Gets or sets the token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the template
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// Gets or sets the payments plans
        /// </summary>
        public List<PromissoryNotePaymentPlan> PaymentPlan { get; set; }
    }
}