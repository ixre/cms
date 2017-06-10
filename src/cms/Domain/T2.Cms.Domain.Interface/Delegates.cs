using System;
using T2.Cms.Domain.Interface.Site.Category;

namespace T2.Cms.Domain.Interface
{
    [Serializable]
    public delegate void CategoryTreeHandler(ICategory category, int level,bool isLast);
}
