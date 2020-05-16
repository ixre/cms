//
// Copyright 2011 @ S1N1.COM,All right reserved.
// Name:TemplateRegister.cs
// Author:newmin
// Create:2011/06/28
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace JR.Stand.Core.Template.Impl
{
    /// <summary>
    /// 模板仓储
    /// </summary>
    public class TemplateRegistry
    {
        private readonly Options _options;
        private readonly IDataContainer _container;
        private readonly IList<string> directories = new List<string>();

        public TemplateRegistry(IDataContainer container,Options options)
        {
            this._options = options ?? new Options();
            this._container = container;
        }

        public TemplateRegistry(IDataContainer container) : this(container,new Options())
        {
        }


        /// <summary>
        /// 注册模板
        /// </summary>
        public void Register(string directory)
        {
            var dir = new DirectoryInfo(EnvUtil.GetBaseDirectory()+"/" + directory);
            if (!dir.Exists) throw new DirectoryNotFoundException("模版文件夹不存在!");
            // 添加到目录数组,用于重新加载模板
            if(!this.directories.Contains(directory))this.directories.Add(directory);
            //注册模板
            RegisterTemplates(dir, this._options.Names);
        }

        /// <summary>
        /// 是否存在模板
        /// </summary>
        /// <param name="templatePath"></param>
        /// <returns></returns>
        public static bool Exists(String templatePath)
        {
            return TemplateCache.Exists(templatePath);
        }

        //递归方式注册模板
        private static void RegisterTemplates(DirectoryInfo dir, TemplateNames nameType)
        {
            // tml 为模板文件，防止可以被直接浏览
            Regex allowExt = new Regex("(.html|.part.html|.phtml)$", RegexOptions.IgnoreCase);
            foreach (FileInfo file in dir.GetFiles())
            {
                if (allowExt.IsMatch(file.Extension))
                {
                   // Console.WriteLine("---" + file.FullName);
                    TemplateCache.RegisterTemplate(TemplateUtility.GetTemplateId(file.FullName, nameType), file.FullName);
                }
            }
            foreach (DirectoryInfo _dir in dir.GetDirectories())
            {
                //如果文件夹是可见的
                if ((_dir.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                {
                    RegisterTemplates(_dir, nameType);
                }
            }
        }

        public void Reload()
        {
            foreach (var s in this.directories)
            {
                this.Register(s);
            }
        }

        public TemplatePage GetPage(string pageName)
        {
           return new TemplatePage(pageName,this._container);
        }
    }
}