using Newtonsoft.Json;
using Sc.Credits.Domain.Model.Credits;
using System;
using System.Collections.Generic;

namespace Sc.Credits.Domain.Model.Refinancings
{
    /// <summary>
    /// Create credit response
    /// </summary>
    public class RefinancingCreditResponse
    {
        /// <summary>
        /// Credit id
        /// </summary>
        public Guid CreditId { get; set; }

        /// <summary>
        /// Credit number
        /// </summary>
        public long CreditNumber { get; set; }

        /// <summary>
        /// Credit value
        /// </summary>
        public decimal CreditValue { get; set; }

        /// <summary>
        /// Fees
        /// </summary>
        public int Fees { get; set; }

        /// <summary>
        /// Down payment percentaje
        /// </summary>
        public decimal DownPaymentPercentage { get; set; }

        /// <summary>
        /// Down payment
        /// </summary>
        public decimal DownPayment { get; set; }

        /// <summary>
        /// Total down payment
        /// </summary>
        public decimal TotalDownPayment { get; set; }

        /// <summary>
        /// Assurance percentage
        /// </summary>
        public decimal AssurancePercentage { get; set; }

        /// <summary>
        /// Assurance value
        /// </summary>
        public decimal AssuranceValue { get; set; }

        /// <summary>
        /// Assurance tax value
        /// </summary>
        public decimal AssuranceTaxValue { get; set; }

        /// <summary>
        /// Assurance total value
        /// </summary>
        public decimal AssuranceTotalValue { get; set; }

        /// <summary>
        /// Assurance fee value
        /// </summary>
        public decimal AssuranceFeeValue { get; set; }

        /// <summary>
        /// Assurance tax fee value
        /// </summary>
        public decimal AssuranceTaxFeeValue { get; set; }

        /// <summary>
        /// Assurance total fee value
        /// </summary>
        public decimal AssuranceTotalFeeValue { get; set; }

        /// <summary>
        /// Interest rate
        /// </summary>
        public decimal InterestRate { get; set; }

        /// <summary>
        /// Total interest value
        /// </summary>
        public decimal TotalInterestValue { get; set; }

        /// <summary>
        /// Fee credit value
        /// </summary>
        public decimal FeeCreditValue { get; set; }

        /// <summary>
        /// Total fee value
        /// </summary>
        public decimal TotalFeeValue { get; set; }

        /// <summary>
        /// Payment credit responses
        /// </summary>
        [JsonIgnore]
        public List<PaymentCreditResponse> PaymentCreditResponses { get; set; }

        /// <summary>
        /// Create credit response
        /// </summary>
        [JsonIgnore]
        public CreateCreditResponse CreateCreditResponse { get; set; }

        /// <summary>
        /// From create credit
        /// </summary>
        /// <param name="createCreditResponse"></param>
        /// <param name="paymentCreditResponses"></param>
        /// <returns></returns>
        public static RefinancingCreditResponse FromCreateCredit(CreateCreditResponse createCreditResponse,
            List<PaymentCreditResponse> paymentCreditResponses) =>
            new RefinancingCreditResponse
            {
                AssuranceFeeValue = createCreditResponse.AssuranceFeeValue,
                AssurancePercentage = createCreditResponse.AssurancePercentage,
                AssuranceTaxFeeValue = createCreditResponse.AssuranceTaxFeeValue,
                AssuranceTaxValue = createCreditResponse.AssuranceTaxValue,
                AssuranceTotalFeeValue = createCreditResponse.AssuranceTotalFeeValue,
                AssuranceTotalValue = createCreditResponse.AssuranceTotalValue,
                AssuranceValue = createCreditResponse.AssuranceValue,
                CreditId = createCreditResponse.CreditId,
                CreditNumber = createCreditResponse.CreditNumber,
                CreditValue = createCreditResponse.CreditValue,
                DownPayment = createCreditResponse.DownPayment,
                DownPaymentPercentage = createCreditResponse.DownPaymentPercentage,
                FeeCreditValue = createCreditResponse.FeeCreditValue,
                Fees = createCreditResponse.Fees,
                InterestRate = createCreditResponse.InterestRate,
                TotalDownPayment = createCreditResponse.TotalDownPayment,
                TotalFeeValue = createCreditResponse.TotalFeeValue,
                TotalInterestValue = createCreditResponse.TotalInterestValue,
                CreateCreditResponse = createCreditResponse,
                PaymentCreditResponses = paymentCreditResponses
            };
    }
}