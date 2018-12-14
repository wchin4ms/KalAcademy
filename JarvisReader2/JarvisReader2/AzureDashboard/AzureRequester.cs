using JarvisReader.Authentication;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace JarvisReader.AzureDashboard
{
    class AzureRequester : Requester<AzureResponse>
    {
        private const string AZURE_DASHBOARD_URL = "https://sqlazureuk2.kustomfa.windows.net/v1/rest/query";
        public static void OptionsRequest()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(AZURE_DASHBOARD_URL);
            request.Method = "OPTIONS";
            request.Headers.Add("Access-Control-Request-Headers", "authorization,content-type,csrftoken,x-ms-app,x-ms-client-version,x-ms-user");
            request.Headers.Add("Access-Control-Request-Method", "POST");
            request.Headers.Add("DNT", "1");
            request.Headers.Add("Origin", "https://jarvis-west.dc.ad.msft.net");
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36";
            request.GetResponse();
        }
        public static AzureResponse PostRequest(AzurePayload payload)
        {
            HttpWebRequest request = CreatePOSTRequest(AZURE_DASHBOARD_URL);
            //HEADER INFO
            request.PreAuthenticate = true;
            request.Headers.Add("csrfToken", Properties.Get("CSRFToken"));
            request.Headers.Add("Authorization", GetBearerAuthToken());
            request.Headers.Add("Origin", "https://jarvis-west.dc.ad.msft.net");
            request.Headers.Add("DNT", "1");
            request.Headers.Add("x-ms-app", "Jarvis.Dashboard");
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36";
            request.Referer = "https://jarvis-west.dc.ad.msft.net/app/canvas/w/mdm/MDMProfilePage?wires={%22account%22:%22%22,%22namespace%22:%22%22,%22profile%22:%22%22,%22selected%22:%22$$$%22,%22globalStartTime%22:-3600000,%22globalEndTime%22:-1,%22pinGlobalTimeRange%22:false,%22syncGlobalTimeRange%22:false,%22presentationMode%22:false,%22refreshTick%22:1542213399097,%22refreshInterval%22:0,%22pendingSave%22:null,%22pendingWidgets%22:[],%22pendingShare%22:false,%22pendingExport%22:null,%22pendingImport%22:false,%22overrides%22:[{%22query%22:%22//kusto%22,%22key%22:%22connection%22,%22replacement%22:%22https://sqlazureuk2.kustomfa.windows.net:443%22},{%22query%22:%22//conditions[id=%27AzureSqlServer%27]%22,%22key%22:%22value%22,%22replacement%22:%22g5ajmo36i2%22}],%22dashboardBrowserTab%22:0,%22dashboardBrowserVisible%22:true,%22selectedAccounts%22:%22SPOProd,SPODogFood,SPOMSIT,sporunners%22,%22dashboardBrowserTreeFilter%22:%22%22,%22selectedPath%22:[%22SPOProd%22,%22OCE%22,%22Azure%20Dashboard%22],%22dashboardId%22:%22%22,%22v%22:1542136435389}";

            byte[] content = ConvertPayloadToContentByteArray(payload);
            request.ContentLength = content.Length;
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(content, 0, content.Length);
                dataStream.Close();
            }

            OptionsRequest();

            // Get response
            AzureResponse jsonResponse = GetRequestResponse(request);
            return jsonResponse;
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

        private static string BearerAuthToken = null;

        public static string GetBearerAuthToken()
        {
            if (BearerAuthToken == null)
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
                BearerAuthToken = "Bearer " + result.Groups[1];
                Console.WriteLine(BearerAuthToken);
            }
            return BearerAuthToken;
        }
    }
}
