using System;
using AtNet.Cms.Domain.Interface.Site.Category;

namespace AtNet.Cms.Domain.Interface
{
    [Serializable]
    public delegate void CategoryTreeHandler(ICategory category, int level);
}
