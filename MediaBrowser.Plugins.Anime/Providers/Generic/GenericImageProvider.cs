using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Providers;
using MediaBrowser.Model.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MediaBrowser.Plugins.Anime.Providers.Generic
{
    abstract class GenericImageProvider : IRemoteImageProvider
    {
        protected readonly IHttpClient _httpClient;
        protected readonly IApplicationPaths _paths;
        protected readonly ILogger _log;
        protected abstract String ProviderName { get; }
        protected readonly IEnumerable<ImageType> supportedImages = new[] { ImageType.Primary };
        public bool Supports(BaseItem item) => item is Season || item is Series;

        public GenericImageProvider(IApplicationPaths appPaths, IHttpClient httpClient, ILogManager logManager, IJsonSerializer jsonSerializer)
        {
            _httpClient = httpClient;
            _log = logManager.GetLogger(Name);
            _paths = appPaths;
        }

        public string Name => ProviderName;
        public IEnumerable<ImageType> GetSupportedImages(BaseItem item) => supportedImages;

        public abstract Task<HttpResponseInfo> GetImageResponse(string url, CancellationToken cancellationToken);

        public abstract Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken);

        

    }
}
