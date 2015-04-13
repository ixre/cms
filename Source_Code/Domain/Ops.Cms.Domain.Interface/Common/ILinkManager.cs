using System.Collections.Generic;

namespace AtNet.Cms.Domain.Interface.Common
{
    public interface ILinkManager
    {
        /// <summary>
        /// 类型标识
        /// </summary>
        string TypeIndent { get; }

        /// <summary>
        /// 是否包含
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        bool Contain(ILink link);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="linkId"></param>
        /// <param name="name"></param>
        /// <param name="title"></param>
        /// <param name="uri"></param>
        /// <param name="enabled"></param>
        void Add(int linkId,string name, string title, string uri, bool enabled);

        /// <summary>
        /// 获取关联的链接
        /// </summary>
        /// <returns></returns>
        IList<ILink> GetRelatedLinks();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ILink GetLinkById(int id);

        ///// <summary>
        ///// 获取被关链的链接
        ///// </summary>
        ///// <returns></returns>
        //IList<ILink> GetRevertRelatedLinks();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="link"></param>
        void Remove(ILink link);

        /// <summary>
        /// 
        /// </summary>
        void SaveLinks();
    }
}