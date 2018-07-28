/*
* Copyright(C) 2010-2012 TO2.NET
* 
* File Name	: ModuleDAL
* Author	: Newmin (new.min@msn.com)
* Create	: 2012/9/30 10:10:58
* Description	:
*
*/

using System.Collections.Generic;
using T2.Cms.Domain.Interface.Models;
using T2.Cms.IDAL;
using System.Data;

namespace T2.Cms.Dal
{
    public class ModuleDAL : DalBase,ImoduleDAL
    {
        public bool AddModule(Module module)
        {
            int rowcount = base.ExecuteNonQuery(
                 base.NewQuery(DbSql.Module_Add,
                                 base.Db.CreateParametersFromArray(

                     new object[,]{
                {"@siteId",module.SiteId},
                {"@name",module.Name},
                {"@isSystem", module.IsSystem},
                {"@isDelete", module.IsDelete}
                     }))
                );

            return rowcount == 1;
        }

        public bool DeleteModule(int moduleID)
        {
            int rowcount = base.ExecuteNonQuery(
                  base.NewQuery(DbSql.Module_Delete,
                                  base.Db.CreateParametersFromArray(

                      new object[,]{
                 {"@id", moduleID}
                      }))
                 );
            return rowcount==1;
        }

        public bool UpdateModule(Module module)
        {
            int rowcount = base.ExecuteNonQuery(
                base.NewQuery(DbSql.Module_Update,
                                base.Db.CreateParametersFromArray(

                    new object[,]{
                {"@id", module.ID},
                {"@name", module.Name},
                {"@isDelete", module.IsDelete}
                    }))
                );
            return rowcount == 1;
        }

        public IList<Module> GetModules()
        {
            IList<Module> list = null;
            base.ExecuteReader(base.NewQuery(DbSql.Module_GetAll,null),
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
            base.ExecuteReader(base.NewQuery(DbSql.Module_GetByID,
                                base.Db.CreateParametersFromArray(

                new object[,]{
                    {"@id", moduleID}
                })),
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
            base.ExecuteReader(base.NewQuery(DbSql.Module_GetByName,
                                base.Db.CreateParametersFromArray(

                new object[,]{
                    {"@name", moduleName}
                })),
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
