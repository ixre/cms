using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace JR.Stand.Abstracts.Web
{
    public interface ICompatibleResponse
    {
        Task WriteAsync(string content);
        void AddHeader(string key, string value);
        void StatusCode(int status);
        void AppendCookie(string key, string value, CookieOptions opt);
        void DeleteCookie(string key, CookieOptions opt);
        void Redirect(string url, bool permanent);
        void AppendHeader(string key, string value);
        void ContentType(string contentType);
        void Write(byte[] bytes, int offset, int count);
        
        void WriteAsync(byte[] bytes);
    }
}