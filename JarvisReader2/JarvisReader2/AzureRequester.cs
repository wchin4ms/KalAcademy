using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JarvisReader
{
    class AzureRequester
    {
        private const string AZURE_DASHBOARD_URL = "https://sqlazureseas2.kustomfa.windows.net/v1/rest/query";
        // Need to ignore metadata strings since response str has '$type' and '$value' as JSON keys
        // And don't serialize request json objects/fields if null
        private static readonly JsonSerializerSettings JSON_SERIALIZER_SETTINGS = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore
        };
        public static void PostRequest(AzureRequestPayload payload)
        {
            string url = AZURE_DASHBOARD_URL;
            Console.WriteLine("URL: " + url);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            //HEADER INFO
            request.Accept = "application/json, text/javascript, */*; q=0.01";
            request.PreAuthenticate = true;
            request.ContentType = "application/json";
            request.Headers.Add("csrfToken", Properties.Get("azureCSRFtoken"));
            request.Headers.Add("Authorization", GetBearerAuthToken());
            request.Headers.Add("Origin", "https://jarvis-west.dc.ad.msft.net");
            request.Headers.Add("DNT", "1");
            request.Headers.Add("x-ms-app", "Jarvis.Dashboard");
            request.Headers.Add("x-ms-client-version", "1541067129595");
            request.Headers.Add("x-ms-user", "weschin");

            string jsonRequestPayload = JsonConvert.SerializeObject(payload, JSON_SERIALIZER_SETTINGS);
            Console.WriteLine("PAYLOAD: " + jsonRequestPayload);
            byte[] toBytes = Encoding.UTF8.GetBytes(jsonRequestPayload);
            request.ContentLength = toBytes.Length;
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(toBytes, 0, toBytes.Length);
                dataStream.Close();
            }

            // Get response
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
            {
                string responseStr = streamReader.ReadToEnd();
                Console.WriteLine("RESPONSE: " + responseStr);
                streamReader.Close();
                response.Close();
            }
        }
        private static CookieContainer cookies = null;
        private static CookieContainer GetCookies()
        {
            if (cookies == null)
            {
                cookies = new CookieContainer();
                Uri jarvisURI = new Uri("http://login.microsoftonline.com");
                cookies.Add(jarvisURI, new Cookie("AADSSO", Properties.Get("AADSSO")));
                cookies.Add(jarvisURI, new Cookie("CCState", Properties.Get("CCState")));
                cookies.Add(jarvisURI, new Cookie("CcsSharedSignInState", Properties.Get("CcsSharedSignInState")));
                cookies.Add(jarvisURI, new Cookie("ESTSAUTH", Properties.Get("ESTSAUTH")));
                cookies.Add(jarvisURI, new Cookie("ESTSAUTHLIGHT", Properties.Get("ESTSAUTHLIGHT")));
                cookies.Add(jarvisURI, new Cookie("ESTSAUTHPERSISTENT", Properties.Get("ESTSAUTHPERSISTENT")));
                cookies.Add(jarvisURI, new Cookie("ESTSSC", Properties.Get("ESTSSC")));
                cookies.Add(jarvisURI, new Cookie("ExternalIdpStateHash", Properties.Get("ExternalIdpStateHash")));
                cookies.Add(jarvisURI, new Cookie("SignInStateCookie", Properties.Get("SignInStateCookie")));
                cookies.Add(jarvisURI, new Cookie("buid", Properties.Get("buid")));
                cookies.Add(jarvisURI, new Cookie("esctx", Properties.Get("esctx")));
                cookies.Add(jarvisURI, new Cookie("fpc", Properties.Get("fpc")));
                cookies.Add(jarvisURI, new Cookie("stsservicecookie", Properties.Get("stsservicecookie")));
                cookies.Add(jarvisURI, new Cookie("x-ms-gateway-slice", Properties.Get("x-ms-gateway-slice")));
                cookies.Add(jarvisURI, new Cookie("x-ms-sts-inner", Properties.Get("x-ms-sts-inner")));
            }
            return cookies;
        }

        public static string GetBearerAuthToken()
        {
            string url = Properties.Get("azureLoginURL");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            //HEADER INFO
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Headers.Add("accept-encoding", "gzip, deflate, br");
            request.Headers.Add("accept-language", "en-US,en;q=0.9");
            request.CookieContainer = GetCookies();
            request.AllowAutoRedirect = false; // redirect loses bearer token

            // Get response
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string loc = response.Headers.Get("Location");
            Match result = Regex.Match(loc, @"#id_token=(.*?(?=&))");
            return "Bearer " + result.Groups[1];
        }
    }
}
