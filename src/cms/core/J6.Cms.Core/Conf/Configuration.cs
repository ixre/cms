//
// Copyright (C) 2007-2008 Z3Q.NET,All rights reseved.
// 
// Project: Cms.Cms
// FileName : ApplicationManager.cs
// author : PC-CWLIU (new.min@msn.com)
// Create : 2011/12/23 14:53:11
// Description :
//
// Get infromation of this software,please visit our site http://cms.z3q.net
//
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using J6.Cms.CacheService;
using J6.Cms.Infrastructure;
using J6.DevFw.Framework;
using J6.Cms.DataTransfer;
using J6.Cms.Domain.Interface.Content;
using J6.DevFw.Framework.IO;

namespace J6.Cms.Conf
{
    public class Configuration
    {

        /// <summary>
        /// 配置文件路径
        /// </summary>
        public static string CmsConfigFile;


        public static event CmsConfigureHandler OnCmsConfigure;


        /// <summary>
        /// 设置应用程序，如在过程中发生异常则重启并提醒！
        /// </summary>
        /// <returns>返回加载消息，如成功返回空</returns>
        public static void Configure()
        {
            if (OnCmsConfigure != null)
            {
                try
                {
                    OnCmsConfigure();
                }
                catch (Exception ex)
                {
                    HttpContext.Current.Server.ClearError();
                    HttpContext.Current.Response.Write("<div style=\"margin:50px auto;width:600px;font-size:14px;color:red;line-height:50px;\"><b style=\"font-size:25px\">500&nbsp;Server Error!</b> <br />" +
                                                       (ex ?? ex.InnerException).Message + "<br />问题出现的详细原因，请见：http://s.z3q.net/cms</div>");
                    HttpRuntime.UnloadAppDomain();

                }
            }
        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>返回加载消息，如成功返回空</returns>
        internal static string Load(string filePath)
        {
            CmsConfigFile = filePath;


            //从配置文件中加载
            SettingFile sf = new SettingFile(CmsConfigFile);
            Settings.loaded = true;
            bool settingChanged = false;

            //try
            // {
            Settings.License_NAME = sf.Contains("license_name") ? sf["license_name"] : "评估用户";
            Settings.License_KEY = sf.Contains("license_key") ? sf["license_key"] : String.Empty;
            Settings.SYS_AUTOWWW = sf.Contains("sys_autowww") && sf["sys_autowww"] == "true";         //自动WWW

            #region 读取模板选项

            Settings.TPL_UseFullPath = sf.Contains("tpl_usefullpath") ? sf["tpl_usefullpath"] == "true" : true;
            Settings.TPL_UseCompress = sf.Contains("tpl_usecompress") ? sf["tpl_usecompress"] == "true" : false;


            #endregion

            Settings.DB_TYPE = sf["db_type"];
            Settings.DB_CONN = sf["db_conn"];
            Settings.DB_PREFIX = sf["db_prefix"];

            Settings.MM_AVATAR_PATH = sf["mm_avatar_path"];


            /**************** 优化项 ******************/
            Settings.Opti_Debug = WebConfig.IsDebug();

            //缓存项
            if (sf.Contains("opti_IndexCacheSeconds"))
            {
                Int32.TryParse(sf["opti_IndexCacheSeconds"], out Settings.Opti_IndexCacheSeconds);
            }

            if (sf.Contains("opti_ClientCacheSeconds"))
            {
                Int32.TryParse(sf["opti_ClientCacheSeconds"], out  Settings.Opti_ClientCacheSeconds);
            }

            if (sf.Contains("Opti_GC_Collect_Interval"))
            {
                Int32.TryParse(sf["Opti_GC_Collect_Interval"], out  Settings.Opti_GC_Collect_Interval);
            }


            /**************** 静态服务器 ******************/

            if (sf.Contains("server_static"))
            {
                Settings.SERVER_STATIC = sf["server_static"];
                if (Settings.SERVER_STATIC.Length == 0)
                {
                    Settings.SERVER_STATIC = Server.DefaultStaticServer;
                }
            }
            else
            {
                sf.Set("server_static", Server.DefaultStaticServer);
                Settings.SERVER_STATIC = Server.DefaultStaticServer;
                settingChanged = true;
            }


            if (sf.Contains("server_upgrade"))
            {
                Settings.SERVER_UPGRADE = sf["server_upgrade"];
                if (Settings.SERVER_UPGRADE.Length == 0)
                {
                    Settings.SERVER_UPGRADE = Server.DefaultUpgradeServer;
                }
            }
            else
            {
                sf.Set("server_upgrade", Server.DefaultUpgradeServer);
                Settings.SERVER_UPGRADE = Server.DefaultUpgradeServer;
                settingChanged = true;
            }

            if (sf.Contains("server_static_enabled"))
            {
                Settings.SERVER_STATIC_ENABLED = sf["server_static_enabled"] == "true";
            }
            else
            {
                sf.Set("server_static_enabled", "false");
                settingChanged = true;
            }



            if (sf.Contains("sys_encode_conf"))
            {
                Settings.SYS_ENCODE_CONF_FILE = sf["sys_encode_conf"] == "true";
            }
            else
            {
                sf.Set("sys_encode_conf", Settings.SYS_ENCODE_CONF_FILE ? "true" : "false");
                settingChanged = true;
            }


            if (sf.Contains("sys_admin_tag"))
            {
                Settings.SYS_ADMIN_TAG = sf["sys_admin_tag"];
            }
            else
            {
                sf.Set("sys_admin_tag", Settings.SYS_ADMIN_TAG);
                settingChanged = true;
            }

            if (settingChanged) sf.Flush();


            return String.Empty;

            /*
        }
        catch (Exception ex)
        {
            const string strtpl = "配置文件不正确，请检查！位置:{0}。{1}";
            string _file = filePath.Replace("/", "\\").Replace("\\\\", "\\");

            return string.Format(strtpl, _file,
               ex.GetType() == typeof(ArgumentOutOfRangeException) ? "此错误可能因为缺少系统所需的配置而引发。" :
               string.Empty
               );
        }*/
        }



        private static SettingFile sf = null;


        public static void UpdateByPrefix(string prefix)
        {
            if (sf == null)
            {
                throw new Exception("请先调用Configuration.BeginWrite()");
            }
            lock (sf)
            {
                UpdateKeys(prefix);
            }
        }

        /// <summary>
        /// 更新资料
        /// </summary>
        /// <param name="prefix"></param>
        private static void UpdateKeys(string prefix)
        {
            switch (prefix)
            {
                case "sys":
                    sf["license_name"] = Settings.License_NAME;
                    sf["license_key"] = Settings.License_KEY;
                    sf["server_static_enabled"] = Settings.SERVER_STATIC_ENABLED ? "true" : "false";
                    sf["server_static"] = Settings.SERVER_STATIC;
                    sf["server_upgrade"] = Settings.SERVER_UPGRADE;
                    sf["sys_admin_tag"] = Settings.SYS_ADMIN_TAG;
                    sf["sys_encode_conf"] = Settings.SYS_ENCODE_CONF_FILE ? "true" : "false";

                    //301跳转
                    
                        sf["sys_autowww"] = Settings.SYS_AUTOWWW ? "true" : "false";

                    //虚拟路径
                    //if (!sf.Contains("sys_virthpath"))
                    //{
                    //    sf.Set("sys_virthpath", Settings.SYS_VIRTHPATH);
                    //}
                    //else
                    //{
                    //    sf["sys_virthpath"] = Settings.SYS_VIRTHPATH;
                    //}
                    break;

                case "db":
                    sf["db_prefix"] = Settings.DB_PREFIX;
                    break;

                case "tpl":

                    //压缩代码
                        sf.Set("tpl_usecompress", Settings.TPL_UseCompress ? "true" : "false");
                    //使用完整路径
                        sf.Set("tpl_usefullpath", Settings.TPL_UseFullPath ? "true" : "false");
               

                    Cms.Template.Register();

                    break;

                //优化
                case "opti":

                    WebConfig.SetDebug(Settings.Opti_Debug);

                    //缓存项
                        sf.Set("opti_IndexCacheSeconds", Settings.Opti_IndexCacheSeconds.ToString());
                        sf.Set("Opti_GC_Collect_Interval", Settings.Opti_GC_Collect_Interval.ToString());
                        sf.Set("opti_ClientCacheSeconds", Settings.Opti_ClientCacheSeconds.ToString());
                    break;
            }

            sf.Flush();
        }

        /// <summary>
        /// 从新加载
        /// </summary>
        public static void Renew()
        {
            if (OnCmsConfigure != null) OnCmsConfigure();
        }

        public static void BeginWrite()
        {
            bool isEncoded = FileEncoder.IsEncoded(CmsConfigFile, CmsVariables.FileEncodeHeader);
            if (isEncoded)
            {
                FileEncoder.DecodeFile(CmsConfigFile, CmsConfigFile, CmsVariables.FileEncodeHeader,
                    CmsVariables.FileEncodeToken);
            }

            sf = new SettingFile(CmsConfigFile);

            if (isEncoded)
            {
                FileEncoder.EncodeFile(CmsConfigFile, CmsConfigFile, CmsVariables.FileEncodeHeader,
                    CmsVariables.FileEncodeToken);
            }
        }

        public static void EndWrite()
        {
            sf.Flush();

            if (Settings.SYS_ENCODE_CONF_FILE)
            {
                FileEncoder.EncodeFile(CmsConfigFile, CmsConfigFile, CmsVariables.FileEncodeHeader,
                    CmsVariables.FileEncodeToken);
            }

            sf = null;
        }

        internal static void LoadRelatedIndent()
        {
            string relatedConf = String.Format("{0}{1}related_indent.conf", Cms.PyhicPath, CmsVariables.SITE_CONF_PATH);
            bool isExists = File.Exists(relatedConf);
            SettingFile sf = new SettingFile(relatedConf);
            var isModify = false;
            var isLoaded = false;
            IDictionary<int, RelateIndent> relatedIndents = ServiceCall.Instance.ContentService.GetRelatedIndents();

            IDictionary<int, RelateIndent> newIndents = new Dictionary<int, RelateIndent>(relatedIndents.Count);

            String key;
            String value;
            foreach (var relatedIndent in relatedIndents)
            {
                key = relatedIndent.Key.ToString();
                if (isExists && sf.Contains(key))
                {
                   value = sf.Get(key);
                    isLoaded = true;
                }
                else
                {
                    value = relatedIndent.Value.ToString();
                    sf.Set(key, value);
                    isModify = true;
                }
                newIndents.Add(relatedIndent.Key,new RelateIndent(value));
            }

            if (isModify)
            {
                sf.Flush();
            }

            if (isLoaded)
            {
                ServiceCall.Instance.ContentService.SetRelatedIndents(newIndents);
            }
        }


        internal static void LoadCmsConfig()
        {
            //初始化设置
            string cmsConfigFile = String.Format("{0}{1}cms.conf", Cms.PyhicPath, CmsVariables.SITE_CONF_PATH);
            FileInfo cfgFile = new FileInfo(cmsConfigFile);
            if (cfgFile.Exists)
            {
                bool isEncoded = FileEncoder.IsEncoded(cmsConfigFile, CmsVariables.FileEncodeHeader);
                if (isEncoded)
                {
                    FileEncoder.DecodeFile(cmsConfigFile, cmsConfigFile, CmsVariables.FileEncodeHeader,
                        CmsVariables.FileEncodeToken);
                }

                Configuration.Load(cmsConfigFile);

                if (isEncoded || Settings.SYS_ENCODE_CONF_FILE)
                {
                    FileEncoder.EncodeFile(cmsConfigFile, cmsConfigFile, CmsVariables.FileEncodeHeader,
                        CmsVariables.FileEncodeToken);
                }
            }
            else
            {
                throw new Exception("CMS配置文件不存在");
            }
        }
    }
}
