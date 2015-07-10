//
// Copyright (C) 2007-2008 S1N1.COM,All rights reseved.
// 
// Project: OPSite.XmlRpc
// FileName : RPCService.cs
// author : PC-CWLIU (new.min@msn.com)
// Create : 2011/10/20 12:22:02
// Description :
//
// Get infromation of this software,please visit our site http://cms.ops.cc
// Modify:
//  2013-04-24 07:11 newmin [!]: deletePost修正了删除文档的bug,必须为本人添加才可以删除
//
//

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using CookComputing.MetaWeblog;
using CookComputing.XmlRpc;
using J6.Cms;
using J6.Cms.CacheService;
using J6.Cms.DataTransfer;
using J6.Cms.Domain.Interface.Content.Archive;
using J6.Cms.BLL;
using Post = CookComputing.MetaWeblog.Post;
using J6.Cms.Conf;
using J6.Cms.Domain.Interface.Models;
using J6.Cms.Domain.Interface.User;
using J6.Cms.Domain.Interface.Value;
using J6.Cms.XmlRpc;
using IUser = J6.Cms.Domain.Interface._old.IUser;

namespace sp.xmlrpc.XmlRpc.src
{
    public class WeblogRPCService : XmlRpcService, ImetaWeblog, ILiveWriterExtension
    {
        private static IUser _ubll;
        private static IUser ubll { get { return _ubll ?? (_ubll = CmsLogic.User); } }

        /// <summary>
        /// 是否使用Base64来保存较小的图片
        /// </summary>
        public static bool EnableBase64Images = !true;

        /// <summary>
        /// 文档地址
        /// </summary>
        private static string post_uri;

        private int siteId;

        public WeblogRPCService()
        {

            //计算文档的地址
            string archiveUri = "/temp/{0}.html";

            //if (archiveUri.StartsWith("/")) archiveUri = archiveUri.Substring(1);
            post_uri = GetDomain() + archiveUri;
        }

        internal bool Internal_BeginRequest(HttpContext context)
        {
            return this.BeginRequest(context);
        }
        protected override bool BeginRequest(HttpContext context)
        {
            string query = context.Request.Url.Query;

            //提供自动检测的文件
            if (String.Compare(query, "?wsd", true) == 0)
            {
                Print_EditXMLConfig(); return false;
            }
            else if (query.StartsWith("?test"))
            {
                getUsersBlogs("1", HttpContext.Current.Request["user"], HttpContext.Current.Request["pwd"]); return false;
            }


            //继续执行，调用XMLRPC
            return true;
        }


        #region SOME GET Method

        /// <summary>
        /// 显示编辑配置
        /// </summary>
        private void Print_EditXMLConfig()
        {

            //返回的WSD配置
            const string xmlFormat =
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<rsd version=""1.0"" xmlns=""http://archipelago.phrasewise.com/rsd"">
  <service>
    <engineName>J6.Cms.NET! XML-RPC PLUGIN!</engineName>
    <engineLink>http://www.ops.cc/cms/xmlrpc</engineLink>
    <homePageLink>{0}</homePageLink>
    <apis>
      <api name=""MetaWeblog"" blogID=""1"" preferred=""true"" apiLink=""{0}{1}"" />
    </apis>
  </service>
</rsd>";
            string domain;
            HttpRequest request = HttpContext.Current.Request;
            HttpResponse response = HttpContext.Current.Response;
            domain = GetDomain();

            response.Write(String.Format(xmlFormat, domain, request.Path));
            response.ContentType = "text/xml";

        }

        #endregion


        /// <summary>
        /// 验证用户是否正确
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private UserDto ValidUser(string username, string password)
        {
            LoginResultDto result = ServiceCall.Instance.UserService.TryLogin(username, password);
            if (result == null || result.Tag == -1) throw new XmlRpcFaultException(500, "用户密码不正确!");
            if (result.Tag == -2) throw new XmlRpcFaultException(500, "用户已被停用!");

            UserDto usr = ServiceCall.Instance.UserService.GetUser(result.Uid);


            if (usr.SiteId > 0)
            {
                this.siteId = usr.SiteId;
            }
            else
            {
                this.siteId = Cms.Context.CurrentSite.SiteId;
            }
            return usr;
        }

        /// <summary>
        /// 获取域名
        /// </summary>
        /// <returns></returns>
        private static string GetDomain()
        {
            //获得当前域名
            string domain;
            const string portDomainStr = "http://{0}:{1}";
            const string domainStr = "http://{0}";

            HttpRequest request = HttpContext.Current.Request;
            domain = String.Format(request.Url.IsDefaultPort ?
                domainStr : portDomainStr, request.Url.Host, request.Url.Port);
            return domain;
        }


        #region ILiveWriterAPI

        /// <summary>
        /// 获取博客列表
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public CookComputing.Blogger.BlogInfo[] getUsersBlogs(string appKey, string username, string password)
        {
            UserDto user;
            if ((user = ValidUser(username, password)) != null)
            {
                //获得当前域名
                string domain;
                const string portDomainStr = "http://{0}:{1}";
                const string domainStr = "http://{0}";

                HttpRequest request = HttpContext.Current.Request;
                domain = String.Format(request.Url.IsDefaultPort ? domainStr : portDomainStr, request.Url.Host, request.Url.Port);



                SiteDto site2 = SiteCacheManager.GetSite(user.SiteId > 0 ? user.SiteId : this.siteId);

                //返回博客列表
                return new CookComputing.Blogger.BlogInfo[] 
                    {
                        new CookComputing.Blogger.BlogInfo{blogid=site2.SiteId.ToString(),blogName=site2.Name,url=domain }
                    };

                //========================================================//

                //返回单个站点
                if (user.SiteId > 0)
                {
                    SiteDto site = SiteCacheManager.GetSite(user.SiteId);
                    //返回博客列表
                    return new CookComputing.Blogger.BlogInfo[] 
                    {
                        new CookComputing.Blogger.BlogInfo{blogid=site.SiteId.ToString(),blogName=site.Name,url=domain }
                    };
                }
                else
                {
                    IList<SiteDto> sites = SiteCacheManager.GetAllSites();
                    CookComputing.Blogger.BlogInfo[] blogs = new CookComputing.Blogger.BlogInfo[sites.Count];
                    for (int i = 0; i < sites.Count; i++)
                    {
                        blogs[i] = new CookComputing.Blogger.BlogInfo
                        {
                            blogid = sites[i].SiteId.ToString(),
                            blogName = sites[i].Name,
                            url = !String.IsNullOrEmpty(sites[i].Domain) ?
                            "http://" + sites[i].Domain + "/" :
                            (!String.IsNullOrEmpty(sites[i].DirName) ?
                            domain + "/" + sites[i].DirName + "/" : domain
                            )
                        };
                    }
                    return blogs;
                }
            }
            return null;
        }

        /// <summary>
        /// 删除文档
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="postid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="publish"></param>
        /// <returns></returns>
        public bool deletePost(string appKey, string postid, string username, string password, bool publish)
        {
            throw new NotImplementedException();
            //todo:
//            if (ValidUser(username, password) != null)
//            {
//                User user = ubll.GetUser(username);
//                ArchiveDto archive = ServiceCall.Instance.ArchiveService.GetArchiveById(this.siteId, int.Parse(postid));
//                if (user.Group == UserGroups.Master || String.Compare(user.UserName, archive.publisher_id, true) == 0)
//                {
//                    ServiceCall.Instance.ArchiveService.DeleteArchive(this.siteId, archive.Id);
//                    return true;
//                }
//            }
//            return false;
        }


        #endregion


        #region MetaWeblogAPI

        /// <summary>
        /// 编辑文档
        /// </summary>
        /// <param name="postid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="post"></param>
        /// <param name="publish"></param>
        /// <returns></returns>
        public object editPost(string postid, string username, string password, Post post, bool publish)
        {
            UserDto user;
            if ((user = ValidUser(username, password)) != null)
            {
                ArchiveDto a = ServiceCall.Instance.ArchiveService.GetArchiveById(this.siteId, int.Parse(postid));
                if (a.Id > 0)
                {
                    //设置栏目
                    if (post.categories.Length != 0)
                    {
                        CategoryDto category = ServiceCall.Instance.SiteService.GetCategoryByName(this.siteId, post.categories[0]);

                        if (category.Id > 0) a.Category = category;
                        else return "";
                    }

                    a.Title = post.title;
                    a.Content = post.description;
                    a.PublisherId = user.Id;
                    a.Source = !String.IsNullOrEmpty(post.source.name) ? post.source.name : a.Source;

                    //更新
                    ServiceCall.Instance.ArchiveService.SaveArchive(this.siteId, a);

                    //执行监视服务
                    /*
                    try
                    {
                        WatchService.UpdateArchive(a);
                    }
                    catch { }
                     */
                }
                else
                {
                    throw new XmlRpcFaultException(0, "文档不存在或已经被删除");
                }

            }
            return null;
        }

        /// <summary>
        /// 获取栏目
        /// </summary>
        /// <param name="blogid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public CategoryInfo[] getCategories(string blogid, string username, string password)
        {
            //第一个博客
            UserDto user;
            if ((user = ValidUser(username, password)) != null)
            {
                CategoryInfo[] categoryInfos;

                IList<CategoryDto> categories;

                //if (user.SiteId > 0)
                //{
                //获取当前管理员的站点
                //    categories =new List<global::Spc.Models.Category>(cbll.GetCategories(a => a.SiteId == user.SiteId));
                //}
                //else
                //{
                //    categories = cbll.GetCategories();
                //}


                categories = new List<CategoryDto>(ServiceCall.Instance.SiteService.GetCategories(this.siteId));
                if (categories == null || categories.Count == 0) return null;


                //赋值栏目数组
                categoryInfos = new CategoryInfo[categories.Count];

                for (int i = 0; i < categories.Count; i++)
                {
                    categoryInfos[i] = new CategoryInfo
                    {
                        categoryid = categories[i].Id.ToString(),
                        title = categories[i].Name,
                        description = categories[i].Description ?? String.Empty,
                        htmlUrl = String.Format("/{0}/", categories[i].Tag),
                        rssUrl = String.Format("/rss/{0}/", categories[i].Tag)
                    };
                }

                return categoryInfos;
            }
            throw new XmlRpcFaultException(0, "用户密码不正确");
        }


        /// <summary>
        /// 获取文档
        /// </summary>
        /// <param name="postid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Post getPost(string postid, string username, string password)
        {
            Post post = default(Post);
            UserDto user;
            if ((user = ValidUser(username, password)) != null)
            {
                ArchiveDto a = ServiceCall.Instance.ArchiveService.GetArchiveById(this.siteId, int.Parse(postid));

                if (a.Id > 0)
                {
                    CategoryDto category = a.Category;
                    string categoryName = category.Name;

                    post = new Post
                    {
                        postid = a.Id,                                                  //编号
                        title = a.Title,                                                 //标题
                        categories = new string[] { categoryName },           //栏目
                        description = a.Content,                                         //内容
                        userid = a.PublisherId.ToString(),                                               //作者
                        source = new Source { name = a.Source },                        //来源
                        link = String.Format(post_uri,                                  //文档链接地址
                                        String.IsNullOrEmpty(a.Alias) ? a.StrId : a.Alias),

                    };

                }
            }
            return post;
        }


        /// <summary>
        /// 获取历史文档
        /// </summary>
        /// <param name="blogid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="numberOfPosts"></param>
        /// <returns></returns>
        public Post[] getRecentPosts(string blogid, string username, string password, int numberOfPosts)
        {
            Post[] posts;
            UserDto user;
            if ((user = ValidUser(username, password)) != null)
            {
                int totalRecords, pages;

                string[,] flags = new string[,]{
                {ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.IsSystem),""},
                {ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.IsSpecial),""},
                {ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.Visible),""},
                {ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.AsPage),""}
                };

                DataTable dt = ServiceCall.Instance.ArchiveService.GetPagedArchives(
                    this.siteId, null,
                    (user.RoleFlag & (int)RoleTag.Master)  != 0?0: user.Id, flags, null,
                    false, numberOfPosts, 1, out totalRecords, out pages);

                //如果返回的数量没有制定数多
                posts = new Post[dt.Rows.Count < numberOfPosts ? dt.Rows.Count : numberOfPosts];

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    posts[i] = new Post
                    {
                        postid = dt.Rows[i]["id"].ToString(),                                       //编号
                        title = dt.Rows[i]["title"].ToString(),                                     //标题
                        categories = new string[] { dt.Rows[i]["cid"].ToString() },           //栏目
                        description = dt.Rows[i]["content"].ToString(),                             //内容
                        userid = dt.Rows[i]["publisher_id"].ToString(),                                     //作者
                        source = new Source { name = dt.Rows[i]["source"].ToString() },             //来源
                        link = String.Format(post_uri,                                              //文档链接地址
                                       String.IsNullOrEmpty(dt.Rows[i]["aias"] as string) ?
                                       dt.Rows[i]["strid"].ToString() : dt.Rows[i]["alias"].ToString()),

                    };
                }

                return posts;
            }
            return null;
        }

        /// <summary>
        /// 创建新文档
        /// </summary>
        /// <param name="blogid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="post"></param>
        /// <param name="publish"></param>
        /// <returns></returns>
        public string newPost(string blogid, string username, string password, Post post, bool publish)
        {
            UserDto user;
            if ((user = ValidUser(username, password)) != null)
            {
                int categoryId = 0;
                string categoryName = post.categories[0];

                //根据提交的栏目设置栏目ID
                if (post.categories.Length != 0)
                {
                    var category = ServiceCall.Instance.SiteService.GetCategoryByName(this.siteId, categoryName);
                    if (category.Id > 0) categoryId = category.Id;
                }

                //如果栏目ID仍然为0,则设置第一个栏目
                if (categoryId == 0)
                {
                    throw new Exception("请选择分类!");
                }

                string flag = ArchiveFlag.GetFlagString(false, false, publish, false, null);

                ArchiveDto dto = new ArchiveDto
                {
                    Title = post.title,
                    PublisherId = user.Id,
                    Outline = String.Empty,
                    Content = post.description,
                    CreateDate = post.dateCreated,
                    Source = post.source.name,
                    ViewCount = 1,
                    Flags = flag,
                    Tags = String.Empty
                };
                dto.Category = new CategoryDto { Id = categoryId };

                return ServiceCall.Instance.ArchiveService.SaveArchive(this.siteId, dto).ToString(); ;

                //执行监视服务
                /*
                try
                {
                    WatchService.PublishArchive(abll.GetArchiveByID(id));
                }
                catch { }
                 */
            }

            return null;
        }

        /// <summary>
        /// 保存媒体文件
        /// </summary>
        /// <param name="blogid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public UrlData newMediaObject(string blogid, string username, string password, FileData file)
        {
            //try
            //{
            UserDto user;
            if ((user = ValidUser(username, password)) != null)
            {
                const string imgExtPattern = "^gif|jpg|jpeg|png|bmp$";          //图片扩展名正则
                const int maxImageFileSize = 48882;                             //最大47KB，超过47KB则保存为文件

                UrlData uri;

                //获取扩展名
                string ext = file.name.Substring(file.name.LastIndexOf(".") + 1);

                //返回图片格式的媒体文件
                if (Regex.IsMatch(file.name, imgExtPattern))
                {

                    //将图片转为JPG格式并保存到内存流中
                    MemoryStream stream = new MemoryStream(file.bits);

                    Bitmap originalImg,
                           newImg;

                    //创建原始图片
                    originalImg = new Bitmap(stream);

                    //绘制新的图片
                    newImg = new Bitmap(originalImg.Width, originalImg.Height);
                    Graphics g = Graphics.FromImage(newImg);
                    g.Clear(Color.White);
                    g.DrawImage(originalImg, new Point(0, 0));
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    //释放原图及内存流资源
                    originalImg.Dispose();
                    stream.Dispose();

                    //创建新的内存流，并将图片存到内存中
                    stream = new MemoryStream();
                    newImg.Save(stream, ImageFormat.Jpeg);

                    //如果使用Base64存取数据且大小小于指定则存为Base64
                    if (EnableBase64Images && stream.Length < maxImageFileSize)
                    {
                        //将内存流转为Base64格式字符
                        string base64Str = Convert.ToBase64String(stream.ToArray());

                        uri = new UrlData { url = String.Format("data:image/jpeg;base64,{0}", base64Str) };
                    }
                    else
                    {
                        //以文件方式存储图片

                        string dateStr;
                        string dirPath = GetMediaObjectSavePath(this.siteId, out dateStr);

                        //文件名称
                        string filePath = String.Format("{0}.{1}", String.Format("{0:hhmmfff}", DateTime.Now), "jpg");

                        //保存图片到文件

                        //EncoderParameters encodeParams = new EncoderParameters(1);
                        //encodeParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)100);
                        ////获取JPEG编码器
                        //ImageCodecInfo[] codes = ImageCodecInfo.GetImageDecoders();
                        //ImageCodecInfo _code = null;
                        //foreach (ImageCodecInfo code in codes)
                        //{
                        //    if (String.Compare(code.MimeType, "image/jpeg", true) == 0)
                        //        _code = code;
                        //}
                        //newImg.Save(dirPath + filePath,_code,encodeParams);

                        newImg.Save(dirPath + filePath, ImageFormat.Jpeg);

                        uri = new UrlData { url = String.Format("/{0}s{1}/lw/{2}/{2}",
                            CmsVariables.RESOURCE_PATH,
                            this.siteId.ToString(), dateStr, filePath) };

                    }

                    //释放资源
                    newImg.Dispose();
                    stream.Dispose();

                    //返回地址
                    return uri;

                }
                else
                {
                    //检测或创建以日期为命名的目录
                    string dateStr;
                    string dirPath = GetMediaObjectSavePath(this.siteId, out dateStr);

                    string filePath = String.Format("{0}.{1}", String.Format("{0:hhmmfff}", DateTime.Now), ext);

                    //保存文件
                    using (FileStream fs = new FileStream(dirPath + filePath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        fs.Write(file.bits, 0, file.bits.Length);
                        fs.Flush();
                        fs.Dispose();
                    }

                    return new UrlData { url = String.Format("/{0}s{1}/lw/{2}/{3}",
                        CmsVariables.RESOURCE_PATH,
                        this.siteId.ToString(),
                        dateStr, filePath) };

                }
            }

            //}
            //catch (Exception ex)
            //{
            //    new OPS.Log.LogFile(AppDomain.CurrentDomain.BaseDirectory + "log.txt").Append(ex.Message + "\r\n");
            //}

            return default(UrlData);
        }

        /// <summary>
        /// 获取媒体文件保存文件夹路径
        /// </summary>
        /// <param name="dateStr"></param>
        /// <returns></returns>
        private static string GetMediaObjectSavePath(int siteID, out string dateStr)
        {
            dateStr = String.Format("{0:yyyyMM}", DateTime.Now);
            string dirPath = String.Format("{0}/{1}s{2}/lw/{3}/", 
                AppDomain.CurrentDomain.BaseDirectory, 
                CmsVariables.RESOURCE_PATH,
                siteID, dateStr);

            //如果文件夹不存在，则创建
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath).Create();
            }
            return dirPath;
        }

        #endregion

    }
}
