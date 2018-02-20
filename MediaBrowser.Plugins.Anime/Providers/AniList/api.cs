﻿using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using MediaBrowser.Plugins.Anime.Configuration;
using MediaBrowser.Model.Serialization;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Plugins.Anime.Providers.AniList;
using MediaBrowser.Plugins.Anime.Utils;
using MediaBrowser.Controller.Entities;

namespace MediaBrowser.Plugins.Anime.Providers.AniList
{
    /// <summary>
    /// Based on the new API from AniList
    /// 🛈 This code works with the API Interface (v2) from AniList
    /// 🛈 https://anilist.gitbooks.io/anilist-apiv2-docs
    /// 🛈 THIS IS AN UNOFFICAL API INTERFACE FOR EMBY
    /// </summary>
    public class api
    {
        private static IJsonSerializer _jsonSerializer;
        private static Curl curl = Curl.instance;
        private readonly List<string> anime_search_names = new List<string>();
        private readonly List<string> anime_search_ids = new List<string>();

        private const string SearchLink = @"https://graphql.anilist.co/api/v2?query=
query ($query: String, $type: MediaType) {
  Page {
    media(search: $query, type: $type) {
      id
      title {
        romaji
        english
        native
      }
      coverImage {
        medium
        large
      }
      format
      type
      averageScore
      popularity
      episodes
      season
      hashtag
      isAdult
      startDate {
        year
        month
        day
      }
      endDate {
        year
        month
        day
      }
    }
  }
}&variables={ ""query"":""{0}"",""type"":""ANIME""}";

        protected internal const string AniList_anime_link = @"https://graphql.anilist.co/api/v2?query=query($id: Int!, $type: MediaType) {
  Media(id: $id, type: $type)
        {
            id
            title {
                romaji
                english
              native
      userPreferred
            }
            startDate {
                year
                month
              day
            }
            endDate {
                year
                month
              day
            }
            coverImage {
                large
                medium
            }
            bannerImage
            format
    type
    status
    episodes
    chapters
    volumes
    season
    description
    averageScore
    meanScore
    genres
    synonyms
    nextAiringEpisode {
                airingAt
                timeUntilAiring
      episode
    }
        }
    }&variables={ ""id"":""{0}"",""type"":""ANIME""}";

     private const string AniList_anime_char_link = @"https://graphql.anilist.co/api/v2?query=query($id: Int!, $type: MediaType, $page: Int = 1) {
  Media(id: $id, type: $type) {
    id
    characters(page: $page, sort: [ROLE]) {
      pageInfo {
        total
        perPage
        hasNextPage
        currentPage
        lastPage
      }
      edges {
        node {
          id
          name {
            first
            last
          }
          image {
            medium
            large
          }
        }
        role
        voiceActors {
          id
          name {
            first
            last
            native
          }
          image {
            medium
            large
          }
          language
        }
      }
    }
  }
}&variables={ ""id"":""{0}"",""type"":""ANIME""}";

        public api(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }
        /// <summary>
        /// API call to get the anime with the id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RemoteSearchResult> GetAnime(string id)
        {
            Models.RootObject WebContent = await curl.PostJson<Models.RootObject>(AniList_anime_link.Replace("{0}", id), null, _jsonSerializer);

            var result = new RemoteSearchResult
            {
                Name = ""
            };

            result.SearchProviderName = WebContent.data.Media.title.romaji;
            result.ImageUrl = WebContent.data.Media.coverImage.large;
            result.SetProviderId(ProviderNames.AniList, id);
            result.Overview = WebContent.data.Media.description;

            return result;
        }

        /// <summary>
        /// API call to select the lang
        /// </summary>
        /// <param name="WebContent"></param>
        /// <param name="preference"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        private string SelectName(Models.RootObject WebContent, TitlePreferenceType preference, string language)
        {
            switch (preference)
            {
                case TitlePreferenceType.Localized when language == "en":
                    return WebContent.data.Media.title.english;
                case TitlePreferenceType.Japanese:
                    return WebContent.data.Media.title.native;
            }

            return  WebContent.data.Media.title.romaji;
        }

        /// <summary>
        /// API call to get the title with the right lang
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="WebContent"></param>
        /// <returns></returns>
        public string Get_title(string lang, Models.RootObject WebContent)
        {
            switch (lang)
            {
                case "en":
                    return WebContent.data.Media.title.english;

                case "jap":
                    return WebContent.data.Media.title.native;

                //Default is jap_r
                default:
                   return WebContent.data.Media.title.romaji;
            }
        }
        public async Task<List<PersonInfo>> getPersonInfo(int id)
        {
            List<PersonInfo> lpi = new List<PersonInfo>();
            Models.RootObject WebContent = await curl.PostJson<Models.RootObject>(AniList_anime_char_link.Replace("{0}", id.ToString()), null, _jsonSerializer);
            foreach (Models.Edge edge in WebContent.data.Media.characters.edges)
            {
                PersonInfo pi = new PersonInfo();
                pi.Name = edge.node.name.first+" "+ edge.node.name.last;
                pi.ItemId = ToGuid(edge.node.id);
                pi.ImageUrl = edge.node.image.large;
                pi.Role = edge.role;
            }
            return lpi;
        }
        /// <summary>
        /// Convert int to Guid
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Guid ToGuid(int value)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }
        /// <summary>
        /// API call to get the genre of the anime
        /// </summary>
        /// <param name="WebContent"></param>
        /// <returns></returns>
        public List<string> Get_Genre(Models.RootObject WebContent)
        {

            return WebContent.data.Media.genres;
        }

        /// <summary>
        /// API call to get the img url
        /// </summary>
        /// <param name="WebContent"></param>
        /// <returns></returns>
        public string Get_ImageUrl(Models.RootObject WebContent)
        {
            return WebContent.data.Media.coverImage.large;
        }

        /// <summary>
        /// API call too get the rating
        /// </summary>
        /// <param name="WebContent"></param>
        /// <returns></returns>
        public string Get_Rating(Models.RootObject WebContent)
        {
            return (WebContent.data.Media.averageScore / 10).ToString();
        }

        /// <summary>
        /// API call to get the description
        /// </summary>
        /// <param name="WebContent"></param>
        /// <returns></returns>
        public string Get_Overview(Models.RootObject WebContent)
        {
            return WebContent.data.Media.description;
        }

        /// <summary>
        /// API call to search a title and return the right one back
        /// </summary>
        /// <param name="title"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<string> Search_GetSeries(string title, CancellationToken cancellationToken)
        {
            anime_search_names.Clear();
            anime_search_ids.Clear();

            Models.RootObject WebContent = await curl.PostJson<Models.RootObject>(SearchLink.Replace("{0}", title), null, _jsonSerializer);
            foreach (Models.Medium media in WebContent.data.Page.media) {
                //get id

                try
                {

                    if (await Task.Run(() => Equals_check.Compare_strings(media.title.romaji, title), cancellationToken))
                    {
                        return media.id.ToString();
                    }
                    if (await Task.Run(() => Equals_check.Compare_strings(media.title.english, title), cancellationToken))
                    {
                        return media.id.ToString();
                    }

                    int n;
                    if (Int32.TryParse(media.id.ToString(), out n))
                    {
                        anime_search_names.Add(media.title.romaji);
                        anime_search_ids.Add(media.id.ToString());
                    }
                }

                catch (Exception) { }
            }
            
            return null;
        }

        /// <summary>
        /// API call to search a title and return a list back
        /// </summary>
        /// <param name="title"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<List<string>> Search_GetSeries_list(string title, CancellationToken cancellationToken)
        {
            List<string> result = new List<string>();
            Models.RootObject WebContent = await curl.PostJson<Models.RootObject>(SearchLink.Replace("{0}", title), null, _jsonSerializer);

            foreach (Models.Medium media in WebContent.data.Page.media)
            {
                //get id

                try
                {

                    if (await Task.Run(() => Equals_check.Compare_strings(media.title.romaji, title), cancellationToken))
                    {
                        result.Add(media.id.ToString());
                    }
                    if (await Task.Run(() => Equals_check.Compare_strings(media.title.english, title), cancellationToken))
                    {
                        result.Add(media.id.ToString());
                    }
                }

                catch (Exception) { }
            }
            return result;
        }

        /// <summary>
        /// SEARCH Title
        /// </summary>
        public async Task<string> FindSeries(string title, CancellationToken cancellationToken)
        {
            string aid = await Search_GetSeries(title, cancellationToken);
            if (!string.IsNullOrEmpty(aid))
            {
                return aid;
            }
            aid = await Search_GetSeries(Equals_check.clear_name(title), cancellationToken);
            if (!string.IsNullOrEmpty(aid))
            {
                return aid;
            }
            return null;
        }

        /// <summary>
        /// Simple regex
        /// </summary>
        public async Task<string> One_line_regex(Regex regex, string match, int group = 1, int match_int = 0)
        {
            int x = 0;
            MatchCollection matches = await Task.Run(() => regex.Matches(match));
            foreach (Match _match in matches)
            {
                if (x == match_int)
                {
                    return await Task.Run(() => _match.Groups[group].Value.ToString());
                }
                x++;
            }
            return "";
        }

    }
}