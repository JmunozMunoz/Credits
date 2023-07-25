using Sc.Credits.Domain.Model.Base;

namespace Sc.Credits.Domain.Model.Locations.Gateway
{
    /// <summary>
    /// The location repository contract
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface ILocationRepository<TEntity>
        : ICommandRepository<TEntity>
        where TEntity : Location
    {
    }
}