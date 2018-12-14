using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisReader.FailedProbes
{
    class FailedProbe
    {
        public string CorrelationId { get; set; }
        public string EndTimeUtc { get; set; }
        public string Error { get; set; }
        public string ExecutionId { get; set; }
        public string Farm { get; set; }
        public string FarmId { get; set; }
        public string HttpStatus { get; set; }
        public string InstanceNum { get; set; }
        public string Latency { get; set; }
        public string MachineName { get; set; }
        public string NetworkId { get; set; }
        public string StartTimeUtc { get; set; }
        public string Url { get; set; }
    }
}
