/*-------------------------------------------
 * Name   :   实体操作管理
 * Author   :   Sonven
 * Create   :   2009-10-06 10:30
 * LastModify   2009-10-08 14:17  
 * 
 * 1.修正Get方法如果没有返回数据行返回对象不为空 | Sonven 14:18 08/10
 *  
 * Note 
 * --------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using JR.Stand.Core.Data.Orm.Mapping;

namespace JR.Stand.Core.Data.Orm
{
    /// <summary>
    /// 实体管理器
    /// </summary>
    /// <typeparam name="Entity"></typeparam>
    public class EntityManager<Entity> : IEntityManager<Entity> where Entity : class
    {
        private string tableName;
        private PropertyInfo[] properties;
        private DataBaseAccess db;

        public EntityManager(DataBaseAccess db)
        {
            object[] attrs; //datatableAttribtue
            attrs = typeof (Entity).GetCustomAttributes(typeof (DataTableAttribute), true);
            if (attrs.Length == 0) throw new DataMappingException("此类未加上DataTable特性!");
            DataTableAttribute tb = attrs[0] as DataTableAttribute;
            tableName = tb.Name;

            properties = typeof (Entity).GetProperties();

            this.db = db;
        }

        /// <summary>
        /// 向数据库添加一个实体
        /// </summary>
        /// <param name="entity"></param>
        public void Insert(Entity entity)
        {
            ColumnAttribute col; //列

            string filedStr = null;
            string paramStr = null;
            IList<DbParameter> parameters = new List<DbParameter>();
            object[] attrs; //columnAttributes
            foreach (PropertyInfo p in properties)
            {
                attrs = p.GetCustomAttributes(typeof (ColumnAttribute), true);
                if (attrs.Length == 0) continue;
                col = attrs[0] as ColumnAttribute;
                if (col.AutoGeneried) continue; //如果为自动创建
                else
                {
                    if (col.Name == null) col.Name = p.Name;
                    filedStr += col.Name + ",";
                    paramStr += "@" + col.Name + ",";
                    parameters.Add(db.GetDialect().CreateParameter("@" + col.Name, p.GetValue(entity, null)));
                }
            }
            db.ExecuteNonQuery("insert into " + tableName + " (" + filedStr.Remove(filedStr.Length - 1) +
                               ")values(" + paramStr.Remove(paramStr.Length - 1) + ")",
                parameters.ToArray());
        }


        public void Save(object primaryValue, Entity entity)
        {
            if (primaryValue == null)
            {
                Insert(entity);
                return;
            }
            StringBuilder sb = new StringBuilder(300);
            sb.Append("UPDATE " + tableName + " SET ");

            IList<DbParameter> parameters = new List<DbParameter>();
            string fieldName = null;
            ColumnAttribute col; //列
            object[] attrs;
            string primaryKey = "id";

            foreach (PropertyInfo p in properties)
            {
                attrs = p.GetCustomAttributes(typeof (ColumnAttribute), true);
                if (attrs.Length == 0) continue;
                col = attrs[0] as ColumnAttribute;
                if (col.AutoGeneried) continue; //如果为自动创建
                else if (col.IsPrimaryKey)
                {
                    primaryKey = col.Name;
                    continue;
                }
                else
                {
                    if (col.Name == null) col.Name = p.Name;
                    sb.Append(col.Name).Append("=@").Append(col.Name).Append(",");
                    parameters.Add(db.GetDialect().CreateParameter("@" + col.Name, p.GetValue(entity, null)));
                }
            }

            sb.Remove(sb.Length - 1, 1); //删除最后的,


            // fieldName = primaryField.GetFieldName();
            sb.Append(" where " + primaryKey + "=@" + primaryKey);
            parameters.Add(db.GetDialect().CreateParameter("@" + primaryKey, primaryValue));
            db.ExecuteNonQuery(sb.ToString(), parameters.ToArray());
        }


        /// <summary>
        /// 根据主键删除数据库记录
        /// </summary>
        /// <param name="field"></param>
        public void Delete(FieldValueDictionary field)
        {
            string fieldName = field.GetFieldName();
            db.ExecuteNonQuery("delete from " + tableName + " where " + fieldName +
                               "=@" + fieldName, db.GetDialect().CreateParameter("@" + fieldName, field.Value));
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="primaryField"></param>
        /// <param name="fields"></param>
        public void Update(PrimaryKeyValueDictionary primaryField, params FieldValueDictionary[] fields)
        {
            StringBuilder sb = new StringBuilder(300);
            sb.Append("update " + tableName + " set ");

            IList<DbParameter> parameters = new List<DbParameter>();
            string fieldName = null;
            foreach (FieldValueDictionary f in fields)
            {
                fieldName = f.GetFieldName();
                sb.Append(fieldName + "=@" + fieldName + ",");
                parameters.Add(db.GetDialect().CreateParameter("@" + fieldName, f.Value));
            }
            sb.Remove(sb.Length - 1, 1); //删除最后的,
            fieldName = primaryField.GetFieldName();
            sb.Append(" where " + fieldName + "=@" + fieldName);
            parameters.Add(db.GetDialect().CreateParameter("@" + fieldName, primaryField.Value));

            db.ExecuteNonQuery(sb.ToString(), parameters.ToArray());
        }


        /// <summary>
        /// 根据主键获取单个实体
        /// </summary>
        /// <param name="primaryField"></param>
        /// <returns></returns>
        public Entity Get(PrimaryKeyValueDictionary primaryField)
        {
            string fieldName = primaryField.GetFieldName();
            string sql = "SELECT TOP 1 * FROM " + tableName +
                         " WHERE " + fieldName + "=@" + fieldName;

            Entity obj = null;
            ColumnAttribute attr;
            Type type = typeof (Entity);
            Assembly ass = type.Assembly;


            db.ExecuteReader(new SqlQuery(sql, new object[,]
            {
                {"@" + fieldName, primaryField.Value}
            }), rd =>
            {
                object[] attrs; //columnAttributes
                while (rd.Read())
                {
                    obj = ass.CreateInstance(type.ToString(), true) as Entity;
                    foreach (PropertyInfo p in properties)
                    {
                        attrs = p.GetCustomAttributes(typeof (ColumnAttribute), true);
                        if (attrs.Length == 0) continue;
                        attr = attrs[0] as ColumnAttribute;
                        p.SetValue(obj, Convert.ChangeType(rd[attr.Name ?? p.Name],
                            p.PropertyType), null);
                    }
                    break;
                }
            });

            return obj;
        }

        public Entity Get(FieldValueDictionary field)
        {
            string fieldName = field.GetFieldName();
            string sql = "SELECT TOP 1 * FROM " + tableName +
                         " WHERE " + fieldName + "=@" + fieldName;

            Entity obj = null;
            ColumnAttribute attr;
            Type type = typeof (Entity);
            Assembly ass = type.Assembly;


            db.ExecuteReader(new SqlQuery(sql, new object[,]
            {
                {"@" + fieldName, field.Value}
            }), rd =>
            {
                object[] attrs; //columnAttributes
                while (rd.Read())
                {
                    obj = ass.CreateInstance(type.ToString(), true) as Entity;
                    foreach (PropertyInfo p in properties)
                    {
                        attrs = p.GetCustomAttributes(typeof (ColumnAttribute), true);
                        if (attrs.Length == 0) continue;
                        attr = attrs[0] as ColumnAttribute;
                        p.SetValue(obj, Convert.ChangeType(rd[attr.Name ?? p.Name],
                            p.PropertyType), null);
                    }
                    break;
                }
            });

            return obj;
        }

        public IEnumerable<Entity> GetEntityList(params FieldValueDictionary[] fields)
        {
            string fieldName = null;
            IList<DbParameter> parameters = new List<DbParameter>();
            StringBuilder sb = new StringBuilder("select * from " + tableName, 400);

            if (fields.Count() != 0)
            {
                sb.Append(" WHERE ");
                int i = 0;
                foreach (FieldValueDictionary field in fields)
                {
                    fieldName = field.GetFieldName();
                    if (i != 0)
                    {
                        sb.Append("AND ");
                    }
                    sb.Append(fieldName + "=@" + fieldName).Append(" ");
                    parameters.Add(db.GetDialect().CreateParameter("@" + fieldName, field.Value));
                    ++i;
                }
            }

            ColumnAttribute attr;
            Type type = typeof (Entity);
            Assembly ass = type.Assembly;

            IList<Entity> list = new List<Entity>();
            Entity entity; //单个实体

            object[] attrs; //特性集合
            db.ExecuteReader(sb.ToString(),
                rd =>
                {
                    while (rd.Read())
                    {
                        entity = ass.CreateInstance(type.ToString(), true) as Entity;

                        foreach (PropertyInfo p in properties)
                        {
                            attrs = p.GetCustomAttributes(typeof (ColumnAttribute), true);
                            if (attrs.Length == 0) continue;
                            attr = attrs[0] as ColumnAttribute;

                            p.SetValue(entity, Convert.ChangeType(rd[attr.Name ?? p.Name], p.PropertyType), null);
                        }
                        list.Add(entity);
                    }
                },
                parameters.ToArray()); //获取数据


            return list;
        }
    }
}