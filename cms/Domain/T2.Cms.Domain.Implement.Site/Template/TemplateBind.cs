using T2.Cms.Domain.Interface.Site.Template;

namespace T2.Cms.Domain.Implement.Site.Template
{
    public class TemplateBind:ITemplateBind
    {

        internal TemplateBind(int id,TemplateBindType type,string templatePath)
        {
            this.Id = id;
            this.BindType = type;
            this.TplPath = templatePath;
        }

        public int Id
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

        public int GetDomainId()
        {
            return this.Id;
        }
    }
}
