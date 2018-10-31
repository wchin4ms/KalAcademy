using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisReader
{
    class DimensionList
    {
        [JsonProperty("$type")]
        public string type { get; set; }

        [JsonProperty("$values")]
        public List<Dimension> Values { get; set; }
    }
}
