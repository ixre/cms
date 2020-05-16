/*
 * 用户： newmin
 * 日期: 2013/11/29
 * 时间: 21:26
 * 
 * 修改说明：
 */

using System;
using System.Collections.Generic;
using System.Threading;
using JR.Stand.Core.Data;
using JR.Stand.Core.Data.Extensions;

namespace ToEntity
{
	/// <summary>
	/// Description of TestToEntity.
	/// </summary>
	public class TestToEntity
	{
		public static void Test()
		{
			DataBaseAccess access=new DataBaseAccess(DataBaseType.SQLite,"Data source=$ROOT$/db.db");
			IList<DataExtendAttr> list=null;
			
		print:
			access.ExecuteReader("SELECT extID as extendID,* FROM  cms_dataExtendAttr",
			                                                rd=>{
			                     	if(rd.HasRows)
			                     	{
			                     		list=rd.ToEntityList<DataExtendAttr>();
			                     	}
			                                                }
			                    );

            
			
			foreach(DataExtendAttr a in list)
			{
				Console.WriteLine("{0}-{1}-{2}-{3}",a.ExtendID,a.AttrName,a.AttrType,a.AttrVal);
            }

            Console.WriteLine("----------------------");

         list= access.GetDataSet("SELECT extID as extendID,* FROM  cms_dataExtendAttr").Tables[0].ToEntityList<DataExtendAttr>();

			
			
			foreach(DataExtendAttr a in list)
			{
				Console.WriteLine("{0}-{1}-{2}-{3}",a.ExtendID,a.AttrName,a.AttrType,a.AttrVal);
			}
			                     
			            

			Console.WriteLine("----------------------");
			Thread.Sleep(1000);
			//goto print;
		}
	}
}
