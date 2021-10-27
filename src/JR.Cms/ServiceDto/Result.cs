using System;
using System.Collections.Generic;

namespace JR.Cms.ServiceDto
{
    /// <summary>
    /// 结果
    /// </summary>
    public class Result1
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public int ErrCode { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrMsg { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public IDictionary<string, string> Data { get; set; }
    }
}