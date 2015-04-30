/*
* Copyright(C) 2010-2012 OPSoft Inc
* 
* File Name	: TemplateBindBLL
* Author	: Administrator
* Create	: 2012/10/28 7:46:14
* Description	:
*
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spc.Models;
using Spc.DAL;
using Spc.IDAL;
using Spc.Logic;
using Ops.Cms;
using Ops.Cms.Domain.Interface.Site.Template;

namespace Spc.BLL
{
    public class TemplateBindBLL : Spc.Logic.ITemplateBind
    {

        private static ICategoryModel _category;
        private static TemplateBindDAL _dal;
        private static IModuleDAL _mdal;


        private static ICategoryModel cbll
        {
        	get{return _category??(_category=CmsLogic.Category);}
        	
        }
        private static TemplateBindDAL dal
        {
        	get{return _dal??(_dal=new TemplateBindDAL());}
        }
 		private static IModuleDAL mdal
        {
 			get{return _mdal??(_mdal=new ModuleDAL());}
        }

        ///// <summary>
        ///// 设置绑定关系
        ///// </summary>
        ///// <param name="bind"></param>
        ///// <returns></returns>
        //public bool SetBind(TemplateBind bind)
        //{
        //    //bool result= dal.SetBind(bind);

        //    //WeakRefCache.RebuiltTemplateBind();

        //    //return result;
        //}

        ///// <summary>
        ///// 获取绑定关系
        ///// </summary>
        ///// <param name="type"></param>
        ///// <param name="bindID"></param>
        ///// <returns></returns>
        //public TemplateBind GetBind(TemplateBindType type, string bindID)
        //{
        //    // return dal.GetBind(type, bindID);

        //    //int typeID=(int)type;
        //    //foreach (TemplateBind t in WeakRefCache.TemplateBinds)
        //    //{
        //    //    if (t.BindType == typeID && t.BindID == bindID)
        //    //    {
        //    //        return t;
        //    //    }
        //    //}
        //    return null;
        //}

        /// <summary>
        /// 获取栏目模板绑定
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        public TemplateBind GetCategoryTemplateBind(int categoryID)
        {
            TemplateBind bind;

            bind = GetBind(TemplateBindType.CategoryTemplate, categoryID.ToString());

            //如果栏目不存在绑定，则查找模块的绑定
            if (false || bind == null)
            {
                int moduleID = cbll.Get(a => a.ID == categoryID).ModuleID;
                bind = GetBind(TemplateBindType.ModuleCategoryTemplate, moduleID.ToString());
            }

            return bind;
        }

        /// <summary>
        /// 获取文档模板绑定
        /// </summary>
        /// <param name="archiveID"></param>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        public TemplateBind GetArchiveTemplateBind(string archiveID, int categoryID)
        {
            TemplateBind bind;

            bind = GetBind(TemplateBindType.ArchiveTemplate, archiveID);

            //如果文档不存在绑定，则查找栏目的绑定
            if (bind == null)
            {
                bind = GetBind(TemplateBindType.CategoryArchiveTemplate, categoryID.ToString());

                //如果栏目不存在绑定，则模块
                if (false || bind == null)
                {
                    int moduleID = cbll.Get(a => a.ID == categoryID).ModuleID;
                    bind = GetBind(TemplateBindType.ModuleArchiveTemplate, moduleID.ToString());
                }
            }

            return bind;
        }

        internal bool RemoveBind(TemplateBindType type, string bindID)
        {
            bool result=dal.RemoveBind(type, bindID);
            //WeakRefCache.RebuiltTemplateBind();
            return result;
        }

        /// <summary>
        /// 移除未关联的栏目模版绑定
        /// </summary>
        /// <returns></returns>
        public int RemoveErrorCategoryBind()
        {
            int result = dal.RemoveErrorCategoryBind();
            //WeakRefCache.RebuiltTemplateBind();
            return result;
        }
    }
}
