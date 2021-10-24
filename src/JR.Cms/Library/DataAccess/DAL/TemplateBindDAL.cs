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
using JR.Stand.Core.Data;

namespace JR.Cms.Library.DataAccess.DAL
{
    public class TemplateBindDal : DalBase
    {
        private bool HasExists(TemplateBindType type, int bindId)
        {
            var parameters = Db.CreateParametersFromArray(
                new object[,]
                {
                    {"@bindId", bindId},
                    {"@bindType", type}
                });
            return int.Parse(ExecuteScalar(
                NewQuery(DbSql.TplBind_CheckExists, parameters)
            ).ToString()) != 0;
        }

        public bool SetBind(TemplateBindType type, int bindId, string templatePath)
        {
            int rowcount;
            //如果模板为空，则删除
            if (string.IsNullOrEmpty(templatePath)) return RemoveBind(type, bindId);

            var paramsters = Db.CreateParametersFromArray(
                new object[,]
                {
                    {"@tplPath", templatePath},
                    {"@bindId", bindId},
                    {"@bindType", (int) type}
                });

            if (!HasExists(type, bindId))
                rowcount = ExecuteNonQuery(
                    NewQuery(DbSql.TplBind_Add, paramsters));
            else
                rowcount = ExecuteNonQuery(
                    NewQuery(DbSql.TplBind_Update, paramsters)
                );

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
            return ExecuteNonQuery(
                NewQuery(DbSql.TplBind_RemoveBind, Db.CreateParametersFromArray(
                    new object[,]
                    {
                        {"@bindId", bindRefrenceId},
                        {"@bindType", type}
                    }))) == 1;
        }


        public int RemoveErrorCategoryBind()
        {
            return ExecuteNonQuery(NewQuery(DbSql.TplBind_RemoveErrorCategoryBind, null));
        }


        public void GetBindList(DataReaderFunc func)
        {
            ExecuteReader(NewQuery(DbSql.TplBind_GetBindList, null),
                func);
        }
    }
}