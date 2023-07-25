using Microsoft.AspNetCore.WebUtilities;
using Sc.Credits.Domain.Model.Google.Recaptcha;
using Sc.Credits.Domain.Model.Google.Recaptcha.Requests;
using Sc.Credits.Domain.Model.Google.Recaptcha.Responses;
using Sc.Credits.Helpers.Commons.Settings;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SC.Credits.DrivenAdapter.Http.ReCaptchaValidators
{
    public class ReCaptchaHttpAdapter : IReCaptchaGateway
    {
        /// <summary>
        /// The HTTP client
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// The application settings
        /// </summary>
        private readonly CredinetAppSettings _appSettings;

        public ReCaptchaHttpAdapter(HttpClient httpClient,
             ISettings<CredinetAppSettings> options)
        {
            _httpClient = httpClient;
            _appSettings = options.Get();

            ConfigureHttpClientDefaults();
        }

        /// <summary>
        /// Configures the http client defaults.
        /// </summary>
        private void ConfigureHttpClientDefaults()
        {
            _httpClient.BaseAddress = new Uri(_appSettings.GoogleBaseUrl);
        }

        /// <summary>
        /// Validates the re captcha.
        /// </summary>
        /// <param name="validationToken">The validation token.</param>
        /// <returns></returns>
        public async Task<bool> ValidateReCaptcha(string validationToken)
        {
            try 
            {
                Dictionary<string, string> queryParams = BuildQueryStringValues(new GoogleRecaptchaRequestModel()
                {
                    Secret = _appSettings.SecretKeyV3ReCaptcha,
                    Response = validationToken
                });
                UriBuilder url = new UriBuilder($"{_httpClient.BaseAddress}{_appSettings.RecaptchaValidationTokenEndpoint}");

                string serviceurl = AddQueryString(url, queryParams);
                var reCaptchaResponseRaw = await _httpClient.GetAsync(serviceurl);

                if (reCaptchaResponseRaw.IsSuccessStatusCode)
                {
                    string result = await reCaptchaResponseRaw.Content.ReadAsStringAsync();
                    GoogleRecaptchaResponseModel reCaptchaResponse = JsonConvert.DeserializeObject<GoogleRecaptchaResponseModel>(result);
                    return reCaptchaResponse.success == true && reCaptchaResponse.score > _appSettings.MinimumRecaptchaScoreValue ? true : false;
                }

                return false;
            } 
            catch  
            {
                return true;
            }
        }

        /// <summary>
        /// Adds the query string.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="queryParams">The query parameters.</param>
        /// <returns></returns>
        private string AddQueryString(UriBuilder url, Dictionary<string, string> queryParams)
        {
            return QueryHelpers.AddQueryString(url.Uri.ToString(), queryParams);
        }

        /// <summary>
        /// Builds the query string values.
        /// </summary>
        /// <param name="QueryStringModel">The query string model.</param>
        /// <returns></returns>
        private Dictionary<string, string> BuildQueryStringValues(GoogleRecaptchaRequestModel QueryStringModel) 
        {
            Dictionary<string, string> queryParams = new Dictionary<string, string>();

            queryParams.Add(nameof(QueryStringModel.Secret).ToLower(), QueryStringModel.Secret);
            queryParams.Add(nameof(QueryStringModel.Response).ToLower(), QueryStringModel.Response);

            return queryParams;
        }


    }
}
