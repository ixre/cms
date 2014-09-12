using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ops.Cms.Domain.Interface.User
{
    /// <summary>
    /// 角色
    /// </summary>
    public interface IRole
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        string Name { set; get; }

        /// <summary>
        /// 关联编号(可由不同归属的多个相同的角色)
        /// </summary>
        long RelationID { get; set; }

        /// <summary>
        /// 角色所属者编号（如门店编号)
        /// </summary>
        long OwnerID { get; set; }
    }
}
