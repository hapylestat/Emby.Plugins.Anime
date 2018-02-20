﻿using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Providers;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Plugins.Anime.Providers.AniList;
using MediaBrowser.Plugins.Anime.Providers.Generic;
using MediaBrowser.Plugins.Anime.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MediaBrowser.Plugins.Anime.Providers.AniList.Metadata
{
    class AniListSeriesProvider : GenericSeriesProvider
    {
        private readonly api _api;
        private readonly Curl curl = Curl.instance;
        private readonly IJsonSerializer _jsonSerializer;
        public static readonly SemaphoreSlim ResourcePool = new SemaphoreSlim(1, 1);


        public AniListSeriesProvider(IApplicationPaths appPaths, IHttpClient httpClient, ILogManager logManager, IJsonSerializer jsonSerializer) : base(appPaths, httpClient, logManager, jsonSerializer)
        {
            _api = new api(jsonSerializer);
            _jsonSerializer = jsonSerializer;
        }

        protected override string ProviderName =>ProviderNames.AniList;

        public override Task<HttpResponseInfo> GetImageResponse(string url, CancellationToken cancellationToken)
        {
            return _httpClient.GetResponse(new HttpRequestOptions
            {
                CancellationToken = cancellationToken,
                Url = url,
                ResourcePool = ResourcePool
            });
        }

        private void StoreImageUrl(string series, string url, string type)
        {
            var path = Path.Combine(_paths.CachePath, "anilist", type, series + ".txt");
            var directory = Path.GetDirectoryName(path);
            Directory.CreateDirectory(directory);

            File.WriteAllText(path, url);
        }

        public async override Task<MetadataResult<Series>> GetMetadata(SeriesInfo info, CancellationToken cancellationToken)
        {
            var result = new MetadataResult<Series>();

            String aid;

            if (!info.ProviderIds.TryGetValue(ProviderNames.AniList, out aid))
            {
                _log.Info("Start AniList... Searching(" + info.Name + ")");
                aid = await _api.FindSeries(info.Name, cancellationToken);
            }

            if (!string.IsNullOrEmpty(aid))
            {
                Models.RootObject WebContent = await curl.PostJson<Models.RootObject>(api.AniList_anime_link.Replace("{0}", aid), null, _jsonSerializer);
                result.Item = new Series();
                result.HasMetadata = true;

                result.People = await _api.getPersonInfo(WebContent.data.Media.id);
                result.Item.ProviderIds.Add(ProviderNames.AniList, aid);
                result.Item.Overview = WebContent.data.Media.description;
                try
                {
                    //AniList has a max rating of 5
                    result.Item.CommunityRating = (WebContent.data.Media.averageScore / 10);
                }
                catch (Exception) { }
                foreach (var genre in _api.Get_Genre(WebContent))
                    result.Item.AddGenre(genre);
                //GenreHelper.CleanupGenres(result.Item);
                //StoreImageUrl(aid, WebContent.data.Media.coverImage.large, "image");
            }
            return result;
        }

        public async override Task<IEnumerable<RemoteSearchResult>> GetSearchResults(SeriesInfo searchInfo, CancellationToken cancellationToken)
        {
            var results = new Dictionary<string, RemoteSearchResult>();

            String aid;

            if (!searchInfo.ProviderIds.TryGetValue(ProviderNames.AniList, out aid))
            {
                if (!results.ContainsKey(aid))
                    results.Add(aid, await _api.GetAnime(aid));
            }

            if (!string.IsNullOrEmpty(searchInfo.Name))
            {
                List<string> ids = await _api.Search_GetSeries_list(searchInfo.Name, cancellationToken);
                foreach (string a in ids)
                {
                    results.Add(a, await _api.GetAnime(a));
                }
            }

            return results.Values;
        }


    }
}
