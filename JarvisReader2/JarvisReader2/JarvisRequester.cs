﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Script.Serialization;

namespace JarvisReader
{
    class JarvisRequester
    {
        private const string JARVIS_URL = "https://jarvis-west.dc.ad.msft.net";
        private static readonly Properties PROPERTIES = new Properties("auth.properties");
        // Need to ignore metadata strings since response str has '$type' and '$value' as JSON keys
        // And don't serialize request json objects/fields if null
        private static readonly JsonSerializerSettings JSON_SERIALIZER_SETTINGS = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore
        };
        private static CookieContainer cookies = null;
        private static CookieContainer GetCookies()
        {
            if (cookies == null)
            {
                cookies = new CookieContainer();
                Uri jarvisURI = new Uri(JARVIS_URL);
                cookies.Add(jarvisURI, new Cookie("FedAuth", PROPERTIES.Get("FedAuth")));
                cookies.Add(jarvisURI, new Cookie("FedAuth1", PROPERTIES.Get("FedAuth1")));
                cookies.Add(jarvisURI, new Cookie("FedAuth2", PROPERTIES.Get("FedAuth2")));
                cookies.Add(jarvisURI, new Cookie("FedAuth3", PROPERTIES.Get("FedAuth3")));
                cookies.Add(jarvisURI, new Cookie("FedAuth4", PROPERTIES.Get("FedAuth4")));
            }
            return cookies;
        }
        public static JsonResponse PostRequest(string url, RequestPayload payload)
        {
            Console.WriteLine("URL: " + url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            //HEADER INFO
            request.Accept = "application/json, text/javascript, */*; q=0.01";
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Headers.Add("accept-encoding", "gzip, deflate, br");
            request.Headers.Add("accept-language", "en-US,en;q=0.9");
            request.Headers.Add("clientid", "Jarvis");
            request.ContentType = "application/json";
            request.CookieContainer = GetCookies();
            request.Headers.Add("csrftoken", PROPERTIES.Get("csrftoken"));
            request.Headers.Add("dnt", "1");
            request.Headers.Add("jarvis.overridetimeout", "601000");
            request.Headers.Add("origin", JARVIS_URL);
            request.Headers.Add("sourceidentity", "{\"user\":\"weschin\",\"time\":1538577606245,\"retry\":false,\"selectedPath\":[\"SPOMSIT\",\"OCE\",\"Farm%20Overview\"],\"selected\":\"%24%24%24\",\"v\":\"1538156178991\"}");
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100 Safari/537.36";
            request.Headers.Add("x-requested-with", "XMLHttpRequest");

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
            JsonResponse jsonResponse;
            using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
            {
                string responseStr = streamReader.ReadToEnd();
                Console.WriteLine("RESPONSE: " + responseStr);
                jsonResponse = JsonConvert.DeserializeObject<JsonResponse>(responseStr, JSON_SERIALIZER_SETTINGS);
                streamReader.Close();
                response.Close();
            }
            return jsonResponse;
        }
    }
}