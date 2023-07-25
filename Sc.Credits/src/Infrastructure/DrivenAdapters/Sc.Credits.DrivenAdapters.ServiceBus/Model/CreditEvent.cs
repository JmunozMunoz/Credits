using Newtonsoft.Json;
using System;

namespace Sc.Credits.DrivenAdapters.ServiceBus.Model
{
    /// <summary>
    /// Credit event
    /// </summary>
    public class CreditEvent
    {
        /// <summary>
        /// Gets or sets the Credit master id
        /// </summary>
        [JsonProperty("CreditId")]
        public Guid CreditMasterId { get; set; }

        /// <summary>
        /// Gets or sets the transaction id
        /// </summary>

        public Guid TransactionId { get; set; }

        /// <summary>
        /// Gets or sets the credit value
        /// </summary>
        public decimal CreditValue { get; set; }

        /// <summary>
        /// Gets or sets the fees
        /// </summary>
        public int Fees { get; set; }

        /// <summary>
        /// Gets or sets the fee value
        /// </summary>
        public decimal FeeValue { get; set; }

        /// <summary>
        /// Gets or sets the assurance percentage
        /// </summary>
        public decimal AssurancePercentage { get; set; }

        /// <summary>
        /// Gets or sets the assurance value
        /// </summary>
        public decimal AssuranceValue { get; set; }

        /// <summary>
        /// Gets or sets the assurance fee
        /// </summary>
        public decimal AssuranceFee { get; set; }

        /// <summary>
        /// Gets or sets the interest rate
        /// </summary>
        public decimal InterestRate { get; set; }

        /// <summary>
        /// Gets or sets the frequency
        /// </summary>
        public int Frequency { get; set; }

        /// <summary>
        /// Gets or sets the balance
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// Gets or sets the last fee
        /// </summary>
        public int LastFee { get; set; }

        /// <summary>
        /// Gets or sets the create date
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the create time
        /// </summary>
        public TimeSpan CreateTime { get; set; }

        /// <summary>
        /// Gets or sets the credit date
        /// </summary>
        public DateTime CreditDate { get; set; }

        /// <summary>
        /// Gets or sets the credit time
        /// </summary>
        public TimeSpan CreditTime { get; set; }

        /// <summary>
        /// Gets or sets the transaction date
        /// </summary>
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// Gets or sets the transaction time
        /// </summary>
        public TimeSpan TransactionTime { get; set; }

        /// <summary>
        /// Gets or sets the products
        /// </summary>
        public string Products { get; set; }

        /// <summary>
        /// Gets or sets the user id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the store id
        /// </summary>
        public string StoreId { get; set; }

        /// <summary>
        /// Gets or sets the customer id
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the customer document type
        /// </summary>
        public string CustomerDocumentType { get; set; }

        /// <summary>
        /// Gets or sets the customer document id
        /// </summary>
        public string CustomerDocumentId { get; set; }

        /// <summary>
        /// Gets or sets the available credit limit
        /// </summary>
        public decimal AvailableCreditLimit { get; set; }

        /// <summary>
        /// Gets or sets the status id
        /// </summary>
        public int StatusId { get; set; }

        /// <summary>
        /// Gets or sets the status name
        /// </summary>
        public string StatusName { get; set; }

        /// <summary>
        /// Gets or sets the source id
        /// </summary>
        public int SourceId { get; set; }

        /// <summary>
        /// Gets or sets the source name
        /// </summary>
        public string SourceName { get; set; }

        /// <summary>
        /// Gets or sets the authmethod id
        /// </summary>
        public int AuthMethodId { get; set; }

        /// <summary>
        /// Gets or sets the authmethod name
        /// </summary>
        public string AuthMethodName { get; set; }

        /// <summary>
        /// Gets or sets the location
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the credit number
        /// </summary>
        public long CreditNumber { get; set; }

        /// <summary>
        /// Gets or sets the alternate payment
        /// </summary>
        public bool AlternatePayment { get; set; }

        /// <summary>
        /// Gets or sets the has arrears charge
        /// </summary>
        public bool HasArrearsCharge { get; set; }

        /// <summary>
        /// Gets or sets the arrears charge
        /// </summary>
        public decimal? ArrearsCharge { get; set; }

        /// <summary>
        /// Gets or sets the charge value
        /// </summary>
        public decimal? ChargeValue { get; set; }

        /// <summary>
        /// Gets or sets the updated payment plan value
        /// </summary>
        public decimal? UpdatedPaymentPlanValue { get; set; }

        /// <summary>
        /// Gets or sets the updated payment plan balance
        /// </summary>
        public decimal? UpdatedPaymentPlanBalance { get; set; }

        /// <summary>
        /// Gets or sets the has updated payment plan
        /// </summary>
        public bool HasUpdatedPaymentPlan { get; set; }

        /// <summary>
        /// Gets or sets the computed arrears days
        /// </summary>
        public int ComputedArrearsDays { get; set; }

        /// <summary>
        /// Gets or sets the arrears days
        /// </summary>
        public int? ArrearsDays { get; set; }

        /// <summary>
        /// Gets or sets the assurance balance
        /// </summary>
        public decimal AssuranceBalance { get; set; }

        /// <summary>
        /// Gets or sets the assurance total value
        /// </summary>
        public decimal AssuranceTotalValue { get; set; }

        /// <summary>
        /// Gets or sets the sccode
        /// </summary>
        public string ScCode { get; set; }

        /// <summary>
        /// Gets or sets the seller
        /// </summary>
        public string Seller { get; set; }

        /// <summary>
        /// Gets or sets the invoice
        /// </summary>
        public string Invoice { get; set; }

        /// <summary>
        /// Gets or sets the reason
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets the certifying authority
        /// </summary>
        public string CertifyingAuthority { get; set; }

        /// <summary>
        /// Gets or sets the certified id
        /// </summary>
        public string CertifiedId { get; set; }

        /// <summary>
        /// Gets or sets the effective annual rate
        /// </summary>
        public decimal EffectiveAnnualRate { get; set; }

        /// <summary>
        /// Gets or sets the total value paid
        /// </summary>
        public decimal TotalValuePaid { get; set; }

        /// <summary>
        /// Gets or sets the fee
        /// </summary>
        public int Fee { get; set; }

        /// <summary>
        /// Gets or sets the transaction type id
        /// </summary>
        public int TransactionTypeId { get; set; }

        /// <summary>
        /// Gets or sets the transaction type name
        /// </summary>
        public string TransactionTypeName { get; set; }

        /// <summary>
        /// Gets or sets the credit value paid
        /// </summary>
        public decimal CreditValuePaid { get; set; }

        /// <summary>
        /// Gets or sets the interest value paid
        /// </summary>
        public decimal InterestValuePaid { get; set; }

        /// <summary>
        /// Gets or sets the charge value paid
        /// </summary>
        public decimal ChargeValuePaid { get; set; }

        /// <summary>
        /// Gets or sets the arrears value paid
        /// </summary>
        public decimal ArrearsValuePaid { get; set; }

        /// <summary>
        /// Gets or sets the assurance value paid
        /// </summary>
        public decimal AssuranceValuePaid { get; set; }

        /// <summary>
        /// Gets or sets the bank account
        /// </summary>
        public string BankAccount { get; set; }

        /// <summary>
        /// Gets or sets the last payment date
        /// </summary>
        public DateTime LastPaymentDate { get; set; }

        /// <summary>
        /// Gets or sets the last payment time
        /// </summary>
        public TimeSpan LastPaymentTime { get; set; }

        /// <summary>
        /// Gets or sets the due date
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Gets or sets the calculation date
        /// </summary>
        public DateTime CalculationDate { get; set; }

        /// <summary>
        /// Gets or sets the payment type id
        /// </summary>
        public int PaymentTypeId { get; set; }

        /// <summary>
        /// Gets or sets the payment type name
        /// </summary>
        public string PaymentTypeName { get; set; }

        /// <summary>
        /// Gets or sets the payment number
        /// </summary>
        public long PaymentNumber { get; set; }

        /// <summary>
        /// Gets or sets the down payment
        /// </summary>
        public decimal DownPayment { get; set; }

        /// <summary>
        /// Gets or sets the total down payment
        /// </summary>
        public decimal TotalDownPayment { get; set; }

        /// <summary>
        /// Gets or sets the assurance total fee value
        /// </summary>
        public decimal AssuranceTotalFeeValue { get; set; }

        /// <summary>
        /// Gets or sets the store name
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// Gets or sets the promissory note file name
        /// </summary>
        public string PromissoryNoteFileName { get; set; }

        /// <summary>
        /// Gets or sets the username
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the previous interest
        /// </summary>
        public decimal PreviousInterest { get; set; }

        /// <summary>
        /// Gets or sets the previous arrears
        /// </summary>
        public decimal PreviousArrears { get; set; }

        /// <summary>
        /// Gets or sets the risk level
        /// </summary>
        public string RiskLevel { get; set; }

        /// <summary>
        /// Gets or sets the business group name
        /// </summary>
        public string BusinessGroupName;

        /// <summary>
        /// Gets or sets the partner nit
        /// </summary>
        public string Nit;

        /// <summary>
        /// Gets or sets the source store
        /// </summary>
        public string SourceCreditStore;

        /// <summary>
        /// Gets or sets the payment store
        /// </summary>
        public string TransactionStore;

        /// <summary>
        /// Gets the id last payment cancelled
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Guid IdLastPaymentCancelled { get; set; }

        /// <summary>
        /// Gets or sets the vendor id
        /// </summary>
        public string VendorId { get; set; }
    }
}