using System;
using System.Collections.Generic;
using System.IO;
using JR.Cms.Domain.Interface.Enum;
using JR.DevFw.Framework;

namespace JR.Cms.Core
{
    /// <summary>
    /// 模板管理器
    /// </summary>
    public class TemplateManager
    {
        private readonly string _rootDirPath;

        private IDictionary<string, TemplateSetting> _dict;

        internal TemplateManager(String dirPath)
        {
            this._rootDirPath = dirPath;
            this._dict = LoadFromDirectory(this._rootDirPath);
        }

        private static IDictionary<string, TemplateSetting> LoadFromDirectory(string rootDirPath)
        {
            IDictionary<string, TemplateSetting> dict;
            DirectoryInfo dir = new DirectoryInfo(rootDirPath);
            DirectoryInfo[] dirs = dir.GetDirectories();
            dict = new Dictionary<string, TemplateSetting>(dirs.Length);
            if (!dir.Exists) return dict;
            foreach (DirectoryInfo d in dirs)
            {
                if ((d.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                {
                    String tplConfigFile = String.Format("{0}{1}/tpl.conf", rootDirPath, d.Name);
                    if (!File.Exists(tplConfigFile))
                    {
                        SettingFile sf = new SettingFile(tplConfigFile);
                        sf.Set("name", d.Name);
                        sf.Flush();
                    }
                    TemplateSetting ts = new TemplateSetting(d.Name, tplConfigFile);
                    ts.CfgEnabledMobiPage = Directory.Exists(String.Format("{0}{1}/_mobile_", rootDirPath, d.Name));
                    dict.Add(d.Name, ts);
                }
            }
            return dict;
        }

        public TemplateSetting Get(string tpl)
        {
            if (!this._dict.ContainsKey(tpl))
            {
                throw new ArgumentException("no such template named "+tpl);
            }
            return this._dict[tpl];
        }
    }
}