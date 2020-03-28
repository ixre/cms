namespace JR.Stand.Core.PluginKernel
{
    public interface IPluginHandleProxy<T>
    {
        /// <summary>
        /// 处理请求,如在Web中可用来处理Get请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="pluginWorkIndent"></param>
        /// <param name="handleResult"></param>
        void HandleRequest(T context,
            string pluginWorkIndent,
            ref bool handleResult);

        /// <summary>
        /// 使用指定的类型处理请求,如果action不正确则抛出异常
        /// </summary>
        /// <typeparam name="HandleClass"></typeparam>
        /// <param name="t"></param>
        /// <param name="context"></param>
        /// <param name="action"></param>
        /// <returns>返回请求的结果,这通常返回String</returns>
        object HandleRequestUse<HandleClass>(
            HandleClass t,
            T context,
            string action);

        /// <summary>
        /// 注册处理事件
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="reqHandler"></param>
        /// <returns></returns>
        bool Register(IPlugin plugin, PluginHandler<T> reqHandler);
    }
}