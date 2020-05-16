//
// Copyright (C) 2007-2008 TO2.NET,All rights reserved.
// 
// Project: jr.Cms.Manager
// FileName : Plugin.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/10/18 16:20:58
// Description :
//
// Get infromation of this software,please visit our site http://to2.net/cms
//
//

using System;
using JR.Cms.Conf;
using JR.Stand.Core.Framework.Xml.AutoObject;
using JR.Stand.Core.PluginKernel;

namespace JR.Cms.Web.Manager.Handle
{
    /// <summary>
    /// 插件管理
    /// </summary>
    public class PluginHandler : BasePage
    {
        #region 旧版插件

        /// <summary>
        /// 批量替换文档标签
        /// </summary>
        public void ReplaceTags()
        {
            RenderTemplate(ResourceMap.ArchiveTagReplace, null);
        }

        public void ReplaceTags_POST()
        {
            //foreach (Archive archive in CmsLogic.Archive.GetAllArchives().ToEntityList<Archive>())
            //{
            //    //archive.Content = new TagsManager().Replace("/tags?t={0}", archive.Content);
            //    CmsLogic.Archive.Update(archive);
            //}
        }

        #endregion


        public void Dashboard()
        {
            RenderTemplate(
                ResourceMap.GetPageContent(ManagementPage.Plugin_Dashboard),
                null);
        }

        /// <summary>
        /// 供首页调用
        /// </summary>
        public void MiniApps()
        {
            RenderTemplate(
                ResourceMap.GetPageContent(ManagementPage.Plugin_MiniApps),
                null);
        }

        /// <summary>
        /// 获取插件Json数据
        /// </summary>
        public void GetPluginsJson_Post()
        {
            var jsonStr = "";
            var xml = new AutoObjectXml(
                string.Concat(Cms.PhysicPath,
                    CmsVariables.PLUGIN_META_FILE));
            var json = xml.GetObject("plugin_json_data");

            if (json == null)
            {
                jsonStr = XmlObject.ToJson(xml.GetObjects());
                xml.InsertObjectNode("plugin_json_data", "插件Json数据", jsonStr);
                xml.Flush();
            }
            else
            {
                jsonStr = json.Descript;
            }

            Response.WriteAsync(jsonStr);
        }

        /// <summary>
        /// 获取图标
        /// </summary>
        public void GetIcon()
        {
            throw new NotImplementedException();
            // string workIndent=base.Request.Query("worker_indent");
            //
            // byte[] data=Cms.Plugins.Manager.GetPluginIcon(workIndent,80,80);
            //
            // if(data!=null)
            // {
            // 	base.Response.BinaryWrite(data);
            // 	base.Response.ContentType="Image/Png";
            // }
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        public void ChangeState_POST()
        {
            throw new NotImplementedException();

            // string workIndent=base.Request.Query("worker_indent");
            // PluginPackAttribute attr;
            // PluginUtil.GetPlugin(workIndent,out attr);
            //
            // attr.State=attr.State== PluginState.Normal?PluginState.Stop:PluginState.Normal;
            //
            // //Cms.Plugins.Manager.Pause(workIndent);
            //
            // //更新元数据
            // Cms.Plugins.Manager.SavePluginMetadataToXml();

            RenderSuccess();
        }

        /// <summary>
        /// 安装/更新
        /// </summary>
        public void Install_POST()
        {
            string url = Request.Query("url");
            var result = PluginUtil.InstallPlugin(url, null);
            if (result)
                RenderSuccess();
            else
                RenderError("升级失败!");
        }

        public void Uninstall_POST()
        {
            string workIndent = Request.Query("worker_indent");
            var result = PluginUtil.RemovePlugin(workIndent);
            if (result)
                RenderSuccess();
            else
                RenderError("卸载失败!");
        }

        public string Shop()
        {
            return "当前系统不支持插件商店！请购买专业版或高级版！";
        }
    }
}