using Sc.Credits.Domain.Model.Locations;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Managment.Services.Locations
{
    /// <summary>
    /// Location service contract
    /// </summary>
    public interface ILocationService
    {
        /// <summary>
        /// Add or update state
        /// </summary>
        /// <param name="stateRequest"></param>
        /// <returns></returns>
        Task<State> AddOrUpdateStateAsync(StateRequest stateRequest);

        /// <summary>
        /// Add or update city
        /// </summary>
        /// <param name="cityRequest"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        Task<City> AddOrUpdateCityAsync(CityRequest cityRequest, State state);
    }
}