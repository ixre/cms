using T2.Cms.Domain.Interface.Site;
using JR.DevFw.Data;

namespace T2.Cms.Dal
{
    public class SiteDal : DalBase
    {
        public int CreateSite(ISite site)
        {
            base.ExecuteNonQuery(base.NewQuery(DbSql.SiteCreateSite,
                                base.Db.CreateParametersFromArray(

                new object[,]
                {
                    {"@name", site.Name},
                    {"@dirName", site.DirName},
                    {"@domain", site.Domain},
                    {"@location", site.Location},
                    {"@tpl", site.Tpl},
                    {"@language", site.Language},
                    {"@note", site.Note},
                    {"@seoTitle", site.SeoTitle},
                    {"@seoKeywords", site.SeoKeywords},
                    {"@seoDescription", site.SeoDescription},
                    {"@state", site.State},
                    {"@proTel", site.ProTel},
                    {"@proPhone", site.ProPhone},
                    {"@proFax", site.ProFax},
                    {"@proAddress", site.ProAddress},
                    {"@proEmail", site.ProEmail},
                    {"@proIm", site.ProIm},
                    {"@proPost", site.ProPost},
                    {"@proNotice", site.ProNotice},
                    {"@proSlogan", site.ProSlogan}
                })));

            return int.Parse(base.ExecuteScalar(base.NewQuery(
                "SELECT MAX(site_id) FROM $PREFIX_site",null)).ToString());
        }

        public void ReadSites(DataReaderFunc func)
        {
            base.ExecuteReader(base.NewQuery(base.DbSql.SiteGetSites,null), func);
        }


        public int UpdateSite(ISite site)
        {
            return base.ExecuteNonQuery(
                base.NewQuery(DbSql.SiteEditSite,
                                base.Db.CreateParametersFromArray(

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
                })));
        }

        /// <summary>
        /// 初始化Root数据
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="rootCategoryId"></param>
        /// <returns></returns>
        public bool InitRootCategory(int siteId ,int rootCategoryId)
        {
            return base.ExecuteNonQuery(
               base.NewQuery(@"INSERT INTO $PREFIX_category 
                                        (id,site_id,page_title,tag,name,page_keywords,page_description,lft,rgt,sort_number)
                                        VALUES(" + rootCategoryId.ToString() + "," + siteId.ToString() +
                                      ",'ROOT','root','根栏目','','',1,2,1)",null)) == 1;
        }
    }
}
