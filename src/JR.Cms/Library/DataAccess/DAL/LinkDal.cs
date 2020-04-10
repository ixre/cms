//
// LinkDAL   友情链接数据访问层
// Copryright 2011 @ TO2.NET,All rights reserved !
// Create by newmin @ 2011/03/13
//

using System;
using System.Collections.Generic;
using JR.Cms.Domain.Interface.Content;
using JR.Cms.Domain.Interface.Site.Link;
using JR.Stand.Core.Data;

namespace JR.Cms.Library.DataAccess.DAL
{
    /// <summary>
    /// 友情链接数据访问
    /// </summary>
    public sealed class LinkDal : DalBase
    {
        /// <summary>
        /// 获取所有的友情链接
        /// </summary>
        /// <returns></returns>
        public void GetAllSiteLinks(int siteId, SiteLinkType type, DataReaderFunc func)
        {
            var data = new object[,]
            {
                {"@siteId", siteId},
                {"@linkType", (int) type}
            };
            var query = SqlQueryHelper.Format(DbSql.Link_GetSiteLinksByLinkType, data);
            ExecuteReader(query, func);
        }

        public int AddSiteLink(int siteId, ISiteLink link)
        {
            return ExecuteNonQuery(NewQuery(DbSql.Link_AddSiteLink,
                Db.CreateParametersFromArray(
                    new object[,]
                    {
                        {"@siteId", siteId},
                        {"@pid", link.Pid},
                        {"@TypeId", link.Type},
                        {"@Text", link.Text},
                        {"@Uri", link.Uri},
                        {"@imgurl", link.ImgUrl},
                        {"@visible", link.Visible},
                        {"@Target", link.Target},
                        {"@sortNumber", link.SortNumber},
                        {"@bind", link.Bind}
                    })));
        }


        public int UpdateSiteLink(int siteId, ISiteLink link)
        {
            return ExecuteNonQuery(
                NewQuery(DbSql.Link_UpdateSiteLink,
                    Db.CreateParametersFromArray(
                        new object[,]
                        {
                            {"@siteId", siteId},
                            {"@pid", link.Pid},
                            {"@TypeId", link.Type},
                            {"@Text", link.Text},
                            {"@Uri", link.Uri},
                            {"@imgurl", link.ImgUrl},
                            {"@Target", link.Target},
                            {"@sortNumber", link.SortNumber},
                            {"@visible", link.Visible},
                            {"@LinkId", link.GetDomainId()},
                            {"@bind", link.Bind}
                        })));
        }

        public int DeleteSiteLink(int siteId, int linkId)
        {
            return ExecuteNonQuery(
                NewQuery(DbSql.Link_DeleteSiteLink,
                    Db.CreateParametersFromArray(
                        new object[,]
                        {
                            {"@siteId", siteId},
                            {"@LinkId", linkId}
                        })));
        }


        public void GetSiteLinkById(int siteId, int linkId, DataReaderFunc func)
        {
            ExecuteReader(
                SqlQueryHelper.Format(DbSql.Link_GetSiteLinkById, new object[,]
                {
                    {"@siteId", siteId},
                    {"@linkId", linkId}
                }), func);
        }

        public void ReadLinksOfContent(string contentType, int contentId, DataReaderFunc func)
        {
            ExecuteReader(
                SqlQueryHelper.Format(DbSql.Link_GetRelatedLinks, new object[,]
                {
                    {"@contentType", contentType},
                    {"@contentId", contentId}
                }), func);
        }

        public void SaveLinksOfContent(string contentType, int contentId, IList<IContentLink> list)
        {
            if (list.Count == 0) return;
            var querys = new SqlQuery[list.Count];

            var i = 0;
            foreach (var link in list)
                querys[i++] = SqlQueryHelper.Format(
                    link.Id <= 0 ? DbSql.Link_InsertRelatedLink : DbSql.Link_UpdateRelatedLink,
                    new object[,]
                    {
                        {"@contentType", contentType},
                        {"@contentId", contentId},
                        {"@id", link.Id},
                        {"@relatedSiteId", link.RelatedSiteId},
                        {"@relatedContentId", link.RelatedContentId},
                        {"@relatedIndent", link.RelatedIndent},
                        {"@enabled", link.Enabled}
                    });

            ExecuteMultiNonQuery(querys);
        }

        public void RemoveRelatedLinks(string contenType, int contentId, string ids)
        {
            var sql = String.Format(DbSql.Link_RemoveRelatedLinks, ids);
            ExecuteNonQuery(  SqlQueryHelper.Format(sql, new object[,]
                {
                    {"@contentType", contenType},
                    {"@contentId", contentId}
                }));
        }
    }
}