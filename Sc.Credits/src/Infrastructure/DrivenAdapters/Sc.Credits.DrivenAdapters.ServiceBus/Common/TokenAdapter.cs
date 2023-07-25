using org.reactivecommons.api;
using org.reactivecommons.api.domain;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Common.Gateway;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Helpers.Commons.Settings;
using Sc.Credits.Helpers.ObjectsUtils;
using System.Threading.Tasks;

namespace Sc.Credits.DrivenAdapters.ServiceBus.Common
{
    /// <summary>
    /// Token adapter is an implementation of <see cref="ITokenRepository"/>
    /// </summary>
    public class TokenAdapter
        : ITokenRepository
    {
        private readonly IDirectAsyncGateway<dynamic> _directAsyncGateway;
        private readonly CredinetAppSettings _credinetAppSettings;

        /// <summary>
        /// New token adapter
        /// </summary>
        /// <param name="directAsyncGateway"></param>
        /// <param name="appSettings"></param>
        public TokenAdapter(IDirectAsyncGateway<dynamic> directAsyncGateway,
            ISettings<CredinetAppSettings> appSettings)
        {
            _directAsyncGateway = directAsyncGateway;
            _credinetAppSettings = appSettings.Get();
        }

        /// <summary>
        /// <see cref="ITokenRepository.IsValidCreditTokenAsync(CreditTokenRequest)"/>
        /// </summary>
        /// <param name="creditTokenRequest"></param>
        /// <returns></returns>
        public async Task<bool> IsValidCreditTokenAsync(CreditTokenRequest creditTokenRequest)
        {
            AsyncQuery<dynamic> queryValidation = new AsyncQuery<dynamic>
            {
                QueryData = creditTokenRequest,
                Resource = "ValidateCreditToken"
            };

            return await _directAsyncGateway.RequestReply<bool>(queryValidation, _credinetAppSettings.CreditTokenValidateQueue);
        }

        /// <summary>
        /// <see cref="ITokenRepository.GenerateCreditTokenAsync(CreditTokenRequest)"/>
        /// </summary>
        /// <param name="creditTokenRequest"></param>
        /// <returns></returns>
        public async Task<TokenResponse> GenerateCreditTokenAsync(CreditTokenRequest creditTokenRequest)
        {
            AsyncQuery<dynamic> queryValidation = new AsyncQuery<dynamic>
            {
                QueryData = creditTokenRequest,
                Resource = "GenerateCreditToken"
            };

            TokenResponse tokenResponse = await _directAsyncGateway.RequestReply<TokenResponse>(queryValidation,
                _credinetAppSettings.CreditTokenGenerateQueue);

            return tokenResponse;
        }

        /// <summary>
        /// <see cref="ITokenRepository.CreditTokenCallRequestAsync(CreditTokenCallRequest)"/>
        /// </summary>
        /// <param name="creditTokenCallRequest"></param>
        /// <returns></returns>
        public async Task CreditTokenCallRequestAsync(CreditTokenCallRequest creditTokenCallRequest)
        {
            Command<dynamic> command = new Command<dynamic>("SendCreditTokenCallRequest", creditTokenCallRequest.IdDocument, creditTokenCallRequest);

            await _directAsyncGateway.SendCommand(_credinetAppSettings.CreditTokenCallRequestQueue, command);
        }
    }
}