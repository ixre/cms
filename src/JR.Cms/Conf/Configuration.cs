//
// Copyright (C) 2007-2008 TO2.NET,All rights reserved.
// 
// Project: Cms.Cms
// FileName : ApplicationManager.cs
// author : PC-CWLIU (new.min@msn.com)
// Create : 2011/12/23 14:53:11
// Description :
//
// Get infromation of this software,please visit our site http://to2.net/jr-cms
//
//

using System;
using System.Collections.Generic;
using System.IO;
using JR.Cms.Domain.Interface.Content;
using JR.Cms.Infrastructure;
using JR.Cms.Library.CacheService;
using JR.Stand.Core.Framework;
using JR.Stand.Core.Framework.IO;

namespace JR.Cms.Conf
{
    public class Configuration
    {
        /// <summary>
        /// 配置文件路径
        /// </summary>
        public static string cmsConfFile;
        public static event CmsConfigureHandler OnCmsConfigure;

        /// <summary>
        /// 设置应用程序，如在过程中发生异常则重启并提醒！
        /// </summary>
        /// <returns>返回加载消息，如成功返回空</returns>
        public static void Configure()
        {
            // try
            // {
            OnCmsConfigure?.Invoke();
            // }
            // catch (Exception ex)
            // {
            //     
            // HttpContext.Current.Server.ClearError();
            // HttpContext.Current.Response.Write("<div style=\"margin:50px auto;width:600px;font-size:14px;color:red;line-height:50px;\"><b style=\"font-size:25px\">500&nbsp;Server Error!</b> <br />" +
            //                                    (ex ?? ex.InnerException).Message + "<br />问题出现的详细原因，请见：http://s.to2.net/cms</div>");
            // HttpRuntime.UnloadAppDomain();

            //}
        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>返回加载消息，如成功返回空</returns>
        private static string Load(string filePath)
        {
            cmsConfFile = filePath;
            //从配置文件中加载
            SettingFile sf = new SettingFile(cmsConfFile);
            Settings.loaded = true;
            bool settingChanged = false;

            //try
            // {
            Settings.LICENSE_NAME = sf.Contains("license_name") ? sf["license_name"] : "评估用户";
            Settings.LICENSE_KEY = sf.Contains("license_key") ? sf["license_key"] : String.Empty;
            Settings.SYS_WWW_RD = GetInt(sf,"sys_www_rd");       
            Settings.SYS_FORCE_HTTPS = CheckTrueValue(sf,"sys_force_https"); 
            Settings.SYS_USE_UPLOAD_RAW_NAME = CheckTrueValue(sf, "sys_use_upload_raw_path");

            #region 读取模板选项

            Settings.TPL_FULL_URL_PATH = !sf.Contains("tpl_full_url_path") || sf["tpl_full_url_path"] == "true";
            Settings.TPL_USE_COMPRESS = sf.Contains("tpl_use_compress") && sf["tpl_use_compress"] == "true";


            #endregion

            Settings.DB_TYPE = sf["db_type"];
            Settings.DB_CONN = sf["db_conn"];
            Settings.DB_PREFIX = sf["db_prefix"];

            Settings.MM_AVATAR_PATH = sf["mm_avatar_path"];


            /**************** 优化项 ******************/
            Settings.OPTI_DEBUG_MODE = WebConfig.IsDebug();

            //缓存项
            if (sf.Contains("opti_index_cache_seconds"))
            {
                Int32.TryParse(sf["opti_index_cache_seconds"], out Settings.Opti_IndexCacheSeconds);
            }

            if (sf.Contains("opti_client_cache_seconds"))
            {
                Int32.TryParse(sf["opti_client_cache_seconds"], out Settings.Opti_ClientCacheSeconds);
            }

            if (sf.Contains("opti_gc_collect_interval"))
            {
                Int32.TryParse(sf["opti_gc_collect_interval"], out Settings.opti_gc_collect_interval);
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

            if (sf.Contains("sql_profile_trace"))
            {
                Settings.SQL_PROFILE_TRACE = sf["sql_profile_trace"] == "true";
            }
            else
            {
                sf.Set("sql_profile_trace", Settings.SQL_PROFILE_TRACE ? "true" : "false");
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
        }

        private static int GetInt(SettingFile s, string key)
        {
            if (!s.Contains(key)) return 0;
            int.TryParse(s[key], out var value);
            return value;
        }

        /// <summary>
        /// 判断设置项的值是否为真
        /// </summary>
        /// <param name="sf"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static bool CheckTrueValue(SettingFile sf, string key)
        {
            return sf.Contains(key) && sf.Get(key) == "true";
        }

        private static SettingFile sf = null;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        /// <exception cref="Exception"></exception>
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
                    sf["license_name"] = Settings.LICENSE_NAME;
                    sf["license_key"] = Settings.LICENSE_KEY;
                    sf["server_static_enabled"] = Settings.SERVER_STATIC_ENABLED ? "true" : "false";
                    sf["server_static"] = Settings.SERVER_STATIC;
                    sf["server_upgrade"] = Settings.SERVER_UPGRADE;
                    sf["sys_admin_tag"] = Settings.SYS_ADMIN_TAG;
                    sf["sys_encode_conf"] = Settings.SYS_ENCODE_CONF_FILE ? "true" : "false";
                    sf["sql_profile_trace"] = Settings.SQL_PROFILE_TRACE ? "true" : "false";
                    sf.Set("sys_use_upload_raw_path", Settings.SYS_USE_UPLOAD_RAW_NAME?"true":"false");
                    sf["sys_www_rd"] = Settings.SYS_WWW_RD.ToString();
                    sf.Set("sys_force_https", Settings.SYS_FORCE_HTTPS?"true":"false");
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
                    sf.Set("tpl_use_compress", Settings.TPL_USE_COMPRESS ? "true" : "false");
                    //使用完整路径
                    sf.Set("tpl_full_url_path", Settings.TPL_FULL_URL_PATH ? "true" : "false");
                    Cms.Template.Reload();
                    break;

                //优化
                case "opti":
                    //缓存项
                    sf.Set("opti_index_cache_seconds", Settings.Opti_IndexCacheSeconds.ToString());
                    sf.Set("opti_gc_collect_interval", Settings.opti_gc_collect_interval.ToString());
                    sf.Set("opti_client_cache_seconds", Settings.Opti_ClientCacheSeconds.ToString());
                    break;
            }
            sf.Flush();
            if (prefix == "opti")
            {
                WebConfig.SetDebug(Settings.OPTI_DEBUG_MODE);
            }

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
            bool isEncoded = FileEncoder.IsEncoded(cmsConfFile, CmsVariables.FileEncodeHeader);
            if (isEncoded)
            {
                FileEncoder.DecodeFile(cmsConfFile, cmsConfFile, CmsVariables.FileEncodeHeader,
                    CmsVariables.FileEncodeToken);
            }

            sf = new SettingFile(cmsConfFile);

            if (isEncoded)
            {
                FileEncoder.EncodeFile(cmsConfFile, cmsConfFile, CmsVariables.FileEncodeHeader,
                    CmsVariables.FileEncodeToken);
            }
        }

        public static void EndWrite()
        {
            sf.Flush();

            if (Settings.SYS_ENCODE_CONF_FILE)
            {
                FileEncoder.EncodeFile(cmsConfFile, cmsConfFile, CmsVariables.FileEncodeHeader,
                    CmsVariables.FileEncodeToken);
            }

            sf = null;
        }

        /// <summary>
        /// 加载关联标识
        /// </summary>
        internal static void LoadRelatedIndent()
        {
            string relatedConf = String.Format("{0}{1}related_indent.conf", Cms.PhysicPath, CmsVariables.SITE_CONF_PATH);
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


        internal static void LoadCmsConfig(String confPath)
        {
            //初始化设置
            if (String.IsNullOrEmpty(confPath))
            {
                confPath = String.Format("{0}{1}cms.conf", Cms.PhysicPath, CmsVariables.SITE_CONF_PATH);
            }
            FileInfo cfgFile = new FileInfo(confPath);
            if (cfgFile.Exists)
            {
                bool isEncoded = FileEncoder.IsEncoded(confPath, CmsVariables.FileEncodeHeader);
                if (isEncoded)
                {
                    FileEncoder.DecodeFile(confPath, confPath, CmsVariables.FileEncodeHeader,
                        CmsVariables.FileEncodeToken);
                }
                Configuration.Load(confPath);
                if (isEncoded || Settings.SYS_ENCODE_CONF_FILE)
                {
                    FileEncoder.EncodeFile(confPath, confPath, CmsVariables.FileEncodeHeader,
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
