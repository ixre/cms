using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SitemapGenerator.Sitemap;

namespace JR.Cms.Web.SiteMap
{
    /// <summary>
    /// 站点地图包装器
    /// </summary>
    public class Sitemapper
    {
        private readonly SitemapDocument _document;
        public string BaseUrl { get; set; }
        public string Domain { get; set; }
        public bool Exclude { get; set; }
        private ILoader _loader = new Loader();
        public delegate void Info();
        public Info Notify;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="basePath"></param>
        public Sitemapper(string domain, string basePath)
        {
            Domain = domain;
            BaseUrl = domain + basePath;
            Exclude = true;
            _document = new SitemapDocument();
        }
        public async Task GenerateSitemap(String path)
        {
            List<string> newUrls = new List<string>();
            List<string> visited = new List<string>();
            _document.Urls = visited;
            newUrls.Add(BaseUrl);
            do
            {
                List<string> hrefs=new List<string>();
                foreach (var url in newUrls)
                {
                    try
                    {
                        string text = await _loader.Get(url);
                        if (string.IsNullOrEmpty(text)) continue;
                        visited.Add(url);
                        Notify?.Invoke();
                        List<string> meta = Parser.GetAHrefs(text).Distinct().ToList();
                        meta = Parser.Normalize(Domain, url, meta);
                        if (Exclude)
                        {
                            meta = meta.Select(u => u.Contains('?') ? u.Split('?')[0] : u).ToList();
                        }
                        hrefs.AddRange(meta);
                        hrefs = hrefs.Distinct().ToList();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ cms][ warning]: resolve site map url failed! url={url} message={ex.Message}");
                    }
                }
                newUrls = hrefs.Except(visited).ToList();
            } while (newUrls.Count != 0);
            _document.Save(path);
        }
    }
}
