namespace JR.Cms.Domain.Interface.Site.Extend
{
    public class ExtendValue:IExtendValue
    {
        public ExtendValue(int id,IExtendField field,string value)
        {
            this.Id = id;
            this.Value = value;
            this.Field = field;
        }

        public IExtendField Field
        {
            get;
            private set;
        }


        public string Value
        {
            get;
           set;
        }

        public int Id
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
