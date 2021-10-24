using System.Reflection;

namespace JR.Stand.Core.PluginKernel
{
    /// <summary>
    /// 插件宿主接口
    /// </summary>
    public interface IPluginHost
    {
        /// <summary>
        /// 连接所有插件
        /// </summary>
        /// <returns></returns>
        bool Connect();

        /// <summary>
        /// 从程序集中加载插件
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        bool LoadFromAssembly(Assembly assembly);

        /// <summary>
        /// 获取插件信息
        /// </summary>
        /// <param name="plugin"></param>
        /// <returns></returns>
        PluginPackAttribute GetAttribute(IPlugin plugin);

        /// <summary>
        /// 迭代插件集合
        /// </summary>
        /// <param name="handler"></param>
        void Iterate(PluginHandler handler);

        /// <summary>
        /// 运行所有插件
        /// </summary>
        void Run();

        /// <summary>
        /// 停用所有插件
        /// </summary>
        void Pause();

        /// <summary>
        /// 运行指定的插件
        /// </summary>
        /// <param name="pluginId"></param>
        /// <returns></returns>
        bool Run(string pluginId);

        /// <summary>
        /// 停用指定的插件
        /// </summary>
        /// <param name="pluginId"></param>
        /// <returns></returns>
        bool Pause(string pluginId);
    }
}