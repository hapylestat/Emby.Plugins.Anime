using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
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
    abstract class GenericSeriesProvider : IRemoteMetadataProvider<Series, SeriesInfo>, IHasOrder
    {
        protected readonly IHttpClient _httpClient;
        protected readonly IApplicationPaths _paths;
        protected readonly ILogger _log;
        protected abstract String ProviderName { get; }
        public int Order => -99;


        public GenericSeriesProvider(IApplicationPaths appPaths, IHttpClient httpClient, ILogManager logManager, IJsonSerializer jsonSerializer)
        {
            _httpClient = httpClient;
            _log = logManager.GetLogger(Name);
            _paths = appPaths;
        }

        public String Name => ProviderName;

        abstract public Task<MetadataResult<Series>> GetMetadata(SeriesInfo info, CancellationToken cancellationToken);

        abstract public Task<IEnumerable<RemoteSearchResult>> GetSearchResults(SeriesInfo searchInfo, CancellationToken cancellationToken);

        abstract public Task<HttpResponseInfo> GetImageResponse(string url, CancellationToken cancellationToken);
    }
}
