//
// LinkDAL   友情链接数据访问层
// Copryright 2011 @ Z3Q.NET,All rights reseved !
// Create by newmin @ 2011/03/13
//

using System.Collections.Generic;
using J6.Cms.Domain.Interface.Common;
using J6.Cms.Domain.Interface.Content;
using J6.Cms.Domain.Interface.Site.Link;
using J6.DevFw.Data;

namespace J6.Cms.Dal
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
        public void GetAllSiteLinks(int siteId,SiteLinkType type,DataReaderFunc func)
        {
            base.ExecuteReader(
                SqlQueryHelper.Format(DbSql.Link_GetSiteLinksByLinkType,new object[,]{
                    {"@siteId",siteId},
                    {"@linkType",(int)type}
                }), func);
        }

        public int AddSiteLink(int siteId,ISiteLink link)
        {
            return base.ExecuteNonQuery(new SqlQuery(base.OptimizeSql(DbSql.Link_AddSiteLink),
                 new object[,]{
                {"@siteId",siteId},
                {"@pid",link.Pid},
                {"@TypeId", link.Type},
                {"@Text", link.Text},
                {"@Uri", link.Uri},
                {"@imgurl",link.ImgUrl},
                {"@visible",link.Visible},
                {"@Target", link.Target},
                {"@sortNumber", link.SortNumber},
                {"@bind",link.Bind}
                }));
        }


        public int UpdateSiteLink(int siteId,ISiteLink link)
        {
           return base.ExecuteNonQuery(
               new SqlQuery(base.OptimizeSql(DbSql.Link_UpdateSiteLink),
                    new object[,]{
                        {"@siteId",siteId},
                {"@pid",link.Pid},
                {"@TypeId", link.Type},
                {"@Text", link.Text},
                {"@Uri", link.Uri},
                {"@imgurl",link.ImgUrl},
                {"@Target", link.Target},
                {"@sortNumber", link.SortNumber},
                {"@visible",link.Visible},
                {"@LinkId",link.Id},
                {"@bind",link.Bind}
                    }));
        }

        public int DeleteSiteLink(int siteId,int linkId)
        {
            return base.ExecuteNonQuery(
                 new SqlQuery(base.OptimizeSql(DbSql.Link_DeleteSiteLink),
                 new object[,]{
                     {"@siteId",siteId},
                {"@LinkId", linkId}
                }));
        }


        public void GetSiteLinkById(int siteId, int linkId,DataReaderFunc func)
        {
            base.ExecuteReader(
              SqlQueryHelper.Format(DbSql.Link_GetSiteLinkById, new object[,]{
                    {"@siteId",siteId},
                    {"@linkId",linkId}
                }), func);
        }

        public void ReadLinksOfContent(string contentType, int contentId,DataReaderFunc func)
        {
            base.ExecuteReader(
              SqlQueryHelper.Format(DbSql.Link_GetRelatedLinks, new object[,]{
                    {"@contentType",contentType},
                    {"@contentId",contentId}
                }), func);
        }

        public void SaveLinksOfContent(string contentType, int contentId, IList<IContentLink> list)
        {
            if (list.Count == 0) return;
            SqlQuery[] querys = new SqlQuery[list.Count];

            int i = 0;
            foreach (IContentLink link in list)
            {
                    querys[i++] = SqlQueryHelper.Format(
                        (link.Id <= 0 ? DbSql.Link_InsertRelatedLink:DbSql.Link_UpdateRelatedLink),
                           new object[,]{
                        {"@contentType",contentType},
                        {"@contentId",contentId},
                        {"@id",link.Id},
                        {"@relatedContentId",link.RelatedContentId},
                        {"@relatedIndent",link.RelatedIndent},
                        {"@enabled",link.Enabled}
                       });
               
            }

            base.ExecuteNonQuery(querys);
        }

        public void RemoveRelatedLinks(string contenType, int contentId, string ids)
        {
            base.ExecuteNonQuery(
             SqlQueryHelper.Format(DbSql.Link_RemoveRelatedLinks, new object[,]{
                    {"@contentType",contenType},
                    {"@contentId",contentId}
                },ids));
        }
    }
}