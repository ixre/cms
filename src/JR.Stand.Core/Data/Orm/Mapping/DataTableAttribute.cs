/* ---------------------------------
 * Name   :   DataTableMapping
 * Author   :   Sonven
 * Create   :   2009-12-04  10:20
 * LastModify
 * Note
 * ---------------------------------*/

using System;

namespace JR.Stand.Core.Data.Orm.Mapping
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class DataTableAttribute : Attribute
    {
        private string name;
        private Type type;
        private ISqlFormat format;

        public DataTableAttribute(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// 数据库表的名称
        /// </summary>
        public string Name
        {
            get
            {
                if (name == null)
                    throw new DataMappingException("数据映射的表名不能为空!");
                return this.format == null ? name : this.format.Format(name);
            }
            set { name = value; }
        }

        /// <summary>
        /// 实现了Ops.Data.ISqlFormat接口的类型
        /// </summary>
        public Type Format
        {
            get { return type; }
            set
            {
                type = value;
                format = Activator.CreateInstance(value) as ISqlFormat;
            }
        }
    }
}