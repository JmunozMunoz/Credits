using Newtonsoft.Json;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Common.Gateway;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.ObjectsUtils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;

namespace Sc.Credits.DrivenAdapters.Autentic
{
    /// <summary>
    /// <see cref="IAutenticRepository"/>
    /// </summary>
    public class AutenticAdapter
        : IAutenticRepository
    {
        private readonly HttpClient _clientHttp;
        private readonly ILoggerService<AutenticAdapter> _loggerService;

        /// <summary>
        /// New autentic adapter
        /// </summary>
        /// <param name="clientHttp"></param>
        public AutenticAdapter(HttpClient clientHttp,
            ILoggerService<AutenticAdapter> loggerService)
        {
            _clientHttp = clientHttp;
            _loggerService = loggerService;
        }

        /// <summary>
        /// <see cref="IAutenticRepository.OAuthAutentic(string, OAuthRequestAutentic)"/>
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<OAuthResponseAutentic> OAuthAutenticAsync(string endPoint, OAuthRequestAutentic parameters, string traceId)
        {
            HttpResponseMessage autenticRequest = await _clientHttp.PostAsJsonAsync(endPoint, parameters);

            string autenticContentResponse = await autenticRequest.Content.ReadAsStringAsync();

            OAuthResponseAutentic oAuthResponseAutentic = autenticRequest.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<OAuthResponseAutentic>(autenticContentResponse)
                : null;

            object logData = new
            {
                parameters,
                autenticContentResponse,
                oAuthResponseAutentic,
                statusCode = autenticRequest.StatusCode,
                headers = autenticRequest.Headers
            };

            if (oAuthResponseAutentic == null)
            {
                await _loggerService.NotifyAsync(traceId,
                    nameof(OAuthResponseAutentic),
                    logData,
                    MethodBase.GetCurrentMethod());
            }
            else
            {
                await _loggerService.NotifyAsync(traceId,
                    nameof(OAuthResponseAutentic),
                    logData,
                    MethodBase.GetCurrentMethod());
            }

            return oAuthResponseAutentic;
        }

        /// <summary>
        /// <see cref="IAutenticRepository.GetSignaturePdf(string, RequestAutentic, string, string)"/>
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="parameters"></param>
        /// <param name="tokenType"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<(byte[] File, string Id)> GetSignaturePdfAsync(string endPoint, RequestAutentic parameters, string tokenType, string token, string traceId)
        {
            _clientHttp.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, token);

            HttpResponseMessage autenticRequest = await _clientHttp.PostAsJsonAsync(endPoint, parameters);

            string autenticContentResponse = await autenticRequest.Content.ReadAsStringAsync();

            ResponseAutentic responseAutentic = autenticRequest.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<ResponseAutentic>(autenticContentResponse)
                : null;

            object logData = new
            {
                parameters,
                autenticContentResponse,
                responseAutentic,
                tokenType,
                token,
                statusCode = autenticRequest.StatusCode,
                headers = autenticRequest.Headers
            };

            if (responseAutentic == null)
            {
                await _loggerService.NotifyAsync(traceId,
                   nameof(GetSignaturePdfAsync),
                   logData,
                   MethodBase.GetCurrentMethod());

                return (null, null);
            }

            await _loggerService.NotifyAsync(token, nameof(GetSignaturePdfAsync), logData, MethodBase.GetCurrentMethod());

            Stream zip = await GetZipFromUrl(responseAutentic.UrlDocuments);

            IEnumerable<byte[]> pdfDocuments = ZipManagerHelper.ProcessZipStream(zip);

            byte[] promisoryNoteBytes = pdfDocuments.FirstOrDefault();

            return (promisoryNoteBytes, responseAutentic.TransactionId);
        }

        /// <summary>
        /// Get Zip From Url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task<Stream> GetZipFromUrl(string url)
        {
            HttpClient httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("ContentType", "application/zip");

            HttpResponseMessage zipRequest = await httpClient.GetAsync(url);

            object logData = new { url };

            if (!zipRequest.IsSuccessStatusCode)
            {
                _loggerService.LogWarning(url,
                    logData,
                    MethodBase.GetCurrentMethod());

                return null;
            }

            _loggerService.LogInfo(url,
               logData,
               MethodBase.GetCurrentMethod());

            Stream zipResponse = await zipRequest.Content.ReadAsStreamAsync();

            return zipResponse;
        }
    }
}