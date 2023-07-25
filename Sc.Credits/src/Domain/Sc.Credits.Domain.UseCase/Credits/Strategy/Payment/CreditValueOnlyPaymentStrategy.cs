using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Parameters;
using System;

namespace Sc.Credits.Domain.UseCase.Credits.Strategy.Payment
{
    /// <summary>
    /// Credit value only payment strategy
    /// </summary>
    public class CreditValueOnlyPaymentStrategy
        : PaymentStrategy
    {
        private readonly PaymentType _paymentType;
        private readonly AppParameters _appParameters;
        private readonly ICreditUsesCase _creditUsesCase;

        /// <summary>
        /// Creates a new instance of <see cref="CreditValueOnlyPaymentStrategy"/>
        /// </summary>
        /// <param name="creditPaymentUsesCase"></param>
        /// <param name="creditUsesCase"></param>
        /// <param name="creditMaster"></param>
        /// <param name="paymentValue"></param>
        /// <param name="calculationDate"></param>
        /// <param name="paymentType"></param>
        /// <param name="parameters"></param>
        public CreditValueOnlyPaymentStrategy(ICreditPaymentUsesCase creditPaymentUsesCase, ICreditUsesCase creditUsesCase,
            CreditMaster creditMaster, decimal paymentValue, DateTime calculationDate, PaymentType paymentType,
            AppParameters parameters)
            : base(creditPaymentUsesCase, creditUsesCase, creditMaster, paymentValue, calculationDate, parameters)
        {
            _paymentType = paymentType;
            _appParameters = parameters;
            _creditUsesCase = creditUsesCase;
        }

        /// <summary>
        /// <see cref="PaymentStrategy.GetPaymentDetail"/>
        /// </summary>
        /// <returns></returns>
        public override PaymentDetail GetPaymentDetail()
        {
            Credit currentCredit = CreditMaster.Current;

            bool fraudOrDeceased = _paymentType.Id == (int)PaymentTypes.Fraud || _paymentType.Id == (int)PaymentTypes.Deceased;

            decimal pendingValue = fraudOrDeceased ? currentCredit.GetBalance : PaymentValue;

            decimal creditValuePayment = GetCreditValuePayment(currentCredit, ref pendingValue);

            decimal balance = currentCredit.GetBalance - creditValuePayment;

            SkipAssurance(currentCredit, creditValuePayment);

            AmortizationScheduleResponse amortizationSchedule = GetAmortizationSchedule();

            int lastPaymentFee = CreditPaymentUsesCase.GetLastPaymentFee(amortizationSchedule, balance, out DateTime nextDueDate);

            bool creditPaid = balance == 0;

            return new PaymentDetail
            {
                PaymentValue = PaymentValue,
                CreditValuePayment = creditValuePayment,
                InterestRate = currentCredit.GetInterestRate,
                LastPaymentFee = lastPaymentFee,
                NextDueDate = nextDueDate,
                CreditPaid = creditPaid,
                LastPaymentDate = CalculationDate
            };
        }

        /// <summary>
        /// Skip assurance
        /// </summary>
        /// <param name="currentCredit"></param>
        /// <param name="creditValuePayment"></param>
        private void SkipAssurance(Credit currentCredit, decimal creditValuePayment)
        {
            decimal assuranceValueToOmit = _creditUsesCase.CalculateAssuranceToBalance(creditValuePayment, currentCredit.GetAssurancePercentage, _appParameters.AssuranceTax);

            decimal newAssuranceBalance = currentCredit.GetAssuranceBalance - assuranceValueToOmit;

            currentCredit.SetAssuranceBalance(newAssuranceBalance);
        }

        /// <summary>
        /// Get credit value payment
        /// </summary>
        /// <param name="currentCredit"></param>
        /// <param name="pendingValue"></param>
        /// <returns></returns>
        private static decimal GetCreditValuePayment(Credit currentCredit, ref decimal pendingValue)
        {
            decimal creditValuePayment = 0;
            if (pendingValue > 0 && currentCredit.GetBalance >= pendingValue)
            {
                creditValuePayment = pendingValue;
            }
            else if (pendingValue > 0 && currentCredit.GetBalance < pendingValue)
            {
                creditValuePayment = currentCredit.GetBalance;
            }

            pendingValue -= creditValuePayment;
            return creditValuePayment;
        }
    }
}