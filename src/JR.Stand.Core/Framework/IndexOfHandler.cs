using System;

namespace JR.Stand.Core.Framework
{
    /// <summary>
    /// 获取索引位置的委托
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <returns></returns>
    public delegate Int32 IndexOfHandler<T>(T t);
}