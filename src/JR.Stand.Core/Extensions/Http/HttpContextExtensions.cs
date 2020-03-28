using Microsoft.AspNetCore.Http;

public static class HttpContextExtensions
{
    /// <summary>
    /// 获取参数,如果为POST请求,默认获取Form表单参数
    /// </summary>
    /// <param name="req"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetParameter(this HttpRequest req,string name)
    {
        if (req.Method == "GET") return req.Query[name];
        return req.Form.ContainsKey(name) ?req.Form[name]:req.Query[name];
    }
}