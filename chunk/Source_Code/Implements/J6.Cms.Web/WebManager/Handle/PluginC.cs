//
// Copyright (C) 2007-2008 S1N1.COM,All rights reseved.
// 
// Project: J6.Cms.Manager
// FileName : Plugin.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/10/18 16:20:58
// Description :
//
// Get infromation of this software,please visit our site http://z3q.net/cms
//
//

using System;
using J6.Cms.Conf;
using J6.Cms.WebManager;
using J6.DevFw.Framework.Xml.AutoObject;
using J6.DevFw.PluginKernel;

namespace J6.Cms.Web.WebManager.Handle
{
    /// <summary>
    /// 插件管理
    /// </summary>
    public class PluginC:BasePage
    {

    	#region 旧版插件
        /// <summary>
        /// 批量替换文档标签
        /// </summary>
        public void ReplaceTags_GET()
        {
            base.RenderTemplate(ResourceMap.ArchiveTagReplace, null);
        }
        public void ReplaceTags_POST()
        {

            //foreach (Archive archive in CmsLogic.Archive.GetAllArchives().ToEntityList<Archive>())
            //{
            //    //archive.Content = new TagsManager().Replace("/tags?t={0}", archive.Content);
            //    CmsLogic.Archive.Update(archive);
            //}
        }

        public void DataPicker_GET()
        {
            base.RenderTemplate(ResourceMap.Datapicker, null);
        }

        public void CreateDataPickerProject_GET()
        {
            base.RenderTemplate(ResourceMap.Createdatapickerproject, null);
        }

        
        #endregion


        public void Dashboard_GET()
        {
            base.RenderTemplate(
                ResourceMap.GetPageContent(ManagementPage.Plugin_Dashboard),
                null);
        }

        /// <summary>
        /// 供首页调用
        /// </summary>
        public void MiniApps_Get()
        {
            base.RenderTemplate(
                ResourceMap.GetPageContent(ManagementPage.Plugin_MiniApps), 
                null);
        }
        
        /// <summary>
        /// 获取插件Json数据
        /// </summary>
        public void GetPluginsJson_Post()
        {
        	string jsonStr="";	
    		AutoObjectXml xml=new AutoObjectXml(
                String.Concat(J6.Cms.Cms.PyhicPath,
                CmsVariables.PLUGIN_META_FILE));
    		XmlObject json=xml.GetObject("plugin_json_data");
    		
    		if(json==null)
    		{
    			jsonStr=XmlObject.ToJson(xml.GetObjects());
    			xml.InsertObjectNode("plugin_json_data","插件Json数据",jsonStr);
    			xml.Flush();
    		}
    		else
    		{
    			jsonStr=json.Descript;
    		}
    		
    		base.Response.Write(jsonStr);
        }
        
        /// <summary>
        /// 获取图标
        /// </summary>
        public void GetIcon_GET()
        {
        	string workIndent=base.Request["worker_indent"];
        	
        	byte[] data=J6.Cms.Cms.Plugins.Manager.GetPluginIcon(workIndent,80,80);
        
        	if(data!=null)
        	{
        		base.Response.BinaryWrite(data);
        		base.Response.ContentType="Image/Png";
        	}
        }
        
        /// <summary>
        /// 切换状态
        /// </summary>
        public void ChangeState_POST()
        {
        	string workIndent=base.Request["worker_indent"];
        	PluginPackAttribute attr;
        	PluginUtil.GetPlugin(workIndent,out attr);
        	
        	attr.State=attr.State== PluginState.Normal?PluginState.Stop:PluginState.Normal;
        	
        	//Cms.Plugins.Manager.Pause(workIndent);
        	
        	//更新元数据
        	J6.Cms.Cms.Plugins.Manager.SavePluginMetadataToXml();
        	
        	base.RenderSuccess();
        }
        
        /// <summary>
        /// 安装/更新
        /// </summary>
        public void Install_POST()
        {
        	string url=base.Request["url"];
        	bool result=PluginUtil.InstallPlugin(url,null);
        	if(result){
        		
        	base.RenderSuccess();
        	}else{
        		base.RenderError("升级失败!");
        	}
        }
        
        public void Uninstall_POST()
        {
        	string workIndent=base.Request["worker_indent"];
        	bool result=PluginUtil.RemovePlugin(workIndent);
        	if(result){
        		
        	base.RenderSuccess();
        	}else{
        		base.RenderError("卸载失败!");
        	}
        }

        public string Shop_GET()
        {
            return "当前系统不支持插件商店！请购买专业版或高级版！";
        }
    }
}
