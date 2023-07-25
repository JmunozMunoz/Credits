using Moq;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Locations;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Domain.UseCase.Stores;
using Sc.Credits.Helper.Test.Model;
using Sc.Credits.Helpers.Commons.Settings;
using Sc.Credits.Helpers.ObjectsUtils;
using Xunit;

namespace Sc.Credits.Domain.UseCase.Tests.Customers
{
    public class StoreUseCaseTest
    {
        private readonly Mock<ISettings<CredinetAppSettings>> _appSettingsMock = new Mock<ISettings<CredinetAppSettings>>();

        public IStoreUseCase StoreUsesCase
        {
            get
            {
                return new StoreUseCase(_appSettingsMock.Object);
            }
        }

        public StoreUseCaseTest()
        {
            _appSettingsMock.Setup(mock => mock.Get())
                .Returns(CredinetAppSettingsHelperTest.GetCredinetAppSettings());
        }

        /// <summary>
        /// Store create test
        /// </summary>
        [Fact]
        public void ShouldStoreCreate()
        {
            decimal effectiveAnnualRate = 0.25401M;
            Store store = StoreHelperTest.GetStore();
            State state = LocationHelperTest.GetSate();
            City city = LocationHelperTest.GetCity();
            StoreRequest storeRequest = StoreHelperTest.GetStoreRequest(store, PaymentTypes.Ordinary);
            Store storeResult = StoreUsesCase.Create(storeRequest, state, city, effectiveAnnualRate);

            Assert.IsType<Store>(storeResult);
            Assert.Equal(storeRequest.Name, storeResult.StoreName);
            Assert.Equal(storeRequest.CollectTypeId, storeResult.GetCollectTypeId);
            Assert.Equal(storeRequest.MandatoryDownPayment, storeResult.GetMandatoryDownPayment);
            Assert.Equal(storeRequest.BusinessGroupId, storeResult.GetBusinessGroupId);
            Assert.Equal(storeRequest.VendorId, storeResult.GetVendorId);
            Assert.Equal(state.Id, storeResult.GetStateId);
            Assert.Equal(city.Id, storeResult.GetCityId);
            Assert.Equal(1, storeResult.GetStoreCategoryId);
            Assert.Equal(effectiveAnnualRate, storeResult.GetEffectiveAnnualRate);
        }

        /// <summary>
        /// Store update test
        /// </summary>
        [Fact]
        public void ShouldStoreUpdate()
        {
            Store store = StoreHelperTest.GetStore();
            State state = LocationHelperTest.GetSate();
            City city = LocationHelperTest.GetCity();
            StoreRequest storeRequest = StoreHelperTest.GetStoreRequest(store, PaymentTypes.Ordinary);

            storeRequest.Name = "New name";
            storeRequest.CollectTypeId = (int)CollectTypes.All;
            storeRequest.MandatoryDownPayment = false;
            storeRequest.MinimumFee = 10000;
            storeRequest.BusinessGroupId = "3";
            storeRequest.VendorId = "TestVendorNew";

            StoreUsesCase.Update(store, storeRequest, state, city);

            Assert.Equal(storeRequest.Name, store.StoreName);
            Assert.Equal(storeRequest.CollectTypeId, store.GetCollectTypeId);
            Assert.Equal(storeRequest.MandatoryDownPayment, store.GetMandatoryDownPayment);
            Assert.Equal(storeRequest.BusinessGroupId, store.GetBusinessGroupId);
            Assert.Equal(storeRequest.VendorId, store.GetVendorId);
            Assert.Equal(state.Id, store.GetStateId);
            Assert.Equal(city.Id, store.GetCityId);
            Assert.Equal(3, store.GetStoreCategoryId);
        }
    }
}