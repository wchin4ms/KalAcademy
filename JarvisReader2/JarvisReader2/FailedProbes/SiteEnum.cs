using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisReader.FailedProbes
{
    enum SiteEnum
    {
        HOMEPAGE,
        TEAMSITEHOMEPAGE,
        UPLOADDOC
    }

    static class SiteEnumExtension
    {
        public static string GetSiteName(this SiteEnum site)
        {
            string siteName = "homepage";
            switch (site)
            {
                case SiteEnum.TEAMSITEHOMEPAGE:
                    siteName = "teamsitehomepage";
                    break;
                case SiteEnum.UPLOADDOC:
                    siteName = "uploaddoc";
                    break;
            }
            return siteName;
        }
    }
}
