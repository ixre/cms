/*
* Copyright(C) 2010-2012 TO2.NET
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
using System.Text;
using JR.Cms.Domain.Interface.Site.Extend;
using JR.Stand.Core.Data;

namespace JR.Cms.Library.DataAccess.DAL
{
    public class ExtendFieldDal : DalBase
    {
        public int AddExtendField(int siteId, IExtendField field)
        {
            var row = ExecuteNonQuery(
                NewQuery(DbSql.DataExtend_CreateField,
                    Db.CreateParametersFromArray(
                        new object[,]
                        {
                            {"@siteId", siteId},
                            {"@name", field.Name},
                            {"@type", field.Type},
                            {"@regex", field.Regex},
                            {"@defaultValue", field.DefaultValue},
                            {"@message", field.Message}
                        }))
            );

            if (row > 0)
            {
                var q = NewQuery("SELECT MAX(id) FROM $PREFIX_extend_field WHERE site_id=" +
                                 siteId.ToString(), EmptyParameter);
                return int.Parse(ExecuteScalar(q).ToString());
            }

            return -1;
        }

        public bool DeleteExtendField(int siteId, int fieldId)
        {
            var rowcount = ExecuteNonQuery(
                NewQuery(DbSql.DataExtend_DeleteExtendField,
                    Db.CreateParametersFromArray(
                        new object[,]
                        {
                            {"@siteId", siteId},
                            {"@id", fieldId}
                        }))
            );
            return rowcount == 1;
        }

        public bool UpdateExtendField(int siteId, IExtendField field)
        {
            var rowcount = ExecuteNonQuery(
                NewQuery(DbSql.DataExtend_UpdateField,
                    Db.CreateParametersFromArray(
                        new object[,]
                        {
                            {"@siteId", siteId},
                            {"@name", field.Name},
                            {"@type", field.Type},
                            {"@regex", field.Regex},
                            {"@defaultValue", field.DefaultValue},
                            {"@message", field.Message},
                            {"@id", field.GetDomainId()}
                        }))
            );
            return rowcount == 1;
        }

        public void GetAllExtendFields(DataReaderFunc func)
        {
            ExecuteReader(NewQuery(DbSql.DataExtend_GetAllExtends, null),
                func);
        }

        public void GetExtendValues(int siteId, int relationType, int relationID, DataReaderFunc func)
        {
            ExecuteReader(NewQuery(DbSql.DataExtend_GetExtendValues,
                Db.CreateParametersFromArray(
                    new object[,]
                    {
                        {"@siteId", siteId},
                        {"@relationType", relationType},
                        {"@relationId", relationID}
                    })
            ), func);
        }

        public void InsertDataExtendFields(ExtendRelationType relationType, int relationId,
            IDictionary<int, string> extendData)
        {
            if (extendData.Count == 0) return;

            var sql = OptimizeSql(DbSql.DataExtend_InsertOrUpdateFieldValue);
            IList<SqlQuery> querys = new List<SqlQuery>();

            querys.Add(SqlQueryHelper.Format(DbSql.DataExtend_ClearupExtendFielValue,
                new object[,]
                {
                    {"@relationType", relationType},
                    {"@relationId", relationId}
                }
            ));

            foreach (var key in extendData.Keys)
                if (!string.IsNullOrEmpty(extendData[key]))
                    querys.Add(new SqlQuery(sql, new object[,]
                    {
                        {"@relationType", relationType},
                        {"@relationId", relationId},
                        {"@fieldId", key},
                        {"@fieldValue", extendData[key]}
                    }));

            ExecuteMultiNonQuery(querys.ToArray());
        }


        public int[] GetCategoryExtendIdList(int siteId, int categoryId)
        {
            IList<int> list = new List<int>();
            ExecuteReader(NewQuery(DbSql.DataExtend_GetCategoryExtendIdList,
                    Db.CreateParametersFromArray(
                        new object[,]
                        {
                            {"@siteId", siteId},
                            {"@categoryId", categoryId}
                        })
                ),
                rd =>
                {
                    while (rd.Read()) list.Add(rd.GetInt32(0));
                });

            return list.ToArray();
        }

        public void GetExtendValuesList(int siteId, int relationType, IList<int> idList, DataReaderFunc func)
        {
            if (idList == null || idList.Count == 0) return;
            var ids = "";
            var tmpInt = 0;
            foreach (var i in idList)
            {
                if (tmpInt++ != 0) ids += ",";
                ids += i.ToString();
            }

            var sql = String.Format(DbSql.DataExtend_GetExtendValuesList, ids);
            ExecuteReader(SqlQueryHelper.Format(sql,
                new object[,]
                {
                    {"@siteId", siteId},
                    {"@relationType", relationType}
                }
            ), func);
        }

        public int GetCategoryExtendRefrenceNum(int siteId, int categoryId, int extendFieldId)
        {
            return int.Parse(ExecuteScalar(
                SqlQueryHelper.Format(DbSql.DataExtend_GetCategoryExtendRefrenceNum, new object[,]
                {
                    {"@siteId", siteId},
                    {"@categoryId", categoryId},
                    {"@fieldId", extendFieldId}
                })
            ).ToString());
        }


        public void UpdateCategoryExtendsBind(int categoryId, int[] extendIds)
        {
            var querys = new SqlQuery[extendIds.Length + 1];
            const string insertSql =
                "INSERT INTO $PREFIX_category_extend (category_id,extend_id,enabled) VALUES (@categoryId, @extendId,1)";
            var sb = new StringBuilder("DELETE FROM $PREFIX_category_extend WHERE category_id = @categoryId");

            if (extendIds.Length > 0)
            {
                sb.Append(" AND extend_id NOT IN(");
                for (var i = 0; i < extendIds.Length; i++)
                {
                    if (i > 0) sb.Append(",");
                    sb.Append(extendIds[i].ToString());
                    querys[i + 1] = NewQuery(insertSql, Db.CreateParametersFromArray(new object[,]
                        {
                            {"@categoryId", categoryId},
                            {"@extendId", extendIds[i]}
                        }
                    ));
                }

                sb.Append(")");
            }

            querys[0] = NewQuery(sb.ToString(), Db.CreateParametersFromArray(new object[,]
            {
                {"@categoryId", categoryId}
            }));
            ExecuteMultiNonQuery(querys);
        }

        public void GetExtendFieldByName(int siteId, string name, string type, DataReaderFunc rd)
        {
            ExecuteReader(
                SqlQueryHelper.Format(DbSql.DataExtend_GetExtendFieldByName, new object[,]
                {
                    {"@siteId", siteId},
                    {"@name", name},
                    {"@type", type}
                }), rd);
        }
    }
}