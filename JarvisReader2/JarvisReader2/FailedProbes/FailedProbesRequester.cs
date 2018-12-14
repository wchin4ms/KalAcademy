using JarvisReader.Authentication;
using JarvisReader.DateUtils;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace JarvisReader.FailedProbes
{
    class FailedProbesRequester : Requester<SearchResponse>
    {
        private const string JARVIS_URL = "https://jarvis-west.dc.ad.msft.net";
        private const string SEARCH_URL = "https://jarvis-west.dc.ad.msft.net/user-api/v2/logs/startSearchV2";
        private static CookieContainer cookies = null;
        private static CookieContainer GetCookies()
        {
            if (cookies == null)
            {
                cookies = new CookieContainer();
                Uri jarvisURI = new Uri(JARVIS_URL);
                cookies.Add(jarvisURI, new Cookie("CloudESAuthCookie", "AAEAAIpLuVPBsOk5YcAz4PSCPrIR7RME72pRRzUljgBUFRhP9rI1g0g3sXjN0Y3klDT5YHKtawwQ0q-YxIFgVuxpZx3oS1K_i06zTko94sB8r11lJy5EKdB536aNoflHU_njfoljHrQ2E9YEU6JrIwgyWcPhobybkOoNWXfVPe-H0D32QEIYXd75KA3T-EztCPFy7smWUhFAV2vhEAnBv0_ela2BjxgROV7LgwHRHlXCGI-r2x-uGECSy4P4YKF8VSo497r6PlicK5IGp5z9AmHgwkayp7EvP5_GJRpKNWkbe9Tz_akb0xDlL6lfKBl3exw--0RV8Ja6cuMAmo9g0YTFl4mBdLJ1KxToTolBugJKo2ddsPka1_xOJzevfDn3LqPdWAABAAAoesp36uvr2nHRxpRQIinw5eQbVhBAtAa_wsU4a7W3YjVguR9NVLcvPWpKtpN_0mx7nTMfn41xips0rNa6Js98Nq--fnDCcP8lExWvKwuzSUL-9710iiMNmvSm8DiKguuhhv-mBrkkkqwmbSsGHUVaJX2fK-CYeEw_DphfhmbIeFJqRPmKOcCHQSDOL4qZ7wswgrNdFus7UiM0YZxhOSFER6pklDeK0OEgriLIzAQW7IzFsBAHJia49ea20zzp31w7LUpOcsJ1H_JpicNNawXAf8W7QSNPiWBNP7XKXTXZ4_eIi1i5MfXBOTDDdatiCJ9Mru0eJTpsLOIOEEMKXl0gIAQAAMVW9n02MhhaIkokvil5q3cn4w11dOJUVYQ8xd-8kmaJVc_bF08aIBjCwwsx4V9wR6h1BoMRprGUFrpc9eIdrd6Ue933RY0hMX55q2L1wzzMOdAuCjYdqYLSC9d6d8CPabbDGlT8WYcWwEhdSntyWq6uKBOiRgtQuKd-o73BT92GTsh5K7Z0fE1w9BUC6f0Xa7rqQTP5WVz3K_uP-0gJSHHqEc3-K2jK-p-ErSIWsL72gsCJ86I3S0LQW32ikWJZG1CM4IjxdXjC7CHrfOswH-qzzTaUhkTRXaKjd_GsdYJ9ENlF2dnZnDiDAl8wl8EFlOw1EkSONFvXbdrn_3ZTeNdbsKURjiVjx0weE0cTM6YQgbn0lGxSPq1Nb1V9NftMq9wv_JVxynjPFWBnv0HO1VXpcMpR8WeHzR6lLcf0dJ-DuwJV3eJJ42AYNJUECfTTB66vHvaHrg0v3eIXmgmfr4EDizkP0wpkaCvJ2gqfoNmIq9RT1GXC-THxM1xl94_kxNxmBjPe4OHol6lNmT-CZUDImR5U6m8bMb6Clk0E99C836C4IRolSZg1dY1H55Uv-S_3qAPzLtwCEoBMIX_ngytR9N6KXtjAE_ChNXu09zB4CRL-fEfLymL2cZ4WY9BDOTB1xNtJrGVbTYhjC8WMybhTtgNEhPHTAkT5R2n9_nPKx_iNfZSWbOvkO_l0Xmdqp1AYBGTotEYm03MJhUMZzzfm29R2lpOjEuqhv07NVMcGB8NeVRvc8wAvCd3Zp4hmdVYCV3mTOx1IUMDBpS7M_BIE8VmjSwTmmd9Rg_9CCIvvEfIuy5nVJwHeN4DpN428e3VQk_Q18NRhZ6tY6uAkjf3Z66g2ZR9oFBIUCNB8pBSqMzQRF7rZmj5tcB52KJNI7OduwKsbXJBe5oOMuk9X3gLoKGFQYFmvdNaQquFvXgAsVKLslKj-IxEhh3ivOgH9hFsrOoNsQ66nyZKvl5HaP25bGzxA6C9V7NFCirsDDzoEgpaN73Bi7TMx9q1-Gqh2OlkFXrW-WJjYSwzpuW5ESmd4oT0HluptA3OPbo4QcSFng4xlL7KVf1SvH1sHD7qeJgsFAWMA35HIEzaaaE4yedg3denJAJlp_h3cC_vKhS5uCPQ16or3aTDXjIjDBQanz-ct_o8bwL2tP7DOEmK-kN6ggwZaLFrQGa0ZWrJS_-uQGXS6FUyEOPO5cXBoZpW6J3sUbPBnFD-PtGkyiXO8oRtD8EyQQ48_JTFw_XkcdIZlRymkJuR1l0H53c2ntMh5w7zmHN1pVpzPtytT8nAUBSdYlczq1vgOr473oj5QLMivwOiwZoXlCei-v6KfQeyFWKZ3sPjbEMX8B8TY5e9oHda6iRbAIF5SzEfmIcz66nlNFD1tEifGaPQfsKcLecQN5w"));
                cookies.Add(jarvisURI, new Cookie("FedAuth", Properties.Get("FPAuth")));
                cookies.Add(jarvisURI, new Cookie("FedAuth1", Properties.Get("FPAuth1")));
                cookies.Add(jarvisURI, new Cookie("FedAuth2", Properties.Get("FPAuth2")));
                cookies.Add(jarvisURI, new Cookie("FedAuth3", Properties.Get("FPAuth3")));
                cookies.Add(jarvisURI, new Cookie("FedAuth4", Properties.Get("FPAuth4")));
                cookies.Add(jarvisURI, new Cookie("FedAuth5", Properties.Get("FPAuth5")));
                cookies.Add(jarvisURI, new Cookie("FedAuth6", Properties.Get("FPAuth6")));
            }
            return cookies;
        }

        public static void Search(SearchPayload payload)
        {
            Console.WriteLine("Start Searching...");
            HttpWebRequest tryThis = (HttpWebRequest)WebRequest.Create("https://jarvis-west.dc.ad.msft.net/user-api/v1/config/security/HPSTSUrl/securityLevel/TwoFA?_=" + DateTimeUtils.GetMillisFromEpoch());

            //HEADER INFO
            tryThis.Accept = "application/json, text/javascript, */*; q=0.01";
            tryThis.Headers.Add("accept-encoding", "gzip, deflate, br");
            tryThis.Method = "GET";
            tryThis.ContentType = "application/json";
            tryThis.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
            tryThis.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            tryThis.Headers.Add("accept-encoding", "gzip, deflate, br");
            tryThis.Headers.Add("accept-language", "en-US,en;q=0.9");
            tryThis.Headers.Add("clientid", "Jarvis");
            tryThis.Headers.Add("origin", JARVIS_URL);
            tryThis.Headers.Add("csrftoken", Properties.Get("CSRFToken"));
            tryThis.CookieContainer = GetCookies();
            Console.WriteLine("ASDF: " + GetStringResponse(tryThis));

            HttpWebRequest request = CreatePOSTRequest("https://jarvis-west.dc.ad.msft.net/user-api/v2/logs/startSearchV2");
            //HEADER INFO
            request.Accept = "application/json, text/javascript, */*; q=0.01";
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
            request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.9");
            request.Headers.Add("clientid", "Jarvis");
            request.CookieContainer = GetCookies();
            request.Headers.Add("csrftoken", "hjk8y6+PqcIxMQRjuroq8Z2DNxwQCt1X31SoOCTME53zEzrC6nI2xjWth74HGGg9jOXP/uYAjFX8AqXZimlqvPv30LrgksPEO0K0XSPELpNGcDdx6dKG5YWADPYAEyOo2XTVC+/lcYfsa5bP+iyUOy3zNVtouO4q80oypyM3tXuaYxl5EUazv2ux8pUTjsbMe2JD4+8DUtYLjHGDqlftP72CCrL+iayIJVReZGWi+q6h/vYwMBIGW8T8Vd9iVrLgboSfvpmNczXDC1X0fapN6QYuUK8J1yNGQrEgwYqzK6rns1ie4lj9yVTUpRSABOxI+e0Kh7K+nkTp43SiYtAuEg==");
            request.Headers.Add("dnt", "1");
            request.Headers.Add("origin", JARVIS_URL);
            request.Referer = "https://jarvis-west.dc.ad.msft.net/logs/dgrep?page=logs&be=DGrep&offset=-15&offsetUnit=Minutes&UTC=false&ep=FirstParty%20PROD&ns=RunnerService&en=RunnerCentralEventTable&conditions=[[%22Status%22,%22%3D%3D%22,%22Unhealthy%22],[%22Role%22,%22%3D%3D%22,%22RunnerContainer%22],[%22InstanceName%22,%22contains%22,%22/Us_11_Content/Primary/%22],[%22Name%22,%22%3D%3D%22,%22ngspouploaddoc%22]]&clientQuery=where%20Status!%3D%22Dead%22%20%26%26%20UserField2!%3D%22%22%20let%20Error%3DUserField2.Substring(0,%20UserField2.IndexOf(%22%7C%22))%20let%20MachineName%3DRegex.Match(UserField2,%22%5C]MachineInfo::(.*?)%5C%7C%22).Groups[1].Value%20let%20CorrelationId%3DRegex.Match(UserField2,%22%5C]CorrelationId::(.*?)%5C%7C%22).Groups[1].Value%20let%20StartTimeUtc%3DRegex.Match(UserField2,%22%5C]ExecutionStartTime::(.*?)%5C%7C%22).Groups[1].Value%20let%20EndTimeUtc%3DRegex.Match(UserField2,%22%5C]ExecutionEndTime::(.*?)%5C%7C%22).Groups[1].Value%20let%20Farm%3DRegex.Match(InstanceName,%22.*?/.*?/(.*?)/[12]%22).Groups[1].Value.Trim()%20let%20Latency%3DInt32.Parse(Regex.Match(UserField2,%22%5C]AlertingLatency::(.*?)%5C%7C%22).Groups[1].Value)%20let%20InstanceNum%3DInstanceName.Split(%27/%27).Last()%20let%20HttpStatus%3DRegex.Match(UserField2,%22%5C]HttpStatus::(.*?)%5C%7C%22).Groups[1].Value%20let%20NetworkId%3DRegex.Match(UserField2,%22%5C]NetworkId::(.*?)%5C%7C%22).Groups[1].Value%20let%20FarmId%3DRegex.Match(UserField2,%22%5C]FarmId::(.*?)%5C%7C%22).Groups[1].Value%20let%20Url%3DRegex.Match(UserField2,%22%5C]Url::(.*?)%5C%7C%22).Groups[1].Value%20let%20ExecutionId%3DRegex.Match(UserField2,%22%5C]RowKey::(.*?)%5C%7C%22).Groups[1].Value%20select%20InstanceNum,Farm,Error,StartTimeUtc,EndTimeUtc,MachineName,CorrelationId,Latency,HttpStatus,NetworkId,FarmId,Url,ExecutionId&chartEditorVisible=true&chartType=Line&chartLayers=[[%22New%20Layer%22,%22%22]]%20";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.80 Safari/537.36";

            byte[] content = ConvertPayloadToContentByteArray(payload);
            request.ContentLength = content.Length;
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(content, 0, content.Length);
                dataStream.Close();
            }

            // Get response; don't care about return since this just initiates the search
            Console.WriteLine("ASDF2: " + GetStringResponse(request));
        }

        public static SearchResponse Ping (string sid)
        {
            Console.WriteLine("pinging...");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BuildPingURL(sid));
            request.Accept = "application/json, text/javascript, */*; q=0.01";
            request.Method = "GET";
            request.ContentType = "application/json";
            //HEADER INFO
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Headers.Add("accept-encoding", "gzip, deflate, br");
            request.Headers.Add("accept-language", "en-US,en;q=0.9");
            request.CookieContainer = GetCookies();
            request.Headers.Add("csrftoken", "CnU6FsprA+I4o4OFph8T8zEuVWLh6/P2/I8diBiP6LyXwgT6s0hN9w73W4vsE3V8SA0DDCOn6lm/jW6zeYgrgYGjn+RPaw3/HU0w771Zm+l2ZWKxPcLKjQ4736avQ8MHuai5x8HJzvSZIwDJFlS/nkDsia62sEOuGvY8UDYg7Ub05ZtaFKwt6sxv6VGhAjgXm/Uot4ZpyqVdrzgoi7CHqp953P8XRt8gh4m2rR3bif8zw2GRXWvWO+IBwEhW73ay7gJpMpxHAtpoY3eAhYauTcNJmADdDz5KL6ioHzDF7wFTrPc1HOPj1Q7CisvNrRKMI0x9MhT+LPSJdD0cy3OLPw==");

            SearchResponse response = GetRequestResponse(request);
            return response;
        }

        public static SearchResponse GetFailedProbes(string sid)
        {
            Console.WriteLine("getting results...");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BuildSearchURL(sid));
            request.Accept = "application/json, text/javascript, */*; q=0.01";
            request.Method = "POST";
            request.ContentType = "application/json";
            //HEADER INFO

            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Headers.Add("accept-encoding", "gzip, deflate, br");
            request.Headers.Add("accept-language", "en-US,en;q=0.9");
            request.CookieContainer = GetCookies();
            request.Headers.Add("csrftoken", Properties.Get("csrftoken"));

            byte[] content = Encoding.ASCII.GetBytes("where Status!=\"Dead\" && UserField2!=\"\" let Error=UserField2.Substring(0, UserField2.IndexOf(\"|\")) let MachineName=Regex.Match(UserField2,\"\\]MachineInfo::(.*?)\\|\").Groups[1].Value let CorrelationId=Regex.Match(UserField2,\"\\]CorrelationId::(.*?)\\|\").Groups[1].Value let StartTimeUtc=Regex.Match(UserField2,\"\\]ExecutionStartTime::(.*?)\\|\").Groups[1].Value let EndTimeUtc=Regex.Match(UserField2,\"\\]ExecutionEndTime::(.*?)\\|\").Groups[1].Value let Farm=Regex.Match(InstanceName,\".*?/.*?/(.*?)/[12]\").Groups[1].Value.Trim() let Latency=Int32.Parse(Regex.Match(UserField2,\"\\]AlertingLatency::(.*?)\\|\").Groups[1].Value) let InstanceNum=InstanceName.Split('/').Last() let HttpStatus=Regex.Match(UserField2,\"\\]HttpStatus::(.*?)\\|\").Groups[1].Value let NetworkId=Regex.Match(UserField2,\"\\]NetworkId::(.*?)\\|\").Groups[1].Value let FarmId=Regex.Match(UserField2,\"\\]FarmId::(.*?)\\|\").Groups[1].Value let Url=Regex.Match(UserField2,\"\\]Url::(.*?)\\|\").Groups[1].Value let ExecutionId=Regex.Match(UserField2,\"\\]RowKey::(.*?)\\|\").Groups[1].Value select InstanceNum,Farm,Error,StartTimeUtc,EndTimeUtc,MachineName,CorrelationId,Latency,HttpStatus,NetworkId,FarmId,Url,ExecutionId");
            request.ContentLength = content.Length;
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(content, 0, content.Length);
                dataStream.Close();
            }

            SearchResponse response = GetRequestResponse(request);
            return response;
        }

        private static string BuildPingURL(string sid)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("https://jarvis-west.dc.ad.msft.net/user-api/v2/logs/searchstatus/id/");
            stringBuilder.Append(sid);
            stringBuilder.Append("/endpoint/https%253A%252F%252Ffirstparty%252Emonitoring%252Ewindows%252Enet%252F?_=");
            stringBuilder.Append(DateTimeUtils.GetMillisFromEpoch().ToString());
            return stringBuilder.ToString();
        }

        private static string BuildSearchURL (string sid)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("https://jarvis-west.dc.ad.msft.net/user-api/v2/logs/results/id/");
            stringBuilder.Append(sid);
            stringBuilder.Append("/endpoint/https%253A%252F%252Ffirstparty%252Emonitoring%252Ewindows%252Enet%252F?startIndex=0&endIndex=20&querytype=MQL");
            return stringBuilder.ToString();
        }
    }
}
