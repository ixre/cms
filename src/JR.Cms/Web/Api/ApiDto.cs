using System;

namespace JR.Cms.Web.Api
{
    /// <summary>
    /// 申请令牌
    /// </summary>
    public struct RequestAccessTokenDto
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username;
        /// <summary>
        /// 密码
        /// </summary>
        public string Password;
        /// <summary>
        /// 令牌有效时间
        /// </summary>
        public int Expires;
    }

    /// <summary>
    /// 令牌
    /// </summary>
    public struct AccessTokenDataDto
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int Code;
        /// <summary>
        /// 错误消息
        /// </summary>
        public string Message;
        /// <summary>
        /// 令牌
        /// </summary>
        public string AccessToken;
        /// <summary>
        /// 过期时间
        /// </summary>
        public long ExpiresTime;
    }

    /// <summary>
    /// 上传结果DTO
    /// </summary>
    public struct UploadResultDto
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int Code; 
        /// <summary>
        /// 错误消息
        /// </summary>
        public string Message;
        /// <summary>
        /// 文件地址
        /// </summary>
        public string Url;
    }

    /// <summary>
    /// 上传的文档
    /// </summary>
    public struct PostedArchiveDto
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title;
        /// <summary>
        /// 内容
        /// </summary>
        public string Content;
        /// <summary>
        /// 缩略图
        /// </summary>
        public string Thumbnail;
    }

    /// <summary>
    /// 文档提交结果
    /// </summary>
    public struct ArchivePostResultDto
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int Code; 
        /// <summary>
        /// 错误消息
        /// </summary>
        public string Message;
        /// <summary>
        /// 文件地址
        /// </summary>
        public string Url; 
    }
}