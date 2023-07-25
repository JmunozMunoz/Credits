using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The payment credit multiple detail entity
    /// </summary>
    public class PaymentCreditMultipleDetail
    {
        /// <summary>
        /// Map into field Ids in MasterCredits entity and creditMasterId in Credits entity.
        /// </summary>
        public Guid CreditId { get; set; }

        /// <summary>
        /// Total value paid
        /// </summary>
        public decimal TotalValuePaid { get; set; }
    }
}