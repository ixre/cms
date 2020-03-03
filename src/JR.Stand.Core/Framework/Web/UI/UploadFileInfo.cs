namespace JR.Stand.Core.Framework.Web.UI
{
    /// <summary>
    /// 上传文件信息
    /// </summary>
    public struct UploadFileInfo
    {
        /// <summary>
        /// 文件编号
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 保存路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 文件长度
        /// </summary>
        public int ContentLength { get; set; }

        /// <summary>
        /// 已上传长度
        /// </summary>
        public int UploadedLength { get; set; }
    }
}