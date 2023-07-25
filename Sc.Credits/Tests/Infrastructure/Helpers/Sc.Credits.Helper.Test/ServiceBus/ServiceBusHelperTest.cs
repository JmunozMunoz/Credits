using Moq;
using org.reactivecommons.api;
using org.reactivecommons.api.domain;
using System;
using System.Threading.Tasks;

namespace Sc.Credits.Helper.Test.ServiceBus
{
    public static class ServiceBusHelperTest
    {
        public static void SetupSubscribeCommandCallback<T>(Mock<IDirectAsyncGateway<T>> ayncGatewayMock, Action callback)
        {
            ayncGatewayMock.Setup(mock => mock.SuscripcionCommand(It.IsAny<string>(), It.IsAny<Func<Command<T>, Task>>(),
                It.IsAny<int>()))
                .Callback(callback);
        }

        public static void SetupSubscribeEventCallback<T>(Mock<IDirectAsyncGateway<T>> ayncGatewayMock, Action callback)
        {
            ayncGatewayMock.Setup(mock => mock.SuscripcionEvent(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Func<DomainEvent<T>, Task>>(), It.IsAny<int>()))
                .Callback(callback);
        }

        public static void SetupSubscribeEventCallbackWithDate<T>(Mock<IDirectAsyncGateway<T>> ayncGatewayMock, Action callback)
        {
            ayncGatewayMock.Setup(mock => mock.SuscripcionEvent(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Func<DomainEvent<T>, DateTime, Task>>(),
                    It.IsAny<int>()))
                .Callback(callback);
        }

        public static void SetupResponseReplyCallback<T, TResult>(Mock<IDirectAsyncGateway<T>> ayncGatewayMock, Action callback)
        {
            ayncGatewayMock.Setup(mock => mock.ResponseReply(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Func<AsyncQuery<T>, Task<TResult>>>(),
                    It.IsAny<int>()))
                .Callback(callback);
        }

        public static void InvokeSubscriptionCommand<T>(Mock<IDirectAsyncGateway<T>> ayncGatewayMock, T commandData)
        {
            async void callbackInvoke(string name, Func<Command<T>, Task> handle, int maxConcurrentCalls) =>
                await handle.Invoke(new Command<T>(name, It.IsAny<string>(), commandData));

            ayncGatewayMock.Setup(mock =>
                mock.SuscripcionCommand(It.IsAny<string>(), It.IsAny<Func<Command<T>, Task>>(), It.IsAny<int>()))
                    .Callback<string, Func<Command<T>, Task>, int>(callbackInvoke);
        }

        public static void InvokeSubscriptionEvent<T>(Mock<IDirectAsyncGateway<T>> ayncGatewayMock, T commandData)
        {
            async void callbackInvoke(string name, string eventId, Func<DomainEvent<T>, Task> handle, int maxConcurrentCalls) =>
                await handle.Invoke(new DomainEvent<T>(name, eventId, commandData));

            ayncGatewayMock.Setup(mock =>
                mock.SuscripcionEvent(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Func<DomainEvent<T>, Task>>(), It.IsAny<int>()))
                    .Callback<string, string, Func<DomainEvent<T>, Task>, int>(callbackInvoke);
        }

        public static void InvokeSubscriptionEventWithDate<T>(Mock<IDirectAsyncGateway<T>> ayncGatewayMock, T commandData)
        {
            async void callbackInvoke(string name, string eventId, Func<DomainEvent<T>, DateTime, Task> handle, int topicMaxConcurrentCalls) =>
                await handle.Invoke(new DomainEvent<T>(name, eventId, commandData), DateTime.Now);

            ayncGatewayMock.Setup(mock =>
                mock.SuscripcionEvent(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Func<DomainEvent<T>, DateTime, Task>>(), It.IsAny<int>()))
                    .Callback<string, string, Func<DomainEvent<T>, DateTime, Task>, int>(callbackInvoke);
        }

        public static void InvokeSubscriptionEventWithDate<T>(Mock<IDirectAsyncGateway<T>> ayncGatewayMock, DomainEvent<T> domainEvent)
        {
            async void callbackInvoke(string name, string eventId, Func<DomainEvent<T>, DateTime, Task> handle, int topicMaxConcurrentCalls) =>
                await handle.Invoke(domainEvent, DateTime.Now);

            ayncGatewayMock.Setup(mock =>
                mock.SuscripcionEvent(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Func<DomainEvent<T>, DateTime, Task>>(), It.IsAny<int>()))
                    .Callback<string, string, Func<DomainEvent<T>, DateTime, Task>, int>(callbackInvoke);
        }

        public static void InvokeResponseReply<T, TResult>(Mock<IDirectAsyncGateway<T>> ayncGatewayMock, T commandData, Action<TResult> callback)
        {
            async Task<TResult> callbackInvoke(string resource, string targetName, Func<AsyncQuery<T>, Task<TResult>> handle, int maxConcurrentCalls) =>
                await handle.Invoke(new AsyncQuery<T>
                {
                    QueryData = commandData,
                    Resource = resource
                });

            ayncGatewayMock.Setup(mock =>
                mock.ResponseReply(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Func<AsyncQuery<T>, Task<TResult>>>(),
                    It.IsAny<int>()))
                    .Callback<string, string, Func<AsyncQuery<T>, Task<TResult>>, int>(async (resource, targetName, handle, maxConcurrentCalls) =>
                        {
                            TResult result = await callbackInvoke(resource, targetName, handle, maxConcurrentCalls);
                            callback(result);
                        });
        }
    }
}