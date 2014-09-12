using Ops.Cms.Domain.Interface.Site.Template;

namespace Ops.Cms.Domain.Implement.Site.Template
{
    public class TemplateBind:ITemplateBind
    {

        internal TemplateBind(int id,TemplateBindType type,string templatePath)
        {
            this.ID = id;
            this.BindType = type;
            this.TplPath = templatePath;
        }

        public int ID
        {
            get;
            set;
        }


        public TemplateBindType BindType
        {
            get;
            private set;
        }

        public string TplPath
        {
            get;
            set;
        }

        public int BindRefrenceId
        {
            get;
            set;
        }
    }
}
