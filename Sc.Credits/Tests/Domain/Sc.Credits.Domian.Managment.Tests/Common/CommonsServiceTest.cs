using Moq;
using Sc.Credits.Domain.Managment.Services.Common;
using Xunit;

namespace Sc.Credits.Domain.Managment.Tests.Common
{
    public class CommonsServiceTest
    {
        private readonly Mock<IAppParametersService> _appParametersServiceMock = new Mock<IAppParametersService>();
        private readonly Mock<INotificationService> _notificationServiceMock = new Mock<INotificationService>();
        private readonly Mock<ITemplatesService> _templatesServiceMock = new Mock<ITemplatesService>();
        private readonly Mock<IStorageService> _storageServiceMock = new Mock<IStorageService>();

        private ICommons _commonsService;

        [Fact]
        public void ShouldNew()
        {
            _commonsService = new Commons(_appParametersServiceMock.Object,
                _notificationServiceMock.Object,
                _templatesServiceMock.Object,
                _storageServiceMock.Object);

            Assert.NotNull(_commonsService);
            Assert.Equal(_appParametersServiceMock.Object, _commonsService.AppParameters);
            Assert.Equal(_notificationServiceMock.Object, _commonsService.Notification);
            Assert.Equal(_templatesServiceMock.Object, _commonsService.Templates);
            Assert.Equal(_storageServiceMock.Object, _commonsService.Storage);
        }
    }
}