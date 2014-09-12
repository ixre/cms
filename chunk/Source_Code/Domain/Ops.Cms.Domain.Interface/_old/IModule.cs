namespace Ops.Cms.Domain.Interface._old
{
    public interface Imodule
    {
        bool AddModule(int siteID, string name);
        bool DeleteModule(int moduleID);
        System.Collections.Generic.IEnumerable<Spc.Models.Module> GetAvailableModules();
        Spc.Models.Module GetModule(int moduleID);
        Spc.Models.Module GetModule(int siteID, string moduleName);
        string GetModuleJson();
        System.Collections.Generic.IList<Spc.Models.Module> GetModules();
        System.Collections.Generic.IEnumerable<Spc.Models.Module> GetSiteAvailableModules(int siteID);
        System.Collections.Generic.IEnumerable<Spc.Models.Module> GetSiteModules(int siteID);
        bool UpdateModule(Spc.Models.Module module);
    }
}
