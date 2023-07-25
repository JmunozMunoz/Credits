using Sc.Credits.Domain.Model.Locations;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Helpers.Commons.Settings;
using Sc.Credits.Helpers.ObjectsUtils;

namespace Sc.Credits.Domain.UseCase.Stores
{
    /// <summary>
    /// Store use case is an implementation of <see cref="IStoreUseCase"/>
    /// </summary>
    public class StoreUseCase : IStoreUseCase
    {
        private readonly CredinetAppSettings _credinetAppSettings;

        /// <summary>
        /// Store use case
        /// </summary>
        /// <param name="appSettings"></param>
        public StoreUseCase(ISettings<CredinetAppSettings> appSettings)
        {
            _credinetAppSettings = appSettings.Get();
        }

        /// <summary>
        /// <see cref="IStoreUseCase.Create(StoreRequest, State, City, decimal)"/>
        /// </summary>
        /// <param name="storeRequest"></param>
        /// <param name="state"></param>
        /// <param name="city"></param>
        /// <param name="effectiveAnnualRate"></param>
        /// <returns></returns>
        public Store Create(StoreRequest storeRequest, State state, City city, decimal effectiveAnnualRate) =>
            StoreBuilder.CreateBuilder()
                .Init(_credinetAppSettings, effectiveAnnualRate)
                .BasicInfo(storeRequest.Id, storeRequest.Name, storeRequest.Phones, storeRequest.Status,
                    _credinetAppSettings.StoreAllowPromissoryNoteSignatureDefault, storeRequest.StoreProfileCode)
                .VendorInfo(storeRequest.VendorId, storeRequest.BusinessGroupId, storeRequest.AssuranceCompanyId, storeRequest.Nit)
                .PaymentInfo(storeRequest.CollectTypeId, storeRequest.PaymentTypeId, storeRequest.AssuranceType,
                    storeRequest.MandatoryDownPayment)
                .SetLocation(state?.Id, city?.Id)
                .CalculationValues(storeRequest.MinimumFee, storeRequest.DownPaymentPercentage)
                .Build();

        /// <summary>
        /// <see cref="IStoreUseCase.Update(Store, StoreRequest, State, City)"/>
        /// </summary>
        /// <param name="store"></param>
        /// <param name="storeRequest"></param>
        /// <param name="state"></param>
        /// <param name="city"></param>
        /// <returns></returns>
        public void Update(Store store, StoreRequest storeRequest, State state, City city) =>
            store.Update(storeRequest, state, city);
    }
}