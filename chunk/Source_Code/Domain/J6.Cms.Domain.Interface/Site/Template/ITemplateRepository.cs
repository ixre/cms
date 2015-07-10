using System.Collections.Generic;
using J6.Cms.Domain.Interface.Site.Category;

namespace J6.Cms.Domain.Interface.Site.Template
{
    public interface ITemplateRepository
    {
        /// <summary>
        /// 创建模板绑定
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="templatePath"></param>
        /// <returns></returns>
        ITemplateBind CreateTemplateBind(int id, TemplateBindType type, string templatePath);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bindRelationId"></param>
        /// <param name="templateBindType"></param>
        /// <returns></returns>
        ITemplateBind GetTemplateBind(int bindRelationId, TemplateBindType templateBindType);

        /// <summary>
        /// 获取栏目的模板绑定
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        IEnumerable<ITemplateBind> GetTemplateBindsForCategory(ICategory category);

        /// <summary>
        /// 保存模板绑定
        /// </summary>
        /// <param name="templateBind"></param>
        /// <param name="bindRefrenceId"></param>
        /// <returns></returns>
        int SaveTemplateBind(ITemplateBind templateBind, int bindRefrenceId);

        void RemoveBind(TemplateBindType templateBindType, int bindRefrenceId);
    }
}
