//
// Link  链接模型
// Copryright 2011 @ OPS Inc,All rights reseved !
// Create by newmin @ 2011/04/07
//

namespace JR.Cms.Domain.Interface.Models
{
    /// <summary>
    /// 链接
    /// </summary>
    public class Link
    {
        /// <summary>
        /// 编号,自动编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 父级链接
        /// </summary>
        public int Pid { get; set; }

        /// <summary>
        /// 站点编号
        /// </summary>
        public int SiteID { get; set; }

        /// <summary>
        /// 链接类型
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 链接文本
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 链接地址
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        public string ImgUrl { get; set; }

        /// <summary>
        /// 链接目标
        /// </summary>
        public string Target { get;set; }

        /// <summary>
        /// 链接顺序
        /// </summary>
        public int Index{ get; set; }

        /// <summary>
        /// 是否可见
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// 绑定信息
        /// </summary>
        public string Bind { get; set; }
    }
}