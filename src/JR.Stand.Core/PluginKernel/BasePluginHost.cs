using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using JR.Stand.Core.Framework;

namespace JR.Stand.Core.PluginKernel
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BasePluginHost : IPluginHost
    {
        internal static IDictionary<IPlugin, PluginPackAttribute> plugins;

        protected static LogFile Log;
        private PluginHostAttribute _attr = null;


        static BasePluginHost()
        {
            plugins = new Dictionary<IPlugin, PluginPackAttribute>();

            string appDirectory = EnvUtil.GetBaseDirectory();

            string pluginDirectory = String.Concat(
                appDirectory,
                PluginConfig.PLUGIN_DIRECTORY);

            //清空日志
            string pluginTmpDirectory = String.Concat(appDirectory, PluginConfig.PLUGIN_TMP_DIRECTORY);
            if (!Directory.Exists(pluginTmpDirectory))
            {
                Directory.CreateDirectory(pluginTmpDirectory);
            }
            Log = new LogFile(String.Format("{0}plugin_load.log",
                pluginTmpDirectory), false);
            Log.Truncate();

            if (Directory.Exists(pluginDirectory))
            {
                LoadPluginFromDirectory(pluginDirectory);
            }
            else
            {
                Directory.CreateDirectory(pluginDirectory).Create();
            }

            //加载程序集的.so文件
            loadFromAppDomain();
        }

        /// <summary>
        /// 从目录中加载插件
        /// </summary>
        /// <param name="pluginDirectory"></param>
        private static void LoadPluginFromDirectory(string pluginDirectory)
        {
            bool loadResult = true;

            //多个后缀的情况下
            IList<String> files = new List<string>(10);

            string[] multExt = PluginConfig.GetFilePartterns();
            foreach (String ext in multExt)
            {
                String[] searchedExt = Directory.GetFiles(pluginDirectory, ext);
                foreach (String sext in searchedExt)
                {
                    files.Add(sext);
                }
            }

            if (Log != null)
            {
                Log.Println(String.Format("{0:yyyy-MM-dd HH:mm:ss}   [+]Plugin Loading"
                                          + "\r\n========================================\r\n"
                                          + "Directory:{1} \t Total DLL:{2}\r\n",
                    DateTime.Now, pluginDirectory.Replace("\\", "/"),
                    files.Count.ToString()));
            }
            foreach (string file in files)
            {
                if (!LoadPlugin(file))
                {
                    loadResult = false;
                }
            }

            if (Log != null)
            {
                Log.Println(String.Format("load complete!result:{0}", loadResult ? "Ok" : "Error"));
            }
        }

        /// <summary>
        /// 加载单个插件
        /// </summary>
        /// <param name="pluginFile"></param>
        public static bool LoadPlugin(string pluginFile)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(pluginFile);
                Assembly ass = Assembly.Load(bytes);
                loadFromAssembly(ass);
            }
            catch (ReflectionTypeLoadException exc)
            {
                if (Log != null)
                {
                    Log.Println(String.Format("Assembly {0} happend exception:{1}\r\nExceptions:\r\n",
                        pluginFile.Substring(pluginFile.LastIndexOfAny(new char[] { '/', '\\' }) + 1),
                        exc.Message));
                    foreach (Exception e in exc.LoaderExceptions)
                    {
                        Log.Println(e.Message);
                    }

                }
                return false;
            }
            catch (Exception err)
            {
                if (Log != null)
                {
                    Log.Println(String.Format("\r\nAssembly {0} happend exception:{1}",
                        pluginFile.Substring(pluginFile.LastIndexOfAny(new char[] { '/', '\\' }) + 1),
                        err.Message));
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        private static void loadFromAppDomain()
        {
            string dir = EnvUtil.GetBaseDirectory()+ "/bin";
            if (!Directory.Exists(dir)) return;
            string[] dllFiles = Directory.GetFiles(dir, "*.so");

            Log.Println(String.Format("{0:yyyy-MM-dd HH:mm:ss}   [+]Plugin Loading From Bin"
                                      + "\r\n========================================\r\n"
                                      + "Total .so file : {1}\r\n",
                DateTime.Now,
                dllFiles.Length.ToString()));

            foreach (string f in dllFiles)
            {
                LoadPlugin(f);
            }
        }

        /// <summary>
        /// 从程序集中加载插件
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private static bool loadFromAssembly(Assembly assembly)
        {
            PluginPackAttribute attribute = null;
            IPlugin plugin = null;

            var attbs = assembly.GetCustomAttributes(typeof(PluginPackAttribute), false);

            if (attbs.Length != 0)
            {
                attribute = (PluginPackAttribute)attbs[0];
            }
            else
            {
                //                foreach (Attribute attr in assembly.GetCustomAttributes(false))
                //                {
                //                    if (attr.ToString().IndexOf("PluginPackAttribute") != -1)
                //                    {
                //                        attribute = (PluginPackAttribute) attr;
                //
                //                        Log.Println(attribute.Name);
                //                    }
                //                    else
                //                    {
                //                        Log.Println(attr.ToString());
                //                    }
                //                }
                //
                //               Log.Println(assembly.GetName().Name+" not a plugin!");
                return false;
            }

            var types = assembly.GetTypes();
            foreach (Type type in types)
            {
                if (type.IsClass)
                {
                    foreach (Type t in type.GetInterfaces())
                    {
                        if (t == typeof(IPlugin))
                        {
                            plugin = Activator.CreateInstance(type) as IPlugin;
                            if (plugin == null)
                            {
                                continue;
                            }

                            if (attribute != null)
                            {
                                plugins.Add(plugin, attribute);
                            }
                        }
                    }
                }
            }

            if (Log != null)
            {
                Log.Println(String.Format("{0}({1}) be found ; version is {2}.",
                    attribute.Name,
                    assembly.GetName().Name,
                    attribute.Version));
            }
            return true;
        }

        public bool LoadFromAssembly(Assembly assembly)
        {
            return loadFromAssembly(assembly);
        }

        /// <summary>
        /// 连接插件
        /// </summary>
        /// <returns></returns>
        public virtual bool Connect()
        {
            Iterate(
                (p, a) =>
                {
                    if (a.State == PluginState.Normal)
                    {
                        p.Connect(this);
                    }
                }
                );
            return true;
        }

        /// <summary>
        /// 迭代插件
        /// </summary>
        /// <param name="handler"></param>
        public void Iterate(PluginHandler handler)
        {
            foreach (IPlugin p in plugins.Keys)
            {
                if (CanAdapter(p))
                {
                    handler(p, plugins[p]);
                }
            }
        }

        public virtual void Run()
        {
            Iterate((p, i) =>
            {
                if (i.State == PluginState.Normal)
                {
                    p.Run();
                }
            });
        }

        public virtual void Pause()
        {
            Iterate((p, i) => { i.State = PluginState.Stop; });
        }

        public bool Run(string pluginId)
        {
            var runed = false;
            Iterate((p, i) =>
            {
                if (!runed && i.State == PluginState.Normal && String.Compare(pluginId, i.WorkIndent, true) == 0)
                {
                    p.Run();
                    runed = true;
                }
            });
            return runed;
        }

        public bool Pause(string pluginId)
        {
            var runed = false;
            Iterate((p, i) =>
            {
                if (!runed && i.State == PluginState.Normal && String.Compare(pluginId, i.WorkIndent, true) == 0)
                {
                    i.State = PluginState.Stop;
                    runed = true;
                }
            });
            return runed;
        }

        /// <summary>
        /// 检查是否能适配插件
        /// </summary>
        public bool CanAdapter(IPlugin plugin)
        {
            if (this.Attribute == null || String.IsNullOrEmpty(this.Attribute.TypePattern))
            {
                return true;
            }
            return Regex.IsMatch(plugin.GetType().Name, this.Attribute.TypePattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 附加日志
        /// </summary>
        /// <param name="content"></param>
        public void Logln(string content)
        {
            if (Log != null)
            {
                Log.Printf("{0:yyyy-MM-dd HH:mm:ss}\r\n========================================\r\n{1}\r\n\r\n",
                    DateTime.Now, content);
            }
        }

        private PluginHostAttribute Attribute
        {
            get
            {
                if (_attr == null)
                {
                    var ppas = GetType().GetCustomAttributes(typeof(PluginHostAttribute), true);
                    if (ppas.Length != 0)
                    {
                        _attr = (PluginHostAttribute)ppas[0];
                    }
                }
                return _attr;
            }
        }


        public PluginPackAttribute GetAttribute(IPlugin plugin)
        {
            return PluginUtil.GetAttribute(plugin);
        }
    }
}