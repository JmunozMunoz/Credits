using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Locations;
using Sc.Credits.Domain.Model.Locations;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Domain.Model.Stores.Gateway;
using Sc.Credits.Domain.Model.Stores.Queries.Commands;
using Sc.Credits.Domain.UseCase.Stores;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Managment.Services.Stores
{
    /// <summary>
    /// Store service is an implementation of <see cref="IStoreService"/>
    /// </summary>
    public class StoreService
        : IStoreService
    {
        private readonly IStoreRepository _storeRepository;
        private readonly IStoreUseCase _storeUsesCase;
        private readonly ILocationService _locationService;
        private readonly IAppParametersService _appParametersService;

        /// <summary>
        /// Creates a new instance of <see cref="StoreService"/>
        /// </summary>
        /// <param name="storeRepository"></param>
        /// <param name="storeUsesCase"></param>
        /// <param name="locationService"></param>
        /// <param name="appParametersService"></param>
        public StoreService(IStoreRepository storeRepository,
            IStoreUseCase storeUsesCase,
            ILocationService locationService,
            IAppParametersService appParametersService)
        {
            _storeRepository = storeRepository;
            _storeUsesCase = storeUsesCase;
            _locationService = locationService;
            _appParametersService = appParametersService;
        }

        /// <summary>
        /// <see cref="IStoreService.CreateOrUpdateAsync(StoreRequest)"/>
        /// </summary>
        /// <param name="storeRequest"></param>
        /// <returns></returns>
        public async Task CreateOrUpdateAsync(StoreRequest storeRequest)
        {
            if (!string.IsNullOrEmpty(storeRequest.BusinessGroupId))
            {
                await CreateOrUpdateBusinessGroupAsync(storeRequest.BusinessGroupId, storeRequest.BusinessGroupName);
            }

            (State State, City City) stateAndCity = await CreateOrUpdateStateAndCityAsync(storeRequest);

            Store store = await _storeRepository.GetByIdAsync(storeRequest.Id);

            if (store == null)
            {
                AppParameters parameters = await _appParametersService.GetAppParametersAsync();
                await CreateAsync(storeRequest, stateAndCity, parameters.EffectiveAnnualRate);
            }
            else
            {
                await UpdateAsync(store, storeRequest, stateAndCity);
            }
        }

        /// <summary>
        /// Creates new store.
        /// </summary>
        /// <param name="storeRequest"></param>
        /// <param name="stateAndCity"></param>
        /// <returns></returns>
        private async Task CreateAsync(StoreRequest storeRequest, (State State, City City) stateAndCity, decimal effectiveAnnualRate)
        {
            Store store = _storeUsesCase.Create(storeRequest, stateAndCity.State, stateAndCity.City, effectiveAnnualRate);
            await _storeRepository.AddAsync(store);

            await CreateStoreIdentificationAsync(storeRequest);
        }

        /// <summary>
        /// Updates current store.
        /// </summary>
        /// <param name="store"></param>
        /// <param name="storeRequest"></param>
        /// <param name="stateAndCity"></param>
        /// <returns></returns>
        private async Task UpdateAsync(Store store, StoreRequest storeRequest, (State State, City City) stateAndCity)
        {
            _storeUsesCase.Update(store, storeRequest, stateAndCity.State, stateAndCity.City);

            await _storeRepository.UpdateAsync(store, StoreCommandsFields.Update);

            StoreIdentification storeIdentification =
               await _storeRepository.GetStoreIdentificationAsync(storeRequest.Id);

            if (storeIdentification == null)
            {
                await CreateStoreIdentificationAsync(storeRequest);
            }
            else
            {
                if (storeIdentification.SetUpdated(storeRequest))
                {
                    await _storeRepository.UpdateStoreIdentificationAsync(storeIdentification);
                }
            }
        }

        /// <summary>
        /// <see cref="IStoreService.GetAsync(string, IEnumerable{Field}, bool, bool, bool)"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fields"></param>
        /// <param name="loadAssuranceCompany"></param>
        /// <param name="loadPaymentType"></param>
        /// <param name="loadProductCategory"></param>
        /// <param name="loadCity"></param>
        /// <param name="loadBusinessGroup"></param>
        /// <returns></returns>
        public async Task<Store> GetAsync(string id, IEnumerable<Field> fields, bool loadAssuranceCompany = false,
            bool loadPaymentType = false, bool loadProductCategory = false, bool loadCity=false, bool loadBusinessGroup = false) =>
               await _storeRepository.GetByIdAsync(id, fields, loadAssuranceCompany, loadPaymentType, loadProductCategory, loadCity, loadBusinessGroup)
                   ??
                   throw new BusinessException(nameof(BusinessResponse.StoreNotFound), (int)BusinessResponse.StoreNotFound);

        /// <summary>
        /// <see cref="IStoreService.GetAssuranceCompanyAsync(long)"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<AssuranceCompany> GetAssuranceCompanyAsync(long id) =>
            await _storeRepository.GetAssuranceCompanyAsync(id);

        /// <summary>
        /// Create store identification
        /// </summary>
        /// <param name="storeRequest"></param>
        private async Task CreateStoreIdentificationAsync(StoreRequest storeRequest)
        {
            StoreIdentification storeIdentification = new StoreIdentification(storeId: storeRequest.Id, scCode: storeRequest.ScCode);
            await _storeRepository.AddStoreIdentificationAsync(storeIdentification);
        }

        /// <summary>
        /// Create or update business group
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private async Task CreateOrUpdateBusinessGroupAsync(string id, string name)
        {
            BusinessGroup businessGroup = await _storeRepository.GetBusinessGroupAsync(id);

            if (businessGroup == null)
            {
                businessGroup = new BusinessGroup(id, name);

                await _storeRepository.AddBusinessGroupAsync(businessGroup);
            }
            else
            {
                if (!businessGroup.MatchName(name))
                {
                    businessGroup.SetName(name);
                    await _storeRepository.UpdateBusinessGroupAsync(businessGroup);
                }
            }
        }

        /// <summary>
        /// Create or update state and city
        /// </summary>
        /// <param name="storeRequest"></param>
        /// <returns></returns>
        private async Task<(State State, City City)> CreateOrUpdateStateAndCityAsync(StoreRequest storeRequest)
        {
            if (storeRequest.State != null && storeRequest.City != null)
            {
                State state = await _locationService.AddOrUpdateStateAsync(storeRequest.State);
                City city = await _locationService.AddOrUpdateCityAsync(storeRequest.City, state);

                return (State: state, City: city);
            }

            return (State: null, City: null);
        }
    }
}