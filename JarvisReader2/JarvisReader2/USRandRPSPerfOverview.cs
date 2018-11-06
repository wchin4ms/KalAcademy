using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisReader
{
    class USRandRPSPerfOverview : IOverview
    {
        private Dictionary<string, SeriesValues> ProcessorTimeCPU = new Dictionary<string, SeriesValues>();
        private Dictionary<string, SeriesValues> ProcessorTimeRequests = new Dictionary<string, SeriesValues>();
        public void SetProcessorTimeCPU(string machine, SeriesValues vals)
        {
            ProcessorTimeCPU.Add(machine, vals);
        }
        public void SetProcessorTimeRequests(string machine, SeriesValues vals)
        {
            ProcessorTimeRequests.Add(machine, vals);
        }
        public void Evaluate()
        {
            throw new NotImplementedException();
        }

        public string InfoString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("SQL USR Processor - % Processor Time (CPU): ");
            foreach (KeyValuePair<string, SeriesValues> entry in ProcessorTimeCPU)
            {
                stringBuilder.Append("\n    ");
                stringBuilder.Append(entry.Key);
                stringBuilder.Append(": ");
                stringBuilder.Append(entry.Value.InfoString());
            }
            stringBuilder.AppendLine("SQL USR Processor - % Processor Time (Requests): ");
            foreach (KeyValuePair<string, SeriesValues> entry in ProcessorTimeRequests)
            {
                stringBuilder.Append("\n    ");
                stringBuilder.Append(entry.Key);
                stringBuilder.Append(": ");
                stringBuilder.Append(entry.Value.InfoString());
            }
            return stringBuilder.ToString();
        }
    }
}
