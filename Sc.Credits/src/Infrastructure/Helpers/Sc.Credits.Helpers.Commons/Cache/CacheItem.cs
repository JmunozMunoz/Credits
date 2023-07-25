using System;
using System.Threading.Tasks;

namespace Sc.Credits.Helpers.Commons.Cache
{
    /// <summary>
    /// Cache item
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class CacheItem<TItem>
    {
        private readonly object _key;
        private readonly Func<Task<TItem>> _itemFactory;
        private readonly TimeSpan _timeOut;

        /// <summary>
        /// Get key
        /// </summary>
        public object GetKey => _key;

        /// <summary>
        /// Get time out
        /// </summary>
        public TimeSpan GetTimeOut => _timeOut;

        /// <summary>
        /// New cache item
        /// </summary>
        /// <param name="key"></param>
        /// <param name="itemFactory"></param>
        /// <param name="timeOut"></param>
        public CacheItem(object key, Func<Task<TItem>> itemFactory, TimeSpan timeOut)
        {
            _key = key;
            _itemFactory = itemFactory;
            _timeOut = timeOut;
        }

        /// <summary>
        /// Get item async
        /// </summary>
        /// <returns></returns>
        public async Task<TItem> GetItemAsync() =>
            await _itemFactory();
    }
}