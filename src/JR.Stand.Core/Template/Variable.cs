//
// Copyright 2011 @ S1N1.COM,All right reserved.
// Name:Variable.cs
// Author:newmin
// Create:2013/09/10
//

using System;

namespace JR.Stand.Core.Template
{
    /// <summary>
    /// 变量
    /// </summary>
    public struct Variable
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public Type Type { get; set; }
    }
}