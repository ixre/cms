//
// FLinkBLL.cs   友情链接逻辑
// Copryright 2011 @ OPS Inc,All rights reseved !
// Create by newmin @ 2011/03/13
//
namespace Spc.BLL
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Data.Extensions;
    using Spc.DAL;
    using Spc.IDAL;
    using Spc.Models;
    using Ops.Cms.Domain.Interface.Site.Link;

    /// <summary>
    /// 友情链接逻辑
    /// </summary>
    public sealed class LinkBLL : Spc.Logic.ILink
    {
        private LinkDAL dal = new LinkDAL();

        /// <summary>
        /// 获取所有链接
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Link> GetAll()
        {
            return WeakRefCache.Links;
        }

        /// <summary>
        /// 获取指定类型的链接
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<Link> GetLinks(LinkType type)
        {
            foreach (Link link in GetAll())
            {
                if (link.Visible &&link.Type == (int)type) yield return link;
            }
        }

        /// <summary>
        /// 查询链接
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public IEnumerable<Link> GetLinks(Func<Link, bool> func)
        {
            foreach (Link link in GetAll())
            {
                if (func(link)) yield return link;
            }
        }

        /// <summary>
        /// 获取链接
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public Link Get(Func<Link, bool> func)
        {
            foreach (Link link in GetAll())
            {
                if (func(link)) return link;
            }
            return null;
        }

        /// <summary>
        /// 设置链接是否显示
        /// </summary>
        /// <param name="linkID"></param>
        public bool SetVisible(int linkID)
        {
            bool result = false;
            Link link = Get(a => a.ID == linkID);
            if (link != null)
            {
                link.Visible = !link.Visible;
                result = dal.SetVisible(linkID, link.Visible) == 1;
                if (result)
                {
                    WeakRefCache.RebuiltLinks();
                }
            }
            return result;
        }

        /// <summary>
        /// 添加链接
        /// </summary>
        public void CreateLink(Link link)
        {
            dal.Add(link);
            WeakRefCache.RebuiltLinks();
        }

        /// <summary>
        /// 更新链接
        /// </summary>
        /// <param name="link"></param>
        public void UpdateLink(Link link)
        {
            if (dal.Update(link) == 1)
            {
                WeakRefCache.RebuiltLinks();
            }
        }

        /// <summary>
        /// 删除链接
        /// </summary>
        /// <param name="linkID"></param>
        /// <returns></returns>
        public bool Delete(int linkID)
        {
            int result = dal.Delete(linkID);
            WeakRefCache.RebuiltLinks();
            return result == 1;
        }
    }
}