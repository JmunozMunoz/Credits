using Sc.Credits.Domain.Model.Credits;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Managment.Services.Credits
{
    /// <summary>
    /// Credit customer service contract
    /// </summary>
    public interface ICreditCustomerService
    {
        /// <summary>
        /// Send migration history
        /// </summary>
        /// <param name="creditCustomerMigrationHistoryRequest"></param>
        Task SendMigrationHistoryAsync(CreditCustomerMigrationHistoryRequest creditCustomerMigrationHistoryRequest);
    }
}