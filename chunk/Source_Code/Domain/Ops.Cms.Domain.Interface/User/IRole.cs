namespace AtNet.Cms.Domain.Interface.User
{
    /// <summary>
    /// 角色
    /// </summary>
    public interface IRole:IDomain<int>
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        string Name { set; get; }

        /// <summary>
        /// 系统应用的编号
        /// </summary>
        long AppId { get; set; }

        /// <summary>
        /// 标志
        /// </summary>
        int Flag { get; set; }

        /// <summary>
        /// 权限标志
        /// </summary>
        int RightFlag { get; set; }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        int Save();
    }
}
