using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.XPath;
using MyPlainAPI.Utils;
using System.Net;
using System.Text.RegularExpressions;

namespace MyPlainAPI.Services.Retriever
{
    abstract internal class DataRetriever 
    {
        private static CookieCollection cookies = null;
        public CookieCollection LoginDirect(string userName, string password)
        {
            var uri =
                new Uri(
                    string.Format(HttpUtils.URL_LOGIN, HttpUtility.UrlEncode(userName),
                        HttpUtility.UrlEncode(password)), UriKind.Absolute);
            try
            {
                var cookies = new CookieCollection();
                HttpWebRequest req = WebRequest.Create(uri) as HttpWebRequest;
                req.PreAuthenticate = true;
                req.UseDefaultCredentials = true;
                req.KeepAlive = false;
                req.AllowAutoRedirect = false;
                req.Timeout = 6000;
                req.Method = WebRequestMethods.Http.Get;
                req.CookieContainer = new CookieContainer();
                using (var response = (HttpWebResponse)req.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
#if DEBUG
                        using (var sr = new StreamReader(response.GetResponseStream()))
                        {
                            var text = sr.ReadToEnd();
                            Global.Log.Write(text);
                        }
#endif
                        if (response.Cookies != null && response.Cookies.Count > 0)
                        {
                            return response.Cookies;
                        }
                    }
                }
                return cookies;
            }
            catch (Exception e)
            {
                Global.Log.Error(e);
            }
            return null;
        }
        protected internal virtual HttpClient CreateHttpClient()
        {
            var httpHandler = new HttpClientHandler() { CookieContainer = new CookieContainer() };
            // don't need cookie if using 'internaluse' service, otherwise cookie needs to be obtained
            if (cookies == null)
            {
                cookies = LoginDirect(HttpUtils.LOGIN_Email, HttpUtils.LOGIN_Password);
            }
            httpHandler.CookieContainer.Add(cookies);

            //var client = new HttpClient();
            var client = new HttpClient(httpHandler);
            client.BaseAddress = new Uri(HttpUtils.ServerHost);
            client.Timeout = HttpUtils.DefaultTimeOut;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue(HttpUtils.ContentTypeApplicationXml));

            return client;
        }
        protected internal async Task<XPathNavigator> GetDataXmlFormatAsync(Uri uri)
        {
            try
            {
                using (var client = CreateHttpClient())
                {
                    Global.Log.Write("Get Uri {0}", uri);
                    using (var response = await client.GetAsync(uri).ConfigureAwait(false))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            return await CheckResultCode(uri.ToString(), response).ConfigureAwait(false);
                        }
                        Global.Log.Error("[GET]Http code {0} for uri: {1}", response.StatusCode, uri);
                    }
                }
            }
            catch (HttpRequestException e)
            {
                // Handle exception.
                Global.Log.Error(e);
            }
            return null;
        }
        /// <summary>
        ///     Post xml Data to Web Service
        /// </summary>
        /// <typeparam name="T">result object type</typeparam>
        /// <param name="value">post data value</param>
        /// <param name="queryString">request query string</param>
        /// <returns></returns>
        protected internal async Task<XPathNavigator> PostDataXmlFormatAsync(Uri uri, string data)
        {
            try
            {
                using (var client = CreateHttpClient())
                {
                    Global.Log.Write("Post Uri {0}\r\n[Data]:{1}", uri, data);

                    var content = new StringContent(data);

                    using (var response = await client.PostAsync(uri, content).ConfigureAwait(false))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            return await CheckResultCode(uri.ToString(), response).ConfigureAwait(false);
                        }
                        Global.Log.Error("[POST]Http code {0} for uri: {1}", response.StatusCode, uri);
                    }
                }
            }
            catch (HttpRequestException e)
            {
                // Handle exception.
                Global.Log.Error(e);
            }
            return null;
        }

        private bool MoveToDataChunk(Stream s)
        {
            // Read chunk size
            int chunkSize = 0;
            while (true)
            {
                int ch = s.ReadByte();
                if (ch == -1)
                    return false;
                if (ch == 13)
                    continue;
                if (ch == 10)
                    break;
                if (ch < 48 || ch > 57)
                    throw new Exception(string.Format("Unexpected character {0} (int value {1}) found when unchunking Direct stream.  The remaining data in the stream is {2}", (char)ch, ch, StreamUtils.ReadAllString(s)));
                chunkSize = chunkSize * 10;
                chunkSize += (ch - 48);
            }
            return true;
        }
        /// <summary>
        ///     to handle reponse xml such as:
        ///     <awd>
        ///         <error code="600000" />
        ///     </awd>
        /// </summary>
        /// <param name="url">Request Uri</param>
        /// <param name="response">Check error code in reponse</param>
        /// <returns></returns>
        protected internal async Task<XPathNavigator> CheckResultCode(string url, HttpResponseMessage response)
        {
            if (response == null || response.Content == null) return null;
            using (var s = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                XPathNavigator nav = null;
                try
                {
                    // reponse from researchdatacenter has prefix numbers before real xml content
                    MoveToDataChunk(s);
                    nav = XmlUtils.LoadFromStream(s);
                    if (nav != null)
                    {
                        Global.Log.Write(nav.OuterXml);

                        var err = nav.SelectSingleNode("/awd/error");
                        if (err != null)
                        {
                            Global.Log.Error("Response Error Code: {0} for Uir {1}",
                                err.GetAttribute("code", string.Empty),
                                url);
                            return null;
                        }
                    }
                    return nav;
                }
                catch (XmlException)
                {
                    var text = response.Content.ReadAsStringAsync().Result;
                    Global.Log.Error("Error Get Response xml format: {0}", text);
                }
                return null;
            }
        }
    }
}