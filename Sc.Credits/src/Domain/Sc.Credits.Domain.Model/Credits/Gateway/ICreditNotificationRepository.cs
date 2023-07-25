using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Stores;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Model.Credits.Gateway
{
    /// <summary>
    /// Credit notification repository contract
    /// </summary>
    public interface ICreditNotificationRepository
    {
        /// <summary>
        /// Notify creation
        /// </summary>
        /// <param name="createCreditResponse"></param>
        /// <returns></returns>
        Task NotifyCreationAsync(CreateCreditResponse createCreditResponse);

        /// <summary>
        /// Send event
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <param name="credit"></param>
        /// <param name="transactionType"></param>
        /// <param name="complementEventName"></param>
        /// <returns></returns>
        Task SendEventAsync(CreditMaster creditMaster, Customer customer, Store store, Credit credit = null, TransactionType transactionType = null, string complementEventName = null);
    }
}