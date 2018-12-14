using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JarvisReader.AzureDashboard
{
    class AzureDashboardRequest
    {
        public static AzureOverview Get(string azureLabel, DateTime startTime, DateTime endTime)
        {
            AzureOverview overview = new AzureOverview(azureLabel, startTime, endTime);

            Console.WriteLine(" ---- NODE DB BREAKDOWN ---- ");
            // Node / DB Breakdown
            AzurePayload requestPayload = new AzurePayload()
            {
                DB = "sqlazure1",
                CSL = GetNodeDBBreakdownCSL(azureLabel)
            };
            AzureResponse response = AzureRequester.PostRequest(requestPayload);

            // find useful info
            AzureResponseTable azureTable = response.Tables.Find(table => table.Columns.Exists(column => column.ColumnName.Equals("database_name")));
            foreach (List<dynamic> row in azureTable.Rows)
            {
                DBNode breakdown = new DBNode((string) row[0], (string) row[1], (string) row[2], (string) row[3], (string) row[4], (string) row[5]);
                overview.DBNodes.Add(breakdown);
            }

            Console.WriteLine(" ---- Average CPU % Per Pool Node ---- ");
            // Average CPU % per Pool Node
            requestPayload.CSL = GetPerNodeCSL(azureLabel, startTime, endTime, "avg_cpu_percent");
            response = AzureRequester.PostRequest(requestPayload);

            // find useful info
            azureTable = response.Tables.Find(table => table.Columns.Exists(column => column.ColumnName.Equals("avg_cpu_percent")));
            foreach (List<dynamic> row in azureTable.Rows)
            {
                AvgCPUPct cpuPct = new AvgCPUPct((DateTime)row[0], (string)row[1], (string)row[2], (string)row[3], (double)row[4]);
                overview.AddAvgCpuPct(cpuPct);
            }

            Console.WriteLine(" ---- Peak Worker % per Pool Node ---- ");
            // Peak Worker % per Pool Node
            requestPayload.CSL = GetPerNodeCSL(azureLabel, startTime, endTime, "peak_worker_percent");
            response = AzureRequester.PostRequest(requestPayload);

            // find useful info
            azureTable = response.Tables.Find(table => table.Columns.Exists(column => column.ColumnName.Equals("peak_worker_percent")));
            foreach (List<dynamic> row in azureTable.Rows)
            {
                PeakWorkPct peakPct = new PeakWorkPct((DateTime)row[0], (string)row[1], (string)row[2], (string)row[3], (double)row[4]);
                overview.AddPeakWorkerPct(peakPct);
            }

            return overview;
        }

        private static string GetNodeDBBreakdownCSL(string azureLabel)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("set truncationmaxsize=1048576; MonResourceStats " +
                "| where server_name =~ ('" + azureLabel + "') ");
            stringBuilder.Append(
                "| summarize MAXTIMESTAMP = max(TIMESTAMP) by server_name, database_name " +
                "| join (MonResourceStats) on $left.MAXTIMESTAMP == $right.TIMESTAMP and $left.server_name == $right.server_name and $left.database_name == $right.database_name " +
                "| order by server_name, database_name " +
                "| project database_name, NodeName, ClusterName, AppName, dtu_limit, logical_database_guid ");
            return stringBuilder.ToString();
        }

        private static string GetPerNodeCSL(string azureLabel, DateTime startTime, DateTime endTime, string metric)
        {
            StringBuilder stringBuilder = new StringBuilder(); 
            stringBuilder.Append("set truncationmaxsize = 1048576; MonResourcePoolStats " +
                "| where server_name == ('" + azureLabel + "') ");
            stringBuilder.Append("and TIMESTAMP >= datetime(" + startTime.ToUniversalTime().ToString("yyyy - MM - dd HH:mm:ss") + ") ");
            stringBuilder.Append("and TIMESTAMP <= datetime(" + endTime.ToUniversalTime().ToString("yyyy - MM - dd HH:mm:ss") + ") ");
            stringBuilder.Append(
                "| project PreciseTimeStamp, NodeName , MachineName, AppName , " + metric);

            return stringBuilder.ToString();
        }
    }
}
