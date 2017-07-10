//
// Copyright (C) 2007-2008 Z3Q.NET,All rights reseved.
// 
// Project: jr.Cms.Manager
// FileName : Ajax.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/10/15 21:16:56
// Description :
//
// Get infromation of this software,please visit our site http://k3f.net/cms
//
//

using System;
using JR.Cms.Conf;
using JR.Cms.WebManager;
using JR.DevFw.Framework.Extensions;
using JR.DevFw.Framework.Web.UI;

namespace JR.Cms.Web.WebManager.Handle
{
    public class UploadC:BasePage
    {

        /// <summary>
        /// 清除缓存
        /// </summary>
        public void UploadImage_POST()
        {
            //远程上传
            System.Web.HttpPostedFile postfile = Request.Files[0];
            string strOutput = string.Empty;
            System.Net.CookieContainer cookie = new System.Net.CookieContainer();
            HttpPostFile("http://img.0xa.com/upload/index", postfile, null, null, ref strOutput);

            JsonData JD = JsonMapper.ToObject(strOutput);
            Response.Write("{" + String.Format("url:'{0}'", "http://img.0xa.com"+JD["raw"]) + "}");
            
            //string uploadfor = base.Request["for"];
            //string id = base.Request["upload.id"];
            //DateTime dt = DateTime.Now;
            //string dir = string.Format("/{0}s{1}/image/{2:yyyyMM}/", CmsVariables.RESOURCE_PATH, base.CurrentSite.SiteId.ToString(), dt);
            //string name = String.Format("{0}{1:ddHHss}{2}",
                //String.IsNullOrEmpty(uploadfor) ? "" : uploadfor + "_",
                //dt, String.Empty.RandomLetters(4));

            //string file = new FileUpload(dir, name).Upload();
            //Response.Write("{" + String.Format("url:'{0}'", file) + "}");
        }

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
