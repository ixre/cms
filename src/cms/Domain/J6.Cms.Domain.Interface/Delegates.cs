using System;
using J6.Cms.Domain.Interface.Site.Category;

namespace J6.Cms.Domain.Interface
{
    [Serializable]
    public delegate void CategoryTreeHandler(ICategory category, int level,bool isLast);
}
