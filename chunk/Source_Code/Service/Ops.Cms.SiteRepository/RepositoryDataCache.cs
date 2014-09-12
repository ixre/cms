using Ops.Cms.Domain.Interface.Site;
using Ops.Cms.Domain.Interface.Site.Category;
using Ops.Cms.Domain.Interface.Site.Template;
using System.Collections.Generic;

namespace Ops.Cms.ServiceRepository
{
    /// <summary>
    /// 
    /// </summary>
    internal static class RepositoryDataCache
    {
        public static IDictionary<int, ISite> _siteDict;
        public static IDictionary<int, IList<ICategory>> _categories;
        public static IList<ITemplateBind> _tplBinds;

        /// <summary>
        /// 
        /// </summary>
        internal static void ClearTemplateBinds()
        {
            if (_tplBinds == null) return;
            for (int i = 0; i < _tplBinds.Count; i++)
            {
                _tplBinds[i] = null;
            }
            _tplBinds = null;
        }
    }
}
