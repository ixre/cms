using Ops.Cms.Domain.Interface.Site.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ops.Cms.DataTransfer
{
    public class TemplateBindDto
    {
        public int BindType
        {
            get;
            set;
        }

        public string TplPath
        {
            get;
            set;
        }

        public int ID
        {
            get;
            set;
        }
    }
}
