using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace JarvisReader
{
    public abstract class Requester<T>
    {
        // Need to ignore metadata strings since response str has '$type' and '$value' as JSON keys
        // And don't serialize request json objects/fields if null
        protected static readonly JsonSerializerSettings JSON_SERIALIZER_SETTINGS = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore
        };

        protected static HttpWebRequest CreatePOSTRequest(string url)
        {
            Console.WriteLine("URL: " + url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Accept = "application/json, text/javascript, */*; q=0.01";
            request.Method = "POST";
            request.ContentType = "application/json";
            return request;
        }

        protected static byte[] ConvertPayloadToContentByteArray (object payload)
        {
            string jsonRequestPayload = JsonConvert.SerializeObject(payload, JSON_SERIALIZER_SETTINGS);
            Console.WriteLine("PAYLOAD: " + jsonRequestPayload);
            byte[] toBytes = Encoding.UTF8.GetBytes(jsonRequestPayload);

            return toBytes;
        }

        protected static string GetStringResponse(HttpWebRequest request)
        {
            string responseStr;
            // Get response
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Console.WriteLine("Status Code: " + response.StatusCode);
            using (Stream stream = response.GetResponseStream())
            {
                byte[] buf = new byte[8192];
                if ("chunked".Equals(response.Headers.Get("Transfer-Encoding")))
                {
                    Console.WriteLine("CHUNKED?!");
                    StringBuilder stringBuilder = new StringBuilder();
                    int count = 0;
                    do
                    {
                        count = stream.Read(buf, 0, buf.Length);
                        Console.WriteLine("Chunk Count: " + count);
                        if (count != 0)
                        {
                            stringBuilder.Append(Encoding.UTF8.GetString(buf, 0, count));
                        }
                    } while (count > 0);
                    responseStr = stringBuilder.ToString();
                }
                else
                {
                    StreamReader reader = new StreamReader(stream);
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
                Console.WriteLine("RESPONSE: " + responseStr);
                response.Close();
            }
            return responseStr;
        }

        protected static T GetRequestResponse(HttpWebRequest request)
        {
            T jsonResponse;

            string responseStr = GetStringResponse(request);
            jsonResponse = JsonConvert.DeserializeObject<T>(responseStr, JSON_SERIALIZER_SETTINGS);

            return jsonResponse;
        }

        // TODO: protected abstract CookieContainer GetCookies();
    }
}
