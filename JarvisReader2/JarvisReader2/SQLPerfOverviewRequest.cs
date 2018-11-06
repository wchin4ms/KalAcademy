using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisReader
{
    class SQLPerfOverviewRequest : OverviewRequest<SQLPerfOverview>
    {
        public static SQLPerfOverview Get(string farmLabel, long startTime, long endTime)
        {
            SQLPerfOverview overview = new SQLPerfOverview();

            // SQL Processor Utilization Payload
            RequestPayload requestPayload = new RequestPayload()
            {
                Instance = new PayloadItem() { Item1 = false, Item2 = new string[0] },
                DataCenter = new PayloadItem() { Item1 = false, Item2 = new string[0] },
                Environment = new PayloadItem() { Item1 = false, Item2 = new string[0] },
                FarmId = new PayloadItem() { Item1 = false, Item2 = new string[0] },
                FarmLabel = new PayloadItem() { Item1 = false, Item2 = new string[1] { farmLabel } },
                FarmType = new PayloadItem() { Item1 = false, Item2 = new string[1] { "Primary" } },
                Machine = new PayloadItem() { Item1 = false, Item2 = new string[0] },
                Network = new PayloadItem() { Item1 = false, Item2 = new string[0] },
                Role = new PayloadItem() { Item1 = false, Item2 = new string[1] { "SQL" } },
            };
            // SQL Processor Utilization URL
            string sqlProcUtilURL = BuildURL("PerfCounters", "%255CProcessor(*)%255C%2525%2520Processor%2520Time", "Max", startTime, endTime, 40, "Automatic", false);

            // Make Post
            JsonResponse response = JarvisRequester.PostRequest(sqlProcUtilURL, requestPayload);
            startTime = response.StartTimeUtc;
            endTime = response.EndTimeUtc;
            foreach (EvaluatedResult eval in response.Results.Values)
            {
                List<Dimension> dimensions = eval.DimensionList.Values;
                string machine = dimensions.Where(dim => dim.Key.Equals(Dimension.MACHINE)).Single().Value;
                SeriesValues seriesValues = new SeriesValues()
                {
                    StartTimeMillisUtc = startTime,
                    EndTimeMillisUtc = endTime,
                    TimeResolutionInMillis = response.TimeResolutionInMilliseconds,
                    Values = eval.Scores.ToArray()
                };
                overview.SetProcessorUtilization(machine, seriesValues);
            }

            // SQL Thread Utilization Payload
            requestPayload.Instance.Item2 = new string[1] { "sqlservr" };

            // SQL Thread Utilization URL
            string sqlThreadUtilURL = BuildURL("PerfCounters", "%255CProcess(*)%255CThread%2520Count", "NullableAverage", startTime, endTime, 40, "Automatic", false);

            // Make Post
            response = JarvisRequester.PostRequest(sqlThreadUtilURL, requestPayload);
            foreach (EvaluatedResult eval in response.Results.Values)
            {
                List<Dimension> dimensions = eval.DimensionList.Values;
                string machine = dimensions.Where(dim => dim.Key.Equals(Dimension.MACHINE)).Single().Value;
                SeriesValues seriesValues = new SeriesValues()
                {
                    StartTimeMillisUtc = startTime,
                    EndTimeMillisUtc = endTime,
                    TimeResolutionInMillis = response.TimeResolutionInMilliseconds,
                    Values = eval.Scores.ToArray()
                };
                overview.SetThreadUtilization(machine, seriesValues);
            }

            // Slowest SQL Connection Time Payload
            requestPayload.Instance = null;
            requestPayload.Machine = null;
            requestPayload.Role.Item2[0] = "USR";
            requestPayload.ContentDatabase = new PayloadItem() { Item1 = true, Item2 = new string[1] { "<null>" } };
            requestPayload.IsContentAppPool = new PayloadItem() { Item1 = false, Item2 = new string[1] { "true" } };

            // Slowest SQL Connection Time URL
            string slowestSQLConnectionTimeURL = BuildURL("RequestUsage", "TotalSqlConnectionDuration", "Sum", startTime, endTime, 40, "None", true);

            // Make Post
            response = JarvisRequester.PostRequest(slowestSQLConnectionTimeURL, requestPayload);
            foreach (EvaluatedResult eval in response.Results.Values)
            {
                List<Dimension> dimensions = eval.DimensionList.Values;
                string database = dimensions.Where(dim => dim.Key.Equals(Dimension.CONTENT_DATABASE)).Single().Value;
                SeriesValues seriesValues = new SeriesValues()
                {
                    StartTimeMillisUtc = startTime,
                    EndTimeMillisUtc = endTime,
                    TimeResolutionInMillis = response.TimeResolutionInMilliseconds,
                    Values = eval.Scores.ToArray()
                };
                overview.SetSlowestSQLConnectionTime(database, seriesValues);
            }

            // Slowest SQL Server Query Durations Payload
            requestPayload.Role = null;
            requestPayload.ContentDatabase = null;
            requestPayload.IsContentAppPool = null;
            requestPayload.SlowestQueryServer = new PayloadItem() { Item1 = false, Item2 = new string[0] };

            // Slowest SQL Server Query Durations URL
            string slowestSQLQueryURL = BuildURL("RequestUsage", "TotalSlowQueryDuration", "Sum", startTime, endTime, 40, "None", true);

            // Make Post
            response = JarvisRequester.PostRequest(slowestSQLQueryURL, requestPayload);
            foreach (EvaluatedResult eval in response.Results.Values)
            {
                List<Dimension> dimensions = eval.DimensionList.Values;
                string server = dimensions.Where(dim => dim.Key.Equals(Dimension.SLOWEST_QUERY_SERVER)).Single().Value;
                SeriesValues seriesValues = new SeriesValues()
                {
                    StartTimeMillisUtc = startTime,
                    EndTimeMillisUtc = endTime,
                    TimeResolutionInMillis = response.TimeResolutionInMilliseconds,
                    Values = eval.Scores.ToArray()
                };
                overview.SetSlowestSQLDuration(server, seriesValues);
            }

            // Slowest SQL DB Query Durations Payload
            requestPayload.SlowestQueryDB = new PayloadItem() { Item1 = false, Item2 = new string[0] };

            // Slowest SQL Server Query Durations URL
            string slowestDBQueryURL = BuildURL("RequestUsage", "TotalSlowQueryDuration", "Sum", startTime, endTime, 200, "None", true);

            // Make Post
            response = JarvisRequester.PostRequest(slowestDBQueryURL, requestPayload);
            foreach (EvaluatedResult eval in response.Results.Values)
            {
                List<Dimension> dimensions = eval.DimensionList.Values;
                string server = dimensions.Where(dim => dim.Key.Equals(Dimension.SLOWEST_QUERY_DATABASE)).Single().Value;
                string database = dimensions.Where(dim => dim.Key.Equals(Dimension.SLOWEST_QUERY_SERVER)).Single().Value;
                SeriesValues seriesValues = new SeriesValues()
                {
                    StartTimeMillisUtc = startTime,
                    EndTimeMillisUtc = endTime,
                    TimeResolutionInMillis = response.TimeResolutionInMilliseconds,
                    Values = eval.Scores.ToArray()
                };
                overview.SetSlowestDBDuration(database, server, seriesValues);
            }

            return overview;
        }

        private static string BuildURL(string metricNamespace, string metric, string samplingType, long startTime, long endTime, 
            int top, string seriesAggregationType, bool zeroAsNoValueSentinel)
        {
            Dictionary<QueryParam, string> queryParams = new Dictionary<QueryParam, string>
            {
                { QueryParam.EndTime, endTime.ToString() },
                { QueryParam.IncludeSeries, "true" },
                { QueryParam.Operand, "-86" },
                { QueryParam.Operator, "NotEqual" },
                { QueryParam.OrderBy, "Descending" },
                { QueryParam.Reducer, "Average" },
                { QueryParam.SamplingType, samplingType },
                { QueryParam.SelectionType, "TopValues" },
                { QueryParam.SeriesResolution, "60000" },
                { QueryParam.SeriesAggregationType, seriesAggregationType },
                { QueryParam.StartTime, startTime.ToString() },
                { QueryParam.Top, top.ToString() },
                { QueryParam.ZeroAsNoValueSentinel, zeroAsNoValueSentinel.ToString() }
            };
            return URLBuilder.BuildURL("SPOProd", metricNamespace, metric, queryParams);
        }
    }
}
