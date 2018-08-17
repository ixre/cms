using T2.Cms.Domain.Interface.Site.Template;

namespace T2.Cms.Domain.Implement.Site.Template
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BaseTemplateRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="templatePath"></param>
        /// <returns></returns>
        public ITemplateBind CreateTemplateBind(int id, TemplateBindType type, string templatePath)
        {
            return new TemplateBind(id, type, templatePath);
        }
    }
}
