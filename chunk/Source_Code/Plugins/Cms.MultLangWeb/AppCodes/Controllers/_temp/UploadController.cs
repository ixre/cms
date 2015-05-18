using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ops.Web.UI;

namespace Cms.Web.AppCodes.Controllers
{
    public class UploadController : Controller
    {
        //
        // GET: /Upload/

        public void Index()
        {
            FileUpload f = new FileUpload( "/upload/", String.Format("{0:yyyyMMddhhmmss}", DateTime.Now));
            f.Upload();
        }

        [AcceptVerbs("POST")]
        public string Process(string processID)
        {
            return FileUpload.GetProcessJson(processID);
        }
    }
}
