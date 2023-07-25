using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Helper.Test.Model;
using Sc.Credits.Helpers.Commons.Extensions;
using Sc.Credits.Helpers.ObjectsUtils;
using Xunit;

namespace Sc.Credits.Domain.UseCase.Tests.Common
{
    public class CommonModelsTest
    {
        [Fact]
        public void TestBase64Attachment()
        {
            Assert.True(ModelHelperTest.TestModel<Base64Attachment>());
        }

        [Fact]
        public void TestBlobStorageAttachment()
        {
            Assert.True(ModelHelperTest.TestModel<BlobStorageAttachment>());
        }

        [Fact]
        public void TestToken()
        {
            Assert.True(ModelHelperTest.TestModel<Token>());
        }

        [Fact]
        public void TestTokenResponse()
        {
            Assert.True(ModelHelperTest.TestModel<TokenResponse>());
        }

        [Fact]
        public void TestTemplateValue()
        {
            Assert.True(ModelHelperTest.TestModel<TemplateValue>());
        }

        [Fact]
        public void TestCredinetAppSettings()
        {
            Assert.True(ModelHelperTest.TestModel<CredinetAppSettings>());
        }

        [Fact]
        public void TestTypeInstance()
        {
            CredinetAppSettings appSettings = TypeInstance.New<CredinetAppSettings>();
            Assert.NotNull(appSettings);
        }

        [Fact]
        public void TestTypeInstanceTypeOf()
        {
            CredinetAppSettings appSettings = (CredinetAppSettings)TypeInstance.New(typeof(CredinetAppSettings));
            Assert.NotNull(appSettings);
        }
    }
}