namespace JR.Cms.Domain.Interface.Models
{
    /// <summary>
    /// 栏目类型
    /// </summary>
    public class ContentModule
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 显示模板
        /// </summary>
        public string ViewTemplate { get; set; }

        /// <summary>
        /// 是否锁定
        /// </summary>
        public bool IsLock { get; set; }
    }
}