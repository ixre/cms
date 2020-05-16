/* *
 * name     : ExecuteHandler.cs 
 * author   : OPS newmin
 * date     : 09/29 2010
 * note     : 用来处理请求,请求的URI参数如:Exc.ashx?cmd=IP,GetIP,127.0.0.1
 * 
 * 要执行操作的类必需要程序集名称命名空间下:
 * 如要执行OPS.Security下的User类,则User类的命名空间为:OPS.Security.User
 * 调用方式**.ashx?cmd=User,GetScore,newmin
 * 
 * 2010/10/18 [+] newmin:新增Uri参数task
 * 2010/10/18 [!] newmin:修正returnObj为空返回ToString()的Bug
 * 2010/11/05 [!] newmin:修正不能接收Session的Bug
 * 2010/12/03 [!] nwemin:修正_type为静态类不能创建多个ExecuteHandler的Bug
 * 2010/12/06 [+] newmin:添加query支持,可采用*.ashx?class,method,parameters调用
 *            [+] newmin:添加无参数时候提示信息
 *            [!] newmin:修正不传参数无法返回结果的BUG
 *            [!] newmin:修正query为?xxxx时候xxxx参数不合法的验证
 *            [+] newmin:将cmd参数该为do
 * */

using System;
using System.Reflection;
using System.Web;
using JR.Stand.Core.Framework.Web;

namespace JR.DevFw.Framework.Web.unused.Handler
{
    public abstract class ExecuteHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        /// <summary>
        /// 绑定类型用于获取程序集及类型的命名空间
        /// </summary>
        protected Type handlerType;

        private static char[] chars = {',', ':'};

        #region IHttpHandler 成员

        public bool IsReusable { get; set; }

        public void ProcessRequest(HttpContext context)
        {
            //包含参数task或query包含,号,如?task=class,method或?class,method

            string task = context.Request["do"] ?? (
                context.Request.Url.Query.IndexOfAny(chars) != -1
                    ? context.Request.Url.Query.Replace("?", "")
                    : context.Request["task"]);

            if (String.IsNullOrEmpty(task))
            {
                context.Response.Write("<b>ExecuteHandler组件</b> Power by <a href=\"http://www.ops.cc\">OPSoft</a>");
                return;
            }
            else task = task.Replace("+", " "); //将空格做为+号替换
            string[] args = task.Split(chars);
            if (args.Length >= 2)
            {
                //获取执行当前代码的程序集并创建实例
                Assembly ass = Assembly.GetAssembly(handlerType);
                object obj = ass.CreateInstance(handlerType.Namespace + "." + args[0], true);

                //获取实例类型
                Type type;
                try
                {
                    type = obj.GetType();
                }
                catch
                {
                    throw new Exception("无法在命名空间:" + handlerType.Namespace + "下找到类型:" + args[0]);
                }

                //未添加WebExecuteAttribute特性的类将不被执行
                object[] attrs = type.GetCustomAttributes(true);

                bool canExecute = false;
                IWebExecute iw;
                foreach (object o in attrs)
                {
                    //如果添加IWebExecuteableAttribute特性
                    //则调用其PreExecuting方法
                    iw = o as IWebExecute;
                    if (iw != null) iw.PreExecuting();
                    else
                    {
                        //如果添加了WebExecuteableAttribute特性，则允许执行
                        if (o as WebExecuteableAttribute != null) canExecute = true;
                    }
                }
                if (!canExecute)
                {
                    context.Response.Write("此模块不允许被执行!请在需要执行的类名上添加WebExecuteable特性!");
                    return;
                }

                //获取方法,及方法上的IWebExecuteableAttribute特性,
                //并调用起PreExecuting方法
                MethodInfo method = type.GetMethod(args[1],
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                IWebExecute[] execAttr = Array.ConvertAll(method.GetCustomAttributes(false), a => a as IWebExecute);
                foreach (IWebExecute a in execAttr) a.PreExecuting();
                //执行方法

                object returnObj;

                returnObj = method.GetParameters().Length != 0 && args.Length > 2
                    ? method.Invoke(obj, task.Substring(args[0].Length + args[1].Length + 2).Split(','))
                    : method.Invoke(obj, null);


                //如国返回String类型或值类型则输出到页面
                bool isValueType = returnObj is ValueType;
                if (isValueType || method.ReturnType == typeof (string))
                    context.Response.Write((isValueType ? returnObj : returnObj ?? "").ToString());
            }
        }

        #endregion
    }
}