<%@ Application Language="C#" %>
<%@ Import Namespace="System.Web.Routing" %>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="Ops.Cms" %>
<%@ Import Namespace="Spc.Web.Mvc" %>

<script RunAt="server">
    public static void RegisterRoutes(RouteCollection routes)
    {
        routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

        
        // ------------------------------------------------
        //     自定义路由放在这里 
        //  -----------------------------------------------

        //注册CMS路由
        Type cmsType = typeof(Spc.Web.MultLangCmsController);
        Routes.RegisterCmsRoutes(routes, "{lang}", cmsType);
        string[] langs = new string[] { "en-us", "zh-cn" };
        Lang.Set(langs);


        routes.MapRoute("HomeIndex", ""
        , new { controller = "MultLangCms", action = "Index", lang = langs[0] });

        // ------------------------------------------------
        //  注册首页路由，可以自由修改首页位置
        //  routes.MapRoute("HomeIndex", ""
        //  , new { controller = "Cms", action = "Index" });
        // ------------------------------------------------
        
        // ------------------------------------------------
        //  注册首页路由，可以自由修改首页位置
        //  routes.MapRoute("HomeIndex", ""
        //  , new { controller = "Cms", action = "Index" });
        // ------------------------------------------------
    }

    protected void Application_Start()
    {
        //初始化
        Cms.OnInit += () => { global::Spc.XmlRpc.WeblogRPCService.EnableBase64Images = false; };
        Cms.OnInit += CmsEventRegister.Init;
        Cms.Init();
        
        //注册路由
        RegisterRoutes(RouteTable.Routes);
        
        //RouteDebug.RouteDebugger.RewriteRoutesForTesting(RouteTable.Routes);
     }

    protected void Application_Error(object o, EventArgs e)
    {
        //PageUtility.RenderException(Server.GetLastError(),true);
        //Server.ClearError();
    }
    </script>
