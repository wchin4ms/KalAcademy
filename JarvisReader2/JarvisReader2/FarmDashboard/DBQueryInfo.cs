using System.Text;

namespace JarvisReader.FarmDashboard
{
    class DBQueryInfo
    {
        public string Database { get; set; }
        public string Server { get; set; }
        public SeriesValues Values { get; set; }
        public string InfoString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("DB ");
            stringBuilder.Append(Database);
            stringBuilder.Append(" on ");
            stringBuilder.Append(Server);
            stringBuilder.Append(" ");
            stringBuilder.Append(Values.InfoString());
            return stringBuilder.ToString();
        }
    }
}
