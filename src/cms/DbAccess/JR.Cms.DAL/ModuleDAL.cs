/*
* Copyright(C) 2010-2012 Z3Q.NET
* 
* File Name	: ModuleDAL
* Author	: Newmin (new.min@msn.com)
* Create	: 2012/9/30 10:10:58
* Description	:
*
*/

using System.Collections.Generic;
using JR.Cms.Domain.Interface.Models;
using JR.Cms.IDAL;
using JR.DevFw.Data;
using JR.DevFw.Data.Extensions;

namespace JR.Cms.Dal
{
    public class ModuleDAL : DalBase,ImoduleDAL
    {
        public bool AddModule(Module module)
        {
            int rowcount = base.ExecuteNonQuery(
                 new SqlQuery(base.OptimizeSql(DbSql.Module_Add),
                     new object[,]{
                {"@siteId",module.SiteId},
                {"@name",module.Name},
                {"@isSystem", module.IsSystem},
                {"@isDelete", module.IsDelete}
                     })
                );

            return rowcount == 1;
        }

        public bool DeleteModule(int moduleID)
        {
            int rowcount = base.ExecuteNonQuery(
                  new SqlQuery(base.OptimizeSql(DbSql.Module_Delete),
                      new object[,]{
                 {"@id", moduleID}
                      })
                 );
            return rowcount==1;
        }

        public bool UpdateModule(Module module)
        {
            int rowcount = base.ExecuteNonQuery(
                new SqlQuery(base.OptimizeSql(DbSql.Module_Update),
                    new object[,]{
                {"@id", module.ID},
                {"@name", module.Name},
                {"@isDelete", module.IsDelete}
                    })
                );
            return rowcount == 1;
        }

        public IList<Module> GetModules()
        {
            IList<Module> list = null;
            base.ExecuteReader(new SqlQuery(base.OptimizeSql(DbSql.Module_GetAll)),
                reader =>
                {
                    if (reader.HasRows)
                    {
                       list= reader.ToEntityList<Module>();
                    }
                });
            return list;
        }

        public Module GetModule(int moduleID)
        {
            Module m = null;
            base.ExecuteReader(new SqlQuery(base.OptimizeSql(DbSql.Module_GetByID),
                new object[,]{
                    {"@id", moduleID}
                }),
                  reader =>
                  {
                      if (reader.HasRows)
                      {
                          m = reader.ToEntity<Module>();
                      }
                  });
            return m;
        }

        public Module GetModule(string moduleName)
        {
            Module m = null;
            base.ExecuteReader(new SqlQuery(base.OptimizeSql(DbSql.Module_GetByName),
                new object[,]{
                    {"@name", moduleName}
                }),
                  reader =>
                  {
                      if (reader.HasRows)
                      {
                          m = reader.ToEntity<Module>();
                      }
                  });
            return m;
        }

    }
}
