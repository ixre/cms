using System.Collections.Generic;
using JR.Cms.Domain.Interface.Models;
using JR.Cms.Infrastructure;

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
        List<SiteTag> GetTags();

        /// <summary>
        /// 保存标签
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        Error SaveTag(SiteTag tag);

        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        Error DeleteTag(SiteTag tag);
    }
}