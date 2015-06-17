//
// Copyright (C) 2007-2008 S1N1.COM,All rights reseved.
// 
// Project: AtNet.Cms.Manager
// FileName : ManagerLogic.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/10/15 19:19:00
// Description :
//
// Get infromation of this software,please visit our site http://cms.ops.cc
//
//

using AtNet.Cms.Cache.CacheCompoment;
using AtNet.Cms.Domain.Interface.Models;
using AtNet.Cms.old;
using AtNet.Cms.Utility;
using AtNet.Cms.Web.WebManager.Handle;

namespace AtNet.Cms.WebManager
{
	using System;
	using System.Web;
	using System.Reflection;
	using System.Text.RegularExpressions;
    using AtNet.Cms.DataTransfer;
    using System.Globalization;

	/// <summary>
	/// 系统管理请求事件
	/// </summary>
	/// <param name="group"></param>
	public delegate void ManageEvent(object obj);

	/// <summary>
	/// 管理核心逻辑
	/// </summary>
	public class Logic
	{

		public static SiteDto CurrentSite
		{
			get
			{
				return new BasePage().CurrentSite;
			}
		}
		/// <summary>
		/// 程序集
		/// </summary>
		private Assembly assembly = Assembly.GetAssembly(typeof(Logic));
		private string nameSpace = typeof(SystemC).Namespace ;

		/// <summary>
		/// 进行管理请求时候发生
		/// </summary>
		public static event ManageEvent OnManageRequest;

		/// <summary>
		/// 当错误时候发生
		/// </summary>
		public static event ManageEvent OnError;

		/// <summary>
		/// 请求
		/// </summary>
		/// <param name="context"></param>
		public static void Request(HttpContext context)
		{
			new Logic().RequestModule(context);
		}

		private HttpRequest request;
		private HttpResponse response;

		/// <summary>
		/// 请求模块
		/// </summary>
		/// <param name="context"></param>
		private void RequestModule(HttpContext context)
		{
			//
			// 参数:
			// module:模块
			// action:动作
			//
			string module,
			action;

			//触发事件
			if (OnManageRequest != null) OnManageRequest(context);

			//获取请求和发送对象
			request = context.Request;
			response = context.Response;


			//不需要进行权限验证的页面
			string query = request.Url.Query;
			if (Regex.IsMatch(query, "^?res=[^&]+"))
			{
				CallMethod("ResourceC", "Read");
				return;
			}
			else if(Regex.IsMatch(query, "^?res_combine=[^&]+"))
			{
				CallMethod("ResourceC", "Combine");
				return;
			}
			//加载页面
			else if(Regex.IsMatch(query,"^?load=(.+?)"))
			{
				CallMethod("SystemC", "Load");
				return;
            }
            else if (query.StartsWith("?ls", true, CultureInfo.InvariantCulture))
            {
                CallMethod("SystemC", "LoadSession");
                return;
            }



			//获取模块和动作参数
			module = request["module"];
			action = request["action"];
		    if (String.IsNullOrEmpty(action)) action = "Index";

			//检测是否已经登录并检验是否有权执行此操作
			if (Regex.IsMatch(action,"^(?!login|verifycode)[a-z]*$",RegexOptions.IgnoreCase))
			{
				//UserState.Administrator.Login("admin", "123000", 1000);

				if (!UserState.Administrator.HasLogin)
				{
					//response.Redirect(String.Format("{0}?action=login",request.Path), true);
					response.Write(String.Format("<script>window.parent.location.replace('{0}?action=login')</script>", request.Path));
					return;
				}

				//无权执行操作则返回
				if (!HasPermissions())
				{
					return;
				}
			}


			//检验参数的合法性
			switch(module){

				default:
					OnError("模块错误,请检查！");
					break;

					//主页
				case "":
				case null:
				case "system":
					CallMethod("SystemC",action); break;

					//站点管理
                case "site": CallMethod("SiteC", action); break;

					//AJAX操作
                    case "ajax": CallMethod("AjaxC", action); break;
					
					//文档
                    case "archive": CallMethod("ArchiveC", action); break;

					//栏目
                    case "category": CallMethod("CategoryC", action); break;

					//链接
					case "link": CallMethod("LinkC", action); break;

					//用户
					case "user": CallMethod("UserC", action); break;

					//模板
                    case "template": CallMethod("TemplateC", action); break;

					//插件
					case "plugin": CallMethod("PluginC", action); break;

					//图像
					case "file": CallMethod("FileC", action); break;

					//上传
					case "upload": CallMethod("UploadC", action); break;

					//模块
					case "module": CallMethod("ModuleC", action); break;

					//表单
					case "table": CallMethod("TableC", action); break;

					//工具
					case "tool": CallMethod("MToolsC", action); break;

                    //扩展字段
                    case "extend": CallMethod("ExtendC", action); break;

			}

		}

	    internal void CallMethod(string className, string methodName)
	    {
	        var requestMethod = request.HttpMethod;

	        var obj = assembly.CreateInstance(String.Format("{0}.{1}", nameSpace, className));

	        if (obj != null)
	        {
	            MethodInfo method = obj.GetType().GetMethod(String.Format("{0}_{1}", methodName, requestMethod),
	                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
	            if (method != null)
	            {
	                if (method.ReturnType == typeof (String))
	                {
	                    response.Write(method.Invoke(obj, null) as String);
	                }
	                else
	                {
	                    method.Invoke(obj, null);
	                }

	                CmsCacheUtility.EvalCacheUpdate<MCacheUpdateAttribute>(method);
	                return;
	            }
	        }

	        OutputInternalError(className, methodName, requestMethod);
	    }

	    private void OutputInternalError(string className, string methodName, string requestMethod)
	    {
	        string tpl = @" <div style=""font-size:12px;text-align:center;padding:10px;"">
                                <h3>访问的页面出错，代码:502</h3>

                                <strong>这可能因为当前系统版本不支持此功能！</strong><br />

                                相关信息:{0}</div>
                                ";
	        response.Write(
	            String.Format(tpl, className + "/" + methodName + "/" + requestMethod));
	        // OnError("操作未定义!"+className+"/"+methodName); response.End();
	        response.StatusCode = 500;
	    }


	    //判断是否有权限执行操作
		private bool HasPermissions()
		{
			string uri = HttpContext.Current.Request.Url.Query;
			UserDto user = UserState.Administrator.Current;

			//如果是超级管理员，拥有所有操作权限
			if ((user.RoleFlag & (int)UserGroups.Master) !=0) return true;

			return new PermissionAttribute(uri).Validate(user);
		}

	}
}
