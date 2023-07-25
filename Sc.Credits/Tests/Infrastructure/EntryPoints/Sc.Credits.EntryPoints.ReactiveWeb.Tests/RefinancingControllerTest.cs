using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Refinancings;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Refinancings;
using Sc.Credits.EntryPoints.ReactiveWeb.Controllers.V1;
using Sc.Credits.Helper.Test.Model;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.EntryPoints.ReactiveWeb.Tests
{
    public class RefinancingControllerTest
    {
        private readonly Mock<IRefinancingService> _refinancingServiceMock = new Mock<IRefinancingService>();
        private readonly Mock<ICommons> _commonsMock = new Mock<ICommons>();
        private readonly Mock<IAppParametersService> _appParametersServiceMock = new Mock<IAppParametersService>();

        private readonly Mock<ILoggerService<RefinancingController>> _loggerServiceMock =
            new Mock<ILoggerService<RefinancingController>>();

        private RefinancingController RefinancingController { get; set; }

        public RefinancingControllerTest()
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

            RefinancingController = new RefinancingController(_refinancingServiceMock.Object,
                _commonsMock.Object,
                _loggerServiceMock.Object);

            RefinancingController.ControllerContext.HttpContext = new DefaultHttpContext();
            RefinancingController.ControllerContext.HttpContext.Request.Headers["Location"] = "1,1";
            RefinancingController.ControllerContext.RouteData = new RouteData();
            RefinancingController.ControllerContext.RouteData.Values.Add("controller", "Payment");
        }

        [Fact]
        public async Task ShouldGetCustomerCreditsWithStatusCode200()
        {
            Guid ApplicationId = Guid.NewGuid();

            CustomerCreditsRequest customerCreditsRequest = new CustomerCreditsRequest()
            {
                ApplicationId = ApplicationId,
                DocumentType = "cc",
                IdDocument = "8177596"
            };

            RefinancingController.ControllerContext.RouteData.Values.Add("action", "GetCustomerCredits");

            var result = await RefinancingController.GetCustomerCredits(customerCreditsRequest);

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _refinancingServiceMock.Verify(repo => repo.GetCustomerCreditsAsync(customerCreditsRequest), Times.Once);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldCalculateFeesWithStatusCode200()
        {
            Guid ApplicationId = Guid.NewGuid();

            CalculateFeesRequest calculateFeesRequest = new CalculateFeesRequest
            {
                ApplicationId = ApplicationId,
                DocumentType = "cc",
                IdDocument = "8177596"
            };

            RefinancingController.ControllerContext.RouteData.Values.Add("action", "CalculateFees");

            var result = await RefinancingController.CalculateFees(calculateFeesRequest);

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _refinancingServiceMock.Verify(repo => repo.CalculateFeesAsync(calculateFeesRequest), Times.Once);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldCreateCreditWithStatusCode200()
        {
            RefinancingCreditRequest refinancingCreditRequest = new RefinancingCreditRequest();

            RefinancingController.ControllerContext.RouteData.Values.Add("action", "CreateCredit");

            var result = await RefinancingController.CreateCredit(refinancingCreditRequest, SCLocation: "1,1");

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _refinancingServiceMock.Verify(mock => mock.CreateCreditAsync(It.IsAny<RefinancingCreditRequest>()), Times.Once);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Once);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldGetRefinancingTokenWithStatusCode200()
        {
            _refinancingServiceMock.Setup(mock => mock.GenerateTokenAsync(It.IsAny<GenerateTokenRequest>()))
                .ReturnsAsync(new TokenResponse
                {
                    Token = new Token()
                });

            RefinancingController.ControllerContext.RouteData.Values.Add("action", "GetRefinancingToken");

            var result = await RefinancingController.GetRefinancingToken(ModelHelperTest.InstanceModel<GenerateTokenRequest>());

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _refinancingServiceMock.Verify(mock => mock.GenerateTokenAsync(It.IsAny<GenerateTokenRequest>()), Times.Once);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }
    }
}