
namespace J6.Cms.Conf
{
    public class CmsVariables
    {
        
        /// <summary>
    	/// 版本号
    	/// </summary>
        public const string VERSION="1.3.33";

        /// <summary>
        /// 框架目录
        /// </summary>
        public const string FRAMEWORK_PATH = "public/";

        /// <summary>
        /// 临时目录
        /// </summary>
        public const string TEMP_PATH = "tmp/";

        /// <summary>
        /// 插件目录
        /// </summary>
        public const string PLUGIN_PATH = "plugins/";


        /// <summary>
        /// 插件元数据
        /// </summary>
        public const string PLUGIN_META_FILE = "tmp/metadata.xml";

        /// <summary>
        /// 插件默认图标
        /// </summary>
        public const string PLUGIN_DEFAULT_ICON = "public/assets/images/app_icon.png";

        /// <summary>
        /// 资源目录
        /// </summary>
        public const string RESOURCE_PATH = "resources/";


        /// <summary>
        /// 配置目录 
        /// </summary>
        public const string SITE_CONF_PATH = "config/";

        /// <summary>
        /// 站点配置目录 
        /// </summary>
        public const string SITE_CONF_PRE_PATH = "config/site_";

        /// <summary>
        /// 缓存
        /// </summary>
        public const string FRAMEWORK_CACHE_PATH = "public/cache/";

        /// <summary>
        /// 程序集目录
        /// </summary>
        public const string FRAMEWORK_ASSEMBLY_PATH = "public/assemblies/";

        /// <summary>
        /// 资源文件目录
        /// </summary>
        public const string FRAMEWORK_ASSETS_PATH = "public/assets/";


        /// <summary>
        /// 文档无图片
        /// </summary>
        public const string FRAMEWORK_ARCHIVE_NoPhoto = "public/assets/images/no_photo.gif";

        /// <summary>
        /// 本地化文件
        /// </summary>
        public static string SITE_LOCALE_PATH = "config/locale.db";

        /// <summary>
        /// 默认的控制器前缀
        /// </summary>
        public static string DEFAULT_CONTROLLER_NAME ="cms.do";

        /// <summary>
        /// 模板目录
        /// </summary>
        public const string TEMPLATE_PATH = "templates/";


        public const string Archive_ThumbPrefix = "thumb";

        internal const string FileEncodeHeader = "J6CMS";
        internal const string FileEncodeToken = "j6cmspowbyk3fnet";
    }

    public static class CmsCharMap
    {
        public const string Dot = "・";
        public static char Connect = '-';
    }
}
