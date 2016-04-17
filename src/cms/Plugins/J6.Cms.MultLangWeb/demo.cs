using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spc.Web;
using Spc.Web.Mvc;

namespace Spc.Web
{
    internal class demo
    {
        public void Test()
        {
            //------------------ Global.asax --------------------

            //register routes
            Routes.RegisterCmsRoutes(null, "{lang}",typeof(MultLangCmsController));

            //set languages
            Lang.Set(new string[] {"zh-cn","en-us"});

            
        }
    }
}
