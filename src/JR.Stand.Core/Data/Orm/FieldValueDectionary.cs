using System.Reflection;
using JR.Stand.Core.Data.Orm.Mapping;

namespace JR.Stand.Core.Data.Orm
{
    /// <summary>
    /// 数据库字段与值字典
    /// </summary>
    public class FieldValueDictionary
    {
        public FieldValueDictionary()
        {
        }

        public FieldValueDictionary(PropertyInfo proper, object value)
        {
            Field = proper;
            Value = value;
        }

        /// <summary>
        /// 数据库字段对应的属性
        /// </summary>
        public PropertyInfo Field { get; set; }

        /// <summary>
        /// 数据库字段对应属性的值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 是否对应数据库主键字段
        /// </summary>
        public virtual bool IsPrimaryKeyField
        {
            get
            {
                object[] attrs = Field.GetCustomAttributes(typeof (ColumnAttribute), false);
                if (attrs.Length > 0)
                {
                    ColumnAttribute att = attrs[0] as ColumnAttribute;
                    return att != null && att.IsPrimaryKey;
                }
                return false;
            }
        }

        /// <summary>
        /// 获取属性对应的数据库字段
        /// </summary>
        /// <returns></returns>
        public string GetFieldName()
        {
            object[] attrs = Field.GetCustomAttributes(typeof (ColumnAttribute), true);
            if (attrs.Length == 0) return null;
            ColumnAttribute attr = attrs[0] as ColumnAttribute;
            return attr.Name == null || attr.Name == "" ? Field.Name : attr.Name;
        }
    }
}