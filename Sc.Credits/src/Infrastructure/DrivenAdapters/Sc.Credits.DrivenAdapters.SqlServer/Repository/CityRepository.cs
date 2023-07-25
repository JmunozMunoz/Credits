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
    /// The default implementation of <see cref="ICityRepository"/>
    /// </summary>
    public class CityRepository
        : LocationRepository<City, CityFields>, ICityRepository
    {
        private static readonly CityQueries _cityQueries = QueriesCatalog.City;

        /// <summary>
        /// Creates new instance of <see cref="CityRepository"/>
        /// </summary>
        /// <param name="creditsConnectionFactory"></param>
        /// <param name="citySqlDelegatedHandlers"></param>
        public CityRepository(ICreditsConnectionFactory creditsConnectionFactory,
            ISqlDelegatedHandlers<City> citySqlDelegatedHandlers)
            : base(_cityQueries, citySqlDelegatedHandlers, creditsConnectionFactory)
        {
        }
    }
}