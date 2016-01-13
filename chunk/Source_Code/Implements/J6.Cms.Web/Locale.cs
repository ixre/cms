using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using J6.Cms.Conf;
using J6.Cms.Domain.Interface.Common.Language;
using Newtonsoft.Json;

namespace J6.Cms.Web
{

    public static class Locale
    {  
        public class KeyPair
        {
            public String key;
            public IDictionary<int, String> value;
        }

        public static void AddKey(string key)
        {
            String file = Cms.PyhicPath + CmsVariables.SITE_CONF_PATH + "locale.json";
            IList<KeyPair> list = ParseToList(file);
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (String.CompareOrdinal(list[i].key, key) == 0)
                    {
                        throw new ArgumentException("已存在相同的键");
                    }
                }
            }
            else
            {
                list = new List<KeyPair>();
            }
            list.Add(new KeyPair{key = key});
            FlushToFile(file, list);
        }

        private static void FlushToFile(string file, IList<KeyPair> list)
        {
            String json = JsonConvert.SerializeObject(list);
            StreamWriter sw = new StreamWriter(file,false);
            sw.Write(json);
            sw.Close();
        }

        private static IList<KeyPair> ParseToList(string file)
        {
            String json = ReadJson(file);
            return JsonConvert.DeserializeObject<List<KeyPair>>(json);
        }

        private static string ReadJson(string file)
        {
            StreamReader rd = new StreamReader(file, Encoding.UTF8);
            String json = rd.ReadToEnd();
            rd.Close();
            return json;
        }
    }
}
