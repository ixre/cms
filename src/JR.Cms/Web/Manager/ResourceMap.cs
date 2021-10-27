using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

/*
 * Created by SharpDevelop.
 * UserBll: newmin
 * Date: 2013/12/27
 * Time: 7:34
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

namespace JR.Cms.Web.Manager
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
        /// <summary>
        /// 本地化
        /// </summary>
        Locale, 
        Site_Index,
        Site_Edit,
        Site_Extend_List,
        Site_Extend_Create,
        Site_Extend_Category_Check,
        
        /// <summary>
        /// 站点变量
        /// </summary>
        SiteVariables,
        
        /// <summary>
        /// 左栏导航树
        /// </summary>
        Category_LeftBar_Tree,
        Category_CreateCategory,
        Category_EditCategory,
        Category_List,

        Link_List,

        Link_Edit,

        Link_Edit_Navigator,
        Link_RelatedLink,


        User_Edit,
        User_SaveProfile,
        User_Index,
        User_Role,

        Archive_Create,
        Archive_Update,
        Archive_List,
        Archive_View,
        Archive_Forward,
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
        /// 默认样式表
        /// </summary>
        Css_Style = 70,

        //首页UI组件
        UI_Index_Css = 71,

        //UI_Component,

        File_Manager,
        File_SelectEdit,
        File_Edit,

        Template_Setting,
        Template_Edit,
        Template_EditFile,
        Template_Manager,


        Assistant_Category_Clone,
        Assistant_Local_Patch,
        Assistant_Archive_Clone_Pub,
        Clear_Page
    }


    /// <summary>
    /// Description of ResourceMap.
    /// </summary>
    public static class ResourceMap
    {
        private static IDictionary<ManagementPage, string> pageSources;
        public static string BasePath;


        //初始化资源
        private static void initialize()
        {
            if (pageSources == null)
            {
                pageSources = new Dictionary<ManagementPage, string>();
                var xmlPath = Cms.BuildOEM.SelectNodeValuePath("/keys/item[@key='manager_set']");
                if (xmlPath == null) return;
                var xd = new XmlDocument();
                xd.Load(xmlPath);
                var type = typeof(ManagementPage);
                var baseDir = xd.SelectSingleNode("/keys/direction").Attributes["path"].Value;
                BasePath = baseDir.StartsWith("/") ? baseDir : "/" + baseDir;
                if (BasePath.EndsWith("/")) BasePath = BasePath.Substring(0, BasePath.Length - 1);
                var nodes = xd.SelectNodes("/keys/group[@name='pages']/item");
                foreach (XmlNode node in nodes)
                {
                    var _for = node.Attributes["key"].Value;
                    if (Enum.IsDefined(type, _for))
                        try
                        {
                            var pg = (ManagementPage) Enum.Parse(type, _for);
                            if (!pageSources.ContainsKey(pg))
                                pageSources.Add(pg, string.Concat(baseDir, node.Attributes["value"].Value));
                        }
                        catch (ArgumentException exc)
                        {
                            throw new ArgumentException(exc.Message + ";" +
                                                        ((ManagementPage) Enum.Parse(type, _for)).ToString());
                        }
                }
            }
        }

        public static string GetPageContent(ManagementPage page)
        {
            var pagePath = GetPageUrl(page);
            if (pagePath == null || pagePath.Trim() == string.Empty)
                throw new Exception("页面不存在,PAGE:" + page.ToString());
            pagePath = Cms.PhysicPath + pagePath;
            return File.ReadAllText(pagePath);
        }

        public static string GetPageUrl(ManagementPage page)
        {
            initialize();
            pageSources.TryGetValue(page, out var pagePath);
            return pagePath;
        }


        // public static 

        private static bool IsOuterLink;


        private static string GetDebugContent(string filePath)
        {
            var path = string.Concat(Cms.PhysicPath, "/public/admin/", filePath);
            if (File.Exists(path))
                return File.ReadAllText(path);
            else
                throw new FileNotFoundException("", path);
        }


        // public static string SetProperties =>GetDebugContent("archive/archive_list.html");

        /// <summary>
        /// 
        /// </summary>
        public static string ArchiveTagReplace => GetDebugContent("archive/archive_tags.html");


        /// <summary>
        /// 
        /// </summary>
        public static string EditTable => GetDebugContent("table/edit_table.html");

        /// <summary>
        /// 
        /// </summary>
        public static string Tables =>GetDebugContent("table/tables.html");

        /// <summary>
        /// 
        /// </summary>
        public static string Columns =>GetDebugContent("table/columns.html");

        /// <summary>
        /// 
        /// </summary>
        public static string EditColumn =>GetDebugContent("table/edit_column.html");

        /// <summary>
        /// 
        /// </summary>
        public static string Rows => GetDebugContent("table/rows.html");


        /// <summary>
        /// 
        /// </summary>
        public static string MemberList => GetDebugContent("user/member_list.html");


        /// <summary>
        /// 
        /// </summary>
        public static string RightText =>GetDebugContent("archive/archive_list.html");

        /// <summary>
        /// 
        /// </summary>
        public static string ErrorText =>GetDebugContent("archive/archive_list.html");

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetBoardMenu()
        {
            var path = Cms.BuildOEM.SelectNodeValuePath("/keys/item[@key='board_set']");
            return File.ReadAllText(path);
        }
    }
}