//
// Copyright (C) 2007-2008 OPSoft INC,All rights reseved.
// 
// Project: Ops.Cms
// FileName : SoftwareActivator.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/12/23 17:01:02
// Description :
//
// Get infromation of this software,please visit our site http://cms.ops.cc
//
// =========================================
//  
//  NOTICE : 开源版本不提供激活功能
// 


namespace Ops.Cms
{
    using Ops.Framework.Extensions;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 软件激活器
    /// </summary>
    internal class SoftwareActivator
    {
        /// <summary>
        /// 用于保存检查的键
        /// </summary>
        private static string checkKey;

        //激活状态是否正常
        //异步检查，如果不正常则设置为false
        //下次运行时候提示激活
        private static bool activiveIsNormal = true;

        /// <summary>
        /// 保存激活信息
        /// </summary>
        private static string activeInfo;

        /// <summary>
        /// 用户地址栈
        /// </summary>
        private static Stack<string> userAddress = new Stack<string>(10);


        /// <summary>
        /// 检查是否寄存在本地调试
        /// </summary>
        /// <returns></returns>
        private static bool CheckIsHostOnLocalhost()
        {
            return true;
        }



        static SoftwareActivator()
        {
            checkKey = StringExtensions.EncodeMD5(String.Format(
                "opsite$activator_{0:yyyy:MM:dd}", DateTime.Now)); //检查KEY  
            activeInfo = String.Empty; //初始化激活信息
        }


        /// <summary>
        /// 校验激活状态
        /// </summary>
        internal static void VerifyActivation()
        {

        	
        }

        /// <summary>
        /// 检查网站激活状态
        /// </summary>
        internal static void CheckActiveState()
        {
        }
    }
}
