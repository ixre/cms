
namespace T2.Cms.Domain.Interface.Site.Template
{
    /// <summary>
    /// 模板绑定
    /// </summary>
    public class TemplateBind : IValueObject
    {

        public TemplateBind(int id, TemplateBindType type, string templatePath)
        {
            this.ID = id;
            this.BindType = type;
            this.TplPath = templatePath;
        }

        /// <summary>
        /// 编号
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 绑定关联编号
        /// </summary>
        public int BindRefrenceId { get; set; }

        /// <summary>
        /// 绑定类型
        /// </summary>
        public TemplateBindType BindType { get; }

        /// <summary>
        /// 模板路径
        /// </summary>
        public string TplPath { get; set; }

        public bool Equal(IValueObject that)
        {
            throw new System.NotImplementedException();
        }
    }
}
