//
// Form
// Copryright 2011 @ TO2.NET,All rights reserved !
// Create by newmin @ 2014-01-06
//

using System;

namespace JR.Cms.Domain.Interface.Models
{
    /// <summary>
    /// 表格
    /// </summary>
    public class Table
    {
        /// <summary>
        /// 编号,自动编号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 表格名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 表格说明
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 接口服务器地址
        /// </summary>
        public string ApiServer { get; set; }

        /// <summary>
        /// 发送短信
        /// </summary>
        //public bool SendSms{ get; set; }

        /// <summary>
        /// 发送邮箱
        /// </summary>
        //public bool SendMail { get; set; }

        /// <summary>
        /// 是否为系统表格
        /// </summary>
        public bool IsSystem { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        public bool Enabled { get; set; }
    }

    /// <summary>
    /// 表格列
    /// </summary>
    public class TableColumn
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 表格编号
        /// </summary>
        public int TableId { get; set; }

        /// <summary>
        /// 列名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 列说明
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 验证格式
        /// </summary>
        public string ValidFormat { get; set; }

        /// <summary>
        /// 排列序号
        /// </summary>
        public int SortNumber { get; set; }
    }

    /// <summary>
    /// 表格行
    /// </summary>
    public class TableRow
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 表格编号
        /// </summary>
        public int TableId { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        public DateTime SubmitTime { get; set; }
    }


    /// <summary>
    /// 表行数据
    /// </summary>
    public class TableRowData
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 行号
        /// </summary>
        public int Rid { get; set; }

        /// <summary>
        /// 列号
        /// </summary>
        public int Cid { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public string Value { get; set; }
    }
}