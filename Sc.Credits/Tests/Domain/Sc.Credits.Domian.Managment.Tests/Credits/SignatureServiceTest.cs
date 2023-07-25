using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Moq;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Credits;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Common.Gateway;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Helper.Test.Model;
using Sc.Credits.Helpers.ObjectsUtils;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.Domain.Managment.Tests.Credits
{
    public class SignatureServiceTest
    {
        private readonly Mock<ICreditCommonsService> _creditCommonsServiceMock = new Mock<ICreditCommonsService>();
        private readonly Mock<IAutenticRepository> _autenticRepositoryMock = new Mock<IAutenticRepository>();
        private readonly Mock<ICommons> _commonsMock = new Mock<ICommons>();
        private readonly Mock<IStorageService> _storageServiceMock = new Mock<IStorageService>();
        private readonly Mock<INotificationService> _notificationServiceMock = new Mock<INotificationService>();
        private readonly Mock<ITemplatesService> _templatesServiceMock = new Mock<ITemplatesService>();
        private readonly Mock<IAppParametersService> _appParametersServiceMock = new Mock<IAppParametersService>();

        private ISignatureService SignatureService =>
            new SignatureService(_creditCommonsServiceMock.Object,
                _autenticRepositoryMock.Object);

        public SignatureServiceTest()
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

            _creditCommonsServiceMock.SetupGet(mock => mock.Commons)
                .Returns(_commonsMock.Object);

            _commonsMock.SetupGet(mock => mock.Storage)
                .Returns(_storageServiceMock.Object);

            _commonsMock.SetupGet(mock => mock.Notification)
                .Returns(_notificationServiceMock.Object);

            _commonsMock.SetupGet(mock => mock.Templates)
                .Returns(_templatesServiceMock.Object);

            _commonsMock.SetupGet(mock => mock.CredinetAppSettings)
                .Returns(_appParametersServiceMock.Object.GetSettings());
        }

        [Fact]
        public async Task ShouldSignWithAutentic()
        {
            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMasterWithStoreAllowPromissoryNoteSignature();
            OAuthResponseAutentic oAuthResponseAutentic = AutenticHelper.GetOAuthResponseAutentic();

            byte[] autenticResponse = Encoding.UTF8.GetBytes(System.IO.File.ReadAllText("./Resources/pdfBase64Resource.txt"));

            _autenticRepositoryMock.Setup(mock => mock.OAuthAutenticAsync(It.IsAny<string>(), It.IsAny<OAuthRequestAutentic>(), It.IsAny<string>()))
                .ReturnsAsync(oAuthResponseAutentic);

            _autenticRepositoryMock.Setup(mock => mock.GetSignaturePdfAsync(It.IsAny<string>(), It.IsAny<RequestAutentic>(),It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((autenticResponse, "TransactionId"));

            await SignatureService.SignAsync(creditMaster, "prueba.pdf", new byte[] { 1, 2, 3 });

            _creditCommonsServiceMock.Verify(mock => mock.Commons.Storage.UploadFileAsync(It.IsAny<byte[]>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task SouldSignWithAutenticThrowsFailedSignaturePromisoryNoteOAuth()
        {
            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMasterWithStoreAllowPromissoryNoteSignature();

            _autenticRepositoryMock.Setup(mock => mock.OAuthAutenticAsync(It.IsAny<string>(),
                    It.IsAny<OAuthRequestAutentic>(),It.IsAny<string>()))
                .ReturnsAsync((OAuthResponseAutentic)null);

            BusinessException exception = await Assert.ThrowsAsync<BusinessException>(() =>
                SignatureService.SignAsync(creditMaster, "prueba.pdf", new byte[] { 1, 2, 3 }));

            Assert.Equal(nameof(BusinessResponse.FailedSignaturePromisoryNote), exception.Message);
        }

        [Fact]
        public async Task SouldSignWithAutenticThrowsFailedSignaturePromisoryNoteGetPdf()
        {
            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMasterWithStoreAllowPromissoryNoteSignature();
            OAuthResponseAutentic oAuthResponseAutentic = AutenticHelper.GetOAuthResponseAutentic();

            _autenticRepositoryMock.Setup(mock => mock.OAuthAutenticAsync(It.IsAny<string>(), It.IsAny<OAuthRequestAutentic>(),It.IsAny<string>()))
                .ReturnsAsync(oAuthResponseAutentic);

            BusinessException exception = await Assert.ThrowsAsync<BusinessException>(() =>
                SignatureService.SignAsync(creditMaster, "prueba.pdf", new byte[] { 1, 2, 3 }));

            Assert.Equal(nameof(BusinessResponse.FailedSignaturePromisoryNote), exception.Message);
        }
    }
}