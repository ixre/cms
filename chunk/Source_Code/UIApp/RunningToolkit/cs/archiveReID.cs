/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 2014/4/17
 * Time: 22:59
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using J6.Cms.Infrastructure;
using J6.DevFw.Data;


namespace CsharpIpy.cs
{
    /// <summary>
    /// Description of archiveReID.
    /// </summary>
    public class ArchiveReId
    {
        public static void ReNotIdArchives()
        {
            DataBaseAccess dba=new DataBaseAccess(
                DataBaseType.MySQL,
                "server=s5.ns-cache.j6.cc;database=mydb;uid=newmin;pwd=$Newmin;charset=utf8");
		
		
            IList<int> list= new List<int>();
		
            dba.ExecuteReader("select id from lms_archives where strid=''",
                rd=>{
                        while(rd.Read()){
                            list.Add(rd.GetInt32(0));
                        }
                });
		
		
            foreach(int intId in list)
            {
			
                string strId;
                do
                {
                    strId = IdGenerator.GetNext(5);              //创建5位ID
                } while (
                    int.Parse(dba.ExecuteScalar("SELECT count(1) FROM lms_archives where id='"+strId+"' OR alias='"+strId+"'").ToString())!=0
			
                    );
			
                dba.ExecuteNonQuery("UPDATE lms_archives set strid='"+strId+"' where id="+intId);
			
			
            }
		
            Console.WriteLine("finish!");
        }
	
        public static void ArchiveReCreateDate()
        {
            DataBaseAccess dba=new DataBaseAccess(
                DataBaseType.MySQL,
                "server=s5.ns-cache.j6.cc;database=mydb;uid=newmin;pwd=$Newmin;charset=utf8");
		
		
            IList<int> list= new List<int>();
		
            dba.ExecuteReader("select id from lms_archives where id>524",
                rd=>{
                        while(rd.Read()){
                            list.Add(rd.GetInt32(0));
                        }
                });
		
		
            SqlQuery[] sqls= new SqlQuery[list.Count];
		
            DateTime dt=DateTime.Now;
            int tmpInt=0;
            foreach(int intId in list)
            {
                dt=dt.AddMinutes(-1);
                sqls[tmpInt]=
                    new SqlQuery(
                        "UPDATE lms_archives set lastmodifydate=@dt WHERE id =@id",
                        new object[,]{
                            {"@dt", dt},
                            {"@id",intId}
						
                        });
                tmpInt++;
            }
		
            dba.ExecuteNonQuery(sqls);
        }
    }
}

