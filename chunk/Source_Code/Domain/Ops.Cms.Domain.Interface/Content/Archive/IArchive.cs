/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2014/2/22
 * Time: 13:19
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using Ops.Cms.Domain.Interface.Site.Extend;
using Ops.Cms.Domain.Interface.Site.Template;
using System;
using System.Collections.Generic;

namespace Ops.Cms.Domain.Interface.Content.Archive
{
	/// <summary>
    /// 编号,一个16位Md5字符串
	/// </summary>
	public interface IArchive:IBaseContent
	{

        String StrID {get;}

        /// <summary>
        /// 文章别名
        /// </summary>
        string Alias { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        string Flags { get; set; }


        /// <summary>
        /// 来源
        /// </summary>
        string Source { get; set; }

        /// <summary>
        /// 大纲,导读
        /// </summary>
        string Outline { get; set; }


        /// <summary>
        /// 缩略图
        /// </summary>
        string Thumbnail { get; set; }

        /// <summary>
        /// 获取文章中的第一张图片,如果没有则返回NULL
        /// </summary>
        //[Obsolete]
        //string FirstImageUrl { get;}


        /// <summary>
        /// 支持数
        /// </summary>
        int Agree { get; set; }

        /// <summary>
        /// 反对数
        /// </summary>
        int Disagree { get; set; }


        /// <summary>
        /// 模板绑定
        /// </summary>
        ITemplateBind Template {get; set;}

        /// <summary>
        /// 更新模板绑定
        /// </summary>
        /// <param name="templatePath"></param>
        void UpdateTemplateBind(string templatePath);

        /// <summary>
        /// 扩展数据
        /// </summary>
        IList<IExtendValue> ExtendValues { get; set; }

        int Save();
	}
}
