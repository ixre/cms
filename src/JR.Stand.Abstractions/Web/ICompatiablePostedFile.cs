using System.IO;

namespace JR.Stand.Abstracts.Web
{
    public interface ICompatiblePostedFile
    {
        string GetFileName();
        
        /// <summary>
        /// 保存文件到本地
        /// </summary>
        /// <param name="path"></param>
        void Save(string path);
        void CopyToAsync(FileStream fs);
        long GetLength();
        Stream OpenReadStream();
        string GetContentType();
    }
}