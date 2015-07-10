/*
* Copyright(C) 2010-2012 OPSoft Inc
* 
* File Name	: ITemplateBindDAL
* Author	: Newmin (new.min@msn.com)
* Create	: 2012/10/28 7:23:01
* Description	:
*
*/

namespace Spc.IDAL
{

    using J6.Cms.Domain.Interface.Site.Template;
    using Spc.Models;
    using System.Collections.Generic;

    /// <summary>
    /// 模板绑定数据接口
    /// </summary>
    public interface ITemplateBindDAL
    {
        /// <summary>
        /// 设置绑定关系
        /// </summary>
        /// <param name="bind"></param>
        /// <returns></returns>
        bool SetBind(TemplateBind bind);

        /// <summary>
        /// 获取绑定关系
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="bindID">绑定关联编号</param>
        /// <returns></returns>
        TemplateBind GetBind(TemplateBindType type, string bindID);


        /// <summary>
        /// 移除绑定关系
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="bindID">绑定关联编号</param>
        /// <returns></returns> 
        bool RemoveBind(TemplateBindType type, string bindID);

        /// <summary>
        /// 移除未关联的栏目模版绑定
        /// </summary>
        /// <returns></returns>
        int RemoveErrorCategoryBind();

        /// <summary>
        /// 获取绑定列表
        /// </summary>
        /// <returns></returns>
        IList<TemplateBind> GetBindList();
    }
}
