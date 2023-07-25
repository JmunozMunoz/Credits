using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Credits;
using Sc.Credits.Domain.Managment.Services.Customers;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.EntryPoints.ReactiveWeb.Controllers.V1;
using Sc.Credits.Helper.Test.Model;
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
    public class CreditControllerTest
    {
        private readonly Mock<ICreditService> _creditServiceMock = new Mock<ICreditService>();
        private readonly Mock<ICreditPaymentService> _creditPaymentServiceMock = new Mock<ICreditPaymentService>();
        private readonly Mock<ICommons> _commonsMock = new Mock<ICommons>();
        private readonly Mock<ILoggerService<CreditController>> _loggerServiceMock = new Mock<ILoggerService<CreditController>>();
        private readonly Mock<ICustomerService> _customerServiceMock = new Mock<ICustomerService>();
        private readonly Mock<IAppParametersService> _appParametersServiceMock = new Mock<IAppParametersService>();
        private readonly Mock<ICreditCommonsService> _creditCommonsService = new Mock<ICreditCommonsService>();

        private CreditController CreditController { get; set; }

        public CreditControllerTest()
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

            CreditController = new CreditController(_creditServiceMock.Object,
                _customerServiceMock.Object,
                _creditPaymentServiceMock.Object,
                _commonsMock.Object,
                _loggerServiceMock.Object);

            CreditController.ControllerContext.HttpContext = new DefaultHttpContext();
            CreditController.ControllerContext.HttpContext.Request.Headers[""] = "1,1";
            CreditController.ControllerContext.RouteData = new RouteData();
            CreditController.ControllerContext.RouteData.Values.Add("controller", "Payment");
        }

        [Fact]
        public async Task ShouldGetOriginalAmortizationScheduleWithStatus200()
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

            _creditServiceMock.Setup(repo => repo.GetOriginalAmortizationScheduleAsync(It.IsAny<AmortizationScheduleRequest>()))
                .ReturnsAsync(new AmortizationScheduleResponse());

            CreditController.ControllerContext.RouteData.Values.Add("action", "getOriginalAmortizationSchedule");

            var result = await CreditController.GetOriginalAmortizationSchedule(
                AmortizationScheduleHelperTest.GetAmortizationScheduleRequest(creditValue, initialDate, feeValue, interestRate, frequency,
                   fees, downPayment, assuranceValue, assuranceFeeValue, assuranceTotalFeeValue));

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldGetTimeLimitInMonthsWithStatus200()
        {
            string idDocument = "TestIdDocument";
            string documentType = "CC";
            decimal creditValue = 1000000;
            string storeId = "TestStoreId";

            _creditServiceMock.Setup(repo => repo.GetTimeLimitInMonthsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>()))
                .ReturnsAsync(9);

            CreditController.ControllerContext.RouteData.Values.Add("action", "getTimeLimitInMonths");

            var result = await CreditController.GetTimeLimitInMonths(idDocument, documentType, creditValue, storeId);

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldGetCreditDetailsWithStatus200()
        {
            decimal creditValue = 1000000;
            int months = 4;
            string idDocument = "TestIdDocument";
            string documentType = "CC";
            string storeId = "TestStoreId";
            int frequency = (int)Frequencies.Monthly;

            _creditServiceMock.Setup(repo => repo.GetCreditDetailsAsync(It.IsAny<RequiredInitialValuesForCreditDetail>()))
                .ReturnsAsync(new CreditDetailResponse());

            CreditController.ControllerContext.RouteData.Values.Add("action", "getCreditDetails");

            var result = await CreditController.GetCreditDetails(creditValue, frequency, months, storeId, documentType, idDocument);

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldGetCustomerCreditLimitWithStatus200()
        {
            string idDocument = "TestIdDocument";
            string documentType = "CC";
            string vendorId = "TestVendorId";

            _creditServiceMock.Setup(repo => repo.GetCustomerCreditLimitAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new CustomerCreditLimitResponse());

            CreditController.ControllerContext.RouteData.Values.Add("action", "getCreditLimitClient");

            var result = await CreditController.GetCustomerCreditLimit(documentType, idDocument, vendorId);

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// Test GetCustomerCreditLimitInCrease
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ShouldGetCustomerCreditLimitInCreaseWithStatus200()
        {
            //Arrage
            string idDocument = "TestIdDocument";
            string documentType = "CC";

            _creditServiceMock.Setup(repo => repo.GetCustomerCreditLimitIncreaseAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            CreditController.ControllerContext.RouteData.Values.Add("action", "GetCustomerCreditLimitIncrease");

            //Act
            IActionResult result = await CreditController.GetCustomerCreditLimitIncrease(documentType, idDocument);

            OkObjectResult okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            //Assert
            _creditServiceMock.Verify(repo => repo.GetCustomerCreditLimitIncreaseAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));
            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldGetCustomerCreditLimitValidateWithStatus200()
        {
            string idDocument = "TestIdDocument";
            string documentType = "CC";
            string vendorId = "TestVendorId";
            decimal creditValue = 200000;

            _creditServiceMock.Setup(repo => repo.GetCustomerCreditLimitAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<decimal>()))
                .ReturnsAsync(new CustomerCreditLimitResponse());

            CreditController.ControllerContext.RouteData.Values.Add("action", "getCreditLimitClient");

            var result = await CreditController.GetCustomerCreditLimitValidate(documentType, idDocument, vendorId, creditValue);

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldCreateWithStatus200()
        {
            _creditServiceMock.Setup(repo => repo.CreateAsync(It.IsAny<CreateCreditRequest>()))
                .ReturnsAsync(new CreateCreditResponse());

            CreditController.ControllerContext.RouteData.Values.Add("action", "create");

            var result = await CreditController.Create(new CreateCreditRequest(), "1,1");

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Once);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldUpdateCreditExtraFieldsWithStatus200()
        {
            CreditController.ControllerContext.RouteData.Values.Add("action", "updateCreditExtraFields");

            var result = await CreditController.UpdateCreditExtraFields(new UpdateCreditExtraFieldsRequest());

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _creditServiceMock.Verify(repo => repo.UpdateExtraFieldsAsync(It.IsAny<UpdateCreditExtraFieldsRequest>()), Times.Once);
            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Once);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldGetCreditTokenWithStatus200()
        {
            _creditServiceMock.Setup(repo => repo.GenerateTokenWithRiskLevelCalculationAsync(It.IsAny<GenerateTokenRequest>()))
                .ReturnsAsync(new TokenResponse { Token = new Token() });

            CreditController.ControllerContext.RouteData.Values.Add("action", "getCreditToken");

            var result = await CreditController.GetCreditToken(ModelHelperTest.InstanceModel<GenerateTokenRequest>());

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldGetCreditTokenCallRequestWithStatus200()
        {
            _creditServiceMock.Setup(repo => repo.TokenCallRequestAsync(It.IsAny<CreditTokenCallRequest>()))
                .ReturnsAsync(true);

            CreditController.ControllerContext.RouteData.Values.Add("action", "creditTokenCallRequest");

            var result = await CreditController.CreditTokenCallRequest(new CreditTokenCallRequest());

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldGetCustomerByIdDocumentWithStatus200()
        {
            _customerServiceMock.Setup(mock => mock.GetActiveAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(CustomerHelperTest.GetCustomer());

            CreditController.ControllerContext.RouteData.Values.Add("action", "getCustomerByIdDocument");

            var result = await CreditController.GetCustomerByIdDocument("CC", "TestIdDocument");

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldGetActiveCreditsWithStatus200()
        {
            _creditPaymentServiceMock.Setup(mock => mock.GetActiveCreditsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<DateTime>()))
                .ReturnsAsync(new List<CreditStatus>());

            CreditController.ControllerContext.RouteData.Values.Add("action", "getActiveCredits");

            var result = await CreditController.GetActiveCredits("CC", "TestIdDocument", "TestStoreId");

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldGetListCreditCustomerWithStatus200()
        {
            _creditPaymentServiceMock.Setup(mock => mock.GetActiveCreditsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<DateTime>()))
                .ReturnsAsync(new List<CreditStatus>());

            CreditController.ControllerContext.RouteData.Values.Add("action", "getListCreditCustomer");

            var result = await CreditController.GetListCreditCustomer("CC", "TestIdDocument", "TestStoreId", DateTime.Now);

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldGetDetailedActiveCreditsWithStatus200()
        {
            _creditPaymentServiceMock.Setup(mock => mock.GetDetailedActiveCreditsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<DateTime>()))
                .ReturnsAsync(new List<DetailedCreditStatus>());

            CreditController.ControllerContext.RouteData.Values.Add("action", "getActiveCredits");

            var result = await CreditController.GetDetailedActiveCredits("CC", "TestIdDocument", "TestStoreId", DateTime.Now);

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldGetCompleteCreditsDataWithStatus200()
        {
            _creditPaymentServiceMock.Setup(mock => mock.GetCompleteCreditsDataAsync(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<DateTime>()))
                .ReturnsAsync(new List<DetailedCreditStatus>());

            CreditController.ControllerContext.RouteData.Values.Add("action", "getActiveCredits");

            var result = await CreditController.GetCompleteCreditsData("CC", "TestIdDocument", DateTime.Now);

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldGetDetailedActiveCreditsByCreditMasterIdWithStatus200()
        {
            _creditPaymentServiceMock.Setup(mock => mock.GetDetailedActiveCreditsByCreditMasterIdAsync(It.IsAny<List<Guid>>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<DetailedCreditStatus>());

            List<Guid> creditM = new List<Guid>();
            creditM.Add(new Guid());
            creditM.Add(new Guid());
            creditM.Add(new Guid());

            CreditController.ControllerContext.RouteData.Values.Add("action", "getDetailedActiveCreditsByCreditMasterId");

            var result = await CreditController.GetDetailedActiveCreditsByCreditMasterId(creditM, DateTime.Now);

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldGetPromissoryNoteInfoWithStatus200()
        {
            _creditServiceMock.Setup(mock => mock.GetPromissoryNoteInfoAsync(It.IsAny<Guid>(), false))
                .ReturnsAsync(new PromissoryNoteInfo());

            CreditController.ControllerContext.RouteData.Values.Add("action", "getPromissoryNoteInfo");

            var result = await CreditController.GetPromissoryNoteInfo(Guid.NewGuid(), false);

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldGetPaidCreditCertificateTemplateWithStatus200()
        {
            _creditServiceMock.Setup(mock => mock.GetPaidCreditCertificateTemplatesAsync(It.IsAny<List<Guid>>(), false))
                .ReturnsAsync(new List<string>());

            CreditController.ControllerContext.RouteData.Values.Add("action", "getPaidCreditCertificateTemplate");

            var result = await CreditController.GetPaidCreditCertificateTemplate(new List<Guid>() { Guid.NewGuid() }, false);

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldGetCustomerCreditHistoryWithStatus200()
        {
            string storeId = "TestStoreId";
            string documentType = "CC";
            string idDocument = "TestIdDocument";

            _creditServiceMock.Setup(repo => repo.GetCustomerCreditHistoryAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<CreditHistoryResponse>());

            CreditController.ControllerContext.RouteData.Values.Add("action", "getCustomerCreditHistory");

            var result = await CreditController.GetCustomerCreditHistory(storeId, documentType, idDocument);

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldRejectCreditLimitWithStatus200()
        {
            _customerServiceMock.Setup(mock => mock.RejectCreditLimit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            CreditController.ControllerContext.RouteData.Values.Add("action", "RejectCreditLimit");

            var result = await CreditController.RejectCreditLimit(new RejectCreditLimitRequest());

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Once);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldReturnTrueValidatedToken()
        {
            _creditServiceMock.Setup(mock => mock.ValidateCreditTokenAsync(It.IsAny<CreateCreditRequest>()))
                .ReturnsAsync(true);

            CreditController.ControllerContext.RouteData.Values.Add("action", "ValidateCreditTokenAsync");

            var result = await CreditController.ValidateCreditToken(new CreateCreditRequest(), "1,1");

            var okObjectResult = result as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);

            _loggerServiceMock.Verify(mock => mock.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Once);
            _loggerServiceMock.Verify(mock => mock.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetRegistrationStatusByCustomerTransactionWithStatus200()
        {
            //Arrange
            _creditServiceMock.Setup(mock => mock.ExistCreditRequestById(It.IsAny<Guid>()))
                .ReturnsAsync(true);
            CreditController.ControllerContext.RouteData.Values.Add("action", "GetRegistrationStatusByCustomerTransaction");

            Guid transactionId = new Guid("00BF5060-0247-42F3-BEBD-9B8AFF393A5E");

            //act
            var result = await CreditController.ExistCreditRequest(transactionId);
            //Assert
            var okObjectResult = result as OkObjectResult;
            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.NotNull(result);
        }
    }
}