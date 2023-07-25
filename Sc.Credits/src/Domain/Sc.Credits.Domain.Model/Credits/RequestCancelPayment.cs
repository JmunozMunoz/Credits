using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Helpers.Commons.Dapper.Annotations;
using Sc.Credits.Helpers.Commons.Domain;
using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// Request cancel payment
    /// </summary>
    public class RequestCancelPayment : Entity<Guid>, IAggregateRoot
    {
        private Guid _creditMasterId;
        private Guid _creditId;
        private string _userName;
        private DateTime _date;
        private TimeSpan _time;
        private string _storeId;
        private DateTime? _processDate;
        private TimeSpan? _processTime;
        private string _reason;
        private int _requestStatusId;
        private Guid? _creditCancelId;
        private string _userId;
        private string _processUserId;
        private string _processUserName;

        /// <summary>
        /// Store
        /// </summary>
        [Write(false)]
        public Store Store { get; private set; }

        /// <summary>
        /// Credit master
        /// </summary>
        [Write(false)]
        public CreditMaster CreditMaster { get; private set; }

        /// <summary>
        /// Payment
        /// </summary>
        [Write(false)]
        public Credit Payment { get; private set; }

        /// <summary>
        /// Canceled payment
        /// </summary>
        [Write(false)]
        public Credit CanceledPayment { get; private set; }

        /// <summary>
        /// Request status
        /// </summary>
        [Write(false)]
        public RequestStatus RequestStatus { get; private set; }

        /// <summary>
        /// RGet quest status id
        /// </summary>
        [Write(false)]
        public int GetRequestStatusId => _requestStatusId;

        /// <summary>
        /// Get date
        /// </summary>
        [Write(false)]
        public DateTime GetDate => _date;

        /// <summary>
        /// Get date complete
        /// </summary>
        [Write(false)]
        public DateTime GetDateComplete => _date + _time;

        /// <summary>
        /// Get credit master id
        /// </summary>
        [Write(false)]
        public Guid GetCreditMasterId => _creditMasterId;

        /// <summary>
        /// Get credit id
        /// </summary>
        [Write(false)]
        public Guid GetCreditId => _creditId;

        /// <summary>
        /// Get credit cancel id
        /// </summary>
        [Write(false)]
        public Guid? GetCreditCancelId => _creditCancelId;

        /// <summary>
        /// Get user name
        /// </summary>
        [Write(false)]
        public string GetUserName => _userName;

        /// <summary>
        /// Get user id
        /// </summary>
        [Write(false)]
        public string GetUserId => _userId;

        /// <summary>
        /// Get store id
        /// </summary>
        [Write(false)]
        public string GetStoreId => _storeId;

        /// <summary>
        /// Get reason
        /// </summary>
        [Write(false)]
        public string GetReason => _reason;

        /// <summary>
        /// Get process date
        /// </summary>
        [Write(false)]
        public DateTime? GetProcessDate => _processDate;

        /// <summary>
        /// Get process date complete
        /// </summary>
        [Write(false)]
        public DateTime? GetProcessDateComplete => _processDate + _processTime;

        /// <summary>
        /// Get time
        /// </summary>
        [Write(false)]
        public TimeSpan GetTime => _time;

        /// <summary>
        /// Get process user id
        /// </summary>
        [Write(false)]
        public string GetProcessUserId => _processUserId;

        /// <summary>
        /// Get process user name
        /// </summary>
        [Write(false)]
        public string GetProcessUserName => _processUserName;

        /// <summary>
        /// New request cancel payment
        /// </summary>
        protected RequestCancelPayment()
        {
        }

        /// <summary>
        /// New request cancel payment
        /// </summary>
        /// <param name="creditId"></param>
        /// <param name="creditMasterId"></param>
        /// <param name="userName"></param>
        /// <param name="storeId"></param>
        /// <param name="reason"></param>
        /// <param name="requestStatusId"></param>
        /// <param name="userId"></param>
        public RequestCancelPayment(Guid creditId, Guid creditMasterId, string userName, string storeId, string reason, int requestStatusId, string userId)
        {
            Id = IdentityGenerator.NewSequentialGuid();
            _creditId = creditId;
            _creditMasterId = creditMasterId;
            _userName = userName;
            _date = DateTime.Today;
            _time = DateTime.Now.TimeOfDay;
            _storeId = storeId;
            _reason = reason;
            _requestStatusId = requestStatusId;
            _userId = userId;
        }

        /// <summary>
        /// New
        /// </summary>
        /// <returns></returns>
        public static RequestCancelPayment New() =>
            new RequestCancelPayment();

        /// <summary>
        /// Set id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public new RequestCancelPayment SetId(Guid id)
        {
            base.SetId(id);
            return this;
        }

        /// <summary>
        /// Set canceled
        /// </summary>
        /// <param name="cancelPayment"></param>
        /// <param name="userName"></param>
        /// <param name="userId"></param>
        public void SetCanceled(Credit cancelPayment, string userName, string userId)
        {
            _requestStatusId = (int)RequestStatuses.Cancel;
            _creditCancelId = cancelPayment.Id;
            CanceledPayment = cancelPayment;

            SetProcessDate(DateTime.Now);

            _processUserName = userName;
            _processUserId = userId;
        }

        /// <summary>
        /// Update request status
        /// </summary>
        /// <param name="requestStatusId"></param>
        /// <param name="userName"></param>
        /// <param name="userId"></param>
        public void UpdateStatus(int requestStatusId, string userName, string userId)
        {
            SetProcessDate(DateTime.Now);

            _requestStatusId = requestStatusId;

            _processUserName = userName;
            _processUserId = userId;
        }

        /// <summary>
        /// Set credit master
        /// </summary>
        /// <param name="creditMaster"></param>
        public RequestCancelPayment SetCreditMaster(CreditMaster creditMaster)
        {
            CreditMaster = creditMaster;
            return this;
        }

        /// <summary>
        /// Set payment
        /// </summary>
        /// <param name="payment"></param>
        public RequestCancelPayment SetPayment(Credit payment)
        {
            Payment = payment;
            return this;
        }

        /// <summary>
        /// Set process date
        /// </summary>
        /// <param name="date"></param>
        public RequestCancelPayment SetProcessDate(DateTime date)
        {
            _processDate = date.Date;
            _processTime = date.TimeOfDay;
            return this;
        }

        /// <summary>
        /// Set request status
        /// </summary>
        /// <param name="requestStatus"></param>
        /// <returns></returns>
        public RequestCancelPayment SetRequestStatus(RequestStatus requestStatus)
        {
            RequestStatus = requestStatus;
            return this;
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
    }
}