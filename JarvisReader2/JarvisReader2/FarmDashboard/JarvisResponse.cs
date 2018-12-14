using Newtonsoft.Json;
using System.Collections.Generic;

namespace JarvisReader.FarmDashboard
{
    class JarvisResponse
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
    class JsonResults
    {
        [JsonProperty("$type")]
        public string Type { get; set; }

        [JsonProperty("$values")]
        public List<EvaluatedResult> Values { get; set; }
    }
    class EvaluatedResult
    {
        [JsonProperty("dimensionList")]
        public DimensionList DimensionList { get; set; }
        [JsonProperty("evaluatedResult")]
        public decimal AverageScore { get; set; }
        [JsonProperty("seriesValues")]
        public List<decimal?> Scores { get; set; }
    }
    class DimensionList
    {
        [JsonProperty("$type")]
        public string Type { get; set; }

        [JsonProperty("$values")]
        public List<Dimension> Values { get; set; }
    }
    class Dimension
    {
        public const string CONTENT_DATABASE = "ContentDatabase";
        public const string FARM_LABEL = "__FarmLabel";
        public const string FARM_TYPE = "__FarmType";
        public const string INSTANCE_NUM = "InstanceNum";
        public const string MACHINE = "__Machine";
        public const string RUNNER_NAME = "RunnerName";
        public const string SLOWEST_QUERY_DATABASE = "SlowestQueryDatabase";
        public const string SLOWEST_QUERY_SERVER = "SlowestQueryServer";
        [JsonProperty("key")]
        public string Key { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
