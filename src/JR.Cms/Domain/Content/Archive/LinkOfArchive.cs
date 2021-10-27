using System;
using JR.Cms.Domain.Interface;
using JR.Cms.Domain.Interface.Content;

namespace JR.Cms.Domain.Content.Archive
{
    internal class LinkOfArchive : IContentLink
    {
        public LinkOfArchive(int id, string contentType, int contentId, int relatedSiteId, int relatedIndent,
            int relatedContentId,
            bool enabled)
        {
            Id = id;
            RelatedContentId = relatedContentId;
            ContentType = contentType;
            RelatedIndent = relatedIndent;
            RelatedSiteId = relatedSiteId;
            ContentId = contentId;
            Enabled = enabled;
        }


        public int RelatedIndent { get; set; }

        public int RelatedContentId { get; set; }

        public string ContentType { get; set; }

        public int Id { get; set; }


        public int ContentId { get; set; }

        public bool Enabled { get; set; }

        public string ToHtml()
        {
            //todo:???
            throw new Exception();
            //            return String.Format(@"<span class=""content_link"">
            //                                    <span class=""content_link_name"">{0}</span>
            //                                    <a href=""{1}"" title=""{2}"">{2}</a>
            //                                   </span>",
            //                                            this.LinkName,
            //                                            this.LinkTitle,
            //                                            this.LinkUri);
        }


        public bool Equal(IValueObject that)
        {
            var link = that as IContentLink;
            if (link != null)
                if (link.ContentId == ContentId
                    && link.RelatedContentId == RelatedContentId
                )
                    return true;
            return false;
        }


        public int RelatedSiteId { get; set; }
    }
}