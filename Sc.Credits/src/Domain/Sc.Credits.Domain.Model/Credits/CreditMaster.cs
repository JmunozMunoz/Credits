using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Refinancings;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Helpers.Commons.Dapper.Annotations;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// Credit master entity
    /// </summary>
    public class CreditMaster
        : Master<Credit>
    {
        private Guid _customerId;
        private string _storeId;
        private int _statusId;
        private string _scCode;
        private string _seller;
        private string _products;
        private string _invoice;
        private long _creditNumber;
        private string _reason;
        private DateTime _creditDate;
        private TimeSpan _creditTime;
        private string _certifyingAuthority;
        private string _certifiedId;
        private decimal _effectiveAnnualRate;
        private string _promissoryNoteFileName;
        private string _token;
        private string _riskLevel;

        /// <summary>
        /// Gets the credit number
        /// </summary>
        [Write(false)]
        public long GetCreditNumber => _creditNumber;

        /// <summary>
        /// Gets the credit seller
        /// </summary>
        [Write(false)]
        public string GetCreditSeller => _seller;

        /// <summary>
        /// Gets the credit invoice
        /// </summary>
        [Write(false)]
        public string GetCreditInvoice => _invoice;

        /// <summary>
        /// Gets the credit products
        /// </summary>
        [Write(false)]
        public string GetCreditProducts => _products;

        /// <summary>
        /// Gets the status id
        /// </summary>
        [Write(false)]
        public int GetStatusId => _statusId;

        /// <summary>
        /// Gets the effective annual rate
        /// </summary>
        [Write(false)]
        public decimal GetEffectiveAnnualRate => _effectiveAnnualRate;

        /// <summary>
        /// Gets the credit date
        /// </summary>
        [Write(false)]
        public DateTime GetCreditDate => _creditDate;

        /// <summary>
        /// Gets the credit date complete
        /// </summary>
        [Write(false)]
        public DateTime GetCreditDateComplete => _creditDate + _creditTime;

        /// <summary>
        /// Gets the customer's id
        /// </summary>
        [Write(false)]
        public Guid GetCustomerId => _customerId;

        /// <summary>
        /// Gets the store's id
        /// </summary>
        [Write(false)]
        public string GetStoreId => _storeId;

        /// <summary>
        /// Gets the credit's code on legacy system
        /// </summary>
        [Write(false)]
        public string GetScCode => _scCode;

        /// <summary>
        /// Gets the promissory note filename
        /// </summary>
        [Write(false)]
        public string GetPromissoryNoteFileName => _promissoryNoteFileName;

        /// <summary>
        /// Gets the reason
        /// </summary>
        [Write(false)]
        public string GetReason => _reason;

        /// <summary>
        /// Gets the certifying authority
        /// </summary>
        [Write(false)]
        public string GetCertifyingAuthority => _certifyingAuthority;

        /// <summary>
        /// Gets the certified id
        /// </summary>
        [Write(false)]
        public string GetCertifiedId => _certifiedId;

        /// <summary>
        /// Gets the token
        /// </summary>
        [Write(false)]
        public string GetToken => _token;

        /// <summary>
        /// Gets the risk level
        /// </summary>
        [Write(false)]
        public string GetRiskLevel => _riskLevel;

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
        /// Gets the store
        /// </summary>
        [Write(false)]
        public Store Store { get; private set; }

        /// <summary>
        /// Gets the is alternate payment
        /// </summary>
        [Write(false)]
        public bool IsAlternatePayment => Current.GetAlternatePayment;

        /// <summary>
        /// Creates a new instance of <see cref="CreditMaster"/>
        /// </summary>
        protected CreditMaster()
            : base()
        {
        }

        /// <summary>
        /// Creates new credit master
        /// </summary>
        /// <returns></returns>
        public static CreditMaster New()
        {
            return new CreditMaster();
        }

        /// <summary>
        /// New credit master
        /// </summary>
        /// <param name="source"></param>
        /// <param name="customer"></param>
        /// <param name="creditValue"></param>
        /// <param name="creditNumber"></param>
        /// <param name="userInfo"></param>
        /// <param name="credinetAppSettings"></param>
        public CreditMaster(Source source, Customer customer, decimal creditValue, long creditNumber, UserInfo userInfo,
            CredinetAppSettings credinetAppSettings, bool setCreditLimit = true)
               : base()
        {
            SetId(NewId());

            _creditNumber = creditNumber;
            _reason = string.Empty;

            Credit newCredit = new Credit(this, source, creditValue, creditNumber, userInfo, credinetAppSettings);

            CreateChild(newCredit);

            SetCustomer(customer, credinetAppSettings, setCreditLimit);

            customer.CreditLimitDecrease(creditValue);

            SetCreditDate(DateTime.Now);

            SetScCode(string.Empty);
        }

        /// <summary>
        /// Set id
        /// </summary>
        /// <param name="id"></param>
        public new CreditMaster SetId(Guid id)
        {
            base.SetId(id);
            return this;
        }

        /// <summary>
        /// Set rates
        /// </summary>
        /// <param name="interestRate"></param>
        /// <param name="effectiveAnnualRate"></param>
        internal void SetRates(decimal interestRate, decimal effectiveAnnualRate)
        {
            _effectiveAnnualRate = effectiveAnnualRate;
            Current.SetInterestRate(interestRate);
        }

        /// <summary>
        /// Set customer
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="credinetAppSettings"></param>
        /// <returns></returns>
        public CreditMaster SetCustomer(Customer customer, CredinetAppSettings credinetAppSettings, bool setCreditLimit = true)
        {
            Customer = customer;
            _customerId = customer.Id;

            if (Current != null)
            {
                Current.SetCustomer(customer, credinetAppSettings, setCreditLimit : setCreditLimit);
            }

            SetUpdated();

            return this;
        }

        /// <summary>
        /// Set customer credit limit update
        /// </summary>
        /// <param name="credinetAppSettings"></param>
        /// <param name="source"></param>
        /// <param name="increaseCreditLimit"></param>
        internal void SetCustomerCreditLimitUpdate(CredinetAppSettings credinetAppSettings, int source = 0, bool increaseCreditLimit = false, bool setCreditLimit = true)
        {
            Customer.IgnoreCreditLimitUpdate(ignore:Current.IsRefinancing(credinetAppSettings, source, increaseCreditLimit, setCreditLimit));
        }

        /// <summary>
        /// Set credit date
        /// </summary>
        /// <param name="creditDate"></param>
        internal void SetCreditDate(DateTime creditDate)
        {
            _creditDate = creditDate.Date;
            _creditTime = creditDate.TimeOfDay;
        }

        /// <summary>
        /// Set store
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        public CreditMaster SetStore(Store store)
        {
            Store = store;
            _storeId = store.Id;

            if (Current != null)
            {
                Current.SetStore(store);
            }

            SetUpdated();

            return this;
        }

        /// <summary>
        /// Set fees info
        /// </summary>
        /// <param name="fees"></param>
        /// <param name="frequency"></param>
        /// <param name="feeValue"></param>
        internal void SetFeesInfo(int fees, int frequency, decimal feeValue)
        {
            Current.SetFeesInfo(fees, frequency, feeValue);
        }

        /// <summary>
        /// Set down payment
        /// </summary>
        /// <param name="downPayment"></param>
        /// <param name="totalDownPayment"></param>
        internal void SetDownPayment(decimal downPayment, decimal totalDownPayment)
        {
            Current.SetDownPayment(downPayment, totalDownPayment);
        }

        /// <summary>
        /// Set seed masters
        /// </summary>
        /// <param name="status"></param>
        /// <param name="authMethod"></param>
        internal void SetSeedMasters(Status status, AuthMethod authMethod)
        {
            SetStatus(status);
            Current.SetSeedMasters(status, authMethod);
        }

        /// <summary>
        /// Set status
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public CreditMaster SetStatus(Status status)
        {
            Status = status;
            SetStatusId(status.Id);

            SetUpdated();

            return this;
        }

        /// <summary>
        /// Set initial payment
        /// </summary>
        /// <param name="transactionType"></param>
        /// <param name="paymentType"></param>
        /// <param name="dueDate"></param>
        /// <param name="calculationDate"></param>
        /// <param name="banckAccount"></param>
        internal void SetInitialPayment(TransactionType transactionType, PaymentType paymentType, DateTime dueDate, DateTime calculationDate,
            string banckAccount = "")
        {
            Current.InitPayment(paymentNumber: 0, transactionType, paymentType, lastFee: 0, totalValuePaid: 0, banckAccount);

            Current.SetCalculationDates(dueDate, calculationDate);
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
            Current.SetAssuranceValues(assurancePercentage, assuranceValue, assuranceFee, assuranceTotalFeeValue, assuranceTotalValue);
            Current.SetAssuranceBalance(assuranceTotalValue);
        }

        /// <summary>
        /// Set seller info
        /// </summary>
        /// <param name="seller"></param>
        /// <param name="products"></param>
        /// <param name="invoice"></param>
        public void SetSellerInfo(string seller, string products, string invoice)
        {
            _invoice = invoice;
            _products = products;
            _seller = seller;
            SetUpdated();
        }

        /// <summary>
        /// Set location
        /// </summary>
        /// <param name="location"></param>
        internal void SetLocation(string location)
        {
            Current.SetLocation(location);
        }

        /// <summary>
        /// Set token
        /// </summary>
        /// <param name="token"></param>
        internal void SetToken(string token)
        {
            _token = token;
        }

        /// <summary>
        /// Set risk level
        /// </summary>
        /// <param name="riskLevel"></param>
        internal void SetRiskLevel(string riskLevel)
        {
            _riskLevel = riskLevel;
        }

        /// <summary>
        /// Set CreditId
        /// </summary>
        /// <param name="id"></param>
        public CreditMaster SetCreditId(Guid id)
        {
            Current.SetId(id);
            SetLastId(id);
            return this;
        }

        /// <summary>
        /// Update ScCode
        /// </summary>
        /// <param name="scCode"></param>
        public void SetScCode(string scCode)
        {
            _scCode = scCode;

            SetUpdated();
        }

        /// <summary>
        /// Update Status
        /// </summary>
        /// <param name="statusId"></param>
        public void SetStatusId(int statusId)
        {
            _statusId = statusId;

            SetUpdated();
        }

        /// <summary>
        /// Set promissory note filename
        /// </summary>
        /// <param name="fileName"></param>
        public void SetPromissoryNoteFileName(string fileName)
        {
            _promissoryNoteFileName = fileName;

            SetUpdated();
        }

        /// <summary>
        /// Set certified
        /// </summary>
        /// <param name="certifyingAuthority"></param>
        /// <param name="certifiedId"></param>
        public void SetCertified(string certifyingAuthority, string certifiedId)
        {
            _certifyingAuthority = certifyingAuthority;
            _certifiedId = certifiedId;

            SetUpdated();
        }

        /// <summary>
        /// Set last record
        /// </summary>
        /// <param name="record"></param>
        /// <param name="transactionType"></param>
        public void SetLastRecord(Credit record, TransactionType transactionType)
        {
            CloneRecordAndSetLast(record);
            Current.CloneCreditPayment();
            Current.CreditPayment.SetTransactionType(transactionType);
        }

        /// <summary>
        /// Set history
        /// </summary>
        /// <param name="masterTransactions"></param>
        /// <returns></returns>
        public CreditMaster SetHistory(IEnumerable<Credit> masterTransactions)
        {
            masterTransactions =
                masterTransactions.Select(t =>
                    {
                        t.SetCreditMaster(this);
                        return t;
                    });

            History = new List<Credit>(masterTransactions);

            return this;
        }

        /// <summary>
        /// Get payment transactions
        /// </summary>
        /// <returns></returns>
        public List<Credit> GetPaymentTransactions() =>
            History
                .Where(credit =>
                    (TransactionTypes)credit.CreditPayment.GetTransactionTypeId == TransactionTypes.Payment).ToList();

        /// <summary>
        /// Get payment transactions not canceled
        /// </summary>
        /// <returns></returns>
        public List<Credit> GetPaymentTransactionsNotCanceled(List<RequestCancelPayment> requestCancelPayments) =>
            GetPaymentTransactions()
                .Where(payment =>
                    !requestCancelPayments
                        .Any(requestCancelPayment => (RequestStatuses)requestCancelPayment.GetRequestStatusId == RequestStatuses.Cancel
                            &&
                            requestCancelPayment.GetCreditId == payment.Id
                            &&
                            requestCancelPayment.GetCreditMasterId == payment.GetCreditMasterId)).ToList();

        /// <summary>
        /// Get payment transactions not canceled in store and maximum previous days
        /// </summary>
        /// <param name="requestCancelPayments"></param>
        /// <param name="storeId"></param>
        /// <param name="maximumDaysRequestCancellationPayments"></param>
        /// <returns></returns>
        public List<Credit> GetPaymentTransactionsNotCanceled(List<RequestCancelPayment> requestCancelPayments, string storeId,
            int maximumDaysRequestCancellationPayments) =>
             GetPaymentTransactionsNotCanceled(requestCancelPayments)
                .Where(credit =>
                        credit.AllowsRequestCancellation(maximumDaysRequestCancellationPayments)
                        &&
                        credit.GetStoreId == storeId)
                .OrderBy(x => x.GetCreditMasterId)
                .ThenByDescending(x => x.GetTransactionDateComplete).ToList();


        /// <summary>
        /// Get cancelable payment
        /// </summary>
        /// <param name="requestCancelPayments"></param>
        /// <returns></returns>
        public Credit GetCancelablePayment(List<RequestCancelPayment> requestCancelPayments) =>
            History
                .Where(credit =>
                    (TransactionTypes)credit.CreditPayment.GetTransactionTypeId == TransactionTypes.Payment
                    &&
                    !requestCancelPayments
                        .Any(requestCancelPayment => (RequestStatuses)requestCancelPayment.GetRequestStatusId != RequestStatuses.Dismissed
                            &&
                            requestCancelPayment.GetCreditId == credit.Id
                            &&
                            requestCancelPayment.GetCreditMasterId == credit.GetCreditMasterId))
                .OrderByDescending(t => t.GetTransactionDateComplete)
                .FirstOrDefault();

        /// <summary>
        /// Has payments
        /// </summary>
        /// <param name="requestCancelPaymentsCanceled"></param>
        /// <returns></returns>
        public bool HasPayments(List<RequestCancelPayment> requestCancelPaymentsCanceled) =>
            GetPaymentTransactionsNotCanceled(requestCancelPaymentsCanceled)
                .Any();

        /// <summary>
        /// Get last payment without cancel
        /// </summary>
        /// <param name="requestCanceleds"></param>
        /// <param name="allRequestCancels"></param>
        /// <returns></returns>
        public Credit GetLastPaymentWithoutCancel(List<RequestCancelPayment> requestCanceleds, List<RequestCancelPayment> allRequestCancels)
        {
            Credit lastCanceledPayment = requestCanceleds.OrderByDescending(request => request.Payment.GetTransactionDateComplete).Last().Payment;

            return
                History
                    .Where(credit =>
                        credit.GetTransactionDateComplete < lastCanceledPayment.GetTransactionDateComplete
                        &&
                        (credit.CreditPayment.GetTransactionTypeId == (int)TransactionTypes.CreateCredit
                        ||
                        credit.CreditPayment.GetTransactionTypeId == (int)TransactionTypes.Payment)
                        &&
                        !allRequestCancels.Any(r =>
                            credit.CreditPayment.GetTransactionTypeId == (int)TransactionTypes.Payment
                            &&
                            r.GetCreditId == credit.Id
                            &&
                            r.GetCreditMasterId == credit.GetCreditMasterId))
                    .OrderByDescending(credit => credit.GetTransactionDateComplete)
                    .First();
        }

        /// <summary>
        /// Get payment
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        public Credit GetPayment(Guid paymentId) =>
            History.FirstOrDefault(c => c.Id == paymentId && c.CreditPayment.GetTransactionTypeId == (int)TransactionTypes.Payment);

        /// <summary>
        /// Is active
        /// </summary>
        /// <returns></returns>
        public bool IsActive() =>
            GetStatusId == (int)Statuses.Active;

        /// <summary>
        /// Is active or cancel request
        /// </summary>
        /// <returns></returns>
        public bool IsActiveOrCancelRequest() =>
            IsActive() || GetStatusId == (int)Statuses.CancelRequest;

        /// <summary>
        /// Is refinancing allowed
        /// </summary>
        /// <param name="allowRefinancingCredits"></param>
        /// <param name="credinetAppSettings"></param>
        /// <returns></returns>
        public bool IsRefinancingAllowed(bool allowRefinancingCredits, CredinetAppSettings credinetAppSettings)
            =>
            (!allowRefinancingCredits && !Current.IsRefinancing(credinetAppSettings)) || allowRefinancingCredits;

        /// <summary>
        /// Paid
        /// </summary>
        /// <returns></returns>
        public bool Paid() =>
            _statusId == (int)Statuses.Paid;

        /// <summary>
        /// Validate date of refinanced credits
        /// </summary>
        /// <param name="credinetAppSettings"></param>
        /// <returns></returns>
        public bool ValidateDateOfRefinancedCredits(CredinetAppSettings credinetAppSettings, bool setCreditLimit = true)
        {
            return this.CreateDate >= credinetAppSettings.RefinancingDate && setCreditLimit;
        }
    }
}