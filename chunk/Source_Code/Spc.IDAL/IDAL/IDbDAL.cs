using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ops.Cms.IDAL
{
    /// <summary>
    /// 数据操作接口
    /// </summary>
    public interface IDbDAL
    {
        /// <summary>
        /// 关闭连接
        /// </summary>
        void CloseConn();
    }
}
