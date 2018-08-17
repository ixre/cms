//
// Copyright (C) 2007-2008 OPSoft INC,All rights reseved.
// 
// Project: OPS.OPSite.Manager
// FileName : Ajax.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/10/15 21:16:56
// Description :
//
// Get infromation of this software,please visit our site http://www.j6.cc
//
//

namespace Spc.WebManager
{
    using Spc.Resource.WebManager;

    internal class MTools:BasePage
    {
        public void ColorPicker_GET()
        {
            base.RenderTemplate(WebManagerResource.colorpicker, null);
        }
    }
}
