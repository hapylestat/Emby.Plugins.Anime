using MediaBrowser.Plugins.Anime.Providers.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediaBrowser.Plugins.Anime.Providers.AniList.Metadata
{
    class AniListExternalId : GenericIdProvider
    {
        protected override string ProviderName => ProviderNames.AniList;

        protected override string IdAccessURL => "http://anilist.co/anime/{0}/";
    }
}
