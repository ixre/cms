
namespace T2.Cms.Domain.Interface.Site.Template
{
    public interface ITemplateBind:IDomain<int>
    {
        /// <summary>
        /// 绑定关联编号
        /// </summary>
        int BindRefrenceId { get; set; }

        /// <summary>
        /// 绑定类型
        /// </summary>
        TemplateBindType BindType { get;}

        /// <summary>
        /// 模板路径
        /// </summary>
        string TplPath { get; set; }
    }
}
