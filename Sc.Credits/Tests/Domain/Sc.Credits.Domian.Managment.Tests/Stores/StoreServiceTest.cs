using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Moq;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Locations;
using Sc.Credits.Domain.Managment.Services.Stores;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Locations;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Domain.Model.Stores.Gateway;
using Sc.Credits.Domain.UseCase.Stores;
using Sc.Credits.Helper.Test.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace Sc.Credits.Domain.Managment.Tests.Customers
{
    public class StoreServiceTest
    {
        private readonly Mock<IStoreRepository> _storeRepositoryMock = new Mock<IStoreRepository>();
        private readonly Mock<IStoreUseCase> _storeUsesCaseMock = new Mock<IStoreUseCase>();
        private readonly Mock<ILocationService> _locationServiceMock = new Mock<ILocationService>();
        private readonly Mock<IAppParametersService> _appParametersService = new Mock<IAppParametersService>();

        private IStoreService StoreService =>
            new StoreService(_storeRepositoryMock.Object,
                _storeUsesCaseMock.Object,
                _locationServiceMock.Object, _appParametersService.Object);

        public StoreServiceTest()
        {
        }

        [Fact]
        public void ShouldCreateStore()
        {
            Store store = StoreHelperTest.GetStore();
            StoreRequest storeRequest = StoreHelperTest.GetStoreRequest(store, PaymentTypes.Ordinary);

            bool modified = false;
            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            _appParametersService.Setup(mock => mock.GetAppParametersAsync()).ReturnsAsync(parameters);

            _storeUsesCaseMock.Setup(cu => cu.Create(It.IsAny<StoreRequest>(), It.IsAny<State>(), It.IsAny<City>(), It.IsAny<decimal>())).Returns(store);

            _storeRepositoryMock.Setup(cm => cm.AddAsync(It.IsAny<Store>(),
                It.IsAny<Transaction>())).Callback(() => modified = true);

            StoreService.CreateOrUpdateAsync(storeRequest);

            _storeUsesCaseMock.Verify(item => item.Create(It.IsAny<StoreRequest>(), It.IsAny<State>(), It.IsAny<City>(), It.IsAny<decimal>()), Times.Once());

            _locationServiceMock.Verify(mock => mock.AddOrUpdateStateAsync(It.IsAny<StateRequest>()), Times.Once);

            _locationServiceMock.Verify(mock => mock.AddOrUpdateCityAsync(It.IsAny<CityRequest>(), It.IsAny<State>()), Times.Once);
            _appParametersService.Verify(mock => mock.GetAppParametersAsync(), Times.Once);

            Assert.True(modified);
        }

        [Fact]
        public async Task ShouldUpdateStore()
        {
            bool modified = false;

            Store store = StoreHelperTest.GetStore();
            StoreRequest storeRequest = StoreHelperTest.GetStoreRequest(store, PaymentTypes.Ordinary);

            _storeRepositoryMock.Setup(cr => cr.GetByIdAsync(It.IsAny<object>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(store);

            _storeRepositoryMock.Setup(cm => cm.UpdateAsync(It.IsAny<Store>(), It.IsAny<IEnumerable<Field>>(),
                It.IsAny<Transaction>())).Callback(() => modified = true);

            await StoreService.CreateOrUpdateAsync(storeRequest);

            _storeUsesCaseMock.Verify(item => item.Update(It.IsAny<Store>(), It.IsAny<StoreRequest>(), It.IsAny<State>(), It.IsAny<City>()), Times.Once());

            _locationServiceMock.Verify(mock => mock.AddOrUpdateStateAsync(It.IsAny<StateRequest>()), Times.Once);

            _locationServiceMock.Verify(mock => mock.AddOrUpdateCityAsync(It.IsAny<CityRequest>(), It.IsAny<State>()), Times.Once);

            Assert.True(modified);
        }

        [Fact]
        public async Task ShouldGetStore()
        {
            Store store = StoreHelperTest.GetStore();

            _storeRepositoryMock.Setup(mock => mock.GetByIdAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(store);

            Store storeResult = await StoreService.GetAsync("TestStoreId", Enumerable.Empty<Field>());

            Assert.NotNull(storeResult);
        }

        [Fact]
        public async Task ShouldGetStoreThowsBusinessExceptionNotFound()
        {
            Store store = null;

            _storeRepositoryMock.Setup(mock => mock.GetByIdAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(store);

            BusinessException exception = await Assert.ThrowsAsync<BusinessException>(() =>
                StoreService.GetAsync("TestStoreId", Enumerable.Empty<Field>()));

            Assert.Equal((int)BusinessResponse.StoreNotFound, exception.code);
        }

        [Fact]
        public void ShouldCreateBusinessGroup()
        {
            Store store = StoreHelperTest.GetStore();
            StoreRequest storeRequest = StoreHelperTest.GetStoreRequest(store, PaymentTypes.Ordinary);

            bool modified = false;

            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            _appParametersService.Setup(mock => mock.GetAppParametersAsync()).ReturnsAsync(parameters);

            _storeUsesCaseMock.Setup(cu => cu.Create(It.IsAny<StoreRequest>(), It.IsAny<State>(), It.IsAny<City>(), It.IsAny<decimal>()))
                .Returns(store);

            _storeRepositoryMock.Setup(cm => cm.GetBusinessGroupAsync(It.IsAny<string>()))
                .ReturnsAsync((BusinessGroup)null);

            _storeRepositoryMock.Setup(cm => cm.AddBusinessGroupAsync(It.IsAny<BusinessGroup>())).Callback(() => modified = true);

            StoreService.CreateOrUpdateAsync(storeRequest);

            _storeRepositoryMock.Verify(cr => cr.AddBusinessGroupAsync(It.IsAny<BusinessGroup>()), Times.Once);

            _appParametersService.Verify(mock => mock.GetAppParametersAsync(), Times.Once);

            Assert.True(modified);
        }

        [Fact]
        public void ShouldUpdateBusinessGroup()
        {
            Store store = StoreHelperTest.GetStore();
            StoreRequest storeRequest = StoreHelperTest.GetStoreRequest(store, PaymentTypes.Ordinary);
            BusinessGroup businessGroup = new BusinessGroup("TestId", "Unmodified group test");

            bool modified = false;
            AppParameters parameters = ParameterHelperTest.GetAppParameters();

            _appParametersService.Setup(mock => mock.GetAppParametersAsync()).ReturnsAsync(parameters);

            _storeUsesCaseMock.Setup(cu => cu.Create(It.IsAny<StoreRequest>(), It.IsAny<State>(), It.IsAny<City>(), It.IsAny<decimal>()))
                .Returns(store);

            _storeRepositoryMock.Setup(cm => cm.GetBusinessGroupAsync(It.IsAny<string>()))
                .ReturnsAsync(businessGroup);

            _storeRepositoryMock.Setup(cm => cm.UpdateBusinessGroupAsync(It.IsAny<BusinessGroup>())).Callback(() => modified = true);

            StoreService.CreateOrUpdateAsync(storeRequest);

            _storeRepositoryMock.Verify(cr => cr.UpdateBusinessGroupAsync(It.IsAny<BusinessGroup>()), Times.Once);
            _appParametersService.Verify(mock => mock.GetAppParametersAsync(), Times.Once);

            Assert.True(modified);
            Assert.Equal(storeRequest.BusinessGroupName, businessGroup.Name);
        }
    }
}