using System;
using System.IO;
using JR.Stand.Core.Framework;

namespace JR.Stand.Core.PluginKernel
{
    /// <summary>
    /// 插件包信息
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public class PluginPackAttribute : Attribute
    {
        private SettingFile settings { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workerIndent"></param>
        public PluginPackAttribute(string workerIndent)
        {
            this.WorkIndent = workerIndent;
            bool isChanged = false;
            var dirPath = String.Concat(
                AppDomain.CurrentDomain.BaseDirectory,
                PluginConfig.PLUGIN_DIRECTORY,
                workerIndent,
                "/");

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath).Create();
            }

            settings = new SettingFile(string.Concat(dirPath, "plugin.config"));

            if (!settings.Contains(PluginSettingKeys.State))
            {
                settings.Set(PluginSettingKeys.State, "Normal");
                isChanged = true;
            }
            if (!settings.Contains(PluginSettingKeys.OverrideUrlIndent))
            {
                settings.Set(PluginSettingKeys.OverrideUrlIndent, "");
                isChanged = true;
            }

            if (isChanged)
            {
                settings.Flush();
            }
        }

        public PluginPackAttribute(string workerIndent, string icon, string name, string author, string portalUrl)
            :
                this(workerIndent)
        {
            Icon = icon;
            Name = name;
            Author = author;
            PortalUrl = portalUrl;
        }

        private string version;
        private string _workSpace;

        /// <summary>
        /// 工作目录标识,较好的命名:com.myplugin 或 mycompany.myplugin
        /// 将会在plugins目录下生成目录，存放需要的文件
        /// </summary>
        public string WorkIndent { get; set; }

        /// <summary>
        /// 插件图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 插件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 插件作者
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// 官方主页
        /// </summary>
        public string WebPage { get; set; }

        /// <summary>
        /// 版本号（主版本号.次版本号.修订版本号），与程序集版本号一致
        /// </summary>
        public string Version
        {
            get
            {
                if (version == null)
                {
                    var v = GetType().Assembly.GetName().Version;
                    version = String.Format("{0}.{1}.{2}", v.Major, v.Minor, v.Revision);
                }
                return version;
            }
        }

        /// <summary>
        /// 设置地址
        /// </summary>
        public string ConfigUrl { get; set; }

        /// <summary>
        /// 插件入口地址
        /// </summary>
        public string PortalUrl { get; set; }

        /// <summary>
        /// 插件描述信息
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 工作目录(插件根目录),如:C:/plugins/com.plugin.demo/
        /// </summary>
        public string WorkSpace
        {
            get
            {
                return this._workSpace ?? (this._workSpace =
                    String.Concat(
                        AppDomain.CurrentDomain.BaseDirectory,
                        PluginConfig.PLUGIN_DIRECTORY,
                        this.WorkIndent,
                        "/"
                        ));
            }
        }

        /// <summary>
        /// 插件状态
        /// </summary>
        public PluginState State
        {
            get
            {
                return (settings["state"] == "1" || settings["state"] == "Normal")
                    ? PluginState.Normal
                    : PluginState.Stop;
            }
            set
            {
                var state = value.ToString();
                settings["state"] = state;
                settings.Flush();
            }
        }

        /// <summary>
        /// 插件配置
        /// </summary>
        public SettingFile Settings
        {
            get { return settings; }
        }
    }
}