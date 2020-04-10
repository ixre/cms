//
// Copyright 2011 @ S1N1.COM,All right reserved.
// Name:ITemplate.cs
// Author:newmin
// Create:2011/06/05
//

namespace JR.Stand.Core.Template.Impl
{
    public interface ITemplate
    {
        /// <summary>
        /// 读取模板
        /// </summary>
        /// <param name="templateID"></param>
        /// <returns></returns>
        string Read(string templateID);

        /// <summary>
        /// 压缩模板
        /// </summary>
        /// <param name="templateID"></param>
        void Compress(string templateID);

        /// <summary>
        /// 压缩Html
        /// </summary>
        /// <param name="templateID"></param>
        string CompressHtml(string html);

        /// <summary>
        /// 根据模板路径获取模板的ID
        /// 计算规则:小写文件名的16位md5
        /// </summary>
        /// <param name="templateFilePath"></param>
        /// <returns></returns>
        string GetTemplateID(string filePath);
    }
}