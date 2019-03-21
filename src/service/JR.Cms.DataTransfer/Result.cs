using System;
using System.Collections.Generic;

namespace JR.Cms.DataTransfer
{
    /// <summary>
    /// 结果
    /// </summary>
    public class Result
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public int ErrCode { get; set; }
        /// <summary>
        /// 错误消息
        /// </summary>
        public String ErrMsg { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public IDictionary<String, String> Data { get; set; }
    }
}
