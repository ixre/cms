//
// Copyright (C) 2007-2008 OPSoft INC,All rights reseved.
// 
// Project: OPS.OPSite.Manager
// FileName : Ajax.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/10/15 21:16:56
// Description :
//
// Get infromation of this software,please visit our site http://www.ops.cc
//
//

namespace Ops.Cms.WebManager
{
    using Ops.Cms.Conf;
    using Ops.Web.UI;
    using System;
    using System.Web;

    public class C_Upload:BasePage
    {

        /// <summary>
        /// 清除缓存
        /// </summary>
        public void UploadImage_POST()
        {
            string uploadfor = base.Request["for"];
            string id = base.Request["upload.id"];
            DateTime dt = DateTime.Now;
            string dir = string.Format("/{0}s{1}/images/{2:yyyyMMdd}/", CmsVariables.RESOURCE_PATH, base.CurrentSite.SiteId.ToString(), dt);
            string name = String.Format("{0}{1:HHss}{2}",
                String.IsNullOrEmpty(uploadfor) ? "" : uploadfor + "_",
                dt, String.Empty.RandomLetters(4));

            string file = new FileUpload(dir, name).Upload(false);
            Response.Write("{" + String.Format("url:'{0}'", file) + "}");
        }
        /// <summary>
        /// 清除缓存
        /// </summary>
        public void UploadFile_POST()
        {
            string uploadfor = base.Request["for"];
            string id = base.Request["upload.id"];
            DateTime dt = DateTime.Now;
            string dir = string.Format("/{0}s{1}/attachments/{2:yyyyMMdd}/",
                CmsVariables.RESOURCE_PATH,
                base.CurrentSite.SiteId.ToString(), dt);
            string name = String.Format("{0:yyyyMMdd_ss}{1}", dt, String.Empty.RandomLetters(4));
            string file = new FileUpload(dir, name).Upload(false);
            Response.Write("{"+String.Format("url:'{0}'",file)+"}");
        }

        /// <summary>
        /// 获取进程信息
        /// </summary>
        public void GetProcess_POST()
        {
            string processID = HttpContext.Current.Request["processId"];
            HttpContext.Current.Response.Write(FileUpload.GetProcessJson(processID));
        }
    }
}
