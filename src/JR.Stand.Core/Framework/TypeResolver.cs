using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JR.Stand.Core.Framework
{
    /// <summary>
    /// 类型仓库
    /// </summary>
    public class TypeRegistry
    {
        private static IDictionary<int, TypeResolver> typeMap = new Dictionary<int, TypeResolver>();
        /// <summary>
        /// 创建类型解析器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static TypeResolver Create<T>()
        {
            Type t = typeof(T);
            int hash = t.GetHashCode();
            if (!typeMap.ContainsKey(hash))
            {
                typeMap[hash] = new TypeResolver(t);
            }
            return typeMap[hash];
        }
    }

    /// <summary>
    /// 别名特性
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class AliasAttribute:Attribute
    {
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="name"></param>
        public AliasAttribute(String name)
        {
            this.Name = name;
        }
        /// <summary>
        /// 名称
        /// </summary>
        public String Name { get; set; }
    }

    /// <summary>
    /// 类型解析器
    /// </summary>
    public class TypeResolver
    {
        private Type t;
        private Dictionary<string, PropertyInfo> properties;
        private Dictionary<string, PropertyInfo> aliasPropMap;
        private static Type aliasAttrType = typeof(AliasAttribute);

        /// <summary>
        /// 创建类型解析器
        /// </summary>
        /// <param name="t"></param>
        internal TypeResolver(Type t)
        {
            this.t = t;
            this.loadProperties();
        }


        internal PropertyInfo[] GetTSetterProperties()
        {
            return new List<PropertyInfo>(this.GetEnumeratorProperties()).ToArray();
        }

        private IEnumerable<PropertyInfo> GetEnumeratorProperties()
        {
            PropertyInfo[] props = this.t.GetProperties();

            foreach (PropertyInfo pro in props)
            {
                if (pro.CanWrite && pro.GetIndexParameters().Length == 0)
                {
                    yield return pro;
                }
            }
        }

        /// <summary>
        /// 查找属性
        /// </summary>
        /// <param name="exp">表达式</param>
        /// <returns></returns>
        public PropertyInfo FindProperty(Func<PropertyInfo, bool> exp)
        {
            return this.properties.Values.FirstOrDefault(exp);
        }

        private void loadProperties()
        {
            if (this.properties != null) return;
            this.properties = new Dictionary<String, PropertyInfo>();
            this.aliasPropMap = new Dictionary<string, PropertyInfo>();
            PropertyInfo[] props = this.t.GetProperties();
            foreach (PropertyInfo pro in props)
            {
                this.properties[pro.Name] = pro;
                object[] arr = pro.GetCustomAttributes(aliasAttrType, true);
                foreach (Object o in arr)
                {
                    if (o is AliasAttribute)
                    {
                        this.aliasPropMap[(o as AliasAttribute).Name] = pro;
                        break;
                    }
                }
            }

            //if (pro.CanWrite && pro.GetIndexParameters().Length == 0)
            //{
            //}
        }

        /// <summary>
        /// 根据名称查找属性
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public PropertyInfo FindPropertyByName(string columnName)
        {
            if (this.properties.ContainsKey(columnName))
            {
                return this.properties[columnName];
            }
            return null;
        }

        /// <summary>
        /// 根据别名查找属性
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public PropertyInfo FindPropertyByAlias(string alias)
        {
            if (this.aliasPropMap.ContainsKey(alias))
            {
                return this.aliasPropMap[alias];
            }
            return null;
        }

        /// <summary>
        /// 根据名称或别名查找属性
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public PropertyInfo FindPropertyByNameOrAlias(string name)
        {
            PropertyInfo p = this.FindPropertyByName(name);
            if(p == null)
            {
                p= this.FindPropertyByAlias(name);
            }
            return p;
        }

        /// <summary>
        /// 获取属性数量
        /// </summary>
        /// <returns></returns>
        public int PropertyLen()
        {
            return this.properties.Count;
        }
    }
}
