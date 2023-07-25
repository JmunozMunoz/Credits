using org.reactivecommons.api;
using org.reactivecommons.api.domain;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Customers.Gateway;
using Sc.Credits.Helpers.Commons.Settings;
using Sc.Credits.Helpers.ObjectsUtils;
using System.Threading.Tasks;

namespace Sc.Credits.DrivenAdapters.ServiceBus.Customers
{
    public class RiskLevelAdapter : IRiskLevelRepository
    {
        private readonly IDirectAsyncGateway<dynamic> _directAsyncGateway;
        private readonly CredinetAppSettings _credinetAppSettings;

        public RiskLevelAdapter(IDirectAsyncGateway<dynamic> directAsyncGateway,
            ISettings<CredinetAppSettings> appSettings)
        {
            _directAsyncGateway = directAsyncGateway;
            _credinetAppSettings = appSettings.Get();
        }

        /// <summary>
        /// <see cref="IRiskLevelRepository.CalculateRiskLevelAsync(CustomerRiskLevelRequest)"/>
        /// </summary>
        /// <param name="riskLevelRequest"></param>
        /// <returns></returns>
        public async Task<CustomerRiskLevel> CalculateRiskLevelAsync(CustomerRiskLevelRequest riskLevelRequest)
        {
            AsyncQuery<dynamic> queryValidation = new AsyncQuery<dynamic>
            {
                QueryData = riskLevelRequest,
                Resource = "ValidateCreditToken"
            };

            return await _directAsyncGateway.RequestReply<CustomerRiskLevel>(queryValidation, _credinetAppSettings.CustomerRiskLevelCalculationQueue);
        }
    }
}