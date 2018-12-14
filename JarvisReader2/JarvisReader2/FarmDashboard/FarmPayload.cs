using Newtonsoft.Json;

namespace JarvisReader.FarmDashboard
{
    public class FarmPayload
    {
        [JsonProperty("Instance")]
        public PayloadItem Instance { get; set; }
        [JsonProperty("InstanceNum")]
        public PayloadItem InstanceNum { get; set; }
        [JsonProperty("RunnerName")]
        public PayloadItem RunnerName { get; set; }
        [JsonProperty("ContentDatabase")]
        public PayloadItem ContentDatabase { get; set; }
        [JsonProperty("IsContentAppPool")]
        public PayloadItem IsContentAppPool { get; set; }
        [JsonProperty("__DataCenter")]
        public PayloadItem DataCenter { get; set; }
        [JsonProperty("__Environment")]
        public PayloadItem Environment { get; set; }
        [JsonProperty("__FarmId")]
        public PayloadItem FarmId { get; set; }
        [JsonProperty("__FarmLabel")]
        public PayloadItem FarmLabel { get; set; }
        [JsonProperty("__FarmType")]
        public PayloadItem FarmType { get; set; }
        [JsonProperty("__Machine")]
        public PayloadItem Machine { get; set; }
        [JsonProperty("__Network")]
        public PayloadItem Network { get; set; }
        [JsonProperty("__Role")]
        public PayloadItem Role { get; set; }
        [JsonProperty("SlowestQueryDatabase")]
        public PayloadItem SlowestQueryDB { get; set; }
        [JsonProperty("SlowestQueryServer")]
        public PayloadItem SlowestQueryServer { get; set; }
    }
    public class PayloadItem
    {
        [JsonProperty("Item1")]
        public bool Item1 { get; set; }
        [JsonProperty("Item2")]
        public string[] Item2 { get; set; }
    }
}
