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
using JR.Cms.Domain.Interface.Models;
using JR.Cms.Library.DataAccess.IDAL;
using JR.Stand.Core.Data.Extensions;

namespace JR.Cms.Library.DataAccess.DAL
{
    public class ModuleDAL : DalBase, ImoduleDAL
    {
        public bool AddModule(Module module)
        {
            var rowcount = ExecuteNonQuery(
                NewQuery(DbSql.Module_Add,
                    Db.CreateParametersFromArray(
                        new object[,]
                        {
                            {"@siteId", module.SiteId},
                            {"@name", module.Name},
                            {"@isSystem", module.IsSystem},
                            {"@isDelete", module.IsDelete}
                        }))
            );

            return rowcount == 1;
        }

        public bool DeleteModule(int moduleID)
        {
            var rowcount = ExecuteNonQuery(
                NewQuery(DbSql.Module_Delete,
                    Db.CreateParametersFromArray(
                        new object[,]
                        {
                            {"@id", moduleID}
                        }))
            );
            return rowcount == 1;
        }

        public bool UpdateModule(Module module)
        {
            var rowcount = ExecuteNonQuery(
                NewQuery(DbSql.Module_Update,
                    Db.CreateParametersFromArray(
                        new object[,]
                        {
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
            ExecuteReader(NewQuery(DbSql.Module_GetAll, null),
                reader =>
                {
                    if (reader.HasRows) list = reader.ToEntityList<Module>();
                });
            return list;
        }

        public Module GetModule(int moduleID)
        {
            Module m = null;
            ExecuteReader(NewQuery(DbSql.Module_GetByID,
                    Db.CreateParametersFromArray(
                        new object[,]
                        {
                            {"@id", moduleID}
                        })),
                reader =>
                {
                    if (reader.HasRows) m = reader.ToEntity<Module>();
                });
            return m;
        }

        public Module GetModule(string moduleName)
        {
            Module m = null;
            ExecuteReader(NewQuery(DbSql.Module_GetByName,
                    Db.CreateParametersFromArray(
                        new object[,]
                        {
                            {"@name", moduleName}
                        })),
                reader =>
                {
                    if (reader.HasRows) m = reader.ToEntity<Module>();
                });
            return m;
        }
    }
}