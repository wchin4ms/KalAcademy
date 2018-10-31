using Newtonsoft.Json;
using System.Collections.Generic;

namespace JarvisReader
{
    class JsonResults
    {
        [JsonProperty("$type")]
        public string Type { get; set; }

        [JsonProperty("$values")]
        public List<EvaluatedResult> Values { get; set; }
    }
}
