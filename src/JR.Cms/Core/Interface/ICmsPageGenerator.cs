/*
* Copyright(C) 2010-2013 OPS.CC
* 
* File Name	: ICmsPageGenerator
* author_id	: Newmin (new.min@msn.com)
* Create	: 2013/04/10 05:51:54
* Description	:
*
*/

using JR.Cms.ServiceDto;

namespace JR.Cms.Core.Interface
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICmsPageGenerator
    {
        /// <summary>
        /// 格式化模板路径
        /// </summary>
        /// <param name="tplPath"></param>
        /// <returns></returns>
        string FormatTemplatePath(string tplPath);

        /// <summary>
        /// 根据模板路径获取模板ID
        /// </summary>
        /// <param name="tplPath"></param>
        /// <returns></returns>
        string GetTemplateId(string tplPath);

        #region page

        /// <summary>
        /// 返回首页
        /// </summary>
        /// <returns></returns>
        string GetIndex();

        /// <summary>
        /// 栏目页
        /// </summary>
        /// <param name="category"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        string GetCategory(CategoryDto category, int pageIndex);

        /// <summary>
        /// 文档
        /// </summary>
        /// <param name="category"></param>
        /// <param name="archive"></param>
        /// <returns></returns>
        string GetArchive(ArchiveDto archive);

        /// <summary>
        /// 搜索
        /// </summary>
        /// <returns></returns>
        string GetSearch(string catPath,string key);


        /// <summary>
        /// 标签文档页
        /// </summary>
        /// <returns></returns>
        string GetTagArchive(string key);

        #endregion
    }
}