using System.Collections.Generic;
using JR.Cms.Infrastructure;

namespace JR.Cms.Domain.Interface.Site.Extend
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
