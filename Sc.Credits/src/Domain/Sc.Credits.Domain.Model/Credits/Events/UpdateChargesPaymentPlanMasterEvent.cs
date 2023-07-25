using Sc.Credits.Domain.Model.Base;

namespace Sc.Credits.Domain.Model.Credits.Events
{
    /// <summary>
    /// Update charges payment plan master event
    /// </summary>
    public class UpdateChargesPaymentPlanMasterEvent : MasterEvent<CreditMaster, Credit>
    {
        private readonly decimal _chargesValue;
        private readonly bool _hasArrearsCharge;
        private readonly decimal _arrearsCharges;
        private readonly decimal _updatedPaymentPlanValue;
        private readonly TransactionType _transactionType;

        /// <summary>
        /// Creates a new instance of <see cref="UpdateChargesPaymentPlanMasterEvent"/>
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="chargesValue"></param>
        /// <param name="hasArrearsCharge"></param>
        /// <param name="arrearsCharges"></param>
        /// <param name="updatedPaymentPlanValue"></param>
        /// <param name="transactionType"></param>
        public UpdateChargesPaymentPlanMasterEvent(CreditMaster creditMaster, decimal chargesValue, bool hasArrearsCharge,
            decimal arrearsCharges, decimal updatedPaymentPlanValue, TransactionType transactionType)
            : base(creditMaster)
        {
            _chargesValue = chargesValue;
            _hasArrearsCharge = hasArrearsCharge;
            _arrearsCharges = arrearsCharges;
            _updatedPaymentPlanValue = updatedPaymentPlanValue;
            _transactionType = transactionType;
        }

        /// <summary>
        /// <see cref="MasterEvent{TMaster, TEntity}.Handle(TEntity)"/>
        /// </summary>
        /// <param name="newEntity"></param>
        public override void Handle(Credit newEntity)
        {
            newEntity.UpdateChargesPaymentPlanValue(_chargesValue, _hasArrearsCharge, _arrearsCharges,
                _updatedPaymentPlanValue, _transactionType);
        }
    }
}