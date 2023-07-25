using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Stores;
using System;

namespace Sc.Credits.Domain.Model.Credits.Events
{
    /// <summary>
    /// Add payment event
    /// </summary>
    public class AddPaymentMasterEvent
        : MasterEvent<CreditMaster, Credit>
    {
        private readonly decimal _interestRate;
        private readonly long _paymentNumber;

        private decimal _creditValuePaid;
        private decimal _interestValuePaid;
        private decimal _chargeValuePaid;
        private decimal _arrearsValuePaid;
        private decimal _assuranceValuePaid;
        private decimal _totalValuePaid;
        private string _bankAccount;
        private DateTime _dueDate;
        private DateTime _calculationDate;
        private int _lastFee;
        private int _arrearsDays;
        private string _location;
        private decimal _previousArrears;
        private decimal _previousInterest;
        private decimal _activeFeeValuePaid;
        private DateTime _lastPaymentDate;
        private string _transactionId;
        private decimal _valor;

        private readonly CreditMaster _creditMaster;
        private readonly TransactionType _transactionType;
        private readonly PaymentType _paymentType;
        private readonly Status _status;
        private readonly Store _store;

        private UserInfo _userInfo;

        /// <summary>
        /// Creates a new instance of <see cref="AddPaymentMasterEvent"/>
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="paymentNumber"></param>
        /// <param name="transactionType"></param>
        /// <param name="paymentType"></param>
        /// <param name="status"></param>
        /// <param name="store"></param>
        /// <param name="interestRate"></param>
        /// <param name="valor"></param>
        public AddPaymentMasterEvent(CreditMaster creditMaster, long paymentNumber, TransactionType transactionType,
            PaymentType paymentType, Status status, Store store, decimal interestRate, decimal valor)
            : base(creditMaster)
        {
            _creditMaster = creditMaster;
            _paymentNumber = paymentNumber;
            _transactionType = transactionType;
            _paymentType = paymentType;
            _status = status;
            _interestRate = interestRate;
            _store = store;
            _valor = valor;
        }

        /// <summary>
        /// Add payment values
        /// </summary>
        /// <param name="totalValuePaid"></param>
        /// <param name="creditValuePaid"></param>
        /// <param name="interestValuePaid"></param>
        /// <param name="chargeValuePaid"></param>
        /// <param name="arrearsValuePaid"></param>
        /// <param name="assuranceValuePaid"></param>
        /// <param name="lastFee"></param>
        /// <returns></returns>
        public AddPaymentMasterEvent AddPaymentValues(decimal totalValuePaid, decimal creditValuePaid, decimal interestValuePaid,
            decimal chargeValuePaid, decimal arrearsValuePaid, decimal assuranceValuePaid, int lastFee)
        {
            _totalValuePaid = totalValuePaid;
            _creditValuePaid = creditValuePaid;
            _interestValuePaid = interestValuePaid;
            _chargeValuePaid = chargeValuePaid;
            _arrearsValuePaid = arrearsValuePaid;
            _assuranceValuePaid = assuranceValuePaid;
            _lastFee = lastFee;

            return this;
        }

        /// <summary>
        /// Set dates
        /// </summary>
        /// <param name="dueDate"></param>
        /// <param name="calculationDate"></param>
        /// <param name="lastPaymentDate"></param>
        /// <returns></returns>
        public AddPaymentMasterEvent SetDates(DateTime dueDate, DateTime calculationDate, DateTime lastPaymentDate)
        {
            _dueDate = dueDate;
            _calculationDate = calculationDate;
            _lastPaymentDate = lastPaymentDate;

            return this;
        }

        /// <summary>
        /// Set adjustment values
        /// </summary>
        /// <param name="previousArrears"></param>
        /// <param name="previousInterest"></param>
        /// <param name="activeFeeValuePaid"></param>
        /// <returns></returns>
        public AddPaymentMasterEvent SetAdjustmentValues(decimal previousArrears, decimal previousInterest,
            decimal activeFeeValuePaid)
        {
            _previousArrears = previousArrears;
            _previousInterest = previousInterest;
            _activeFeeValuePaid = activeFeeValuePaid;

            return this;
        }

        /// <summary>
        /// Set additional info
        /// </summary>
        /// <param name="bankAccount"></param>
        /// <param name="location"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public AddPaymentMasterEvent SetAdditionalInfo(string bankAccount, string location, UserInfo userInfo)
        {
            _bankAccount = bankAccount;
            _location = location;
            _userInfo = userInfo;

            return this;
        }

        /// <summary>
        /// Set arrears
        /// </summary>
        /// <param name="arrearsDays"></param>
        /// <returns></returns>
        public AddPaymentMasterEvent SetArrears(int arrearsDays)
        {
            _arrearsDays = arrearsDays;

            return this;
        }

        /// <summary>
        /// Set transaction reference
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public AddPaymentMasterEvent SetTransactionReference(string transactionId)
        {
            _transactionId = transactionId;
            return this;
        }

        /// <summary>
        /// <see cref="MasterEvent{TMaster, TEntity}.Handle(TEntity)"/>
        /// </summary>
        /// <param name="newEntity"></param>
        public override void Handle(Credit newEntity)
        {
            newEntity.InitPayment(_paymentNumber, _transactionType, _paymentType, _lastFee, _totalValuePaid, _bankAccount);
            newEntity.SetPaymentInfo(_userInfo, _status, _store, _location, _arrearsDays, _interestRate);
            newEntity.SetPaymentValues(_creditValuePaid, _interestValuePaid, _chargeValuePaid, _arrearsValuePaid, _assuranceValuePaid);
            newEntity.SetCalculationDates(_dueDate, _calculationDate);
            newEntity.SetLastPaymentDate(_lastPaymentDate);
            newEntity.SetAdjustmentPaymentValues(_previousArrears, _previousInterest, _activeFeeValuePaid);
            newEntity.SetTransactionReference(_transactionId);

            _creditMaster.SetStatus(_status);
  
            _creditMaster.Customer.CreditLimitIncrease(_valor > 0 ? _valor: _creditValuePaid);
        }
    }
}