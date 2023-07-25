using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Sc.Credits.Domain.Model.Common.Gateway;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.Helpers.Commons.Cache;
using Sc.Credits.Helpers.Commons.Settings;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Managment.Services.Common
{
    /// <summary>
    /// App parameters service is an implementation of <see cref="IAppParametersService"/>
    /// </summary>
    public class AppParametersService
        : IAppParametersService
    {
        private readonly IAppParametersRepository _appParamatersRepository;
        private readonly ICache _cache;
        private readonly CredinetAppSettings _credinetAppSettings;
        private TimeSpan CacheTime => TimeSpan.FromMilliseconds(_credinetAppSettings.ParametersCacheMilliseconds);

        /// <summary>
        /// Creates a new instance of <see cref="AppParametersService"/>
        /// </summary>
        /// <param name="appParamatersRepository"></param>
        /// <param name="cache"></param>
        /// <param name="appSettings"></param>
        public AppParametersService(IAppParametersRepository appParamatersRepository,
            ICache cache,
            ISettings<CredinetAppSettings> appSettings)
        {
            _appParamatersRepository = appParamatersRepository;
            _cache = cache;
            _credinetAppSettings = appSettings.Get();
        }

        /// <summary>
        /// <see cref="IAppParametersService.GetAppParametersAsync"/>
        /// </summary>
        /// <returns></returns>
        public async Task<AppParameters> GetAppParametersAsync() =>
             new AppParameters(await GetParametersAsync());

        /// <summary>
        /// Get parameters
        /// </summary>
        /// <returns></returns>
        private async Task<List<Parameter>> GetParametersAsync() =>
             await _cache.GetOrCreateAsync(new CacheItem<List<Parameter>>(
                 new
                 {
                     name = "ParametersCache"
                 },
                 async () =>
                 {
                     return await _appParamatersRepository.GetAllParametersAsync();
                 },
                 CacheTime));

        /// <summary>
        /// <see cref="IAppParametersService.GetTransactionTypeAsync(int)"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TransactionType> GetTransactionTypeAsync(int id) =>
            await _cache.GetOrCreateAsync(new CacheItem<TransactionType>(
                new
                {
                    id,
                    type = nameof(TransactionType)
                },
                async () => await _appParamatersRepository.GetTransactionTypeAsync(id),
                CacheTime))
            ??
            throw new BusinessException(nameof(BusinessResponse.TransactionTypeNotFound), (int)BusinessResponse.TransactionTypeNotFound);

        /// <summary>
        /// <see cref="IAppParametersService.GetStatusAsync(int)"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Status> GetStatusAsync(int id) =>
            await _cache.GetOrCreateAsync(new CacheItem<Status>(
                new
                {
                    id,
                    type = nameof(Status)
                },
                async () => await _appParamatersRepository.GetStatusAsync(id),
                CacheTime))
           ??
           throw new BusinessException(nameof(BusinessResponse.StatusNotFound), (int)BusinessResponse.StatusNotFound);

        /// <summary>
        /// <see cref="IAppParametersService.GetSourceAsync(int)"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Source> GetSourceAsync(int id) =>
            await _cache.GetOrCreateAsync(new CacheItem<Source>(
                new
                {
                    id,
                    type = nameof(Source)
                },
                async () => await _appParamatersRepository.GetSourceAsync(id),
                CacheTime))
            ??
            throw new BusinessException(nameof(BusinessResponse.SourceNotFound), (int)BusinessResponse.SourceNotFound);

        /// <summary>
        /// <see cref="IAppParametersService.GetPaymentTypeAsync(int)"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PaymentType> GetPaymentTypeAsync(int id) =>
            await _cache.GetOrCreateAsync(new CacheItem<PaymentType>(
                 new
                 {
                     id,
                     type = nameof(PaymentType)
                 },
                 async () => await _appParamatersRepository.GetPaymentTypeAsync(id),
                 CacheTime))
             ??
             throw new BusinessException(nameof(BusinessResponse.PaymentTypeNotFound), (int)BusinessResponse.PaymentTypeNotFound);

        /// <summary>
        /// <see cref="IAppParametersService.GetAuthMethodAsync(int)"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<AuthMethod> GetAuthMethodAsync(int id) =>
            await _cache.GetOrCreateAsync(new CacheItem<AuthMethod>(
                 new
                 {
                     id,
                     type = nameof(AuthMethod)
                 },
                 async () => await _appParamatersRepository.GetAuthMethodAsync(id),
                 CacheTime))
             ??
             throw new BusinessException(nameof(BusinessResponse.AuthMethodNotFound), (int)BusinessResponse.AuthMethodNotFound);

        /// <summary>
        /// <see cref="IAppParametersService.GetSettings"/>
        /// </summary>
        /// <returns></returns>
        public CredinetAppSettings GetSettings() =>
            _credinetAppSettings;
    }
}