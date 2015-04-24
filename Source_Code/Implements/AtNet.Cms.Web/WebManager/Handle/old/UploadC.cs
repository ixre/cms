//
// Copyright (C) 2007-2008 S1N1.COM,All rights reseved.
// 
// Project: AtNet.Cms.Manager
// FileName : Ajax.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/10/15 21:16:56
// Description :
//
// Get infromation of this software,please visit our site http://cms.ops.cc
//
//

using AtNet.Cms.Conf;
using AtNet.DevFw.Framework.Extensions;

namespace AtNet.Cms.WebManager
{
    using AtNet.DevFw.Framework.Web.UI;
    using System;

    public class UploadC:BasePage
    {

        /// <summary>
        /// 清除缓存
        /// </summary>
        public void UploadImage_POST()
        {
            string uploadfor = base.Request["for"];
            string id = base.Request["upload.id"];
            DateTime dt = DateTime.Now;
            string dir = string.Format("/{0}s{1}/image/{2:yyyyMM}/", CmsVariables.RESOURCE_PATH, base.CurrentSite.SiteId.ToString(), dt);
            string name = String.Format("{0}{1:ddHHss}{2}",
                String.IsNullOrEmpty(uploadfor) ? "" : uploadfor + "_",
                dt, String.Empty.RandomLetters(4));

            string file = new FileUpload(dir, name).Upload();
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
            string dir = string.Format("/{0}s{1}/attachment/{2:yyyyMM}/",
                CmsVariables.RESOURCE_PATH,
                base.CurrentSite.SiteId.ToString(), dt);
            string name = String.Format("{0:ddHHss}{1}", dt, String.Empty.RandomLetters(4));
            string file = new FileUpload(dir, name).Upload();
            Response.Write("{"+String.Format("url:'{0}'",file)+"}");
        }
    }
}
