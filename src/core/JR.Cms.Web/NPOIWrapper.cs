using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using NPOI.HSSF;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace Spc
{
    public class NPOIWrapper
    {
        public static MemoryStream ExportExcel(DataTable dt,params string[] customColumnName)
        {
            //HttpResponse rsp = HttpContext.Current.Response;



            HSSFWorkbook wb = new HSSFWorkbook();
            MemoryStream ms = new MemoryStream();

            ISheet sheet = wb.CreateSheet("Sheet1");

            //填充表头
            IRow dataRow = sheet.CreateRow(0);

            int tmpInt = 0;

            if (customColumnName!=null && customColumnName.Length != 0)
            {
                foreach (string columnName in customColumnName)
                {
                    dataRow.CreateCell(tmpInt++).SetCellValue(columnName);
                }
            }
            else
            {
                foreach (DataColumn column in dt.Columns)
                {
                    dataRow.CreateCell(tmpInt++).SetCellValue(column.ColumnName);
                }
            }

            //填充内容
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dataRow = sheet.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    dataRow.CreateCell(j).SetCellValue(dt.Rows[i][j].ToString());
                }
            }


            wb.Write(ms);
            //rsp.AddHeader("Content-Disposition", string.Format("attachment; filename=EmptyWorkbook.xls"));
            //rsp.BinaryWrite(ms.ToArray());

            wb = null;
            ms.Close();
            return ms;
        }
    }
}