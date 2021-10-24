using System;
using System.Collections.Generic;
using System.IO;
using JR.Cms.Domain.Interface.Enum;
using JR.Stand.Core.Framework;
using JR.Stand.Core.Template;

namespace JR.Cms.Core
{
    /// <summary>
    /// 模板管理器
    /// </summary>
    public class TemplateManager
    {
        private readonly string _rootDirPath;

        private IDictionary<string, TemplateSetting> _dict;

        internal TemplateManager(string dirPath)
        {
            _rootDirPath = dirPath;
            _dict = LoadFromDirectory(_rootDirPath);
        }

        private static IDictionary<string, TemplateSetting> LoadFromDirectory(string rootDirPath)
        {
            IDictionary<string, TemplateSetting> dict;
            var dir = new DirectoryInfo(rootDirPath);
            var dirs = dir.GetDirectories();
            dict = new Dictionary<string, TemplateSetting>(dirs.Length);
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

                    var ts = new TemplateSetting(d.Name, tplConfigFile);
                    ts.CfgEnabledMobiPage = Directory.Exists(string.Format("{0}{1}/_mobile_", rootDirPath, d.Name));
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
            if (!_dict.ContainsKey(tpl)) throw new TemplateException($"Can not find template in folder '/templates/{tpl}'");
            return _dict[tpl];
        }
    }
}