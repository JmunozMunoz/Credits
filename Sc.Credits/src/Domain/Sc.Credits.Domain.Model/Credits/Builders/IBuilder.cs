namespace Sc.Credits.Domain.Model.Credits.Builders
{
    /// <summary>
    /// Builder contract
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBuilder<out T>
    {
        /// <summary>
        /// Build
        /// </summary>
        /// <returns></returns>
        T Build();
    }
}