
/*
* Copyright(C) 2010-2013 OPSoft Inc
* 
* File Name	: BLL
* Author	: Newmin (new.min@msn.com)
* Create	: 2013/07/29 06:52
* Description	:
*
*/

namespace Spc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Spc.BLL;

    public static class BLogic
    {
        public static ArchiveBLL Archive = new ArchiveBLL();
        public static CategoryBLL Category = new CategoryBLL();
        public static CommentBLL Comment = new CommentBLL();
        public static PropertyBLL Property = new PropertyBLL();
        public static ModuleBLL Module = new ModuleBLL();
        public static LinkBLL Link = new LinkBLL();
        public static UserBLL User = new UserBLL();
        public static SiteBLL Site = new SiteBLL();
        public static TemplateBindBLL TemplateBind = new TemplateBindBLL();
    }
}
