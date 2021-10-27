using System;
using System.Collections.Generic;
using JR.Cms.Domain.Interface.Site;
using JR.Cms.Domain.Interface.Site.Variable;
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
            data.Add("@seoForceHttps",site.SeoForceHttps);
            data.Add("@seoForceRedirect",site.SeoForceRedirect);
            data.Add("@aloneBoard",site.AloneBoard);
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
            data.Add("@seoForceHttps",site.SeoForceHttps);
            data.Add("@seoForceRedirect",site.SeoForceRedirect);
            data.Add("@aloneBoard",site.AloneBoard);
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

        /// <summary>
        /// 添加变量
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="v"></param>
        public void AddVariable(int siteId, SiteVariable v)
        {
            IDictionary<string, object> data = new Dictionary<string, object>();
            data.Add("@name", v.Name);
            data.Add("@value", v.Remark);
            data.Add("@siteId", siteId);
            data.Add("@remark",v.Remark);
            ExecuteNonQuery(NewQuery(DbSql.CreateSiteVariable, Db.GetDialect().ParseParameters(data)));
            v.Id = int.Parse(ExecuteScalar(NewQuery(
                "SELECT MAX(id) FROM $PREFIX_site_variables", null)).ToString());
        }

        /// <summary>
        /// 保存变量
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="v"></param>
        public void UpdateVariable(int siteId, SiteVariable v)
        {
            IDictionary<string, object> data = new Dictionary<string, object>();
            data.Add("@id",v.Id);
            data.Add("@siteId", siteId);
            data.Add("@name", v.Name);
            data.Add("@value", v.Remark);
            data.Add("@remark",v.Remark);
             ExecuteNonQuery(NewQuery(DbSql.UpdateSiteVariable, Db.GetDialect().ParseParameters(data)));
        }

        /// <summary>
        /// 删除变量
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="varId"></param>
        public void DeleteVariable(int siteId, int varId)
        {
            IDictionary<string, object> data = new Dictionary<string, object>();
            data.Add("@id",varId);
            data.Add("@siteId", siteId);
            ExecuteNonQuery(NewQuery(DbSql.DeleteSiteVariable, Db.GetDialect().ParseParameters(data)));
        }

        /// <summary>
        /// 获取站点所有变量
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="func"></param>
        public void GetVariables(int siteId, DataReaderFunc func)
        {
            IDictionary<string, object> data = new Dictionary<string, object>();
            data.Add("@siteId", siteId);
            ExecuteReader(NewQuery(DbSql.GetSiteVariables,  Db.GetDialect().ParseParameters(data)), func);
        }
    }
}