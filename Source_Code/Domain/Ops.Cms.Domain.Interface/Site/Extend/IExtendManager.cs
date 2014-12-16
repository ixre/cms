using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ops.Cms.Domain.Interface.Site.Extend
{
    public interface IExtendManager
    {
        int SiteId { get; }
        IList<IExtendField> GetAllExtends();
        //<IExtendField> GetExtends(bool enabled);
        int SaveExtendField(IExtendField extendField);
        bool DeleteExtendAttr(int extendFieldId);
        IExtendField GetExtendField(int extendId);
        IExtendField GetExtendFieldByName(string extendName);
    }
}
