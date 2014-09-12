//
// Copyright (C) 2007-2008 OPSoft INC,All rights reseved.
// 
// Project: OPSite.Plugin
// FileName : TrafficCounter.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/11/28 14:32:09
// Description :
//
// Get infromation of this software,please visit our site http://www.ops.cc
//
//

namespace Ops.Cms.Utility
{
    using Ops.Framework;
    using Ops.Json;
    using System;
    using System.Threading;

    /// <summary>
    /// 流量统计
    /// </summary>
    public class TrafficCounter
    {
        private const string defaultJson = "{ip:0,pv:0}";
        private static SettingFile trafficFile = new SettingFile(AppDomain.CurrentDomain.BaseDirectory + "data/traffic.xml");

        /// <summary>
        /// 获取指定日期的数据
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string GetData(DateTime date)
        {
            string key = String.Format("{0:yyyyMMdd}", date);
            if (trafficFile.Contains(key))
            {

                return trafficFile[key];
            }
            else
            {
                trafficFile.Append(key, defaultJson);
                trafficFile.Flush();
            }

            //try
            //{
            //    trafficFile["ips"] = String.Empty;
            //}
            //catch
            //{
            //    trafficFile.Append("ips", String.Empty);
            //}

            return defaultJson;
        }

        private static int GetIntValue(string field)
        {
            int i = 0;

            string json = GetData(DateTime.Now);
            if (String.Compare(json, defaultJson, true) != 0)
            {
                JsonAnalyzer js = new JsonAnalyzer(json);
                int.TryParse(js.GetValue(field), out i);
            }
            return i;
        }

        /// <summary>
        /// 获取今日IP数量
        /// </summary>
        /// <returns></returns>
        public static int GetTodayIPAmount()
        {
            return GetIntValue("ip");
        }

        /// <summary>
        /// 获取今日PV数量
        /// </summary>
        /// <returns></returns>
        public static int GetTodayPVAmount()
        {
            return GetIntValue("pv");
        }

        /// <summary>
        /// 获取累计IP数
        /// </summary>
        /// <returns></returns>
        public static int GetTotalIPAmount()
        {
            return int.Parse(trafficFile["totalIP"]);
        }

        /// <summary>
        /// 获取累计PV数
        /// </summary>
        /// <returns></returns>
        public static int GetTotalPVAmount()
        {
            return int.Parse(trafficFile["totalPV"]);
        }

        /// <summary>
        /// 记录IP信息
        /// </summary>
        /// <param name="ip"></param>
        public static void Record(string ip)
        {
            new Thread(() =>
            {

                try
                {
                    string key = String.Format("{0:yyyyMMdd}", DateTime.Now);
                    string data;                                   //流量数据
                    int todayIP = 0, todayPV = 0,
                        totalIP = 0, totalPV = 0;

                    string ips = String.Empty;                     //IP库

                    //获取累计的IP和PV,如果不存在，则创建字段
                    if (trafficFile.Contains("totalIP"))
                    {
                        int.TryParse(trafficFile["totalIP"], out totalIP);
                    }
                    else
                    {
                        trafficFile.Append("totalIP", "0");
                    }

                    if (trafficFile.Contains("totalPV"))
                    {
                        int.TryParse(trafficFile["totalPV"], out totalPV);
                    }
                    else
                    {
                        trafficFile.Append("totalPV", "0");
                    }

                    data = GetData(DateTime.Now);
                    JsonAnalyzer js = new JsonAnalyzer(data);

                    int.TryParse(js.GetValue("ip"), out todayIP);
                    int.TryParse(js.GetValue("pv"), out todayPV);

                    //检测是否是独立访客,如果是则增加IP并记录
                    if (trafficFile.Contains("ips"))
                    {
                        ips = trafficFile["ips"];
                    }
                    else
                    {
                        trafficFile.Append("ips", "");
                    }

                    if (ips.IndexOf(ip) == -1)
                    {
                        ips = String.Format("{0}{1}|", ips, ip);
                        trafficFile["ips"] = ips;
                        ++todayIP;
                        ++totalIP;

                        //更新今日IP数据并返回新的JSON
                        js = new JsonAnalyzer(js.SetValue("ip", todayIP.ToString()));

                        //保存总的IP数据
                        if (trafficFile.Contains("totalIP"))
                        {
                            trafficFile["totalIP"] = totalIP.ToString();
                        }
                        else
                        {
                            trafficFile.Append("totalIP", totalIP.ToString());
                        }
                    }
                    ++todayPV;
                    ++totalPV;

                    //保存今日PV
                    trafficFile[key] = js.SetValue("pv", todayPV.ToString());

                    //保存总的PV数据
                    trafficFile["totalPV"] = totalPV.ToString();
                }
                catch (Exception ex)
                {
                    //
                    //Catch error
                    //

                    trafficFile.Flush();
                }
            }).Start();

        }

    }
}
