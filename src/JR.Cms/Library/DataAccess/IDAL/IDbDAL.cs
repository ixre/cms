namespace JR.Cms.Library.DataAccess.IDAL
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