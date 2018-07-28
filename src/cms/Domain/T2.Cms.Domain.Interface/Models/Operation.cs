//
// Operation.cs
// create : 2011/05/25
// author : 
//    newmin(new.min@msn.com)
// 操作类
//
namespace T2.Cms.Domain.Interface.Models
{
    /// <summary>
    /// 后台管理操作
    /// </summary>
    public class Operation
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
        /// 请求路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>
        public bool Available { get; set; }
    }
}