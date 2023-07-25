using Sc.Credits.Domain.Model.Locations.Queries;
using Sc.Credits.Domain.Model.Queries;

namespace Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Locations
{
    /// <summary>
    /// City queries
    /// </summary>
    internal class CityQueries
        : CommandQueries<CityFields>
    {
        /// <summary>
        /// New city queries
        /// </summary>
        public CityQueries()
            : base(Tables.Catalog.Cities)
        {
        }
    }
}