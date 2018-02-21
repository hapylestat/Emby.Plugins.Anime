using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Providers;
using MediaBrowser.Model.Serialization;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MediaBrowser.Plugins.Anime.Providers.Generic
{
    /// <summary>
    /// Experiment class, the way on making Emby Adaptors which will relate on particular Api implemntation, 
    /// this should force developer to implement only the way how to get data from the provider and all underlying process woud be the same.
    /// </summary>
    abstract class GenericSeasonProvider<T> : IRemoteMetadataProvider<Season, SeasonInfo> 
    {
        protected readonly IHttpClient _httpClient;
        protected readonly IApplicationPaths _paths;
        protected readonly ILogger _log;
        protected readonly string ProviderName;

        private readonly GenericSeriesProvider _seriesProvider;


        public GenericSeasonProvider(IApplicationPaths appPaths, IHttpClient httpClient, ILogManager logManager, IJsonSerializer jsonSerializer)
        {
            _httpClient = httpClient;
            _log = logManager.GetLogger(Name);
            _paths = appPaths;

            _seriesProvider = (GenericSeriesProvider)Activator.CreateInstance(typeof(T), new object[] { appPaths, httpClient, logManager, jsonSerializer });

            ProviderName = _seriesProvider.Name;
        }

        public String Name => ProviderName;


        public Task<HttpResponseInfo> GetImageResponse(string url, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<MetadataResult<Season>> GetMetadata(SeasonInfo info, CancellationToken cancellationToken)
        {
            var result = new MetadataResult<Season>
            {
                HasMetadata = true,
                Item = new Season
                {
                    Name = info.Name,
                    IndexNumber = info.IndexNumber
                }
            };

            var seriesId = info.SeriesProviderIds.GetOrDefault(Name);
            if (seriesId == null)
                return result;

            var seriesInfo = new SeriesInfo();
            seriesInfo.ProviderIds.Add(Name, seriesId);

            var seriesResult = await _seriesProvider.GetMetadata(seriesInfo, cancellationToken);
            if (seriesResult.HasMetadata)
            {
                result.Item.Studios = seriesResult.Item.Studios;
                result.Item.Genres = seriesResult.Item.Genres;
                
            }

            return result;
        }

        public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(SeasonInfo searchInfo, CancellationToken cancellationToken)
        {
            var metadata = await GetMetadata(searchInfo, cancellationToken).ConfigureAwait(false);
            var list = new List<RemoteSearchResult>();

            if (metadata.HasMetadata)
            {
                var res = new RemoteSearchResult
                {
                    Name = metadata.Item.Name,
                    PremiereDate = metadata.Item.PremiereDate,
                    ProductionYear = metadata.Item.ProductionYear,
                    ProviderIds = metadata.Item.ProviderIds,
                    SearchProviderName = Name
                };

                list.Add(res);
            }

            return list;
        }
    }

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
