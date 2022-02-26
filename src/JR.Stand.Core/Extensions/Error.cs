using System;

namespace JR.Stand.Core.Extensions
{
    /// <summary>
    /// 错误
    /// </summary>
    [Serializable]
    public class Error:Exception
    {
        public Error(string message)
        {
            this.Message = message;
        }

        public override string Message { get; }
    }
}