﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace JarvisReader.FarmDashboard
{
    class ProbeOverviewRequest
    {
        public static ProbeOverview Get(string farmLabel, long startMillisFromEpoch, long endMillisFromEpoch)
        {
            ProbeOverview overview = new ProbeOverview();

            // Availability Payload
            FarmPayload requestPayload = new FarmPayload
            {
                InstanceNum = new PayloadItem { Item1 = false, Item2 = new string[0] },
                RunnerName = new PayloadItem { Item1 = false, Item2 = new string[] { "homepage", "uploaddoc", "teamsitehomepage" } },
                FarmLabel = new PayloadItem { Item1 = false, Item2 = new string[1] { farmLabel } },
                FarmType = new PayloadItem { Item1 = false, Item2 = new string[1] { "Primary" } }
            };
            // Availability URL
            string availabilityURL = BuildURL("Availability", "Success%20Rate", startMillisFromEpoch, endMillisFromEpoch);

            // Availability
            JarvisResponse response = JarvisRequester.PostRequest(availabilityURL, requestPayload);
            startMillisFromEpoch = response.StartTimeUtc;
            endMillisFromEpoch = response.EndTimeUtc;

            foreach (EvaluatedResult eval in response.Results.Values)
            {
                List<Dimension> dimensions = eval.DimensionList.Values;
                string runnerName = dimensions.Where(dim => dim.Key.Equals(Dimension.RUNNER_NAME)).Single().Value;
                int instanceNum = Convert.ToInt32(dimensions.Where(dim => dim.Key.Equals(Dimension.INSTANCE_NUM)).Single().Value);
                SeriesValues seriesValues = new SeriesValues()
                {
                    StartTimeMillisUtc = startMillisFromEpoch,
                    EndTimeMillisUtc = endMillisFromEpoch,
                    TimeResolutionInMillis = response.TimeResolutionInMilliseconds,
                    Values = eval.Scores.ToArray()
                };
                overview.SetPageAvailability(seriesValues, runnerName, instanceNum);
            }

            requestPayload.InstanceNum = null;
            requestPayload.Environment = new PayloadItem()
            {
                Item1 = false,
                Item2 = new string[0]
            };
            requestPayload.Machine = new PayloadItem
            {
                Item1 = false,
                Item2 = new string[0]
            };

            // Availability By Machine
            response = JarvisRequester.PostRequest(availabilityURL, requestPayload);
            foreach (EvaluatedResult eval in response.Results.Values)
            {
                List<Dimension> dimensions = eval.DimensionList.Values;
                string runnerName = dimensions.Where(dim => dim.Key.Equals(Dimension.RUNNER_NAME)).Single().Value;
                string machine = dimensions.Where(dim => dim.Key.Equals(Dimension.MACHINE)).Single().Value;
                SeriesValues seriesValues = new SeriesValues()
                {
                    StartTimeMillisUtc = startMillisFromEpoch,
                    EndTimeMillisUtc = endMillisFromEpoch,
                    TimeResolutionInMillis = response.TimeResolutionInMilliseconds,
                    Values = eval.Scores.ToArray()
                };
                overview.SetMachineAvailability(seriesValues, runnerName, machine);
            }

            // Latency Payload
            requestPayload.Environment = null;
            requestPayload.Machine = null;
            // Latency URL
            string latencyURL = BuildURL("Latency", "Average", startMillisFromEpoch, endMillisFromEpoch);

            // Latency
            response = JarvisRequester.PostRequest(availabilityURL, requestPayload);

            foreach (EvaluatedResult eval in response.Results.Values)
            {
                List<Dimension> dimensions = eval.DimensionList.Values;
                string runnerName = dimensions.Where(dim => dim.Key.Equals(Dimension.RUNNER_NAME)).Single().Value;

                SeriesValues seriesValues = new SeriesValues()
                {
                    StartTimeMillisUtc = startMillisFromEpoch,
                    EndTimeMillisUtc = endMillisFromEpoch,
                    TimeResolutionInMillis = response.TimeResolutionInMilliseconds,
                    Values = eval.Scores.ToArray()
                };
                overview.SetPageLatency(seriesValues, runnerName);
            }

            return overview;
        }

        private static string BuildURL(string metric, string samplingType, long startTime, long endTime)
        {
            Dictionary<QueryParam, string> queryParams = new Dictionary<QueryParam, string>
            {
                { QueryParam.SamplingType           , samplingType },
                { QueryParam.StartTime              , startTime.ToString() },
                { QueryParam.EndTime                , endTime.ToString() },
                { QueryParam.Operator               , "NotEqual" },
                { QueryParam.Reducer                , "Average" },
                { QueryParam.Operand                , "-86" },
                { QueryParam.Top                    , "10" },
                { QueryParam.OrderBy                , "Ascending" },
                { QueryParam.IncludeSeries          , "true" },
                { QueryParam.SeriesResolution       , "60000" }, // resolution is 1 minute
                { QueryParam.SeriesAggregationType  , "Automatic" },
                { QueryParam.ZeroAsNoValueSentinel  , "false" }
            };
            return URLBuilder.BuildURL("sporunners", "SharePointOnline-ActiveMonitoring", metric, queryParams);
        }
    }
}
