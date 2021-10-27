

namespace JR.Stand.Core.Web
{
   /// <summary>
   /// 
   /// </summary>
   public static class WebConf
    {
        /// <summary>
        /// 是否支持GZip压缩
        /// </summary>
        public static bool Opti_SupportGZip = false;

        /// <summary>
        /// 调试模式
        /// </summary>
        public static bool Opti_Debug = false;

        /// <summary>
        /// 客户端缓存
        /// </summary>
        public static bool Opti_ClientCache { get { return Opti_ClientCacheSeconds > 0; } }

        /// <summary>
        /// 客户端缓存秒数
        /// </summary>
        public static int Opti_ClientCacheSeconds = 0;

        /// <summary>
        /// 首页缓存秒数
        /// </summary>
        public static int Opti_IndexCacheSeconds = 0;

        /// <summary>
        /// GC回收间隔(默认30分钟回收一次)
        /// </summary>
        public static int Opti_GC_Collect_Interval = 3600000 * 30;


    }
}
