using System.Collections.Generic;
using JR.Cms.Domain.Interface.Site.Category;
using JR.Cms.Infrastructure;

namespace JR.Cms.Domain.Interface.Site.Template
{
    public interface ITemplateRepo
    {
        /// <summary>
        /// 创建模板绑定
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="templatePath"></param>
        /// <returns></returns>
        TemplateBind CreateTemplateBind(int id, TemplateBindType type, string templatePath);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bindRelationId"></param>
        /// <param name="templateBindType"></param>
        /// <returns></returns>
        TemplateBind GetTemplateBind(int bindRelationId, TemplateBindType templateBindType);

        /// <summary>
        /// 获取栏目的模板绑定
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        IEnumerable<TemplateBind> GetTemplateBindsForCategory(ICategory category);


        /// <summary>
        /// 保存模板绑定
        /// </summary>
        /// <param name="templateBind"></param>
        /// <param name="refrenceId"></param>
        /// <returns></returns>
        int SaveTemplateBind(int refrenceId, TemplateBind templateBind);

        /// <summary>
        /// 删除模板绑定
        /// </summary>
        /// <param name="refrenceId"></param>
        /// <param name="templateBindType"></param>
        /// <returns></returns>
        Error RemoveBind(int refrenceId, TemplateBindType templateBindType);

        /// <summary>
        /// 删除模板绑定
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        Error RemoveBinds(int refrenceId, TemplateBind[] list);

        /// <summary>
        /// 保存模板绑定
        /// </summary>
        /// <param name="refrenceId"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        Error SaveTemplateBinds(int refrenceId, TemplateBind[] list);
    }
}
