using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace SitemapGenerator.Sitemap
{
    public class SitemapDocument
    {
        public string LastMode { get; set; }
        public string Changefreq { get; set; }
        public string Priority { get; set; }
        public bool UseOpt { get; set; }
        public List<string> Urls { get; set; }
        public void Save(string path)
        {
            string doc = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";

            doc += OpenTag("urlset", "xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\"");

            if (UseOpt)
            {
                foreach (var url in Urls)
                {
                    doc += OpenTag("url");
                    doc += Tag("loc", url);
                    doc += Tag("lastmod", LastMode);
                    doc += Tag("changefreq", Changefreq);
                    doc += Tag("priority", Priority);
                    doc += CloseTag("url");
                }
            }
            else
            {
                foreach(var url in Urls)
                {
                    doc += OpenTag("url");
                    doc += Tag("loc", url);
                    doc += CloseTag("url");
                }
            }

            doc += CloseTag("urlset");

            File.WriteAllText(path,doc);
        }
        private string OpenTag(string name)
        {
            return $"<{name}>";
        }
        private string OpenTag(string name, string attr)
        {
            return $"<{name} {attr}>";
        }
        private string Tag(string name, string content)
        {
            return OpenTag(name) + content + CloseTag(name);
        }
        private string CloseTag(string name)
        {
            return $"</{name}>";
        }
    }
}
