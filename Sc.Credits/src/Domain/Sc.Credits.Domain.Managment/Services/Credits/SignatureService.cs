using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Microsoft.EntityFrameworkCore.Internal;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Common.Gateway;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Helpers.ObjectsUtils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Managment.Services.Credits
{
    /// <summary>
    /// Signature service is an implementation of <see cref="ISignatureService"/>
    /// </summary>
    public class SignatureService : ISignatureService
    {
        private readonly ICreditCommonsService _creditCommonsService;
        private readonly IAutenticRepository _autenticRepository;
        private readonly CredinetAppSettings _credinetAppSettings;

        /// <summary>
        /// Creates a new instance of <see cref="SignatureService"/>
        /// </summary>
        /// <param name="creditCommonsService"></param>
        /// <param name="autenticRepository"></param>
        public SignatureService(ICreditCommonsService creditCommonsService,
            IAutenticRepository autenticRepository)
        {
            _creditCommonsService = creditCommonsService;
            _autenticRepository = autenticRepository;
            _credinetAppSettings = creditCommonsService.Commons.CredinetAppSettings;
        }

        /// <summary>
        /// <see cref="ISignatureService.SignAsync(CreditMaster, PromissoryNoteResponse, byte[])"/>
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="promissoryNote"></param>
        /// <param name="promisoryNoteFile"></param>
        /// <returns></returns>
        public async Task<(string Authority, string Id)> SignAsync(CreditMaster creditMaster, string fileName, byte[] promisoryNoteFile) =>
            (@"Autentic", await WithAutenticAsync(creditMaster, fileName, promisoryNoteFile));

        /// <summary>
        /// With autentic
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="fileName"></param>
        /// <param name="promisoryNoteFile"></param>
        /// <returns></returns>
        private async Task<string> WithAutenticAsync(CreditMaster creditMaster, string fileName, byte[] promisoryNoteFile)
        {
            RequestAutentic requestAutentic = new RequestAutentic
            {
                Metadata = new Metadata
                {
                    Names = creditMaster.Customer.Name.GetNames,
                    LastNames = creditMaster.Customer.Name.GetLastNames,
                    DocId = creditMaster.Customer.IdDocument,
                    SecureKey = creditMaster.Id.ToString()
                },
                Files = new List<AttachmentPdf>
                {
                    new AttachmentPdf
                    {
                        FileContent = promisoryNoteFile,
                        FileName = fileName
                    }
                }
            };

            OAuthRequestAutentic oAuthRequestAutentic = new OAuthRequestAutentic()
            {
                Audience = _credinetAppSettings.AudienceAutentic,
                GrantType = _credinetAppSettings.GrantTypeAutentic,
                ClientId = _credinetAppSettings.ClientIdAutentic,
                ClientSecret = _credinetAppSettings.ClientSecretAutentc,
            };

            OAuthResponseAutentic oAuthResponseAutentic =
                await _autenticRepository.OAuthAutenticAsync(_credinetAppSettings.EndPointOauthAutentic, oAuthRequestAutentic, creditMaster.Id.ToString());

            if (oAuthResponseAutentic != null)
            {
                (byte[] PromisoryNoteSignatured, string Id) = await _autenticRepository.GetSignaturePdfAsync(_credinetAppSettings.EndPointAutentic, requestAutentic,
                    oAuthResponseAutentic.TokenType, oAuthResponseAutentic.AccessToken, creditMaster.Id.ToString());

                if (PromisoryNoteSignatured == null || !PromisoryNoteSignatured.Any())
                {
                    ThrowsFailedSignature();
                }

                await _creditCommonsService.Commons
                    .Storage.UploadFileAsync(PromisoryNoteSignatured, _credinetAppSettings.PromissoryNotePath, fileName,
                        _credinetAppSettings.PdfBlobContainerName);

                return Id;
            }
            else
            {
                ThrowsFailedSignature();
            }

            return null;
        }

        /// <summary>
        /// Throws failed signature
        /// </summary>
        private void ThrowsFailedSignature()
        {
            throw new BusinessException(nameof(BusinessResponse.FailedSignaturePromisoryNote), (int)BusinessResponse.FailedSignaturePromisoryNote);
        }
    }
}