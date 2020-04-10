/*
 * Copyright 2010 OPS,All rights reserved !
 * name     : 泛型扩展
 * author   : newmin
 * date     : 2010/11/08 07:31
 * 
 */

using System;
using System.Globalization;
using System.Reflection;

namespace JR.Stand.Core.Framework.Extensions
{
    public static class TExtensions
    {
        /// <summary>
        ///从SourceType拷贝对象的数据,将相同名称属性赋值
        /// </summary>
        /// <param name="s">包含数据的对象</param>
        /// <param name="c">要拷贝数据的对象</param>
        /// <param name="ignoreNullValue">是否忽略空值</param>
        public static ToType CloneData<SourceType, ToType>(this ToType t, SourceType s, bool ignoreNullValue)
        {
            Type sourceType = typeof (SourceType);
            Type type = typeof (ToType);
            Type propertyType;
            Type fieldType;

            if (t is String || s is String) return t;


            PropertyInfo[] cp = type.GetProperties();
            FieldInfo[] fields = type.GetFields();

            object _v;


            FieldInfo _f;

            foreach (FieldInfo f in sourceType.GetFields())
            {
                if (f.IsStatic || f.IsInitOnly) continue;

                _f = Array.Find(fields, a => string.Compare(a.Name, f.Name, true,
                    CultureInfo.InvariantCulture) == 0);

                if (_f == null || _f.IsStatic || f.IsInitOnly) continue;

                _v = f.GetValue(s);
                if (!ignoreNullValue && _v == null) continue;

                fieldType = _f.FieldType;
                if (fieldType.IsEnum)
                {
                    _f.SetValue(t, Enum.Parse(fieldType, _v.ToString()));
                }
                else
                {
                    if (_v is IConvertible)
                    {
                        _f.SetValue(t, Convert.ChangeType(_v, fieldType));
                    }
                    else
                    {
                        try
                        {
                            _f.SetValue(t, _v);
                        }
                        catch
                        {
                        }
                    }
                }
            }

            PropertyInfo _p;
            foreach (PropertyInfo p in sourceType.GetProperties())
            {
                if (!p.CanRead) continue;

                _p = Array.Find(cp, a => string.Compare(a.Name, p.Name, true,
                    CultureInfo.InvariantCulture) == 0);

                if (_p == null || !_p.CanWrite) continue;


                _v = p.GetValue(s, null);
                if (!ignoreNullValue && _v == null) continue;
                propertyType = _p.PropertyType;

                if (propertyType.IsEnum)
                {
                    _p.SetValue(t, Enum.Parse(propertyType, _v.ToString()), null);
                }
                else
                {
                    if (_v is IConvertible)
                    {
                        _p.SetValue(t, Convert.ChangeType(_v, propertyType), null);
                    }
                    else
                    {
                        try
                        {
                            _p.SetValue(t, _v, null);
                        }
                        catch
                        {
                        }
                    }
                }
            }


            return t;
        }

        /// <summary>
        /// 拷贝对象的数据
        /// </summary>
        public static ToType CloneData<SourceType, ToType>(this ToType t, SourceType s)
        {
            return CloneData(t, s, true);
        }
    }
}