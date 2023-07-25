using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Builders;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.Helpers.Commons.Extensions;
using System;

namespace Sc.Credits.Domain.UseCase.Credits.Strategy.Payment
{
    /// <summary>
    /// Ordinary payment strategy
    /// </summary>
    public class OrdinaryPaymentStrategy
        : PaymentStrategy
    {
        private readonly int _graceDays;
        private readonly decimal _effectiveAnnualRate;
        private readonly decimal _arrearsEffectiveAnnualRate;
        private readonly decimal _totalPayment;
        private readonly DateTime _arrearsAdjustmentDate;

        /// <summary>
        /// Creates a new instance of <see cref="OrdinaryPaymentStrategy"/>
        /// </summary>
        /// <param name="creditPaymentUsesCase"></param>
        /// <param name="creditUsesCase"></param>
        /// <param name="creditMaster"></param>
        /// <param name="paymentValue"></param>
        /// <param name="totalPayment"></param>
        /// <param name="calculationDate"></param>
        /// <param name="parameters"></param>
        public OrdinaryPaymentStrategy(ICreditPaymentUsesCase creditPaymentUsesCase, ICreditUsesCase creditUsesCase,
            CreditMaster creditMaster, decimal paymentValue, decimal totalPayment, DateTime calculationDate, AppParameters parameters)
            : base(creditPaymentUsesCase, creditUsesCase, creditMaster, paymentValue, calculationDate, parameters)
        {
            _graceDays = parameters.ArrearsGracePeriod;
            _effectiveAnnualRate = creditUsesCase.GetValidEffectiveAnnualRate(creditMaster.GetEffectiveAnnualRate,
                creditMaster.Store.GetEffectiveAnnualRate);
            _arrearsEffectiveAnnualRate = parameters.ArrearsEffectiveAnnualRate;
            _totalPayment = totalPayment;
            _arrearsAdjustmentDate = parameters.ArrearsAdjustmentDate;
        }

        /// <summary>
        /// <see cref="PaymentStrategy.GetPaymentDetail"/>
        /// </summary>
        /// <returns></returns>
        public override PaymentDetail GetPaymentDetail()
        {
            Credit currentCredit = CreditMaster.Current;

            AmortizationScheduleResponse amortizationSchedule = GetAmortizationSchedule();

            decimal interestRate = CreditUsesCase.GetInterestRate(_effectiveAnnualRate, currentCredit.GetFrequency).Round(InterestRateDecimalNumbers);

            decimal pendingValue = PaymentValue;

            int lastPaymentFee = currentCredit.CreditPayment.GetLastFee;
            DateTime nextDueDate = currentCredit.CreditPayment.GetDueDate;

            if (lastPaymentFee == 0 && CalculationDate < nextDueDate && pendingValue == currentCredit.GetBalance)
            {
                lastPaymentFee = CreditPaymentUsesCase.GetLastPaymentFee(amortizationSchedule, creditValueBalance: 0, out nextDueDate);

                return new PaymentDetail
                {
                    PaymentValue = PaymentValue,
                    CreditValuePayment = PaymentValue,
                    InterestRate = interestRate,
                    LastPaymentFee = lastPaymentFee,
                    NextDueDate = nextDueDate,
                    CreditPaid = true,
                    LastPaymentDate = CalculationDate
                };
            }

            bool hasArrears = CreditUsesCase.HasArrears(CalculationDate, currentCredit.CreditPayment.GetDueDate, _graceDays);

            PaymentMatrix paymentMatrix = PaymentMatrixBuilder.CreateBuilder()
                .BasicInfo(amortizationSchedule, currentCredit.GetBalance, currentCredit.GetAssuranceBalance)
                .DateInfo(CalculationDate.Date, currentCredit.CreditPayment.GetLastPaymentDate)
                .CreditParametersInfo(_effectiveAnnualRate, _arrearsEffectiveAnnualRate, _graceDays)
                .ArrearsAdjustmentDate(_arrearsAdjustmentDate)
                .Build();

            decimal interestValuePayment = GetInterestValuePayment(currentCredit, paymentMatrix, ref pendingValue);
            decimal arrearsValuePayment = GetArrearsValuePayment(currentCredit, paymentMatrix, hasArrears, ref pendingValue, out int arrearsDays);
            decimal chargesValuePayment = GetChargesValuePayment(currentCredit, ref pendingValue);
            decimal assuranceValuePayment = GetAssuranceValuePayment(currentCredit, paymentMatrix, amortizationSchedule, ref pendingValue, interestValuePayment,
                arrearsValuePayment, chargesValuePayment);
            decimal creditValuePayment = GetCreditValuePayment(currentCredit, ref pendingValue);

            decimal residue = Math.Max(pendingValue, 0);

            decimal previousArrears = 0;
            decimal previousInterest = 0;
            DateTime lastPaymentDate = CalculationDate;
            if (creditValuePayment == 0)
            {
                previousArrears = currentCredit.CreditPayment.GetPreviousArrears + arrearsValuePayment;
                previousInterest = currentCredit.CreditPayment.GetPreviousInterest + interestValuePayment;
                lastPaymentDate = currentCredit.CreditPayment.GetLastPaymentDateComplete;
            }

            decimal balance = currentCredit.GetBalance - creditValuePayment;
            bool creditPaid = balance == 0;
            lastPaymentFee = CreditPaymentUsesCase.GetLastPaymentFee(amortizationSchedule, balance, out DateTime nextDueDateEnd);

            decimal paymentValue = PaymentValue;
            if (!creditPaid)
            {
                PaymentAdjustmentByResidue(currentCredit, amortizationSchedule, ref lastPaymentFee, ref creditValuePayment,
                    ref balance, ref nextDueDateEnd, ref creditPaid, ref paymentValue);
            }

            decimal activeFeeValuePaid = GetActiveFeeValuePaid(lastPaymentFee, currentCredit, interestValuePayment, creditValuePayment, assuranceValuePayment, hasArrears);
            return new PaymentDetail
            {
                PaymentValue = paymentValue,
                InterestValuePayment = interestValuePayment,
                ArrearsValuePayment = arrearsValuePayment,
                AssuranceValuePayment = assuranceValuePayment,
                ChargesValuePayment = chargesValuePayment,
                CreditValuePayment = creditValuePayment,
                ArrearsDays = arrearsDays,
                InterestRate = interestRate,
                LastPaymentFee = lastPaymentFee,
                NextDueDate = nextDueDateEnd,
                CreditPaid = creditPaid,
                Residue = residue,
                PreviousArrears = previousArrears,
                PreviousInterest = previousInterest,
                ActiveFeeValuePaid = activeFeeValuePaid,
                LastPaymentDate = lastPaymentDate
            };
        }

        /// <summary>
        /// Get active fee value paid
        /// </summary>
        /// <param name="lastFee"></param>
        /// <param name="currentCredit"></param>
        /// <param name="interestValuePayment"></param>
        /// <param name="creditValuePayment"></param>
        /// <param name="assuranceValuePayment"></param>
        /// <param name="hasArrears"></param>
        /// <returns></returns>
        private decimal GetActiveFeeValuePaid(int lastFee, Credit currentCredit, decimal interestValuePayment, decimal creditValuePayment, decimal assuranceValuePayment, bool hasArrears)
        {
            int previousLastFee = currentCredit.CreditPayment.GetLastFee;

            if (hasArrears || lastFee > previousLastFee + 1)
                return 0;

            decimal feeValuePaid = interestValuePayment + creditValuePayment + assuranceValuePayment;
            decimal activeFeeValuePaid = currentCredit.CreditPayment.GetActiveFeeValuePaid;
            bool isCreditOnNextFee = lastFee == previousLastFee + 1;

            return Math.Max(feeValuePaid + activeFeeValuePaid - (isCreditOnNextFee ? currentCredit.GetTotalFeeValue : 0), 0);
        }

        /// <summary>
        /// Get interest value payment
        /// </summary>
        /// <param name="currentCredit"></param>
        /// <param name="paymentMatrix"></param>
        /// <param name="pendingValue"></param>
        /// <returns></returns>
        private decimal GetInterestValuePayment(Credit currentCredit, PaymentMatrix paymentMatrix, ref decimal pendingValue)
        {
            decimal interestValuePayment =
                CreditPaymentUsesCase.GetInterestPayment(paymentMatrix, currentCredit.CreditPayment.GetPreviousInterest).Round(DecimalNumbersRound);

            interestValuePayment = Math.Min(pendingValue, interestValuePayment);
            pendingValue -= interestValuePayment;

            return interestValuePayment;
        }

        /// <summary>
        /// Get arrears values payment
        /// </summary>
        /// <param name="currentCredit"></param>
        /// <param name="paymentMatrix"></param>
        /// <param name="hasArrears"></param>
        /// <param name="pendingValue"></param>
        /// <param name="arrearsDays"></param>
        /// <returns></returns>
        private decimal GetArrearsValuePayment(Credit currentCredit, PaymentMatrix paymentMatrix, bool hasArrears, ref decimal pendingValue, out int arrearsDays)
        {
            decimal arrearsValuePayment = 0;
            arrearsDays = CreditUsesCase.GetArrearsDays(CalculationDate, currentCredit.CreditPayment.GetDueDate);
            if (!hasArrears)
            {
                return arrearsValuePayment;
            }
            if (pendingValue > 0)
            {
                arrearsValuePayment = CreditPaymentUsesCase.GetArrearsPayment(paymentMatrix, currentCredit.GetArrearsCharge,
                    currentCredit.GetHasArrearsCharge, currentCredit.CreditPayment.GetPreviousArrears).Round(DecimalNumbersRound);

                arrearsValuePayment = Math.Min(pendingValue, arrearsValuePayment);

                pendingValue -= arrearsValuePayment;
            }

            return arrearsValuePayment;
        }

        /// <summary>
        /// Get charges value payment
        /// </summary>
        /// <param name="currentCredit"></param>
        /// <param name="pendingValue"></param>
        /// <returns></returns>
        private decimal GetChargesValuePayment(Credit currentCredit, ref decimal pendingValue)
        {
            decimal chargesValuePayment = 0;
            if (pendingValue > 0 && currentCredit.GetChargeValue > 0)
            {
                chargesValuePayment = currentCredit.GetChargeValue;

                chargesValuePayment = Math.Min(pendingValue, chargesValuePayment);

                pendingValue -= chargesValuePayment;
            }

            return chargesValuePayment;
        }

        /// <summary>
        /// Get assurance value payment
        /// </summary>
        /// <param name="currentCredit"></param>
        /// <param name="paymentMatrix"></param>
        /// <param name="amortizationSchedule"></param>
        /// <param name="pendingValue"></param>
        /// <param name="interestValuePayment"></param>
        /// <param name="arrearsValuePayment"></param>
        /// <param name="chargeValue"></param>
        /// <returns></returns>
        private decimal GetAssuranceValuePayment(Credit currentCredit, PaymentMatrix paymentMatrix, AmortizationScheduleResponse amortizationSchedule, ref decimal pendingValue, decimal interestValuePayment,
            decimal arrearsValuePayment, decimal chargeValue)
        {
            decimal assuranceValuePayment = 0;
            if (pendingValue > 0)
            {
                assuranceValuePayment = CreditPaymentUsesCase.GetAssuranceOrdinaryPayment(paymentMatrix, pendingValue, amortizationSchedule,
                    currentCredit.GetBalance, MaximumPaymentAdjustmentResidue, CalculationDate, PaymentValue, interestValuePayment,
                    arrearsValuePayment, chargeValue, _graceDays, _totalPayment);

                assuranceValuePayment = Math.Min(pendingValue, assuranceValuePayment);

                pendingValue -= assuranceValuePayment;
            }

            return assuranceValuePayment;
        }

        /// <summary>
        /// Get credit value payment
        /// </summary>
        /// <param name="currentCredit"></param>
        /// <param name="pendingValue"></param>
        /// <returns></returns>
        private static decimal GetCreditValuePayment(Credit currentCredit, ref decimal pendingValue)
        {
            decimal creditValuePayment = Math.Min(pendingValue, currentCredit.GetBalance);

            pendingValue -= creditValuePayment;

            return creditValuePayment;
        }

        /// <summary>
        /// Ajuste de diferencias
        /// </summary>
        /// <param name="currentCredit"></param>
        /// <param name="amortizationSchedule"></param>
        /// <param name="lastPaymentFee"></param>
        /// <param name="creditValuePayment"></param>
        /// <param name="balance"></param>
        /// <param name="nextDueDateEnd"></param>
        /// <param name="creditPaid"></param>
        /// <param name="paymentValue"></param>
        private void PaymentAdjustmentByResidue(Credit currentCredit, AmortizationScheduleResponse amortizationSchedule, ref int lastPaymentFee,
            ref decimal creditValuePayment, ref decimal balance, ref DateTime nextDueDateEnd, ref bool creditPaid, ref decimal paymentValue)
        {
            bool balanceInMaximumRange = balance <= MaximumPaymentAdjustmentResidue;

            if (balanceInMaximumRange)
            {
                creditValuePayment += balance;
                paymentValue += balance;

                balance = currentCredit.GetBalance - creditValuePayment;
                lastPaymentFee = CreditPaymentUsesCase.GetLastPaymentFee(amortizationSchedule, balance, out nextDueDateEnd);
                creditPaid = balance == 0;
            }
        }
    }
}