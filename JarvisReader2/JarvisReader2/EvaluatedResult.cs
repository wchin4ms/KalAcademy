using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace JarvisReader
{
    class EvaluatedResult
    {
        [JsonProperty("dimensionList")]
        public DimensionList DimensionList { get; set; }
        [JsonProperty("evaluatedResult")]
        public decimal AverageScore { get; set; }
        [JsonProperty("seriesValues")]
        public List<decimal?> Scores { get; set; }
    }
}
