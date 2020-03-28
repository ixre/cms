

using System;

namespace JR.Stand.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class VersionSet
    {
        private string _versionStr;

        /// <summary>
        /// 主要版本号
        /// </summary>
        public string MajorVersion = "0";

        /// <summary>
        /// 次要版本号
        /// </summary>
        public string MinorVersion = "0";

        /// <summary>
        /// 修订版本号
        /// </summary>
        public string BuildNumber = "1";


        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <returns></returns>
        public string GetVersion()
        {
            return this._versionStr ??
                   (this._versionStr =
                       String.Format("{0}.{1}.{2}", this.MajorVersion, this.MinorVersion, this.BuildNumber));


        }
    }
}
