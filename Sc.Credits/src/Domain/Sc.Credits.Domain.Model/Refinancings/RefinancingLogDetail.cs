using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Helpers.Commons.Dapper.Annotations;
using Sc.Credits.Helpers.Commons.Domain;
using System;

namespace Sc.Credits.Domain.Model.Refinancings
{
    /// <summary>
    /// Refinancing log detail
    /// </summary>
    public class RefinancingLogDetail : Entity<Guid>
    {

        private Guid _refinancingLogId;

        private Guid _creditMasterId;

        private Guid _creditId;

        private decimal _value;

        private decimal _balance;

        /// <summary>
        /// Refinancing log id
        /// </summary>
        [Write(false)]
        public Guid GetRefinancingLogId => _refinancingLogId;

        /// <summary>
        /// Credit master id
        /// </summary>
        [Write(false)]
        public Guid GetCreditMasterId => _creditMasterId;

        /// <summary>
        /// Credit id
        /// </summary>
        [Write(false)]
        public Guid GetCreditId => _creditId;

        /// <summary>
        /// Value
        /// </summary>
        [Write(false)]
        public decimal GetValue => _value;

        /// <summary>
        /// Value
        /// </summary>
        [Write(false)]
        public decimal GetBalance => _balance;

        /// <summary>
        /// New refinancing log detail
        /// </summary>
        protected RefinancingLogDetail()
        {
        }

        /// <summary>
        /// New refinancing log detail
        /// </summary>
        /// <param name="refinancingLogId"></param>
        /// <param name="creditMasterId"></param>
        /// <param name="creditId"></param>
        /// <param name="value"></param>
        /// <param name="balance"></param>
        internal RefinancingLogDetail(Guid refinancingLogId, Guid creditMasterId, Guid creditId, decimal value, decimal balance)
        {
            Id = IdentityGenerator.NewSequentialGuid();
            _refinancingLogId = refinancingLogId;
            _creditMasterId = creditMasterId;
            _creditId = creditId;
            _value = value;
            _balance = balance;
        }

        /// <summary>
        /// New
        /// </summary>
        /// <returns></returns>
        public static RefinancingLogDetail New() =>
            new RefinancingLogDetail();
    }
}