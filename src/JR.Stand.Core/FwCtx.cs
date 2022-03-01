using System;
using JR.Stand.Core.Framework;
using JR.Stand.Core.Utils;

namespace JR.Stand.Core
{
    /// <summary>
    /// 开发框架上下文对象
    /// </summary>
    public static class FwCtx
    {
        private static string _physicalPath;
        private static Variable _variables;
        private static VersionSet _version;
        private static int _platformId;

        /// <summary>
        /// 物理路径
        /// </summary>
        public static string PhysicalPath
        {
            get
            {
                if (_physicalPath == null)
                {
                    _physicalPath = EnvUtil.GetBaseDirectory();
                }

                return _physicalPath;
            }
        }

        /// <summary>
        /// 变量设置
        /// </summary>
        public static Variable Variables
        {
            get
            {
                if (_variables == null)
                {
                    _variables = new Variable();
                }
                return _variables;
            }
        }

        /// <summary>
        /// 版本
        /// </summary>
        public static VersionSet Version
        {
            get { return _version ?? (_version = new VersionSet()); }
        }

        /// <summary>
        /// 是否运行在非Windows平台
        /// </summary>
        /// <returns></returns>
        public static bool Mono()
        {
            //获取平台编号
            if (_platformId == 0)
            {
                _platformId = (Int32) Environment.OSVersion.Platform;
            }

            return _platformId == 4 || _platformId == 6 || _platformId == 128;
        }

        /// <summary>
        /// 从默认的位置加载程序集
        /// </summary>
        public static void ResolveAssemblies()
        {
            InnerAppDomainResolver.Resolve(null);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyFullPath"></param>
        public static void ResolveAssembliesByPath(String assemblyFullPath)
        {
            InnerAppDomainResolver.Resolve(assemblyFullPath);
        }
    }
}
