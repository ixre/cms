using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ops;
using J6.Cms;

public class QtEduController:J6.Cms.Web.Mvc.CmsController
{
    private static SettingFile file = new SettingFile(AppDomain.CurrentDomain.BaseDirectory + "config/label.conf");
    public static void InitRoute(RouteCollection rt)
    {
        rt.MapRoute("SubSchool",
            "{area}/",
            new { controller = "QtEdu", action = "SchoolIndex" },
            new { area = "^[a-zA-z]{2}$" }
            );
    }

    public void SchoolIndex(string area)
    {
        if (file.Contains(area))
        {
            string html=PageUtility.Require("/default/school", new
            {
                name=file[area],
                area=area
            });

            base.Render(html);
        }
        else
        {
           base.Category(area);
        }
    }

}

