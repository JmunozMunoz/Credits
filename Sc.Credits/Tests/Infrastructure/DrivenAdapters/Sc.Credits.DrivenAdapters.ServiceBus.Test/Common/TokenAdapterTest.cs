using Moq;
using org.reactivecommons.api;
using org.reactivecommons.api.domain;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Common.Gateway;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.DrivenAdapters.ServiceBus.Common;
using Sc.Credits.Helpers.Commons.Settings;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.DrivenAdapters.ServiceBus.Test.Common
{
    public class TokenAdapterTest
    {
        private readonly Mock<IDirectAsyncGateway<dynamic>> _directAsyncGatewayMock = new Mock<IDirectAsyncGateway<dynamic>>();
        private readonly Mock<ISettings<CredinetAppSettings>> _appSettingsMock = new Mock<ISettings<CredinetAppSettings>>();

        private ITokenRepository TokenValidatorRepository =>
            new TokenAdapter(_directAsyncGatewayMock.Object,
                _appSettingsMock.Object);

        public TokenAdapterTest()
        {
            _appSettingsMock.Setup(mock => mock.Get())
                .Returns(new CredinetAppSettings());
        }

        [Fact]
        public async Task SouldValidateCreditToken()
        {
            await TokenValidatorRepository.IsValidCreditTokenAsync(new CreditTokenRequest
            {
                AdditionalData = string.Empty,
                IdDocument = "1037610201",
                CustomerId = new Guid("527af1e7-bcbf-4559-bc38-054f785976f5"),
                Token = "023402"
            });

            _directAsyncGatewayMock.Verify(mock => mock.RequestReply<bool>(It.IsAny<AsyncQuery<dynamic>>(), It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public async Task SouldGenerateToken()
        {
            Token result = new Token { Value = "023402" };

            _directAsyncGatewayMock.Setup(mock => mock.RequestReply<Token>(It.IsAny<AsyncQuery<dynamic>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(result));

            await TokenValidatorRepository.GenerateCreditTokenAsync(new CreditTokenRequest
            {
                AdditionalData = string.Empty,
                IdDocument = "1037610201",
                CustomerId = new Guid("527af1e7-bcbf-4559-bc38-054f785976f5")
            });

            _directAsyncGatewayMock.Verify(mock => mock.RequestReply<TokenResponse>(It.IsAny<AsyncQuery<dynamic>>(), It.IsAny<string>()), Times.Once());
        }
    }
}