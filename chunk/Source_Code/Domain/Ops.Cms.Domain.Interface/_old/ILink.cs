using System;
using Spc.Models;
using Ops.Cms.Domain.Interface.Site.Link;

namespace Spc.Logic
{
    public interface ILink
    {
        void CreateLink(Spc.Models.Link link);
        bool Delete(int linkID);
        Spc.Models.Link Get(Func<Spc.Models.Link, bool> func);
        System.Collections.Generic.IEnumerable<Spc.Models.Link> GetAll();
        System.Collections.Generic.IEnumerable<Spc.Models.Link> GetLinks(LinkType type);
        System.Collections.Generic.IEnumerable<Spc.Models.Link> GetLinks(Func<Spc.Models.Link, bool> func);
        bool SetVisible(int linkID);
        void UpdateLink(Spc.Models.Link link);
    }
}
