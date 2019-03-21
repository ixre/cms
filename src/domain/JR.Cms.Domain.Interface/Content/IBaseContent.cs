using JR.Cms.Domain.Interface.Site.Category;
using JR.Cms.Infrastructure;

namespace JR.Cms.Domain.Interface.Content
{
    public interface IBaseContent:IAggregateroot
    {
        /// <summary>
        /// 链接管理
        /// </summary>
        IContentLinkManager LinkManager {get;}

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        Error Save();

        /// <summary>
        /// 下移排序
        /// </summary>
        void MoveSortDown();

        /// <summary>
        /// 上移排序
        /// </summary>
        void MoveSortUp();
    }
}
