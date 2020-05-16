/*
* Copyright(C) 2010-2012 OPSoft Inc
* 
* File Name	: ExtendAttr
* Author	: Newmin (new.min@msn.com)
* Create	: 2012/9/30 9:54:29
* Description	:
*
*/

using System;
using JR.Stand.Core.Data.Orm.Mapping;

namespace ToEntity
{
    /// <summary>
    /// 模块属性
    /// </summary>
    [DataTable("$PREFIX_dataExtendAttr")]
    public class DataExtendAttr
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 所属扩展编号
        /// </summary>
        [Column("ExtID")]
        public int ExtendID { get; set; }

        /// <summary>
        /// 属性键
        /// </summary>
        [Obsolete]
        public string Key { get; set; }

        /// <summary>
        /// 属性名称
        /// </summary>
        public string AttrName { get; set; }

        /// <summary>
        /// UI类型
        /// </summary>
        public string AttrType { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public string AttrVal { get; set; }

        /// <summary>
        /// 属性是否可用
        /// </summary>
        public bool Enabled { get; set; }
    }
}
