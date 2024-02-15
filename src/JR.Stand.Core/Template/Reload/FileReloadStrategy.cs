using System.IO.Packaging;

namespace JR.Stand.Core.Template.Reload
{
    public class FileReloadStrategy : ITemplateReloadStrategy
    {
        public bool Check(string filePath)
        {
            return false;
            //  FileInfo fi = new FileInfo(this.FilePath);
            //     long lastWriteUnix = TemplateUtility.Unix(fi.LastWriteTime);

        }
    }
}