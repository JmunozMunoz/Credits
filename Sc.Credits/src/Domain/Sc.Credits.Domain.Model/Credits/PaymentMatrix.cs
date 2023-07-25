using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// Payment Matrix
    /// </summary>
    public class PaymentMatrix
    {
        private readonly int _totalFees;
        private readonly decimal _balance;
        private readonly decimal _assuranceBalance;
        private readonly DateTime _creditInitialDate;
        private DateTime _calculationDate;
        private DateTime _lastPaymentDate;
        private int _arrearsGracePeriod;
        private decimal _effectiveAnnualRate;
        private decimal _arrearsEffectiveAnnualRate;
        private bool _isCreditPaid;
        private DateTime _arrearsAdjustmentDate;

        /// <summary>
        /// Gets the active fee
        /// </summary>
        public PaymentMatrixFee ActiveFee { get; private set; }

        /// <summary>
        /// Gets the payment matrix fees
        /// </summary>
        public IEnumerable<PaymentMatrixFee> Fees { get; private set; }

        /// <summary>
        /// Gets the payable interest
        /// </summary>
        public decimal PayableInterest { get; private set; }

        /// <summary>
        /// Gets the payable arrears
        /// </summary>
        public decimal PayableArrears { get; private set; }

        /// <summary>
        /// Gets the payable assurance
        /// </summary>
        public decimal PayableAssurance { get; private set; }

        /// <summary>
        /// Gets the last fee paid number
        /// </summary>
        public int LastFeePaidNumber { get; private set; }

        /// <summary>
        /// Gets the First unpaid fee
        /// </summary>
        public PaymentMatrixFee FirstUnpaidFee { get; private set; }

        /// <summary>
        /// Gets the first unpaid fee number
        /// </summary>
        public int FirstUnpaidFeeNumber { get => FirstUnpaidFee.FeeNumber; }

        /// <summary>
        /// Gets the number of pending fees
        /// </summary>
        public int PendingFees { get => _totalFees - LastFeePaidNumber; }

        /// <summary>
        /// Gets the number of arrears fees
        /// </summary>
        public int ArrearsFees { get => Fees.Count(fee => fee.FeeDate <= _calculationDate.Date && fee.FeeNumber >= FirstUnpaidFeeNumber); }

        /// <summary>
        /// Payment matrix constructor
        /// </summary>
        /// <param name="amortizationSchedule"></param>
        /// <param name="balance"></param>
        /// <param name="assuranceBalance"></param>
        public PaymentMatrix(AmortizationScheduleResponse amortizationSchedule, decimal balance, decimal assuranceBalance)
        {
            List<AmortizationScheduleFee> amortizationScheduleFees = amortizationSchedule.AmortizationScheduleFees;

            _totalFees = amortizationScheduleFees.Last().Fee;
            _balance = balance;
            _assuranceBalance = assuranceBalance;
            _creditInitialDate = amortizationScheduleFees.First().FeeDate;
            _calculationDate = _creditInitialDate;
            _lastPaymentDate = _creditInitialDate;

            CreateBasicPaymentMatrixFees(amortizationScheduleFees, amortizationSchedule.AmortizationScheduleAssuranceFees);
        }

        /// <summary>
        /// Set date info.
        /// </summary>
        /// <param name="calculationDate"></param>
        /// <param name="lastPaymentDate"></param>
        public void SetDateInfo(DateTime calculationDate, DateTime lastPaymentDate)
        {
            _calculationDate = DateTimeHelper.LatestDate(_creditInitialDate, calculationDate);
            _lastPaymentDate = DateTimeHelper.LatestDate(_creditInitialDate, lastPaymentDate);

            ActiveFee = GetActiveFee(_calculationDate);
        }

        /// <summary>
        /// Set credit parameters info
        /// </summary>
        /// <param name="effectiveAnnualRate"></param>
        /// <param name="arrearsEffectiveAnnualRate"></param>
        /// <param name="arrearsGracePeriod"></param>
        public void SetCreditParametersInfo(decimal effectiveAnnualRate, decimal arrearsEffectiveAnnualRate, int arrearsGracePeriod)
        {
            _effectiveAnnualRate = effectiveAnnualRate < 0 ? 0 : effectiveAnnualRate;
            _arrearsEffectiveAnnualRate = arrearsEffectiveAnnualRate < 0 ? 0 : arrearsEffectiveAnnualRate;
            _arrearsGracePeriod = arrearsGracePeriod < 0 ? 0 : arrearsGracePeriod;
        }

        /// <summary>
        /// Set arrears adjustment date
        /// </summary>
        /// <param name="arrearsAdjustmentDate"></param>
        public void SetArrearsAdjustmentDate(DateTime arrearsAdjustmentDate) => _arrearsAdjustmentDate = arrearsAdjustmentDate;

        /// <summary>
        /// Calculate payable values
        /// </summary>
        /// <returns></returns>
        public PaymentMatrix CalculatePayableValues()
        {
            PayableInterest = 0;
            PayableArrears = 0;
            PayableAssurance = 0;
            if (_isCreditPaid) return this;

            IEnumerable<PaymentMatrixFee> payableFees = GetPayableFees(ActiveFee.FeeNumber);

            PaymentMatrixFee previousPayableFee = null;

            foreach (PaymentMatrixFee payableFee in payableFees)
            {
                SetInterestDays(previousPayableFee, payableFee);
                SetInterestPayment(payableFee, _effectiveAnnualRate);

                SetArrearsDays(payableFee);
                SetArrearsPayment(payableFee);

                PayableInterest += payableFee.InterestPayment;
                PayableArrears += payableFee.ArrearsPayment;
                PayableAssurance += payableFee.AssuranceValuePayment;

                previousPayableFee = payableFee;
            }

            return this;
        }

        /// <summary>
        /// Create basic payment matrix fees
        /// </summary>
        /// <param name="amortizationScheduleFees"></param>
        /// <param name="amortizationScheduleAssuranceFees"></param>
        private void CreateBasicPaymentMatrixFees(List<AmortizationScheduleFee> amortizationScheduleFees,
            List<AmortizationScheduleAssuranceFee> amortizationScheduleAssuranceFees)
        {
            List<PaymentMatrixFee> paymentMatrixFees = new List<PaymentMatrixFee>(new PaymentMatrixFee[amortizationScheduleFees.Count]);

            decimal remainingBalance = _balance;
            decimal remainingAssuranceBalance = _assuranceBalance;
            decimal creditValueNotDue = 0;

            for (int fee = _totalFees; fee >= 0; fee--)
            {
                decimal creditValuePayment = Math.Min(amortizationScheduleFees[fee].CreditValuePayment, remainingBalance);
                decimal assuranceValuePayment = Math.Min(amortizationScheduleAssuranceFees[fee].AssurancePaymentValue, remainingAssuranceBalance);

                creditValueNotDue += creditValuePayment;

                paymentMatrixFees[fee] = new PaymentMatrixFee
                {
                    FeeNumber = amortizationScheduleFees[fee].Fee,
                    FeeDate = amortizationScheduleFees[fee].FeeDate,
                    CreditValuePayment = creditValuePayment,
                    AssuranceValuePayment = assuranceValuePayment,
                    CreditValueNotDue = creditValueNotDue,
                };

                remainingBalance -= paymentMatrixFees[fee].CreditValuePayment;
                remainingAssuranceBalance -= paymentMatrixFees[fee].AssuranceValuePayment;

                if (paymentMatrixFees[fee].CreditValuePayment > 0)
                {
                    FirstUnpaidFee = paymentMatrixFees[fee];
                }
            }

            _isCreditPaid = FirstUnpaidFee == null;
            LastFeePaidNumber = _isCreditPaid ? _totalFees : FirstUnpaidFee.FeeNumber - 1;
            Fees = paymentMatrixFees;
        }

        /// <summary>
        /// Set interest days
        /// </summary>
        /// <param name="previousPayableFee"></param>
        /// <param name="payableFee"></param>
        private void SetInterestDays(PaymentMatrixFee previousPayableFee, PaymentMatrixFee payableFee)
        {
            bool interestPaymentAlreadyPaid = payableFee.FeeDate <= _lastPaymentDate;
            if (interestPaymentAlreadyPaid)
            {
                payableFee.InterestDays = 0;
                return;
            }

            bool isFirstUnpaidFee = previousPayableFee == null;

            DateTime initialDate = isFirstUnpaidFee ? _lastPaymentDate : DateTimeHelper.LatestDate(_lastPaymentDate, previousPayableFee.FeeDate);
            DateTime finalDate = DateTimeHelper.EarliestDate(_calculationDate, payableFee.FeeDate);

            payableFee.InterestDays = DateTimeHelper.Difference360BetweenDates(initialDate, finalDate);
        }

        /// <summary>
        /// Set arrears days
        /// </summary>
        /// <param name="payableFee"></param>
        private void SetArrearsDays(PaymentMatrixFee payableFee)
        {
            bool feeIsDue = payableFee.FeeDate < _calculationDate;
            if (!feeIsDue)
            {
                payableFee.ArrearsDays = 0;
                return;
            }

            int arrearsDays = _calculationDate.Subtract(DateTimeHelper.LatestDate(_lastPaymentDate, payableFee.FeeDate, _arrearsAdjustmentDate)).Days;

            payableFee.ArrearsDays = arrearsDays > 0 ? arrearsDays : 0;
        }

        /// <summary>
        /// Get active fee
        /// </summary>
        /// <param name="calculationDate"></param>
        /// <returns></returns>
        private PaymentMatrixFee GetActiveFee(DateTime calculationDate)
        {
            return Fees.FirstOrDefault(fee => fee.FeeDate >= calculationDate && fee.CreditValuePayment > 0) ?? Fees.Last();
        }

        /// <summary>
        /// Get payable fees
        /// </summary>
        /// <param name="activeFeeNumber"></param>
        /// <returns></returns>
        private IEnumerable<PaymentMatrixFee> GetPayableFees(int activeFeeNumber)
        {
            return Fees.Where(fee => fee.FeeNumber > LastFeePaidNumber && fee.FeeNumber <= activeFeeNumber);
        }

        /// <summary>
        /// Set interest payment
        /// </summary>
        /// <param name="payableFee"></param>
        /// <param name="effectiveAnnualRate"></param>
        private void SetInterestPayment(PaymentMatrixFee payableFee, decimal effectiveAnnualRate)
        {
            decimal interestRate = (decimal)(Math.Pow((double)(1 + effectiveAnnualRate), ((double)payableFee.InterestDays / 360)) - 1);

            payableFee.InterestPayment = payableFee.CreditValueNotDue * interestRate;
        }

        /// <summary>
        /// Set arrears payment
        /// </summary>
        /// <param name="payableFee"></param>
        private void SetArrearsPayment(PaymentMatrixFee payableFee)
        {
            if (payableFee.ArrearsDays > _arrearsGracePeriod)
            {
                decimal arrearsRate = _arrearsEffectiveAnnualRate * payableFee.ArrearsDays / 360;
                payableFee.ArrearsPayment = payableFee.CreditValuePayment * arrearsRate;
            }
        }

        /// <summary>
        /// Gets the payment matrix fee by fee number
        /// </summary>
        /// <param name="feeNumber"></param>
        /// <returns></returns>
        public PaymentMatrixFee GetFeeByFeeNumber(int feeNumber)
            => Fees.FirstOrDefault(fee => fee.FeeNumber == feeNumber);
    }
}