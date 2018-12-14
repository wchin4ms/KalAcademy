using Newtonsoft.Json;
using System.Collections.Generic;

namespace JarvisReader.FailedProbes
{
    class SearchPayload
    {
        [JsonProperty("endTime")]
        public string EndTime { get; set; }
        [JsonProperty("endpoint")]
        public string Endpoint { get; set; }
        [JsonProperty("eventNames")]
        public List<string> EventNames { get; set; }
        [JsonProperty("identityColumns")]
        public Dictionary<string, string> IdentityColumns { get; set; }
        [JsonProperty("maxResults")]
        public int MaxResults { get; set; }
        [JsonProperty("namespaces")]
        public List<string> Namespaces { get; set; }
        [JsonProperty("query", NullValueHandling=NullValueHandling.Include)]
        public string Query { get; set; }
        [JsonProperty("queryID")]
        public string QueryID { get; set; }
        [JsonProperty("queryType")]
        public int QueryType { get; set; }
        [JsonProperty("searchCriteria")]
        public List<LogSearchCriteria> SearchCriteria { get; set; }
        [JsonProperty("startTime")]
        public string StartTime { get; set; }
    }
    public class LogSearchCriteria
    {
        [JsonProperty("key")]
        public string Key { get; set; }
        [JsonProperty("operation")]
        public int Operation { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
