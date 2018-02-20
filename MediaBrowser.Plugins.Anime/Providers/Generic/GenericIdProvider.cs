using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediaBrowser.Plugins.Anime.Providers.Generic
{
    abstract class GenericIdProvider : IExternalId
    {
        protected abstract String ProviderName { get; }
        protected abstract String IdAccessURL { get; }

        public string Name => ProviderName;

        public string Key => ProviderName;

        public string UrlFormatString => IdAccessURL;

        public bool Supports(IHasProviderIds item)
        {
            return item is Series;
        }
    }
}
