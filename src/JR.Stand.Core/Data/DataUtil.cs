using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using JR.Stand.Core.Data.Extensions;

namespace JR.Stand.Core.Data
{
    public class DataUtil
    {
        /// <summary>
        /// 转换为参数
        /// </summary>
        /// <param name="db"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DbParameter[] ToParams(IDbDialect db, object[,] data)
        {
            if (data != null)
            {
                int l = data.GetLength(0);
                if (l != 0 && data.GetLength(1) != 2)
                {
                    throw new ArgumentOutOfRangeException("data", "多纬数组的二维长度必须为2");
                }

                DbParameter[] parameter = new DbParameter[l];
                for (int i = 0; i < l; i++)
                {
                    parameter[i] = db.CreateParameter(data[i, 0].ToString(), data[i, 1]);
                }
                return parameter;
            }
            return null;
        }

        /// <summary>
        /// 参数转为字符
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static String ParamsToString(DbParameter[] parameters)
        {
            if (parameters != null)
            {
                int l = parameters.Length;
                if (l > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i=0;i<l;i++){
                        if (i > 0) sb.Append(" ");
                        sb.Append(parameters[i].ParameterName).Append(":")
                                .Append(parameters[i].Value.ToString());
                    }
                    return sb.ToString();
                }
            }
            return "";
        }

        /// <summary>
        /// 参数字典转为数组
        /// </summary>
        /// <param name="d"></param>
        /// <param name="paramMap"></param>
        /// <returns></returns>
        public static DbParameter[] ParameterMapToArray(IDbDialect d, IDictionary<string, object> paramMap)
        {
            List<DbParameter> parameters = new List<DbParameter>();
            foreach (KeyValuePair<String, Object> p in paramMap)
            {
                parameters.Add(d.CreateParameter(p.Key, p.Value ?? ""));
            }
            return parameters.ToArray();
        }

        /// <summary>
        /// 转换DataTable为实体列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static IList<T> ConvertToEntityList<T>(DataTable table) where T : new()
        {
            return table.ToEntityList<T>();
        }

        /// <summary>
        ///将DataReader中的行转换成实体集合(仅拷贝实体与数据表列名相同的数据)
        /// </summary>
        public static IList<T> ConvertToEntityList<T>(DbDataReader reader) where T : new()
        {
            return reader.ToEntityList<T>();
        }

        public static T ConvertToEntity<T>(DataRow row) where T : new()
        {
            return row.ToEntity<T>();
        }

        /// <summary>
        ///将DataReader转换成实体(仅拷贝实体与数据表列名相同的数据)
        /// </summary>
        public static T ConvertToEntity<T>(DbDataReader reader) where T : new()
        {
            return reader.ToEntity<T>();
        }

        /// <summary>
        /// 生成SQL语句参数对象数组
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="obj"></param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="fields">字段，用空格隔开多个字段。参数名称需与字段名称一致！</param>
        /// <returns></returns>
        public static DbParameter[] GetDbParameter<T>(T obj, DataBaseType dbType, String fields)
        {
            return obj.GetDbParameter(dbType, fields);
        }
    }
}
