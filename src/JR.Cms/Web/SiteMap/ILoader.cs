using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SitemapGenerator.Sitemap
{
    public interface ILoader
    {
        Task<string> Get(string url);
    }
}
