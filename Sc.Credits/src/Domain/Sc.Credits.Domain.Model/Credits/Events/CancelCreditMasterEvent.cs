using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Helpers.ObjectsUtils;

namespace Sc.Credits.Domain.Model.Credits.Events
{
    /// <summary>
    /// Cancel credit master event
    /// </summary>
    public class CancelCreditMasterEvent : MasterEvent<CreditMaster, Credit>
    {
        private readonly TransactionType _cancelCreditTransactionType;
        private readonly Status _canceledStatus;
        private readonly string _userName;
        private readonly string _userId;

        /// <summary>
        /// Creates a new instance of <see cref="CancelCreditMasterEvent"/>
        /// </summary>
        /// <param name="cancelCreditDomainRequest"></param>
        /// <param name="credinetAppSettings"></param>
        public CancelCreditMasterEvent(CancelCreditDomainRequest cancelCreditDomainRequest,
            CredinetAppSettings credinetAppSettings)
            : base(cancelCreditDomainRequest.CreditMaster)
        {
            _cancelCreditTransactionType = cancelCreditDomainRequest.CancelCreditTransactionType;
            _canceledStatus = cancelCreditDomainRequest.CanceledStatus;
            _userName = cancelCreditDomainRequest.UserInfo.UserName;
            _userId = cancelCreditDomainRequest.UserInfo.UserId;

            Master.SetCustomerCreditLimitUpdate(credinetAppSettings);
        }

        /// <summary>
        /// <see cref="MasterEvent{TMaster, TEntity}.Handle(TEntity)"/>
        /// </summary>
        /// <param name="newEntity"></param>
        public override void Handle(Credit newEntity)
        {
            newEntity.CreditCancel(_cancelCreditTransactionType, _canceledStatus, _userName, _userId);

            Master.SetStatus(_canceledStatus);

            Master.Customer.CreditLimitIncrease(newEntity.GetCreditValue);
        }
    }
}