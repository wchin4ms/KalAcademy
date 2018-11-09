using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisReader
{
    public static class Properties
    {
        private static Dictionary<String, String> list = null;
        private const string FileName = "auth.properties";

        public static string Get(String field)
        {
            if (list == null)
            {
                Load();
            }

            string value = "";
            if (list.ContainsKey(field))
            {
                value = list[field];
            }
            return value;
        }

        private static void Load()
        {
            list = new Dictionary<String, String>();

            if (System.IO.File.Exists(FileName))
            {
                LoadFromFile(FileName);
            }
        }

        private static void LoadFromFile(String file)
        {
            foreach (String line in System.IO.File.ReadAllLines(file))
            {
                if ((!String.IsNullOrEmpty(line)) &&
                    (!line.StartsWith(";")) &&
                    (!line.StartsWith("#")) &&
                    (!line.StartsWith("'")) &&
                    (line.Contains('=')))
                {
                    int index = line.IndexOf('=');
                    String key = line.Substring(0, index).Trim();
                    String value = line.Substring(index + 1).Trim();

                    if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                        (value.StartsWith("'") && value.EndsWith("'")))
                    {
                        value = value.Substring(1, value.Length - 2);
                    }

                    try
                    {
                        //ignore dublicates
                        list.Add(key, value);
                    }
                    catch { }
                }
            }
        }
    }
}
