/* *
 * name     : 服务接口
 * author   : OPS newmin
 * date     : 09/29 2010
 * */

using System;

namespace JR.DevFw.Framework.Service
{
    /// <summary>
    /// 服务接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal interface IService<T> where T : MarshalByRefObject
    {
        /// <summary>
        /// 获取服务的实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetInstance();
    }
}