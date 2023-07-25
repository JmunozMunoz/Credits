using Sc.Credits.Domain.Model.Stores;
using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Credits;
using Sc.Credits.Domain.Managment.Services.Stores;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Helpers.Commons.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Sc.Credits.Domain.Model.Stores.Queries.Reading;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.UseCase.Credits;
using Sc.Credits.Helpers.ObjectsUtils;
using Sc.Credits.Domain.Model.Validation.Validators;
using Sc.Credits.Domain.Model.Validation.Extensions;
using Sc.Credits.Domain.Model.Google.Recaptcha;

namespace Sc.Credits.Domain.Managment.Services.Simulator
{
    public class SimulatorService : ISimulatorService
    {

        private readonly IStoreService _storeService;
        private readonly ISimulatorCommonService _creditCommonsService;
        private readonly IAppParametersService _appParametersService;
        private readonly ICreditUsesCase _creditUsesCase;
        private readonly IReCaptchaGateway _reCaptchaGateway;
        private readonly CredinetAppSettings _credinetAppSettings;


        /// <summary>
        /// Initializes a new instance of the <see cref="SimulatorService"/> class.
        /// </summary>
        /// <param name="simulatorCommons">The simulator commons.</param>
        /// <param name="loggerService">The logger service.</param>
        public SimulatorService(SimulatorCommons simulatorCommons,
            IReCaptchaGateway reCaptchaGateway,
            ILoggerService<SimulatorService> loggerService)
        {
            _creditUsesCase = simulatorCommons.CreditUsesCase;
            _creditCommonsService = simulatorCommons.Service;
            _appParametersService = _creditCommonsService.Commons.AppParameters;
            _storeService = _creditCommonsService.StoreService;
            _credinetAppSettings = simulatorCommons.Service.Commons.CredinetAppSettings;
            _reCaptchaGateway = reCaptchaGateway;
        }


        /// <summary>
        /// <see cref="ISimulatorService.GetCreditDetailsAsync(RequiredInitialValuesForCreditSimulation)"/>
        /// </summary>
        /// <param name="requiredValues">The required values.</param>
        /// <returns>CreditDetailResponse with the credit operations applied</returns>
        public async Task<CreditDetailResponse> GetCreditDetailsAsync(RequiredInitialValuesForCreditSimulation requiredValues)
        {
            ValidateInputValues(requiredValues.months);
            return await CalculateDetails(requiredValues);
        }

        /// <summary>
        /// simulations without sotreid and customer
        /// </summary>
        /// <param name="requiredValues">The required values.</param>
        /// <returns>just TotalFeeValue and fee of simulation</returns>
        public async Task<SimulationDetailsResponse> IndependentSimulation(InitialValuesForIndependentSimulation requiredValues)
        {
            await ValidateReCaptcha(requiredValues.ValidationToken);
            await requiredValues.ValidateAndThrowsAsync<InitialValuesForIndependentSimulation, InitialValuesForIndependentSimulationValidator>(settings: _credinetAppSettings.Validation);
            requiredValues.storeId = string.IsNullOrEmpty(requiredValues.storeId) ? _credinetAppSettings.GenericStore : requiredValues.storeId;
            AppParameters parameters = await _appParametersService.GetAppParametersAsync();
            if (parameters.SaveSimulationRecord)
                await SaveRecord(requiredValues);
            return await GetSimulationDetails(requiredValues);
        }


        /// <summary>
        /// Gets the simulation details.
        /// </summary>
        /// <param name="requiredValues">The required values.</param>
        /// <returns>just TotalFeeValue and fee of simulation</returns>
        private async Task<SimulationDetailsResponse> GetSimulationDetails(RequiredInitialValues requiredValues)
        {
            CreditDetailResponse creditDeatilReponse = await CalculateDetails(requiredValues);
            return new SimulationDetailsResponse(creditDeatilReponse.TotalFeeValue, creditDeatilReponse.Fees);
        }

        /// <summary>
        /// <see cref="ISimulatorService.GetTotalFeeValue(RequiredInitialValuesForCreditSimulation)"/>
        /// </summary>
        /// <param name="requiredValues">Basic values to make de simulation</param>
        /// <returns>SimulationDetailsResponse with the needed details</returns>
        public async Task<SimulationDetailsResponse> GetTotalFeeValue(RequiredInitialValuesForBasicSimulation requiredValues)
        {
            requiredValues.frequency = (int)Frequencies.Monthly;
            return await GetSimulationDetails(requiredValues);
        }


        /// <summary>
        /// Creates the detail object with all the operations applied
        /// </summary>
        /// <param name="requiredValues"></param>
        /// <returns>CreditDetailResponse with the credit operations applied</returns>

        private async Task<CreditDetailResponse> CalculateDetails(RequiredInitialValues requiredValues)
        {
            ValidateFieldsEmpty(requiredValues.storeId, requiredValues.creditValue);

            Store store = await _storeService.GetAsync(requiredValues.storeId, StoreReadingFields.CreditDetails, loadProductCategory: true);

            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            ValidateInputValues(requiredValues.creditValue, requiredValues.frequency);

            SimulatedCreditRequest simulationRequest = new SimulatedCreditRequest(store, requiredValues.creditValue, requiredValues.frequency, parameters);
            simulationRequest.SetFeesByMonths(requiredValues.months);
            CreditDetailResponse creditDetailResponse = _creditUsesCase.GetCreditDetails(simulationRequest);


            return creditDetailResponse;
        }

        /// <summary>
        /// <see cref="ICreditService.GetTimeLimitInMonthsAsync(LimitMonhsInitialValuesRequest)"/>
        /// </summary>
        /// <param name="initialValues">The initial values.</param>
        /// <returns>Allowed range o months to get credit</returns>
        public async Task<int> GetTimeLimitInMonthsAsync(LimitMonhsInitialValuesRequest initialValues)
        {
            await initialValues.ValidateAndThrowsAsync<LimitMonhsInitialValuesRequest, LimitMonhsInitialValuesRequestValidator>(settings: _credinetAppSettings.Validation);

            initialValues.storeId = string.IsNullOrEmpty(initialValues.storeId) ? _credinetAppSettings.GenericStore : initialValues.storeId;

            Store store = await _storeService.GetAsync(initialValues.storeId, StoreReadingFields.LimitMonths, loadProductCategory: true);

            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            int decimalNumbersRound = parameters.DecimalNumbersRound;

            return _creditUsesCase.GetTimeLimitInMonths(initialValues.creditValue, store, decimalNumbersRound);
        }

        /// <summary>
        /// <see cref="ICreditService.GetStoreMinAndMaxCreditLimitByStoreId(LimitMonhsInitialValuesRequest)"/>
        /// </summary>
        /// <param name="storeId">The store identifier.</param>
        /// <returns>range of value from storecategory of store</returns>
        public async Task<StoreCategoryRange> GetStoreMinAndMaxCreditLimitByStoreId(string storeId)
        {
            storeId = string.IsNullOrEmpty(storeId) ? _credinetAppSettings.GenericStore : storeId;

            Store store = await _storeService.GetAsync(storeId, StoreReadingFields.LimitMonths, loadProductCategory: true);

            AppParameters parameters = await _appParametersService.GetAppParametersAsync();

            int decimalNumbersRound = parameters.DecimalNumbersRound;

            return _creditUsesCase.GetStoreMinAndMaxCreditLimitByStoreId(store, decimalNumbersRound);
        }

        /// <summary>
        /// Saves the record.
        /// </summary>
        /// <param name="requiredValues">The required values.</param>
        private async Task SaveRecord(InitialValuesForIndependentSimulation requiredValues)
        {
            await _creditCommonsService.SaveRecordRequestAsync(requiredValues);
        }

        /// <summary>
        /// Validates the fields empty.
        /// </summary>
        /// <param name="storeId">The store identifier.</param>
        /// <param name="creditValue">The credit value.</param>
        /// <exception cref="BusinessException">RequestValuesInvalid</exception>
        private void ValidateFieldsEmpty(string storeId, decimal creditValue)
        {
            if (string.IsNullOrEmpty(storeId?.Trim()) || creditValue <= 0)
            {
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);
            }
        }



        /// <summary>
        /// Validates the input values.
        /// </summary>
        /// <param name="creditValue">The credit value.</param>
        /// <param name="frequency">The frequency.</param>
        /// <exception cref="BusinessException">RequestValuesInvalid</exception>
        private void ValidateInputValues(decimal creditValue, int frequency)
        {
            if (creditValue < 0 || !ValidateFrequency(frequency))
            {
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);
            }
        }

        /// <summary>
        /// Validates the input values.
        /// </summary>
        /// <param name="months">The months.</param>
        /// <exception cref="BusinessException">MonthsNumberNotValid</exception>
        private void ValidateInputValues(int months)
        {
            if (months < 0)
            {
                throw new BusinessException(nameof(BusinessResponse.MonthsNumberNotValid), (int)BusinessResponse.MonthsNumberNotValid);
            }
        }

        /// <summary>
        /// Validate the frequency.
        /// </summary>
        /// <param name="frequency"></param>
        /// <returns>either frequency is valid or not</returns>
        private bool ValidateFrequency(int frequency) =>
            Enum.IsDefined(typeof(Frequencies), frequency);

        /// <summary>
        /// Validates the re captcha.
        /// </summary>
        private async Task ValidateReCaptcha(string ValidationToken) 
        {
            if(string.IsNullOrEmpty(ValidationToken))
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);

            if (!await _reCaptchaGateway.ValidateReCaptcha(ValidationToken))
                throw new BusinessException(nameof(BusinessResponse.Unauthorized), (int)BusinessResponse.Unauthorized);
        } 



    }
}
