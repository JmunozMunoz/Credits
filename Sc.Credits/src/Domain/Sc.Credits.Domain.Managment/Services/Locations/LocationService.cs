using Sc.Credits.Domain.Model.Locations;
using Sc.Credits.Domain.Model.Locations.Gateway;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.Model.Queries.Extensions;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Managment.Services.Locations
{
    /// <summary>
    /// Location service is an implementation of <see cref="ILocationService"/>
    /// </summary>
    public class LocationService
        : ILocationService
    {
        private readonly IStateRepository _stateRepository;
        private readonly ICityRepository _cityRepository;

        /// <summary>
        /// Creates a new instance of <see cref="LocationService"/>
        /// </summary>
        /// <param name="stateRepository"></param>
        /// <param name="cityRepository"></param>
        public LocationService(IStateRepository stateRepository,
            ICityRepository cityRepository)
        {
            _stateRepository = stateRepository;
            _cityRepository = cityRepository;
        }

        /// <summary>
        /// <see cref="ILocationService.AddOrUpdateStateAsync(StateRequest)"/>
        /// </summary>
        /// <param name="stateRequest"></param>
        /// <returns></returns>
        public async Task<State> AddOrUpdateStateAsync(StateRequest stateRequest)
        {
            State state = await _stateRepository.GetByIdAsync(stateRequest.Code);

            if (state == null)
            {
                state = new State(stateRequest.Code, stateRequest.Name);
                await _stateRepository.AddAsync(state);
            }
            else if (state.Name.Trim().ToLower() != stateRequest.Name.Trim().ToLower())
            {
                state.SetName(stateRequest.Name);
                await _stateRepository.UpdateAsync(state, Tables.Catalog.States.Fields.GetAllFields());
            }

            return state;
        }

        /// <summary>
        /// <see cref="ILocationService.AddOrUpdateCityAsync(CityRequest, State)"/>
        /// </summary>
        /// <param name="cityRequest"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public async Task<City> AddOrUpdateCityAsync(CityRequest cityRequest, State state)
        {
            City city = await _cityRepository.GetByIdAsync(cityRequest.Code);

            if (city == null)
            {
                city = new City(cityRequest.Code, cityRequest.Name, state.Id);
                await _cityRepository.AddAsync(city);
            }
            else if (city.Name.Trim().ToLower() != cityRequest.Name.Trim().ToLower())
            {
                city.SetName(cityRequest.Name);
                await _cityRepository.UpdateAsync(city, Tables.Catalog.Cities.Fields.GetAllFields());
            }

            return city;
        }
    }
}