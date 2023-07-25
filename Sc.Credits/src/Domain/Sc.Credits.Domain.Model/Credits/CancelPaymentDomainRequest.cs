using Sc.Credits.Domain.Model.Common;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The cancel payment domain request entity
    /// </summary>
    public class CancelPaymentDomainRequest
    {
        /// <summary>
        /// Gets the credit master
        /// </summary>
        public CreditMaster CreditMaster { get; private set; }

        /// <summary>
        /// Gets the cancel payment
        /// </summary>
        public Credit CancelPayment { get; private set; }

        /// <summary>
        /// Gets the cancel transaction type
        /// </summary>
        public TransactionType CancelTransactionType { get; private set; }

        /// <summary>
        /// Gets the active status
        /// </summary>
        public Status ActiveStatus { get; private set; }

        /// <summary>
        /// Gets the user info
        /// </summary>
        public UserInfo UserInfo { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="CancelPaymentDomainRequest"/>
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="cancelPayment"></param>
        /// <param name="cancelTransactionType"></param>
        /// <param name="activeStatus"></param>
        /// <param name="userInfo"></param>
        public CancelPaymentDomainRequest(CreditMaster creditMaster,
            Credit cancelPayment,
            TransactionType cancelTransactionType,
            Status activeStatus,
            UserInfo userInfo)
        {
            CreditMaster = creditMaster;
            CancelPayment = cancelPayment;
            CancelTransactionType = cancelTransactionType;
            ActiveStatus = activeStatus;
            UserInfo = userInfo;
        }
    }
}