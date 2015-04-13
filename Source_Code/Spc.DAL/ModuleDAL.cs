/*
* Copyright(C) 2010-2012 S1N1.COM
* 
* File Name	: ModuleDAL
* Author	: Newmin (new.min@msn.com)
* Create	: 2012/9/30 10:10:58
* Description	:
*
*/

using System.Collections.Generic;
using AtNet.Cms.Domain.Interface.Models;
using AtNet.Cms.IDAL;
using AtNet.DevFw.Data;
using AtNet.DevFw.Data.Extensions;

namespace AtNet.Cms.DAL
{
    public class ModuleDAL : DALBase,ImoduleDAL
    {
        public bool AddModule(Module module)
        {
            int rowcount = base.ExecuteNonQuery(
                 new SqlQuery(base.OptimizeSQL(SP.Module_Add),
                     new object[,]{
                {"@siteid",module.SiteId},
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
                  new SqlQuery(base.OptimizeSQL(SP.Module_Delete),
                      new object[,]{
                 {"@id", moduleID}
                      })
                 );
            return rowcount==1;
        }

        public bool UpdateModule(Module module)
        {
            int rowcount = base.ExecuteNonQuery(
                new SqlQuery(base.OptimizeSQL(SP.Module_Update),
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
            base.ExecuteReader(new SqlQuery(base.OptimizeSQL(SP.Module_GetAll)),
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
            base.ExecuteReader(new SqlQuery(base.OptimizeSQL(SP.Module_GetByID),
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
            base.ExecuteReader(new SqlQuery(base.OptimizeSQL(SP.Module_GetByName),
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
