using System.Threading.Tasks;

namespace JR.Cms.Web.SiteMap
{
    /// <summary>
    /// 
    /// </summary>
    public interface ILoader
    {
        Task<string> Get(string url);
    }
}
