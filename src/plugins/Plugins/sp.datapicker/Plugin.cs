using JR.DevFw.PluginKernel;
using JR.Cms.Web.WebManager;
using System;
using System.Web;
using JR.DevFw.Toolkit.NetCrawl;
using JR.DevFw.Web.Plugin;

namespace sp.datapicker
{
    public class Plugin:IPlugin
    {
        private WebManage handler;
        private PluginPackAttribute _attr;

        public PluginConnectionResult Connect(IPluginHost _app)
        {
            IPluginApp app = _app as IPluginApp;
            if (app != null)
            {
                //注册
                app.Register(this, app_OnExtendModuleRequest, app_OnExtendModulePost);
                app.MapPluginRoute(this, "admin/dpicker");
            }

            return PluginConnectionResult.Success;
        }

        private void app_OnExtendModulePost(HttpContext t, ref bool b)
        {
            int siteId = CmsWebMaster.CurrentManageSite.SiteId;
            if (siteId <= 0)
            {
                b = false; return;
            }
            string filePath = PluginUtil.GetAttribute(this).WorkSpace + "site_" + siteId.ToString() + ".conf";
            t.Handler = new CollectionExtend(Collector.Create(filePath));
            t.Handler.ProcessRequest(t);
            b = true;
        }

        private void app_OnExtendModuleRequest(HttpContext t, ref bool b)
        {
            int siteId = CmsWebMaster.CurrentManageSite.SiteId;
            if (siteId <= 0)
            {
                b = false; return;
            }
            string filePath = PluginUtil.GetAttribute(this).WorkSpace + "site_" + siteId.ToString() + ".conf";
            t.Handler = new CollectionExtend(Collector.Create(filePath));
            t.Handler.ProcessRequest(t);
            b = true;
        }

        public string GetMessage()
        {
            return "";
        }

        public bool Install()
        {
            return true;
        }

        public object Call(string method, params object[] parameters)
        {
            throw new NotImplementedException();
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


        public void Logln(string line)
        {
            Logger.Println(line);
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
