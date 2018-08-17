using System;
using System.IO;
using T2.Cms.Conf;
using JR.DevFw.Framework.Xml.AutoObject;
using JR.DevFw.PluginKernel;

namespace T2.Cms.Core.Plugins
{
    /// <summary>
    /// CMS插件
    /// </summary>
    [PluginHost("CMS基础插件", "")]
	public class CmsPlugin : BasePluginHost
	{
		public CmsPlugin():base()
		{
			SavePluginMetadataToXml();
		}
		
		/// <summary>
		/// 保存插件数据到XML文件中
		/// </summary>
		public void SavePluginMetadataToXml()
		{
			string fileName=String.Concat(Cms.PyhicPath,CmsVariables.PLUGIN_META_FILE);
			
			if(File.Exists(fileName))
			{
				File.Delete(fileName);
			}
			
			AutoObjectXml xml=new AutoObjectXml(fileName);
			
			base.Iterate((p,a)=>{
			             	xml.InsertObjectNode(a.WorkIndent,a.Name,a.Description,
			             	                     new XmlObjectProperty("version","版本",a.Version),
			             	                     new XmlObjectProperty("state","状态",((int)a.State).ToString()),
			             	                     new XmlObjectProperty("author","作者",a.Author),
			             	                     new XmlObjectProperty("icon","图标",a.Icon),
			             	                     new XmlObjectProperty("webpage","官网",a.WebPage),
			             	                     new XmlObjectProperty("portalUrl","入口地址",a.PortalUrl),
			             	                     new XmlObjectProperty("configUrl","设置地址",a.ConfigUrl)
			             	                    );
			             	
			             });
			xml.Flush();
		}
		
		/// <summary>
		/// 获取插件的图标
		/// </summary>
		/// <param name="workerIndent"></param>
		/// <returns></returns>
		public byte[] GetPluginIcon(string workerIndent,int width,int height)
		{
			return PluginUtil.GetPluginIcon(workerIndent,width,height,String.Concat(Cms.PyhicPath,CmsVariables.PLUGIN_DEFAULT_ICON));
		}
	}
}
