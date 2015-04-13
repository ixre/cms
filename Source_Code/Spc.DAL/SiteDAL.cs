using AtNet.Cms.Domain.Interface.Site;
using AtNet.DevFw.Data;

namespace AtNet.Cms.DAL
{
    public class SiteDAL : DALBase
    {
        public int CreateSite(ISite site)
        {
            base.ExecuteNonQuery(SqlQueryHelper.Format(SP.Site_CreateSite,
                new object[,]{
                    {"@name",site.Name},
                    {"@dirname",site.DirName},
                    {"@domain",site.Domain},
                    {"@location",site.Location},
                    {"@tpl",site.Tpl},
                    {"@language",site.Language},
                    {"@note",site.Note},
                    {"@seotitle",site.SeoTitle},
                    {"@seokeywords",site.SeoKeywords},
                    {"@seodescription",site.SeoDescription},
                    {"@state",site.State},
                    {"@protel",site.Tel},
                    {"@prophone",site.Phone},
                    {"@profax",site.Fax},
                    {"@proaddress",site.Address},
                    {"@proemail",site.Email},
                    {"@im",site.Im},
                    {"@postcode",site.PostCode},
                    {"@pronotice",site.Notice},
                    {"@proslogan",site.Slogan}
                }));

            int siteId = int.Parse(base.ExecuteScalar(SqlQueryHelper.Format(
            "SELECT MAX(siteid) FROM $PREFIX_site")).ToString());

            //初始化Root数据
            base.ExecuteNonQuery(
                SqlQueryHelper.Format(@"INSERT INTO $PREFIX_category 
                                        (siteid,pagetitle,moduleid,tag,name,keywords,description,lft,rgt,orderindex)
                                        VALUES(" + siteId + ",'ROOT',1,'root','根栏目','','',1,2,1)")
                );
            return siteId;
        }

        public void LoadSites(DataReaderFunc func)
        {
            base.ExecuteReader(new SqlQuery(base.OptimizeSQL(SP.Site_GetSites)), func);
        }


        public int UpdateSite(ISite site)
        {
            return base.ExecuteNonQuery(
                SqlQueryHelper.Format(SP.Site_EditSite,
                new object[,]{
                    {"@siteId",site.Id},
                    {"@name",site.Name},
                    {"@dirname",site.DirName},
                    {"@tpl",site.Tpl},
                    {"@domain",site.Domain},
                    {"@location",site.Location},
                    {"@language",site.Language},
                    {"@note",site.Note},
                    {"@seotitle",site.SeoTitle},
                    {"@seokeywords",site.SeoKeywords},
                    {"@seodescription",site.SeoDescription},
                    {"@state",site.State},
                    {"@protel",site.Tel},
                    {"@prophone",site.Phone},
                    {"@profax",site.Fax},
                    {"@proaddress",site.Address},
                    {"@proemail",site.Email},
                    {"@im",site.Im},
                    {"@postcode",site.PostCode},
                    {"@pronotice",site.Notice},
                    {"@proslogan",site.Slogan}
                }));
        }
    }
}
