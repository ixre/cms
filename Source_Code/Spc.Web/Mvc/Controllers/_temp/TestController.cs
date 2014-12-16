using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ops.Cms;
using Ops.Cms.BLL;
using Ops.Cms.Models;
using Ops.Template;

namespace Cms.Web.AppCodes.Controllers
{
	
    public class TestController : ControllerBase
    {
        private static ArchiveBLL bll = new ArchiveBLL();
        private static CategoryBLL cbll = new CategoryBLL();


        public string TestID()
        {
          return  bll.__TESTID__();
        }

        [AcceptVerbs("POST")]
        public string Ajax(FormCollection form)
        {
            return form["name"];
        }
    }
}