using Newtonsoft.Json;

namespace JarvisReader.AzureDashboard
{
    class AzurePayload
    {
        [JsonProperty("csl")]
        public string CSL { get; set; }
        [JsonProperty("db")]
        public string DB { get; set; }
    }
}
