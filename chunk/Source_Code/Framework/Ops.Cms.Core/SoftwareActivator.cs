//
// Copyright (C) 2007-2008 S1N1.COM,All rights reseved.
// 
// Project: AtNet.Cms
// FileName : SoftwareActivator.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/12/23 17:01:02
// Description :
//
// Get infromation of this software,please visit our site http://cms.ops.cc
//
// =========================================
// 激活规则
// 1.允许本机和局域网运行
//

using System;
using System.Collections.Generic;
using System.Web;
using AtNet.Cms.Conf;
using AtNet.DevFw.Framework.Extensions;
using AtNet.DevFw.Framework.Net;
using AtNet.DevFw.Utils;

namespace AtNet.Cms
{
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
            //获取用户的IP信息并放入栈中
            //如果栈存满，则弹出最开始的IP地址
            string ip = HttpContext.Current.Request.UserHostAddress;

            lock (userAddress)
            {
                if (!userAddress.Contains(ip))
                {
                    userAddress.Push(ip);
                    if (userAddress.Count == 10)
                    {
                        userAddress.Pop();
                    }
                }
            }

            //迭代用户IP,如果包含外部IP,则返回false
            foreach (string str in userAddress)
            {
                if (String.Compare(str, "127.0.0.1") != 0)
                {
                    return false;
                }
            }

            return true;
        }



        static SoftwareActivator()
        {
            checkKey = String.Format("opsite$activator_{0:yyyy:MM:dd}",DateTime.Now).EncodeMD5();  //检查KEY  
            activeInfo = String.Empty;                                                             //初始化激活信息
        }


        /// <summary>
        /// 校验激活状态
        /// </summary>
        internal static void VerifyActivation()
        {

        	if(DateTime.Now<new DateTime(2014,02,16)){
        		return;
        	}
            //
            // 如果异步方式调用
            // 或在本地调试则不校验
            //
            if (Cms.Cache.Get(checkKey) != null || CheckIsHostOnLocalhost())
            {
                return;
            }
            


            //
            // 异步从服务器读取激活状态
            //
            /*
            new Thread(() =>
            {
                lock (activeInfo)
                {
                    CheckActiveState();
                }

            }).Start();*/


            CheckActiveState();

            //=====分析获得的数据并返回到客户端=====//
            if (!String.IsNullOrEmpty(activeInfo))
            {
                lock (activeInfo)
                {
                    try
                    {
                        JsonAnalyzer js = new JsonAnalyzer(activeInfo);
                        string result = js.GetValue("state");

                        if (result != "ok")
                        {
                            activiveIsNormal = false;                           //设置状态

                            string content = js.GetValue("content");
                            HttpContext.Current.Response.ClearContent();
                            HttpContext.Current.Response.Write(content);

                            //返回字符串
                            if (result == "end")
                            {
                                HttpRuntime.UnloadAppDomain();
                                HttpContext.Current.Response.End();
                                return;
                            }
                            else if (result == "go")
                            {
                                return;
                            }
                        }
                        else
                        {
                            activiveIsNormal = true;                           //设置状态
                            //如果通过校验，则缓存1日激活状态

                            Cms.Cache.Insert(checkKey, "1", DateTime.Now.AddDays(7));

                        }
                    }
                    catch
                    {
                        //如果发生异常
                        //则创建缓存3天候过期来避免频繁向服务器发送校验请求
                         Cms.Cache.Insert(checkKey, "1", DateTime.Now.AddDays(3));
                    }
                }
            }
        }

        /// <summary>
        /// 检查网站激活状态
        /// </summary>
        internal static void CheckActiveState()
        {
            object checkCache =Cms.Cache.Get(checkKey);
                
            if (checkCache == null)
            {
                try
                {
                    //如果Key为空则Key为temp.ops.cc产生的Key
                    string key = Settings.License_KEY 
                          ?? "YmIyNDAwMGI3YmEyZGMwZTgxZWI2OGQxYzk3MWU4NWI=";

                    string responseText = HttpClient.Post("http://ct.ops.cc/ct/license",
                        "token=YmIyNDAwMGI3YmEyZGMwZTgxZWI2OGQxYzk3MWU4NWI&license_key="
                        + key + "&license_name=" + Settings.License_NAME,null);

                    activeInfo = responseText;
                }
                catch
                {
                    //如果校验服务器请求产生问题，则不检测激活状态
                    //activeInfo = null;
                    //activeInfo = ex.Message;
                }
            }
        }
    }
}
