using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JarvisReader.FarmDashboard
{
    class ProbeOverview : IOverview
    {
        private const string HOME_PAGE_RUNNER_NAME = "HomePage";
        private const string ORG_ID_AUTH_RUNNER_NAME = "OrgIdAuth";
        private const string TEAM_SITE_HOME_PAGE_RUNNER_NAME = "TeamSiteHomePage";
        private const string UPLOAD_DOC_RUNNER_NAME = "UploadDoc";

        //HomePage 1 is at index 0
        private SeriesValues[] HomePage = new SeriesValues[2];
        private SeriesValues[] OrgIdAuth = new SeriesValues[2];
        private SeriesValues[] TeamSite = new SeriesValues[2];
        private SeriesValues[] UploadDoc = new SeriesValues[2];

        //Latency Info
        private SeriesValues HomePageLatency { get; set; }
        private SeriesValues UploadDocLatency { get; set; }
        private SeriesValues TeamSiteLatency { get; set; }

        // Dictionary - Machine Name : SeriesValues
        private Dictionary<string, SeriesValues> HomePageMachineValues = new Dictionary<string, SeriesValues>();
        private Dictionary<string, SeriesValues> UploadDocMachineValues = new Dictionary<string, SeriesValues>();
        private Dictionary<string, SeriesValues> TeamSiteMachineValues = new Dictionary<string, SeriesValues>();

        public void SetPageAvailability (SeriesValues seriesValues, string runner, int instanceNum)
        {
            switch (runner)
            {
                case ProbeOverview.HOME_PAGE_RUNNER_NAME:
                    HomePage[instanceNum - 1] = seriesValues;
                    break;
                case ProbeOverview.ORG_ID_AUTH_RUNNER_NAME:
                    OrgIdAuth[instanceNum - 1] = seriesValues;
                    break;
                case ProbeOverview.TEAM_SITE_HOME_PAGE_RUNNER_NAME:
                    TeamSite[instanceNum - 1] = seriesValues;
                    break;
                case ProbeOverview.UPLOAD_DOC_RUNNER_NAME:
                    UploadDoc[instanceNum - 1] = seriesValues;
                    break;
            }
        }

        public void SetPageLatency (SeriesValues seriesValues, string runner)
        {
            switch (runner)
            {
                case ProbeOverview.HOME_PAGE_RUNNER_NAME:
                    HomePageLatency = seriesValues;
                    break;
                case ProbeOverview.TEAM_SITE_HOME_PAGE_RUNNER_NAME:
                    TeamSiteLatency = seriesValues;
                    break;
                case ProbeOverview.UPLOAD_DOC_RUNNER_NAME:
                    UploadDocLatency = seriesValues;
                    break;
            }
        }

        public void SetMachineAvailability (SeriesValues seriesValues, string runner, string machine)
        {
            switch (runner)
            {
                case ProbeOverview.HOME_PAGE_RUNNER_NAME:
                    HomePageMachineValues.Add(machine, seriesValues);
                    break;
                case ProbeOverview.TEAM_SITE_HOME_PAGE_RUNNER_NAME:
                    TeamSiteMachineValues.Add(machine, seriesValues);
                    break;
                case ProbeOverview.UPLOAD_DOC_RUNNER_NAME:
                    UploadDocMachineValues.Add(machine, seriesValues);
                    break;
            }
        }

        public void Evaluate()
        {
        }

        public string InfoString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            SeriesValues svPointer;
            stringBuilder.Append("\nAvailability:");
            for (int index = 0; index < HomePage.Length; index++)
            {
                stringBuilder.Append("\n  HomePage");
                stringBuilder.Append(index + 1);
                stringBuilder.Append(": ");
                svPointer = HomePage[index];
                stringBuilder.Append(svPointer.InfoString());
            }
            for (int index = 0; index < UploadDoc.Length; index++)
            {
                stringBuilder.Append("\n  UploadDoc");
                stringBuilder.Append(index + 1);
                stringBuilder.Append(": ");
                svPointer = UploadDoc[index];
                stringBuilder.Append(svPointer.InfoString());
            }
            for (int index = 0; index < TeamSite.Length; index++)
            {
                stringBuilder.Append("\n  TeamSite");
                stringBuilder.Append(index + 1);
                stringBuilder.Append(": ");
                svPointer = TeamSite[index];
                stringBuilder.Append(svPointer.InfoString());
            }
            stringBuilder.Append("\nLatency:");
            stringBuilder.Append("\n  HomePage");
            svPointer = HomePageLatency;
            stringBuilder.Append(svPointer.InfoString());
            stringBuilder.Append("\n  UploadDoc");
            svPointer = UploadDocLatency;
            stringBuilder.Append(svPointer.InfoString());
            stringBuilder.Append("\n  TeamSite");
            svPointer = TeamSiteLatency;
            stringBuilder.Append(svPointer.InfoString());
            stringBuilder.Append("\nMachine Availability:");
            stringBuilder.Append("\n  HomePage");
            foreach(KeyValuePair<string, SeriesValues> entry in HomePageMachineValues)
            {
                stringBuilder.Append("\n    ");
                stringBuilder.Append(entry.Key);
                stringBuilder.Append(": ");
                stringBuilder.Append(entry.Value.InfoString());
            }
            foreach (KeyValuePair<string, SeriesValues> entry in UploadDocMachineValues)
            {
                stringBuilder.Append("\n    ");
                stringBuilder.Append(entry.Key);
                stringBuilder.Append(": ");
                stringBuilder.Append(entry.Value.InfoString());
            }
            foreach (KeyValuePair<string, SeriesValues> entry in TeamSiteMachineValues)
            {
                stringBuilder.Append("\n    ");
                stringBuilder.Append(entry.Key);
                stringBuilder.Append(": ");
                stringBuilder.Append(entry.Value.InfoString());
            }
            return stringBuilder.ToString();
        }
    }
}
