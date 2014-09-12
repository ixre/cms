using Ops.Cms.Infrastructure;

namespace Ops.Cms.Domain.Interface._old
{
    public interface ITable
    {
        OperateResult AddColumn(Spc.Models.TableColumn column);
        OperateResult AddTable(Spc.Models.Table table, Spc.Models.TableColumn[] columns);
        OperateResult DeleteColumn(int tableID, int columnID);
        OperateResult DeleteRow(int tableId, int rowId);
        OperateResult DeleteTable(int tableID);
        Spc.Models.TableColumn GetColumn(int columnID);
        System.Collections.Generic.IList<Spc.Models.TableColumn> GetColumns(int tableID);
        System.Data.DataTable GetPagedRecords(int tableID, string keyword, int pageSize, int currentPageIndex, out int recordCount, out int pageCount);
        Spc.Models.TableRow GetRecord(int rowId);
        int GetRowsCount(int tableID);
        Spc.Models.Table GetTable(int tableID);
        System.Collections.Generic.IList<Spc.Models.Table> GetTables();
        int SubmitRow(int tableId, System.Collections.Specialized.NameValueCollection form);
        OperateResult UpdateColumn(Spc.Models.TableColumn tableColumn);
        OperateResult UpdateTable(Spc.Models.Table table, Spc.Models.TableColumn[] columns);
    }
}
