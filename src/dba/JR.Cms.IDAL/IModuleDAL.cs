/*
* Copyright(C) 2010-2012 TO2.NET
* 
* File Name	: ImoduleDAL
* Author	: Newmin (new.min@msn.com)
* Create	: 2012/9/30 10:03:33
* Description	:
*
*/

using System.Collections.Generic;
using JR.Cms.Domain.Interface.Models;

namespace JR.Cms.IDAL
{
   public interface ImoduleDAL
    {
       /// <summary>
       /// 添加模块
       /// </summary>
       /// <param name="module"></param>
       /// <returns></returns>
       bool AddModule(Module module);

       /// <summary>
       /// 删除模块
       /// </summary>
       /// <param name="moduleId"></param>
       /// <returns></returns>
       bool DeleteModule(int moduleID);

       /// <summary>
       /// 更新模块
       /// </summary>
       /// <param name="module"></param>
       /// <returns></returns>
       bool UpdateModule(Module module);

       /// <summary>
       /// 获取所有模块
       /// </summary>
       /// <returns></returns>
       IList<Module> GetModules();

       /// <summary>
       /// 根据ID获取模块
       /// </summary>
       /// <param name="moduleId"></param>
       /// <returns></returns>
       Module GetModule(int moduleID);

       /// <summary>
       /// 根据模块名称获取模块
       /// </summary>
       /// <param name="moduleName"></param>
       /// <returns></returns>
       Module GetModule(string moduleName);

    }
}
