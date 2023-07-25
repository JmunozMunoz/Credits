using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Parameters;
using System;

namespace Sc.Credits.Domain.UseCase.Credits.Strategy.Payment
{
    /// <summary>
    /// Down payment strategy
    /// </summary>
    public class DownPaymentStrategy
        : PaymentStrategy
    {
        /// <summary>
        /// Creates a new instance of <see cref="DownPaymentStrategy"/>
        /// </summary>
        /// <param name="creditPaymentUsesCase"></param>
        /// <param name="creditUsesCase"></param>
        /// <param name="creditMaster"></param>
        /// <param name="downPayment"></param>
        /// <param name="calculationDate"></param>
        /// <param name="parameters"></param>
        public DownPaymentStrategy(ICreditPaymentUsesCase creditPaymentUsesCase, ICreditUsesCase creditUsesCase,
            CreditMaster creditMaster, decimal downPayment, DateTime calculationDate, AppParameters parameters)
            : base(creditPaymentUsesCase, creditUsesCase, creditMaster, downPayment, calculationDate, parameters)
        {
        }

        /// <summary>
        /// <see cref="PaymentStrategy.GetPaymentDetail"/>
        /// </summary>
        /// <returns></returns>
        public override PaymentDetail GetPaymentDetail()
        {
            Credit currentCredit = CreditMaster.Current;

            AmortizationScheduleResponse amortizationSchedule = GetAmortizationSchedule();

            decimal pendingValue = PaymentValue;

            decimal assuranceTotalFeeValue = currentCredit.GetAssuranceTotalFeeValue;

            decimal assuranceValuePayment = (pendingValue < assuranceTotalFeeValue) ? pendingValue : assuranceTotalFeeValue;

            pendingValue -= assuranceValuePayment;

            decimal creditValuePayment = GetCreditValuePayment(ref pendingValue);

            decimal balance = currentCredit.GetBalance - creditValuePayment;

            int lastPaymentFee = CreditPaymentUsesCase.GetLastPaymentFee(amortizationSchedule, balance, out DateTime nextDueDate);

            return new PaymentDetail
            {
                PaymentValue = PaymentValue,
                AssuranceValuePayment = assuranceValuePayment,
                CreditValuePayment = creditValuePayment,
                InterestRate = currentCredit.GetInterestRate,
                LastPaymentFee = lastPaymentFee,
                NextDueDate = nextDueDate,
                LastPaymentDate = CalculationDate
            };
        }

        /// <summary>
        /// Get credit value payment
        /// </summary>
        /// <param name="pendingValue"></param>
        /// <returns></returns>
        private static decimal GetCreditValuePayment(ref decimal pendingValue)
        {
            decimal creditValuePayment = 0;
            if (pendingValue > 0)
            {
                creditValuePayment = pendingValue;
                pendingValue -= creditValuePayment;
            }

            return creditValuePayment;
        }
    }
}