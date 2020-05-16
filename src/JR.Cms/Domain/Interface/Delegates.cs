using System;
using JR.Cms.Domain.Interface.Site.Category;

namespace JR.Cms.Domain.Interface
{
    [Serializable]
    public delegate void CategoryTreeHandler(ICategory category, int level, bool isLast);
}