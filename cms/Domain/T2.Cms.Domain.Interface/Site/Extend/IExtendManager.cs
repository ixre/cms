using System.Collections.Generic;
using T2.Cms.Infrastructure;

namespace T2.Cms.Domain.Interface.Site.Extend
{
    public interface IExtendManager
    {
        int SiteId { get; }
        IList<IExtendField> GetAllExtends();
        //<IExtendField> GetExtends(bool enabled);
        Error SaveExtendField(IExtendField extendField);
        bool DeleteExtendAttr(int extendFieldId);
        IExtendField GetExtendField(int extendId);
        IExtendField GetExtendFieldByName(string extendName);
    }
}
