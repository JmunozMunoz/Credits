using Microsoft.Extensions.Options;

namespace Sc.Credits.Helpers.Commons.Settings
{
    /// <summary>
    /// <see cref="ISettings{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Settings<T>
        : ISettings<T>
        where T : class, new()
    {
        private readonly T _instance;

        /// <summary>
        /// New settings
        /// </summary>
        /// <param name="options"></param>
        public Settings(IOptions<T> options)
        {
            _instance = options.Value;
        }

        /// <summary>
        /// <see cref="ISettings{T}.Get"/>
        /// </summary>
        /// <returns></returns>
        public T Get() =>
            _instance;
    }
}