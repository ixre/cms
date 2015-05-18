/**
 * Copyright (C) 2007-2015 S1N1.COM,All rights reseved.
 * Get more infromation of this software,please visit site http://cms.ops.cc
 * 
 * name : Config.cs
 * author : newmin (new.min@msn.com)
 * date : 2012/12/01 23:00:00
 * description : 
 * history : 
 */

using AtNet.DevFw.PluginKernel;

namespace com.plugin.sso
{
    /// <summary>
    /// Description of Config.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// 是否为开发模式
        /// </summary>
        public static bool DebugMode = !true;

        internal static PluginPackAttribute PluginAttr;

        public static void Init(IPlugin plugin)
        {
            PluginAttr = PluginUtil.GetAttribute(plugin);
            InitCfg(PluginAttr);
        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        /// <param name="attr"></param>
        private static void InitCfg(PluginPackAttribute attr)
        {
                //            bool isChanged = false;
                //            if (!attr.Settings.Contains("test_conf"))
                //            {
                //                attr.Settings.Add("test_conf", "测试配置", true);
                //                isChanged = true;
                //            }
                //
                //            if (isChanged) attr.Settings.Flush();
        }
    }
}
