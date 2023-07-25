using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.Helpers.ObjectsUtils;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Managment.Services.Common
{
    /// <summary>
    /// App parameters service contract
    /// </summary>
    public interface IAppParametersService
    {
        /// <summary>
        /// Get app parameters
        /// </summary>
        /// <returns></returns>
        Task<AppParameters> GetAppParametersAsync();

        /// <summary>
        /// Get transaction type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TransactionType> GetTransactionTypeAsync(int id);

        /// <summary>
        /// Get status
        /// </summary>
        /// <param name="id"></param>
        /// <param name="attach"></param>
        /// <returns></returns>
        Task<Status> GetStatusAsync(int id);

        /// <summary>
        /// Get source
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Source> GetSourceAsync(int id);

        /// <summary>
        /// Get payment type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<PaymentType> GetPaymentTypeAsync(int id);

        /// <summary>
        /// Get auth method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<AuthMethod> GetAuthMethodAsync(int id);

        /// <summary>
        /// Get settings
        /// </summary>
        /// <returns></returns>
        CredinetAppSettings GetSettings();
    }
}