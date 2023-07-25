using credinet.exception.middleware.models;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Sc.Credits.Domain.Model.Call.Gateway;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Helpers.Commons.Logging.Gateway;
using Sc.Credits.Helpers.Commons.Settings;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace SC.Credits.DrivenAdapter.Http.UdpCall
{
    public class UdpCallHttpAdapter : IUdpCallHttpRepository
    {
        private readonly HttpClient _httpClient;
        private readonly CredinetAppSettings _appSettings;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IEventsRepository _eventsRepository;

        public UdpCallHttpAdapter(HttpClient httpClient,
             ISettings<CredinetAppSettings> options,
             IHostingEnvironment hostingEnvironment,
             IEventsRepository eventsRepository)
        {
            _httpClient = httpClient;
            _appSettings = options.Get();
            _hostingEnvironment = hostingEnvironment;
            _eventsRepository = eventsRepository;
            ConfigureHttpClientDefaults();
        }

        public async Task CallAsync(CreditTokenCallRequest creditTokenCallRequest)
        {
            var tokenCallRequest = new
            {
                id = creditTokenCallRequest.IdDocument,
                mobile = creditTokenCallRequest.Mobile,
                message = creditTokenCallRequest.Message,
                urlRet = _appSettings.UdpResponseCallUrl
            };

            string reportRequestJson = JsonConvert.SerializeObject(tokenCallRequest);

            StringContent content = new StringContent(reportRequestJson, Encoding.UTF8, MediaTypeNames.Application.Json);

            HttpResponseMessage responseMessage = await _httpClient.PostAsync(_appSettings.UdpCallUrlRute, content);

            if (!responseMessage.IsSuccessStatusCode)
            {
                string exception = $"Token call fault. SCOrigen: {_hostingEnvironment.EnvironmentName}, " +
                    $"id: {tokenCallRequest.id}, mobile: {tokenCallRequest.mobile}, urlRet: {tokenCallRequest.urlRet}";

                await _eventsRepository.SendAsync("CreditTokenCall.Exeption", creditTokenCallRequest.IdDocument,
                    new
                    {
                        Exception = exception,
                        tokenCallRequest
                    });

                throw new BusinessException(exception, 500);
            }

            await _eventsRepository.SendAsync("CreditTokenCall", creditTokenCallRequest.IdDocument, tokenCallRequest);
        }

        private void ConfigureHttpClientDefaults()
        {
            _httpClient.BaseAddress = new Uri(_appSettings.UdpCallUrlEndPoint);

            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _appSettings.ReportTemplatesApiKey);
            _httpClient.DefaultRequestHeaders.Add("country", EnvironmentHelper.GetCountryOrDefault(_appSettings.DefaultCountry));
            _httpClient.DefaultRequestHeaders.Add("SCOrigen", EnvironmentHelper.CurrentEnvironment());
            _httpClient.DefaultRequestHeaders.Add("SCLocation", "1,1");
        }
    }
}