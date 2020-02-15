/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2014/2/22
 * Time: 13:19
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using JR.Cms.Domain.Interface.Site.Category;
using JR.Cms.Domain.Interface.Site.Extend;
using JR.Cms.Domain.Interface.Site.Template;
using JR.Cms.Infrastructure;
using JR.Cms.Models;

namespace JR.Cms.Domain.Interface.Content.Archive
{
	/// <summary>
    /// 编号,一个16位Md5字符串
	/// </summary>
	public interface IArchive:IBaseContent
	{
        /// <summary>
        /// 获取文档
        /// </summary>
        /// <returns></returns>
        CmsArchiveEntity Get();

        /// <summary>
        /// 设置文档值
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        Error Set(CmsArchiveEntity src);

        /// <summary>
        /// 模板绑定
        /// </summary>
        TemplateBind Template {get; set;}

        /// <summary>
        /// 设置模板
        /// </summary>
        /// <param name="templatePath"></param>
        void SetTemplatePath(string templatePath);

        /// <summary>
        /// 扩展数据
        /// </summary>
        IList<IExtendValue> GetExtendValues();


        /// <summary>
        /// 设置扩展数据
        /// </summary>
        /// <param name="extendValues"></param>
        /// <returns></returns>
        Error SetExtendValue(IList<IExtendValue> extendValues);

        /// <summary>
        /// 获取关联的分类
        /// </summary>
        ICategory Category { get; }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        Error Save();
    }
}
