using Ops.Cms.Domain.Interface.Site.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ops.Cms.Domain.Interface.Site.Extend
{
    public class ExtendField:IExtendField
    {
        //private IExtendFieldRepository _resp;

        public ExtendField(int id,string name)
        {
            this.Id = id;
            this.Name = name;
            //this._resp = resp;
        }

        public int Id
        {
            get;
            set;
        }
        public string Name
        {
            get;
            private set;
        }

        public string Type
        {
            get;
            set;
        }

        public string DefaultValue
        {
            get;
            set;
        }

        public string Regex
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }
    }
}
