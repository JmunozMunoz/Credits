using System.Threading.Tasks;

namespace Sc.Credits.Domain.Model.Customers.Gateway
{
    /// <summary>
    /// The customer events repository contract
    /// </summary>
    public interface ICustomerEventsRepository
    {
        /// <summary>
        /// Notify credit limit update
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        Task NotifyCreditLimitUpdateAsync(Customer customer);
    }
}