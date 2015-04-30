using System.Collections.Generic;
using AtNet.Cms.Domain.Interface.Site;
using AtNet.Cms.Domain.Interface.Site.Category;
using AtNet.Cms.Domain.Interface.Site.Template;

namespace AtNet.Cms.ServiceRepository
{
    /// <summary>
    /// 
    /// </summary>
    internal static class RepositoryDataCache
    {
        public static IDictionary<int, ISite> _siteDict;
        public static IDictionary<int, IList<ICategory>> _categories;


        public static IList<ITemplateBind> _tplbind;

        /// <summary>
        /// 
        /// </summary>
        internal static void ClearTemplateBinds()
        {
            if (_tplbind == null) return;
            for (int i = 0; i < _tplbind.Count; i++)
            {
                _tplbind[i] = null;
            }
            _tplbind = null;
        }
    }
}
