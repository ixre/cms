//
// Copyright (C) 2007-2008 TO2.NET,All rights reserved.
// 
// Project: jr.Cms.Manager
// FileName : ManagerLogic.cs
// Author : Jarrysix (new.min@msn.com)
// Create : 2011/10/15 19:19:00
// Description :
//
// Get information of this software,please visit our site http://to2.net/cms
//
//

using System;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JR.Cms.Library.CacheProvider.CacheComponent;
using JR.Cms.Library.Utility;
using JR.Cms.ServiceDto;
using JR.Cms.Web.Manager.Handle;
using JR.Stand.Abstracts.Safety;
using JR.Stand.Abstracts.Web;
using JR.Stand.Core.Web;
using Newtonsoft.Json;

namespace JR.Cms.Web.Manager
{
    /// <summary>
    /// 系统管理请求事件
    /// </summary>
    public delegate void ManageEvent(object obj);

    /// <summary>
    /// 管理核心逻辑
    /// </summary>
    public class Logic
    {
        /// <summary>
        /// 
        /// </summary>
        public static SiteDto CurrentSite => new BasePage().CurrentSite;

        /// <summary>
        /// 程序集
        /// </summary>
        private static readonly Assembly Assembly = Assembly.GetAssembly(typeof(Logic));


        /// <summary>
        /// 进行管理请求时候发生
        /// </summary>
        public static event ManageEvent OnManageRequest;

        /// <summary>
        /// 请求
        /// </summary>
        /// <param name="context"></param>
        public static Task Request(ICompatibleHttpContext context)
        {
            return RequestModule(context);
        }

        /// <summary>
        /// 请求模块
        /// </summary>
        /// <param name="context"></param>
        private static Task RequestModule(ICompatibleHttpContext context)
        {
            //
            // 参数:
            // module:模块
            // action:动作
            //

            //触发事件
            OnManageRequest?.Invoke(context);

            //获取请求和发送对象
            var request = context.Request;
            var response = context.Response;
            
            //不需要进行权限验证的页面
            var query = context.Request.GetQueryString();
            if (Regex.IsMatch(query, "^\\?res=[^&]+"))
            {
                return CallMethod(context, typeof(ResourceHandler), "Read");
            }
            else
            {
                if (Regex.IsMatch(query, "^\\?res_combine=[^&]+"))
                {
                    return CallMethod(context, typeof(ResourceHandler), "Combine");
                }

                //加载页面
                if (Regex.IsMatch(query, "^\\?load=(.+?)"))
                {
                    return CallMethod(context, typeof(SystemHandler), "Load");
                }

                if (query.StartsWith("?ls", true, CultureInfo.InvariantCulture))
                {
                    return CallMethod(context, typeof(SystemHandler), "LoadSession");
                }
            }
            
            //获取模块和动作参数
            var module = request.Query("module");
            var action = request.Query("action");
            if (string.IsNullOrEmpty(action)) action = "Index";

            //检测是否已经登录并检验是否有权执行此操作
            if (Regex.IsMatch(action, "^(?!login|verifycode)[a-z]*$", RegexOptions.IgnoreCase))
            {
                //UserState.Administrator.Login("admin", "123000", 1000);
                //
                //			    context.Session["$jr.ASK"] = new UserDto
                //			    {
                //                    Id = 1,
                //			        IsMaster =true,
                //			        Name = "管理员"
                //			    };
                if (!UserState.Administrator.HasLogin)
                {
                    //response.Redirect(String.Format("{0}?action=login",request.Path), true);
                    return response.WriteAsync(
                        $"<script>window.parent.location.replace('{request.GetPath()}?action=login')</script>");
                }
                // 无权执行操作则返回
                if (!HasPermissions()) return  SafetyTask.CompletedTask;
            }

            // 默认返回system的界面
            if (String.IsNullOrEmpty(module))
            {
                return CallMethod(context, typeof(SystemHandler), action);
            }

            //　检验参数的合法性
            switch (module)
            {
                //主页
                case "system":return CallMethod(context, typeof(SystemHandler), action);

                //站点管理
                case "site":return CallMethod(context, typeof(SiteHandler), action);

                //AJAX操作
                case "ajax":return CallMethod(context, typeof(AjaxHandler), action);

                //文档
                case "archive":return CallMethod(context, typeof(ArchiveHandler), action);

                //栏目
                case "category":return CallMethod(context, typeof(CategoryHandler), action);
                
                //链接
                case "link":return CallMethod(context, typeof(LinkHandler), action);

                //用户
                case "user":return CallMethod(context, typeof(UserHandler), action);

                //模板
                case "template":return CallMethod(context, typeof(TemplateHandler), action);

                //插件
                case "plugin":return CallMethod(context, typeof(PluginHandler), action);

                //图像
                case "file":return CallMethod(context, typeof(FileHandler), action);

                //上传
                case "upload":return CallMethod(context, typeof(UploadHandler), action);

                //模块
                // case "module":
                //     return CallMethod(context,  typeof(ModuleHandler), action);

                //表单
                case "table":return CallMethod(context, typeof(TableHandler), action);

                //工具
                // case "tool":
                //    return CallMethod(context,  typeof(MToolsHandler), action);

                //扩展字段
                case "extend":return CallMethod(context, typeof(ExtendHandler), action);
                // 助手
                case "assistant":return CallMethod(context, typeof(AssistantHandler), action);
                // 编辑器
                case "editor":return CallMethod(context, typeof(EditorHandler), action);
                // Seo
                case "seo": return CallMethod(context, typeof(SeoHandler), action);
            }
            return context.Response.WriteAsync("模块错误,请检查！");
        }

        private static Task CallMethod(ICompatibleHttpContext context, Type typeName, string methodName)
        {
            var requestMethod = context.Request.Method();
            var obj = Assembly.CreateInstance($"{typeName.FullName}");
            if (obj != null)
            {
                if (requestMethod != "GET") methodName += "_" + requestMethod;
                var method = obj.GetType().GetMethod(methodName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (method != null)
                {
                    Task task;
                    if (method.ReturnType == typeof(Task))
                    {
                        task = method.Invoke(obj, new object[] {context}) as Task;
                    }
                    else if (method.ReturnType == typeof(string))
                    {
                        task = context.Response.WriteAsync(method.Invoke(obj, null) as string);
                    }
                    else if (method.ReturnType == typeof(void))
                    {
                        method.Invoke(obj, null);
                        task = SafetyTask.CompletedTask;
                    }
                    else
                    {
                        var result = method.Invoke(obj, null);
                        task = context.Response.WriteAsync(JsonConvert.SerializeObject(result));
                    }

                    if (requestMethod == "POST")
                    {
                        CmsCacheUtility.EvalCacheUpdate<MCacheAttribute>(method); //清理缓存
                    }
                    return task;
                }

            }

            return OutputInternalError(context, typeName.FullName, methodName, requestMethod);
        }

        private static Task OutputInternalError(ICompatibleHttpContext context, string className, string methodName,
            string requestMethod)
        {
            var tpl = @" <div style=""font-size:12px;text-align:center;padding:10px;"">
                                <h3>访问的页面出错，代码:502</h3>

                                <strong>这可能因为当前系统版本不支持此功能！</strong><br />

                                相关信息:{0}</div>
                                ";
            context.Response.StatusCode(500);
            return context.Response.WriteAsync(string.Format(tpl, className + "/" + methodName + "/" + requestMethod));
            // OnError("操作未定义!"+className+"/"+methodName); response.End();
        }


        //判断是否有权限执行操作
        private static bool HasPermissions()
        {
            var uri = HttpHosting.Context.Request.GetQueryString();
            var user = UserState.Administrator.Current;

            //如果是超级管理员，拥有所有操作权限
            if (user.IsMaster) return true;

            return new PermissionAttribute(uri).Validate(user);
        }
    }
}