using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Credits;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.EntryPoints.ReactiveWeb.Controllers.V1;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.EntryPoints.ReactiveWeb.Tests
{
    public class CancelControllerTest
    {
        private readonly Mock<ICancelCreditService> _cancelCreditServiceMock = new Mock<ICancelCreditService>();
        private readonly Mock<ICancelPaymentService> _cancelPaymentServiceMock = new Mock<ICancelPaymentService>();
        private readonly Mock<ICommons> _commonsMock = new Mock<ICommons>();
        private readonly Mock<ILoggerService<CancelController>> _loggerServiceMock = new Mock<ILoggerService<CancelController>>();
        private readonly Mock<IAppParametersService> _appParametersServiceMock = new Mock<IAppParametersService>();
        private readonly Mock<IPartialCancelationCreditService> _partialCancelationCreditService = new Mock<IPartialCancelationCreditService>();

        private CancelController CancelController { get; set; }

        public CancelControllerTest()
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

            CancelController = new CancelController(_cancelCreditServiceMock.Object,
                _cancelPaymentServiceMock.Object,
                _commonsMock.Object,
                _partialCancelationCreditService.Object,
                _loggerServiceMock.Object);

            CancelController.ControllerContext.HttpContext = new DefaultHttpContext();
            CancelController.ControllerContext.HttpContext.Request.Headers["Location"] = "1,1";
            CancelController.ControllerContext.RouteData = new RouteData();
            CancelController.ControllerContext.RouteData.Values.Add("controller", "Cancel");
        }

        [Fact]
        public async Task ShoudSendRequestCancelCreditWithStatus200()
        {
            CancelCreditRequest cancelCreditRequest = new CancelCreditRequest()
            {
                CreditId = Guid.NewGuid(),
                StoreId = "123456",
                UserId = "123123",
                Reason = "Prueba"
            };

            CancelController.ControllerContext.RouteData.Values.Add("action", "requestCancelCredit");

            var result = await CancelController.RequestCancelCredit(cancelCreditRequest);

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Once);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldSendRequestCancelPaymentWithStatus200()
        {
            CancelPaymentRequest cancelPaymentRequest = new CancelPaymentRequest()
            {
                PaymentId = Guid.NewGuid(),
                StoreId = "123456",
                UserId = "123456",
                Reason = "Prueba"
            };

            CancelController.ControllerContext.RouteData.Values.Add("action", "requestCancelPayment");

            var result = await CancelController.RequestCancelPayment(cancelPaymentRequest);

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Once);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldListCreditsPendingCancellationWithStatus200()
        {
            _cancelCreditServiceMock.Setup(mock => mock.GetPendingsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .ReturnsAsync(new CancelCreditDetailResponsePaged());

            CancelController.ControllerContext.RouteData.Values.Add("action", "getListCreditsPendingCancellation");

            var result = await CancelController.GetListCreditsPendingCancellation("TestVendorId", 1, 1);

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldListPaymentsPendingCancellationWithStatus200()
        {
            _cancelPaymentServiceMock.Setup(mock => mock.GetPendingsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .ReturnsAsync(new CancelPaymentDetailResponsePaged());

            CancelController.ControllerContext.RouteData.Values.Add("action", "getListPaymentsPendingCancellation");

            var result = await CancelController.GetListPaymentsPendingCancellation("TestVendorId", 1, 1);

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldGetActiveAndPendingCancellationCreditsWithStatus200()
        {
            _cancelCreditServiceMock.Setup(mock => mock.GetActiveAndPendingCancellationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<ActivePendingCancellationCreditResponse>());

            CancelController.ControllerContext.RouteData.Values.Add("action", "getActiveAndPendingCancellationCredits");

            var result = await CancelController.GetActiveAndPendingCancellationCredits("CC", "TestIdDocument", "TestStoreId");

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldGetActiveAndPendingCancellationPaymentsWithStatus200()
        {
            _cancelPaymentServiceMock.Setup(mock => mock.GetActiveAndPendingCancellationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<ActivePendingCancellationPaymentResponse>());

            CancelController.ControllerContext.RouteData.Values.Add("action", "getActiveAndPendingCancellationPayments");

            var result = await CancelController.GetActiveAndPendingCancellationPayments("CC", "TestIdDocument", "TestStoreId");

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldCancelCreditWithStatus200()
        {
            CancelController.ControllerContext.RouteData.Values.Add("action", "cancelCredit");

            var result = await CancelController.CancelCredit(new CancelCredit());

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Once);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldCancelPaymentsWithStatus200()
        {
            CancelController.ControllerContext.RouteData.Values.Add("action", "cancelPayments");

            var result = await CancelController.CancelPayments(new CancelPayments());

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Once);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldRejectRequestCancelCreditWithStatus200()
        {
            CancelController.ControllerContext.RouteData.Values.Add("action", "rejectRequestCancelCredit");

            var result = await CancelController.RejectRequestCancelCredit(new CancelCredit());

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Once);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldRejectRequestCancelPaymentsWithStatus200()
        {
            CancelController.ControllerContext.RouteData.Values.Add("action", "cancelPayments");

            var result = await CancelController.RejectRequestCancelPayments(new CancelPayments());

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Once);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetValidationToPartiallyCancelACreditWithStatus200()
        {
            _partialCancelationCreditService.Setup(mock => mock.GetValidationToPartiallyCancelACreditAsync(It.IsAny<PartialCancellationRequest>()))
              .ReturnsAsync(true);

            CancelController.ControllerContext.RouteData.Values.Add("action", "GetValidationToPartiallyCancelACredit");

            var result = await CancelController.ValidationToPartiallyCancelACredit(new PartialCancellationRequest());

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetValidationToPartiallyCancelACreditThrowException()
        {
            _partialCancelationCreditService.Setup(mock => mock.GetValidationToPartiallyCancelACreditAsync(It.IsAny<PartialCancellationRequest>()))
              .ThrowsAsync(new Exception());

            CancelController.ControllerContext.RouteData.Values.Add("action", "GetValidationToPartiallyCancelACredit");

            BusinessException exception = await Assert.ThrowsAsync<BusinessException>(() => CancelController.ValidationToPartiallyCancelACredit(new PartialCancellationRequest()));

            Assert.Equal((int)BusinessResponse.NotControlledException, exception.code);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Once);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }
    }
}