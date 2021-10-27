namespace JR.Stand.Core.PluginKernel
{
    /// <summary>
    /// 插件接口
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// 连接插件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        PluginConnectionResult Connect(IPluginHost app);


        /// <summary>
        /// 安装
        /// </summary>
        bool Install();

        /// <summary>
        /// 卸载
        /// </summary>
        /// <returns></returns>
        bool Uninstall();

        /// <summary>
        /// 运行
        /// </summary>
        void Run();

        /// <summary>
        /// 暂停运行
        /// </summary>
        void Pause();

        /// <summary>
        /// 返回插件操作的消息
        /// </summary>
        /// <returns></returns>
        string GetMessage();

        /// <summary>
        /// 获取插件的属性
        /// </summary>
        /// <returns></returns>
        PluginPackAttribute GetAttribute();

        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="line"></param>
        void Logln(string line);

        /// <summary>
        /// 开放调用
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        object Call(string method, params object[] parameters);
    }
}