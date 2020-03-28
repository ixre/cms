using System.IO;
using JR.Stand.Abstracts.Web;
using Microsoft.AspNetCore.Http;

namespace JR.Stand.Core.Web
{
    public class PostedFileImpl : ICompatiblePostedFile
    {
        private readonly IFormFile _file;

        public PostedFileImpl(IFormFile file)
        {
            this._file = file;
        }

        public string GetFileName()
        {
            return this._file.FileName;
        }

        public void Save(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create | FileMode.Truncate))
            {
                this._file.CopyTo(fs);
                fs.Flush();
            }
        }

        public void CopyToAsync(FileStream fs)
        {
            this._file.CopyToAsync(fs);
        }

        public long GetLength()
        {
            return this._file.Length;
        }

        public Stream OpenReadStream()
        {
            return this._file.OpenReadStream();
        }

        public string GetContentType()
        {
            return this._file.ContentType;
        }
    }
}