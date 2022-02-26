using System.Collections.Generic;
using JR.Cms.Domain.Interface.Models;
using JR.Cms.Infrastructure;
using JR.Stand.Core.Extensions;

namespace JR.Cms.Dao
{
    /// <summary>
    /// 站点标签
    /// </summary>
    public interface ISiteTagDao
    {
        /// <summary>
        /// 获取所有的标签,并排序
        /// </summary>
        /// <returns></returns>
        List<SiteWord> GetTags();

        /// <summary>
        /// 保存标签
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        Error SaveTag(SiteWord word);

        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        Error DeleteTag(SiteWord word);
    }
}