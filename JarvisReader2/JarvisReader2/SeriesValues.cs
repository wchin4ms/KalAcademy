using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisReader
{
    class SeriesValues
    {
        public long StartTimeMillisUtc { get; set; }
        public long EndTimeMillisUtc { get; set; }
        public int TimeResolutionInMillis { get; set; }
        public decimal?[] Values { get; set; }
        public string InfoString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("(");
            DateTime epoch = new DateTime(1970, 1, 1);
            TimeSpan tempTime = TimeSpan.FromMilliseconds(StartTimeMillisUtc);
            stringBuilder.Append(epoch + tempTime);
            stringBuilder.Append(" - ");
            tempTime = TimeSpan.FromMilliseconds(EndTimeMillisUtc);
            stringBuilder.Append(epoch + tempTime);
            stringBuilder.Append("):");
            stringBuilder.Append("\n    ");
            stringBuilder.Append(string.Join(", ", Values));
            return stringBuilder.ToString();
        }
    }
}
