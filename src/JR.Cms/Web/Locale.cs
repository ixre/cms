using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JR.Cms.Conf;
using JR.Cms.Domain.Interface.Common.Language;
using JR.Stand.Abstracts.Web;
using Newtonsoft.Json;

namespace JR.Cms.Web
{
    /// <summary>
    /// 本地化(管理后台)
    /// </summary>
    public static class Locale
    {
        public static void AddKey(string key)
        {
            var file = Cms.PhysicPath + CmsVariables.SITE_LOCALE_PATH;
            ChkFile(file);
            var list = ParseToList(file);
            if (list != null)
            {
                for (var i = 0; i < list.Count; i++)
                    if (string.CompareOrdinal(list[i].key, key) == 0)
                        throw new ArgumentException("已存在相同的键");
            }
            else
            {
                list = new List<LangKvPair>();
            }

            list.Add(new LangKvPair {key = key});
            FlushToFile(file, SortLocaleList(list));
        }

        private static void ChkFile(string file)
        {
            if (!File.Exists(file)) File.Create(file).Dispose();
        }

        private static string FlushToFile(string file, IList<LangKvPair> list)
        {
            ChkFile(file);
            var json = JsonConvert.SerializeObject(list);
            var sw = new StreamWriter(file, false);
            sw.Write(json);
            sw.Close();
            return json;
        }

        private static IList<LangKvPair> ParseToList(string file)
        {
            var json = ReadJson(file);
            return JsonConvert.DeserializeObject<List<LangKvPair>>(json);
        }

        private static string ReadJson(string file)
        {
            if (File.Exists(file))
            {
                var rd = new StreamReader(file, Encoding.UTF8);
                var json = rd.ReadToEnd();
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
            var file = Cms.PhysicPath + CmsVariables.SITE_LOCALE_PATH;
            ChkFile(file);
            var list = ParseToList(file);
            if (list == null) return;
            for (var i = 0; i < list.Count; i++)
                if (string.CompareOrdinal(list[i].key, key) == 0)
                {
                    list.Remove(list[i]);
                    FlushToFile(file, list);
                    break;
                }
        }

        public static string SaveByPostForm(ICompatibleRequest form)
        {
            var file = Cms.PhysicPath + CmsVariables.SITE_LOCALE_PATH;
            IList<LangKvPair> list = new List<LangKvPair>();
            var row = 0;
            var lang = 0;
            LangKvPair p;
            IList<string> k1 = new List<string>();
            IList<string> v1 = new List<string>();
            foreach (var pk in form.FormKeys())
                if (pk.StartsWith("k_"))
                    k1.Add(pk);
                else if (pk.StartsWith("f_")) v1.Add(pk);

            foreach (var k in k1)
                if (int.TryParse(k.Substring(2), out row)) //获取行号
                {
                    p = new LangKvPair();
                    p.key = form.Form(k);
                    p.value = new Dictionary<int, string>();
                    var fPre = "f_" + row.ToString() + "_";
                    for (var j = 0; j < v1.Count; j++)
                        if (v1[j].StartsWith(fPre)) //获取对英语言的值，并移除
                            if (int.TryParse(v1[j].Substring(fPre.Length), out lang))
                                p.value[lang] = form.Form(v1[j]);
                        //v1.Remove(v1[j]);
                    list.Add(p);
                }

            if (list.Count > 0)
            {
                var arr = SortLocaleList(list);
                return FlushToFile(file, arr);
            }

            return null;
        }

        private static LangKvPair[] SortLocaleList(IList<LangKvPair> list)
        {
            var arr = list.ToArray();
            Array.Sort(arr, (e1, e2) => { return string.CompareOrdinal(e1.key, e2.key); });
            return arr;
        }
    }
}