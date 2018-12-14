using Newtonsoft.Json;
using System.Collections.Generic;

namespace JarvisReader.FailedProbes
{
    class SearchResponse
    {
        // INFO RESPONSE
        [JsonProperty("ColumnDefinitions")]
        public ColumnDefinitions ColumnDefs { get; set; }
        [JsonProperty("Columns")]
        public object SearchColumns { get; set; }
        [JsonProperty("ConstantValues")]
        public object ConstantValues { get; set; }
        [JsonProperty("Count")]
        public int Count;
        [JsonProperty("Rows")]
        public List<FailedProbe> Rows { get; set; }

        // PING RESPONSE
        [JsonProperty("EnumeratedBlobSize")]
        public long EnumeratedBlobSize { get; set; }
        [JsonProperty("FailedBlobSize")]
        public long FailedBlobSize { get; set; }
        [JsonProperty("ProcessedBlobSize")]
        public long ProcessedBlobSize { get; set; }
        [JsonProperty("ResultCount")]
        public int ResultCount { get; set; }
        [JsonProperty("ScheduledBlobSize")]
        public long ScheduledBlobSize { get; set; }
        [JsonProperty("Status")]
        public string Status { get; set; }
        [JsonProperty("ThrottlingReason")]
        public string ThrottlingReason { get; set; }
    }

    class ColumnDefinitions
    {
        public ColumnDefinition CorrelationId { get; set; }
        public ColumnDefinition EndTimeUtc { get; set; }
        public ColumnDefinition Error { get; set; }
        public ColumnDefinition ExecutionId { get; set; }
        public ColumnDefinition Farm { get; set; }
        public ColumnDefinition FarmId { get; set; }
        public ColumnDefinition HttpStatus { get; set; }
        public ColumnDefinition InstanceNum { get; set; }
        public ColumnDefinition Latency { get; set; }
        public ColumnDefinition MachineName { get; set; }
        public ColumnDefinition NetworkId { get; set; }
        public ColumnDefinition StartTimeUtc { get; set; }
        public ColumnDefinition Url { get; set; }
    }

    class ColumnDefinition
    {
        public string ColumnType { get; set; }
        public string ConstantValue { get; set; }
    }
}
