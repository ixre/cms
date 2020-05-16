﻿using System;

 namespace JR.Stand.Core.PluginKernel
{
    /// <summary>
    /// 插件异常
    /// </summary>
    public class PluginException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public PluginException(string message) : base(message)
        {
        }

        public override string HelpLink
        {
            get { return "http://www.s1n1.com"; }
            set { throw new NotImplementedException(); }
        }

        public override string Message
        {
            get { return "[ Plugin] -" + base.Message; }
        }
    }
}