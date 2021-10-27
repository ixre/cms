/*
* Copyright(C) 2010-2013 OPS.CC
* 
* File Name	: ITableDAL
* Author	: Newmin (new.min@msn.com)
* Create	: 2012-01-06 16:33:33
* Description :
*
*/


using System.Data;
using JR.Cms.Domain.Interface.Models;
using JR.Cms.Infrastructure;
using JR.Stand.Core.Data;

namespace JR.Cms.Library.DataAccess.IDAL
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITableDAL
    {
        /// <summary>
        /// 添加表格
        /// </summary>
        /// <param name="form"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        OperateResult AddTable(Table form, TableColumn[] columns);

        /// <summary>
        /// 删除表格
        /// </summary>
        /// <param name="tableId"></param>
        /// <param name="tableId"></param>
        /// <returns></returns>
        OperateResult DeleteTable(int tableId);

        /// <summary>
        /// 更新表格
        /// </summary>
        /// <param name="form"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        OperateResult UpdateTable(Table form, TableColumn[] columns);

        /// <summary>
        /// 获取表格
        /// </summary>
        /// <param name="tableId"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        void GetTable(int tableId, DataReaderFunc func);

        /// <summary>
        /// 获取所有表格
        /// </summary>
        /// <returns></returns>
        void GetTables(DataReaderFunc func);

        /// <summary>
        /// 获取表格列
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns></returns>
        void GetColumns(int tableId, DataReaderFunc func);

        /// <summary>
        /// 添加列
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        OperateResult AddColumn(TableColumn column);

        /// <summary>
        /// 获取列
        /// </summary>
        /// <param name="columnId"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        void GetColumn(int columnId, DataReaderFunc func);

        /// <summary>
        /// 更新列
        /// </summary>
        /// <param name="tableColumn"></param>
        /// <returns></returns>
        OperateResult UpdateColumn(TableColumn tableColumn);

        /// <summary>
        /// 删除列
        /// </summary>
        /// <param name="tableId"></param>
        /// <param name="columnId"></param>
        /// <returns></returns>
        OperateResult DeleteColumn(int tableId, int columnId);

        /// <summary>
        /// 获取行数
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns></returns>
        int GetRowsCount(int tableId);

        /// <summary>
        /// 创建行并返回行的行号
        /// </summary>
        /// <param name="tableId"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        int CreateRow(int tableId, TableRowData[] rows);

        /// <summary>
        /// 获取分页表格记录
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="pageSize"></param>
        /// <param name="currentPageIndex"></param>
        /// <param name="rowCount"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        DataTable GetPagedRows(int tableid, string keyword, int pageSize, int currentPageIndex, out int rowCount,
            out int pageCount);

        /// <summary>
        /// 删除表格记录
        /// </summary>
        /// <param name="tableId"></param>
        /// <param name="rowId"></param>
        /// <returns></returns>
        OperateResult DeleteRow(int tableId, int rowId);

        /// <summary>
        /// 获取表格记录
        /// </summary>
        /// <param name="rowId"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        void GetRow(int rowId, DataReaderFunc func);

        /// <summary>
        /// 获取行数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        DataTable GetRowsData(int[] ids);
    }
}