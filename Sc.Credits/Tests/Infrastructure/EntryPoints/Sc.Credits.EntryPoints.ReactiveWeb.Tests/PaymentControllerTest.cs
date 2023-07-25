using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Credits;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.EntryPoints.ReactiveWeb.Controllers.V1;
using Sc.Credits.Helper.Test.Model;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace Sc.Credits.EntryPoints.ReactiveWeb.Tests
{
    public class PaymentControllerTest
    {
        private readonly Mock<ICreditPaymentService> _creditPaymentServiceMock = new Mock<ICreditPaymentService>();
        private readonly Mock<ICommons> _commonsMock = new Mock<ICommons>();
        private readonly Mock<IAppParametersService> _appParametersServiceMock = new Mock<IAppParametersService>();

        private readonly Mock<ILoggerService<PaymentController>> _loggerServiceMock =
            new Mock<ILoggerService<PaymentController>>();

        private PaymentController PaymentController { get; set; }

        public PaymentControllerTest()
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

            PaymentController = new PaymentController(_creditPaymentServiceMock.Object,
                _commonsMock.Object,
                _loggerServiceMock.Object);

            PaymentController.ControllerContext.HttpContext = new DefaultHttpContext();
            PaymentController.ControllerContext.HttpContext.Request.Headers["Location"] = "1,1";
            PaymentController.ControllerContext.RouteData = new RouteData();
            PaymentController.ControllerContext.RouteData.Values.Add("controller", "Payment");
        }

        [Fact]
        public async Task ShouldGetPaymentFeesWithStatusCode200()
        {
            Guid getPaymentFeesRequest = Guid.NewGuid();

            PaymentController.ControllerContext.RouteData.Values.Add("action", "getPaymentFees");

            var result = await PaymentController.GetPaymentFees(getPaymentFeesRequest);

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _creditPaymentServiceMock.Verify(repo => repo.GetPaymentFeesAsync(getPaymentFeesRequest), Times.Once);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldGetPaymentTemplatesWithStatusCode200()
        {
            List<Guid> paymentIds = new List<Guid> { Guid.NewGuid() };

            PaymentController.ControllerContext.RouteData.Values.Add("action", "getPaymentTemplates");

            var result = await PaymentController.GetPaymentTemplates(paymentIds, false);

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _creditPaymentServiceMock.Verify(mock => mock.GetPaymentTemplatesAsync(paymentIds, false), Times.Once);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldPayCreditWithStatusCode200()
        {
            PaymentController.ControllerContext.RouteData.Values.Add("action", "payCredit");

            var result = await PaymentController.PayCredit(new PaymentCreditRequest(), "1,1");

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Once);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldPayCreditBankWithStatusCode200()
        {
            PaymentController.ControllerContext.RouteData.Values.Add("action", "payCreditBank");

            var result = await PaymentController.PayCreditBank(new PaymentCreditRequestComplete(), "1,1");

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Once);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldPayCreditMultipleWithStatusCode200()
        {
            PaymentController.ControllerContext.RouteData.Values.Add("action", "payCreditMultiple");

            var result = await PaymentController.PayCreditMultiple(new PaymentCreditMultipleRequest
            {
                CreditPaymentDetails = new List<PaymentCreditMultipleDetail>()
            }, "1,1");

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Once);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldGetCurrentAmortizationScheduleWithStatus200()
        {
            decimal creditValue = 1000000;
            DateTime initialDate = DateTime.Now;
            decimal feeValue = 10;
            decimal interestRate = 0;
            int frequency = 12;
            int fees = 10;
            decimal downPayment = 0;
            decimal assuranceValue = 0;
            decimal assuranceFeeValue = 0;
            decimal assuranceTotalFeeValue = 0;
            DateTime calculationDate = DateTime.Now;
            DateTime lastPaymentDate = DateTime.Now;
            decimal balance = 100000;
            decimal assuranceBalance = 5000;
            bool hasArrearsCharge = false;
            decimal arrearsCharges = 0;

            _creditPaymentServiceMock.Setup(repo => repo.GetCurrentAmortizationScheduleAsync(It.IsAny<CurrentAmortizationScheduleRequest>()))
                .ReturnsAsync(new CurrentAmortizationScheduleResponse());

            PaymentController.ControllerContext.RouteData.Values.Add("action", "getCurrentAmortizationSchedule");

            var result = await PaymentController.GetCurrentAmortizationSchedule(
                 AmortizationScheduleHelperTest.GetCurrentAmortizationScheduleRequest(creditValue, balance, initialDate, feeValue, interestRate, frequency,
                   fees, downPayment, assuranceValue, assuranceFeeValue, assuranceTotalFeeValue, assuranceBalance, calculationDate, lastPaymentDate,
                   hasArrearsCharge, arrearsCharges));

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldGetCustomerPaymentHistoryWithStatus200()
        {
            string storeId = "TestStoreId";
            string documentType = "CC";
            string idDocument = "TestIdDocument";

            _creditPaymentServiceMock.Setup(repo => repo.GetCustomerPaymentHistoryAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<PaymentHistoryResponse>());

            PaymentController.ControllerContext.RouteData.Values.Add("action", "getCustomerPaymentHistory");

            var result = await PaymentController.GetCustomerPaymentHistory(storeId, documentType, idDocument);

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldGetCurrentPaymentScheduleWithStatus200()
        {
            CurrentPaymentScheduleRequest currentPaymentSchedule = CurrentPaymentScheduleHelperTest.GetCurrentPaymentScheduleRequest();
            _creditPaymentServiceMock.Setup(mock => mock.GetCurrentPaymentScheduleAsync(It.IsAny<CurrentPaymentScheduleRequest>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new CurrentPaymentScheduleResponse());

            PaymentController.ControllerContext.RouteData.Values.Add("action", "getCurrentPaymentSchedule");

            var result = await PaymentController.GetCurrentPaymentSchedule(currentPaymentSchedule, DateTime.Now);

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldGetCurrentPaymentScheduleWithException()
        {
            CurrentPaymentScheduleRequest currentPaymentSchedule = CurrentPaymentScheduleHelperTest.GetCurrentPaymentScheduleRequest();
            _creditPaymentServiceMock.Setup(mock => mock.GetCurrentPaymentScheduleAsync(It.IsAny<CurrentPaymentScheduleRequest>(), It.IsAny<DateTime>()))
                .Throws(new Exception());

            PaymentController.ControllerContext.RouteData.Values.Add("action", "getCurrentPaymentSchedule");

            var exception = await Assert.ThrowsAsync<BusinessException>(async () => await PaymentController.GetCurrentPaymentSchedule(currentPaymentSchedule, DateTime.Now));

            Assert.Equal((int)BusinessResponse.NotControlledException, exception.code);
            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Once);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }
    }
}