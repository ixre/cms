using T2.Cms.Domain.Interface.Models;

namespace T2.Cms.Domain.Interface._old
{
    public interface Imodule
    {
        bool AddModule(int siteID, string name);
        bool DeleteModule(int moduleID);
        System.Collections.Generic.IEnumerable<Module> GetAvailableModules();
        Models.Module GetModule(int moduleID);
        Models.Module GetModule(int siteID, string moduleName);
        string GetModuleJson();
        System.Collections.Generic.IList<Module> GetModules();
        System.Collections.Generic.IEnumerable<Module> GetSiteAvailableModules(int siteID);
        System.Collections.Generic.IEnumerable<Module> GetSiteModules(int siteID);
        bool UpdateModule(Models.Module module);
    }
}
