//
// Copyright (C) 2007-2008 S1N1.COM,All rights reseved.
// 
// Project: J6.Cms.Utility
// FileName : ScriptUtility.cs
// author : PC-CWLIU (new.min@msn.com)
// Create : 2011/10/31 18:03:20
// Description :
//
// Get infromation of this software,please visit our site http://cms.j6.cc
//
//

using System;

namespace J6.Cms.Utility
{
    public class ScriptUtility
    {
        /// <summary>
        /// 客户端调用方法
        /// </summary>
        public const string ClientCallFunc = "function clientCall(script){if(script)eval(script);}";

        /// <summary>
        /// 调用客户端父窗调用
        /// </summary>
        /// <returns></returns>
        public static string ParentClientScriptCall(string script)
        {
            const string parentCallScript = "<script type=\"text/javascript\">window.parent.clientCall('{0}')</script>";

            if (String.IsNullOrEmpty(script)) return String.Empty;
            else
            {
                script = script.Replace("'", "\\'");
                return String.Format(parentCallScript, script);
            }
        }
    }
}
