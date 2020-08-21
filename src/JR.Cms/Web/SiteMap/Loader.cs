using System;
using System.Net.Http;
using System.Threading.Tasks;
using SitemapGenerator.Sitemap;

namespace JR.Cms.Web.SiteMap
{
    /// <summary>
    /// 
    /// </summary>
    public class Loader : ILoader
    {
        private static HttpClient _client;
        /// <summary>
        /// 
        /// </summary>
        public Loader()
        {
            _client = HttpClientFactory.Create();
            _client.Timeout = TimeSpan.FromSeconds(300);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<string> Get(string url)
        {
            HttpResponseMessage resp = await _client.GetAsync(url);
            return await resp.Content.ReadAsStringAsync();
        }
    }
}
