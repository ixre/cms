using System;
using System.Collections.Generic;
using JR.Cms.Domain.Interface.Site;
using JR.Stand.Core.Data;

namespace JR.Cms.Library.DataAccess.DAL
{
    public class SiteDal : DalBase
    {
        public int CreateSite(ISite ist)
        {
            var site = ist.Get();
            IDictionary<string, object> data = new Dictionary<string, object>();
            data.Add("@name", site.Name);
            data.Add("@appName", site.AppPath);
            data.Add("@domain", site.Domain);
            data.Add("@location", site.Location);
            data.Add("@tpl", site.Tpl);
            data.Add("@language", site.Language);
            data.Add("@note", site.Note);
            data.Add("@seoTitle", site.SeoTitle);
            data.Add("@seoKeywords", site.SeoKeywords);
            data.Add("@seoDescription", site.SeoDescription);
            data.Add("@state", site.State);
            data.Add("@proTel", site.ProTel);
            data.Add("@proPhone", site.ProPhone);
            data.Add("@proFax", site.ProFax);
            data.Add("@proAddress", site.ProAddress);
            data.Add("@proEmail", site.ProEmail);
            data.Add("@proIm", site.ProIm);
            data.Add("@proPost", site.ProPost);
            data.Add("@proNotice", site.ProNotice);
            data.Add("@proSlogan", site.ProSlogan);
            ExecuteNonQuery(CreateQuery(DbSql.SiteCreateSite, data));
            return int.Parse(ExecuteScalar(NewQuery(
                "SELECT MAX(site_id) FROM $PREFIX_site", null)).ToString());
        }

        public void ReadSites(DataReaderFunc func)
        {
            ExecuteReader(NewQuery(DbSql.SiteGetSites, null), func);
        }


        public int UpdateSite(ISite ist)
        {
            var site = ist.Get();
            IDictionary<string, object> data = new Dictionary<string, object>();
            data.Add("@name", site.Name);
            data.Add("@appName", site.AppPath);
            data.Add("@domain", site.Domain);
            data.Add("@location", site.Location);
            data.Add("@tpl", site.Tpl);
            data.Add("@language", site.Language);
            data.Add("@note", site.Note);
            data.Add("@seoTitle", site.SeoTitle);
            data.Add("@seoKeywords", site.SeoKeywords);
            data.Add("@seoDescription", site.SeoDescription);
            data.Add("@state", site.State);
            data.Add("@proTel", site.ProTel);
            data.Add("@proPhone", site.ProPhone);
            data.Add("@proFax", site.ProFax);
            data.Add("@proAddress", site.ProAddress);
            data.Add("@proEmail", site.ProEmail);
            data.Add("@proIm", site.ProIm);
            data.Add("@proPost", site.ProPost);
            data.Add("@proNotice", site.ProNotice);
            data.Add("@proSlogan", site.ProSlogan);
            data.Add("@siteId", site.SiteId);
            return ExecuteNonQuery(NewQuery(DbSql.SiteEditSite, Db.GetDialect().ParseParameters(data)));
        }

        /// <summary>
        /// 初始化Root数据
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="rootCategoryId"></param>
        /// <returns></returns>
        public bool InitRootCategory(int siteId, int rootCategoryId)
        {
            return ExecuteNonQuery(
                NewQuery(@"INSERT INTO $PREFIX_category 
                                        (id,site_id,page_title,tag,name,page_keywords,page_description,lft,rgt,sort_number)
                                        VALUES(" + rootCategoryId.ToString() + "," + siteId.ToString() +
                         ",'ROOT','root','根栏目','','',1,2,1)", null)) == 1;
        }
    }
}