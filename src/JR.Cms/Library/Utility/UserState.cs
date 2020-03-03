/*
 * Copyright 2010 OPS,All rights reseved.
 * 
 * name     : account state
 * author_id   : newmin
 * date     : 2010/11/06 23:29
 * 
 * Modify:
 *   2012-12-22 15:28   newmin [!]: modify cookie save method
 */

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using JR.Cms.CacheService;
using JR.Cms.Conf;
using JR.Cms.Domain.Interface.Models;
using JR.Cms.Domain.Interface.Value;
using JR.Cms.Infrastructure.Domain;
using JR.Cms.Library.DataAccess.BLL;
using LoginResultDto = JR.Cms.ServiceDto.LoginResultDto;
using UserDto = JR.Cms.ServiceDto.UserDto;

namespace JR.Cms.Library.Utility
{
    //
    //TODO:需更新到CSite中
    // todo: upgrade sha1 pwd
    //
    /// <summary>
    /// 
    /// </summary>
    public static class UserState
    {
        private static readonly int[] CharArray;

        private const int TokenLength = 10;

        private const string AdminSk = "$jr.ASK";

        /// <summary>
        /// 管理员Cookie键匹配模式
        /// </summary>
        private static readonly string AdministratorTokenPattern;

        /// <summary>
        /// 会员Cookie键匹配模式
        /// </summary>
        private static string memberTokenPattern;

        static UserState()
        {
            StringBuilder sb = new StringBuilder();
            CharArray = new int[] { 98, 100, 101, 104, 108, 120, 111, 107, 113 };
            Array.ForEach(CharArray, a =>
            {
                sb.Append((char)a);
            });

            AdministratorTokenPattern = String.Format("^cms_sid_([{0}]+)$", sb.ToString());
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
            int arrayLength = CharArray.Length;

            int i = 0;
            do
            {
                ++i;
                sb.Append((char)CharArray[rd.Next(0, arrayLength - 1)]);
            }
            while (i < TokenLength);

            return sb.ToString();
        }



        public static class Member
        {

            /// <summary>
            /// 当前登录的会员
            /// </summary>
            public static global::JR.Cms.Domain.Interface.Models.Member Current
            {
                get
                {
                    global::JR.Cms.Domain.Interface.Models.Member m = HttpContext.Current.Session["member"] as global::JR.Cms.Domain.Interface.Models.Member;
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
                    UserDto user = HttpContext.Current.Session[AdminSk] as UserDto;
                    if (user == null)
                    {
                        HttpCookie cookie = null;
                        bool cookieKeyIsRight = true;       //检查cookie字符是否存在数组中
                        string tokenKey;
                        foreach (string key in HttpContext.Current.Request.Cookies.Keys)
                        {
                            if (Regex.IsMatch(key, AdministratorTokenPattern))
                            {
                                tokenKey = Regex.Match(key, AdministratorTokenPattern).Groups[1].Value;

                                if (tokenKey.Length == TokenLength)
                                {
                                    foreach (char c in tokenKey)
                                    {
                                        if (!Array.Exists<int>(CharArray, a => a == c))
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
                            if (cre == null) return null;
                            user =ServiceCall.Instance.UserService.GetUser(cre.UserId);

                            //用户不存在则返回false
                            if (user == null) return null;

                            string token = (username +Generator.Offset + cre.Password).EncodeMD5();

                            //密钥一致，则登录
                            if (String.Compare(token, cookieToken, StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                HttpContext.Current.Session[AdminSk] = user;
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
            public static UserGroup Group { get { return CmsLogic.UserBll.GetUserGroup(Type); } }

            /// <summary>
            /// 是否登录
            /// </summary>
            public static bool HasLogin { get { return Current != null; } }

            public static int Login(string username, string password, double minutes)
            {
                String sha1Pwd = password;
                LoginResultDto result = ServiceCall.Instance.UserService.TryLogin(username,sha1Pwd);

                if (result.Tag == 1)
                {
                    Exit();

                    UserDto user = ServiceCall.Instance.UserService.GetUser(result.Uid);

                    HttpContext.Current.Session[AdminSk] = user;
                    HttpCookie cookie = new HttpCookie(String.Format("cms_sid_{0}", GeneratorRandomStr()));
                    cookie.Expires = DateTime.Now.AddMinutes(minutes);
                    // cookie.Domain=AppContext.Config.Domain.HostName;

                    //保存到Cookie中的密钥
                    string token = (username + Generator.Offset + sha1Pwd).EncodeMD5();

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
                //UserBll user = Current;
                //移除会话
                HttpContext.Current.Session.Remove(AdminSk);

                //移除Cookie
                HttpResponse response = HttpContext.Current.Response;
                HttpCookie cookie = null;
                var keys = HttpContext.Current.Request.Cookies.Keys;
                int keysLength = keys.Count;
                for (int i = 0; i < keysLength; i++)
                {
                    if (Regex.IsMatch(keys[i].ToString(), AdministratorTokenPattern))
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
                HttpContext.Current.Session.Remove(AdminSk);
            }
        }

    }
}