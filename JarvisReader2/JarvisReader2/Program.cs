using System;

namespace JarvisReader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("FarmLabel: ");
            string farmLabel = Console.ReadLine();
            Console.Write("Site: ");
            string site = Console.ReadLine();
            DateTime epoch = new DateTime(1970, 1, 1);
            long endTimeUtc = (long)(DateTime.Now - epoch).TotalMilliseconds;
            long startTimeUtc = endTimeUtc - 1000 * 60 * 60; //default to an hour for now
            //JarvisRequester.TestRequest();
            ProbeOverview probeOverview = ProbeOverviewRequest.Get(farmLabel, startTimeUtc, endTimeUtc);
            Console.WriteLine("\n\n\n\n\n\n\n\n\n\n");
            Console.WriteLine(probeOverview.InfoString());
            Console.WriteLine("\n\n\n\n\n\n\n\n\n\n");
            SQLPerfOverview sqlPerfOverview = SQLPerfOverviewRequest.Get(farmLabel, startTimeUtc, endTimeUtc);
            Console.WriteLine(sqlPerfOverview.InfoString());
        }
    }
}
