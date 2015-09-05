/*
* Copyright(C) 2010-2012 Z3Q.NET
* 
* File Name	: ExtendAttrDAL
* Author	: Newmin (new.min@msn.com)
* Create	: 2012/9/30 10:18:51
* Description	:
*
*/

using System;
using System.Collections.Generic;
using System.Linq;
using J6.Cms.Domain.Interface.Site.Extend;
using J6.DevFw.Data;

namespace J6.Cms.Dal
{
    public class ExtendFieldDal : DalBase
    {
        public bool AddExtendField(int siteId, IExtendField field)
        {
            int rowcount = base.ExecuteNonQuery(
                 new SqlQuery(base.OptimizeSql(DbSql.DataExtend_CreateField),
                     new object[,]{
                         {"@siteId",siteId},
                        {"@name", field.Name},
                        {"@type", field.Type},
                        {"@regex", field.Regex},
                        {"@defaultValue",field.DefaultValue},
                        {"@message",field.Message}
                     })
                );

            return rowcount == 1;

        }

        public bool DeleteExtendField(int siteId, int fieldId)
        {
            int rowcount = base.ExecuteNonQuery(
                 new SqlQuery(base.OptimizeSql(DbSql.DataExtend_DeleteExtendField), new object[,]{
                     {"@siteId",siteId},
                    {"@id", fieldId}
                 })
                );
            return rowcount == 1;
        }

        public bool UpdateExtendField(int siteId, IExtendField field)
        {
            int rowcount = base.ExecuteNonQuery(
                 new SqlQuery(base.OptimizeSql(DbSql.DataExtend_UpdateField),
                     new object[,]{
                        {"@siteId",siteId},
                        {"@name", field.Name},
                        {"@type", field.Type},
                        {"@regex", field.Regex},
                        {"@defaultValue", field.DefaultValue},
                        {"@message",field.Message},
                        {"@id", field.Id}
                     })
                );
            return rowcount == 1;
        }

        public void GetAllExtendFields(DataReaderFunc func)
        {
            base.ExecuteReader(new SqlQuery(base.OptimizeSql(DbSql.DataExtend_GetAllExtends)),
                func);
        }

        public void GetExtendValues(int siteId, int relationType, int relationID, DataReaderFunc func)
        {
            base.ExecuteReader(new SqlQuery(base.OptimizeSql(DbSql.DataExtend_GetExtendValues),
                 new object[,]{
                     {"@siteId",siteId},
                     {"@relationType",relationType},
                     {"@relationId",relationID}
                 }
                ), func);
        }

        public void InsertDataExtendFields(ExtendRelationType relationType, int relationId, IDictionary<int, string> extendData)
        {

            if (extendData.Count == 0) return;

            string sql = base.OptimizeSql(DbSql.DataExtend_InsertOrUpdateFieldValue);
            IList<SqlQuery> querys = new List<SqlQuery>();

            querys.Add(SqlQueryHelper.Format(DbSql.DataExtend_ClearupExtendFielValue,
                 new object[,]{
                        {"@relationType",relationType},
                        {"@relationId",relationId}
                 }
            ));

            foreach (int key in extendData.Keys)
            {
                if (!String.IsNullOrEmpty(extendData[key]))
                {
                    querys.Add(new SqlQuery(sql, new object[,]{
                        {"@relationType",relationType},
                        {"@relationId",relationId},
                        {"@fieldId",key},
                        {"@fieldValue",extendData[key]}
                    }));
                }
            }

            base.ExecuteNonQuery(querys.ToArray());
        }


        public int[] GetCategoryExtendIdList(int siteId, int categoryId)
        {
            IList<int> list = new List<int>();
            base.ExecuteReader(new SqlQuery(base.OptimizeSql(DbSql.DataExtend_GetCategoryExtendIdList),
                   new object[,]{
                     {"@siteId",siteId},
                     {"@categoryId",categoryId}
                   }
                  ),
                    rd =>
                    {
                        while (rd.Read())
                        {
                            list.Add(rd.GetInt32(0));
                        }
                    });

            return list.ToArray();
        }

        public void GetExtendValuesList(int siteId, int relationType, IList<int> idList, DataReaderFunc func)
        {
            if (idList == null || idList.Count == 0) return;
            string ids = "";
            int tmpInt = 0;
            foreach (int i in idList)
            {
                if (tmpInt++ != 0)
                {
                    ids += ",";
                }
                ids += i.ToString();
            }

            base.ExecuteReader(SqlQueryHelper.Format(DbSql.DataExtend_GetExtendValuesList,
                  new object[,]{
                     {"@siteId",siteId},
                     {"@relationType",relationType}
                 }, ids
                 ), func);
        }

        public int GetCategoryExtendRefrenceNum(int siteId, int categoryId, int extendFieldId)
        {
            return int.Parse(base.ExecuteScalar(
                SqlQueryHelper.Format(DbSql.DataExtend_GetCategoryExtendRefrenceNum, new object[,]{
                    {"@siteId",siteId},
                    {"@categoryId",categoryId},
                    {"@fieldId", extendFieldId}
                 })
               ).ToString());
        }


        public void UpdateCategoryExtendsBind(int categoryId, int[] extendIds)
        {
            SqlQuery[] querys = new SqlQuery[extendIds.Length + 1];
            const string insertSql = "INSERT INTO $PREFIX_category_extend (category_id,extend_id,enabled) VALUES (@categoryId, @extendId,1)";
            const string delSql = "DELETE FROM $PREFIX_category_extend WHERE category_id=@categoryId";

            querys[0] = SqlQueryHelper.Format(delSql, new object[,]{
                {"@categoryId",categoryId}
            });

            for (int i = 0; i < extendIds.Length; i++)
            {
                querys[i + 1] = SqlQueryHelper.Format(insertSql, new object[,]{
                                    {"@categoryId",categoryId},
                                    {"@extendId",extendIds[i]}
                                }
                             );
            }

            base.ExecuteNonQuery(querys);
        }
    }
}
