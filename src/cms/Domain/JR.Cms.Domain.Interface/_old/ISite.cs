using System;
using Spc.Models;

namespace Spc.Logic
{
    public interface ISite
    {
        bool CreateSite(Spc.Site site);
        Spc.Site DefaultSite { get; }
        System.Collections.Generic.IList<Spc.Site> GetAllSites();
        Spc.Site GetSite(int siteID);
        bool UpdateSite(Spc.Site site);
    }
}
