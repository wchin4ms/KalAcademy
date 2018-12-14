using System;
using System.Collections.Generic;
using System.Text;

namespace JarvisReader.AzureDashboard
{
    class AzureOverview : IOverview
    {
        public string AzureLabel { get; private set; }
        public DateTime StartTime { get; }
        public DateTime Endtime { get; }
        public List<DBNode> DBNodes { get; private set; }
        private Dictionary<string, SortedSet<AvgCPUPct>> AvgCpuPcts;
        private Dictionary<string, SortedSet<PeakWorkPct>> PeakWorkPcts;

        public AzureOverview (string label, DateTime startTime, DateTime endTime)
        {
            AzureLabel = label;
            DBNodes = new List<DBNode>();
            AvgCpuPcts = new Dictionary<string, SortedSet<AvgCPUPct>>();
            PeakWorkPcts = new Dictionary<string, SortedSet<PeakWorkPct>>();
        }
        public void AddAvgCpuPct(AvgCPUPct pct)
        {
            if (!AvgCpuPcts.ContainsKey(pct.NodeName))
            {
                AvgCpuPcts.Add(pct.NodeName, new SortedSet<AvgCPUPct>());
            }
            AvgCpuPcts[pct.NodeName].Add(pct);
        }
        public void AddPeakWorkerPct(PeakWorkPct pct)
        {
            if (!PeakWorkPcts.ContainsKey(pct.NodeName))
            {
                PeakWorkPcts.Add(pct.NodeName, new SortedSet<PeakWorkPct>());
            }
            PeakWorkPcts[pct.NodeName].Add(pct);
        }
        public void Evaluate()
        {
            throw new NotImplementedException();
        }

        public string InfoString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(" --- AZURE OVERVIEW --- ");
            stringBuilder.AppendLine("Label: " + AzureLabel);
            stringBuilder.AppendLine("DB Nodes --- ");
            foreach (DBNode dbnode in DBNodes)
            {
                stringBuilder.AppendLine("    DB Node: " + dbnode.DBName);
            }
            stringBuilder.AppendLine("Average CPU Percent --- ");
            foreach (KeyValuePair<string, SortedSet<AvgCPUPct>> entry in AvgCpuPcts)
            {
                stringBuilder.AppendLine("    DB Node " + entry.Key);
                foreach(AvgCPUPct cpuPct in entry.Value)
                {
                    stringBuilder.AppendLine("        " + cpuPct.Timestamp + " | " + cpuPct.CPUpercent);
                }
            }
            return stringBuilder.ToString();
        }
    }

    class DBNode
    {
        public string DBName { get; private set; }
        public string NodeName { get; private set; }
        public string ClusterName { get; private set; }
        public string AppName { get; private set; }
        public int DTULimit { get; private set; }
        public string LogicalDBguid { get; private set; }
        public DBNode(string dbName, string node, string cluster, string app, string dtuLimit, string logicalDBguid)
        {
            DBName = dbName;
            NodeName = node;
            ClusterName = cluster;
            AppName = app;
            DTULimit = Convert.ToInt32(dtuLimit);
            LogicalDBguid = logicalDBguid;
        }
    }

    class AvgCPUPct : IComparable<AvgCPUPct>
    {
        public DateTime Timestamp { get; private set; }
        public string NodeName { get; private set; }
        public string MachineName { get; private set; }
        public string AppName { get; private set; }
        public double CPUpercent { get; private set; }
        public AvgCPUPct(DateTime time, string node, string machine, string app, double cpuPct)
        {
            Timestamp = time; //DateTime.ParseExact(time, "yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture);
            NodeName = node;
            MachineName = machine;
            AppName = app;
            CPUpercent = cpuPct;
        }
        public int CompareTo(AvgCPUPct other)
        {
            return this.Timestamp.CompareTo(other.Timestamp);
        }
    }

    class PeakWorkPct : IComparable<PeakWorkPct>
    {
        public DateTime Timestamp { get; private set; }
        public string NodeName { get; private set; }
        public string MachineName { get; private set; }
        public string AppName { get; private set; }
        public double PeakPct { get; private set; }
        public PeakWorkPct(DateTime time, string node, string machine, string app, double peakWorkPct)
        {
            Timestamp = time; // DateTime.ParseExact(time, "yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture);
            NodeName = node;
            MachineName = machine;
            AppName = app;
            PeakPct = peakWorkPct;
        }

        public int CompareTo(PeakWorkPct other)
        {
            return this.Timestamp.CompareTo(other.Timestamp);
        }
    }
}
