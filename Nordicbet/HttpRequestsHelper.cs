using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Nordicbet
{
    public class HttpRequestsHelper
    {
        private CookieContainer _cc = new CookieContainer();
        private Dictionary<string, string> _cookieJar = new Dictionary<string, string>();
        private HttpWebRequest _req;
        private bool _useProxy;
        private const string PROXYPORT = "9666";

        public HttpRequestsHelper(bool useProxy)
        {
            ServicePointManager.DefaultConnectionLimit = 500;
            this._useProxy = useProxy;
        }

        private void Fillcookiejar(WebResponse webresp)
        {
            HttpWebResponse response = (HttpWebResponse)webresp;
            foreach (Cookie cookie in response.Cookies)
            {
                if (this._cookieJar.ContainsKey(cookie.Name))
                {
                    this._cookieJar.Remove(cookie.Name);
                }
                this._cc.Add(cookie);
                this._cookieJar.Add(cookie.Name, cookie.Value);
            }
        }

        public string GetRequest(string url, string referer = null, ContentTypes contentType = null)
        {
            try
            {
                if (contentType != null)
                {
                    this._req.ContentType = contentType.ToString();
                    if (contentType == ContentTypes.Json)
                    {
                        this._req.Accept = "application/json, text/javascript, */*; q=0.01";
                    }
                }
                this.ReqSetup(url, referer);
                this._req.Method = "GET";
                WebResponse webresp = this._req.GetResponse();
                HttpWebResponse response1 = (HttpWebResponse)webresp;
                this.Fillcookiejar(webresp);
                return this.GetResponseStream(webresp);
            }
            catch
            {
            }
            return "";
        }

        private string GetResponseStream(WebResponse resp)
        {
            if (resp == null)
            {
                return string.Empty;
            }
            StreamReader reader = new StreamReader(resp.GetResponseStream());
            return reader.ReadToEnd();
        }

        public string PostRequest(string url, string postdata, string referer, ContentTypes contentType = null, Dictionary<string, string> customHeaders = null)
        {
            try
            {
                this.ReqSetup(url, referer);
                this._req.Method = "POST";
                this._req.KeepAlive = true;
                this._req.ServicePoint.Expect100Continue = false;
                if (contentType == ContentTypes.Json)
                {
                    this._req.ContentType = "application/json";
                    this._req.Accept = "application/json";
                }
                else if (contentType == ContentTypes.FormUrlencoded)
                {
                    this._req.ContentType = "application/x-www-form-urlencoded";
                }
                else
                {
                    this._req.ContentType = "application/x-www-form-urlencoded";
                }
                if ((customHeaders != null) && (customHeaders.Count > 0))
                {
                    foreach (KeyValuePair<string, string> pair in customHeaders)
                    {
                        this._req.Headers[pair.Key] = pair.Value;
                    }
                }
                byte[] bytes = Encoding.UTF8.GetBytes(postdata);
                this._req.ContentLength = bytes.Length;
                Stream requestStream = this._req.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
                WebResponse webresp = this._req.GetResponse();
                this.Fillcookiejar(webresp);
                return this.GetResponseStream(webresp);
            }
            catch
            {
            }
            return "";
        }

        private void ReqSetup(string url, string referer = null)
        {
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            ServicePointManager.FindServicePoint(new Uri(url));
            this._req = (HttpWebRequest)WebRequest.Create(url);
            this._req.ReadWriteTimeout = 0xb71b00;
            this._req.Timeout = 0xb71b00;
            this._req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
            this._req.Headers.Add("Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.3");
            this._req.Headers.Add("Accept-Language: en-US");
            this._req.KeepAlive = true;
            this._req.CookieContainer = this._cc;
            this._req.AllowAutoRedirect = true;
            if (referer != null)
            {
                this._req.Referer = referer;
            }
            if (this._cookieJar.Count > 0)
            {
                foreach (KeyValuePair<string, string> pair in this._cookieJar)
                {
                    this._req.Headers.Add(HttpRequestHeader.Cookie, pair.Key + "=" + pair.Value);
                }
            }
        }
    }
}
