

using J6.DevFw.Toolkit.NetCrawl;
using J6.DevFw.Toolkit.Tags;

namespace sp.datapicker
{
    //
    // Copyright 2011 (C) Z3Q.NET,All rights reseved.
    // Project : OPSite.Plugin
    // Name : 采集插件
    // author : newmin
    // Date : 2011-08-27
    //

    using System;
    using System.IO;
    using System.Text;
    using J6.Cms.Domain.Interface.Content.Archive;
    using J6.Cms.DataTransfer;
    using J6.Cms;
    using J6.Cms.Web.WebManager;
    using J6.Cms.CacheService;
    using System.Collections.Generic;
    using J6.Cms.Utility;
    using J6.Cms.Conf;
    using System.Text.RegularExpressions;
    using System.Net;


    public class CollectionExtend : WebManage
    {
        private int siteId;
        private static IDictionary<int, HttpTags> tags =
            new Dictionary<int, HttpTags>();

        private static Regex linkRegex = new Regex(
            "<A\\s*[^>]+>\\s*([\\s\\S]+?)\\s*</A>",

            RegexOptions.IgnoreCase);
        private static Regex srcRegex = new Regex(
            "((src|href)\\s*=\\s*(\"|')*)/");

        private static Regex remoteImgRegex = new Regex(
            "IMG[^>]*?src\\s*=\\s*(?:\"(?<1>[^\"]*)\"|'(?<1>[^\']*)')",
            RegexOptions.IgnoreCase);

        public CollectionExtend(Collector collector)
            : base(collector)
        {
        }

        //开始之前执行
        public override void Begin_Request()
        {
            SiteDto site = CmsWebMaster.CurrentManageSite;

            if (!(site.SiteId > 0)) throw new Exception("请登陆后再进行操作!");
            this.siteId = site.SiteId;

            //PermissionAttribute permission = new PermissionAttribute(HttpContext.Current.Request.Path);
            //permission.Validate(UserState.Administrator.Current);
        }


        /// <summary>
        /// 生成一个select HTML元素
        /// </summary>
        /// <param name="value">初始值</param>
        /// <returns></returns>
        private string GetSelectElement(string value)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<select id=\"categoryselect\" name=\"category\">");
            sb.Append("<option value=\"-1\">一 选择栏目 一</option>");
            ServiceCall.Instance.SiteService.HandleCategoryTree(this.siteId, 1, (c, level) =>
            {
                sb.Append("<option value=\"").Append(c.Id.ToString()).Append("\">");

                for (int i = 0; i < level; i++)
                {
                    sb.Append(CmsCharMap.Dot);
                }
                sb.Append(c.Name).Append("</option>");
            });


            sb.Append("</select>");

            return sb.ToString();
        }

        public override string Return_InvokePageHtml()
        {
            return String.Format(@"<h1 style=""margin:5px;padding:0;font-size:14px;color:green"">采集说明：</h1>
                                选择栏目：{0}&nbsp;&nbsp;&nbsp;&nbsp;<b style=""color:red"">发布选项：</b>
                                <input type=""checkbox"" style=""border: none"" name=""visible"" field=""visible"" title=""采集后即显示到网站！"" id=""ck_visible"" />
                                <label for=""ck_visible"">立即显示</label>                               
                                <input id=""ck_dlpic"" type=""checkbox"" style=""border: none"" name=""dlpic"" field=""dlpic"" title=""将图片从远程网站下载并存放到本地""/>
                                 <label for=""ck_dlpic"">图片远程下载</label>
                                <input id=""ck_removelink"" type=""checkbox"" style=""border: none"" name=""removelink"" field=""removelink"" title=""去除采集内容中的链接"" checked=""checked"" />
                                 <label for=""ck_removelink"">移除超链接</label>
                                <input id=""ck_autotag"" type=""checkbox"" style=""border: none"" name=""autotag"" field=""autotag"" title=""自动链接Tags"" checked=""checked"" />
                                 <label for=""ck_autotag"">生成内链(自动生成tags链接，有利于seo)</label>
                                ",
                this.GetSelectElement(null));
        }

        public override string Invoke_SinglePage(Project project, string pageUri)
        {
            project.UseMultiThread = true;

            //重置计数
            project.ResetState();

            int categoryId = int.Parse(request.Form["category"]);
            if (categoryId == -1) return "<strong>错误，请先选择采集目标栏目!<strong><br />";

            project.InvokeSingle(pageUri, GetDataPackHandler(categoryId));

            return String.Format("任务总数:{0},成功：{1},失败:{2}", project.State.TotalCount, project.State.SuccessCount,
                project.State.FailCount);
        }

        public override string Invoke_ListPage(Project project, string listPageUri)
        {
            project.UseMultiThread = true;

            //重置计数
            project.ResetState();

            int categoryId = int.Parse(request.Form["category"]);
            if (categoryId == -1) return "<strong>错误，请先选择采集目标栏目!<strong><br />";


            project.InvokeList(listPageUri, GetDataPackHandler(categoryId));

            return String.Format("任务总数:{0},成功：{1},失败:{2}", project.State.TotalCount, project.State.SuccessCount,
                project.State.FailCount);
        }
        public override string Invoke_ListPage(Project project, object parameter)
        {
            project.UseMultiThread = true;

            //重置计数
            project.ResetState();

            int categoryId = int.Parse(request.Form["category"]);
            if (categoryId == -1) return "<strong>错误，请先选择采集目标栏目!<strong><br />";

            project.InvokeList(parameter, GetDataPackHandler(categoryId));

            return String.Format("任务总数:{0},成功：{1},失败:{2}",
                project.State.TotalCount,
                project.State.SuccessCount,
                project.State.FailCount);
        }

        private HttpTags GetTags(int siteId)
        {
            HttpTags _tags = null;
            if (!tags.Keys.Contains(siteId))
            {
                string dirPath = String.Concat(Cms.PyhicPath, CmsVariables.SITE_CONF_PRE_PATH, siteId.ToString(),"/");
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath).Create();
                }
                _tags = new HttpTags(String.Concat(dirPath, "tags.conf"));
                tags.Add(siteId, _tags);
            }
            else
            {
                _tags = tags[siteId];
            }
            return _tags;
        }


        private DataPackFunc GetDataPackHandler(int categoryId)
        {
            int publisherId;
            string flag;
            bool isAutoTag;
            bool removeLink;
            bool dlPic;

            publisherId = UserState.Administrator.Current.Id;

            flag = ArchiveFlag.GetFlagString(
             false,
             false,
             request.Form["visible"] == "on",
             false,
             null);

            isAutoTag = request.Form["autotag"] == "on";
            removeLink = request.Form["removelink"] == "on";
            dlPic = request.Form["dlpic"] == "on";
            string domain = null;
            HttpTags _tags = this.GetTags(siteId);

            return data =>
             {
                 string content = data["content"];
                 string thumbnail = data["thumbnail"] ?? (data["image"] ?? "");
                 bool thumbIsNull = String.IsNullOrEmpty(thumbnail);

                 if (domain == null)
                 {
                     Match m = Regex.Match(data.ReferenceUrl, "((http|https)://[^/]+)/(.+?)"
                         , RegexOptions.IgnoreCase);
                     domain = m.Groups[1].Value;
                 }

                 //替换src
                 content = srcRegex.Replace(content, "$1" + domain + "/");
                 if (!thumbIsNull && thumbnail[0] == '/')
                 {
                     thumbnail = domain + thumbnail;
                 }

                 //移除链接
                 if (removeLink)
                 {
                     content = linkRegex.Replace(content, "$1");
                 }

                 //远程下载
                 if (dlPic)
                 {
                     content = AutoUpload(content);
                     if (!thumbIsNull)
                     {
                         thumbnail = downloadThumbnail(thumbnail);
                     }
                 }


                 //自动替换Tags
                 if (isAutoTag)
                 {
                     // content = _tags.Tags.RemoveAutoTags(content);
                     content = _tags.Tags.ReplaceSingleTag(content);
                 }

                 this.CreateNewArchive(categoryId, content, thumbnail, publisherId, flag, data);
             };
        }


        /// <summary>
        /// 创建新的文档
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="data"></param>
        /// <param name="thumbnail"></param>
        /// <param name="publisherId" />
        /// <param name="categoryId"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private int CreateNewArchive(int categoryId, string content, string thumbnail, int publisherId, string flag, DataPack data)
        {

            ArchiveDto archive = new ArchiveDto
            {
                Agree = 0,
                Disagree = 0,
                Category = new CategoryDto { Id = categoryId },
                Flags = flag,
                PublisherId = publisherId,
                Tags = String.Empty,
                Outline = String.Empty,
                Source = String.Empty,
                Title = data["title"],
                Content = content,
                LastModifyDate = DateTime.Now,
                CreateDate = DateTime.Now,
                Thumbnail = thumbnail
            };

            return ServiceCall.Instance.ArchiveService.SaveArchive(this.siteId, archive);
        }

        /// <summary>
        /// 自动上传文本中的图片
        /// </summary>
        /// <param name="content">文本内容</param>
        /// <returns>上传结果信息</returns>
        private string AutoUpload(string content)
        {
            MatchCollection matches = remoteImgRegex.Matches(content);
            if (matches.Count == 0) return content;

            //自动保存远程图片
            WebClient client = new WebClient();
            DateTime dt = DateTime.Now;
            bool isRecordError = true;

            string saveDir = String.Format("/{0}s{1}/image/{2:yyyyMM}/",
                   CmsVariables.RESOURCE_PATH, this.siteId.ToString(), dt);
            string physicDir = Cms.PyhicPath + saveDir;
            if (!Directory.Exists(physicDir)) Directory.CreateDirectory(physicDir).Create();

            foreach (Match match in matches)
            {
                string imgUrl = match.Groups[1].Value;
                string ext = imgUrl.Substring(imgUrl.LastIndexOf(".") + 1);
                string newFile = String.Format("{0}p_{1:yyyyMMddHHmmss_ffff}.{2}", saveDir, dt, ext);

                try
                {
                    //保存图片
                    client.DownloadFile(imgUrl,Cms.PyhicPath + newFile);
                    content = content.Replace(imgUrl, newFile);
                }
                catch(Exception exc)
                {
                    if (!isRecordError)
                    {
                        isRecordError = true;
                        content = "[Error]下载远程图片过程中发生错误：" + exc.Message + "\n下载目录:" + physicDir
                            +"\n" + exc.StackTrace + "\r\n<br />" + content;
                    }
                }
                client.Dispose();
            }

            return content;
        }

        /// <summary>
        /// 下载缩略图
        /// </summary>
        /// <param name="thumbnail"></param>
        /// <returns></returns>
        private string downloadThumbnail(string thumbnail)
        {
            //自动保存远程图片
            WebClient client = new WebClient();
            DateTime dt = DateTime.Now;

            string saveDir = String.Format("/{0}s{1}/image/{2:yyyyMM}/",
                   CmsVariables.RESOURCE_PATH, this.siteId.ToString(), dt);
            if (!Directory.Exists(saveDir)) Directory.CreateDirectory(saveDir).Create();

            string ext = thumbnail.Substring(thumbnail.LastIndexOf(".") + 1);
            string newFile = String.Format("{0}thumb_{1:yyyyMMddHHmmss}.{2}", saveDir, dt, ext);

            try
            {
                //保存图片
                client.DownloadFile(thumbnail, Cms.PyhicPath + newFile);
            }
            catch
            {
                client.Dispose();
                return "";
            }
            client.Dispose();
            return newFile;
        }
    }

}
