using System;
namespace JR.Cms.DataTransfer
{
    public class LoginResultDto:MarshalByRefObject
    {
        /// <summary>
        ///  用户编号
        /// </summary>
        public Int32 Uid { get; set; }

        /// <summary>
        /// 结果
        /// </summary>
        public Int32 Tag { get; set; }
    }
}
