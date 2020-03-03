/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2013/12/10
 * 时间: 11:24
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */

using System;
using System.IO;
using System.Reflection;

namespace JR.Stand.Core.Framework
{
    /// <summary>
    /// Description of MyClass.
    /// </summary>
    internal class AssemblyResolve
    {
        /// <summary>
        /// 解决依赖
        /// </summary>
        /// <param name="domain">AppDomain</param>
        /// <param name="directory">目录</param>
        /// <param name="fileExtension">文件扩展名</param>
        /// <param name="aes">是否AES解密</param>
        public static void Resolve(AppDomain domain, string directory, string fileExtension, bool aes)
        {
            // WHY NOT WORK
            //http://stackoverflow.com/questions/9734657/why-assemblyresolve-not-working
            //

            domain.AssemblyResolve += delegate(object sender, ResolveEventArgs args)
            {
                string filePath = String.Concat(directory, directory.EndsWith("/") ? "" : "/", args.Name.Split(',')[0],
                    fileExtension);
                if (!File.Exists(filePath))
                {
                    return null;
                }
                else
                {
                    return Assembly.Load(File.ReadAllBytes(filePath));
                }
            };
        }

        /// <summary>
        /// 默认解析.dll
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="directory"></param>
        public static void Resolve(AppDomain domain, string directory)
        {
            Resolve(domain, directory, ".dll", false);
        }

        /// <summary>
        /// 默认从lib目录下解析.dll
        /// </summary>
        /// <param name="domain"></param>
        public static void Resolve(AppDomain domain)
        {
            Resolve(domain, domain.BaseDirectory + "lib/", ".dll", false);
        }
    }
}