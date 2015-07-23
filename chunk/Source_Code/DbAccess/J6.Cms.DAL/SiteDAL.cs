using J6.Cms.Domain.Interface.Site;
using J6.DevFw.Data;

namespace J6.Cms.Dal
{
    public class SiteDal : DalBase
    {
        public int CreateSite(ISite site)
        {
            base.ExecuteNonQuery(SqlQueryHelper.Format(DbSql.SiteCreateSite,
                new object[,]{
                    {"@name",site.Name},
                    {"@dirName",site.DirName},
                    {"@domain",site.Domain},
                    {"@location",site.Location},
                    {"@tpl",site.Tpl},
                    {"@language",site.Language},
                    {"@note",site.Note},
                    {"@seoTitle",site.SeoTitle},
                    {"@seoKeywords",site.SeoKeywords},
                    {"@seoDescription",site.SeoDescription},
                    {"@state",site.State},
                    {"@proTel",site.ProTel},
                    {"@proPhone",site.ProPhone},
                    {"@proFax",site.ProFax},
                    {"@proAddress",site.ProAddress},
                    {"@proEmail",site.ProEmail},
                    {"@proIm",site.ProIm},
                    {"@proPost",site.ProPost},
                    {"@proNotice",site.ProNotice},
                    {"@proSlogan",site.ProSlogan}
                }));

            int siteId = int.Parse(base.ExecuteScalar(SqlQueryHelper.Format(
            "SELECT MAX(site_id) FROM $PREFIX_site")).ToString());

            //初始化Root数据
            base.ExecuteNonQuery(
                SqlQueryHelper.Format(@"INSERT INTO $PREFIX_category 
                                        (site_id,page_title,moduleid,tag,name,page_keywords,page_description,lft,rgt,sort_number)
                                        VALUES(" + siteId + ",'ROOT',1,'root','根栏目','','',1,2,1)")
                );
            return siteId;
        }

        public void ReadSites(DataReaderFunc func)
        {
            base.ExecuteReader(new SqlQuery(base.OptimizeSql(base.DbSql.SiteGetSites)), func);
        }


        public int UpdateSite(ISite site)
        {
            return base.ExecuteNonQuery(
                SqlQueryHelper.Format(DbSql.SiteEditSite,
                new object[,]{
                    {"@siteId",site.Id},
                    {"@name",site.Name},
                    {"@dirName",site.DirName},
                    {"@tpl",site.Tpl},
                    {"@domain",site.Domain},
                    {"@location",site.Location},
                    {"@language",site.Language},
                    {"@note",site.Note},
                    {"@seoTitle",site.SeoTitle},
                    {"@seoKeywords",site.SeoKeywords},
                    {"@seoDescription",site.SeoDescription},
                    {"@state",site.State},
                    {"@proTel",site.ProTel},
                    {"@proPhone",site.ProPhone},
                    {"@proFax",site.ProFax},
                    {"@proAddress",site.ProAddress},
                    {"@proEmail",site.ProEmail},
                    {"@proIm",site.ProIm},
                    {"@proPost",site.ProPost},
                    {"@proNotice",site.ProNotice},
                    {"@proSlogan",site.ProSlogan}
                }));
        }
    }
}
