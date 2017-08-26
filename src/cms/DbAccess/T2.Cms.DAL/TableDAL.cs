/*
* Copyright(C) 2010-2013 TO2.NET
* 
* File Name	: TableDAL
* Author	: Newmin (new.min@msn.com)
* Create	: 2012-01-06 16:33:33
* Description :
*
*/


using System;
using System.Data;
using T2.Cms.Domain.Interface.Models;
using T2.Cms.IDAL;
using T2.Cms.Infrastructure;
using JR.DevFw.Data;

namespace T2.Cms.Dal
{
    public class TableDal : DalBase, ITableDAL
    {

        public OperateResult AddTable(Table table, TableColumn[] columns)
        {
            // try
            // {
           
            int tableId = 0;

            base.ExecuteReader(
                 base.NewQuery(DbSql.Table_GetTableIDByName,
                 base.Db.CreateParametersFromArray(
                     new object[,]{
                          {"@name",table.Name}
                     })),
                 rd =>
                 {
                     if (rd.Read())
                     {
                         tableId = rd.GetInt32(0);
                     }
                 }
               );

            if (tableId != 0)
            {
                return OperateResult.Exists;
            }

            int rowcount = base.ExecuteNonQuery(
                base.NewQuery(DbSql.Table_Add,base.Db.CreateParametersFromArray(
                     new object[,]{
                {"@name",table.Name},
               {"@note", table.Note},
               {"@apiServer", table.ApiServer},
               {"@isSystem", table.IsSystem},
                 {"@enabled", table.Enabled}
                     }))
               );


            //添加表单成功
            if (rowcount == 1)
            {
                base.ExecuteReader(
                 base.NewQuery(DbSql.Table_GetTableIDByName,
                 base.Db.CreateParametersFromArray(
                      new object[,]{
                      
                {"@name",table.Name}
                      })),
                 rd =>
                 {
                     if (rd.Read())
                     {
                         tableId = rd.GetInt32(0);
                     }
                 }
               );

                //添加列
                if (columns != null)
                {
                    foreach (TableColumn col in columns)
                    {
                        base.ExecuteNonQuery(
                               base.NewQuery(DbSql.Table_CreateColumn,
                               base.Db.CreateParametersFromArray(
                                   new object[,]{
                              {"@tableId", tableId},
                              {"@name", col.Name},
                              {"@note", col.Note},
                              {"@validformat", col.ValidFormat},
                              {"@sortNumber", col.SortNumber}
                                   }))
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

            DataBaseAccess db = base.Db;
            if (int.Parse(base.ExecuteScalar(base.NewQuery(DbSql.Table_HasExistsSystemTale,
                base.Db.CreateParametersFromArray(new object[,] { { "@tableId", tableId } }))).ToString()) != 0)
            {
                return OperateResult.IsSystem;
            }
            else if (int.Parse(base.ExecuteScalar(base.NewQuery(DbSql.Table_GetMinTableId,null)).ToString()) == tableId)
            {
                return OperateResult.Disallow;
            }
            else if (int.Parse(base.ExecuteScalar(base.NewQuery(DbSql.Table_GetRowsCount,
                base.Db.CreateParametersFromArray(new object[,]{{"@tableId", tableId}}))).ToString()) != 0)
            {
                return OperateResult.Related;   //存在表单记录无法删除
            }
            else
            {
                //删除列
                base.ExecuteNonQuery(base.NewQuery(DbSql.Table_DeleteColumns,
                    base.Db.CreateParametersFromArray(
                    new object[,]{
                 {"@tableId", tableId}
                    })));

                //删除表单
                return base.ExecuteNonQuery(
                        base.NewQuery(DbSql.Table_DeleteTable,
                        base.Db.CreateParametersFromArray(
                            new object[,]{
                        {"@tableId", tableId}
                            }))) == 1
                        ? OperateResult.Success
                        : OperateResult.Fail;
            }
        }

        public OperateResult UpdateTable(Table table, TableColumn[] columns)
        {
            int tableId = 0;
            base.ExecuteReader(
                     base.NewQuery(DbSql.Table_GetTableIDByName,
                     base.Db.CreateParametersFromArray(
                         new object[,]{
                      {"@name", table.Name}
                         })),
                     rd =>
                     {
                         if (rd.Read())
                         {
                             tableId = rd.GetInt32(0);
                         }
                     }
                   );

            if (tableId != 0 && tableId != table.Id)
            {
                return OperateResult.Exists;
            }


            int rowcount = base.ExecuteNonQuery(
                      base.NewQuery(DbSql.Table_Update,
                      base.Db.CreateParametersFromArray(
                          new object[,]{
                     {"@name", table.Name},
                     {"@note", table.Note},
                     {"@apiServer", table.ApiServer},
                     {"@isSystem", table.IsSystem},
                     {"@enabled", table.Enabled},
                     {"@tableId", table.Id}
                          }))
                     );

            return rowcount == 1 ? OperateResult.Success : OperateResult.Fail;
        }

        public void GetTable(int tableId, DataReaderFunc func)
        {
            base.ExecuteReader(
                    base.NewQuery(DbSql.Table_GetTableById,
                    base.Db.CreateParametersFromArray(
                        new object[,]{
                        {"@tableId", tableId}
                        })),
                    func
                     
                  );
        }

        public void GetTables(DataReaderFunc func)
        {
            base.ExecuteReader(
                     base.NewQuery(DbSql.Table_GetTables,null),
                     func
                   );
        }

        public void GetColumns(int tableId, DataReaderFunc func)
        {
            base.ExecuteReader(
                     base.NewQuery(DbSql.TableGetColumnsByTableId,
                     base.Db.CreateParametersFromArray(
                         new object[,]{{"@tableId", tableId}
                         })),
                     func

                   );
        }

        public OperateResult AddColumn(TableColumn column)
        {
            int rowCount = base.ExecuteNonQuery(
                      base.NewQuery(DbSql.Table_CreateColumn,
                      base.Db.CreateParametersFromArray(
                          new object[,]{
                      {"@name", column.Name},
                      {"@note", column.Note},
                      {"@validformat", column.ValidFormat},
                      {"@sortNumber", column.SortNumber},
                      {"@tableId", column.TableId}
                          }))
                    );
            return rowCount == 1 ? OperateResult.Success : OperateResult.Fail;
        }

        public void GetColumn(int columnId,DataReaderFunc func)
        {
            base.ExecuteReader(
                    base.NewQuery(DbSql.Table_GetColumn,
                    base.Db.CreateParametersFromArray(
                        new object[,]{
                        {"@columnId", columnId}
                        })),
                    func
                     
                  );
        }

        public OperateResult UpdateColumn(TableColumn column)
        {
            int rowCount = base.ExecuteNonQuery(
                      base.NewQuery(DbSql.Table_UpdateColumn,
                      base.Db.CreateParametersFromArray(
                          new object[,]{
                      {"@name", column.Name},
                      {"@note", column.Note},
                      {"@validformat", column.ValidFormat},
                      {"@sortNumber", column.SortNumber},
                      {"@columnId", column.Id}
                          }))
                    );
            return rowCount == 1 ? OperateResult.Success : OperateResult.Fail;
        }


        public OperateResult DeleteColumn(int tableId, int columnId)
        {
            OperateResult result = base.ExecuteNonQuery(
                        base.NewQuery(DbSql.Table_DeleteColumn,
                        base.Db.CreateParametersFromArray(
                            new object[,]{
                        {"@tableId", tableId},
                        {"@columnId", columnId}
                            }))
                        ) == 1
                        ? OperateResult.Success
                        : OperateResult.Fail;

            if (result == OperateResult.Success)
            {
                base.ExecuteNonQuery(
                        base.NewQuery(DbSql.Table_ClearDeletedColumnData,
                        base.Db.CreateParametersFromArray(
                            new object[,]{
                        {"@tableId", tableId},
                        {"@columnId", columnId}
                            }))
                        );
            }
            return result;
        }




        public int GetRowsCount(int tableId)
        {
           return int.Parse(base.ExecuteScalar(
                base.NewQuery(DbSql.Table_GetRowsCount
                , base.Db.CreateParametersFromArray(new object[,]{{"@tableId",tableId}}))
                ).ToString());
        }


        public int CreateRow(int tableId, TableRowData[] rows)
        {
            DataBaseAccess db = base.Db;
            int rowID = 0;
            string date = String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);

            base.ExecuteNonQuery(
                base.NewQuery(DbSql.Table_CreateRow,
                base.Db.CreateParametersFromArray(
                    new object[,]{
                {"@tableId", tableId},
                {"@submittime",date}
                    })));

            //获取生成的行编号
            rowID = int.Parse(base.ExecuteScalar(base.NewQuery(DbSql.TableGetLastedRowId,null)).ToString());

            foreach (TableRowData row in rows)
            {
                var i = rowID;
                base.ExecuteNonQuery(
                     base.NewQuery(DbSql.TableInsertRowData,
                     base.Db.CreateParametersFromArray(
                         new object[,]{
                     {"@rowId", rowID},
                     {"@columnId", row.Cid},
                     {"@value", row.Value}
                         }))
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
            const string sql1 = @"SELECT TOP $[pagesize] * FROM $PREFIX_table_row WHERE tableid=$[tableid] ORDER BY submittime DESC";


            //记录数
            recordCount = int.Parse(base.ExecuteScalar(
                base.NewQuery(DbSql.Table_GetRowsCount,
                base.Db.CreateParametersFromArray(
                    new object[,]{
                {"@tableId",tableID}
                    }))
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
                      base.OptimizeSql(sql1) :
                        base.OptimizeSql(DbSql.TableGetPagedRows);


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
                    base.NewQuery(DbSql.Table_ClearDeletedRowData,
                    base.Db.CreateParametersFromArray(
                        new object[,]{
                    {"@tableId", tableId},
                    {"@rowId", rowId}
                        }))
                    );

            //删除行
            return base.ExecuteNonQuery(
                      base.NewQuery(DbSql.Table_DeleteRow,
                      base.Db.CreateParametersFromArray(
                          new object[,]{
                      {"@tableId", tableId},
                      {"@rowId", rowId}
                          }))
                    ) == 1 ? OperateResult.Success : OperateResult.Fail;
        }

        [Obsolete]
        public void GetRow(int rowId,DataReaderFunc func)
        {
            base.ExecuteReader(
                     base.NewQuery(DbSql.Table_GetRow,
                     base.Db.CreateParametersFromArray(
                         new object[,]{
                            {"@rowId", rowId}
                         })),
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

            string sql = base.OptimizeSql(DbSql.table_GetRowData);
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
