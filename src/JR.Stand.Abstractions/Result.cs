using System;

namespace JR.Stand.Abstracts
{
    /// <summary>
    /// 结果
    /// </summary>
    [Serializable]
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
        public object Data { get; set; }

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
        public Result(int code, String msg, object data)
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
        public static Result New(int code, String msg, object data)
        {
            return new Result {ErrCode = code, ErrMsg = msg, Data = data};
        }

        public static Result Success(object data)
        {
            return new Result {ErrCode = 0, ErrMsg = "", Data = data};
        }
    }
}