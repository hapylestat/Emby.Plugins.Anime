using MediaBrowser.Common.Net;
using MediaBrowser.Model.Serialization;
using System;
using System.IO;
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
        private readonly IHttpClient httpClient;
 

        public Curl(IHttpClient _httpClient) {
            httpClient = _httpClient;
        }


        #region "Base functions"

        public async Task<HttpResponseInfo> Get(string uri, CurlAuth curlAuth = null, CancellationToken cancellationToken = default(CancellationToken), SemaphoreSlim _resourcePool = null)
        {
            
            var r = new HttpRequestOptions();

            if (curlAuth != null) {
                r.RequestHeaders.Add("Authorization", "Basic " + curlAuth.Token);
            }
            if (_resourcePool != null) r.ResourcePool = _resourcePool;


            r.RequestHeaders.Add("Accept-Charset", "utf-8");
            r.CancellationToken = cancellationToken;
            r.Url = uri;

            return await httpClient.SendAsync(r, HttpMethod.Get.ToString().ToUpper());
        }

        public async Task<HttpResponseInfo> Post(string uri, string content, CurlAuth curlAuth = null, CancellationToken cancellationToken = default(CancellationToken), SemaphoreSlim _resourcePool = null)
        {
            var r = new HttpRequestOptions();

            if (curlAuth != null) {
                r.RequestHeaders.Add("Authorization", "Basic " + curlAuth.Token);
            }
            if (content != null) r.RequestContent = content;
            if (_resourcePool != null) r.ResourcePool = _resourcePool;
            r.RequestHeaders.Add("Accept-Charset", "utf-8");
            r.CancellationToken = cancellationToken;
            r.Url = uri;


            return await httpClient.SendAsync(r, HttpMethod.Post.ToString().ToUpper());
         }

        #endregion

        #region "Wrappers around base function"

        public async Task<string> GetString(string uri, CurlAuth curlAuth = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var httpContent = await Get(uri, curlAuth, cancellationToken);
            string result;
            using (StreamReader streamReader = new StreamReader(httpContent.Content))
            {
                result = await streamReader.ReadToEndAsync();
            }

            return result;
        }

        public async Task<Stream> GetStream(string uri, CurlAuth curlAuth = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var httpContent = await Get(uri, curlAuth, cancellationToken);
            return httpContent.Content;
        }

        public async Task<string> PostString(string uri, string content, CurlAuth curlAuth = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var httpContent = await Post(uri, content, curlAuth, cancellationToken);
            string result;
            using (StreamReader streamReader = new StreamReader(httpContent.Content))
            {
                result = await streamReader.ReadToEndAsync();
            }

            return result;
        }

        public async Task<Stream> PostStream(string uri, string content, CurlAuth curlAuth = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var httpContent = await Post(uri, content, curlAuth, cancellationToken);
            return httpContent.Content;
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
