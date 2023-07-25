using credinet.exception.middleware.models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Simulator;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.EntryPoints.ReactiveWeb.Controllers.V1;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.EntryPoints.ReactiveWeb.Tests
{
    public class CreditSimulatorControllerTest
    {
        private readonly Mock<ISimulatorService> _simulatorServiceMock = new Mock<ISimulatorService>();
        private readonly Mock<ICommons> _commonsMock = new Mock<ICommons>();
        private readonly Mock<ILoggerService<CreditSimulatorController>> _loggerServiceMock = new Mock<ILoggerService<CreditSimulatorController>>();
        private readonly Mock<IAppParametersService> _appParametersServiceMock = new Mock<IAppParametersService>();

        private CreditSimulatorController SimulatorController { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreditSimulatorControllerTest"/> class.
        /// </summary>
        public CreditSimulatorControllerTest()
        {
            _appParametersServiceMock.Setup(settings => settings.GetSettings())
                .Returns(new CredinetAppSettings
                {
                    CultureInfo = "es-CO"
                });

            _commonsMock.SetupGet(mock => mock.AppParameters)
                .Returns(_appParametersServiceMock.Object);

            _commonsMock.SetupGet(mock => mock.CredinetAppSettings)
                .Returns(_appParametersServiceMock.Object.GetSettings());

            SimulatorController = new CreditSimulatorController(_simulatorServiceMock.Object,
                _commonsMock.Object,
                _loggerServiceMock.Object);

            SimulatorController.ControllerContext.HttpContext = new DefaultHttpContext();
            SimulatorController.ControllerContext.HttpContext.Request.Headers[""] = "1,1";
            SimulatorController.ControllerContext.RouteData = new RouteData();
            SimulatorController.ControllerContext.RouteData.Values.Add("controller", "Payment");
        }

        /// <summary>
        /// Gets the credit details when all data is correct then expect status ok.
        /// </summary>
        [Fact]
        public async Task GetCreditDetailsWhenAllDataIsCorrectThenExpectStatusOk()
        {
            decimal creditValue = 1000000;
            int months = 4;
            string storeId = "TestStoreId";
            int frequency = (int)Frequencies.Monthly;

            _simulatorServiceMock.Setup(service => service.GetCreditDetailsAsync(It.IsAny<RequiredInitialValuesForCreditSimulation>()))
                .ReturnsAsync(new CreditDetailResponse());

            SimulatorController.ControllerContext.RouteData.Values.Add("action", "getCreditDetails");

            var result = await SimulatorController.GetCreditDetails(creditValue, frequency, months, storeId);

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }


        /// <summary>
        /// Gets the total fee value when all data is correct then expect status ok.
        /// </summary>
        [Fact]
        public async Task GetTotalFeeValueWhenAllDataIsCorrectThenExpectStatusOk()
        {
            //Arrange
            decimal creditValue = 1000000;
            string storeId = "TestStoreId";


            _simulatorServiceMock.Setup(service => service.GetTotalFeeValue(It.Is<RequiredInitialValuesForBasicSimulation>(info =>
                                                                                                                 info.storeId == storeId &&
                                                                                                                 info.creditValue == creditValue ))).ReturnsAsync(new SimulationDetailsResponse()).Verifiable();


            SimulatorController.ControllerContext.RouteData.Values.Add("action", "getCreditDetails");

            //Act
            var result = await SimulatorController.GetTotalFeeValue(creditValue, storeId);

            var okObjectResult = result as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _simulatorServiceMock.VerifyAll();
            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }


        [Fact]
        public async Task GetTimeLimitInMonths_When_AllDataIsCorrect_Then_ExpectStatusOk()
        {
            decimal creditValue = 1000000;
            int months = 4;
            string storeId = "TestStoreId";

            _simulatorServiceMock.Setup(service => service.GetTimeLimitInMonthsAsync(It.IsAny<LimitMonhsInitialValuesRequest>()))
                .ReturnsAsync(months);

            SimulatorController.ControllerContext.RouteData.Values.Add("action", "GetTimeLimitInMonths");

            var result = await SimulatorController.GetTimeLimitInMonthsByValueAndStore(creditValue, storeId );

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetStoreMinAndMaxCreditLimit_When_AllDataIsCorrect_Then_ExpectStatusOk()
        {
            decimal creditValue = 1000000;
            int months = 4;
            string storeId = "TestStoreId";

            StoreCategoryRange category = new StoreCategoryRange() {
                GetMinimumFeeValue = 1000,
                GetMaximumCreditValue = 3000 
            };

            _simulatorServiceMock.Setup(service => service.GetStoreMinAndMaxCreditLimitByStoreId(It.IsAny<string>()))
                .ReturnsAsync(category);

            SimulatorController.ControllerContext.RouteData.Values.Add("action", "GetStoreMinAndMaxCreditLimit");

            var result = await SimulatorController.GetStoreMinAndMaxCreditLimitByStoreId(storeId);

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

    }
}
