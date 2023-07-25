using System.Threading.Tasks;

namespace Sc.Credits.Helpers.Commons.Cache
{
    /// <summary>
    /// Cache
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// Get or create async
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<TItem> GetOrCreateAsync<TItem>(CacheItem<TItem> item);
    }
}