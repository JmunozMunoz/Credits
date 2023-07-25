using System.Threading.Tasks;

namespace Sc.Credits.Domain.Model.Sequences.Gateway
{
    /// <summary>
    /// Repository for sequence calculations
    /// </summary>
    public interface ISequenceRepository
    {
        /// <summary>
        /// Get next sequence by type and store
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<long> GetNextAsync(string storeId, string type);
    }
}