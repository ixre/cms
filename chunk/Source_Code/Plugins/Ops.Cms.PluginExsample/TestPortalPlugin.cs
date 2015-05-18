using System;
using AtNet.Cms.Core.Plugins;
using AtNet.DevFw.PluginKernel;

namespace AtNet.Cms.PluginExample
{
    public class TestPortalPlugin : IPlugin
    {
        private PortalPlugin portal;

        public PluginConnectionResult Connect(IPluginHost app)
        {
            portal = app as PortalPlugin;
            portal.OnPortalRequest += PluginMethods.PortalRequest;
            return PluginConnectionResult.Success;
        }

        public string GetMessage()
        {
            throw new NotImplementedException();
        }

        public bool Install()
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            portal.OnPortalRequest -= PluginMethods.PortalRequest;
        }

        public void Run()
        {
        }

        public bool Uninstall()
        {
            throw new NotImplementedException();
        }


        public object Call(string method, params object[] parameters)
        {
            throw new NotImplementedException();
        }


        public void Logln(string line)
        {
            Logger.Println(line);
        }


        public PluginPackAttribute GetAttribute()
        {
            throw new NotImplementedException();
        }
    }
}