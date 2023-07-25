using credinet.exception.middleware.models;
using Moq;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Credits;
using Sc.Credits.Domain.Managment.Services.Simulator;
using Sc.Credits.Domain.Managment.Services.Stores;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.UseCase.Credits;
using Sc.Credits.Helper.Test.Model;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Sc.Credits.Domain.Model.Stores;
using credinet.exception.middleware.enums;
using Sc.Credits.Domain.Model.Google.Recaptcha;

namespace Sc.Credits.Domain.Managment.Tests.Credits
{
    public class SimulatorServiceTest
    {
        private readonly Mock<IStoreService> _storeServiceMock = new Mock<IStoreService>();
        private readonly Mock<ISimulatorCommonService> _simulatorCommonsServiceMock = new Mock<ISimulatorCommonService>();
        private readonly Mock<IAppParametersService> _appParametersServiceMock = new Mock<IAppParametersService>();
        private readonly Mock<ICreditUsesCase> _creditsUsesCaseMock = new Mock<ICreditUsesCase>();
        private readonly Mock<ILoggerService<SimulatorService>> _loggerServiceMock = new Mock<ILoggerService<SimulatorService>>();
        private readonly Mock<ICommons> _commonsMock = new Mock<ICommons>();
        private readonly Mock<IReCaptchaGateway> _reCaptchaMock= new Mock<IReCaptchaGateway>();

        private SimulatorService simulatorService;

        private SimulatorCommons simulatorCommons =>
    SimulatorCommonsHelperTest.Create(_creditsUsesCaseMock,
        _simulatorCommonsServiceMock);

        public SimulatorServiceTest() 
        {
            _appParametersServiceMock.Setup(mock => mock.GetSettings())
                .Returns(new CredinetAppSettings
                {
                    PromissoryNotePath = "promissorynotes",
                    PdfBlobContainerName = "pdf",
                    ValidateTokenOnCreate = true,
                    RefinancingSourcesAllowed = "5"
                });

            _commonsMock.SetupGet(mock => mock.AppParameters)
                .Returns(_appParametersServiceMock.Object);

            _simulatorCommonsServiceMock.SetupGet(mock => mock.Commons)
                    .Returns(_commonsMock.Object);

            _simulatorCommonsServiceMock.SetupGet(mock => mock.StoreService)
            .Returns(_storeServiceMock.Object);


            _commonsMock.SetupGet(mock => mock.CredinetAppSettings)
                .Returns(_appParametersServiceMock.Object.GetSettings());

            _simulatorCommonsServiceMock.Setup(mock => mock.Commons.CredinetAppSettings)
                .Returns(CredinetAppSettingsHelperTest.GetCredinetAppSettings());

            simulatorService = new SimulatorService(simulatorCommons,
                _reCaptchaMock.Object, _loggerServiceMock.Object);
        }

        /// <summary>
        /// Gets the credit details asynchronous when store identifier is empty then expect business exception to be request values invalid.
        /// </summary>
        [Fact]
        public async Task GetCreditDetailsAsyncWhenStoreIdIsEmptyThenExpectBusinessExceptionToBeRequestValuesInvalid()
        {
            //Arrange
            int frequency = (int)Frequencies.Monthly;
            int months = 4;
            decimal creditValue = 250000;

            RequiredInitialValuesForCreditSimulation RequiredValues = new RequiredInitialValuesForCreditSimulation()
            {
                creditValue = creditValue,
                months = months,
                storeId = "",
                frequency = frequency
            };

            //Act
            //Assert
            var result = await Assert.ThrowsAsync<BusinessException>(async () => await simulatorService.GetCreditDetailsAsync(RequiredValues));

        }

        /// <summary>
        /// Gets the credit details asynchronous when credit value is zero then expect business exception to be request values invalid.
        /// </summary>
        [Fact]
        public async Task GetCreditDetailsAsyncWhenCreditValueIsZeroThenExpectBusinessExceptionToBeRequestValuesInvalid()
        {
            //Arrange
            int frequency = (int)Frequencies.Monthly;
            int months = 4;

            RequiredInitialValuesForCreditSimulation RequiredValues = new RequiredInitialValuesForCreditSimulation()
            {
                creditValue = 0,
                months = months,
                storeId = "5d0d0e361477572ee0326f97",
                frequency = frequency
            };

            //Act
            //Assert
            var result = await Assert.ThrowsAsync<BusinessException>(async () => await simulatorService.GetCreditDetailsAsync(RequiredValues));

        }

        /// <summary>
        /// Gets the credit details asynchronous when frequency is not valid expect business exception to be request values invalid.
        /// </summary>
        [Fact]
        public async Task GetCreditDetailsAsyncWhenFrequencyIsNotValidExpectBusinessExceptionToBeRequestValuesInvalid()
        {
            //Arrange
            int frequency = 10;
            int months = 4;
            decimal creditValue = 250000;
            Store store = StoreHelperTest.GetStore();

            _storeServiceMock.Setup(ce => ce.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(store);

            _appParametersServiceMock.Setup(cr => cr.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            RequiredInitialValuesForCreditSimulation RequiredValues = new RequiredInitialValuesForCreditSimulation()
            {
                creditValue = creditValue,
                months = months,
                storeId = "5d0d0e361477572ee0326f97",
                frequency = frequency
            };

            //Act
            //Assert
            var result = await Assert.ThrowsAsync<BusinessException>(async () => await simulatorService.GetCreditDetailsAsync(RequiredValues));

            _storeServiceMock.Verify(storeService => storeService.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once());
            _appParametersServiceMock.Verify(paramsService => paramsService.GetAppParametersAsync(), Times.Once());

        }

        /// <summary>
        /// Gets the credit details asynchronous when all data is right then expect credit detail response.
        /// </summary>
        [Fact]
        public async Task GetCreditDetailsAsyncWhenAllDataIsRightThenExpectCreditDetailResponse()
        {
            //Arrange
            CreditDetailResponse creditDetail = CreditHelperTest.GetCreditDetails();
            int frequency = 15;
            int months = 4;
            decimal creditValue = 250000;
            Store store = StoreHelperTest.GetStore();

            _storeServiceMock.Setup(ce => ce.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(store);

            _appParametersServiceMock.Setup(cr => cr.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditsUsesCaseMock.Setup(useCase => useCase.GetCreditDetails(It.Is<SimulatedCreditRequest>(Simulation =>
                                                                                                              Simulation.Store == store &&
                                                                                                              Simulation.CreditValue == creditValue &&
                                                                                                              Simulation.Frequency == (Frequencies)frequency &&
                                                                                                              Simulation.Fees == (months * 2)

                ))).Returns(creditDetail).Verifiable();

            RequiredInitialValuesForCreditSimulation RequiredValues = new RequiredInitialValuesForCreditSimulation()
            {
                creditValue = creditValue,
                months = months,
                storeId = "5d0d0e361477572ee0326f97",
                frequency = frequency
            };

            //Act            
            var result = await simulatorService.GetCreditDetailsAsync(RequiredValues);

            //Assert
            _creditsUsesCaseMock.VerifyAll();
            _storeServiceMock.Verify(storeService => storeService.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once());
            _appParametersServiceMock.Verify(paramsService => paramsService.GetAppParametersAsync(), Times.Once());

        }

        /// <summary>
        /// Gets the total fee value when store identifier is empty then expect business exception to be request values invalid.
        /// </summary>
        [Fact]
        public async Task GetTotalFeeValueWhenStoreIdIsEmptyThenExpectBusinessExceptionToBeRequestValuesInvalid()
        {
            //Arrange
            int frequency = (int)Frequencies.Monthly;
            int months = 4;
            decimal creditValue = 250000;

            RequiredInitialValuesForBasicSimulation RequiredValues = new RequiredInitialValuesForBasicSimulation()
            {
                creditValue = creditValue,
                storeId = "",
                frequency = frequency
            };

            //Act
            //Assert
            var result = await Assert.ThrowsAsync<BusinessException>(async () => await simulatorService.GetTotalFeeValue(RequiredValues));

            Assert.Equal(result.Message, BusinessResponse.RequestValuesInvalid.ToString());

        }


        /// <summary>
        /// Gets the total fee value when credit value is zero then expect business exception to be request values invalid.
        /// </summary>
        [Fact]
        public async Task GetTotalFeeValueWhenCreditValueIsZeroThenExpectBusinessExceptionToBeRequestValuesInvalid()
        {
            //Arrange
            int frequency = (int)Frequencies.Monthly;
            int months = 4;

            RequiredInitialValuesForBasicSimulation RequiredValues = new RequiredInitialValuesForBasicSimulation()
            {
                creditValue = 0,
                storeId = "5d0d0e361477572ee0326f97",
                frequency = frequency
            };

            //Act
            //Assert
            var result = await Assert.ThrowsAsync<BusinessException>(async () => await simulatorService.GetTotalFeeValue(RequiredValues));

            Assert.Equal(result.Message, BusinessResponse.RequestValuesInvalid.ToString());

        }

        /// <summary>
        /// Gets the total fee value when all data is right then expect simulation details response.
        /// </summary>
        [Fact]
        public async Task GetTotalFeeValueWhenAllDataIsRightThenExpectSimulationDetailsResponse()
        {
            //Arrange
            CreditDetailResponse creditDetail = CreditHelperTest.GetCreditDetails();
            decimal creditValue = 250000;
            Store store = StoreHelperTest.GetStore();

            _storeServiceMock.Setup(ce => ce.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(store);

            _appParametersServiceMock.Setup(cr => cr.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditsUsesCaseMock.Setup(useCase => useCase.GetCreditDetails(It.Is<SimulatedCreditRequest>(Simulation =>
                                                                                                              Simulation.Store == store &&
                                                                                                              Simulation.CreditValue == creditValue &&
                                                                                                              Simulation.Fees == (int)SimulationMonths.Default

                ))).Returns(creditDetail).Verifiable();

            RequiredInitialValuesForBasicSimulation RequiredValues = new RequiredInitialValuesForBasicSimulation()
            {
                creditValue = creditValue,
                storeId = "5d0d0e361477572ee0326f97"
            };

            //Act            
            var result = await simulatorService.GetTotalFeeValue(RequiredValues);

            //Assert
            _creditsUsesCaseMock.VerifyAll();
            _storeServiceMock.Verify(storeService => storeService.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once());
            _appParametersServiceMock.Verify(paramsService => paramsService.GetAppParametersAsync(), Times.Once());
            Assert.IsType<SimulationDetailsResponse>(result);
            Assert.Equal(creditDetail.TotalFeeValue, result.TotalFeeValue);
            Assert.Equal(creditDetail.Fees, result.Fees);

        }

        /// <summary>
        /// Gets the time limit in months asynchronous when store identifier is empty then expect limith months.
        /// </summary>
        [Fact]
        public async Task GetTimeLimitInMonthsAsync_When_StoreIdIsEmpty_Then_ExpectLimithMonths()
        {
            //Arrange
            string storeId = "";
            decimal creditValue = 250000;
            Store store = StoreHelperTest.GetStore();
            int months = 4;
            int decimalNumbersRound = 1;

            LimitMonhsInitialValuesRequest RequiredValues = new LimitMonhsInitialValuesRequest()
            {
                creditValue = creditValue,
                storeId = storeId
            };

            _storeServiceMock.Setup(ce => ce.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
             It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
               .ReturnsAsync(store);

            _appParametersServiceMock.Setup(cr => cr.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditsUsesCaseMock.Setup(useCase => useCase.GetTimeLimitInMonths(It.IsAny<decimal>(), It.IsAny<Store>(), It.IsAny<int>())).Returns(months);

            //Act
            
            var result = await simulatorService.GetTimeLimitInMonthsAsync(RequiredValues);

            //Assert
            _storeServiceMock.Verify(storeService => storeService.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
            It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once());
            _appParametersServiceMock.Verify(paramsService => paramsService.GetAppParametersAsync(), Times.Once());
            Assert.IsType<int>(result);
        }

        /// <summary>
        /// Gets the time limit in months asynchronous when store identifier is not empty then expect limith months.
        /// </summary>
        [Fact]
        public async Task GetTimeLimitInMonthsAsync_When_StoreIdIsNotEmpty_Then_ExpectLimithMonths()
        {
            //Arrange
            string storeId = "storeID";
            decimal creditValue = 250000;
            Store store = StoreHelperTest.GetStore();
            int months = 4;

            LimitMonhsInitialValuesRequest RequiredValues = new LimitMonhsInitialValuesRequest()
            {
                creditValue = creditValue,
                storeId = storeId
            };

            _storeServiceMock.Setup(ce => ce.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
             It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
               .ReturnsAsync(store);

            _appParametersServiceMock.Setup(cr => cr.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditsUsesCaseMock.Setup(useCase => useCase.GetTimeLimitInMonths(It.IsAny<decimal>(), It.IsAny<Store>(), It.IsAny<int>())).Returns(months);

            //Act

            var result = await simulatorService.GetTimeLimitInMonthsAsync(RequiredValues);

            //Assert
            _storeServiceMock.Verify(storeService => storeService.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
            It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once());
            _appParametersServiceMock.Verify(paramsService => paramsService.GetAppParametersAsync(), Times.Once());
            Assert.IsType<int>(result);
        }

        /// <summary>
        /// Gets the time limit in months asynchronous when credit value is negative then business exception to be request values invalid.
        /// </summary>
        [Fact]
        public async Task GetTimeLimitInMonthsAsync_When_CreditValueIsNegative_Then_BusinessExceptionToBeRequestValuesInvalid()
        {
            //Arrange
            string storeId = "";
            decimal creditValue = -250000;
            Store store = StoreHelperTest.GetStore();
            int months = 4;

            LimitMonhsInitialValuesRequest RequiredValues = new LimitMonhsInitialValuesRequest()
            {
                creditValue = creditValue,
                storeId = storeId
            };

            _storeServiceMock.Setup(ce => ce.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
             It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
               .ReturnsAsync(store);

            _appParametersServiceMock.Setup(cr => cr.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditsUsesCaseMock.Setup(useCase => useCase.GetTimeLimitInMonths(It.IsAny<decimal>(), It.IsAny<Store>(), It.IsAny<int>())).Returns(months);

            //Act
            //Assert
            var result = await Assert.ThrowsAsync<BusinessException>(async () => await simulatorService.GetTimeLimitInMonthsAsync(RequiredValues));

            Assert.Equal(result.Message, BusinessResponse.RequestValuesInvalid.ToString());
        }

        /// <summary>
        /// Gets the store minimum and maximum credit limit when store identifier is empty then expect store category range.
        /// </summary>
        [Fact]
        public async Task GetStoreMinAndMaxCreditLimit_When_StoreIdIsEmpty_Then_ExpectStoreCategoryRange()
        {
            //Arrange
            string storeId = "";
            decimal creditValue = 250000;
            Store store = StoreHelperTest.GetStore();
            int months = 4;
            int decimalNumbersRound = 1;

            StoreCategoryRange category = new StoreCategoryRange()
            {
                GetMaximumCreditValue = 3000,
                GetMinimumFeeValue = 2000
            };

            _storeServiceMock.Setup(ce => ce.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
             It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
               .ReturnsAsync(store);

            _appParametersServiceMock.Setup(cr => cr.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditsUsesCaseMock.Setup(useCase => useCase.GetStoreMinAndMaxCreditLimitByStoreId(It.IsAny<Store>(), It.IsAny<int>())).Returns(category);

            //Act

            var result = await simulatorService.GetStoreMinAndMaxCreditLimitByStoreId(storeId);

            //Assert
            _storeServiceMock.Verify(storeService => storeService.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
            It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once());
            _appParametersServiceMock.Verify(paramsService => paramsService.GetAppParametersAsync(), Times.Once());
            Assert.IsType<StoreCategoryRange>(result);
        }

        /// <summary>
        /// Gets the store minimum and maximum credit limit when store identifier is not empty then expect store category range.
        /// </summary>
        [Fact]
        public async Task GetStoreMinAndMaxCreditLimit_When_StoreIdIsNotEmpty_Then_ExpectStoreCategoryRange()
        {
            //Arrange
            string storeId = "StoreId";
            decimal creditValue = 250000;
            Store store = StoreHelperTest.GetStore();
            int months = 4;
            int decimalNumbersRound = 1;

            StoreCategoryRange category = new StoreCategoryRange()
            {
                GetMaximumCreditValue = 3000,
                GetMinimumFeeValue = 2000
            };

            _storeServiceMock.Setup(ce => ce.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
             It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
               .ReturnsAsync(store);

            _appParametersServiceMock.Setup(cr => cr.GetAppParametersAsync())
                .ReturnsAsync(ParameterHelperTest.GetAppParameters());

            _creditsUsesCaseMock.Setup(useCase => useCase.GetStoreMinAndMaxCreditLimitByStoreId(It.IsAny<Store>(), It.IsAny<int>())).Returns(category);

            //Act

            var result = await simulatorService.GetStoreMinAndMaxCreditLimitByStoreId(storeId);

            //Assert
            _storeServiceMock.Verify(storeService => storeService.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>(),
            It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once());
            _appParametersServiceMock.Verify(paramsService => paramsService.GetAppParametersAsync(), Times.Once());
            Assert.IsType<StoreCategoryRange>(result);
        }




    }
}
