using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Providers;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Plugins.Anime.Providers.Generic;
using MediaBrowser.Plugins.Anime.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MediaBrowser.Plugins.Anime.Providers.AniList.Metadata
{
    class AniListImageProvider : GenericImageProvider
    {
        private readonly ApiClient _api;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly Curl curl = Curl.instance;

        public AniListImageProvider(IApplicationPaths appPaths, IHttpClient httpClient, ILogManager logManager, IJsonSerializer jsonSerializer) : base(appPaths, httpClient, logManager, jsonSerializer)
        {
            _api = new ApiClient(jsonSerializer);
            _jsonSerializer = jsonSerializer;
        }

        protected override string ProviderName => ProviderNames.AniList;

        protected override IEnumerable<ImageType> supportedImages => new[] { ImageType.Primary, ImageType.Banner };

        public override Task<HttpResponseInfo> GetImageResponse(string url, CancellationToken cancellationToken)
        {
            return _httpClient.GetResponse(new HttpRequestOptions
            {
                CancellationToken = cancellationToken,
                Url = url,
                ResourcePool = AniListSeriesProvider.ResourcePool
            });
        }

        public async override Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken)
        {
            var list = new List<RemoteImageInfo>();
            var seriesId = item.GetProviderId(ProviderNames.AniList);

            if (!string.IsNullOrEmpty(seriesId))
            {
                var rootObject = await curl.PostJson<Models.RootObject>(ApiClient.AniList_anime_link.Replace("{0}", seriesId), null, _jsonSerializer);
                var primary = _api.Get_ImageUrl(rootObject);
                var banner = _api.Get_BannerUrl(rootObject);
                if (primary != null)
                {
                    list.Add(new RemoteImageInfo
                    {
                        ProviderName = Name,
                        Type = ImageType.Primary,
                        Url = primary
                    });
                }
                if (banner != null) {
                    list.Add(new RemoteImageInfo
                    {
                        ProviderName = Name,
                        Type = ImageType.Banner,
                        Url = banner
                    });
                }
            }
            return list;
        }
    }
}
