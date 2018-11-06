using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisReader
{
    class FarmOverview : IOverview
    {
        public string FarmLabel { get; }
        public ProbeOverview Probe { get; }
        public SQLPerfOverview SQL { get; }
        public USRandRPSPerfOverview USR { get; }
        public FarmOverview(string farmLabel)
        {
            FarmLabel = farmLabel;
            // milliseconds from epoch
            long endTime = (long) DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            long startTime = endTime - (1000 * 60 * 60);  // grab 1 hours worth

            Probe = ProbeOverviewRequest.Get(FarmLabel, startTime, endTime);
            SQL = SQLPerfOverviewRequest.Get(FarmLabel, startTime, endTime);
            USR = USRandRPSPerfOverviewRequest.Get(FarmLabel, startTime, endTime);
        }

        public FarmOverview (string farmLabel, long startMillisFromEpoch, long endMillisFromEpoch)
        {
            FarmLabel = farmLabel;

            Probe = ProbeOverviewRequest.Get(FarmLabel, startMillisFromEpoch, endMillisFromEpoch);
            SQL = SQLPerfOverviewRequest.Get(FarmLabel, startMillisFromEpoch, endMillisFromEpoch);
            USR = USRandRPSPerfOverviewRequest.Get(FarmLabel, startMillisFromEpoch, endMillisFromEpoch);
        }
        public void Evaluate()
        {
            throw new NotImplementedException();
        }

        public string InfoString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("------------ Probe Overview ---------------");
            stringBuilder.AppendLine(Probe.InfoString());
            stringBuilder.AppendLine("------------ SQL Perf Overview ---------------");
            stringBuilder.AppendLine(SQL.InfoString());
            stringBuilder.AppendLine("------------ USR and RPS Perf Overview ---------------");
            stringBuilder.AppendLine(USR.InfoString());
            return stringBuilder.ToString();
        }
    }
}
