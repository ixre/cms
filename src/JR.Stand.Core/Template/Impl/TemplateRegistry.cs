//
// Copyright 2011 @ S1N1.COM,All right reseved.
// Name:TemplateRegister.cs
// Author:newmin
// Create:2011/06/28
//

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace JR.Stand.Core.Template.Impl
{
    /// <summary>
    /// 模板仓储
    /// </summary>
    public class TemplateRegistry
    {
        private DirectoryInfo directory;
        private TemplateNames nameType;

        /// <summary>
        /// 注册模板时发生
        /// </summary>
        public event TemplateBehavior OnRegister;

        public TemplateRegistry(string directoryPath, TemplateNames nametype)
        {
            this.nameType = nametype;
            this.directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + directoryPath);
            if (!this.directory.Exists) throw new DirectoryNotFoundException("模版文件夹不存在!");
        }

        public TemplateRegistry(string directoryPath)
        {
            this.directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + directoryPath);
            if (!this.directory.Exists) throw new DirectoryNotFoundException("模版文件夹不存在!");
        }

        public TemplateRegistry(DirectoryInfo templateDirectory, TemplateNames nameType)
        {
            this.nameType = nameType;
            this.directory = templateDirectory;
            if (!this.directory.Exists) throw new DirectoryNotFoundException("模版文件夹不存在!");
        }

        /// <summary>
        /// 注册模板
        /// </summary>
        public void Register()
        {
            //注册模板
            RegisterTemplates(directory, this.nameType);
            //触发注册模板事件
            if (OnRegister != null) OnRegister();
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
    }
}