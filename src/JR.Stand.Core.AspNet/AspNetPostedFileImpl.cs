using System.IO;
using System.Threading.Tasks;
using System.Web;
using JR.Stand.Abstracts.Web;

namespace JR.Stand.Core.AspNet
{
    public class AspNetPostedFileImpl : ICompatiblePostedFile
    {
        private readonly HttpPostedFile _file;

        public AspNetPostedFileImpl(HttpPostedFile file)
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
                this._file.InputStream.CopyTo(fs);
                fs.Flush();
            }
        }

        public Task CopyToAsync(Stream fs)
        {
            return this._file.InputStream.CopyToAsync(fs);
        }

        public long GetLength()
        {
            return this._file.ContentLength;
        }

        public Stream OpenReadStream()
        {
            return this._file.InputStream;
        }

        public string GetContentType()
        {
            return this._file.ContentType;
        }
    }
}