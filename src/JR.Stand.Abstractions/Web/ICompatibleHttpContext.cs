using System.Diagnostics.CodeAnalysis;

namespace JR.Stand.Abstracts.Web
{
    public interface ICompatibleHttpContext
    {
        /// <summary>
        /// 获取主机服务
        /// </summary>
        IServeHosting Hosting { get; }
        
        /// <summary>
        /// 将上下文转换为原生的上下文对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        object RawContext();
        
        /// <summary>
        /// 获取请求
        /// </summary>
        ICompatibleRequest Request { get; }
        
        /// <summary>
        /// 获取响应
        /// </summary>
        ICompatibleResponse Response { get; }
        
        /// <summary>
        /// 获取会话
        /// </summary>
        ICompatibleSession Session { get; }

        /// <summary>
        /// 尝试获取项目
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGetItem<T>(string key, out T value);

        /// <summary>
        /// 获取请求路径
        /// </summary>
        /// <returns></returns>
        string RequestPath();

        /// <summary>
        /// 设置上下文项目
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SaveItem(string key, object value);

        /// <summary>
        /// 获取远程用户的IP地址
        /// </summary>
        /// <returns></returns>
        string RemoteAddress();
    }



}