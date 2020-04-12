using System.Threading.Tasks;

namespace JR.Stand.Abstracts.Web
{
    public interface ICompatibleResponse
    {
        Task WriteAsync(string content);
        void AddHeader(string key, string value);
        void StatusCode(int status);
        void AppendCookie(string key, string value, HttpCookieOptions opt);
        void DeleteCookie(string key, HttpCookieOptions opt);
        void Redirect(string url, bool permanent);
        void AppendHeader(string key, string value);
        void ContentType(string contentType);
        void Write(byte[] bytes, int offset, int count);
        
        void WriteAsync(byte[] bytes);
    }
}