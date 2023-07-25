using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Refinancings;
using Sc.Credits.Helpers.ObjectsUtils;

namespace Sc.Credits.Domain.Model.Credits.Events
{
    /// <summary>
    /// Cancel payment master event
    /// </summary>
    public class CancelPaymentMasterEvent : MasterEvent<CreditMaster, Credit>
    {
        private readonly Credit _cancelPayment;
        private readonly TransactionType _transactionType;
        private readonly Status _activeStatus;
        private readonly string _userName;
        private readonly string _userId;
        private readonly CredinetAppSettings _credinetAppSettings;
        private decimal _balanceReleaseForRefinancing;
        private bool _isRefinancing;


        /// <summary>
        /// Creates a new instance of <see cref="CancelPaymentMasterEvent"/>
        /// </summary>
        /// <param name="cancelPaymentDomainRequest"></param>
        /// <param name="credinetAppSettings"></param>
        /// <param name="balanceReleaseForRefinancing"></param>
        /// <param name="isRefinancing"></param>
        public CancelPaymentMasterEvent(CancelPaymentDomainRequest cancelPaymentDomainRequest,
            CredinetAppSettings credinetAppSettings, decimal balanceReleaseForRefinancing = 0,
            bool isRefinancing=false)
            : base(cancelPaymentDomainRequest.CreditMaster)
        {
            _cancelPayment = cancelPaymentDomainRequest.CancelPayment;
            _transactionType = cancelPaymentDomainRequest.CancelTransactionType;
            _activeStatus = cancelPaymentDomainRequest.ActiveStatus;
            _userName = cancelPaymentDomainRequest.UserInfo.UserName;
            _userId = cancelPaymentDomainRequest.UserInfo.UserId;
            _credinetAppSettings = credinetAppSettings;
            _balanceReleaseForRefinancing = balanceReleaseForRefinancing;
            _isRefinancing = isRefinancing;

            bool increaseCreditLimit = Master.Current.GetStatusId == (int)Statuses.Paid &&
                                       Master.ValidateDateOfRefinancedCredits(credinetAppSettings);

            Master.SetCustomerCreditLimitUpdate(credinetAppSettings, increaseCreditLimit: increaseCreditLimit);
            _balanceReleaseForRefinancing = balanceReleaseForRefinancing;
        }

        public void SetBalanceReleaseForRefinancing(decimal balanceReleaseForRefinancing)
        {
            _balanceReleaseForRefinancing = balanceReleaseForRefinancing;
        }

        /// <summary>
        /// <see cref="MasterEvent{TMaster, TEntity}.Handle(TEntity)"/>
        /// </summary>
        /// <param name="newEntity"></param>
        public override void Handle(Credit newEntity)
        {
            newEntity.CancelPayment(_transactionType, _activeStatus, _userName, _userId);
            bool ignoreCreditLimitUpdate = Master.Customer.IgnoreCreditLimitUpdate();

            Master.Customer.CreditLimitDecrease(!ignoreCreditLimitUpdate && _isRefinancing ? _balanceReleaseForRefinancing 
                                                                                           : _cancelPayment.CreditPayment.GetCreditValuePaid);

            Master.SetStatus(_activeStatus);
        }
    }
}