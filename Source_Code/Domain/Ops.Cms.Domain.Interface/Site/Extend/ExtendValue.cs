using Ops.Cms.Domain.Interface.Site.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ops.Cms.Domain.Interface.Site.Extend
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
    }
}
