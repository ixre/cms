/**
 * Copyright (C) 2007-2015 OPSoft INC,All rights reseved.
 * Get more infromation of this software,please visit site http://cms.ops.cc
 * 
 * name : ExportItem.cs
 * author : newmin (new.min@msn.com)
 * date : 2012/12/01 23:00:00
 * description : 
 * history : 
 */

using System;
using System.Collections;
using System.Data;
using System.Text.RegularExpressions;

namespace JR.Cms.Repository.Export
{
    public class ExportItem:BaseDataExportPortal
    {

        private static readonly Regex UnsafeSqlReg = new Regex("(DROP|SELECT|DELETE|UPDATE)\\s", RegexOptions.IgnoreCase);

        public ExportItem(string queryName, DataColumnMapping[] columns):base(columns)
        {
            this.PortalKey = queryName;
        }



        private DataTable GetQueryView(string queryName, Hashtable hash, int pageSize, int currentPageIndex,
            out int totalCount)
        {
            DataBaseAccess db = ExportManager.GetDao();

            ExportItemConfig item = ExportManager.GetConfigByQueryName(queryName);
            string query = item.Query;
            string queryTotal = item.Total;

            //添加分页参数
            if (hash != null)
            {
                foreach (DictionaryEntry o in hash)
                {
                    if (UnsafeSqlReg.IsMatch(o.Value.ToString()))
                    {
                        throw new ArgumentException("含有不安全的查询!");
                    }

                }
                hash.Add("page_start", currentPageIndex <= 0 ? 0 : (currentPageIndex - 1) * pageSize);
                hash.Add("page_end", (currentPageIndex) * pageSize);
                hash.Add("page_size", pageSize);


                //格式化
                query = query.Template(hash);
                // throw new Exception(query + "/" + currentPageIndex+"/"+pageSize);
                if (!String.IsNullOrEmpty(queryTotal))
                {
                    queryTotal = queryTotal.Template(hash);
                }
            }


            //获取分页结果
            DataTable dataTable = db.GetDataSet(query, hash).Tables[0];

            //获取统计数据
            if (!String.IsNullOrEmpty(queryTotal))
            {
                object data = db.ExecuteScalar(queryTotal, hash);
                int.TryParse(data.ToString(), out totalCount);
            }
            else
            {
                totalCount = dataTable.Rows.Count;
            }

            return dataTable;
        }

        public override DataTable GetShemalAndData( Hashtable hash,out int totalCount)
        {
            return this.GetQueryView(
                this.PortalKey,
                hash,
                hash.ContainsKey("pageSize") ? int.Parse(hash["pageSize"].ToString()) : 100000,
                hash.ContainsKey("pageIndex") ? int.Parse(hash["pageIndex"].ToString()) : 1, out totalCount);
        }

        public override DataRow GetTotalView(Hashtable hash)
        {
            /*
            return IocObject.Data.GetTotalView(
               this.PortalKey,
                hash);
            */
            throw new NotImplementedException();
        }


        public override sealed string PortalKey { get; set; }
    }
}