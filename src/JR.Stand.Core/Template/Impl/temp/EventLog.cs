//
// Copyright (C) 2011 S1N1.COM,All right reserved.
// Name:EventLog.cs
// Author:newmin
// Create:2011/06/13
//

using System;
using System.Text;

namespace JR.Stand.Core.Template.Impl.temp
{
    /// <summary>
    /// 事件日志
    /// </summary>
    [Obsolete]
    internal sealed class EventLog
    {
        private static StringBuilder _csb = new StringBuilder();


        internal EventLog()
        {
        }

        public string Content
        {
            get { return _csb.ToString(); }
        }

        /// <summary>
        /// 附加日志
        /// </summary>
        /// <param name="logText"></param>
        public void Append(string logText)
        {
            _csb.Append(logText);
        }

        public void Clear()
        {
            _csb.Remove(0, _csb.Length);
        }

        public override string ToString()
        {
            return _csb.ToString();
        }
    }
}