using JarvisReader.DateUtils;
using System.Text;

namespace JarvisReader.FarmDashboard
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
            stringBuilder.Append(DateTimeUtils.EPOCH_1970.AddMilliseconds(StartTimeMillisUtc));
            stringBuilder.Append(" - ");
            stringBuilder.Append(DateTimeUtils.EPOCH_1970.AddMilliseconds(EndTimeMillisUtc));
            stringBuilder.Append("):");
            stringBuilder.Append("\n    ");
            stringBuilder.Append(string.Join(", ", Values));
            return stringBuilder.ToString();
        }
    }
}
