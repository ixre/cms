using System.Web.Mvc;
using J6.Cms;
using J6.Cms.BLL;
using J6.Data;
using System;
using System.Data;
using J6.Cms.Models;

namespace Cms.Web.Controllers
{
    public class ExportController : Controller
    {
        private static ArchiveBLL bll = new ArchiveBLL();
        private static CategoryBLL cbll = new CategoryBLL();

        public string Index()
        {
            DataBaseAccess db = new DataBaseAccess(DataBaseType.MySQL, "server=s3.ns-cache.j6.cc;uid=root;pwd=$Newmin;database=mydb;charset=utf8");
            DataBaseAccess db2 = new DataBaseAccess(DataBaseType.SQLite, String.Format("Data Source={0}/data/#db.db3", AppDomain.CurrentDomain.BaseDirectory));

            int i = 0, j = 0;
            foreach (DataRow dr in db2.GetDataSet("SELECT * FROM O_archives").Tables[0].Rows)
            {
                try
                {
                    
                    db.ExecuteNonQuery("INSERT INTO opsblog_archives(id,cid,`author`,`title`,`properties`,`content`,tags,viewcount,createdate,lastmodifydate) values(@id,@cid,@author,@title,@properties,@content,@tags,@viewcount,createdate,lastmodifydate)",
                        db.NewParameter("@id", dr["id"].ToString()),
                        db.NewParameter("@cid", dr["cid"].ToString()),
                        db.NewParameter("@author", dr["author"].ToString()),
                        db.NewParameter("@title", dr["title"].ToString()),
                        db.NewParameter("@properties", dr["properties"].ToString()),
                        db.NewParameter("@content", dr["content"].ToString()),
                        db.NewParameter("@tags", dr["tags"].ToString()),
                        db.NewParameter("@viewcount", dr["viewcount"].ToString()),
                        db.NewParameter("@createdate", dr["createdate"].ToString()),
                        db.NewParameter("@lastmodifydate", dr["lastmodifydate"].ToString())
                        );
                    
                    ++i;
                }
                catch (Exception ex)
                {
                    ++j;
                }


                /*
                db.ExecuteNonQuery("update  opsblog_archives set createdate=@dt,lastmodifydate=@dt where id=@id",
                    db.dbFactory.CreateParameter("@dt", dr["createdate"]),
                    db.dbFactory.CreateParameter("@id", dr["id"].ToString())
                    );
                */

            }


            //db.ExecuteNonQuery("update  opsblog_archives set source='',outline='',agree=0,disagree=0,isspecial=0,issystem=0,visible=1");

            return string.Format("共导入{0}条，成功{1}条，失败{2}条", (i + j), i, j);
        }
    }
}
