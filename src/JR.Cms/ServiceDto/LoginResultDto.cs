using System;

namespace JR.Cms.ServiceDto
{
    public class LoginResultDto : MarshalByRefObject
    {
        /// <summary>
        ///  用户编号
        /// </summary>
        public int Uid { get; set; }

        /// <summary>
        /// 结果
        /// </summary>
        public int Tag { get; set; }
    }
}