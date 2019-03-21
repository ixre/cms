<%@ Application Language="C#" %>

<script RunAt="server">

    public static void RegisterRoutes(RouteCollection routes)
    {
        routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

        //激活检测
        routes.MapRoute(
            "Activator",                                    // Route name
            "@Activator/$",                           // URL with parameters
            new { controller = "Activator", action = "Verify", id = "" }
        );
        
        routes.MapRoute(
            "Default",                                    // Route name
            "{controller}/{action}/{id}",                           // URL with parameters
            new { controller = "Home", action = "Index", id = "" },
            new { controller = "[A-Za-z]+", action = "[A-Za-z]*" }  // Parameter defaults
        );
    }

    protected void Application_Start()
    {
        RegisterRoutes(RouteTable.Routes);
        
    }
    </script>
