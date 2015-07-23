/*
* Copyright(C) 2010-2013 S1N1.COM
* 
* File Name	: TableBLL
* Author	: Newmin (new.min@msn.com)
* Create	: 2013-01-06 16:30:20
* Description	:
*
*/


using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using J6.Cms.Dal;
using J6.Cms.Domain.Interface.Models;
using J6.Cms.Domain.Interface._old;
using J6.Cms.IDAL;
using J6.Cms.Infrastructure;
using J6.DevFw.Data.Extensions;

namespace J6.Cms.BLL
{
    public class TableBLL : ITable
    {
        private static ITableDAL dal = new TableDal();

        public OperateResult AddTable(Table table, TableColumn[] columns)
        {
            return dal.AddTable(table, columns);
        }

        public OperateResult DeleteTable(int tableID)
        {
            return dal.DeleteTable(tableID);
        }

        public OperateResult UpdateTable(Table table, TableColumn[] columns)
        {
            return dal.UpdateTable(table, columns);
        }

        public Table GetTable(int tableID)
        {
            Table e = null;
            dal.GetTable(tableID, rd =>
            {
               e=rd.ToEntity<Table>();
            });

            return e;
        }

        public IList<Table> GetTables()
        {
            IList<Table> e = null;
            dal.GetTables(rd =>
            {
                if (rd.HasRows)
                {
                    e = rd.ToEntityList<Table>();
                }
            });

            return e;
        }


        #region 列
        public TableColumn GetColumn(int columnID)
        {
            TableColumn e = null;
            dal.GetColumn(columnID, rd =>
            {
                if (rd.HasRows)
                {
                    e = rd.ToEntity<TableColumn>();
                }
            });
            return e;
        }

        public IList<TableColumn> GetColumns(int tableID)
        {
            IList<TableColumn> e = null;
            dal.GetColumns(tableID, rd =>
            {
                if (rd.HasRows)
                {
                    e = rd.ToEntityList<TableColumn>();
                }
            });

            return e??new List<TableColumn>();

        }

        public OperateResult AddColumn(TableColumn column)
        {
            return dal.AddColumn(column);
        }

        public OperateResult UpdateColumn(TableColumn tableColumn)
        {
            return dal.UpdateColumn(tableColumn);
        }

        public OperateResult DeleteColumn(int tableID, int columnID)
        {
            return dal.DeleteColumn(tableID, columnID);
        }

        #endregion

        #region 行

       public  int GetRowsCount(int tableID)
        {
            return dal.GetRowsCount(tableID);
        }

        public DataTable GetPagedRecords(int tableID,string keyword, int pageSize, int currentPageIndex, out int recordCount, out int pageCount)
       {
           IList<TableColumn> columns = GetColumns(tableID);
           DataTable dt=dal.GetPagedRows(tableID,keyword, pageSize, currentPageIndex, out recordCount, out pageCount);

            //将列添加进入datatable(列名为:$+ID)
            if (columns != null && columns.Count!=0)
            {
                foreach (TableColumn c in columns)
                {
                    dt.Columns.Add(new DataColumn("$" + c.Id.ToString()));
                }
            }

            //如果不存在行则并取得数据
            if (dt.Rows.Count != 0)
            {
                //计算RowID范围
                int indent = 0;
                int[] ids = new int[dt.Rows.Count];
                foreach (DataRow dr in dt.Rows)
                {
                    ids[indent] = int.Parse(dr["id"].ToString());
                    ++indent;
                }

                IList<TableRowData> rowDatas = dal.GetRowsData(ids).ToEntityList<TableRowData>();
                TableRowData tr;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < columns.Count; j++)
                    {
                        //获取包含数据的行
                        tr = rowDatas.FirstOrDefault(a => a.Rid == int.Parse(dt.Rows[i]["id"].ToString()) && a.Cid == columns[j].Id);

                        //对列赋值
                        if (tr != null)
                        {
                            dt.Rows[i]["$" + columns[j].Id.ToString()] = tr.Value;
                        }
                    }
                }

            }

            return dt;
          
        }

        [Obsolete]
        public TableRow GetRecord(int rowId)
        {
            TableRow e = null;
            dal.GetRow(rowId, rd =>
            {
                e = rd.ToEntity<TableRow>();
            });

            return e;
        }

        /// <summary>
        /// 提交表单，返回行号
        /// </summary>
        /// <param name="tableId"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        public int SubmitRow(int tableId, NameValueCollection form)
        {
            //table name :cms_form_tableid
            //column name:field_tableid_columnid
            //button name:cms_form_tableid_btn


            StringBuilder sb = new StringBuilder();
            Regex cregex = new Regex("^field_\\d+_(\\d+)$");

            IDictionary<int, string> rowsData = new Dictionary<int, string>();

            foreach (string key in form)
            {
                if (cregex.IsMatch(key))
                {
                    int columnId = int.Parse(cregex.Match(key).Groups[1].Value);
                    //sb.Append("$").Append(columnID.ToString()).Append("=").Append(form[key]);
                    rowsData.Add(columnId, form[key]);
                }
            }

            TableRowData[] rows = new TableRowData[rowsData.Count];

            int i = 0;
            foreach (KeyValuePair<int, string> pair in rowsData)
            {
                rows[i] = new TableRowData
                {
                    Value = pair.Value,
                    Cid = pair.Key
                };
                ++i;
            }


            return dal.CreateRow(tableId, rows);
        }

        /// <summary>
        /// 删除行
        /// </summary>
        /// <param name="tableId"></param>
        /// <param name="rowId"></param>
        /// <returns></returns>
        public OperateResult DeleteRow(int tableId, int rowId)
        {
            return dal.DeleteRow(tableId,rowId);
        }

        #endregion

    }
}
