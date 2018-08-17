
namespace T2.Cms.Extend.SSO
{
    /// <summary>
    /// 会话数据集合
    /// </summary>
    public interface ISessionSet
    {
        /// <summary>
        /// 更新会话信息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>返回之前的会话信息</returns>
        string Put(string key, string value);

        /// <summary>
        /// 删除会话信息
        /// </summary>
        /// <param name="key"></param>
        void Delete(string key);

        /// <summary>
        /// 获取会话信息
        /// </summary>
        /// <param name="key"></param>

        string Get(string key);
    }
}
