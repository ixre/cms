using Newtonsoft.Json;

namespace JR.Cms.Web.Util
{
    /// <summary>
    /// Json序列化
    /// </summary>
    internal static class JsonSerializer
    {
        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}