using J6.Cms.Cache;
/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2013/12/27
 * Time: 7:34
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Caching;
using System.Xml;
using J6.Cms.Web;
using J6.Cms.Web.Resource;
using J6.Cms.Web.Resource.WebManager;

namespace J6.Cms.WebManager
{
    public enum ManagementPage
    {
        /// <summary>
        /// 登陆页
        /// </summary>
        Login = 1,

        /// <summary>
        /// 首页
        /// </summary>
        Index,

        /// <summary>
        /// 欢迎页
        /// </summary>
        Welcome,

        /// <summary>
        /// 服务器信息页
        /// </summary>
        Server_Summary,

        /// <summary>
        /// 首页
        /// </summary>
        IndexMain,


        App_Config,
        Site_Index,
        Site_Edit,
        Site_Extend_List,
        Site_Extend_Create,
        Site_Extend_Category_Check,

        /// <summary>
        /// 左栏导航树
        /// </summary>
        Category_LeftBar_Tree,
        Category_CreateCategory,
        Category_EditCategory,
        Category_List,

        Link_SiteLinkList,

        Link_SiteLinkEdit,

        Link_SiteLinkEdit_Navigator,
        Link_RelatedLink,


        User_Edit,
        User_SaveProfile,
        User_Index,
        User_Role,

        Archive_Create,
        Archive_Update,
        Archive_List,
        Archive_View,
        Archive_Forword,
        Comment_List,
        
        Archive_Tags,
        Archive_Search,

        /// <summary>
        /// 插件控制台
        /// </summary>
        Plugin_Dashboard,

        /// <summary>
        /// 插件迷你应用
        /// </summary>
        Plugin_MiniApps,

        /// <summary>
        /// 子站首页
        /// </summary>
        SUB_Index = 41,


        /// <summary>
        /// 默认样式表
        /// </summary>
        Css_Style = 70,

        //首页UI组件
        UI_Index_Css = 71,
        UI_Index_Custom_Js,

        //UI_Component,

        File_Manager,
        File_SelectEdit,
        File_Edit,

        Template_Setting,
        Template_Edit,
        Template_EditFile,
        Template_Manager,
    }



    /// <summary>
    /// Description of ResourceMap.
    /// </summary>
    public static class ResourceMap
    {
        private static IDictionary<ManagementPage, String> pageSrcs;
        public static string BasePath;


        //初始化资源
        private static void initialize()
        {
            if (pageSrcs == null || pageSrcs.Count == 0)
            {

                //IsOuterLink=System.IO.File.Exists(String.Concat(Cms.PyhicPath,"//frameworkadmin/lock"));

                pageSrcs = new Dictionary<ManagementPage, String>();

                String xmlPath = CmsMapping.GetManagementMappingXml();
                if (xmlPath == null) return;

                XmlDocument xd = new XmlDocument();
                xd.Load(xmlPath);

                Type type = typeof(ManagementPage);
                string _for;
                string baseDir;

                baseDir = xd.SelectSingleNode("/maps/direction").Attributes["path"].Value;
                BasePath = baseDir.StartsWith("/") ? baseDir : "/" + baseDir;
                if (BasePath.EndsWith("/")) BasePath = BasePath.Substring(0, BasePath.Length - 1);

                XmlNodeList nodes = xd.SelectNodes("/maps/group[@name='pages']/map");
                foreach (XmlNode node in nodes)
                {
                    _for = node.Attributes["for"].Value;
                    if (Enum.IsDefined(type, _for))
                    {
                        try
                        {
                            pageSrcs.Add(
                                (ManagementPage) Enum.Parse(type, _for),
                                String.Concat(baseDir,
                                    node.Attributes["to"].Value)
                                );
                        }
                        catch (ArgumentException exc)
                        {
                            throw new ArgumentException(exc.Message + ";" +( (ManagementPage)Enum.Parse(type, _for)).ToString());
                        }
                    }
                }
            }
        }

        public static string GetPageContent(ManagementPage page)
        {
            initialize();

            string cacheKey = String.Concat("$MP_", ((int)page).ToString());
            return CacheFactory.Sington.GetResult<String>(
                cacheKey,
                () =>
                {
                    string pageContent = null;
                    string pagePath = null;
                    pageSrcs.TryGetValue(page, out pagePath);

                    if (pagePath == null || pagePath.Trim() == String.Empty)
                    {
                        throw new Exception("页面不存在,PAGE:" + page.ToString());
                    }

                    pagePath = Cms.PyhicPath + pagePath;
                    pageContent = File.ReadAllText( pagePath);

                    HttpRuntime.Cache.Insert(
                        cacheKey,
                        pageContent,
                        new CacheDependency(pagePath),
                         DateTime.Now.AddHours(1),
                         TimeSpan.Zero
                        );

                    return pageContent;
                }
                );
        }

        public static string GetPageUrl(ManagementPage page)
        {
            initialize();

            string pagePath = null;
            pageSrcs.TryGetValue(page, out pagePath);
            return pagePath;
        }


        // public static 

        private static bool IsOuterLink;


        private static string GetDebugContent(string filePath)
        {
            string path = String.Concat(J6.Cms.Cms.PyhicPath, "/framework/admin/", filePath);
            if (System.IO.File.Exists(path))
            {
                return System.IO.File.ReadAllText(path);
            }
            else
            {
                throw new FileNotFoundException("", path);
            }
        }


        public static string ModuleList
        {
            get
            {
                return IsOuterLink ? GetDebugContent("archive/archivelist.html") : WebManagerResource.moduleList;
            }
        }

        public static string EditModule
        {
            get
            {
                return IsOuterLink ? GetDebugContent("archive/archivelist.html") : WebManagerResource.editModule;
            }
        }

        public static string SetProperties
        {
            get
            {
                return IsOuterLink ? GetDebugContent("archive/archivelist.html") : WebManagerResource.setProperties;
            }
        }

        public static string ArchiveTagReplace
        {
            get
            {
                return IsOuterLink ? GetDebugContent("archive/archivelist.html") : WebManagerResource.archiveTagReplace;
            }
        }

        public static string Createdatapickerproject
        {
            get
            {
                return IsOuterLink ? GetDebugContent("archive/archivelist.html") : WebManagerResource.createdatapickerproject;
            }
        }

        public static string Datapicker
        {
            get
            {
                return IsOuterLink ? GetDebugContent("archive/archivelist.html") : WebManagerResource.datapicker;
            }
        }




        public static string PageLoading
        {
            get
            {
                return IsOuterLink ? GetDebugContent("system/pageloading.html") : WebManagerResource.pageLoading;
            }
        }




        public static string Patch
        {
            get
            {
                return IsOuterLink ? GetDebugContent("system/patch.html") : WebManagerResource.patch;
            }
        }

        public static string Edittable
        {
            get
            {
                return IsOuterLink ? GetDebugContent("archive/archivelist.html") : WebManagerResource.edittable;
            }
        }

        public static string Tables
        {
            get
            {
                return IsOuterLink ? GetDebugContent("archive/archivelist.html") : WebManagerResource.tables;
            }
        }

        public static string Columns
        {
            get
            {
                return IsOuterLink ? GetDebugContent("archive/archivelist.html") : WebManagerResource.columns;
            }
        }

        public static string Editcolumn
        {
            get
            {
                return IsOuterLink ? GetDebugContent("archive/archivelist.html") : WebManagerResource.editcolumn;
            }
        }

        public static string Rows
        {
            get
            {
                return IsOuterLink ? GetDebugContent("archive/archivelist.html") : WebManagerResource.rows;
            }
        }




        public static string MemberList
        {
            get
            {
                return IsOuterLink ? GetDebugContent("user/memberList.html") : WebManagerResource.memberList;
            }
        }

        public static string OperationList
        {
            get
            {
                return IsOuterLink ? GetDebugContent("archive/archivelist.html") : WebManagerResource.operationList;
            }
        }

        public static string SetPermissions
        {
            get
            {
                return IsOuterLink ? GetDebugContent("archive/archivelist.html") : WebManagerResource.setPermissions;
            }
        }

        public static string RightText
        {
            get
            {
                return IsOuterLink ? GetDebugContent("archive/archivelist.html") : WebManagerResource.rightText;
            }
        }

        public static string ErrorText
        {
            get
            {
                return IsOuterLink ? GetDebugContent("archive/archivelist.html") : WebManagerResource.errorText;
            }
        }

        public static string Sysset_conf
        {
            get
            {
                return IsOuterLink ? GetDebugContent("archive/archivelist.html") : WebManagerResource.sysset_conf;
            }
        }





    }
}
