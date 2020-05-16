namespace JR.Cms.ServiceDto
{
    public class RelatedLinkDto
    {
        public bool Enabled { get; set; }
        public int ContentId { get; set; }
        public string ContentType { get; set; }
        public string RelatedSiteName { get; set; }

        public int RelatedContentId { get; set; }
        public int RelatedIndent { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Thumbnail { get; set; }
        public string IndentName { get; set; }
        public int RelatedSiteId { get; set; }
    }
}