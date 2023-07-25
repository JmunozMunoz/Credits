using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// Amortization schedule request
    /// </summary>
    public class AmortizationScheduleRequest
    {
        /// <summary>
        /// Gets or sets the credit value
        /// </summary>
        public decimal CreditValue { get; set; }

        /// <summary>
        /// Gets or sets the credit's initial date
        /// </summary>
        public DateTime InitialDate { get; set; }

        /// <summary>
        /// Gets or sets the fee value
        /// </summary>
        public decimal FeeValue { get; set; }

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
        /// Gets or sets the down payment
        /// </summary>
        public decimal DownPayment { get; set; }

        /// <summary>
        /// Gets or sets the assurance value
        /// </summary>
        public decimal AssuranceValue { get; set; }

        /// <summary>
        /// Gets or sets the assurance fee value
        /// </summary>
        public decimal AssuranceFeeValue { get; set; }

        /// <summary>
        /// Gets or sets the assurance total fee value
        /// </summary>
        public decimal AssuranceTotalFeeValue { get; set; }

        /// <summary>
        /// Create new amortization schedule response from credit an specific date
        /// </summary>
        /// <param name="credit"></param>
        /// <param name="initialDate"></param>
        /// <param name="interestRate"></param>
        /// <param name="downPayment"></param>
        /// <returns></returns>
        public static AmortizationScheduleRequest FromCredit(Credit credit, DateTime initialDate, decimal? interestRate = null, decimal? downPayment = null) =>
            new AmortizationScheduleRequest
            {
                CreditValue = credit.GetCreditValue,
                InitialDate = initialDate,
                FeeValue = credit.GetFeeValue,
                InterestRate = interestRate ?? credit.GetInterestRate,
                Frequency = credit.GetFrequency,
                Fees = credit.GetFees,
                DownPayment = downPayment ?? credit.GetDownPayment,
                AssuranceValue = credit.GetAssuranceValue,
                AssuranceFeeValue = credit.GetAssuranceFee,
                AssuranceTotalFeeValue = credit.GetAssuranceTotalFeeValue
            };

        /// <summary>
        /// Create new amortization schedule response from current
        /// </summary>
        /// <param name="currentAmortizationScheduleRequest"></param>
        /// <returns></returns>
        public static AmortizationScheduleRequest FromCurrent(CurrentAmortizationScheduleRequest currentAmortizationScheduleRequest) =>
             new AmortizationScheduleRequest
             {
                 CreditValue = currentAmortizationScheduleRequest.CreditValue,
                 InitialDate = currentAmortizationScheduleRequest.InitialDate,
                 FeeValue = currentAmortizationScheduleRequest.FeeValue,
                 InterestRate = currentAmortizationScheduleRequest.InterestRate,
                 Frequency = currentAmortizationScheduleRequest.Frequency,
                 Fees = currentAmortizationScheduleRequest.Fees,
                 DownPayment = currentAmortizationScheduleRequest.DownPayment,
                 AssuranceValue = currentAmortizationScheduleRequest.AssuranceValue,
                 AssuranceFeeValue = currentAmortizationScheduleRequest.AssuranceFeeValue,
                 AssuranceTotalFeeValue = currentAmortizationScheduleRequest.AssuranceTotalFeeValue
             };
    }
}