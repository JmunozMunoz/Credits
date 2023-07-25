using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Helpers.Commons.Dapper.Annotations;
using Sc.Credits.Helpers.Commons.Domain;
using System;
using System.Collections.Generic;

namespace Sc.Credits.Domain.Model.Refinancings
{
    /// <summary>
    /// Refinancing log
    /// </summary>
    public class RefinancingLog : Entity<Guid>, IAggregateRoot
    {
        private DateTime _date;
        private Guid _applicationId;
        private Guid _creditMasterId;
        private decimal _value;
        private string _storeId;
        private Guid _customerId;
        private string _referenceText;
        private string _referenceCode;
        private string _userId;
        private string _userName;



        /// <summary>
        /// Date
        /// </summary>
        [Write(false)]
        public DateTime GetDate => _date;

        /// <summary>
        /// Application id
        /// </summary>
        [Write(false)]
        public Guid GetApplicationId => _applicationId;

        /// <summary>
        /// Credit master id
        /// </summary>
        [Write(false)]
        public Guid GetCreditMasterId => _creditMasterId;

        /// <summary>
        /// Value
        /// </summary>
        [Write(false)]
        public decimal GetValue => _value;

        /// <summary>
        /// Store id
        /// </summary>
        [Write(false)]
        public string GetStoreId => _storeId;

        /// <summary>
        /// Customer id
        /// </summary>
        [Write(false)]
        public Guid GetCustomerId => _customerId;

        /// <summary>
        /// Reference text
        /// </summary>
        [Write(false)]
        public string GetReferenceText => _referenceText;

        /// <summary>
        /// Reference code
        /// </summary>
        [Write(false)]
        public string GetReferenceCode => _referenceCode;

        /// <summary>
        /// User id
        /// </summary>
        [Write(false)]
        public string GetUserId => _userId;

        /// <summary>
        /// User name
        /// </summary>
        [Write(false)]
        public string GetUserName => _userName;

        /// <summary>
        /// Details
        /// </summary>
        [Write(false)]
        public List<RefinancingLogDetail> Details { get; private set; }

        /// <summary>
        /// New refinancing log
        /// </summary>
        protected RefinancingLog()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="RefinancingLog"/>
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="creditMasterId"></param>
        /// <param name="value"></param>
        /// <param name="storeId"></param>
        /// <param name="customerId"></param>
        public RefinancingLog(Guid applicationId, Guid creditMasterId, decimal value, string storeId, Guid customerId)
        {
            Id = IdentityGenerator.NewSequentialGuid();
            _date = DateTime.Now;

            _applicationId = applicationId;
            _creditMasterId = creditMasterId;
            _value = value;
            _storeId = storeId;
            _customerId = customerId;
        }

        /// <summary>
        /// Set additional info
        /// </summary>
        /// <returns></returns>
        public RefinancingLog SetAdditionalInfo(string referenceText, string referenceCode, UserInfo userInfo)
        {
            _referenceText = referenceText;
            _referenceCode = referenceCode;
            _userId = userInfo.UserId;
            _userName = userInfo.UserName;

            return this;
        }

        /// <summary>
        /// Set details
        /// </summary>
        /// <returns></returns>
        public RefinancingLog SetDetails(List<RefinancingLogDetail> details)
        {
            Details = details;
            return this;
        }


        /// <summary>
        /// Set detail
        /// </summary>
        /// <returns></returns>
        public RefinancingLog SetDetail(RefinancingLogDetail detail)
        {
            Details.Add(detail);
            return this;
        }

        /// <summary>
        /// Create detail
        /// </summary>
        /// <param name="creditMasterId"></param>
        /// <param name="creditId"></param>
        /// <param name="value"></param>
        /// <param name="balance"></param>
        /// <returns></returns>
        public RefinancingLogDetail CreateDetail(Guid creditMasterId, Guid creditId, decimal value, decimal balance) =>
            new RefinancingLogDetail(Id, creditMasterId, creditId, value, balance);


        /// <summary>
        /// New
        /// </summary>
        /// <returns></returns>
        public static RefinancingLog New() =>
            new RefinancingLog();
    }
}