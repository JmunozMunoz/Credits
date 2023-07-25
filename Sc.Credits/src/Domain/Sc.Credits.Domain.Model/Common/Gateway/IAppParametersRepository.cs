using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Parameters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Model.Common.Gateway
{
    /// <summary>
    /// App paramaters repository contract
    /// </summary>
    public interface IAppParametersRepository
    {
        /// <summary>
        /// Get all parameters
        /// </summary>
        /// <returns></returns>
        Task<List<Parameter>> GetAllParametersAsync();

        /// <summary>
        /// Get transaction type
        /// </summary>
        /// <returns></returns>
        Task<TransactionType> GetTransactionTypeAsync(int id);

        /// <summary>
        /// Get status
        /// </summary>
        /// <param name="id"></param>
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
    }
}