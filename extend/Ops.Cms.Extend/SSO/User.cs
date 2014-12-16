using System;

namespace Ops.Cms.Extend.SSO
{
    /// <summary>
    /// 用户信息
    /// </summary>
    [Obsolete]
    internal struct User
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 人员编号
        /// </summary>
        public int PersonId { get; set; }

    }
}
