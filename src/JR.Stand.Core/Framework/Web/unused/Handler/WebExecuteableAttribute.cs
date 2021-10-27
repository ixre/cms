/*
 * name     : WebExecuteAttribute
 * author   : OPS newmin
 * Data     : 09/20 2010
 * note     : 添加此特性才能被ExecuteHandler用反射的方式调用
 */

using System;

namespace JR.DevFw.Framework.Web.unused.Handler
{
    /// <summary>
    /// 可通过页面请求执行类的操作
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class WebExecuteableAttribute : Attribute
    {
    }
}