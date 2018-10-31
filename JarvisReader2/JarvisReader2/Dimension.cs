using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisReader
{
    class Dimension
    {
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
