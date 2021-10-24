namespace JR.Stand.Core.PluginKernel
{
    /// <summary>
    /// 插件处理事件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    public delegate void PluginHandler<T>(T t, ref bool b);

    public delegate void PluginHandler<T, T1>(T t, T1 t1, ref bool b);

    public delegate void PluginHandler<T, T1, T2>(T t, T1 t1, T2 t2, ref bool b);

    public delegate void PluginHandler<T, T1, T2, T3>(T t, T1 t1, T2 t2, T3 t3, ref bool b);


    /// <summary>
    /// 插件处理事件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    public delegate void PluginHandler(IPlugin plugin, PluginPackAttribute info);
}