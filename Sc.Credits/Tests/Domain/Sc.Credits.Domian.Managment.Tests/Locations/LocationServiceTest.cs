using Moq;
using Sc.Credits.Domain.Managment.Services.Locations;
using Sc.Credits.Domain.Model.Locations;
using Sc.Credits.Domain.Model.Locations.Gateway;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Helper.Test.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace Sc.Credits.Domain.Managment.Tests.Locations
{
    public class LocationServiceTest
    {
        private readonly Mock<IStateRepository> _stateRepositoryMock = new Mock<IStateRepository>();
        private readonly Mock<ICityRepository> _cityRepositoryMock = new Mock<ICityRepository>();

        public ILocationService LocationService =>
            new LocationService(_stateRepositoryMock.Object,
                _cityRepositoryMock.Object);

        [Fact]
        public async Task ShouldAddState()
        {
            State state = null;
            StateRequest stateRequest = LocationHelperTest.GetSateResquest();

            _stateRepositoryMock.Setup(mock => mock.GetByIdAsync(It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(state);

            await LocationService.AddOrUpdateStateAsync(stateRequest);

            _stateRepositoryMock.Verify(mock => mock.AddAsync(It.IsAny<State>(),
                It.IsAny<Transaction>()), Times.Once);
        }

        [Fact]
        public async Task ShouldUpdateState()
        {
            State state = LocationHelperTest.GetSate();
            StateRequest stateRequest = new StateRequest
            {
                Code = "01",
                Name = "Antioquia updated"
            };

            _stateRepositoryMock.Setup(mock => mock.GetByIdAsync(It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(state);

            await LocationService.AddOrUpdateStateAsync(stateRequest);

            _stateRepositoryMock.Verify(mock => mock.UpdateAsync(It.IsAny<State>(),
                It.IsAny<IEnumerable<Field>>(), It.IsAny<Transaction>()), Times.Once);
        }

        [Fact]
        public async Task ShouldAddCity()
        {
            City city = null;
            CityRequest cityRequest = LocationHelperTest.GetCityResquest();
            State state = LocationHelperTest.GetSate();

            _cityRepositoryMock.Setup(mock => mock.GetByIdAsync(It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(city);

            await LocationService.AddOrUpdateCityAsync(cityRequest, state);

            _cityRepositoryMock.Verify(mock => mock.AddAsync(It.IsAny<City>(),
                It.IsAny<Transaction>()), Times.Once);
        }

        [Fact]
        public async Task ShouldUpdateCity()
        {
            City city = LocationHelperTest.GetCity();
            CityRequest cityRequest = new CityRequest
            {
                Code = "001",
                Name = "Medellín updated"
            };
            State state = LocationHelperTest.GetSate();

            _cityRepositoryMock.Setup(mock => mock.GetByIdAsync(It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(city);

            await LocationService.AddOrUpdateCityAsync(cityRequest, state);

            _cityRepositoryMock.Verify(mock => mock.UpdateAsync(It.IsAny<City>(),
                It.IsAny<IEnumerable<Field>>(), It.IsAny<Transaction>()), Times.Once);
        }
    }
}