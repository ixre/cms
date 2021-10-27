/*
* Copyright(C) 2010-2013 OPS.CC
* 
* File Name	: Site.cs
* author_id	: Newmin (new.min@msn.com)
* Create	: 2013/05/21 19:59:54
* Description	:
*
*/

using System;
using System.IO;
using SharpCompress.Archive;
using SharpCompress.Archive.Zip;
using SharpCompress.Common;
using SharpCompress.Writer;

namespace JR.Cms
{
    /// <summary>
    /// 压缩辅助工具
    /// </summary>
    public class ZipHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="dir"></param>
        public delegate void ZipWriterHandler(IWriter writer,DirectoryInfo dir);
        /// <summary>
        /// 压缩并保存
        /// </summary>
        /// <param name="directionPath"></param>
        /// <param name="savePath"></param>
        /// <returns></returns>
        public static void ZipAndSave(string directionPath, string savePath,string dirName=null)
        {
            if (dirName == null)
            {
                ZipArchive zip = ZipArchive.Create();
                zip.AddAllFromDirectory(directionPath);
                zip.SaveTo(savePath, CompressionType.None);
                zip.Dispose();
            }
            else
            {
                using (Stream zip = File.OpenWrite(directionPath))
                using (var zipWriter = WriterFactory.Open(zip, ArchiveType.Zip, CompressionType.None))
                {
                    DirectoryInfo dir = new DirectoryInfo(directionPath);
                    Explore(dir, zipWriter, dirName, dir.FullName);
                }
            }
        }

        /// <summary>
        /// 压缩并返回字节数组
        /// </summary>
        /// <param name="directionPath"></param>
        /// <returns></returns>
        public static byte[] Compress(string directionPath, string dirName = null)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                if (dirName == null)
                {

                    ZipArchive zip = ZipArchive.Create();
                    zip.AddAllFromDirectory(directionPath);
                    zip.SaveTo(ms, CompressionType.None);
                    zip.Dispose();

                }
                else
                {
                    using (var zipWriter = WriterFactory.Open(ms, ArchiveType.Zip, CompressionType.None))
                    {
                        DirectoryInfo dir = new DirectoryInfo(directionPath);
                        Explore(dir, zipWriter, dirName,dir.FullName);
                    }
                }

                return ms.ToArray();
            }
        }


        /// <summary>
        /// 迭代目录
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="writer"></param>
        /// <param name="rootDir"></param>
        /// <param name="tplPath"></param>
        private static void Explore(DirectoryInfo dir, IWriter writer, string rootDir,string tplPath)
        {
            foreach (FileInfo file in dir.GetFiles())
            {
                if ((file.Attributes & FileAttributes.Hidden)!=FileAttributes.Hidden && !file.Name.EndsWith(".bak"))
                {
                    writer.Write(
                        String.Format("{0}/{1}", rootDir, file.FullName.Substring(tplPath.Length))
                        , file.FullName );
                }
            }

            foreach (DirectoryInfo d in dir.GetDirectories())
            {
                if ((dir.Attributes & FileAttributes.Hidden)!=FileAttributes.Hidden)
                {
                    Explore(d, writer, rootDir, tplPath);
                }
            }
        }
    }
}
