using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using J6.Cms;
using J6.Cms.BLL;
using J6.Cms.Models;
using J6.Template;

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