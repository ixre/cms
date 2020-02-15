using System;
using System.Collections.Generic;
using JR.Cms.Dal;
using JR.Cms.Domain.Interface.Models;
using JR.Cms.IDAL;
using JR.Cms.Infrastructure;

namespace JR.Cms.Library.DataAccess.BLL
{
    /// <summary>
    /// 弱引用缓存
    /// </summary>
    public class WeakRefCache
    {
        public static WatchBehavior OnModuleBuilting;


        #region  Module

        //栏目的弱引用,保证释放资源时回收
        private static WeakReference module_ref;
        private static ImoduleDAL dal = new ModuleDAL();
        private static IList<Module> modules = new List<Module>();



        /// <summary>
        /// 模块缓存
        /// </summary>
        public static IList<Module> Modules
        {
            get
            {
                if (module_ref == null)
                {
                    RebuiltModule();
                }
                return module_ref.Target as IList<Module>;
            }
        }

        /// <summary>
        /// 重建模块缓存
        /// </summary>
        public static void RebuiltModule()
        {

            //释放资源
            module_ref = null;


            //重新赋值
            if (modules != null)
            {
                for (int i = 0; i < modules.Count; i++)
                {
                    modules.Remove(modules[i]);
                }
            }

            modules = dal.GetModules();

            //指定弱引用
            module_ref = new WeakReference(modules);

            if (OnModuleBuilting != null)
            {
                OnModuleBuilting();
            }

        }

        #endregion

   

    }
}
