using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisReader
{
    class AzureRequestPayload
    {
        [JsonProperty("csl")]
        public string CSL { get; set; }
        [JsonProperty("db")]
        public string DB { get; set; }
    }
}
