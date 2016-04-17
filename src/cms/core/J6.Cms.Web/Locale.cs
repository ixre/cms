using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using J6.Cms.Conf;
using J6.Cms.Domain.Interface.Common.Language;
using Newtonsoft.Json;

namespace J6.Cms.Web
{

    public static class Locale
    {  
        public static void AddKey(string key)
        {
            String file = Cms.PyhicPath + CmsVariables.SITE_LOCALE_PATH;
            ChkFile(file);
            IList<LangKvPair> list = ParseToList(file);
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
                list = new List<LangKvPair>();
            }
            list.Add(new LangKvPair{key = key});
            FlushToFile(file, SortLocaleList(list));
        }

        private static void ChkFile(string file)
        {
            if (!File.Exists(file))
            {
                File.Create(file).Dispose();
            }
        }

        private static string FlushToFile(string file, IList<LangKvPair> list)
        {
            ChkFile(file);
            String json = JsonConvert.SerializeObject(list);
            StreamWriter sw = new StreamWriter(file,false);
            sw.Write(json);
            sw.Close();
            return json;
        }

        private static IList<LangKvPair> ParseToList(string file)
        {
            String json = ReadJson(file);
            return JsonConvert.DeserializeObject<List<LangKvPair>>(json);
        }

        private static string ReadJson(string file)
        {
            if (File.Exists(file))
            {
                StreamReader rd = new StreamReader(file, Encoding.UTF8);
                String json = rd.ReadToEnd();
                rd.Close();
                return json;
            }
            return "[]";
        }

        /// <summary>
        /// 删除键
        /// </summary>
        /// <param name="key"></param>
        public static void DelKey(string key)
        {
            String file = Cms.PyhicPath + CmsVariables.SITE_LOCALE_PATH;
            ChkFile(file);
            IList<LangKvPair> list = ParseToList(file);
            if (list == null) return;
            for (int i = 0; i < list.Count; i++)
            {
                if (String.CompareOrdinal(list[i].key, key) == 0)
                {
                    list.Remove(list[i]);
                    FlushToFile(file, list);
                    break;
                }
            }
        }

        public static string SaveByPostForm(NameValueCollection form)
        {
            String file = Cms.PyhicPath + CmsVariables.SITE_LOCALE_PATH;
            IList<LangKvPair> list = new List<LangKvPair>();
            int row = 0;
            int lang = 0;
            LangKvPair p;
            IList<String> k1 = new List<string>();
            IList<String> v1 = new List<string>();
            foreach (String pk in form.Keys)
            {
                if (pk.StartsWith("k_"))
                {
                    k1.Add(pk);
                }else if (pk.StartsWith("f_"))
                {
                    v1.Add(pk);
                }
            }

            foreach (String k in k1)
            {
                if (int.TryParse(k.Substring(2), out row)) //获取行号
                {
                    p = new LangKvPair();
                    p.key = form.Get(k);
                    p.value = new Dictionary<int, string>();
                    String fPre = "f_" + row.ToString()+"_";
                    for (int j = 0;j < v1.Count;j++)
                    {
                        if (v1[j].StartsWith(fPre))  //获取对英语言的值，并移除
                        {
                            if (int.TryParse(v1[j].Substring(fPre.Length), out lang))
                            {
                                p.value[lang] = form[v1[j]];
                            }
                            //v1.Remove(v1[j]);
                        }
                    }
                    list.Add(p);
                }
            }

            if (list.Count > 0)
            {
                var arr = SortLocaleList(list);
                return  FlushToFile(file,arr);
            }
            return null;
        }

        private static LangKvPair[] SortLocaleList(IList<LangKvPair> list)
        {
            LangKvPair[] arr = list.ToArray();
            Array.Sort(arr, (e1, e2) => { return String.CompareOrdinal(e1.key, e2.key); });
            return arr;
        }
    }
}
