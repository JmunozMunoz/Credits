using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Helpers.Commons.Dapper.Annotations;
using Sc.Credits.Helpers.Commons.Domain;
using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// Request cancel credit
    /// </summary>
    public class RequestCancelCredit
        : Entity<Guid>, IAggregateRoot
    {
        private Guid _creditMasterId;
        private string _userName;
        private DateTime _date;
        private TimeSpan _time;
        private string _storeId;
        private DateTime? _processDate;
        private TimeSpan? _processTime;
        private string _reason;
        private int _requestStatusId;
        private string _userId;
        private string _processUserId;
        private string _processUserName;
        private decimal? _valueCancel;
        private int _cancellationType;

        /// <summary>
        /// Store
        /// </summary>
        [Write(false)]
        public Store Store { get; private set; }

        /// <summary>
        /// Get date
        /// </summary>
        [Write(false)]
        public DateTime GetDate => _date;

        /// <summary>
        /// Get reason
        /// </summary>
        [Write(false)]
        public string GetReason => _reason;

        /// <summary>
        /// Get credit master id
        /// </summary>
        [Write(false)]
        public Guid GetCreditMasterId => _creditMasterId;

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
        /// Get request status id
        /// </summary>
        [Write(false)]
        public int GetRequestStatusId => _requestStatusId;

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
        /// Get store id
        /// </summary>
        [Write(false)]
        public string GetStoreId => _storeId;

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
        /// Get value cancel
        /// </summary>
        [Write(false)]
        public decimal? GetValueCancel => _valueCancel;

        /// <summary>
        /// Get cancellation type
        /// </summary>
        [Write(false)]
        public int GetCancellationType => _cancellationType;

        /// <summary>
        /// Credit master
        /// </summary>
        [Write(false)]
        public CreditMaster CreditMaster { get; private set; }

        /// <summary>
        /// New request cancel credit
        /// </summary>
        protected RequestCancelCredit()
        {
        }

        /// <summary>
        /// New request cancel credit
        /// </summary>
        /// <param name="creditMasterId"></param>
        /// <param name="userName"></param>
        /// <param name="storeId"></param>
        /// <param name="reason"></param>
        /// <param name="requestStatusId"></param>
        /// <param name="userId"></param>
        /// <param name="cancellationType"></param>
        /// <param name="valueCancel"></param>
        public RequestCancelCredit(Guid creditMasterId, string userName, string storeId, string reason, int requestStatusId, string userId, int cancellationType, decimal? valueCancel)
        {
            Id = IdentityGenerator.NewSequentialGuid();
            _creditMasterId = creditMasterId;
            _userName = userName;
            _date = DateTime.Today;
            _time = DateTime.Now.TimeOfDay;
            _storeId = storeId;
            _reason = reason;
            _requestStatusId = requestStatusId;
            _userId = userId;
            _cancellationType = cancellationType;
            _valueCancel = valueCancel;
        }

        /// <summary>
        /// New
        /// </summary>
        /// <returns></returns>
        public static RequestCancelCredit New() =>
            new RequestCancelCredit();

        /// <summary>
        /// Set id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public new RequestCancelCredit SetId(Guid id)
        {
            base.SetId(id);
            return this;
        }

        /// <summary>
        /// Set store
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        public RequestCancelCredit SetStore(Store store)
        {
            Store = store;
            _storeId = store.Id;
            return this;
        }

        /// <summary>
        /// Update request status
        /// </summary>
        /// <param name="requestStatusId"></param>
        /// <param name="userName"></param>
        /// <param name="userId"></param>
        public void UpdateStatus(int requestStatusId, string userName, string userId)
        {
            _requestStatusId = requestStatusId;

            SetProcessDate(DateTime.Now);

            _processUserName = userName;
            _processUserId = userId;
        }

        /// <summary>
        /// Set canceled
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userId"></param>
        public void SetCanceled(string userName, string userId)
        {
            _requestStatusId = (int)RequestStatuses.Cancel;

            SetProcessDate(DateTime.Now);

            _processUserName = userName;
            _processUserId = userId;
        }

        /// <summary>
        /// Set credit master
        /// </summary>
        /// <param name="creditMaster"></param>
        public RequestCancelCredit SetCreditMaster(CreditMaster creditMaster)
        {
            CreditMaster = creditMaster;
            return this;
        }

        /// <summary>
        /// Set process date
        /// </summary>
        /// <param name="date"></param>
        public RequestCancelCredit SetProcessDate(DateTime date)
        {
            _processDate = date.Date;
            _processTime = date.TimeOfDay;
            return this;
        }
    }
}