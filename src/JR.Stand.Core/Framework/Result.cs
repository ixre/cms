using System;
using System.Collections.Generic;

namespace JR.Stand.Core.Framework
{
    /// <summary>
    /// 消息
    /// </summary>
    public class Result
    {
        /// <summary>
        /// 错误码,0表示成功
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

        /// <summary>
        /// 创建新的消息
        /// </summary>
        public Result()
        {

        }
        /// <summary>
        /// 创建新的消息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        public Result(int code, String msg, IDictionary<String, String> data)
        {
            this.ErrCode = code;
            this.ErrMsg = msg;
            this.Data = data;
        }
        /// <summary>
        /// 创建新的消息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        public static Result New(int code, String msg, IDictionary<String, String> data)
        {
            Result m = new Result();
            m.ErrCode = code;
            m.ErrMsg = msg;
            m.Data = data;
            return m;
        }
    }
}
