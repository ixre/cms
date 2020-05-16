using JR.Cms.Domain.Interface.Models;
using JR.Cms.Infrastructure;

namespace JR.Cms.Domain.Interface._old
{
    public interface ITable
    {
        OperateResult AddColumn(TableColumn column);
        OperateResult AddTable(Table table, TableColumn[] columns);
        OperateResult DeleteColumn(int tableID, int columnID);
        OperateResult DeleteRow(int tableId, int rowId);
        OperateResult DeleteTable(int tableId);
        TableColumn GetColumn(int columnID);
        System.Collections.Generic.IList<TableColumn> GetColumns(int tableID);

        System.Data.DataTable GetPagedRecords(int tableId, string keyword, int pageSize, int currentPageIndex,
            out int recordCount, out int pageCount);

        TableRow GetRecord(int rowId);
        int GetRowsCount(int tableId);
        Table GetTable(int tableId);
        System.Collections.Generic.IList<Table> GetTables();
        int SubmitRow(int tableId, System.Collections.Specialized.NameValueCollection form);
        OperateResult UpdateColumn(TableColumn tableColumn);
        OperateResult UpdateTable(Table table, TableColumn[] columns);
    }
}