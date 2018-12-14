using System;

namespace JarvisReader.DateUtils
{
    public static class DateTimeUtils
    {
        public static readonly DateTime EPOCH_1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static double GetMillisFromEpoch()
        {
            return GetMillisFromEpoch(DateTime.Now);
        }
        public static double GetMillisFromEpoch(DateTime date)
        {
            return date.SafeUniversal().Subtract(EPOCH_1970).TotalMilliseconds;
        }
        public static string ToZuluString(this DateTime date)
        {
            return date.SafeUniversal().ToString("yyyy-MM-ddTHH:mm:ss.fffK");
        }
        private static DateTime SafeUniversal(this DateTime date)
        {
            return (DateTimeKind.Unspecified == date.Kind)
                ? new DateTime(date.Ticks, DateTimeKind.Utc)
                : date.ToUniversalTime();
        }
    }
}
