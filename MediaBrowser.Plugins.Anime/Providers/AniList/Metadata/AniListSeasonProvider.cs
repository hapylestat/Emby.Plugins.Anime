using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Plugins.Anime.Providers.Generic;


namespace MediaBrowser.Plugins.Anime.Providers.AniList.Metadata
{
    class AniListSeasonProvider : GenericSeasonProvider<AniListSeriesProvider>
    {
        public AniListSeasonProvider(IApplicationPaths appPaths, IHttpClient httpClient, ILogManager logManager, IJsonSerializer jsonSerializer) : base(appPaths, httpClient, logManager, jsonSerializer)
        {
            // Basic auto-provider
        }
    }
}
