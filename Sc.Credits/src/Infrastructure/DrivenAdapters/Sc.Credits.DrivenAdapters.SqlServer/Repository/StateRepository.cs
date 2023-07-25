using Sc.Credits.Domain.Model.Locations;
using Sc.Credits.Domain.Model.Locations.Gateway;
using Sc.Credits.Domain.Model.Locations.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Connection;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.QueriesKata;
using Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Locations;
using Sc.Credits.DrivenAdapters.SqlServer.Repository.Base;

namespace Sc.Credits.DrivenAdapters.SqlServer.Repository
{
    /// <summary>
    /// The default implementation of <see cref="IStateRepository"/>
    /// </summary>
    public class StateRepository
        : LocationRepository<State, StateFields>, IStateRepository
    {
        private static readonly StateQueries _stateQueries = QueriesCatalog.State;

        /// <summary>
        /// Creates new instance of <see cref="StateRepository"/>
        /// </summary>
        /// <param name="connectionFactory"></param>
        /// <param name="stateSqlDelegatedHandlers"></param>
        public StateRepository(ICreditsConnectionFactory connectionFactory,
            ISqlDelegatedHandlers<State> stateSqlDelegatedHandlers)
            : base(_stateQueries, stateSqlDelegatedHandlers, connectionFactory)
        {
        }
    }
}