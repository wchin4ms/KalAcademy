﻿using System.Collections.Generic;
using System.Text;

namespace JarvisReader.FarmDashboard
{
    class SQLPerfOverview : IOverview
    {
        // Dictionary - SQL Machine Name : Series Values
        // ( SQL <optional 3 digit Network><FarmId>-<3 digit rounds up vm name>)
        // if SQL17212253-035 = machine name
        // then 172 is the network
        // 12253 is the farm id
        private Dictionary<string, SeriesValues> ProcessorUtilization = new Dictionary<string, SeriesValues>();
        private Dictionary<string, SeriesValues> ThreadUtilization = new Dictionary<string, SeriesValues>();
        private Dictionary<string, SeriesValues> SlowestSQLDurations = new Dictionary<string, SeriesValues>();
        private List<DBQueryInfo> SlowestDBQueryInfos = new List<DBQueryInfo>();
        private Dictionary<string, SeriesValues> SlowestSQLConnectionTime = new Dictionary<string, SeriesValues>();

        public void SetProcessorUtilization (string machine, SeriesValues vals)
        {
            ProcessorUtilization.Add(machine, vals);
        }
        public void SetThreadUtilization(string machine, SeriesValues vals)
        {
            ThreadUtilization.Add(machine, vals);
        }
        public void SetSlowestSQLDuration(string server, SeriesValues vals)
        {
            SlowestSQLDurations.Add(server, vals);
        }
        public void SetSlowestSQLConnectionTime(string database, SeriesValues vals)
        {
            SlowestSQLConnectionTime.Add(database, vals);
        }
        public void SetSlowestDBDuration(string database, string server, SeriesValues vals)
        {
            SlowestDBQueryInfos.Add(new DBQueryInfo()
            {
                Database = database,
                Server = server,
                Values = vals
            });
        }

        public void Evaluate()
        {

        }

        public string InfoString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("\nSQL Processor Utilization: ");
            foreach (KeyValuePair<string, SeriesValues> entry in ProcessorUtilization)
            {
                stringBuilder.Append("\n    ");
                stringBuilder.Append(entry.Key);
                stringBuilder.Append(": ");
                stringBuilder.Append(entry.Value.InfoString());
            }
            stringBuilder.Append("\nSQL Thread Utilization: ");
            foreach (KeyValuePair<string, SeriesValues> entry in ThreadUtilization)
            {
                stringBuilder.Append("\n    ");
                stringBuilder.Append(entry.Key);
                stringBuilder.Append(": ");
                stringBuilder.Append(entry.Value.InfoString());
            }
            stringBuilder.Append("\nSlowest SQL Server Durations: ");
            foreach (KeyValuePair<string, SeriesValues> entry in SlowestSQLDurations)
            {
                stringBuilder.Append("\n    ");
                stringBuilder.Append(entry.Key);
                stringBuilder.Append(": ");
                stringBuilder.Append(entry.Value.InfoString());
            }
            stringBuilder.Append("\nSlowest SQL DB Durations: ");
            foreach (DBQueryInfo info in SlowestDBQueryInfos)
            {
                stringBuilder.Append(info.InfoString());
            }
            return stringBuilder.ToString();
        }
    }
}
