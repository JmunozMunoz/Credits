using Sc.Credits.Domain.Model.Locations;
using Sc.Credits.Domain.Model.Locations.Gateway;
using Sc.Credits.Domain.Model.Locations.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Connection;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;

namespace Sc.Credits.DrivenAdapters.SqlServer.Repository.Base
{
    /// <summary>
    /// The default base implementation of <see cref="ILocationRepository{TEntity}"/>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TFields"></typeparam>
    public class LocationRepository<TEntity, TFields>
        : CommandRepository<TEntity, TFields>, ILocationRepository<TEntity>
        where TEntity : Location
        where TFields : LocationFields
    {
        /// <summary>
        /// Creates new instance of <see cref="LocationRepository{TEntity, TFields}"/>
        /// </summary>
        /// <param name="queries"></param>
        /// <param name="entitySqlDelegatedHandlers"></param>
        /// <param name="connectionFactory"></param>
        public LocationRepository(ICommandQueries<TFields> queries,
            ISqlDelegatedHandlers<TEntity> entitySqlDelegatedHandlers,
            IConnectionFactory connectionFactory)
            : base(queries, entitySqlDelegatedHandlers, connectionFactory)
        {
        }
    }
}