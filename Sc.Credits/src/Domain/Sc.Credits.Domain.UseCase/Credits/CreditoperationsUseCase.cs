using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Helpers.Commons.Extensions;
using System;

namespace Sc.Credits.Domain.UseCase.Credits
{
    public class CreditOperationsUseCase : ICreditOperationsUseCase
    {
        /// <summary>
        /// Creates the credit details.
        /// </summary>
        /// <param name="generalCreditDetailDomainRequest">The detail request.</param>
        /// <returns>Object that contains the credit operations needed</returns>
        public CreditDetailResponse CreateCreditDetails(GeneralCreditDetailDomainRequest generalCreditDetailDomainRequest)
        {
            bool hasDownPayment = generalCreditDetailDomainRequest.DownPayment > 0;

            decimal assuranceValue = generalCreditDetailDomainRequest.HaveAssurance
                ?
                CalculateAssuranceValue(generalCreditDetailDomainRequest.CreditValue, generalCreditDetailDomainRequest.Store.GetAssurancePercentage)
                : 0;

            decimal assuranceTaxValue = generalCreditDetailDomainRequest.HaveAssuranceTax
                ?
                GetTaxValue(assuranceValue, generalCreditDetailDomainRequest.AssuranceTax)
                : 0;

            decimal feeCreditValue =
                    GetFeeValueWithoutAssuranceValue(generalCreditDetailDomainRequest.CreditValue, generalCreditDetailDomainRequest.Fees,
                        generalCreditDetailDomainRequest.Store.GetEffectiveAnnualRate, (int)generalCreditDetailDomainRequest.Frequency, generalCreditDetailDomainRequest.DownPayment,
                        generalCreditDetailDomainRequest.InterestRateDecimalNumbers).Round(generalCreditDetailDomainRequest.DecimalNumbersRound);

            decimal assuranceFeeValue = GetAssuranceFeeValue(assuranceValue, generalCreditDetailDomainRequest.Fees, hasDownPayment)
                .Round(generalCreditDetailDomainRequest.DecimalNumbersRound);

            decimal assuranceTaxFeeValue = GetAssuranceTaxFeeValue(assuranceTaxValue, generalCreditDetailDomainRequest.Fees, hasDownPayment);

            decimal assuranceTotalFeeValue = (assuranceFeeValue + assuranceTaxFeeValue).Round(generalCreditDetailDomainRequest.DecimalNumbersRound);

            decimal feeValue =
                CalculateFeeValue((AssuranceTypes)generalCreditDetailDomainRequest.Store.GetAssuranceType, assuranceValue, assuranceTaxValue,
                    feeCreditValue, assuranceFeeValue, assuranceTaxFeeValue)
                .Round(generalCreditDetailDomainRequest.DecimalNumbersRound);

            decimal totalDownPaymentValue =
                (hasDownPayment
                    ? generalCreditDetailDomainRequest.DownPayment + assuranceFeeValue + assuranceTaxFeeValue
                    : generalCreditDetailDomainRequest.DownPayment)
                .Round(generalCreditDetailDomainRequest.DecimalNumbersRound);

            decimal totalInterestValue = generalCreditDetailDomainRequest.Store.GetEffectiveAnnualRate == 0
                ?
                0 :
                (feeCreditValue * generalCreditDetailDomainRequest.Fees) - (generalCreditDetailDomainRequest.CreditValue - generalCreditDetailDomainRequest.DownPayment);

            return new CreditDetailResponse
            {
                DownPayment = generalCreditDetailDomainRequest.DownPayment.Round(generalCreditDetailDomainRequest.DecimalNumbersRound),
                TotalFeeValue = feeValue,
                CreditValue = generalCreditDetailDomainRequest.CreditValue.Round(generalCreditDetailDomainRequest.DecimalNumbersRound),
                Fees = generalCreditDetailDomainRequest.Fees,
                AssuranceValue = assuranceValue.Round(generalCreditDetailDomainRequest.DecimalNumbersRound),
                InterestRate = GetInterestRate(generalCreditDetailDomainRequest.Store.GetEffectiveAnnualRate,
                    (int)generalCreditDetailDomainRequest.Frequency).Round(generalCreditDetailDomainRequest.InterestRateDecimalNumbers),
                TotalInterestValue = totalInterestValue.Round(generalCreditDetailDomainRequest.DecimalNumbersRound),
                TotalDownPayment = totalDownPaymentValue,
                FeeCreditValue = feeCreditValue,
                AssuranceFeeValue = assuranceFeeValue,
                AssuranceTotalValue = (assuranceValue + assuranceTaxValue).Round(generalCreditDetailDomainRequest.DecimalNumbersRound),
                AssuranceTaxValue = assuranceTaxValue.Round(generalCreditDetailDomainRequest.DecimalNumbersRound),
                AssuranceTaxFeeValue = assuranceTaxFeeValue.Round(generalCreditDetailDomainRequest.DecimalNumbersRound),
                AssuranceTotalFeeValue = assuranceTotalFeeValue,
                DownPaymentPercentage = generalCreditDetailDomainRequest.Store.GetDownPaymentPercentage,
                AssurancePercentage = generalCreditDetailDomainRequest.Store.GetAssurancePercentage,
                TotalPaymentValue = ((feeValue * generalCreditDetailDomainRequest.Fees) + totalDownPaymentValue).Round(generalCreditDetailDomainRequest.DecimalNumbersRound)
            };
        }

        /// <summary>
        /// Get tax value
        /// </summary>
        /// <param name="assuranceValue"></param>
        /// <param name="taxPercentage"></param>
        /// <returns>Tax value</returns>
        private decimal GetTaxValue(decimal assuranceValue, decimal taxPercentage) =>
            assuranceValue * taxPercentage;

        /// <summary>
        /// Get fee value without assurance value.
        /// </summary>
        /// <param name="creditValue"></param>
        /// <param name="fees"></param>
        /// <param name="effectiveAnnualRate"></param>
        /// <param name="frequency"></param>
        /// <param name="downPayment"></param>
        /// <param name="interestRateDecimalNumbersRound"></param>
        /// <returns>Fee value without assurance</returns>
        private decimal GetFeeValueWithoutAssuranceValue(decimal creditValue, int fees, decimal effectiveAnnualRate,
            int frequency, decimal downPayment, int interestRateDecimalNumbersRound)
        {
            if (effectiveAnnualRate == 0)
                return (creditValue - downPayment) / fees;

            decimal interestRate = GetInterestRate(effectiveAnnualRate, frequency).Round(interestRateDecimalNumbersRound);

            decimal interestRateFrequency = (decimal)(Math.Pow((double)(1 + interestRate), fees));

            decimal feeValueWithoutAssuranceValue = ((creditValue - downPayment) * (interestRate * interestRateFrequency)) / (interestRateFrequency - 1);

            return feeValueWithoutAssuranceValue;
        }

        /// <summary>
        /// <see cref="ICreditOperationsUseCase.GetInterestRate(decimal, int)"/>
        /// </summary>
        /// <param name="effectiveAnnualRate"></param>
        /// <param name="frequency"></param>
        /// <returns>Interest Rate</returns>
        public decimal GetInterestRate(decimal effectiveAnnualRate, int frequency) =>
            (decimal)(Math.Pow((double)(1 + effectiveAnnualRate), ((double)frequency / 360)) - 1);

        /// <summary>
        /// Get assurance fee value
        /// </summary>
        /// <param name="assuranceValue"></param>
        /// <param name="fees"></param>
        /// <param name="hasDownPayment"></param>
        /// <returns>Assurance fee value</returns>
        private decimal GetAssuranceFeeValue(decimal assuranceValue, decimal fees, bool hasDownPayment)
        {
            if (hasDownPayment)
            {
                return assuranceValue / (fees + 1);
            }
            return assuranceValue / fees;
        }

        /// <summary>
        /// Get fee tax value
        /// </summary>
        /// <param name="assuranceTaxValue"></param>
        /// <param name="fees"></param>
        /// <param name="hasDownPayment"></param>
        /// <returns>Assurance tax fee</returns>
        private decimal GetAssuranceTaxFeeValue(decimal assuranceTaxValue, int fees, bool hasDownPayment)
        {
            if (hasDownPayment)
            {
                return assuranceTaxValue / (fees + 1);
            }
            return assuranceTaxValue / fees;
        }

        /// <summary>
        /// Calculate Fee Value
        /// </summary>
        /// <param name="assuranceTypes"></param>
        /// <param name="assuranceValue"></param>
        /// <param name="assuranceTaxValue"></param>
        /// <param name="feeValueWithoutAssuranceValue"></param>
        /// <param name="assuranceFeeValue"></param>
        /// <param name="assuranceTaxFeeValue"></param>
        /// <returns>Fee value</returns>
        private decimal CalculateFeeValue(AssuranceTypes assuranceTypes, decimal assuranceValue, decimal assuranceTaxValue, decimal feeValueWithoutAssuranceValue,
            decimal assuranceFeeValue, decimal assuranceTaxFeeValue)
        {
            decimal feeValue = 0;

            switch (assuranceTypes)
            {
                case AssuranceTypes.TypeA:
                    feeValue = feeValueWithoutAssuranceValue + assuranceFeeValue + assuranceTaxFeeValue;
                    break;

                case AssuranceTypes.TypeB:
                    feeValue = feeValueWithoutAssuranceValue + GetFinalBalanceTypeB(assuranceValue, assuranceTaxValue);
                    break;
            }

            return feeValue;
        }

        /// <summary>
        /// Get final balance type b
        /// </summary>
        /// <param name="assuranceValue"></param>
        /// <param name="assuranceTaxValue"></param>
        /// <returns>Final balance for type B</returns>
        private decimal GetFinalBalanceTypeB(decimal assuranceValue, decimal assuranceTaxValue)
        {
            return assuranceValue + assuranceTaxValue;
        }

        /// <summary>
        /// Get assurance value.
        /// </summary>
        /// <param name="creditValue"></param>
        /// <param name="assurancePercentage"></param>
        /// <returns>Assurance value</returns>
        public decimal CalculateAssuranceValue(decimal creditValue, decimal assurancePercentage) =>
            creditValue * assurancePercentage;
    }
}