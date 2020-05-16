namespace JR.Stand.Abstracts
{
    public interface IServeHosting
    {
        /// <summary>
        /// 停止服务,AspNet和IIS将会自动重启,其他启动方式需要手动重启
        /// </summary>
         void Stop();

        /// <summary>
        /// 获取DI实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetService<T>();

        /// <summary>
        /// 获取应用程序目录
        /// </summary>
        /// <returns></returns>
        string BaseDirectory();

        /// <summary>
        /// 是否为Linux系统
        /// </summary>
        /// <returns></returns>
        bool IsLinuxOS();
    }
}