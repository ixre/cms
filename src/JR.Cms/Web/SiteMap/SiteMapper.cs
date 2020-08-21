using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JR.Cms.Web.SiteMap
{
    /// <summary>
    /// 站点地图包装器
    /// </summary>
    public class SiteMapper
    {
        private readonly SitemapDocument _document;
        /// <summary>
        /// 
        /// </summary>
        private string BaseUrl { get; }
        /// <summary>
        /// 
        /// </summary>
        private string Domain { get; }
        /// <summary>
        /// 
        /// </summary>
        public bool Exclude { get; set; }
        private readonly ILoader _loader = new Loader();
        public delegate void Info();
        /// <summary>
        /// 
        /// </summary>
        private Info Notify;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="basePath"></param>
        public SiteMapper(string domain, string basePath)
        {
            Domain = domain;
            BaseUrl = domain + basePath;
            Exclude = true;
            _document = new SitemapDocument
            {
                UseOpt = true,
                Priority = "",
                Changefreq = "weekly", 
                LastMode = $"{DateTime.Now:yyyy-MM-dd}"
            };
            //_document.Priority = "0.5";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task GenerateSiteMap(String path)
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
