using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace JarvisReader
{
    enum QueryParam
    {
        EndTime,
        FarmLabel,
        FarmType,
        IncludeSeries,
        Operand,
        Operator,
        OrderBy,
        Reducer,
        SamplingType,
        SelectionType,
        SeriesAggregationType,
        SeriesResolution,
        StartTime,
        Top,
        Window,
        ZeroAsNoValueSentinel
    }
    class URLBuilder
    {
        private static StringBuilder stringBuilder = new StringBuilder();
        public static string BuildURL(string monitoringAccount, string metricNamespace, string metric, Dictionary<QueryParam, string> queryParams)
        {
            stringBuilder.Clear();
            stringBuilder.Append("https://jarvis-west.dc.ad.msft.net/passthrough/user-api/flight/dq/batchedReadv3/V2/monitoringAccount/");
            stringBuilder.Append(monitoringAccount);
            stringBuilder.Append("/metricNamespace/");
            stringBuilder.Append(metricNamespace);
            stringBuilder.Append("/metric/");
            stringBuilder.Append(metric);
            stringBuilder.Append("?");
            stringBuilder.Append(BuildQueryParamString(queryParams));
            return stringBuilder.ToString();
        }

        private static string BuildQueryParamString(Dictionary<QueryParam, string> queryParamPairs)
        {
            List<string> queryParams = new List<string>();
            foreach (KeyValuePair<QueryParam, string> entry in queryParamPairs)
            {
                if (entry.Value != null)
                {
                    queryParams.Add(entry.Key.ToString() + "=" + entry.Value);
                }
            }
            return string.Join("&", queryParams);
        }
    }
}
