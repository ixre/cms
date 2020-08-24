using System;
using System.Collections.Generic;
using System.IO;
using JR.Stand.Core.Framework;
using JR.Stand.Core.Template;

namespace JR.Cms.Core
{
    /// <summary>
    /// 模板管理器
    /// </summary>
    public class TemplateManager
    {
        private readonly IDictionary<string, TemplateSetting> _dict;

        internal TemplateManager(string dirPath)
        {
            _dict = LoadFromDirectory(dirPath);
        }

        private static IDictionary<string, TemplateSetting> LoadFromDirectory(string rootDirPath)
        {
            var dir = new DirectoryInfo(rootDirPath);
            var dirs = dir.GetDirectories();
            IDictionary<string, TemplateSetting> dict = new Dictionary<string, TemplateSetting>(dirs.Length);
            if (!dir.Exists) return dict;
            foreach (var d in dirs)
                if ((d.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                {
                    var tplConfigFile = $"{rootDirPath}{d.Name}/tpl.conf";
                    if (!File.Exists(tplConfigFile))
                    {
                        var sf = new SettingFile(tplConfigFile);
                        sf.Set("name", d.Name);
                        sf.Flush();
                    }

                    var ts = new TemplateSetting(d.Name, tplConfigFile)
                    {
                        CfgEnabledMobiPage = Directory.Exists($"{rootDirPath}{d.Name}/_mobile_")
                    };
                    dict.Add(d.Name, ts);
                }

            return dict;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tpl"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public TemplateSetting Get(string tpl)
        {
            if (!_dict.ContainsKey(tpl))
                throw new TemplateException($"Can not find template in folder '/templates/{tpl}'");
            return _dict[tpl];
        }
    }
}