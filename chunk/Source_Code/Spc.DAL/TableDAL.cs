/*
* Copyright(C) 2010-2013 OPSoft Inc
* 
* File Name	: TableDAL
* Author	: Newmin (new.min@msn.com)
* Create	: 2012-01-06 16:33:33
* Description :
*
*/


namespace Ops.Cms.DAL
{
    using Ops.Cms.IDAL;
    using Spc.Models;
    using Ops;
    using Ops.Data;
    using System.Data;
    using System;
    using Ops.Cms.Infrastructure;

    public class TableDAL : DALBase, ITableDAL
    {

        public OperateResult AddTable(Table table, TableColumn[] columns)
        {
            // try
            // {

            int tableID = 0;

            base.ExecuteReader(
                 new SqlQuery(base.OptimizeSQL(SP.Table_GetTableIDByName),
                     new object[,]{
                          {"@name",table.Name}
                     }),
                 rd =>
                 {
                     if (rd.Read())
                     {
                         tableID = rd.GetInt32(0);
                     }
                 }
               );

            if (tableID != 0)
            {
                return OperateResult.Exists;
            }

            int rowcount = base.ExecuteNonQuery(
                new SqlQuery(base.OptimizeSQL(SP.Table_Add),
                     new object[,]{
                {"@name",table.Name},
               {"@note", table.Note},
               {"@apiserver", table.ApiServer},
               {"@issystem", table.IsSystem},
                 {"@available", table.Available}
                     })
               );


            //添加表单成功
            if (rowcount == 1)
            {
                base.ExecuteReader(
                 new SqlQuery(base.OptimizeSQL(SP.Table_GetTableIDByName),

                      new object[,]{
                      
                {"@name",table.Name}
                      }),
                 rd =>
                 {
                     if (rd.Read())
                     {
                         tableID = rd.GetInt32(0);
                     }
                 }
               );

                //添加列
                if (columns != null)
                {
                    foreach (TableColumn col in columns)
                    {
                        base.ExecuteNonQuery(
                               new SqlQuery(base.OptimizeSQL(SP.Table_CreateColumn),
                                   new object[,]{
                              {"@tableid", tableID},
                              {"@name", col.Name},
                              {"@note", col.Note},
                              {"@validformat", col.ValidFormat},
                              {"@orderindex", col.OrderIndex}
                                   })
                              );
                    }
                }

                return OperateResult.Success;

            }
            else
            {
                return OperateResult.Fail;
            }

            //}
            // catch(Exception ex)
            // {
            //     return OperateResult.Except;
            //}
        }

        public OperateResult DeleteTable(int tableId)
        {

            DataBaseAccess db = base.db;
            if (int.Parse(base.ExecuteScalar(new SqlQuery(base.OptimizeSQL(SP.Table_HasExistsSystemTale),new object[,]{ {"@tableid", tableId}})).ToString()) != 0)
            {
                return OperateResult.IsSystem;
            }
            else if (int.Parse(base.ExecuteScalar(new SqlQuery(base.OptimizeSQL(SP.Table_GetMinTableID))).ToString()) == tableId)
            {
                return OperateResult.Disallow;
            }
            else if (int.Parse(base.ExecuteScalar(new SqlQuery(base.OptimizeSQL(SP.Table_GetRowsCount), new object[,]{{"@tableid", tableId}})).ToString()) != 0)
            {
                return OperateResult.Related;   //存在表单记录无法删除
            }
            else
            {
                //删除列
                base.ExecuteNonQuery(new SqlQuery(base.OptimizeSQL(SP.Table_DeleteColumns),
                    new object[,]{
                 {"@tableid", tableId}
                    }));

                //删除表单
                return base.ExecuteNonQuery(
                        new SqlQuery(base.OptimizeSQL(SP.Table_DeleteTable),
                            new object[,]{
                        {"@tableid", tableId}
                            })) == 1
                        ? OperateResult.Success
                        : OperateResult.Fail;
            }
        }

        public OperateResult UpdateTable(Table table, TableColumn[] columns)
        {
            int tableID = 0;
            base.ExecuteReader(
                     new SqlQuery(base.OptimizeSQL(SP.Table_GetTableIDByName),
                         new object[,]{
                      {"@name", table.Name}
                         }),
                     rd =>
                     {
                         if (rd.Read())
                         {
                             tableID = rd.GetInt32(0);
                         }
                     }
                   );

            if (tableID != 0 && tableID != table.ID)
            {
                return OperateResult.Exists;
            }


            int rowcount = base.ExecuteNonQuery(
                      new SqlQuery(base.OptimizeSQL(SP.Table_Update),
                          new object[,]{
                     {"@name", table.Name},
                     {"@note", table.Note},
                     {"@apiserver", table.ApiServer},
                     {"@issystem", table.IsSystem},
                     {"@available", table.Available},
                     {"@tableid", table.ID}
                          })
                     );

            return rowcount == 1 ? OperateResult.Success : OperateResult.Fail;
        }

        public void GetTable(int tableId, DataReaderFunc func)
        {
            base.ExecuteReader(
                    new SqlQuery(base.OptimizeSQL(SP.Table_GetTableByID),
                        new object[,]{
                        {"@tableid", tableId}
                        }),
                    func
                     
                  );
        }

        public void GetTables(DataReaderFunc func)
        {
            base.ExecuteReader(
                     new SqlQuery(base.OptimizeSQL(SP.Table_GetTables)),
                     func
                   );
        }

        public void GetColumns(int tableId, DataReaderFunc func)
        {
            base.ExecuteReader(
                     new SqlQuery(base.OptimizeSQL(SP.Table_GetColumnsByTableID),
                         new object[,]{{"@tableid", tableId}
                         }),
                     func

                   );
        }

        public OperateResult AddColumn(TableColumn column)
        {
            int rowCount = base.ExecuteNonQuery(
                      new SqlQuery(base.OptimizeSQL(SP.Table_CreateColumn),
                          new object[,]{
                      {"@name", column.Name},
                      {"@note", column.Note},
                      {"@validformat", column.ValidFormat},
                      {"@orderindex", column.OrderIndex},
                      {"@tableid", column.TableID}
                          })
                    );
            return rowCount == 1 ? OperateResult.Success : OperateResult.Fail;
        }

        public void GetColumn(int columnId,DataReaderFunc func)
        {
            base.ExecuteReader(
                    new SqlQuery(base.OptimizeSQL(SP.Table_GetColumn),
                        new object[,]{
                        {"@columnid", columnId}
                        }),
                    func
                     
                  );
        }

        public OperateResult UpdateColumn(TableColumn column)
        {
            int rowCount = base.ExecuteNonQuery(
                      new SqlQuery(base.OptimizeSQL(SP.Table_UpdateColumn),
                          new object[,]{
                      {"@name", column.Name},
                      {"@note", column.Note},
                      {"@validformat", column.ValidFormat},
                      {"@orderindex", column.OrderIndex},
                      {"@columnid", column.ID}
                          })
                    );
            return rowCount == 1 ? OperateResult.Success : OperateResult.Fail;
        }


        public OperateResult DeleteColumn(int tableId, int columnId)
        {
            OperateResult result = base.ExecuteNonQuery(
                        new SqlQuery(base.OptimizeSQL(SP.Table_DeleteColumn),
                            new object[,]{
                        {"@tableid", tableId},
                        {"@columnid", columnId}
                            })
                        ) == 1
                        ? OperateResult.Success
                        : OperateResult.Fail;

            if (result == OperateResult.Success)
            {
                base.ExecuteNonQuery(
                        new SqlQuery(base.OptimizeSQL(SP.Table_ClearDeletedColumnData),
                            new object[,]{
                        {"@tableid", tableId},
                        {"@columnid", columnId}
                            })
                        );
            }
            return result;
        }




        public int GetRowsCount(int tableId)
        {
           return int.Parse(base.ExecuteScalar(
                new SqlQuery(base.OptimizeSQL(SP.Table_GetRowsCount)
                ,new object[,]{{"@tableid",tableId}})
                ).ToString());
        }


        public int CreateRow(int tableId, TableRowData[] rows)
        {
            DataBaseAccess db = base.db;
            int rowID = 0;
            string date = String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);

            base.ExecuteNonQuery(
                new SqlQuery(base.OptimizeSQL(SP.Table_CreateRow),
                    new object[,]{
                {"@tableid", tableId},
                {"@submittime",date}
                    }));

            //获取生成的行编号
            rowID = int.Parse(base.ExecuteScalar(new SqlQuery(base.OptimizeSQL(SP.Table_GetLastedRowID))).ToString());

            foreach (TableRowData row in rows)
            {
                var i = rowID;
                base.ExecuteNonQuery(
                     new SqlQuery(base.OptimizeSQL(SP.Table_InsertRowData),
                         new object[,]{
                     {"@rowid", rowID},
                     {"@columnid", row.Cid},
                     {"@value", row.Value}
                         })
                     );

            }
            return rowID;
        }

        public DataTable GetPagedRows(int tableID, string keyword, int pageSize, int currentPageIndex, out int recordCount, out int pageCount)
        {
            /*
            string condition = ArchiveFlag.GetSQLString(new string[,]{
                    {"st","0"},
                    {"v","1"}
                });
             */

          //  string condition = String.Empty ;
           // if (!String.IsNullOrEmpty(keyword))
           // {
           //     condition = String.Format(" AND value LIKE '%{0}%'", keyword.Replace("'", "''"));
          //  }


            //数据库为OLEDB,且为第一页时
            const string sql1 = @"SELECT TOP $[pagesize] * FROM $PREFIX_table_rows WHERE tableid=$[tableid] ORDER BY submittime DESC";


            //记录数
            recordCount = int.Parse(base.ExecuteScalar(
                new SqlQuery(base.OptimizeSQL(SP.Table_GetRowsCount),
                    new object[,]{
                {"@tableid",tableID}
                    })
                ).ToString());

            //页数
            pageCount = recordCount / pageSize;
            if (recordCount % pageSize != 0) pageCount++;

            //对当前页数进行验证
            if (currentPageIndex > pageCount && currentPageIndex != 1) currentPageIndex = pageCount;
            if (currentPageIndex < 1) currentPageIndex = 1;

            //跳过记录数
            int skipCount = pageSize * (currentPageIndex - 1);

            //如果调过记录为0条，且为OLEDB时候，则用sql1
            string sql = skipCount == 0 && base.DbType== DataBaseType.OLEDB ?
                      base.OptimizeSQL(sql1) :
                        base.OptimizeSQL(SP.Table_GetPagedRows);


            sql = SQLRegex.Replace(sql, (match) =>
            {
                switch (match.Groups[1].Value)
                {
                    case "tableid": return tableID.ToString();
                    case "pagesize": return pageSize.ToString();
                    case "skipsize": return skipCount.ToString();
                   // case "keyword": return keyword;
                }
                return null;
            });
            return base.GetDataSet(new SqlQuery(sql)).Tables[0];

        }

        public OperateResult DeleteRow(int tableId, int rowId)
        {
            //清理数据
            base.ExecuteNonQuery(
                    new SqlQuery(base.OptimizeSQL(SP.Table_ClearDeletedRowData),
                        new object[,]{
                    {"@tableid", tableId},
                    {"@rowid", rowId}
                        })
                    );

            //删除行
            return base.ExecuteNonQuery(
                      new SqlQuery(base.OptimizeSQL(SP.Table_DeleteRow),
                          new object[,]{
                      {"@tableid", tableId},
                      {"@rowid", rowId}
                          })
                    ) == 1 ? OperateResult.Success : OperateResult.Fail;
        }

        [Obsolete]
        public void GetRow(int rowId,DataReaderFunc func)
        {
            base.ExecuteReader(
                     new SqlQuery(base.OptimizeSQL(SP.Table_GetRow),
                         new object[,]{
                            {"@rowid", rowId}
                         }),
                     func
                       
                   );
        }

        /// <summary>
        /// 获取行数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public DataTable GetRowsData(int[] ids)
        {
            string rowRange=String.Empty;
            int i=0;
            Array.ForEach(ids, a =>
            {
                if (++i != 1) rowRange += ",";
                rowRange += a.ToString();
            });

            string sql = base.OptimizeSQL(SP.table_GetRowData);
            sql = SQLRegex.Replace(sql, (match) =>
            {
                switch (match.Groups[1].Value)
                {
                    case "range": return rowRange;
                }
                return null;
            });

            return base.GetDataSet(new SqlQuery( sql)).Tables[0];
        }

    }

}
