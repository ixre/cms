using J6.Cms.Domain.Interface.Models;
using J6.Cms.Infrastructure;

namespace J6.Cms.Domain.Interface._old
{
    public interface ITable
    {
        OperateResult AddColumn(Models.TableColumn column);
        OperateResult AddTable(Models.Table table, Models.TableColumn[] columns);
        OperateResult DeleteColumn(int tableID, int columnID);
        OperateResult DeleteRow(int tableId, int rowId);
        OperateResult DeleteTable(int tableId);
        Models.TableColumn GetColumn(int columnID);
        System.Collections.Generic.IList<TableColumn> GetColumns(int tableID);
        System.Data.DataTable GetPagedRecords(int tableId, string keyword, int pageSize, int currentPageIndex, out int recordCount, out int pageCount);
        Models.TableRow GetRecord(int rowId);
        int GetRowsCount(int tableId);
        Models.Table GetTable(int tableId);
        System.Collections.Generic.IList<Table> GetTables();
        int SubmitRow(int tableId, System.Collections.Specialized.NameValueCollection form);
        OperateResult UpdateColumn(Models.TableColumn tableColumn);
        OperateResult UpdateTable(Models.Table table, Models.TableColumn[] columns);
    }
}
