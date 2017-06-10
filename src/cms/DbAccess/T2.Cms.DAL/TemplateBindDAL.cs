/*
* Copyright(C) 2010-2012 Z3Q.NET
* 
* File Name	: TemplateBindDAL
* Author	: Administrator
* Create	: 2012/10/28 7:29:08
* Description	:
*
*/

using System;
using T2.Cms.Domain.Interface.Site.Template;
using JR.DevFw.Data;

namespace T2.Cms.Dal
{
    public class TemplateBindDal : DalBase
    {
        private bool HasExists(TemplateBindType type, int bindId)
        {
            return int.Parse(base.ExecuteScalar(
                 new SqlQuery(base.OptimizeSql(DbSql.TplBind_CheckExists),
                     new object[,]{
                 {"@bindId", bindId},
                 {"@bindType", type}
                     })
                 ).ToString()) != 0;
        }

        public bool SetBind(TemplateBindType type, int bindId, string templatePath)
        {
            int rowcount;
            //如果模板为空，则删除
            if (String.IsNullOrEmpty(templatePath))
            {
                return RemoveBind(type, bindId);
            }

            if (!HasExists(type, bindId))
            {
                rowcount = base.ExecuteNonQuery(
                    new SqlQuery(base.OptimizeSql(DbSql.TplBind_Add),
                        new object[,]
                        {
                            {"@bindId", bindId},
                            {"@bindType", (int) type},
                            {"@tplPath", templatePath}
                        }));
            }
            else
            {
                rowcount = base.ExecuteNonQuery(
                    new SqlQuery(base.OptimizeSql(DbSql.TplBind_Update),
                        new object[,]
                        {
                            {"@tplPath", templatePath},
                            {"@bindId", bindId},
                            {"@bindType", (int) type}
                        }));
            }

            return rowcount == 1;
        }

        //private TemplateBind GetBind(TemplateBindType type, string bindID)
        //{
        //    TemplateBind entity = null;
        //    base.ExecuteReader(new SqlQuery(base.OptimizeSQL(SP.TplBind_GetBind),
        //        new object[,]{
        //          {"@bindId", bindID},
        //          {"@bindType", type}
        //        }),
        //          reader =>
        //          {
        //              if (reader.HasRows)
        //              {
        //                  entity = reader.ToEntity<TemplateBind>();
        //              }
        //          });
        //    return entity;
        //}


        public bool RemoveBind(TemplateBindType type, int bindRefrenceId)
        {
            return base.ExecuteNonQuery(
                 new SqlQuery(base.OptimizeSql(DbSql.TplBind_RemoveBind),
                      new object[,]{
                 {"@bindId", bindRefrenceId},
                 {"@bindType", type}
                      })) == 1;
        }


        public int RemoveErrorCategoryBind()
        {
            return base.ExecuteNonQuery(new SqlQuery(base.OptimizeSql(DbSql.TplBind_RemoveErrorCategoryBind)));
        }


        public void GetBindList(DataReaderFunc func)
        {
            base.ExecuteReader(new SqlQuery(base.OptimizeSql(DbSql.TplBind_GetBindList)),
                 func);
        }
    }
}
