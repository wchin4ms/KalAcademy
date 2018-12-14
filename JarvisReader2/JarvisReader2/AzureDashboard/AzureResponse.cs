using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisReader.AzureDashboard
{
    class AzureResponse
    {
        [JsonProperty("Tables")]
        public List<AzureResponseTable> Tables;
    }

    class AzureResponseTable
    {
        [JsonProperty("TableName")]
        public string TableName;
        [JsonProperty("Columns")]
        public List<AzureResponseColumn> Columns;
        [JsonProperty("Rows")]
        public List<List<dynamic>> Rows;
    }

    class AzureResponseColumn
    {
        [JsonProperty("ColumnName")]
        public string ColumnName;
        [JsonProperty("DataType")]
        public string DataType;
        [JsonProperty("ColumnType")]
        public string ColumnType;
    }
}
