using Sc.Credits.Domain.Model.Locations;
using Sc.Credits.Domain.Model.Stores;

namespace Sc.Credits.Domain.UseCase.Stores
{
    /// <summary>
    /// Store use case contract
    /// </summary>
    public interface IStoreUseCase
    {
        /// <summary>
        /// Create store
        /// </summary>
        /// <param name="storeRequest"></param>
        /// <param name="state"></param>
        /// <param name="city"></param>
        /// <param name="effectiveAnnualRate"></param>
        /// <returns></returns>
        Store Create(StoreRequest storeRequest, State state, City city, decimal effectiveAnnualRate);

        /// <summary>
        /// Update store
        /// </summary>
        /// <param name="store"></param>
        /// <param name="storeRequest"></param>
        /// <param name="state"></param>
        /// <param name="city"></param>
        /// <returns></returns>
        void Update(Store store, StoreRequest storeRequest, State state, City city);
    }
}