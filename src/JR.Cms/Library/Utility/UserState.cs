/*
 * Copyright 2010 OPS.CC,All rights reserved.
 * 
 * name     : account state
 * author_id   : newmin
 * date     : 2010/11/06 23:29
 * 
 * Modify:
 *   2012-12-22 15:28   newmin [!]: modify cookie save method
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using JR.Cms.Conf;
using JR.Cms.Domain.Interface.Models;
using JR.Cms.Infrastructure.Domain;
using JR.Cms.Library.CacheService;
using JR.Cms.Library.DataAccess.BLL;
using JR.Cms.ServiceDto;
using JR.Stand.Abstracts.Web;
using JR.Stand.Core.Framework;
using JR.Stand.Core.Framework.Extensions;
using JR.Stand.Core.Web;
using JWT.Algorithms;
using JWT.Builder;
using Org.BouncyCastle.Utilities;

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
            var sb = new StringBuilder();
            CharArray = new int[] {98, 100, 101, 104, 108, 120, 111, 107, 113};
            Array.ForEach(CharArray, a => { sb.Append((char) a); });

            AdministratorTokenPattern = string.Format("^cms_sid_([{0}]+)$", sb.ToString());
            memberTokenPattern = string.Format("^_token_[{0}]+_$", sb.ToString());
        }

        /// <summary>
        /// 创建随机的COOKIE 
        /// </summary>
        /// <returns></returns>
        private static string GeneratorRandomStr()
        {
            var sb = new StringBuilder();
            var rd = new Random();
            var arrayLength = CharArray.Length;

            var i = 0;
            do
            {
                ++i;
                sb.Append((char) CharArray[rd.Next(0, arrayLength - 1)]);
            } while (i < TokenLength);

            return sb.ToString();
        }


        public static class Member
        {
            /// <summary>
            /// 当前登录的会员
            /// </summary>
            public static Domain.Interface.Models.Member Current
            {
                get
                {
                    var ctx = HttpHosting.Context;
                    var member = ctx.Session.GetObjectFromJson<Domain.Interface.Models.Member>("member");
                    if (member == null)
                        if (ctx.Request.TryGetCookie("member", out var strUid))
                            if (int.TryParse(strUid, out var id))
                            {
                                member = CmsLogic.Member.GetMember(id);
                                ctx.Session.SetObjectAsJson("member", member);
                            }

                    return member;
                }
                set
                {
                    var ctx = HttpHosting.Context;
                    if (value != null)
                    {
                        ctx.Session.SetObjectAsJson("member", value);
                        var opt = new HttpCookieOptions
                        {
                            Expires = DateTime.Now.AddDays(2),
                            MaxAge = new TimeSpan(2, 0, 0, 0, 0),
                            Path = "/"
                        };

                        ctx.Response.AppendCookie("member", value.ID.ToString(), opt);
                    }
                    else
                    {
                        var opt = new HttpCookieOptions
                        {
                            Expires = DateTime.Now.AddDays(-2),
                            MaxAge = new TimeSpan(-2, 0, 0, 0, 0),
                            Path = "/"
                        };
                        ctx.Response.DeleteCookie("member", opt);
                    }
                }
            }
        }

        /// <summary>
        /// 管理员 
        /// </summary>
        public static class Administrator
        {
            

            private static UserDto GetUserFromJwt(ICompatibleHttpContext ctx, string adminSk)
            {
                ctx.Request.TryGetCookie("cms_ds_token", out var cookieValue);
                if (string.IsNullOrEmpty(cookieValue))return null;
                var payload = JwtBuilder.Create()
                    .WithAlgorithm(new HMACSHA256Algorithm()) // symmetric
                    .WithSecret(Settings.SYS_RSA_KEY)
                    .MustVerifySignature()
                    .Decode<IDictionary<string, object>>(cookieValue);
                if ((string) payload["iss"] != "JRCms")
                {
                    throw new Exception("401: error jwt iss, access denied");
                }
                long exp = Convert.ToInt64(payload["exp"]);
                if (TimeUtils.Unix(DateTime.Now) > exp)
                {
                    throw new Exception("401: session timeout");
                }
                String aud  = (String)payload["aud"];
                var cre = LocalService.Instance.UserService.GetCredentialByUserName(aud);
                if (cre == null) return null;
                UserDto user = LocalService.Instance.UserService.GetUser(cre.UserId);
                //用户不存在则返回false
                if (user != null)
                {
                    ctx.Session.SetObjectAsJson(adminSk, user);
                }

                return user;
            }

            /// <summary>
            /// 账户信息
            /// </summary>
            public static UserDto Current
            {
                get
                {
                    var ctx = HttpHosting.Context;
                    var user = ctx.Session.GetObjectFromJson<UserDto>(AdminSk);
                    if (user != null) return user;
                    return GetUserFromJwt(ctx,AdminSk);
                    
                    string cookieValue = null;
                    var cookieKeyIsRight = true; //检查cookie字符是否存在数组中
                    foreach (var key in ctx.Request.CookiesKeys())
                        if (Regex.IsMatch(key, AdministratorTokenPattern))
                        {
                            var tokenKey = Regex.Match(key, AdministratorTokenPattern).Groups[1].Value;
                            if (tokenKey.Length == TokenLength)
                            {
                                foreach (var c in tokenKey)
                                    if (!Array.Exists(CharArray, a => a == c))
                                    {
                                        cookieKeyIsRight = false;
                                        break;
                                    }

                                if (cookieKeyIsRight) ctx.Request.TryGetCookie(key, out cookieValue);
                            }

                            // break;
                        }

                    if (!String.IsNullOrEmpty(cookieValue))
                    {
                        var decodedBytes = Convert.FromBase64String(cookieValue);
                        var paramStr = Encoding.UTF8.GetString(decodedBytes).Split('&');
                        var username = paramStr[0];
                        var cookieToken = paramStr[1];
                        var cre = LocalService.Instance.UserService.GetCredentialByUserName(username);
                        if (cre == null) return null;
                        user = LocalService.Instance.UserService.GetUser(cre.UserId);
                        //用户不存在则返回false
                        if (user != null)
                        {
                            var token = (username + Generator.Salt + cre.Password).Md5();
                            //密钥一致，则登录
                            if (string.Compare(token, cookieToken, StringComparison.OrdinalIgnoreCase) == 0)
                                ctx.Session.SetObjectAsJson(AdminSk, user);
                        }
                    }

                    return user;
                }
            }
            /// <summary>
            /// 用户类型
            /// </summary>
            public static UserGroups Type => throw new Exception();

            /// <summary>
            /// 用户所在的组信息
            /// </summary>
            public static UserGroup Group => CmsLogic.UserBll.GetUserGroup(Type);

            /// <summary>
            /// 是否登录
            /// </summary>
            public static bool HasLogin => Current != null;

            /// <summary>
            /// 用户登录
            /// </summary>
            /// <param name="username"></param>
            /// <param name="password"></param>
            /// <param name="minutes"></param>
            /// <returns></returns>
            public static int Login(string username, string password, double minutes)
            {
                var sha1Pwd = Generator.CreateUserPwd(password);
                var result = LocalService.Instance.UserService.TryLogin(username, sha1Pwd);
                if (result.Tag == 1)
                {
                    Exit();
                    var ctx = HttpHosting.Context;
                    var user = LocalService.Instance.UserService.GetUser(result.Uid);
                    ctx.Session.SetObjectAsJson(AdminSk, user);
                    var opt = new HttpCookieOptions
                    {
                        Expires = DateTime.Now.AddMinutes(minutes),
                        // cookie.Domain=AppContext.Config.Domain.HostName;
                        Path = "/" + Settings.SYS_ADMIN_TAG
                    };

                    long expiresTime = DateTime.UtcNow.AddMinutes(minutes).Unix();
                    
                    String token = JwtBuilder.Create()
                        .WithAlgorithm(new HMACSHA256Algorithm()) // symmetric
                        .WithSecret(Settings.SYS_RSA_KEY)
                        .AddClaim("aud", username)
                        .AddClaim("iss", "JRCms")
                        .AddClaim("sub", "JRCms-Dashboard")
                        .AddClaim("exp", expiresTime)
                        .Encode();
                    //保存到Cookie中的密钥
                    // var token = (username + Generator.Salt + sha1Pwd).Md5();
                    // var encodeBytes = Encoding.UTF8.GetBytes(username + "&" + token);
                    // var encodedTokenStr = Convert.ToBase64String(encodeBytes);
                    ctx.Response.AppendCookie("cms_ds_token", token,opt);
                }

                return result.Tag;
            }

            public static void Exit()
            {
                var ctx = HttpHosting.Context;
                //UserBll user = Current;
                //移除会话
                ctx.Session.Remove(AdminSk);
                //移除Cookie
                foreach (var key in ctx.Request.CookiesKeys())
                    if (Regex.IsMatch(key.ToString(), AdministratorTokenPattern))
                    {
                        var opt = new HttpCookieOptions();
                        opt.Expires = DateTime.Now.AddYears(-1);
                        opt.Path = "/" + Settings.SYS_ADMIN_TAG;
                        ctx.Response.DeleteCookie(key, opt);
                    }
            }

            /// <summary>
            /// 清除会话
            /// </summary>
            public static void Clear()
            {
                HttpHosting.Context.Session.Remove(AdminSk);
            }
        }
    }
}