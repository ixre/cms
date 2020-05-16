using System;
using JR.Cms.Domain.Interface.Models;
using JR.Cms.Domain.Interface.Site.Link;

namespace JR.Cms.Domain.Interface._old
{
    public interface ILink
    {
        void CreateLink(Link link);
        bool Delete(int linkId);
        Link Get(Func<Link, bool> func);
        System.Collections.Generic.IEnumerable<Link> GetAll();
        System.Collections.Generic.IEnumerable<Link> GetLinks(SiteLinkType type);
        System.Collections.Generic.IEnumerable<Link> GetLinks(Func<Link, bool> func);
        bool SetVisible(int linkId);
        void UpdateLink(Link link);
    }
}
