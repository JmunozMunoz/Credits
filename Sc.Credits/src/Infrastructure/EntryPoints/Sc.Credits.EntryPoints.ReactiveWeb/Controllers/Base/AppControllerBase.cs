using credinet.comun.api;
using credinet.comun.negocio;
using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static credinet.comun.negocio.RespuestaNegocio<credinet.exception.middleware.models.ResponseEntity>;
using static credinet.exception.middleware.models.ResponseEntity;

namespace Sc.Credits.EntryPoints.ReactiveWeb.Controllers.Base
{
    /// <summary>
    /// App controller base
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AppControllerBase<T>
        : BaseController<T>
    {
        public string Country => EnvironmentHelper.GetCountryOrDefault(_credinetAppSettings.DefaultCountry);

        private readonly ILoggerService<T> _loggerService;
        private readonly CredinetAppSettings _credinetAppSettings;

        /// <summary>
        /// New app controller base
        /// </summary>
        /// <param name="credinetAppSettings"></param>
        /// <param name="loggerService"></param>
        public AppControllerBase(CredinetAppSettings credinetAppSettings,
            ILoggerService<T> loggerService)
        {
            _loggerService = loggerService;
            _credinetAppSettings = credinetAppSettings;
        }

        /// <summary>
        /// Handle async request
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="requestHandler"></param>
        /// <param name="logId"></param>
        /// <param name="notifyBusinessException"></param>
        /// <param name="exceptionCode"></param>
        /// <returns></returns>
        public async Task<IActionResult> HandleAsyncRequestAsync<TResult>(Func<Task<TResult>> requestHandler, string logId,
            bool notifyBusinessException = false, BusinessResponse exceptionCode = BusinessResponse.NotControlledException)
        {
            string responseMessage = string.Empty;
            int responseCode = 0;
            dynamic data = null;
            bool businessException = false;
            try
            {
                _loggerService.LogInfo(logId, GetEventName(), GetLogEventDetailsAsync());

                data = await requestHandler();
            }
            catch (BusinessException be)
            {
                await LogBusinessExceptionAsync(be, logId);

                if (notifyBusinessException)
                {
                    await _loggerService.NotifyAsync(logId, GetEventName(be), GetLogExceptionDetailsAsync(be));
                }

                responseCode = be.code;
                responseMessage = be.Message;
                businessException = true;
            }
            catch (Exception ex)
            {
                await LogExceptionAndNotifyAsync(ex, logId);

                throw new BusinessException(ex.Message, (int)exceptionCode);
            }

            ResponseEntity responseEntity = Build(Request.Path.Value, responseCode, responseMessage, Country, data);

            return businessException
                ? BadRequest(responseEntity)
                : await ProcesarResultadoAsincrono(Exito(responseEntity));
        }

        /// <summary>
        /// Handle request
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="requestHandler"></param>
        /// <param name="logId"></param>
        /// <param name="notifyBusinessException"></param>
        /// <param name="exceptionCode"></param>
        /// <param name="isAsyncOperation"></param>
        /// <returns></returns>
        public async Task<IActionResult> HandleRequestAsync<TResult>(Func<Task<TResult>> requestHandler, string logId,
            bool notifyBusinessException = false, BusinessResponse exceptionCode = BusinessResponse.NotControlledException,
            bool isAsyncOperation = false)
        {
            string responseMessage = string.Empty;
            int responseCode = 0;
            dynamic data = null;
            bool businessException = false;
            try
            {
                _loggerService.LogInfo(logId, GetEventName(), GetLogEventDetailsAsync());

                data = await requestHandler();
            }
            catch (BusinessException be)
            {
                await LogBusinessExceptionAsync(be, logId);

                if (notifyBusinessException)
                {
                    await _loggerService.NotifyAsync(logId, GetEventName(be), GetLogExceptionDetailsAsync(be));
                }

                responseCode = be.code;
                responseMessage = be.Message;
                businessException = true;
            }
            catch (Exception ex)
            {
                await LogExceptionAndNotifyAsync(ex, logId);

                throw new BusinessException(ex.Message, (int)exceptionCode);
            }

            ResponseEntity responseEntity = Build(Request.Path.Value, responseCode, responseMessage, Country, data);

            if (businessException)
                return BadRequest(responseEntity);

            return isAsyncOperation
                ? await ProcesarResultadoAsincrono(Exito(responseEntity))
                : await ProcesarResultado(Exito(responseEntity));
        }

        /// <summary>
        /// Handle async request and log event
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="requestHandler"></param>
        /// <param name="logId"></param>
        /// <param name="notifyBusinessException"></param>
        /// <param name="exceptionCode"></param>
        /// <returns></returns>
        public async Task<IActionResult> HandleAsyncRequestAndLogEventAsync<TResult>(Func<Task<TResult>> requestHandler, string logId,
            bool notifyBusinessException = false, BusinessResponse exceptionCode = BusinessResponse.NotControlledException) =>
            await HandleAsyncRequestAsync(async () =>
            {
                await LogEventAsync(logId);

                return await requestHandler();
            },
            logId,
            notifyBusinessException,
            exceptionCode);

        /// <summary>
        /// Handle request and log event
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="requestHandler"></param>
        /// <param name="logId"></param>
        /// <param name="notifyBusinessException"></param>
        /// <param name="exceptionCode"></param>
        /// <param name="isAsyncOperation"></param>
        /// <returns></returns>
        public async Task<IActionResult> HandleRequestAndLogEventAsync<TResult>(Func<Task<TResult>> requestHandler, string logId,
            bool notifyBusinessException = false, BusinessResponse exceptionCode = BusinessResponse.NotControlledException,
            bool isAsyncOperation = false) =>
            await HandleRequestAsync(async () =>
            {
                await LogEventAsync(logId);

                return await requestHandler();
            },
            logId,
            notifyBusinessException,
            exceptionCode,
            isAsyncOperation);

        /// <summary>
        /// Get event name
        /// </summary>
        /// <param name="ex"></param>
        private string GetEventName(Exception ex = null)
        {
            string actionName = ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = ControllerContext.RouteData.Values["controller"].ToString();

            string defaultEventName = $"{_credinetAppSettings.DomainName}.{controllerName}.{actionName}";

            return ex != null ?
                $"Exception.{defaultEventName}.{ex.GetType().Name}"
                :
                defaultEventName;
        }

        /// <summary>
        /// Log exception and notify
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="logId"></param>
        /// <returns></returns>
        private async Task LogExceptionAndNotifyAsync(Exception ex, string logId)
        {
            object logDetails = await GetLogExceptionDetailsAsync(ex);

            string eventName = GetEventName(ex);

            _loggerService.LogError(logId, eventName, logDetails, ex);

            await _loggerService.NotifyAsync(logId, eventName, logDetails);
        }

        /// <summary>
        /// Log event
        /// </summary>
        /// <param name="logId"></param>
        /// <returns></returns>
        private async Task LogEventAsync(string logId)
        {
            object logDetails = await GetLogEventDetailsAsync();

            await _loggerService.NotifyAsync(logId, GetEventName(), logDetails);
        }

        /// <summary>
        /// Log business exception
        /// </summary>
        /// <param name="be"></param>
        /// <param name="logId"></param>
        /// <returns></returns>
        private async Task LogBusinessExceptionAsync(BusinessException be, string logId)
        {
            object logDetails = await GetLogExceptionDetailsAsync(be);

            string eventName = GetEventName(be);

            _loggerService.LogWarning(logId, eventName, logDetails, be);
        }

        /// <summary>
        /// Get log exception details
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private async Task<object> GetLogExceptionDetailsAsync(Exception ex) =>
            new
            {
                exception = ex.ToString(),
                queryString = Request.QueryString.Value,
                body = await GetBodyAsync(),
                headers = Request.Headers
                    .Select(header =>
                        new
                        {
                            header.Key,
                            header.Value
                        })
            };

        /// <summary>
        /// Get log event details
        /// </summary>
        /// <returns></returns>
        private async Task<object> GetLogEventDetailsAsync() =>
            new
            {
                queryString = Request.QueryString.Value,
                body = await GetBodyAsync(),
                headers = Request.Headers
                    .Select(header =>
                        new
                        {
                            header.Key,
                            header.Value
                        })
            };

        /// <summary>
        /// Get body
        /// </summary>
        /// <returns></returns>
        private async Task<object> GetBodyAsync()
        {
            try
            {
                StreamReader reader = new StreamReader(Request.Body);
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                return JsonConvert.DeserializeObject(await reader.ReadToEndAsync());
            }
            catch (NotSupportedException)
            {
                return null;
            }
        }

        protected async Task<ObjectResult> ProcesarResultadoAsincrono<Y>(RespuestaNegocio<Y> respuesta)
        {
            if (respuesta.Correcto)
            {
                return new AcceptedResult("", respuesta.Resultado);
            }

            return await ResultadoError(respuesta);
        }
    }
}