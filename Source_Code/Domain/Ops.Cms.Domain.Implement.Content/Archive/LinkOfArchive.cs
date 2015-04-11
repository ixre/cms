using System;
using AtNet.Cms.Domain.Interface;
using AtNet.Cms.Domain.Interface.Common;
using AtNet.Cms.Domain.Interface.Content;

namespace AtNet.Cms.Domain.Implement.Content.Archive
{
    internal class LinkOfArchive : IContentLink
    {
        public LinkOfArchive(int linkId,string name, string title, string uri, bool enabled)
        {
            this.LinkId = linkId;
            this.LinkName = name;
            this.LinkTitle = title;
            this.LinkUri = uri;
            this.Enabled = enabled;
        }

        public int LinkId
        {
            get;
            set;
        }

        public int ContentId
        {
            get;
            set;
        }

        public string LinkName
        {
            get;
            set;
        }

        public string LinkTitle
        {
            get;
            set;
        }

        public string LinkUri
        {
            get;
            set;
        }

        public bool Enabled
        {
            get;
            set;
        }

        public string ToHtml()
        {
            return String.Format(@"<span class=""content_link"">
                                    <span class=""content_link_name"">{0}</span>
                                    <a href=""{1}"" title=""{2}"">{2}</a>
                                   </span>",
                                            this.LinkName,
                                            this.LinkTitle,
                                            this.LinkUri);
        }


        public bool Equal(IValueObject that)
        {
            ILink link = that as ILink;
            if (link != null)
            {
                if (link.LinkName == this.LinkName
                    && link.LinkTitle == this.LinkTitle
                    )
                    return true;
            }
            return false;
        }
    }
}
