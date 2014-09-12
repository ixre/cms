using System;
using System.Web;
using Com.PluginKernel;
using Ops.Cms;
using Ops.Cms.Core.Plugins;
using sp.xmlrpc.XmlRpc.src;
using Spc.XmlRpc;

namespace sp.xmlrpc
{
	/// <summary>
	/// 使用/test.sh/开始访问
	/// </summary>
	public class XmlRpcPluginCore:IPlugin
	{
		/// <summary>
		/// 插件访问模块名称（使用/test.sh/访问）
		/// </summary>
		const string moduleName="xmlrpc";
		internal static PluginPackAttribute attr;
		private static WeblogRPCService blogService;
		
		/// <summary>
		/// 初始化插件
		/// </summary>
		/// <param name="_app"></param>
		/// <returns></returns>
		public PluginConnectionResult Connect(IPluginHost _app)
		{
			IExtendApp app = _app as IExtendApp;
			if (app != null)
			{
				//注册
                app.Register(this,app_OnExtendModuleRequest, app_OnExtendModulePost);
                Cms.Plugins.MapExtendPluginRoute("xmlrpc", app.GetAttribute(this).WorkIndent);
			}
			
			blogService=new WeblogRPCService();
			
			//初始化设置
			attr=PluginUtil.GetAttribute<XmlRpcPluginCore>();
			bool isChanged=false;
			
			if(attr.Settings.Contains("enable_base64_image"))
			{
				WeblogRPCService.EnableBase64Images=attr.Settings["enable_base64_image"]=="yes";
			}
			else
			{
				WeblogRPCService.EnableBase64Images=false;
				attr.Settings.Append("enable_base64_image","no");
				isChanged=true;
			}
			
			if(isChanged)
			{
				attr.Settings.Flush();
			}
			
			return PluginConnectionResult.Success;
		}

		void app_OnExtendModuleRequest(HttpContext t,ref bool handled)
		{
            t.Handler = blogService;
			t.Handler.ProcessRequest(t);
			handled=true;
		}
		
		void app_OnExtendModulePost(HttpContext t,  ref bool handled)
		{
            t.Handler = blogService;
			t.Handler.ProcessRequest(t);
			handled=true;
		}

		
		#region IPlugin的成员
		
		public string GetMessage()
		{
			return "";
		}

		public bool Install()
		{
			return true;
		}

		public void Pause()
		{
		}

		public void Run()
		{
		}

		public bool Uninstall()
		{
			return false;
		}




		
		public object Call(string method, params object[] parameters)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
