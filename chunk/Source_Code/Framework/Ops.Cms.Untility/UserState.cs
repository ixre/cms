/*
 * Copyright 2010 OPS,All rights reseved.
 * 
 * name     : account state
 * publisher_id   : newmin
 * date     : 2010/11/06 23:29
 * 
 * Modify:
 *   2012-12-22 15:28   newmin [!]: modify cookie save method
 */

using System;
using System.ComponentModel.Design;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using AtNet.Cms.BLL;
using AtNet.Cms.CacheService;
using AtNet.Cms.Conf;
using AtNet.Cms.DataTransfer;
using AtNet.Cms.Domain.Interface.Models;
using AtNet.Cms.Domain.Interface.Value;
using AtNet.Cms.Infrastructure.Domain;
using AtNet.DevFw.Framework.Extensions;

namespace AtNet.Cms.Utility
{
    //
    //TODO:需更新到CSite中
    //
    /// <summary>
    /// 
    /// </summary>
    public static class UserState
    {
        private static int[] charArray;

        private const int tokenLength = 10;

        private const string adminSK = "$cms.ASK";

        /// <summary>
        /// 管理员Cookie键匹配模式
        /// </summary>
        private static string administatorTokenPattern;

        /// <summary>
        /// 会员Cookie键匹配模式
        /// </summary>
        private static string memberTokenPattern;

        static UserState()
        {
            StringBuilder sb = new StringBuilder();
            charArray = new int[] { 98, 100, 101, 104, 108, 120, 111, 107, 113 };
            Array.ForEach(charArray, a =>
            {
                sb.Append((char)a);
            });

            administatorTokenPattern = String.Format("^cms_sid_([{0}]+)$", sb.ToString());
            memberTokenPattern = String.Format("^_token_[{0}]+_$", sb.ToString());
        }
        /// <summary>
        /// 创建随机的COOKIE 
        /// </summary>
        /// <returns></returns>
        private static string GeneratorRandomStr()
        {
            StringBuilder sb = new StringBuilder();
            Random rd = new Random();
            int arrayLength = charArray.Length;

            int i = 0;
            do
            {
                ++i;
                sb.Append((char)charArray[rd.Next(0, arrayLength - 1)]);
            }
            while (i < tokenLength);

            return sb.ToString();
        }



        public static class Member
        {

            /// <summary>
            /// 当前登录的会员
            /// </summary>
            public static global::AtNet.Cms.Domain.Interface.Models.Member Current
            {
                get
                {
                    global::AtNet.Cms.Domain.Interface.Models.Member m = HttpContext.Current.Session["member"] as global::AtNet.Cms.Domain.Interface.Models.Member;
                    if (m == null)
                    {
                        HttpCookie c = HttpContext.Current.Request.Cookies["member"];
                        if (c != null)
                        {
                            int id;
                            int.TryParse(c.Values["uid"], out id);
                            if (id != 0)
                            {
                                m = CmsLogic.Member.GetMember(id);
                                HttpContext.Current.Session["member"] = m;
                            }

                        }
                    }
                    return m;
                }
                set
                {
                    HttpContext context = HttpContext.Current;
                    context.Session["member"] = value;
                    if (value != null)
                    {
                        HttpCookie c = new HttpCookie("member");
                        c.Values["uid"] = value.ID.ToString();
                        c.Expires = DateTime.Now.AddDays(2);
                        context.Response.Cookies.Add(c);
                    }
                    else
                    {
                        try
                        {
                            context.Response.Cookies.Remove("member");
                        }
                        catch { }
                    }
                }
            }
        }

        /// <summary>
        /// 管理员 
        /// </summary>
        public static class Administrator
        {
            /// <summary>
            /// 账户信息
            /// </summary>
            public static UserDto Current
            {
                get
                {
                    UserDto user = HttpContext.Current.Session[adminSK] as UserDto;
                    if (user == null)
                    {
                        HttpCookie cookie = null;
                        bool cookieKeyIsRight = true;       //检查cookie字符是否存在数组中
                        string tokenKey;
                        foreach (string key in HttpContext.Current.Request.Cookies.Keys)
                        {
                            if (Regex.IsMatch(key, administatorTokenPattern))
                            {
                                tokenKey = Regex.Match(key, administatorTokenPattern).Groups[1].Value;

                                if (tokenKey.Length == tokenLength)
                                {
                                    foreach (char c in tokenKey)
                                    {
                                        if (!Array.Exists<int>(charArray, a => a == c))
                                        {
                                            cookieKeyIsRight = false;
                                            break;
                                        }
                                    }

                                    if (cookieKeyIsRight)
                                    {
                                        cookie = HttpContext.Current.Request.Cookies[key.ToString()];
                                    }
                                }
                               // break;
                            }
                        }

                        if (cookie != null)
                        {
                            byte[] decodedBytes = Convert.FromBase64String(cookie.Value);
                            string[] paramStr = Encoding.UTF8.GetString(decodedBytes).Split('&');
                            string username = paramStr[0],
                                   cookieToken = paramStr[1];

                            Credential cre = ServiceCall.Instance.UserService.GetCredentialByUserName(username);
                            user =ServiceCall.Instance.UserService.GetUser(cre.UserId);

                            //用户不存在则返回false
                            if (user == null) return null;

                            string token = (username + "ADMIN@OPSoft.CMS" + cre.Password).EncodeMD5();

                            //密钥一致，则登录
                            if (String.Compare(token, cookieToken, StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                HttpContext.Current.Session[adminSK] = user;
                                return user;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                    return user;
                }
            }
            /// <summary>
            /// 用户类型
            /// </summary>
            public static UserGroups Type { get { throw new Exception(); } }
            /// <summary>
            /// 用户所在的组信息
            /// </summary>
            public static UserGroup Group { get { return CmsLogic.User.GetUserGroup(Type); } }

            /// <summary>
            /// 是否登录
            /// </summary>
            public static bool HasLogin { get { return Current != null; } }

            public static int Login(string username, string password, double minutes)
            {
                String md5Pwd = Generator.Md5Pwd(password, null);
                LoginResultDto result = ServiceCall.Instance.UserService.TryLogin(username, md5Pwd);
                if (result.Tag == 1)
                {
                    Exit();

                    UserDto user = ServiceCall.Instance.UserService.GetUser(result.Uid);

                    HttpContext.Current.Session[adminSK] = user;
                    HttpCookie cookie = new HttpCookie(String.Format("cms_sid_{0}", GeneratorRandomStr()));
                    cookie.Expires = DateTime.Now.AddMinutes(minutes);
                    // cookie.Domain=AppContext.Config.Domain.HostName;

                    //保存到Cookie中的密钥
                    string token = (username + "ADMIN@OPSoft.CMS" + md5Pwd).EncodeMD5();

                    byte[] encodeBytes = Encoding.UTF8.GetBytes(username + "&" + token);
                    string encodedtokenStr = Convert.ToBase64String(encodeBytes);

                    cookie.Value = encodedtokenStr;
                    cookie.Path = "/" + Settings.SYS_ADMIN_TAG;

                    HttpContext.Current.Response.Cookies.Add(cookie);
                }
                return result.Tag;
            }

            public static void Exit()
            {
                //User user = Current;
                //移除会话
                HttpContext.Current.Session.Remove(adminSK);

                //移除Cookie
                HttpResponse response = HttpContext.Current.Response;
                HttpCookie cookie = null;
                var keys = HttpContext.Current.Request.Cookies.Keys;
                int keysLength = keys.Count;
                for (int i = 0; i < keysLength; i++)
                {
                    if (Regex.IsMatch(keys[i].ToString(), administatorTokenPattern))
                    {
                        cookie = HttpContext.Current.Request.Cookies[i];

                        if (cookie != null)
                        {
                            cookie.Expires = DateTime.Now.AddYears(-1);
                            cookie.Path = "/" + Settings.SYS_ADMIN_TAG;
                            response.Cookies.Add(cookie);
                        }
                    }
                }

            }

            /// <summary>
            /// 清除会话
            /// </summary>
            public static void Clear()
            {
                HttpContext.Current.Session.Remove(adminSK);
            }
        }

    }
}