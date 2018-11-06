using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisReader
{
    class USRandRPSPerfOverviewRequest : OverviewRequest<USRandRPSPerfOverview>
    {
        public static USRandRPSPerfOverview Get(string farmLabel, long startTime, long endTime)
        {
            USRandRPSPerfOverview overview = new USRandRPSPerfOverview();

            // USR Processor - % Processor Time for CPU
            RequestPayload requestPayload = new RequestPayload()
            {
                Instance = new PayloadItem() { Item1 = false, Item2 = new string[1] { "_Total" } },
                DataCenter = new PayloadItem() { Item1 = false, Item2 = new string[0] },
                Environment = new PayloadItem() { Item1 = false, Item2 = new string[0] },
                FarmId = new PayloadItem() { Item1 = false, Item2 = new string[0] },
                FarmLabel = new PayloadItem() { Item1 = false, Item2 = new string[1] { farmLabel } },
                FarmType = new PayloadItem() { Item1 = false, Item2 = new string[1] { "Primary" } },
                Machine = new PayloadItem() { Item1 = false, Item2 = new string[0] },
                Network = new PayloadItem() { Item1 = false, Item2 = new string[0] },
                Role = new PayloadItem() { Item1 = false, Item2 = new string[1] { "USR" } },
            };
            // USR Processor - % Processor Time for CPU URL
            string processorCPUTimeURL = BuildURL("%255CProcessor(*)%255C%2525%2520Processor%2520Time", "NullableAverage", startTime, endTime);

            // Make Post
            JsonResponse response = JarvisRequester.PostRequest(processorCPUTimeURL, requestPayload);
            startTime = response.StartTimeUtc; // The jarvis site does some alignment w/ data (like gettin rid of milliseconds, etc.)
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
                overview.SetProcessorTimeCPU(machine, seriesValues);
            }

            // USR Processor - % Processor Time for Requests
            requestPayload.Instance.Item2 = new string[0];

            // USR Processor - % Processor Time for Requests
            string processorTimeRequestsURL = BuildURL("%255CASP%252ENET%255CRequests%2520Current", "Max", startTime, endTime);

            // Make Post
            response = JarvisRequester.PostRequest(processorTimeRequestsURL, requestPayload);
            startTime = response.StartTimeUtc; // The jarvis site does some alignment w/ data (like gettin rid of milliseconds, etc.)
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
                overview.SetProcessorTimeRequests(machine, seriesValues);
            }

            return overview;
        }

        private static string BuildURL(string metric, string samplingType, long startTime, long endTime)
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
                { QueryParam.SeriesAggregationType, "Automatic" },
                { QueryParam.StartTime, startTime.ToString() },
                { QueryParam.Top, "10" },
                { QueryParam.ZeroAsNoValueSentinel, "false" }
            };
            return URLBuilder.BuildURL("SPOProd", "PerfCounters", metric, queryParams);
        }
    }
}
