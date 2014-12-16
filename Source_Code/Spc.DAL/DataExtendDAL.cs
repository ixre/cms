/*
* Copyright(C) 2010-2012 OPSoft Inc
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
using Ops.Cms.IDAL;
using Ops.Data;
using System.Data;

using Spc.Models;
using Ops.Cms.DataTransfer;
using Ops.Cms.Domain.Interface.Site.Extend;
namespace Ops.Cms.DAL
{
    public class ExtendFieldDAL : DALBase
    {
        public bool AddExtendField(int siteId, IExtendField field)
        {
            int rowcount = base.ExecuteNonQuery(
                 new SqlQuery(base.OptimizeSQL(SP.DataExtend_CreateField),
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
                 new SqlQuery(base.OptimizeSQL(SP.DataExtend_DeleteExtendField), new object[,]{
                     {"@siteId",siteId},
                    {"@id", fieldId}
                 })
                );
            return rowcount == 1;
        }

        public bool UpdateExtendField(int siteId, IExtendField field)
        {
            int rowcount = base.ExecuteNonQuery(
                 new SqlQuery(base.OptimizeSQL(SP.DataExtend_UpdateField),
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
            base.ExecuteReader(new SqlQuery(base.OptimizeSQL(SP.DataExtend_GetAllExtends)),
                func);
        }

        public void GetExtendValues(int siteId, int relationType, int relationID, DataReaderFunc func)
        {
            base.ExecuteReader(new SqlQuery(base.OptimizeSQL(SP.DataExtend_GetExtendValues),
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

            string sql = base.OptimizeSQL(SP.DataExtend_InsertOrUpdateFieldValue);
            IList<SqlQuery> querys = new List<SqlQuery>();

            querys.Add(SqlQueryHelper.Format(SP.DataExtend_ClearupExtendFielValue,
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
            base.ExecuteReader(new SqlQuery(base.OptimizeSQL(SP.DataExtend_GetCategoryExtendIdList),
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

            base.ExecuteReader(SqlQueryHelper.Format(SP.DataExtend_GetExtendValuesList,
                  new object[,]{
                     {"@siteId",siteId},
                     {"@relationType",relationType}
                 }, ids
                 ), func);
        }

        public int GetCategoryExtendRefrenceNum(int siteId, int categoryId, int extendFieldId)
        {
            return int.Parse(base.ExecuteScalar(
                SqlQueryHelper.Format(SP.DataExtend_GetCategoryExtendRefrenceNum, new object[,]{
                    {"@siteId",siteId},
                    {"@categoryId",categoryId},
                    {"@fieldId", extendFieldId}
                 })
               ).ToString());
        }


        public void UpdateCategoryExtendsBind(int categoryId, int[] extendIds)
        {
            SqlQuery[] querys = new SqlQuery[extendIds.Length + 1];
            const string insertSql = "INSERT INTO $PREFIX_categoryExtend (categoryId,extendId,enabled) VALUES (@categoryId, @extendId,1)";
            const string delSql = "DELETE FROM $PREFIX_categoryExtend WHERE categoryId=@categoryId";

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
