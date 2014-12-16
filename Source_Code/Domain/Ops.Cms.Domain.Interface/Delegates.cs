using Ops.Cms.Domain.Interface.Site.Category;
using System;

namespace Ops.Cms.Domain.Interface
{
    [Serializable]
    public delegate void CategoryTreeHandler(ICategory category, int level);
}
