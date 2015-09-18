using System;
using J6.Cms.Domain.Interface;
using J6.Cms.Domain.Interface.Content;

namespace J6.Cms.Domain.Implement.Content.Archive
{
    internal class LinkOfArchive : IContentLink
    {
        public LinkOfArchive(int id, string contentType, int contentId, int relatedIndent, int relatedContentId,
            bool enabled)
        {
            this.Id = id;
            this.RelatedContentId = relatedContentId;
            this.ContentType = contentType;
            this.RelatedIndent = relatedIndent;
            this.ContentId = contentId;
            this.Enabled = enabled;
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
            IContentLink link = that as IContentLink;
            if (link != null)
            {
                if (link.ContentId == this.ContentId
                    && link.RelatedContentId == this.RelatedContentId
                    )
                    return true;
            }
            return false;
        }
    }
}
