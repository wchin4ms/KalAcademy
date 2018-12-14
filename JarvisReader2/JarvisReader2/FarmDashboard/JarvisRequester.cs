using JarvisReader.Authentication;
using System;
using System.IO;
using System.Net;

namespace JarvisReader.FarmDashboard
{
    class JarvisRequester : Requester<JarvisResponse>
    {
        private const string JARVIS_URL = "https://jarvis-west.dc.ad.msft.net";
        private static CookieContainer cookies = null;
        private static CookieContainer GetCookies()
        {
            if (cookies == null)
            {
                cookies = new CookieContainer();
                Uri jarvisURI = new Uri(JARVIS_URL);
                cookies.Add(jarvisURI, new Cookie("FedAuth", Properties.Get("JAuth")));
                cookies.Add(jarvisURI, new Cookie("FedAuth1", Properties.Get("JAuth1")));
                cookies.Add(jarvisURI, new Cookie("FedAuth2", Properties.Get("JAuth2")));
                cookies.Add(jarvisURI, new Cookie("FedAuth3", Properties.Get("JAuth3")));
                cookies.Add(jarvisURI, new Cookie("FedAuth4", Properties.Get("JAuth4")));
            }
            return cookies;
        }
        public static JarvisResponse PostRequest(string url, FarmPayload payload)
        {
            HttpWebRequest request = CreatePOSTRequest(url);
            //HEADER INFO
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Headers.Add("accept-encoding", "gzip, deflate, br");
            request.Headers.Add("accept-language", "en-US,en;q=0.9");
            request.CookieContainer = GetCookies();
            request.Headers.Add("csrftoken", Properties.Get("CSRFToken"));
            request.Headers.Add("jarvis.overridetimeout", "601000");

            byte[] toBytes = ConvertPayloadToContentByteArray(payload);
            request.ContentLength = toBytes.Length;
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(toBytes, 0, toBytes.Length);
                dataStream.Close();
            }

            // Get response
            JarvisResponse jsonResponse = GetRequestResponse(request);
            return jsonResponse;
        }
    }
}
