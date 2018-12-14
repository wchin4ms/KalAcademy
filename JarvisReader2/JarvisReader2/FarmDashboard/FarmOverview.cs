using JarvisReader.AzureDashboard;
using JarvisReader.DateUtils;
using System;
using System.Collections.Generic;
using System.Text;

namespace JarvisReader.FarmDashboard
{
    class FarmOverview : IOverview
    {
        public string FarmLabel { get; }
        public DateTime StartTime { get; }
        public DateTime EndTime { get; }
        public ProbeOverview Probe { get; }
        public SQLPerfOverview SQL { get; }
        public USRandRPSPerfOverview USR { get; }
        public Dictionary<string, AzureOverview> AzureDashboards = null;
        public FarmOverview(string farmLabel)
        {
            FarmLabel = farmLabel;
            EndTime = DateTime.Now;
            StartTime = EndTime.AddHours(-1); // grab 1 hours worth
            // milliseconds from epoch
            long endTime = (long) DateTime.Now.Subtract(DateTimeUtils.EPOCH_1970).TotalMilliseconds;
            long startTime = endTime - (1000 * 60 * 60);  // grab 1 hours worth

            Probe = ProbeOverviewRequest.Get(FarmLabel, startTime, endTime);
            SQL = SQLPerfOverviewRequest.Get(FarmLabel, startTime, endTime);
            USR = USRandRPSPerfOverviewRequest.Get(FarmLabel, startTime, endTime);
        }

        public FarmOverview (string farmLabel, DateTime startTime, DateTime endTime)
        {
            FarmLabel = farmLabel;
            StartTime = startTime;
            EndTime = endTime;
            // convert to milliseconds from epoch
            long startMillisFromEpoch = (long) StartTime.Subtract(DateTimeUtils.EPOCH_1970).TotalMilliseconds;
            long endMillisFromEpoch = (long) EndTime.Subtract(DateTimeUtils.EPOCH_1970).TotalMilliseconds;

            Probe = ProbeOverviewRequest.Get(FarmLabel, startMillisFromEpoch, endMillisFromEpoch);
            SQL = SQLPerfOverviewRequest.Get(FarmLabel, startMillisFromEpoch, endMillisFromEpoch);
            USR = USRandRPSPerfOverviewRequest.Get(FarmLabel, startMillisFromEpoch, endMillisFromEpoch);
        }

        public void GetAzureOverview (string AzureLabel)
        {
            long startMillisFromEpoch = (long)StartTime.Subtract(DateTimeUtils.EPOCH_1970).TotalMilliseconds;
            long endMillisFromEpoch = (long)EndTime.Subtract(DateTimeUtils.EPOCH_1970).TotalMilliseconds;
            AzureDashboardRequest.Get(AzureLabel, StartTime, EndTime);
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
