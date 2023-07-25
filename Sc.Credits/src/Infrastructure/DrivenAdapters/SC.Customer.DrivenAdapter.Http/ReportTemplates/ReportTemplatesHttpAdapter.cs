using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Newtonsoft.Json;
using Sc.Credits.Domain.Model.ReportTemplates;
using Sc.Credits.Domain.Model.ReportTemplates.Extensions;
using Sc.Credits.Domain.Model.ReportTemplates.Gateway;
using Sc.Credits.Helpers.Commons.Settings;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SC.Customer.DrivenAdapter.Http.ReportTemplates
{
    public class ReportTemplatesHttpAdapter : IReportTemplatesGateway
    {
        private readonly HttpClient _httpClient;
        private readonly CredinetAppSettings _appSettings;

        /// <summary>
        /// Creates new instance of <see cref="ReportTemplatesHttpAdapter"/>
        /// </summary>
        /// <param name="httpClient"></param>
        public ReportTemplatesHttpAdapter(HttpClient httpClient,
            ISettings<CredinetAppSettings> options)
        {
            _httpClient = httpClient;
            _appSettings = options.Get();

            ConfigureHttpClientDefaults();
        }

        /// <summary>
        /// <see cref="IReportTemplatesGateway.GenerateAsync{T}(T, string, string)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="reportName"></param>
        /// <param name="renderName"></param>
        /// <returns></returns>
        public async Task<string> GenerateAsync<T>(T request, string reportName, string renderName = null)
            where T : class
        {
            ReportRequest reportRequest = new ReportRequest
            {
                ApplicationName = _appSettings.DomainName,
                Data = request.ToReportRequestData(),
                Name = reportName,
                RenderName = renderName
            };

            string reportRequestJson = JsonConvert.SerializeObject(reportRequest);

            StringContent content = new StringContent(reportRequestJson, Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = await _httpClient.PostAsync(_appSettings.ReportTemplatesGenerationPath, content);

            string responseJson = await responseMessage.Content?.ReadAsStringAsync();

            if (!responseMessage.IsSuccessStatusCode)
                throw new BusinessException(nameof(BusinessResponse.NotControlledException), (int)BusinessResponse.NotControlledException, responseJson);

            ApiResponse<RerportResponse> apiResponse = JsonConvert.DeserializeObject<ApiResponse<RerportResponse>>(responseJson);

            return apiResponse.Data.Uri;
        }

        /// <summary>
        /// Configures the http client defaults.
        /// </summary>
        private void ConfigureHttpClientDefaults()
        {
            _httpClient.BaseAddress = new Uri(_appSettings.ReportTemplatesEndpoint);

            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _appSettings.ReportTemplatesApiKey);
            _httpClient.DefaultRequestHeaders.Add("country", EnvironmentHelper.GetCountryOrDefault(_appSettings.DefaultCountry));
            _httpClient.DefaultRequestHeaders.Add("SCOrigen", EnvironmentHelper.CurrentEnvironment());
            _httpClient.DefaultRequestHeaders.Add("SCLocation", "0,0");
        }
    }
}