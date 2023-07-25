using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Helpers.Commons.Dapper.Annotations;
using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// Credit payment class
    /// </summary>
    public class CreditPayment
        : Entity<Guid>
    {
        private decimal _totalValuePaid;
        private int _lastFee;
        private int _transactionTypeId;
        private string _transactionTypeName;
        private decimal _creditValuePaid;
        private decimal _interestValuePaid;
        private decimal _chargeValuePaid;
        private decimal _arrearsValuePaid;
        private decimal _assuranceValuePaid;
        private string _bankAccount;
        private DateTime _lastPaymentDate;
        private TimeSpan _lastPaymentTime;
        private DateTime _dueDate;
        private DateTime _calculationDate;
        private int _paymentTypeId;
        private string _paymentTypeName;
        private long _paymentNumber;
        private decimal _activeFeeValuePaid;
        private decimal _previousInterest;
        private decimal _previousArrears;

        /// <summary>
        /// Gets the transaction type id
        /// </summary>
        [Write(false)]
        public int GetTransactionTypeId => _transactionTypeId;

        /// <summary>
        /// Gets the due date
        /// </summary>
        [Write(false)]
        public DateTime GetDueDate => _dueDate;

        /// <summary>
        /// Gets the last payment date
        /// </summary>
        [Write(false)]
        public DateTime GetLastPaymentDate => _lastPaymentDate;

        /// <summary>
        /// Gets the last payment time
        /// </summary>
        [Write(false)]
        public TimeSpan GetLastPaymentTime => _lastPaymentTime;

        /// <summary>
        /// Gets the last payment date complete
        /// </summary>
        [Write(false)]
        public DateTime GetLastPaymentDateComplete => _lastPaymentDate + _lastPaymentTime;

        /// <summary>
        /// Gets the last fee
        /// </summary>
        [Write(false)]
        public int GetLastFee => _lastFee;

        /// <summary>
        /// Gets the payment number
        /// </summary>
        [Write(false)]
        public long GetPaymentNumber => _paymentNumber;

        /// <summary>
        /// Gets the arrears value paid
        /// </summary>
        [Write(false)]
        public decimal GetArrearsValuePaid => _arrearsValuePaid;

        /// <summary>
        /// Gets the assurance value paid
        /// </summary>
        [Write(false)]
        public decimal GetAssuranceValuePaid => _assuranceValuePaid;

        /// <summary>
        /// Gets the charge value paid
        /// </summary>
        [Write(false)]
        public decimal GetChargeValuePaid => _chargeValuePaid;

        /// <summary>
        /// Gets the credit value paid
        /// </summary>
        [Write(false)]
        public decimal GetCreditValuePaid => _creditValuePaid;

        /// <summary>
        /// Gets the interest value paid
        /// </summary>
        [Write(false)]
        public decimal GetInterestValuePaid => _interestValuePaid;

        /// <summary>
        /// Gets the total value paid
        /// </summary>
        [Write(false)]
        public decimal GetTotalValuePaid => _totalValuePaid;

        /// <summary>
        /// Gets the active fee value paid
        /// </summary>
        [Write(false)]
        public decimal GetActiveFeeValuePaid => _activeFeeValuePaid;

        /// <summary>
        /// Gets the previous interest
        /// </summary>
        [Write(false)]
        public decimal GetPreviousInterest => _previousInterest;

        /// <summary>
        /// Gets the previous arrears
        /// </summary>
        [Write(false)]
        public decimal GetPreviousArrears => _previousArrears;

        /// <summary>
        /// Gets the transaction type name
        /// </summary>
        [Write(false)]
        public string GetTransactionTypeName => _transactionTypeName;

        /// <summary>
        /// Gets the bank account
        /// </summary>
        [Write(false)]
        public string GetBankAccount => _bankAccount;

        /// <summary>
        /// Gets the calculation date
        /// </summary>
        [Write(false)]
        public DateTime GetCalculationDate => _calculationDate;

        /// <summary>
        /// Gets the payment type id
        /// </summary>
        [Write(false)]
        public int GetPaymentTypeId => _paymentTypeId;

        /// <summary>
        /// Gets the payment type name
        /// </summary>
        [Write(false)]
        public string GetPaymentTypeName => _paymentTypeName;

        /// <summary>
        /// Creates a new instance of <see cref="CreditPayment"/>
        /// </summary>
        protected CreditPayment()
        {
        }

        /// <summary>
        /// New
        /// </summary>
        /// <returns></returns>
        public static CreditPayment New() =>
            new CreditPayment();

        /// <summary>
        /// New credit payment
        /// </summary>
        /// <param name="creditId"></param>
        /// <param name="paymentNumber"></param>
        /// <param name="transactionType"></param>
        /// <param name="totalValuePaid"></param>
        /// <param name="lastFee"></param>
        /// <param name="paymentType"></param>
        /// <param name="bankAccount"></param>
        internal CreditPayment(Guid creditId, long paymentNumber, TransactionType transactionType, PaymentType paymentType, int lastFee, decimal totalValuePaid = 0,
            string bankAccount = "")
        {
            SetId(creditId);
            SetPaymentNumber(paymentNumber);

            _totalValuePaid = totalValuePaid;
            _lastFee = lastFee;
            _bankAccount = bankAccount;

            SetTransactionType(transactionType);
            SetPaymentType(paymentType);
        }

        /// <summary>
        /// Set id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal new CreditPayment SetId(Guid id)
        {
            base.SetId(id);
            return this;
        }

        /// <summary>
        /// Set payment number
        /// </summary>
        /// <param name="paymentNumber"></param>
        private void SetPaymentNumber(long paymentNumber)
        {
            _paymentNumber = paymentNumber;
        }

        /// <summary>
        /// Set transaction type
        /// </summary>
        /// <param name="transactionType"></param>
        internal void SetTransactionType(TransactionType transactionType)
        {
            _transactionTypeId = transactionType.Id;
            _transactionTypeName = transactionType.Name;
        }

        /// <summary>
        /// Set payment type
        /// </summary>
        /// <param name="paymentType"></param>
        private void SetPaymentType(PaymentType paymentType)
        {
            _paymentTypeId = paymentType.Id;
            _paymentTypeName = paymentType.Name;
        }

        /// <summary>
        /// Set payment values
        /// </summary>
        /// <param name="creditValuePaid"></param>
        /// <param name="interestValuePaid"></param>
        /// <param name="chargeValuePaid"></param>
        /// <param name="arrearsValuePaid"></param>
        /// <param name="assuranceValuePaid"></param>
        internal void SetPaymentValues(decimal creditValuePaid, decimal interestValuePaid, decimal chargeValuePaid, decimal arrearsValuePaid,
            decimal assuranceValuePaid)
        {
            _creditValuePaid = creditValuePaid;
            _interestValuePaid = interestValuePaid;
            _chargeValuePaid = chargeValuePaid;
            _arrearsValuePaid = arrearsValuePaid;
            _assuranceValuePaid = assuranceValuePaid;
        }

        /// <summary>
        /// Set calculation date
        /// </summary>
        /// <param name="calculationDate"></param>
        internal void SetCalculationDate(DateTime calculationDate)
        {
            _calculationDate = calculationDate.Date;
        }

        /// <summary>
        /// Set last payment date
        /// </summary>
        /// <param name="lastPaymentDate"></param>
        internal void SetLastPaymentDate(DateTime lastPaymentDate)
        {
            _lastPaymentDate = lastPaymentDate.Date;
            _lastPaymentTime = lastPaymentDate.TimeOfDay;
        }

        /// <summary>
        /// Set due date
        /// </summary>
        /// <param name="dueDate"></param>
        internal void SetDueDate(DateTime dueDate)
        {
            _dueDate = dueDate.Date;
        }

        /// <summary>
        /// Set adjustment values
        /// </summary>
        /// <param name="previousArrears"></param>
        /// <param name="previousInterest"></param>
        /// <param name="activeFeeValuePaid"></param>
        internal void SetAdjustmentValues(decimal previousArrears, decimal previousInterest, decimal activeFeeValuePaid)
        {
            _previousArrears = previousArrears;
            _previousInterest = previousInterest;
            _activeFeeValuePaid = activeFeeValuePaid;
        }

        /// <summary>
        /// Sel last fee
        /// </summary>
        /// <param name="lastFee"></param>
        internal void SetLastFee(int lastFee)
        {
            _lastFee = lastFee;
        }
    }
}