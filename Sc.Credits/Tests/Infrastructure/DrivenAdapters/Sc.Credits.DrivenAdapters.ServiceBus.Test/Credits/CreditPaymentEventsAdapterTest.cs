using Moq;
using org.reactivecommons.api;
using org.reactivecommons.api.domain;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.DrivenAdapters.ServiceBus.Credits;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.Commons.Messaging;
using Sc.Credits.Helpers.Commons.Settings;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.DrivenAdapters.ServiceBus.Test.Credits
{
    public class CreditPaymentEventsAdapterTest
    {
        private readonly Mock<IDirectAsyncGateway<dynamic>> _directAsyncGatewayCreditLimitResponseMock = new Mock<IDirectAsyncGateway<dynamic>>();
        private readonly Mock<ISettings<CredinetAppSettings>> _appSettingsMock = new Mock<ISettings<CredinetAppSettings>>();
        private readonly Mock<IMessagingLogger> _messagingLoggerMock = new Mock<IMessagingLogger>();
        private readonly Mock<ILoggerService<CreditPaymentEventsAdapter>> _loggerServiceMock = new Mock<ILoggerService<CreditPaymentEventsAdapter>>();

        public ICreditPaymentEventsRepository creditPaymentEventsRepository =>
            new CreditPaymentEventsAdapter(_directAsyncGatewayCreditLimitResponseMock.Object, _messagingLoggerMock.Object,
                                            _appSettingsMock.Object, _loggerServiceMock.Object);

        public CreditPaymentEventsAdapterTest()
        {
            _appSettingsMock.Setup(mock => mock.Get())
                .Returns(new CredinetAppSettings());
        }

        [Fact]
        public async Task ShouldSendActiveCreditsEvents()
        {
            List<CreditStatus> creditStatusList = new List<CreditStatus>();
            CreditPaymentEventResponse<List<CreditStatus>> activeCreditsEventResponse;
            activeCreditsEventResponse = CreditPaymentEventResponse<List<CreditStatus>>.BuildSuccessfulResponse("fer33de",
                                                                                                                creditStatusList);
            await creditPaymentEventsRepository.SendActiveCreditsEventsAsync("Credits.ActiveCreditsEvents", "4545", activeCreditsEventResponse);

            _directAsyncGatewayCreditLimitResponseMock.Verify(mock => mock.SendEvent(It.IsAny<string>(), It.IsAny<DomainEvent<object>>()), Times.Once);
        }

        [Fact]
        public async Task ShouldSendActiveCreditsEvents_SendException()
        {
            List<CreditStatus> creditStatusList = new List<CreditStatus>();
            CreditPaymentEventResponse<List<CreditStatus>> activeCreditsEventResponse;
            activeCreditsEventResponse = CreditPaymentEventResponse<List<CreditStatus>>.BuildSuccessfulResponse("fer33de",
                                                                                                                creditStatusList);
            _directAsyncGatewayCreditLimitResponseMock.Setup(mock => mock.SendEvent(It.IsAny<string>(), It.IsAny<DomainEvent<object>>()))
                    .ThrowsAsync(new Exception());

            await creditPaymentEventsRepository.SendActiveCreditsEventsAsync("Credits.ActiveCreditsEvents", "4545", activeCreditsEventResponse);

            _loggerServiceMock.Verify(mock => mock.LogError(It.IsAny<string>(), It.IsAny<object>(),
                                                           It.IsAny<Exception>(), It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Once);
            _messagingLoggerMock.Verify(mock => mock.LogTopicErrorAsync(It.IsAny<DomainEvent<object>>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldSendPayCreditsEventsAsync()
        {
            List<PaymentCreditResponse> creditsPaymentResponses = new List<PaymentCreditResponse>();
            CreditPaymentEventResponse<List<PaymentCreditResponse>> paymentCreditsEventResponse;
            paymentCreditsEventResponse = CreditPaymentEventResponse<List<PaymentCreditResponse>>.BuildSuccessfulResponse("fer33de",
                                                                                                 creditsPaymentResponses);
            await creditPaymentEventsRepository.SendPayCreditsEventsAsync("Credits.PayCreditsEvents", "45fer33de45", paymentCreditsEventResponse);

            _directAsyncGatewayCreditLimitResponseMock.Verify(mock => mock.SendEvent(It.IsAny<string>(), It.IsAny<DomainEvent<object>>()), Times.Once);
        }

        [Fact]
        public async Task ShouldSendPayCreditsEventsAsync_SendException()
        {
            List<PaymentCreditResponse> creditsPaymentResponses = new List<PaymentCreditResponse>();
            CreditPaymentEventResponse<List<PaymentCreditResponse>> paymentCreditsEventResponse;
            paymentCreditsEventResponse = CreditPaymentEventResponse<List<PaymentCreditResponse>>.BuildSuccessfulResponse("fer33de",
                                                                                                 creditsPaymentResponses);

            _directAsyncGatewayCreditLimitResponseMock.Setup(mock => mock.SendEvent(It.IsAny<string>(), It.IsAny<DomainEvent<object>>()))
                    .Throws(new Exception());

            await creditPaymentEventsRepository.SendPayCreditsEventsAsync("Credits.PayCreditsEvents", "45fer33de45", paymentCreditsEventResponse);

            _loggerServiceMock.Verify(mock => mock.LogError(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<Exception>(),
                It.IsAny<MethodBase>(), It.IsAny<string>()), Times.Once);
            _messagingLoggerMock.Verify(mock => mock.LogTopicErrorAsync(It.IsAny<DomainEvent<object>>(), It.IsAny<string>()), Times.Once);
        }
    }
}