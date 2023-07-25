using System;
using System.Threading.Tasks;
using System.Transactions;

namespace Sc.Credits.Domain.Model.Refinancings.Gateway
{
    /// <summary>
    /// The refinancing repository contract
    /// </summary>
    public interface IRefinancingRepository
    {
        /// <summary>
        /// Get application
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<RefinancingApplication> GetApplicationAsync(Guid id);

        /// <summary>
        /// Add log
        /// </summary>
        /// <param name="log"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task AddLogAsync(RefinancingLog log, Transaction transaction);
    }
}