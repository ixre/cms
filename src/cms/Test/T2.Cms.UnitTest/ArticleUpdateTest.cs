using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JR.DevFw.Data;
using System.Threading;
using System.Data.Common;
using T2.Cms.Domain.Interface.Site.Extend;

namespace T2.Cms.UnitTest
{
    [TestClass]
    public class ArticleUpdateTest
    {
        private TestBase _base = new TestBase();
        [TestMethod]
        public void TestArchiveUpdate()
        {
            DataBaseAccess db = this._base.GetDb();
            int aid = 94;
            int sid = 1;

            int cid = this.GetCid(db, aid, sid);
            new Thread(()=>
            {
                Update(db, aid, cid, "测试标题", "", "", "", "", "", "", "{st:'0',sc:'0',v:'1',p:'0'}", "", "", 75);
            }).Start();
            Update(db, aid, cid, "测试标题", "", "", "", "", "", "", "{st:'0',sc:'0',v:'1',p:'0'}", "", "", 75);
            Thread.Sleep(2000);
        }

        private int GetCid(DataBaseAccess db,int aid, int sid)
        {
            db.ExecuteReader(new SqlQuery(@"SELECT * FROM cms_related_link WHERE content_type = 'archive' AND content_id = 102"),(rd)=>
            {
            });
            db.ExecuteScalar(new SqlQuery(@"SELECT MAX(cms_archive.sort_number) FROM  cms_archive 
                        INNER JOIN cms_category ON cms_archive.cid=cms_category.id
                        WHERE cms_category.site_id=1"));
            db.ExecuteScalar(new SqlQuery(@"SELECT alias FROM cms_archive INNER JOIN 
                                cms_category ON cms_category.id = cms_archive.cid
                                WHERE site_id = 1 AND(alias = 'gj175' or cms_archive.str_id = 'gj175')"));

            db.ToEntityList<object>(new SqlQuery(@"SELECT v.id as id,relation_id,field_id,f.name as fieldName,field_value
	        FROM cms_extend_value v INNER JOIN cms_extend_field f ON v.field_id=f.id
	        WHERE relation_id=94 AND f.site_id=1 AND relation_type=1"));

            int cid = 0;
            db.ExecuteReader(new SqlQuery(
                @"SELECT * FROM cms_archive INNER JOIN cms_category
                    ON cms_category.id = cms_archive.cid  WHERE site_id = @siteId AND cms_archive.id = @id",
                new object[,]
                {
                        {"@siteId",sid },
                        {"@id",aid }
                }), (rd) =>
                {
                    while (rd.Read())
                    {
                        cid = int.Parse(rd["cid"].ToString());
                    }
                });
            return cid;
        }



        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="a"></param>
        public void Update(DataBaseAccess db,int id, int categoryID, string title, string smallTitle, string alias,
            string source, string thumbnail, string outline, string content,
            string tags, string flags, string location, int sortNumber)
        {
            string sql = @"UPDATE cms_archive SET[cid] = @CategoryId,[Title] = @Title, small_title = @smallTitle, sort_number = @sortNumber, flags = @flags,
                                    [Alias] = @Alias, location = @location,[Source] = @Source, thumbnail = @thumbnail, lastmodifydate = @lastmodifyDate,
                                    [Outline] = @Outline,[Content] = @Content,[Tags] = @Tags WHERE id = @id";
            string date = String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
            db.ExecuteNonQuery(new SqlQuery(sql,new object[,]{
                                {"@CategoryId", categoryID},
                                {"@Title", title},
                                {"@smallTitle", smallTitle??""},
                                {"@Flags", flags},
                                {"@Alias", alias??""},
                                {"@location", location},
                                {"@sortNumber",sortNumber},
                                {"@Source", source??""},
                                {"@thumbnail",thumbnail??""},
                                {"@Outline", outline??""},
                                {"@Content", content},
                                {"@Tags", tags??""},
                                {"@lastModifyDate",date},
                                {"@id", id}
                 }));
        }
    }
}
