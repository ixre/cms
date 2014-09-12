using Ops.Cms.Domain.Interface.Site.Link;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ops.Cms.DataTransfer
{
    public struct SiteLinkDto
    {
        public int Pid
        {
            get;
            set;
        }

        public SiteLinkType Type
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }

        public string Uri
        {
            get;
            set;
        }

        public string ImgUrl
        {
            get;
            set;
        }

        public string Target
        {
            get;
            set;
        }

        public int OrderIndex
        {
            get;
            set;
        }

        public bool Visible
        {
            get;
            set;
        }

        public string Bind
        {
            get;
            set;
        }

        public int ID
        {
            get;
            set;
        }

        public static SiteLinkDto ConvertFrom(ISiteLink link)
        {
            return new SiteLinkDto
            {
                Bind = link.Bind,
                ID = link.ID,
                ImgUrl = link.ImgUrl,
                OrderIndex = link.OrderIndex,
                Pid = link.Pid,
                Target = link.Target,
                Text = link.Text,
                Type = link.Type,
                Uri = link.Uri,
                Visible = link.Visible
            };
        }
    }
}
