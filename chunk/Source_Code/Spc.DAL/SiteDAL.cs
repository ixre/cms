using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ops.Cms.IDAL;
using Ops.Data;
using Ops.Data.Orm;
using Ops.Cms.Domain.Interface.Site;

namespace Ops.Cms.DAL
{
    public class SiteDAL:DALBase
    {
        public int CreateSite(ISite site)
        {
            int row = base.ExecuteNonQuery(
                SqlQueryHelper.Format(SP.Site_CreateSite,
                new object[,]{
                    {"@name",site.Name},
                    {"@dirname",site.DirName},
                    {"@domain",site.Domain},
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
                }),
                //初始化Root数据
                SqlQueryHelper.Format(@"INSERT INTO $PREFIX_categories 
                                        (siteid,pagetitle,moduleid,tag,name,keywords,description,lft,rgt,orderindex)
                                        VALUES((SELECT MAX(siteid) FROM $PREFIX_sites),
                                         'ROOT',1,'root','根栏目','','',1,2,1)
                                        ")
                );



            if (row != 2) return -1;

            return int.Parse(base.ExecuteScalar(SqlQueryHelper.Format("SELECT MAX(siteid) FROM $PREFIX_sites")).ToString());
        }

        public void LoadSites(DataReaderFunc func)
        {
            base.ExecuteReader(new SqlQuery(base.OptimizeSQL(SP.Site_GetSites)),func);
        }


        public int UpdateSite(ISite site)
        {
            return base.ExecuteNonQuery(
                SqlQueryHelper.Format(SP.Site_EditSite,
                new object[,]{
                    {"@siteId",site.ID},
                    {"@name",site.Name},
                    {"@dirname",site.DirName},
                    {"@tpl",site.Tpl},
                    {"@domain",site.Domain},
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
