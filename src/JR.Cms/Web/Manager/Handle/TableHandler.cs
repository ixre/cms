﻿//
// Copyright (C) 2007-2008 fze.NET,All rights reserved.
// 
// Project: jr.Cms.Manager
// FileName : Table.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2013-01-06 10:56:06
// Description :
//
// Get infromation of this software,please visit our site http://fze.NET/cms
//
//

using System.Collections.Generic;
using System.Data;
using System.Text;
using JR.Cms.Domain.Interface.Models;
using JR.Cms.Library.DataAccess.BLL;
using JR.Stand.Core.Web;

namespace JR.Cms.Web.Manager.Handle
{
    /// <summary>
    /// 文档
    /// </summary>
    public class TableHandler : BasePage
    {
        /// <summary>
        /// 创建文档
        /// </summary>
        public void Create()
        {
            RenderTemplate(ResourceMap.EditTable, new
            {
                table_id = 0,
                name = string.Empty,
                note = string.Empty,
                apiserver = string.Empty,
                issystem = string.Empty,
                isavailable = " checked=\"checked\""
            });
        }

        public void Create_POST()
        {
            var result = (int)CmsLogic.Table.AddTable(new Table
            {
                IsSystem = Request.Form("issystem") == "on",
                Enabled = Request.Form("isavailable") == "on",
                Name = Request.Form("name"),
                Note = Request.Form("note"),
                ApiServer = Request.Form("apiserver")
            }, null);

            Response.WriteAsync(result.ToString());
        }

        public void EditTable()
        {
            var tableId = int.Parse(Request.Query("table_id"));
            var table = CmsLogic.Table.GetTable(tableId);

            RenderTemplate(ResourceMap.EditTable, new
            {
                table_id = tableId,
                name = table.Name,
                note = table.Note,
                apiserver = table.ApiServer ?? "",
                issystem = table.IsSystem ? " checked=\"checked\"" : string.Empty,
                isavailable = table.Enabled ? " checked=\"checked\"" : string.Empty
            });
        }

        public void EditTable_POST()
        {
            var result = (int)CmsLogic.Table.UpdateTable(new Table
            {
                Id = int.Parse(Request.Form("TableId")),
                IsSystem = Request.Form("issystem") == "on",
                Enabled = Request.Form("isavailable") == "on",
                Name = Request.Form("name"),
                Note = Request.Form("note"),
                ApiServer = Request.Form("apiserver")
            }, null);

            Response.WriteAsync(result.ToString());
        }

        public void All()
        {
            var sb = new StringBuilder(500);
            var tbll = CmsLogic.Table;

            var list = tbll.GetTables();
            IList<TableColumn> clist;

            if (list == null || list.Count == 0)
                sb.Append("<tr><td colspan=\"10\" class=\"center hightlight\">还未添加表！</td></tr>");
            else
                foreach (var t in list)
                {
                    clist = tbll.GetColumns(t.Id);
                    sb.Append("<tr><td class=\"center\">").Append(t.Id.ToString())
                        .Append("</td><td>").Append(t.Name)
                        .Append(
                            "</td><td class=\"center\"><a href=\"?module=table&action=columns&control=true&table_id=")
                        .Append(t.Id.ToString()).Append("\">").Append(clist == null ? "0" : clist.Count.ToString())
                        .Append(
                            "</a></td><td class=\"center\"><a href=\"?module=table&action=rows&control=true&table_id=")
                        .Append(t.Id.ToString()).Append("\">").Append(tbll.GetRowsCount(t.Id).ToString())
                        .Append("</a></td><td class=\"center\">")
                        .Append(t.Enabled ? ResourceMap.RightText : ResourceMap.ErrorText)
                        .Append("</td><td class=\"center\">")
                        .Append(t.IsSystem ? ResourceMap.RightText : ResourceMap.ErrorText)
                        .Append("</td><td>")
                        .Append(string.IsNullOrEmpty(t.ApiServer) ? "<span class=\"center\">-</span>" : t.ApiServer)
                        .Append("</td>")
                        .Append(
                            "<td class=\"center\"><a style=\"margin:0;\" href=\"?module=table&action=columns&control=true&table_id=")
                        .Append(t.Id.ToString()).Append("\">列</a> / ")
                        .Append("<a href=\"?module=table&action=rows&control=true&table_id=").Append(t.Id.ToString())
                        .Append("\">行</a></td>")
                        .Append(
                            "<td class=\"center\"><button class=\"edit\" /></td><td><button class=\"delete\" /></td>")
                        .Append("</tr>");
                }

            RenderTemplate(ResourceMap.Tables, new
            {
                tableListHtml = sb.ToString(),
                count = list.Count.ToString()
            });
        }

        public void DeleteTable_POST()
        {
            var tableID = int.Parse(Request.Query("table_id"));
            var result = (int)CmsLogic.Table.DeleteTable(tableID);
            Response.WriteAsync(result.ToString());
        }

        public void Columns()
        {
            int tableID;
            string tableName;

            tableID = int.Parse(Request.Query("table_id"));

            //判断表格状态并取得名称
            var table = CmsLogic.Table.GetTable(tableID);
            if (table == null) return;
            tableName = table.Name;


            var sb = new StringBuilder();
            var list = CmsLogic.Table.GetColumns(tableID);
            if (list == null || list.Count == 0)
            {
                sb.Append(
                        "<tr><td colspan=\"8\" class=\"center hightlight\">暂无任何列，您可以<a href=\"?module=table&action=createcolumn&table_id=")
                    .Append(tableID.ToString()).Append("\">添加列</a></td></tr>");
            }
            else
            {
                var i = 0;
                foreach (var t in list)
                    sb.Append("<tr><td class=\"hidden\">").Append(t.Id.ToString())
                        .Append("</td><td class=\"center\">").Append((++i).ToString()).Append("</td><td>")
                        .Append(t.Name).Append("</td><td class=\"center\">")
                        .Append(t.Note).Append("</td><td class=\"center\">").Append(t.ValidFormat)
                        .Append("</td><td class=\"center\">").Append(t.SortNumber.ToString()).Append("</td>")
                        .Append(
                            "<td class=\"center\"><button class=\"edit\" /></td><td class=\"center\"><button class=\"delete\" /></td>")
                        .Append("</tr>");
            }

            RenderTemplate(ResourceMap.Columns, new
            {
                table_id = tableID.ToString(),
                tableName = tableName,
                count = list.Count.ToString(),
                columnListHtml = sb.ToString()
            });
        }

        public void CreateColumn()
        {
            var tableID = int.Parse(Request.Query("table_id"));
            //判断表格状态并取得名称
            var table = CmsLogic.Table.GetTable(tableID);
            if (table == null) return;

            RenderTemplate(ResourceMap.EditColumn, new
            {
                tableName = table.Name,
                name = string.Empty,
                note = string.Empty,
                validformat = string.Empty,
                orderindex = 1,
                table_id = int.Parse(Request.Query("table_id")),
                columnid = ""
            });
        }

        public void CreateColumn_POST()
        {
            var tableId = int.Parse(Request.Form("table_id"));
            var result = (int)CmsLogic.Table.AddColumn(new TableColumn
            {
                TableId = tableId,
                Name = Request.Form("name"),
                Note = Request.Form("note"),
                ValidFormat = Request.Form("validformat"),
                SortNumber = int.Parse(Request.Form("orderindex"))
            });
            Response.WriteAsync(result.ToString());
        }

        public void EditColumn()
        {
            var tableId = int.Parse(Request.Query("table_id"));
            var columnID = int.Parse(Request.Query("columnid"));

            //判断表格状态并取得名称
            var table = CmsLogic.Table.GetTable(tableId);
            if (table == null) return;


            var column = CmsLogic.Table.GetColumn(columnID);

            RenderTemplate(ResourceMap.EditColumn, new
            {
                tableName = table.Name,
                name = column.Name,
                note = column.Note,
                validformat = column.ValidFormat,
                orderindex = column.SortNumber.ToString(),
                table_id = column.TableId.ToString(),
                columnid = columnID.ToString()
            });
        }

        public void EditColumn_POST()
        {
            var columnID = int.Parse(Request.Form("columnid"));
            var result = (int)CmsLogic.Table.UpdateColumn(new TableColumn
            {
                Id = columnID,
                Name = Request.Form("name"),
                Note = Request.Form("note"),
                ValidFormat = Request.Form("validformat"),
                SortNumber = int.Parse(Request.Form("orderindex"))
            });
            Response.WriteAsync(result.ToString());
        }

        public void DeleteColumn_POST()
        {
            var columnID = int.Parse(Request.Query("columnid"));
            var tableId = int.Parse(Request.Query("table_id"));

            var result = (int)CmsLogic.Table.DeleteColumn(tableId, columnID);
            Response.WriteAsync(result.ToString());
        }


        public void Rows()
        {
            int pageSize;
            int recordCount;
            int pageCount;
            int pageIndex;
            string pagerHtml; //分页
            string columnsHtml; //列
            string rowsHtml; //行
            string controlHtml;
            string tableName;


            var tbll = CmsLogic.Table;

            var tableId = int.Parse(Request.Query("table_id"));
            string keywords = Request.Query("keyword");
            int.TryParse(Request.Query("page"), out pageIndex);
            int.TryParse(Request.Query("size"), out pageSize);
            if (pageIndex == 0) pageIndex = 1;
            if (pageSize == 0) pageSize = 10;

            var sb = new StringBuilder();

            //判断表格状态并取得名称
            var table = CmsLogic.Table.GetTable(tableId);
            if (table == null) return;
            tableName = table.Name;

            //控制表格操作
            if (Request.Query("control") == "true")
                controlHtml = string.Format(@"<a href=""?module=table&amp;action=all"">所有表</a>
                                                                  <a href=""?module=table&action=columns&table_id={0}"">结构</a>
                                                                  <a href=""javascript:;""  class=""current"">数据</a>",
                    tableId.ToString());
            else
                controlHtml = @"<a href=""javascript:;""  class=""current"">数据</a>";

            //生成表格数据
            var dt = tbll.GetPagedRecords(tableId, keywords, pageSize, pageIndex, out recordCount, out pageCount);

            //判断表格列
            var columns = tbll.GetColumns(tableId);
            if (columns == null || columns.Count == 0)
            {
                pagerHtml = rowsHtml = string.Empty;
                columnsHtml = "<tr><td class=\"center hightlight\">暂无任何数据!</td></tr>";
                goto render;
            }

            //生成表头
            sb.Append("<tr><th class=\"hidden\">ID</th>");
            foreach (var tc in columns)
                sb.Append("<th class=\"tleft\" style=\"text-align:left\">").Append(tc.Name).Append("</th>");
            sb.Append("<th class=\"center\" width=\"30\">操作</th></tr>");
            columnsHtml = sb.ToString();

            //生成表行
            if (dt.Rows.Count == 0)
            {
                rowsHtml = string.Format("<tr><td colspan=\"{0}\" class=\"center hightlight\">暂无任何行!</td></tr>",
                    (columns.Count + 2).ToString());
                pagerHtml = string.Empty;
            }
            else
            {
                sb.Remove(0, sb.Length); //清空内容

                foreach (DataRow dr in dt.Rows)
                {
                    sb.Append("<tr><td class=\"hidden\">").Append(dr["id"].ToString()).Append("</td>");
                    foreach (var tc in columns) sb.Append("<td>").Append(dr["$" + tc.Id.ToString()]).Append("</td>");

                    sb.Append("<td class=\"center\"><button class=\"delete\" /></td></tr>");
                }

                rowsHtml = sb.ToString();

                var format =
                    $"?module=table&action=rows&table_id={tableId}&keyword={HttpUtils.UrlEncode(keywords)}&size={pageSize}&&page={"{0}"}";

                pagerHtml = Helper.BuildPagerInfo(format, pageIndex, recordCount, pageCount);
            }

        render:
            RenderTemplate(ResourceMap.Rows, new
            {
                tableid = tableId.ToString(),
                tableName = tableName,
                controlHtml = controlHtml,
                columnsHtml = columnsHtml,
                pagerHtml = pagerHtml,
                rowsHtml = rowsHtml
            });
        }

        public void DeleteRow_POST()
        {
            var rowID = int.Parse(Request.Query("rowid"));
            var tableId = int.Parse(Request.Query("table_id"));

            var result = (int)CmsLogic.Table.DeleteRow(tableId, rowID);
            Response.WriteAsync(result.ToString());
        }
    }
}