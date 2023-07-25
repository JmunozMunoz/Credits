using Sc.Credits.Domain.Model.Common.Gateway;
using Sc.Credits.Helpers.Commons.Cache;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Managment.Services.Common
{
    /// <summary>
    /// Templates service is an implementation of <see cref="ITemplatesService"/>
    /// </summary>
    public class TemplatesService : ITemplatesService
    {
        private readonly CredinetAppSettings _credinetAppSettings;
        private readonly IBlobStoreRepository _blobStoreRepository;
        private readonly ICache _cache;

        /// <summary>
        /// Creates a new instance of <see cref="TemplatesService"/>
        /// </summary>
        /// <param name="appParametersService"></param>
        /// <param name="blobStoreRepository"></param>
        /// <param name="cache"></param>
        public TemplatesService(IAppParametersService appParametersService,
            IBlobStoreRepository blobStoreRepository,
            ICache cache)
        {
            _credinetAppSettings = appParametersService.GetSettings();
            _blobStoreRepository = blobStoreRepository;
            _cache = cache;
        }

        /// <summary>
        /// <see cref="ITemplatesService.GetAsync(string)"/>
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        public async Task<string> GetAsync(string templateName) =>
            await _cache.GetOrCreateAsync(new CacheItem<string>(templateName,
                async () =>
                {
                    byte[] fileBytes = await _blobStoreRepository.DownloadFileAsync(templateName, string.Empty,
                        _credinetAppSettings.TemplatesBlobContainerName);
                    using (StreamReader streamReader = new StreamReader(new MemoryStream(fileBytes)))
                    {
                        return streamReader.ReadToEnd();
                    }
                },
                timeOut: TimeSpan.FromMinutes(_credinetAppSettings.TemplatesCacheMinutes)));
    }
}