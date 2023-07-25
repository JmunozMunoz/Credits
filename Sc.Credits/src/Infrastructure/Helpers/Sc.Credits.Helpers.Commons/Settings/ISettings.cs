namespace Sc.Credits.Helpers.Commons.Settings
{
    /// <summary>
    /// Settings
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISettings<out T>
        where T : class, new()
    {
        /// <summary>
        /// Get
        /// </summary>
        /// <returns></returns>
        T Get();
    }
}