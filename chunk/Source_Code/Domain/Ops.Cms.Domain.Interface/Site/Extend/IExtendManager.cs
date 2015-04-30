using System.Collections.Generic;

namespace AtNet.Cms.Domain.Interface.Site.Extend
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
