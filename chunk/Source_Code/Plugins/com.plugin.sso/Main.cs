/**
 * Copyright (C) 2007-2015 K3F.NET,All rights reseved.
 * Get more infromation of this software,please visit site http://h3f.net/cms
 * 
 * name : Main.cs
 * author : newmin (new.min@msn.com)
 * date : 2012/12/01 23:00:00
 * description : 
 * history : 
 */

using J6.DevFw.PluginKernel;
using System;
using J6.DevFw.Web.Plugin;

namespace com.plugin.sso
{
	/// <summary>
	/// Description of Main.
	/// </summary>
	public class Main:IPlugin
	{
        private PluginPackAttribute _attr;
		public PluginConnectionResult Connect(IPluginHost app)
		{
			IPluginApp _app = app as IPluginApp;
			if(_app!=null)
            {
                Config.Init(this);

				RequestProxry req=new RequestProxry(_app,this);
				_app.Register(this,req.HandleGet,req.HandlePost);
                this.init();
			}


			return PluginConnectionResult.Success;
		}

        private void init()
        {
            this.Logln("Loaded OK!");
        }
		
		public bool Install()
		{
			return true;
		}
		
		public bool Uninstall()
		{
			return true;
		}
		
		public void Run()
		{
			
		}
		
		public void Pause()
		{
		}
		
		public string GetMessage()
		{
			return "";
		}
		
		public object Call(string method, params object[] parameters)
		{
			throw new NotImplementedException();
		}


        public void Logln(string line)
        {
            Logger.Println("[ SSO]:"+line);
        }


        public PluginPackAttribute GetAttribute()
        {
            if (this._attr == null)
            {
                this._attr = PluginUtil.GetAttribute(this);
            }
            return this._attr;
        }
    }
}
