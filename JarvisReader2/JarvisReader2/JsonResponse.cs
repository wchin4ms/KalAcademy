using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisReader
{
    class JsonResponse
    {
        [JsonProperty("startTimeUtc")]
        public long StartTimeUtc { get; set; }
        [JsonProperty("endTimeUtc")]
        public long EndTimeUtc { get; set; }
        [JsonProperty("results")]
        public JsonResults Results { get; set; }
        [JsonProperty("timeResolutionInMilliseconds")]
        public int TimeResolutionInMilliseconds { get; set; }
    }
}
