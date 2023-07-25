using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// Payment template response
    /// </summary>
    public class PaymentTemplateResponse
    {
        /// <summary>
        /// Generation date
        /// </summary>
        public DateTime GenerationDate { get; set; }

        /// <summary>
        /// Credit value paid
        /// </summary>
        public decimal CreditValuePaid { get; set; }

        /// <summary>
        /// Total value paid
        /// </summary>
        public decimal TotalValuePaid { get; set; }

        /// <summary>
        /// Cell phone
        /// </summary>
        public string CellPhone { get; set; }

        /// <summary>
        /// Nit
        /// </summary>
        public string Nit { get; set; }

        /// <summary>
        /// Customer full name
        /// </summary>
        public string CustomerFullName { get; set; }

        /// <summary>
        /// Customer id document
        /// </summary>
        public string CustomerIdDocument { get; set; }

        /// <summary>
        /// Customer document type
        /// </summary>
        public string CustomerDocumentType { get; set; }

        /// <summary>
        /// Payment date
        /// </summary>
        public DateTime PaymentDate { get; set; }

        /// <summary>
        /// Store payment name
        /// </summary>
        public string StorePaymentName { get; set; }

        /// <summary>
        /// Store credit name
        /// </summary>
        public string StoreCreditName { get; set; }

        /// <summary>
        /// Store phone
        /// </summary>
        public string StorePhone { get; set; }

        /// <summary>
        /// Payment number
        /// </summary>
        public long PaymentNumber { get; set; }

        /// <summary>
        /// Credit number
        /// </summary>
        public long CreditNumber { get; set; }

        /// <summary>
        /// Interest value paid
        /// </summary>
        public decimal InterestValuePaid { get; set; }

        /// <summary>
        /// Arrears value paid
        /// </summary>
        public decimal ArrearsValuePaid { get; set; }

        /// <summary>
        /// Assurance value paid
        /// </summary>
        public decimal AssuranceValuePaid { get; set; }

        /// <summary>
        /// Charge value paid
        /// </summary>
        public decimal ChargeValuePaid { get; set; }

        /// <summary>
        /// Assurance tax
        /// </summary>
        public decimal AssuranceTax { get; set; }

        /// <summary>
        /// Balance
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// Total balance
        /// </summary>
        public decimal TotalBalance { get; set; }

        /// <summary>
        /// Next due date
        /// </summary>
        public DateTime? NextDueDate { get; set; }

        /// <summary>
        /// Available credit limit
        /// </summary>
        public decimal AvailableCreditLimit { get; set; }

        /// <summary>
        /// Template
        /// </summary>
        public string Template { get; set; }
    }
}