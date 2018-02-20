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
    abstract class GenericSeasonProvider : IRemoteMetadataProvider<Season, SeasonInfo>
    {
        protected readonly IHttpClient _httpClient;
        protected readonly IApplicationPaths _paths;
        protected readonly ILogger _log;
        protected abstract String ProviderName { get; }


        public GenericSeasonProvider(IApplicationPaths appPaths, IHttpClient httpClient, ILogManager logManager, IJsonSerializer jsonSerializer)
        {
            _httpClient = httpClient;
            _log = logManager.GetLogger(Name);
            _paths = appPaths;
        }

        public String Name => ProviderName;


        public abstract Task<HttpResponseInfo> GetImageResponse(string url, CancellationToken cancellationToken);

        public abstract Task<MetadataResult<Season>> GetMetadata(SeasonInfo info, CancellationToken cancellationToken);

        public abstract Task<IEnumerable<RemoteSearchResult>> GetSearchResults(SeasonInfo searchInfo, CancellationToken cancellationToken);
    }
}
