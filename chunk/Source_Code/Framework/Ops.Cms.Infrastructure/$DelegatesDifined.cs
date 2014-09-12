/*
* Copyright(C) 2010-2012 OPSoft Inc
* 
* File Name	: DelegatesDifined
* Author	: Newmin (new.min@msn.com)
* Create	: 2013/0/05 9:26:29
* Description	:
*
*/

using System.IO;
namespace Ops.Cms.Infrastructure
{
    public delegate void WatchBehavior();
    //public delegate void ArchiveHandler(Archive archive);
    //public delegate string ArchiveBehavior(Category category, Archive archive);
    //public delegate void CategoryBahavior(Category category);
    // public delegate void CategoryTreeHandler(Category category,int level);

    /// <summary>
    /// 为应用程序读取配置添加处理程序
    /// </summary>
    public delegate void CmsConfigureHandler();

    /// <summary>
    /// CMS处理行为
    /// </summary>
    /// <returns></returns>
    public delegate void CmsHandler();

    /// <summary>
    /// 文件处理委托
    /// </summary>
    /// <param name="file"></param>
    public delegate void FileHandler(FileInfo file);

}
