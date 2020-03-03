/*-----------------------------------
 * Name   :   DataColumnMappingAttribute
 * Author   :   Sonven
 * Create   :   2009-12-04 10:15
 * LastModify   
 * Note 
 * ---------------------------------*/

using System;

namespace JR.Stand.Core.Data.Orm.Mapping
{
    /// <summary>
    ///提供对数据表的列的映射
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute()
        {
        }

        public ColumnAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 列名,如果列名与属性名称相同可省略
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否为主键
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// 是否由数据库自动创建
        /// </summary>
        public bool AutoGeneried { get; set; }

        /// <summary>
        /// 匹配模式(Regex,用于验证数据是否正确)
        /// </summary>
        public string Parttern { get; set; }
    }
}