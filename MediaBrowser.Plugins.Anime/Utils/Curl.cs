using MediaBrowser.Model.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MediaBrowser.Plugins.Anime.Utils
{
    public class CurlAuth
    {
        private string _token;

        public CurlAuth(string username, string password)
        {
            _token = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
        }

        public string Token
        {
            get
            {
                return _token;
            }
        }
    }

    public sealed class Curl
    {
        private const String _app_tag = "MediaBrowser.Anime";
        private static volatile Curl _instance;
        private static object syncRoot = new Object();
        private readonly HttpClient httpClient;


        private Curl() {
            httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("User-Agent", _app_tag + "/Beta");
            //httpClient.DefaultRequestHeaders.ConnectionClose = true;
        }


        public static Curl instance
        {
            get {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null) _instance = new Curl();
                    }
                }

                return _instance;
            }
        }
        #region "Base functions"

        public async Task<HttpContent> Get(string uri, CurlAuth curlAuth = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var r = new HttpRequestMessage(HttpMethod.Get, uri);
            if (curlAuth != null)
            {
                r.Headers.Add("Authorization", "Basic " + curlAuth.Token);
            }

            var httpResponse = await httpClient.SendAsync(r, cancellationToken);
            return httpResponse.Content;           
        }

        public async Task<HttpContent> Post(string uri, string content, CurlAuth curlAuth = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var r = new HttpRequestMessage(HttpMethod.Post, uri);
            if (curlAuth != null)
            {
                r.Headers.Add("Authorization", "Basic " + curlAuth.Token);
            }

            if (content != null)
            {
                r.Content = new StringContent(content);
            }

            var httpResponse = await httpClient.SendAsync(r, cancellationToken);
            return httpResponse.Content;
        }

        #endregion

        #region "Wrappers around base function"

        public async Task<string> GetString(string uri, CurlAuth curlAuth = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var httpContent = await Get(uri, curlAuth, cancellationToken);
            return await httpContent.ReadAsStringAsync();
        }

        public async Task<Stream> GetStream(string uri, CurlAuth curlAuth = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var httpContent = await Get(uri, curlAuth, cancellationToken);
            return await httpContent.ReadAsStreamAsync();
        }

        public async Task<string> PostString(string uri, string content, CurlAuth curlAuth = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var httpContent = await Post(uri, content, curlAuth, cancellationToken);
            return await httpContent.ReadAsStringAsync();
        }

        public async Task<Stream> PostStream(string uri, string content, CurlAuth curlAuth = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var httpContent = await Post(uri, content, curlAuth, cancellationToken);
            return await httpContent.ReadAsStreamAsync();
        }

        public async Task<T> PostJson<T>(string uri, string content, IJsonSerializer jsonSerializer, CurlAuth curlAuth = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var s = await PostStream(uri, content, curlAuth, cancellationToken);
            return await Task.Run(() => jsonSerializer.DeserializeFromStream<T>(s), cancellationToken);
        }

        public async Task<T> GetJson<T>(string uri, string content, IJsonSerializer jsonSerializer, CurlAuth curlAuth = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var s = await GetStream(uri, curlAuth, cancellationToken);
            return await Task.Run(() => jsonSerializer.DeserializeFromStream<T>(s), cancellationToken);
        }

        #endregion

    }

}
