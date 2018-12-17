using System;
using J6.Json;

//
// 2012-10-01 添加文档扩展属性
// 2013-06-09 14:00 newmin [+]: Thumbnail
//


namespace Spc.Models
{
    /// <summary>
    /// 文档
    /// </summary>
    public sealed class Archive
    {
    	/// <summary>
    	/// 文档自增编号
    	/// </summary>
    	public int AID{get;set;}
    	
        /// <summary>
        /// 编号,一个16位Md5字符串
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 栏目编号
        /// </summary>
        public int Cid { get; set; }

        /// <summary>
        /// 文章别名
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string Flags { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        public string Author { get; set; }


        /// <summary>
        /// 来源
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 大纲,导读
        /// </summary>
        public string Outline { get; set; }

        /// <summary>
        /// 文档内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 缩略图
        /// </summary>
        public string Thumbnail { get; set; }

        /// <summary>
        /// 标签（关键词）
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// 支持数
        /// </summary>
        public int Agree { get; set; }

        /// <summary>
        /// 反对数
        /// </summary>
        public int Disagree { get; set; }

        /// <summary>
        /// 显示次数
        /// </summary>
        public int ViewCount { get; set; }


        //创建时间
        public DateTime CreateDate { get; set; }

        //最后修改时间
        public DateTime LastModifyDate { get; set; }

    }
}
