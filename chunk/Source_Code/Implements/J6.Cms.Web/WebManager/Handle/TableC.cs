//
// Copyright (C) 2007-2008 S1N1.COM,All rights reseved.
// 
// Project: J6.Cms.Manager
// FileName : Table.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2013-01-06 10:56:06
// Description :
//
// Get infromation of this software,please visit our site http://J6.Cms.cc
//
//

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using J6.Cms.BLL;
using J6.Cms.Domain.Interface.Models;
using J6.Cms.Domain.Interface._old;
using J6.Cms.WebManager;

namespace J6.Cms.Web.WebManager.Handle
{
    /// <summary>
    /// 文档
    /// </summary>
    public class TableC : BasePage
    {

        /// <summary>
        /// 创建文档
        /// </summary>
        public void Create_GET()
        {
            base.RenderTemplate(ResourceMap.Edittable, new
            {
                tableid="",
                name = String.Empty,
                note = String.Empty,
                apiserver = String.Empty,
                issystem = String.Empty,
                isavailable = " checked=\"checked\"" 
            });
        }
        public void Create_POST()
        {
            var form =base.Request.Form;
            int result=(int)CmsLogic.Table.AddTable(new Table
            {
                IsSystem = form["issystem"] == "on",
                Available = form["isavailable"] == "on",
                Name = form["name"],
                Note = form["note"],
                ApiServer = form["apiserver"]
            }, null);

            base.Response.Write(result.ToString());
        }
        public void EditTable_GET()
        {
            int tableID = int.Parse(base.Request["tableid"]);
           Table table =CmsLogic.Table.GetTable(tableID);

            base.RenderTemplate(ResourceMap.Edittable, new
            {
                tableid=tableID,
                name = table.Name,
                note = table.Note,
                apiserver = table.ApiServer,
                issystem = table.IsSystem ? " checked=\"checked\"" : String.Empty,
                isavailable = table.Available ? " checked=\"checked\"" : String.Empty
            });
        }
        public void EditTable_POST()
        {
            var form = base.Request.Form;
            int result = (int)CmsLogic.Table.UpdateTable(new Table
            {
                 Id=int.Parse(form["tableid"]),
                IsSystem = form["issystem"] == "on",
                Available = form["isavailable"] == "on",
                Name = form["name"],
                Note = form["note"],
                ApiServer = form["apiserver"]
            }, null);

            base.Response.Write(result.ToString());
        }
        public void All_GET()
        {
            StringBuilder sb = new StringBuilder(500);
            ITable tbll=CmsLogic.Table;
            
            IList<Table> list=tbll.GetTables();
            IList<TableColumn> clist;

            if (list == null || list.Count == 0)
            {
                sb.Append("<tr><td colspan=\"10\" class=\"center hightlight\">还未添加表！</td></tr>");
            }
            else
            {
                foreach (Table t in list)
                {
                    clist = tbll.GetColumns(t.Id);
                    sb.Append("<tr><td class=\"center\">").Append(t.Id.ToString())
                        .Append("</td><td>").Append(t.Name).Append("</td><td class=\"center\"><a href=\"?module=table&action=columns&control=true&tableid=")
                        .Append(t.Id.ToString()).Append("\">").Append(clist == null ? "0" : clist.Count.ToString())
                        .Append("</a></td><td class=\"center\"><a href=\"?module=table&action=rows&control=true&tableid=")
                        .Append(t.Id.ToString()).Append("\">").Append(tbll.GetRowsCount(t.Id).ToString()).Append("</a></td><td class=\"center\">")
                        .Append(t.Available ? ResourceMap.RightText : ResourceMap.ErrorText)
                        .Append("</td><td class=\"center\">").Append(t.IsSystem ? ResourceMap.RightText : ResourceMap.ErrorText)
                        .Append("</td><td>").Append(String.IsNullOrEmpty(t.ApiServer) ? "<span class=\"center\">-</span>" : t.ApiServer).Append("</td>")
                        .Append("<td class=\"center\"><a style=\"margin:0;\" href=\"?module=table&action=columns&control=true&tableid=").Append(t.Id.ToString()).Append("\">列</a> / ")
                        .Append("<a href=\"?module=table&action=rows&control=true&tableid=").Append(t.Id.ToString()).Append("\">行</a></td>")
                        .Append("<td class=\"center\"><button class=\"edit\" /></td><td><button class=\"delete\" /></td>")
                        .Append("</tr>");
                }
            }
            base.RenderTemplate(ResourceMap.Tables, new
            {
                tableListHtml=sb.ToString(),
                count=list.Count.ToString()
            });
        }
        public void DeleteTable_POST()
        {
            int tableID = int.Parse(base.Request["tableid"]);
            int result=(int) CmsLogic.Table.DeleteTable(tableID);
            base.Response.Write(result.ToString());
        }

        public void Columns_GET()
        {
            int tableID;
            string tableName;

            tableID=int.Parse(base.Request["tableid"]);

            //判断表格状态并取得名称
            Table table = CmsLogic.Table.GetTable(tableID);
            if (table == null) return;
            tableName = table.Name;


            StringBuilder sb = new StringBuilder();
            IList<TableColumn> list = CmsLogic.Table.GetColumns(tableID);
            if (list == null || list.Count == 0)
            {
                sb.Append("<tr><td colspan=\"8\" class=\"center hightlight\">暂无任何列，您可以<a href=\"?module=table&action=createcolumn&tableid=")
                    .Append(tableID.ToString()).Append("\">添加列</a></td></tr>");
            }
            else
            {
                int i = 0;
                foreach (TableColumn t in list)
                {
                    sb.Append("<tr><td class=\"hidden\">").Append(t.Id.ToString())
                        .Append("</td><td class=\"center\">").Append((++i).ToString()).Append("</td><td>").Append(t.Name).Append("</td><td class=\"center\">")
                        .Append(t.Note).Append("</td><td class=\"center\">").Append(t.ValidFormat)
                        .Append("</td><td class=\"center\">").Append(t.OrderIndex.ToString()).Append("</td>")
                        .Append("<td class=\"center\"><button class=\"edit\" /></td><td class=\"center\"><button class=\"delete\" /></td>")
                        .Append("</tr>");
                }
            }
            base.RenderTemplate(ResourceMap.Columns, new
            {
                tableid=tableID.ToString(),
                tableName=tableName,
                count=list.Count.ToString(),
                columnListHtml=sb.ToString()
            });
        }
        public void CreateColumn_GET()
        {
            int tableID = int.Parse(base.Request["tableid"]);
            //判断表格状态并取得名称
            Table table =CmsLogic.Table.GetTable(tableID);
            if (table == null) return;

            base.RenderTemplate(ResourceMap.Editcolumn, new
            {
                tableName=table.Name,
                name=string.Empty,
                note=string.Empty,
                validformat=string.Empty,
                orderindex=1,
                tableid=int.Parse(base.Request["tableid"]),
                columnid=""
            });
        }
        public void CreateColumn_POST()
        {
            var form = base.Request.Form;
            int tableID = int.Parse(form["tableid"]);
            int result = (int)CmsLogic.Table.AddColumn(new TableColumn
            {
                TableId = tableID,
                Name = form["name"],
                Note = form["note"],
                ValidFormat = form["validformat"],
                OrderIndex = int.Parse(form["orderindex"])
            });
            base.Response.Write(result.ToString());
        }
        public void EditColumn_GET()
        {
            int tableID = int.Parse(base.Request["tableid"]);
            int columnID = int.Parse(base.Request["columnid"]);

            //判断表格状态并取得名称
            Table table = CmsLogic.Table.GetTable(tableID);
            if (table == null) return;


            TableColumn column =CmsLogic.Table.GetColumn(columnID);

            base.RenderTemplate(ResourceMap.Editcolumn, new
            {
                tableName=table.Name,
                name =column.Name,
                note = column.Note,
                validformat = column.ValidFormat,
                orderindex = column.OrderIndex.ToString(),
                tableid=column.TableId.ToString(),
                columnid = columnID.ToString()
            });
        }
        public void EditColumn_POST()
        {
            var form = base.Request.Form;
            int columnID = int.Parse(form["columnid"]);
            int result = (int)CmsLogic.Table.UpdateColumn(new TableColumn
            {
                Id = columnID,
                Name = form["name"],
                Note = form["note"],
                ValidFormat = form["validformat"],
                OrderIndex = int.Parse(form["orderindex"])
            });
            base.Response.Write(result.ToString());
        }
        public void DeleteColumn_POST()
        {
            int columnID = int.Parse(base.Request["columnid"]);
            int tableID = int.Parse(base.Request["tableid"]);

            int result = (int)CmsLogic.Table.DeleteColumn(tableID,columnID);
            base.Response.Write(result.ToString());
        }


        public void Rows_GET()
        {
            int pageSize; 
            int recordCount;
            int pageCount;
            int pageIndex;
            string pagerHtml;         //分页
            string columnsHtml;      //列
            string rowsHtml;            //行
            string controlHtml;
            string tableName;

            
            ITable tbll=CmsLogic.Table;
            
            int tableID = int.Parse(base.Request["tableid"]);
            string keywords = base.Request["keyword"];
            int.TryParse(base.Request["page"], out pageIndex);
            int.TryParse(base.Request["size"], out pageSize);
            if (pageIndex == 0) pageIndex = 1;
            if (pageSize == 0) pageSize = 10;
            
            StringBuilder sb = new StringBuilder();

            //判断表格状态并取得名称
            Table table = CmsLogic.Table.GetTable(tableID);
            if (table == null) return;
            tableName = table.Name;

            //控制表格操作
            if (base.Request["control"] == "true")
            {
                controlHtml = String.Format(@"<a href=""?module=table&amp;action=all"">所有表</a>
                                                                  <a href=""?module=table&action=columns&tableid={0}"">结构</a>
                                                                  <a href=""javascript:;""  class=""current"">数据</a>", tableID.ToString());
            }
            else
            {
                controlHtml = @"<a href=""javascript:;""  class=""current"">数据</a>";
            }

            //生成表格数据
            DataTable dt=tbll.GetPagedRecords(tableID, keywords,pageSize, pageIndex, out recordCount, out pageCount);

            //判断表格列
            IList<TableColumn> columns = tbll.GetColumns(tableID);
            if (columns==null || columns.Count == 0)
            {
                pagerHtml = rowsHtml= String.Empty;
                columnsHtml = "<tr><td class=\"center hightlight\">暂无任何数据!</td></tr>";
                goto render;
            }

            //生成表头
            sb.Append("<tr><th class=\"hidden\">ID</th>");
            foreach (TableColumn tc in columns)
            {
                sb.Append("<th class=\"tleft\" style=\"text-align:left\">").Append(tc.Name).Append("</th>");
            }
            sb.Append("<th class=\"center\" width=\"30\">操作</th></tr>");
            columnsHtml = sb.ToString();

            //生成表行
            if (dt.Rows.Count==0)
            {
                rowsHtml = String.Format("<tr><td colspan=\"{0}\" class=\"center hightlight\">暂无任何行!</td></tr>", (columns.Count + 2).ToString());
                pagerHtml = String.Empty;
            }
            else
            {
                sb.Remove(0, sb.Length);        //清空内容

                foreach(DataRow dr in dt.Rows)
                {
                    sb.Append("<tr><td class=\"hidden\">").Append(dr["id"].ToString()).Append("</td>");
                    foreach(TableColumn tc in columns)
                    {
                        sb.Append("<td>").Append(dr["$"+tc.Id.ToString()]).Append("</td>");
                    }

                    sb.Append("<td class=\"center\"><button class=\"delete\" /></td></tr>");
                }

                rowsHtml = sb.ToString();

                string format = String.Format("?module=table&action=rows&tableid={0}&keyword={1}&size={2}&&page={3}",
                    tableID, HttpUtility.UrlEncode(keywords), pageSize, "{0}");

                pagerHtml = Helper.BuildPagerInfo(format, pageIndex, recordCount, pageCount);
            }

            render:
            base.RenderTemplate(ResourceMap.Rows, new
            {
                tableid = tableID.ToString(),
                tableName=tableName,
                controlHtml=controlHtml,
                columnsHtml=columnsHtml,
                pagerHtml=pagerHtml,
                rowsHtml = rowsHtml
            });
        }

        public void DeleteRow_POST()
        {
            int rowID = int.Parse(base.Request["rowid"]);
            int tableID = int.Parse(base.Request["tableid"]);

            int result = (int)CmsLogic.Table.DeleteRow(tableID,rowID);
            base.Response.Write(result.ToString());
        }

    }
}

