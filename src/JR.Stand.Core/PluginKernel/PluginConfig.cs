namespace JR.Stand.Core.PluginKernel
{
    /// <summary>
    /// 
    /// </summary>
    public class PluginConfig
    {
        /// <summary>
        /// 插件目录名称,以"/"结尾
        /// </summary>
        public static string PLUGIN_DIRECTORY = "plugins/";

        /// <summary>
        /// 插件临时文件存放目录,以"/"结尾。用于存放日志，临时文件等
        /// </summary>
        public static string PLUGIN_TMP_DIRECTORY = "tmp/plugin/";

        /// <summary>
        /// 插件日志是否打开,默认为关闭
        /// </summary>
        public static bool PLUGIN_LOG_OPENED = false;

        /// <summary>
        /// 插件异常日志格式,默认为：<{time}>:{message}\r\nSource:{source}\r\nAddress:{addr}\r\nStack:{stack}\r\n\r\n
        /// </summary>
        public static string PLUGIN_LOG_EXCEPT_FORMAT =
            "**{time}** [Exeption] -{message}\r\nSource:{source} - Addr:{addr}\r\nStack:{stack}\r\n\r\n";

        /// <summary>
        /// 插件文件后缀,多个后缀用","隔开
        /// 默认加载bin目录下的.so文件
        /// </summary>
        public static string PLUGIN_FILE_PARTTERN = "*.so,*.dll";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string[] GetFilePartterns()
        {
           return (PLUGIN_FILE_PARTTERN ?? "*.dll").Split(',');
        }
    }
}