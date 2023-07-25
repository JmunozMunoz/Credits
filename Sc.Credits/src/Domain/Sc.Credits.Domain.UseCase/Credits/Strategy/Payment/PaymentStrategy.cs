using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.Helpers.Commons.Extensions;
using System;

namespace Sc.Credits.Domain.UseCase.Credits.Strategy.Payment
{
    /// <summary>
    /// Payment strategy
    /// </summary>
    public abstract class PaymentStrategy
    {
        /// <summary>
        /// Gets the payment value
        /// </summary>
        protected decimal PaymentValue { get; private set; }

        /// <summary>
        /// Gets the calculation date
        /// </summary>
        protected DateTime CalculationDate { get; private set; }

        /// <summary>
        /// Gets the credit master
        /// </summary>
        protected CreditMaster CreditMaster { get; private set; }

        /// <summary>
        /// Gets the credit payment uses case
        /// </summary>
        protected ICreditPaymentUsesCase CreditPaymentUsesCase { get; private set; }

        /// <summary>
        /// Gets the credit uses case
        /// </summary>
        protected ICreditUsesCase CreditUsesCase { get; private set; }

        /// <summary>
        /// Gets the parameters
        /// </summary>
        protected AppParameters Parameters { get; private set; }

        /// <summary>
        /// Gets the maximum payment adjustment residue
        /// </summary>
        protected decimal MaximumPaymentAdjustmentResidue { get; private set; }

        /// <summary>
        /// Gets the decimal numbers round
        /// </summary>
        protected int DecimalNumbersRound { get; private set; }

        /// <summary>
        /// Gets the interest rate decimal numbers
        /// </summary>
        protected int InterestRateDecimalNumbers { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="PaymentStrategy"/>
        /// </summary>
        /// <param name="creditPaymentUsesCase"></param>
        /// <param name="creditUsesCase"></param>
        /// <param name="creditMaster"></param>
        /// <param name="paymentValue"></param>
        /// <param name="calculationDate"></param>
        /// <param name="parameters"></param>
        protected PaymentStrategy(ICreditPaymentUsesCase creditPaymentUsesCase, ICreditUsesCase creditUsesCase, CreditMaster creditMaster,
            decimal paymentValue, DateTime calculationDate, AppParameters parameters)
        {
            CreditPaymentUsesCase = creditPaymentUsesCase;
            CreditUsesCase = creditUsesCase;
            CreditMaster = creditMaster;
            PaymentValue = paymentValue;
            CalculationDate = calculationDate;
            Parameters = parameters;
            MaximumPaymentAdjustmentResidue = parameters.MaximumPaymentAdjustmentResidue;
            DecimalNumbersRound = parameters.DecimalNumbersRound;
            InterestRateDecimalNumbers = parameters.InterestRateDecimalNumbers;
        }

        /// <summary>
        /// Get payment detail
        /// </summary>
        /// <returns></returns>
        public abstract PaymentDetail GetPaymentDetail();

        /// <summary>
        /// Get Amortization Schedule
        /// </summary>
        /// <returns></returns>
        protected AmortizationScheduleResponse GetAmortizationSchedule()
        {
            Credit currentCredit = CreditMaster.Current;
            decimal originalInterestRate = CreditUsesCase.GetInterestRate(CreditMaster.GetEffectiveAnnualRate, currentCredit.GetFrequency).Round(InterestRateDecimalNumbers);

            return CreditUsesCase.GetOriginalAmortizationSchedule(AmortizationScheduleRequest.FromCredit(currentCredit, CreditMaster.GetCreditDate,
                originalInterestRate), DecimalNumbersRound);
        }
    }
}