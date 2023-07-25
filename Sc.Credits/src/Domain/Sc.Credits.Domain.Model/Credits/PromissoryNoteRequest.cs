using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Helpers.ObjectsUtils;
using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The promissory note request entity
    /// </summary>
    public class PromissoryNoteRequest
    {

        /// <summary>
        /// Gets or sets customer full name.
        /// </summary>
        public string CustomerFullName {get;set; }

        /// <summary>
        /// Gets or sets type document.
        /// </summary>
        public string TypeDocument {get;set; }

        /// <summary>
        /// Gets or sets id document.
        /// </summary>
        public string IdDocument {get;set; }

        /// <summary>
        /// Gets or sets store name.
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// Gets or sets store phone.
        /// </summary>
        public string StorePhone { get; set; }

        /// <summary>
        /// Gets or sets assurance company.
        /// </summary>
        public string AssuranceCompany { get; set; }

        /// <summary>
        /// Gets or sets credit details.
        /// </summary>
        public CreditDetailResponse CreditDetails {get;set; }

        /// <summary>
        /// Gets or sets credit value
        /// </summary>
        public string CreditValue { get; set; }

        /// <summary>
        /// Gets or sets total payment.
        /// </summary>
        public string TotalPayment { get; set; }

        /// <summary>
        /// Gets or sets interest rate.
        /// </summary>
        public string InterestRate { get; set; }

        /// <summary>
        /// Gets or sets assurance percentage
        /// </summary>
        public string AssurancePercentage { get; set; }

        /// <summary>
        /// Gets or sets assurance value
        /// </summary>
        public string AssuranceValue { get; set; }

        /// <summary>
        /// Gets or sets assurance tax value
        /// </summary>
        public string AssuranceTaxValue { get; set; }

        /// <summary>
        /// Gets or sets letters total payment
        /// </summary>
        public string LettersTotalPayment { get; set; }

        /// <summary>
        /// Gets or sets amortization schedule fees.
        /// </summary>
        public string AmortizationScheduleFees { get; set; }

        /// <summary>
        /// Gets or sets credit master.
        /// </summary>
        public CreditMaster CreditMaster {get;set; }

        /// <summary>
        /// Gets or sets create date.
        /// </summary>
        public string CreateDate { get; set; }

        /// <summary>
        /// Gets or sets last fee date.
        /// </summary>
        public string LastFeeDate { get; set; }

        /// <summary>
        /// Gets or sets credit number.
        /// </summary>
        public string CreditNumber { get; set; }

        /// <summary>
        /// Gets or sets invoice.
        /// </summary>
        public string Invoice { get; set; }

        /// <summary>
        /// Gets or sets store city.
        /// </summary>
        public string StoreCity { get; set; }


        /// <summary>
        /// Gets or sets file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets frequency.
        /// </summary>
        public int Frequency {get;set; }

        /// <summary>
        /// Gets or sets effective annual rate.
        /// </summary>
        public decimal EffectiveAnnualRate {get;set; }

        /// <summary>
        /// Gets or sets arrears effective annual rate.
        /// </summary>
        public decimal ArrearsEffectiveAnnualRate {get;set; }

        /// <summary>
        /// Gets or sets token.
        /// </summary>
        public string Token {get;set; }

        /// <summary>
        /// Gets or sets decimal numbers round.
        /// </summary>
        public int DecimalNumbersRound {get;set; }

        /// <summary>
        /// Gets or sets cell phone.
        /// </summary>
        public string CellPhone {get;set; }

        /// <summary>
        /// Gets or sets nit.
        /// </summary>
        public string Nit { get; set; }

        /// <summary>
        /// Gets or sets company name.
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets government effective annual rate.
        /// </summary>
        public decimal GovernmentEffectiveAnnualRate { get; set; }

        /// <summary>
        /// Gets or sets assurance tax.
        /// </summary>
        public decimal AssuranceTax { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="PromissoryNoteRequest"/>
        /// </summary>
        public PromissoryNoteRequest(CreditDetailResponse creditDetails, CreditMaster creditMaster,string companyName,
            int frequency, decimal effectiveAnnualRate, decimal arrearsEffectiveAnnualRate, int decimalNumbersRoundPercentage, 
            int decimalNumbersRound, string cellPhone, string nit, decimal governmentEffectiveAnnualRate, decimal assuranceTax)
        {



            CustomerFullName = creditMaster.Customer.GetFullName;
            IdDocument = creditMaster.Customer.IdDocument;
            TypeDocument = creditMaster.Customer.DocumentType;


            StoreName = creditMaster.Store.StoreName;
            StorePhone = creditMaster.Store.GetPhone;
            AssuranceCompany = creditMaster.Store.AssuranceCompany?.Name;

            CreditDetails = creditDetails;

            CreditValue = NumberFormat.Currency(creditDetails.CreditValue, decimalNumbersRound);
            TotalPayment = NumberFormat.Currency(creditDetails.TotalPaymentValue, decimalNumbersRound);
            InterestRate = Math.Round((creditDetails.InterestRate * 100),decimalNumbersRoundPercentage).ToString();
            AssurancePercentage = Math.Round((creditDetails.AssurancePercentage * 100), decimalNumbersRoundPercentage).ToString();
            AssuranceValue = NumberFormat.Currency(creditDetails.AssuranceValue, decimalNumbersRound);
            AssuranceTaxValue = NumberFormat.Currency(creditDetails.AssuranceTaxValue, decimalNumbersRound);
            LettersTotalPayment = $"{NumberConversions.ToLettersSpanish(creditDetails.TotalPaymentValue.ToString())} Pesos";

            CreditMaster = creditMaster;
            CellPhone = cellPhone;
            CompanyName = companyName;
            CreateDate = creditMaster.GetCreditDate.ToShortDateString();

            CreditNumber = creditMaster.GetCreditNumber.ToString();
            Invoice = creditMaster.GetCreditInvoice;
            StoreCity = creditMaster.Store.City.Name;
            FileName = $"{creditMaster.Customer.IdDocument}-{creditMaster.GetCreditDate:yyyyMMdd}{creditMaster.CreateTime:hhmm}";

            Frequency = frequency;
            EffectiveAnnualRate = Math.Round((effectiveAnnualRate * 100), decimalNumbersRoundPercentage);
            ArrearsEffectiveAnnualRate = Math.Round((arrearsEffectiveAnnualRate * 100), decimalNumbersRoundPercentage);
            Token = creditMaster.GetToken;
            DecimalNumbersRound = decimalNumbersRound;
            Nit = nit;
            GovernmentEffectiveAnnualRate = Math.Round((governmentEffectiveAnnualRate*100),decimalNumbersRoundPercentage);
            AssuranceTax = Math.Round((assuranceTax * 100), decimalNumbersRoundPercentage);
        }
    }
}
