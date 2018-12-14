using JarvisReader.DateUtils;
using System;
using System.Collections.Generic;
using System.Threading;

namespace JarvisReader.FailedProbes
{
    class FailedProbesRequest
    {
        private const string ENDPOINT = "https://firstparty.monitoring.windows.net/";
        public static FailedProbesOverview Get(string farmLabel, SiteEnum site, DateTime startTime, DateTime endTime)
        {
            FailedProbesOverview overview = new FailedProbesOverview();

            string searchID = Guid.NewGuid().ToString();
            // start the search
            SearchPayload payload = new SearchPayload
            {
                EndTime = DateTimeUtils.ToZuluString(endTime),
                Endpoint = ENDPOINT,
                EventNames = new List<string>()
                {
                    "RunnerCentralEventTable"
                },
                IdentityColumns = new Dictionary<string, string>(),
                MaxResults = 50000,
                Namespaces = new List<string>()
                {
                    "RunnerService"
                },
                Query = null,
                QueryID = searchID,
                QueryType = 0,
                SearchCriteria = new List<LogSearchCriteria>()
                {
                    new LogSearchCriteria
                    {
                        Key = "Status",
                        Operation = 1,
                        Value = "Unhealthy"
                    },
                    new LogSearchCriteria
                    {
                        Key = "Role",
                        Operation = 1,
                        Value = "RunnerContainer"
                    },
                    new LogSearchCriteria
                    {
                        Key = "InstanceName",
                        Operation = 5,
                        Value = "/" + farmLabel + "/Primary/"
                    },
                    new LogSearchCriteria
                    {
                        Key = "Name",
                        Operation = 1,
                        Value = "ngspo" + site.GetSiteName()
                    }
                },
                StartTime = DateTimeUtils.ToZuluString(startTime)
            };
            FailedProbesRequester.Search(payload);

            // search started, let's ping until we find results
            SearchResponse lastResponse;
            int count = 0;
            do
            {
                Console.WriteLine("sleeping...");
                Thread.Sleep(1000);
                lastResponse = FailedProbesRequester.Ping(searchID);
            } while (!lastResponse.Status.Equals("Completed") && count++ < 60);

            Console.WriteLine("COUNT: " + count);

            // search is complete, let's grab top 20 results
            lastResponse = FailedProbesRequester.GetFailedProbes(searchID);

            Console.Write(lastResponse);
            return overview;
        }
    }
}
