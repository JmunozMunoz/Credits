using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Refinancings;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Helpers.Commons.Dapper.Annotations;
using Sc.Credits.Helpers.Commons.Domain;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// Credits class
    /// </summary>
    public class Credit
        : Entity<Guid>
    {
        private Guid _creditMasterId;
        private decimal _creditValue;
        private int _fees;
        private decimal _feeValue;
        private decimal _assurancePercentage;
        private decimal _assuranceValue;
        private decimal _assuranceFee;
        private decimal _interestRate;
        private int _frequency;
        private decimal _balance;
        private DateTime _createDate;
        private TimeSpan _createTime;
        private DateTime _transactionDate;
        private TimeSpan _transactionTime;
        private string _userId;
        private string _storeId;
        private Guid _customerId;
        private int _statusId;
        private string _statusName;
        private int _sourceId;
        private string _sourceName;
        private int _authMethodId;
        private string _authMethodName;
        private string _location;
        private long _creditNumber;
        private bool _alternatePayment;
        private bool _hasArrearsCharge;
        private decimal? _arrearsCharge;
        private decimal? _chargeValue;
        private decimal? _updatedPaymentPlanValue;

        [Write(false)]
        private int _computedArrearsDays;

        private int? _arrearsDays;
        private decimal _assuranceBalance;
        private decimal _assuranceTotalValue;
        private decimal _downPayment;
        private string _userName;
        private decimal _assuranceTotalFeeValue;
        private decimal _totalDownPayment;

        /// <summary>
        /// Gets the computed arrears days
        /// </summary>
        [Write(false)]
        public int GetComputedArrearsDays => _computedArrearsDays;

        /// <summary>
        /// Gets the store's id
        /// </summary>
        [Write(false)]
        public string GetStoreId => _storeId;

        /// <summary>
        /// Gets the frequency
        /// </summary>
        [Write(false)]
        public int GetFrequency => _frequency;

        /// <summary>
        /// Gets the credit value
        /// </summary>
        [Write(false)]
        public decimal GetCreditValue => _creditValue;

        /// <summary>
        /// Gets the arrears days
        /// </summary>
        [Write(false)]
        public int? GetArrearsDays => _arrearsDays;

        /// <summary>
        /// Gets the fee value
        /// </summary>
        [Write(false)]
        public decimal GetFeeValue => _feeValue;

        /// <summary>
        /// Gets the credit's number
        /// </summary>
        [Write(false)]
        public long GetCreditNumber => _creditNumber;

        /// <summary>
        /// Gets the balance
        /// </summary>
        [Write(false)]
        public decimal GetBalance => _balance;

        /// <summary>
        /// Gets the alternate payment
        /// </summary>
        [Write(false)]
        public bool GetAlternatePayment => _alternatePayment;

        /// <summary>
        /// Gets the has arrears charge
        /// </summary>
        [Write(false)]
        public bool GetHasArrearsCharge => _hasArrearsCharge;

        /// <summary>
        /// Gets the arrears charge
        /// </summary>
        [Write(false)]
        public decimal GetArrearsCharge => _arrearsCharge ?? 0;

        /// <summary>
        /// Gets the assurance balance
        /// </summary>
        [Write(false)]
        public decimal GetAssuranceBalance => _assuranceBalance;

        /// <summary>
        /// Gets the fees
        /// </summary>
        [Write(false)]
        public int GetFees => _fees;

        /// <summary>
        /// Gets the charge value
        /// </summary>
        [Write(false)]
        public decimal GetChargeValue => _chargeValue ?? 0;

        /// <summary>
        /// Gets the interest rate
        /// </summary>
        [Write(false)]
        public decimal GetInterestRate => _interestRate;

        /// <summary>
        /// Gets the assurance value
        /// </summary>
        [Write(false)]
        public decimal GetAssuranceValue => _assuranceValue;

        /// <summary>
        /// Gets the assurance fee
        /// </summary>
        [Write(false)]
        public decimal GetAssuranceFee => _assuranceFee;

        /// <summary>
        /// Gets the down payment
        /// </summary>
        [Write(false)]
        public decimal GetDownPayment => _downPayment;

        /// <summary>
        /// Gets the updated payment plan value
        /// </summary>
        [Write(false)]
        public decimal? GetUpdatedPaymentPlanValue => _updatedPaymentPlanValue;

        /// <summary>
        /// Gets the status id
        /// </summary>
        [Write(false)]
        public int GetStatusId => _statusId;

        /// <summary>
        /// Gets the credit master id
        /// </summary>
        [Write(false)]
        public Guid GetCreditMasterId => _creditMasterId;

        /// <summary>
        /// Gets the transaction date
        /// </summary>
        [Write(false)]
        public DateTime GetTransactionDate => _transactionDate;

        /// <summary>
        /// Gets the transaction date complete
        /// </summary>
        [Write(false)]
        public DateTime GetTransactionDateComplete => _transactionDate + _transactionTime;

        /// <summary>
        /// Gets the create date
        /// </summary>
        [Write(false)]
        public DateTime GetCreateDate => _createDate;

        /// <summary>
        /// Gets the create time
        /// </summary>
        [Write(false)]
        public TimeSpan GetCreateTime => _createTime;

        /// <summary>
        /// Gets the total down payment
        /// </summary>
        [Write(false)]
        public decimal GetTotalDownPayment => _totalDownPayment;

        /// <summary>
        /// Gets the assurance total fee value
        /// </summary>
        [Write(false)]
        public decimal GetAssuranceTotalFeeValue => _assuranceTotalFeeValue;

        /// <summary>
        /// Gets the total fee value
        /// </summary>
        [Write(false)]
        public decimal GetTotalFeeValue => _feeValue + _assuranceTotalFeeValue;

        /// <summary>
        /// Gets the assurance percentage
        /// </summary>
        [Write(false)]
        public decimal GetAssurancePercentage => _assurancePercentage;

        /// <summary>
        /// Gets the user id
        /// </summary>
        [Write(false)]
        public string GetUserId => _userId;

        /// <summary>
        /// Gets the customer id
        /// </summary>
        [Write(false)]
        public Guid GetCustomerId => _customerId;

        /// <summary>
        /// Gets the status name
        /// </summary>
        [Write(false)]
        public string GetStatusName => _statusName;

        /// <summary>
        /// Gets the source id
        /// </summary>
        [Write(false)]
        public int GetSourceId => _sourceId;

        /// <summary>
        /// Gets the source name
        /// </summary>
        [Write(false)]
        public string GetSourceName => _sourceName;

        /// <summary>
        /// Gets the auth method id
        /// </summary>
        [Write(false)]
        public int GetAuthMethodId => _authMethodId;

        /// <summary>
        /// Gets the auth method name
        /// </summary>
        [Write(false)]
        public string GetAuthMethodName => _authMethodName;

        /// <summary>
        /// Gets the location
        /// </summary>
        [Write(false)]
        public string GetLocation => _location;

        /// <summary>
        /// Gets the assurance total value
        /// </summary>
        [Write(false)]
        public decimal GetAssuranceTotalValue => _assuranceTotalValue;

        /// <summary>
        /// Gets the user name
        /// </summary>
        [Write(false)]
        public string GetUserName => _userName;

        /// <summary>
        /// Gets the credit payment
        /// </summary>
        [Write(false)]
        public CreditPayment CreditPayment { get; private set; }

        /// <summary>
        /// Gets the store
        /// </summary>
        [Write(false)]
        public Store Store { get; private set; }

        /// <summary>
        /// Gets the customer
        /// </summary>
        [Write(false)]
        public Customer Customer { get; private set; }

        /// <summary>
        /// Gets the status
        /// </summary>
        [Write(false)]
        public Status Status { get; private set; }

        /// <summary>
        /// Gets the source
        /// </summary>
        [Write(false)]
        public Source Source { get; private set; }

        /// <summary>
        /// Gets the auth method
        /// </summary>
        [Write(false)]
        public AuthMethod AuthMethod { get; private set; }

        /// <summary>
        /// Gets the credit master
        /// </summary>
        [Write(false)]
        public CreditMaster CreditMaster { get; private set; }

        /// <summary>
        /// Gets the transaction reference
        /// </summary>
        [Write(false)]
        public TransactionReference TransactionReference { get; private set; }

        /// <summary>
        /// Gets the id last payment cancelled
        /// </summary>
        [Write(false)]
        public Guid IdLastPaymentCancelled { get; private set; }

        /// <summary>
        /// Gets the is payment gateway indicator
        /// </summary>
        [Write(false)]
        internal bool IsPaymentGateway => _sourceId == (int)Sources.PaymentGateways;

        /// <summary>
        /// Creates a new instance of <see cref="Credit"/>
        /// </summary>
        protected Credit()
        {
        }

        /// <summary>
        /// New
        /// </summary>
        /// <returns></returns>
        public static Credit New() =>
            new Credit();

        /// <summary>
        /// New credit
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="source"></param>
        /// <param name="creditValue"></param>
        /// <param name="creditNumber"></param>
        /// <param name="userInfo"></param>
        /// <param name="credinetAppSettings"></param>
        internal Credit(CreditMaster creditMaster, Source source, decimal creditValue, long creditNumber, UserInfo userInfo,
            CredinetAppSettings credinetAppSettings)
        {
            SetId(IdentityGenerator.NewSequentialGuid());

            _creditValue = creditValue;
            _creditNumber = creditNumber;

            _balance = creditValue;

            _createDate = DateTime.Today;
            _createTime = DateTime.Now.TimeOfDay;

            _sourceId = source.Id;
            _sourceName = source.Name;

            bool alternatePayment = IsPaymentGateway || IsRefinancing(credinetAppSettings);

            SetCreditMaster(creditMaster);

            SetAlternatePayment(alternatePayment);

            SetArrearsDays(0);
            SetUserInfo(userInfo);

            SetDownPayment(downPayment: 0, totalDownPayment: 0);
        }

        /// <summary>
        /// Is refinancing
        /// </summary>
        /// <param name="credinetAppSettings"></param>
        /// <param name="source"></param>
        /// <param name="increaseCreditLimit"></param>
        public bool IsRefinancing(CredinetAppSettings credinetAppSettings,  int source = 0,bool increaseCreditLimit = false, bool setCreditLimit = true)
        {
            if (increaseCreditLimit && RefinancingParams.IsAllowedSource(credinetAppSettings, _sourceId))
            {
                return false;
            }

            return RefinancingParams.IsAllowedSource(credinetAppSettings, source == 0 ?_sourceId: source, setCreditLimit);
            

        }

        /// <summary>
        /// Gets a value indicating credit is paid
        /// </summary>
        /// <returns></returns>
        public bool IsPaid() => _statusId == (int)Statuses.Paid;

        /// <summary>
        /// Set credit master
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <returns></returns>
        public void SetCreditMaster(CreditMaster creditMaster)
        {
            CreditMaster = creditMaster;
            _creditMasterId = creditMaster.Id;
        }

        /// <summary>
        /// Set customer
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="credinetAppSettings"></param>
        /// <param name="increaseCreditLimit"></param>
        public void SetCustomer(Customer customer, CredinetAppSettings credinetAppSettings , bool increaseCreditLimit= false, bool setCreditLimit = true)
        {
            Customer = customer;
            _customerId = customer.Id;

            Customer.IgnoreCreditLimitUpdate(ignore: IsRefinancing(credinetAppSettings, increaseCreditLimit: increaseCreditLimit, setCreditLimit : setCreditLimit));
        }

        /// <summary>
        /// Set fees info
        /// </summary>
        /// <param name="fees"></param>
        /// <param name="frequency"></param>
        /// <param name="feeValue"></param>
        internal void SetFeesInfo(int fees, int frequency, decimal feeValue)
        {
            _fees = fees;
            _frequency = frequency;
            _feeValue = feeValue;
        }

        /// <summary>
        /// Set store
        /// </summary>
        /// <param name="store"></param>
        public void SetStore(Store store)
        {
            Store = store;
            _storeId = store.Id;
        }

        /// <summary>
        /// Set assurance values
        /// </summary>
        /// <param name="assurancePercentage"></param>
        /// <param name="assuranceValue"></param>
        /// <param name="assuranceFee"></param>
        /// <param name="assuranceTotalFeeValue"></param>
        /// <param name="assuranceTotalValue"></param>
        internal void SetAssuranceValues(decimal assurancePercentage, decimal assuranceValue, decimal assuranceFee, decimal assuranceTotalFeeValue,
            decimal assuranceTotalValue)
        {
            _assurancePercentage = assurancePercentage;
            _assuranceValue = assuranceValue;
            _assuranceFee = assuranceFee;
            _assuranceTotalFeeValue = assuranceTotalFeeValue;
            _assuranceTotalValue = assuranceTotalValue;
            _assuranceBalance = assuranceTotalValue;
        }

        /// <summary>
        /// Set assurance balance
        /// </summary>
        /// <param name="assuranceBalance"></param>
        public void SetAssuranceBalance(decimal assuranceBalance)
        {
            _assuranceBalance = assuranceBalance;
        }

        /// <summary>
        /// Set interest rate
        /// </summary>
        /// <param name="interestRate"></param>
        internal void SetInterestRate(decimal interestRate)
        {
            _interestRate = interestRate == 0 ? _interestRate : interestRate;
        }

        /// <summary>
        /// Set user info
        /// </summary>
        /// <param name="userInfo"></param>
        private void SetUserInfo(UserInfo userInfo)
        {
            _userId = userInfo.UserId;
            _userName = userInfo.UserName;
        }

        /// <summary>
        /// Set seed masters
        /// </summary>
        /// <param name="status"></param>
        /// <param name="authMethod"></param>
        internal void SetSeedMasters(Status status, AuthMethod authMethod)
        {
            _statusId = status.Id;
            _statusName = status.Name;
            _authMethodId = authMethod.Id;
            _authMethodName = authMethod.Name;
        }

        /// <summary>
        /// Set down payment
        /// </summary>
        /// <param name="downPayment"></param>
        /// <param name="totalDownPayment"></param>
        internal void SetDownPayment(decimal downPayment, decimal totalDownPayment)
        {
            _downPayment = downPayment;
            _totalDownPayment = totalDownPayment;
        }

        /// <summary>
        /// Set alternate payment
        /// </summary>
        /// <param name="alternatePayment"></param>
        internal void SetAlternatePayment(bool alternatePayment)
        {
            _alternatePayment = alternatePayment;
        }

        /// <summary>
        /// Set location
        /// </summary>
        /// <param name="location"></param>
        internal void SetLocation(string location)
        {
            _location = location;
        }

        /// <summary>
        /// Init payment
        /// </summary>
        /// <param name="paymentNumber"></param>
        /// <param name="transactionType"></param>
        /// <param name="totalValuePaid"></param>
        /// <param name="lastFee"></param>
        /// <param name="paymentType"></param>
        /// <param name="banckAccount"></param>
        internal void InitPayment(long paymentNumber, TransactionType transactionType, PaymentType paymentType, int lastFee, decimal totalValuePaid = 0,
            string banckAccount = "")
        {
            _createDate = DateTime.Today;
            _createTime = DateTime.Now.TimeOfDay;

            CreditPayment = new CreditPayment(Id, paymentNumber, transactionType, paymentType, lastFee, totalValuePaid, banckAccount);

            CreditPayment.SetAdjustmentValues(previousArrears: 0, previousInterest: 0, activeFeeValuePaid: 0);

            SetPaymentValues();
        }

        /// <summary>
        /// Set payment values
        /// </summary>
        /// <param name="creditValuePaid"></param>
        /// <param name="interestValuePaid"></param>
        /// <param name="chargeValuePaid"></param>
        /// <param name="arrearsValuePaid"></param>
        /// <param name="assuranceValuePaid"></param>
        internal void SetPaymentValues(decimal creditValuePaid = 0, decimal interestValuePaid = 0, decimal chargeValuePaid = 0,
            decimal arrearsValuePaid = 0, decimal assuranceValuePaid = 0)
        {
            _balance -= creditValuePaid;
            _assuranceBalance -= IsPaid() ? _assuranceBalance : assuranceValuePaid;

            CreditPayment.SetPaymentValues(creditValuePaid, interestValuePaid, chargeValuePaid, arrearsValuePaid, assuranceValuePaid);
        }

        /// <summary>
        /// Set payment info
        /// Set payment info
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="arrearsDays"></param>
        /// <param name="interestRate"></param>
        /// <param name="status"></param>
        /// <param name="store"></param>
        /// <param name="location"></param>
        internal void SetPaymentInfo(UserInfo userInfo, Status status, Store store, string location, int arrearsDays,
            decimal interestRate = 0)
        {
            SetUserInfo(userInfo);
            SetArrearsDays(arrearsDays);
            SetStatus(status);
            SetLocation(location);
            SetInterestRate(interestRate);

            SetStore(store);
        }

        /// <summary>
        /// Set calculation dates
        /// </summary>
        /// <param name="dueDate"></param>
        /// <param name="calculationDate"></param>
        internal void SetCalculationDates(DateTime dueDate, DateTime calculationDate)
        {
            CreditPayment.SetDueDate(dueDate.Date);
            CreditPayment.SetCalculationDate(calculationDate.Date);

            SetTransactionDate(calculationDate);
            SetLastPaymentDate(calculationDate);
        }

        /// <summary>
        /// Set last payment date
        /// </summary>
        /// <param name="lastPaymentDate"></param>
        internal void SetLastPaymentDate(DateTime lastPaymentDate)
        {
            CreditPayment.SetLastPaymentDate(lastPaymentDate);
        }

        /// <summary>
        /// Set adjustment payment values
        /// </summary>
        /// <param name="previousArrears"></param>
        /// <param name="previousInterest"></param>
        /// <param name="activeFeeValuePaid"></param>
        internal void SetAdjustmentPaymentValues(decimal previousArrears = 0, decimal previousInterest = 0, decimal activeFeeValuePaid = 0)
        {
            CreditPayment.SetAdjustmentValues(previousArrears, previousInterest, activeFeeValuePaid);
        }

        /// <summary>
        /// Set arrears days
        /// </summary>
        /// <param name="arrearsDays"></param>
        /// <returns></returns>
        public void SetArrearsDays(int arrearsDays)
        {
            _arrearsDays = arrearsDays;
        }

        /// <summary>
        /// Set transaction date with specific date
        /// </summary>
        /// <param name="date"></param>
        internal void SetTransactionDate(DateTime date)
        {
            _transactionDate = date.Date;
            _transactionTime = date.TimeOfDay;
        }

        /// <summary>
        /// Set credit payment
        /// </summary>
        /// <param name="creditPayment"></param>
        public void SetCreditPayment(CreditPayment creditPayment)
        {
            CreditPayment = creditPayment;
        }

        /// <summary>
        /// Clone credit payment
        /// </summary>
        internal void CloneCreditPayment()
        {
            SetCreditPayment(((CreditPayment)CreditPayment.Clone()).SetId(Id));
        }

        /// <summary>
        /// Update Charge Payment Plan Value
        /// </summary>
        /// <param name="charges"></param>
        /// <param name="hasArrearsCharge"></param>
        /// <param name="arrearsCharges"></param>
        /// <param name="updatedPaymentPlanValue"></param>
        /// <param name="transactionType"></param>
        internal void UpdateChargesPaymentPlanValue(decimal charges, bool hasArrearsCharge, decimal arrearsCharges,
            decimal updatedPaymentPlanValue, TransactionType transactionType)
        {
            _chargeValue = charges;
            _hasArrearsCharge = hasArrearsCharge;
            _arrearsCharge = arrearsCharges;
            _updatedPaymentPlanValue = updatedPaymentPlanValue;

            CloneCreditPayment();

            CreditPayment.SetTransactionType(transactionType);

            SetTransactionDate(DateTime.Now);
        }

        /// <summary>
        /// Credit cancel
        /// </summary>
        /// <param name="cancelTransactionType"></param>
        /// <param name="canceledStatus"></param>
        /// <param name="userName"></param>
        /// <param name="userId"></param>
        public void CreditCancel(TransactionType cancelTransactionType, Status canceledStatus, string userName, string userId)
        {
            _balance = 0;

            _userId = userId;
            _userName = userName;

            SetStatus(canceledStatus);

            CloneCreditPayment();

            CreditPayment.SetTransactionType(cancelTransactionType);

            SetTransactionDate(DateTime.Now);
        }

        /// <summary>
        /// Cancel payment
        /// </summary>
        /// <param name="transactionType"></param>
        /// <param name="status"></param>
        /// <param name="userName"></param>
        /// <param name="userId"></param>
        public void CancelPayment(TransactionType transactionType, Status status, string userName, string userId)
        {
            _userId = userId;
            _userName = userName;

            CloneCreditPayment();

            CreditPayment.SetTransactionType(transactionType);

            SetStatus(status);

            SetTransactionDate(DateTime.Now);
        }

        /// <summary>
        /// Has update payment plan
        /// </summary>
        /// <returns></returns>
        public bool HasUpdatedPaymentPlan() => _updatedPaymentPlanValue != null && _updatedPaymentPlanValue > 0;

        /// <summary>
        /// Has down payment
        /// </summary>
        /// <returns></returns>
        public bool HasDownPayment() => _downPayment > 0;

        /// <summary>
        /// Set status
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public Credit SetStatus(Status status)
        {
            _statusId = status.Id;
            _statusName = status.Name;
            return this;
        }

        /// <summary>
        /// Has charges
        /// </summary>
        /// <returns></returns>
        public bool HasCharges() => (_chargeValue != null && _chargeValue > 0) || _hasArrearsCharge;

        /// <summary>
        /// Has cancel request
        /// </summary>
        /// <param name="allRequestCancelPaymentsUndismissed"></param>
        /// <returns></returns>

        internal bool HasCancelRequest(List<RequestCancelPayment> allRequestCancelPaymentsUndismissed) =>
            allRequestCancelPaymentsUndismissed
                .Any(requestCancel => requestCancel.GetCreditMasterId == _creditMasterId
                    &&
                    requestCancel.GetCreditId == Id);

        /// <summary>
        /// Allows request cancellation
        /// </summary>
        /// <param name="maximumDaysRequestCancellationPayments"></param>
        /// <returns></returns>
        public bool AllowsRequestCancellation(int maximumDaysRequestCancellationPayments) =>
            _transactionDate >= DateTime.Today.Subtract(TimeSpan.FromDays(maximumDaysRequestCancellationPayments));

        /// <summary>
        /// Set transaction reference
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public void SetTransactionReference(string transactionId)
        {
            if (!string.IsNullOrEmpty(transactionId))
            {
                TransactionReference = new TransactionReference(transactionId, Id);
            }
        }

        /// <summary>
        /// Set id last payment cancelled
        /// </summary>
        /// <param name="idLastPaymentCancelled"></param>
        public void SetIdLastPaymentCancelled(Guid idLastPaymentCancelled)
        {
            IdLastPaymentCancelled = idLastPaymentCancelled;    
        }
    }
}