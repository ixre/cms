/*
* Copyright(C) 2010-2012 TO2.NET
* 
* File Name	: ModuleBLL
* Author	: Newmin (new.min@msn.com)
* Create	: 2012/9/30 10:30:20
* Description	:
*
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JR.Cms.Dal;
using JR.Cms.Domain.Interface._old;
using JR.Cms.Domain.Interface.Models;
using JR.Cms.IDAL;

namespace JR.Cms.Library.DataAccess.BLL
{
   public class ModuleBLL : Imodule
    {
        private ImoduleDAL dal = new ModuleDAL();

        public bool AddModule(int siteID,string name)
        {
            if (this.GetModule(siteID,name) == null)
            {
                //
                //todo:check siteid
                //

                bool result = dal.AddModule(new Module
                {
                    SiteId=siteID,
                    Name = name,
                    IsSystem = false,
                    IsDelete = false
                });

                WeakRefCache.RebuiltModule();

                return result;
            }
            return false;
        }

        public bool DeleteModule(int moduleID)
        {
            throw new Exception("暂不允许真删除模块");
            return dal.DeleteModule(moduleID);
        }

        public bool UpdateModule(Module module)
        {
            bool result= dal.UpdateModule(module);
            if (result)
            {
                WeakRefCache.RebuiltModule();
            }
            return result;
        }

        public IList<Module> GetModules()
        {
            return WeakRefCache.Modules;
        }

        public IEnumerable<Module> GetSiteModules(int siteID)
        {
            foreach (Module m in WeakRefCache.Modules)
            {
                if (m.SiteId==0 || m.SiteId==siteID)
                {
                    yield return m;
                }
            }
        }

        public IEnumerable<Module> GetSiteAvailableModules(int siteID)
        {
            foreach (Module m in WeakRefCache.Modules)
            {
                if (!m.IsDelete &&(m.SiteId==0 || m.SiteId==siteID))
                {
                    yield return m;
                }
            }
        }

        public IEnumerable<Module> GetAvailableModules()
        {
            foreach (Module m in WeakRefCache.Modules)
            {
                if (!m.IsDelete)
                {
                    yield return m;
                }
            }
        }

        public Module GetModule(int moduleID)
        {
            //return dal.GetModule(moduleID);
            return WeakRefCache.Modules.SingleOrDefault(a => a.ID == moduleID);
        }

        public Module GetModule(int siteID,string moduleName)
        {
            //return dal.GetModule(moduleName);

            return WeakRefCache.Modules.FirstOrDefault(a =>!a.IsDelete && 
                                                        (a.SiteId==0 || siteID==0 || siteID==a.SiteId) && 
                                                        String.Compare(a.Name,moduleName,true)==0);
        }

        /// <summary>
        /// 获取模块的Json数据
        /// </summary>
        /// <returns></returns>
        public string GetModuleJson()
        {
            string json;
            StringBuilder sb = new StringBuilder("[");
            foreach (var m in this.GetModules())
            {
                if (!m.IsDelete)
                {
                    sb.Append("{\"id\":\"").Append(m.ID.ToString()).Append("\",\"name\":\"").Append(m.Name).Append("\"},");
                }
            }
            if (sb.Length > 1) sb.Remove(sb.Length - 1, 1);
            json = sb.Append("]").ToString();
            return json;
        }
    }
}
