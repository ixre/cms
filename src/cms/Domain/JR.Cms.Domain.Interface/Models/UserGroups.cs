namespace JR.Cms.Domain.Interface.Models
{
    public enum UserGroups:int
    {
        /// <summary>
        /// 超级管理员(站长)
        /// </summary>
        Master=1,
        /// <summary>
        /// 管理员
        /// </summary>
        Administrator=2,
        /// <summary>
        /// 编辑
        /// </summary>
        Editor=3,
        /// <summary>
        /// 会员
        /// </summary>
        Member,
        /// <summary>
        /// 游客
        /// </summary>
        Guest
    }
}