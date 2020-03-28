/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2013/12/8
 * Time: 20:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace JR.Stand.Core.Framework.Interface
{
    /// <summary>
    /// Description of ICallable.
    /// </summary>
    public interface ICallable
    {
        /// <summary>
        /// 获取可调用的对象
        /// </summary>
        /// <returns></returns>
        Object GetCalledObject();

        /// <summary>
        /// 调用方法
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Object Call(string method, params object[] parameters);
    }
}