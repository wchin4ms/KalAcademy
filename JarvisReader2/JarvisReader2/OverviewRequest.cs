using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisReader
{
    abstract class OverviewRequest<T>
    {
        static T Get(string farmLabel, long startTime, long endTime)
        {
            throw new NotImplementedException();
        }
    }
}
