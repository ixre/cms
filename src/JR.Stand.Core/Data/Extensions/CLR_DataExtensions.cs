using System;
using System.Data.Common;

namespace JR.Stand.Core.Data.Extensions
{
    public static class CLR_DataExtensions
    {
        /// <summary>
        /// 获取DataReader的列名
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="lower"></param>
        /// <returns></returns>
        public static String[] GetColumns(this DbDataReader reader, bool lower)
        {
            String[] columns = new String[reader.FieldCount];
            for (int i = 0, j = columns.Length; i < j; i++)
            {
                columns[i] = lower ? reader.GetName(i).ToLower() : reader.GetName(i);
            }
            return columns;
        }
    }
}