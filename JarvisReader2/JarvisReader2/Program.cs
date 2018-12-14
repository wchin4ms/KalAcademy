using JarvisReader.AzureDashboard;
using JarvisReader.FailedProbes;
using JarvisReader.FarmDashboard;
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
            DateTime now = DateTime.Now.AddMinutes(-10);
            //FarmOverview farm = new FarmOverview(farmLabel);
            Console.WriteLine("------------------");
            //Console.Write(farm.InfoString());
            //test : yj1b1gykud
            Console.WriteLine("#######################################################################");
            AzureOverview azure = AzureDashboardRequest.Get("g5ajmo36i2", now.AddHours(-1), now);
            Console.WriteLine(" ------ ");
            DateTime testEnd = new DateTime(2018, 12, 13, 9, 1, 0, DateTimeKind.Local);
            DateTime testStart = new DateTime(2018, 12, 13, 8, 45, 0, DateTimeKind.Local);
            //FailedProbesRequest.Get("EMEA_52_CONTENT", SiteEnum.TEAMSITEHOMEPAGE, testStart, testEnd);
        }
    }
}
