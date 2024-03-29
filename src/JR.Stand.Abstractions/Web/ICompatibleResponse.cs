using System.Threading.Tasks;

namespace JR.Stand.Abstracts.Web
{
    public interface ICompatibleResponse
    {
        void AddHeader(string key, string value);
        void StatusCode(int status);
        void AppendCookie(string key, string value, HttpCookieOptions opt);
        void DeleteCookie(string key, HttpCookieOptions opt);
        void Redirect(string url, bool permanent);
        void AppendHeader(string key, string value);
        void ContentType(string contentType);
        void Write(byte[] bytes, int offset, int count);
        Task WriteAsync(byte[] bytes);
        Task WriteAsync(string content);
    }
}