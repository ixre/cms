
/*
* Copyright(C) 2010-2013 OPSoft Inc
* 
* File Name	: Templates
* Author	: Newmin (new.min@msn.com)
* Create	: 2013/07/29 06:52
* Description	:
*
*/


namespace J6.Cms.Web.Template
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using J6.Template;
    using J6.Cms;
    using J6.Cms.Cache;
    using J6.Cms.Models;

    public partial class Templates
    {


        #region 站点标签



        protected string Title()
        {
            string pageSign = Cms.Context.Items["pageSign"] as string;
            switch (pageSign)
            {
                case "category":
                    Category category = cbll.Get(a => String.Compare(a.Tag, Cms.Context.Items["category.tag"].ToString(), true) == 0);
                    if (String.IsNullOrEmpty(category.PageTitle))
                    {
                        string pageStr = "";
                        string pageIndex = Cms.Context.Items["page.index"].ToString();
                        if (pageIndex != "1" && pageIndex != "0")
                        {
                            pageStr = "(第" + pageIndex + "页)";
                        }
                        return String.Format("{0}{1}_{2}", category.Name, pageStr, this.site.SeoTitle);
                    }
                    else
                    {
                        return category.PageTitle;
                    }
            }
            return "";
        }

        protected string Keywords()
        {
            string pageSign = Cms.Context.Items["pageSign"] as string;
            switch (pageSign)
            {
                case "category":
                    Category category = cbll.Get(a => String.Compare(a.Tag, Cms.Context.Items["category.tag"].ToString(), true) == 0);
                    if (!String.IsNullOrEmpty(category.PageTitle))
                    {
                        return category.Keywords;
                    }
                    return String.Empty;
            }
            return "";
        }

        protected string Description()
        {
            string pageSign = Cms.Context.Items["pageSign"] as string;
            switch (pageSign)
            {
                case "category":
                    Category category = cbll.Get(a => String.Compare(a.Tag, Cms.Context.Items["category.tag"].ToString(), true) == 0);
                    if (!String.IsNullOrEmpty(category.PageTitle))
                    {
                        return category.Description;
                    }
                    return String.Empty;
            }
            return "";
        }

        #endregion
    }
}
