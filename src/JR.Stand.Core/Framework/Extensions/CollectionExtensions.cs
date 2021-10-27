/*
 * Copyright 2010 OPS.CC,All rights reserved !
 * name     : 集合扩展
 * author   : newmin
 * date     : 2010/11/26 07:31
 */

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace JR.Stand.Core.Framework.Extensions
{
    /// <summary>
    /// 集合扩展
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// 返回符合条件的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static IEnumerable<T> WhereBy<T>(this IEnumerable<T> collection, Func<T, bool> condition)
        {
            foreach (T t in collection)
            {
                if (condition(t)) yield return t;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ConvertToEntity<T>(this NameValueCollection form)
        {
            return BindToEntity<T>(form, default(T), false);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ConvertToEntity<T>(this IFormCollection form)
        {
            return BindToEntity<T>(form, default(T), false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <param name="t"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T BindToEntity<T>(this NameValueCollection form, T t)
        {
            return BindToEntity<T>(form, t, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <param name="entity"></param>
        /// <param name="allowError"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public static T BindToEntity<T>(this NameValueCollection form, T entity, bool allowError)
        {
            Type type = typeof (T); //实体类型
            Type strType = typeof (String); //字符串类型
            Type bitType = typeof (Boolean);
            Type proType = null; //属性类型

            object t = entity;
            if (t == null)
            {
                t = Activator.CreateInstance(type);
            }

            PropertyInfo[] pros = type.GetProperties();
            string value;

            foreach (PropertyInfo pro in pros)
            {
                if (!pro.CanWrite) continue;
                value = form[pro.Name];

                //获取值
                if (value == null)
                {
                    value = form["field_" + pro.Name];
                    if (String.IsNullOrEmpty(value)) continue;
                }


                proType = pro.PropertyType;
                object obj;

                try
                {
                    obj = GetPropertyValue<T>(pro, strType, bitType, proType, value);
                    if (obj != null)
                    {
                        pro.SetValue(t, obj, null);
                    }

                    //if (pro.PropertyType.IsValueType)
                    //{
                    //        }
                    //        else
                    //        {
                    //            pro.SetValue(t,
                    //                Convert.ChangeType(value, pro.PropertyType),
                    //                null);
                    //        }

                    //    }


                    //if (pro.PropertyType.IsValueType)
                    //{
                    //        }
                    //        else
                    //        {
                    //            pro.SetValue(t,
                    //                Convert.ChangeType(value, pro.PropertyType),
                    //                null);
                    //        }

                    //    }

                    //else if (pro.PropertyType == typeof(String))
                    //{
                    //    pro.SetValue(t,
                    //        Convert.ChangeType(value, pro.PropertyType),
                    //        null);
                    //}
                }

                catch (FormatException exc)
                {
                    if (!allowError)
                    {
                        throw new FormatException("转换错误,属性名：" + pro.Name);
                    }
                }
            }

            return (T) t;
        }

         /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <param name="entity"></param>
        /// <param name="allowError"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public static T BindToEntity<T>(this IFormCollection form, T entity, bool allowError)
        {
            Type type = typeof (T); //实体类型
            Type strType = typeof (String); //字符串类型
            Type bitType = typeof (Boolean);
            Type proType = null; //属性类型

            object t = entity;
            if (t == null)
            {
                t = Activator.CreateInstance(type);
            }

            PropertyInfo[] pros = type.GetProperties();

            foreach (PropertyInfo pro in pros)
            {
                if (!pro.CanWrite) continue;
                string value = form[pro.Name];

                //获取值
                if (value == null)
                {
                    value = form["field_" + pro.Name];
                    if (String.IsNullOrEmpty(value)) continue;
                }


                proType = pro.PropertyType;

                try
                {
                    var obj = GetPropertyValue<T>(pro, strType, bitType, proType, value);
                    if (obj != null)
                    {
                        pro.SetValue(t, obj, null);
                    }

                    //if (pro.PropertyType.IsValueType)
                    //{
                    //        }
                    //        else
                    //        {
                    //            pro.SetValue(t,
                    //                Convert.ChangeType(value, pro.PropertyType),
                    //                null);
                    //        }

                    //    }


                    //if (pro.PropertyType.IsValueType)
                    //{
                    //        }
                    //        else
                    //        {
                    //            pro.SetValue(t,
                    //                Convert.ChangeType(value, pro.PropertyType),
                    //                null);
                    //        }

                    //    }

                    //else if (pro.PropertyType == typeof(String))
                    //{
                    //    pro.SetValue(t,
                    //        Convert.ChangeType(value, pro.PropertyType),
                    //        null);
                    //}
                }

                catch (FormatException exc)
                {
                    if (!allowError)
                    {
                        throw new FormatException("转换错误,属性名：" + pro.Name);
                    }
                }
            }

            return (T) t;
        }

         
        public static object GetPropertyValue<T>(PropertyInfo pro, Type strType, Type bitType, Type proType,
            string value)
        {
            if (proType == strType)
            {
                return value;
            }
            else if (proType == bitType)
            {
                return value == "True" || value == "1" || value == "true";
            }
            else if (proType.IsEnum) //枚举类型
            {
                return Enum.Parse(proType, value);
            }
            else if (proType.GetInterface("IConvertible") != null)
            {
                //支持转换的类型
                return Convert.ChangeType(value, proType);
            }
            else if (pro.PropertyType.IsGenericType)
            {
                //泛型(可空类型)
                //获取类型定义判断是否为可空类型
                Type genericTypeDefinition = proType.GetGenericTypeDefinition();
                if (genericTypeDefinition == typeof (Nullable<>))
                {
                    Type baseType = Nullable.GetUnderlyingType(proType);
                    if (baseType.GetInterface("IConvertible") != null)
                    {
                        if (baseType == bitType)
                        {
                            return value == "True" || value == "1" || value == "true";
                        }
                        else if (baseType.IsEnum) //枚举类型
                        {
                            return Enum.Parse(baseType, value);
                        }
                        else
                        {
                            return Convert.ChangeType(value, baseType);
                        }
                    }
                }
            }
            return null;
        }
    }
}