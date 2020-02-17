/*
* Copyright(C) 2010-2012 TO2.NET
* 
* File Name	: TemplateBindDAL
* Author	: Administrator
* Create	: 2012/10/28 7:29:08
* Description	:
*
*/

using System;
using JR.Cms.Domain.Interface.Site.Template;
using JR.DevFw.Data;

namespace JR.Cms.Dal
{
    public class TemplateBindDal : DalBase
    {
        private bool HasExists(TemplateBindType type, int bindId)
        {
            var parameters = base.Db.CreateParametersFromArray(
                          new object[,]{
                 {"@bindId", bindId},
                 {"@bindType", type}
                      });
            return int.Parse(base.ExecuteScalar(
                 base.NewQuery(DbSql.TplBind_CheckExists, parameters)
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

            var paramsters = base.Db.CreateParametersFromArray(
                        new object[,]
                        {
                            {"@tplPath", templatePath},
                            {"@bindId", bindId},
                            {"@bindType", (int) type}
                        });

            if (!HasExists(type, bindId))
            {
                rowcount = base.ExecuteNonQuery(
                    base.NewQuery(DbSql.TplBind_Add, paramsters));
            }
            else
            {
                rowcount = base.ExecuteNonQuery(
                    base.NewQuery(DbSql.TplBind_Update, paramsters)
                    );
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
                 base.NewQuery(DbSql.TplBind_RemoveBind, base.Db.CreateParametersFromArray(
                      new object[,]{
                 {"@bindId", bindRefrenceId},
                 {"@bindType", type}
                      }))) == 1;
        }


        public int RemoveErrorCategoryBind()
        {
            return base.ExecuteNonQuery(base.NewQuery(DbSql.TplBind_RemoveErrorCategoryBind, null));
        }


        public void GetBindList(DataReaderFunc func)
        {
            base.ExecuteReader(base.NewQuery(DbSql.TplBind_GetBindList, null),
                 func);
        }
    }
}
