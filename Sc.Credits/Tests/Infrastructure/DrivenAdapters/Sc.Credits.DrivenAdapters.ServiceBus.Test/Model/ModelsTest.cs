using Sc.Credits.Domain.Model.Common;
using Sc.Credits.DrivenAdapters.ServiceBus.Model;
using Sc.Credits.Helper.Test.Model;
using Xunit;

namespace Sc.Credits.DrivenAdapters.ServiceBus.Test.Model
{
    public class ModelsTest
    {
        [Fact]
        public void TestTokenResponse()
        {
            Assert.True(ModelHelperTest.TestModel<CreditEvent>());
        }

        [Fact]
        public void TestTemplateValue()
        {
            TokenGenerateResponse tokenGenerateResponse = ModelHelperTest.InstanceModel<TokenGenerateResponse>();
            tokenGenerateResponse.Data = ModelHelperTest.InstanceModel<TokenResponse>();
            Assert.NotNull(tokenGenerateResponse);
            Assert.NotNull(tokenGenerateResponse.Data);
        }

        [Fact]
        public void TestCredinetAppSettings()
        {
            Assert.True(ModelHelperTest.TestModel<TokenValidationResponse>());
        }
    }
}