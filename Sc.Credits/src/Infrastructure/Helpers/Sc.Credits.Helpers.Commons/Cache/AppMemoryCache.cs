using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;

namespace Sc.Credits.Helpers.Commons.Cache
{
    /// <summary>
    /// App memory cache
    /// </summary>
    public class AppMemoryCache : ICache
    {
        private static IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());

        private static ICache _instance;

        /// <summary>
        /// Current instance
        /// </summary>
        public static ICache Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AppMemoryCache();
                }
                return _instance;
            }
        }

        /// <summary>
        /// New app memory cache
        /// </summary>
        protected AppMemoryCache()
        {
        }

        /// <summary>
        /// Set cache
        /// </summary>
        /// <param name="memoryCache"></param>
        public static void SetCache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        #region ICache Members

        /// <summary>
        /// <see cref="ICache.GetOrCreateAsync{TItem}(CacheItem{TItem})"/>
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public Task<TItem> GetOrCreateAsync<TItem>(CacheItem<TItem> item) =>
            _memoryCache.GetOrCreateAsync(item.GetKey, async entry =>
            {
                entry.SetAbsoluteExpiration(item.GetTimeOut);
                return await item.GetItemAsync();
            });

        #endregion ICache Members
    }
}