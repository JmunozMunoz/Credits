using Sc.Credits.Domain.Model.Common;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The cancel credit domain request entity
    /// </summary>
    public class CancelCreditDomainRequest
    {
        /// <summary>
        /// Gets the credit master
        /// </summary>
        public CreditMaster CreditMaster { get; private set; }

        /// <summary>
        /// Gets the cancel credit transaction type
        /// </summary>
        public TransactionType CancelCreditTransactionType { get; internal set; }

        /// <summary>
        /// Gets the canceled status
        /// </summary>
        public Status CanceledStatus { get; private set; }

        /// <summary>
        /// Gets the user's info
        /// </summary>
        public UserInfo UserInfo { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="CancelCreditDomainRequest"/>
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="cancelCreditTransactionType"></param>
        /// <param name="canceledStatus"></param>
        /// <param name="userInfo"></param>
        public CancelCreditDomainRequest(CreditMaster creditMaster,
            TransactionType cancelCreditTransactionType,
            Status canceledStatus,
            UserInfo userInfo)
        {
            CreditMaster = creditMaster;
            CancelCreditTransactionType = cancelCreditTransactionType;
            CanceledStatus = canceledStatus;
            UserInfo = userInfo;
        }
    }
}