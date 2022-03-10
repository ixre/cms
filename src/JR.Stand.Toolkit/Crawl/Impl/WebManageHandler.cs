using System;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using JR.Stand.Core;
using JR.Stand.Core.Extensions.Http;
using JR.Stand.Core.Utils;
using Microsoft.AspNetCore.Http;

namespace JR.Stand.Toolkit.Crawl.Impl
{
    public class WebManageHandler
    {
        private static string subtitle = "-采集管理插件 Power by z3q";
        private static string ct_css = "<link rel=\"StyleSheet\" type=\"text/css\" href=\"?action=css\"/>";

        protected HttpRequest request;
        protected HttpResponse response;
        private Collector director;

        private static  Assembly assembly = typeof(WebManageHandler).Assembly;
        private static string prefix = "Crawl/Impl/Assets/";
        
        public WebManageHandler(Collector director)
        {
            this.director = director;
        }


        public Task ProcessRequest(HttpContext context)
        {
            string action = context.Request.GetParameter("action");
            string httpMethod = context.Request.Method;

            //未指定动作，则显示欢迎页面
            if (String.IsNullOrEmpty(action))
            {
                return this.Show_WelcomePage(context);
            }

            request = context.Request;
            response = context.Response;

            Type type = this.GetType();
            MethodInfo method = type.GetMethod($"{action}_{httpMethod}",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (method == null)
            {
                return context.Response.WriteAsync("执行出错");
            }

            var result = method.Invoke(this, null) as string;
            return context.Response.WriteAsync(result);
        }


        /// <summary>
        /// 加载样式表
        /// </summary>
        protected string Css_Get()
        {
            return  ResourcesReader.Read( assembly, prefix+"style.css");
        }

        protected Task Show_WelcomePage(HttpContext ctx)
        {
            var welcomeHtml = ResourcesReader.Read(assembly, prefix+"welcome_page.html");
            var navigatorHtml =  ResourcesReader.Read(assembly, prefix+"partial_navigator.html");
            var html =  welcomeHtml.Replace("${ct_css}", ct_css)
                .Replace("${navigator}", navigatorHtml);
            return ctx.Response.WriteAsync(html);
        }

        protected string Help_Get()
        {
            return "文档整理中，请上我们官网<a href=\"http://www.56x.net/cms\">www.ops.cc</a>查询相关信息！";
        }

        /// <summary>
        /// 项目列表
        /// </summary>
        protected string List_Get()
        {
            string prjsListHtml,
                //项目列表Html
                propertyHtml;


            StringBuilder sb = new StringBuilder();
            StringBuilder propertyStr = new StringBuilder();


            Project[] projects = this.director.GetProjects();
            if (projects == null)
            {
                sb.Append("<li>暂无采集项目![<a href=\"?action=create\">点击添加</a>]</li>");
            }
            else
            {
                foreach (Project prj in projects)
                {
                    //项目属性
                    foreach (string key in prj.Rules)
                    {
                        propertyStr.Append("[").Append(key).Append("],");
                    }
                    if (propertyStr.Length == 0)
                    {
                        propertyHtml = "无";
                    }
                    else
                    {
                        propertyHtml = propertyStr.Remove(propertyStr.Length - 1, 1).ToString();
                        propertyStr.Remove(0, propertyStr.Length);
                    }

                    //项目基本信息
                    sb.Append("<div class=\"project\"><h2><strong>项目名称：")
                        .Append(prj.Name)
                        .Append("</strong>(编号：")
                        .Append(prj.Id)
                        .Append(")</h2><p class=\"details\"> 编码方式：").Append(prj.RequestEncoding).Append("<br />列表规则：")
                        .Append(prj.ListUriRule).Append("<br />页面规则：")
                        .Append(prj.PageUriRule).Append("<br />采集属性：").Append(propertyHtml)
                        .Append("</p><span class=\"project_operate\">[&nbsp;<a href=\"?action=invoke&projectId=")
                        .Append(prj.Id).Append("\">开始采集</a>&nbsp;]&nbsp;&nbsp;[&nbsp;<a href=\"?action=edit&projectId=")
                        .Append(prj.Id).Append("\">修改</a>&nbsp;]&nbsp;&nbsp;[&nbsp;<a href=\"?action=delete&projectId=")
                        .Append(prj.Id).Append("\">删除</a>&nbsp;]</span></div>");
                }
            }

            prjsListHtml = sb.ToString();

            var html =  ResourcesReader.Read(assembly, prefix+"project_list.html");
            var html2 =  ResourcesReader.Read(assembly, prefix+"partial_navigator.html");

            return html.Replace("${ct_css}", ct_css)
                .Replace("${subtitle}", subtitle)
                .Replace("${navigator}", html2)
                .Replace("${listHtml}", prjsListHtml);
        }

        #region 创建项目

        protected string CreateProject_Get()
        {
            
            var html =  ResourcesReader.Read(assembly, prefix+"project_list.html");
            var html2 =  ResourcesReader.Read(assembly, prefix+"partial_navigator.html");
            return html.Replace("${ct_css}", ct_css)
                .Replace("${subtitle}", subtitle)
                .Replace("${navigator}",html2);
        }

        protected string CreateProject_Post()
        {
            bool result; //新建项目是否成功

            Project project = new Project();
            project.Rules = new PropertyRule();

            string id = request.Form["id"],
                name = request.Form["name"],
                encoding = request.Form["encoding"],
                listRule = request.Form["listRule"],
                blockRule = request.Form["listBlockRule"],
                pageRule = request.Form["pageRule"],
                filterRule = request.Form["filterWordsRule"];


#if DEBUG
            response.WriteAsync(HttpUtility.HtmlEncode(
                $"ID:{id}<br />Name:{name}\r\nListRule:{listRule}\r\nListBlockRule:{blockRule}\r\nPageRule:{pageRule}\r\nFilterRule:{filterRule}\r\nencoding:{encoding}"));

            response.WriteAsync("<br />");
#endif

            if (String.IsNullOrEmpty(id) || String.IsNullOrEmpty(name))
            {
                return "<script>window.parent.tip('编号或名称不能为空！');</script>";
            }


            project.Id = id;
            project.Name = name;
            project.RequestEncoding = encoding;
            project.ListUriRule = listRule;
            project.ListBlockRule = blockRule;
            project.PageUriRule = pageRule;
            project.FilterWordsRule = filterRule;


            //添加属性并赋值
            //客户端属性与规则匹配：p1 <-> r1
            Regex propertyNameRegex = new Regex("^p(\\d+)$");
            string propertyIndex; //属性编号

            foreach (var pair in request.Form)
            {
                if (propertyNameRegex.IsMatch(pair.Key))
                {
                    propertyIndex = propertyNameRegex.Match(pair.Key).Groups[1].Value;

                    //如果值不为空，则添加属性
                    if (request.Form[pair.Key] != String.Empty)
                    {
                        project.Rules.Add(request.Form[pair.Key], request.Form["r" + propertyIndex]);
                    }
                }
            }

            /*
            //输出添加到的属性
            foreach (string key in project.Rules)
            {
                response.Write(HttpContext.Current.Server.HtmlEncode(key + "->" + project.Rules[key]+"<br />"));
            }
             */

            result = this.director.CreateProject(project);

            //清除项目缓存
            this.director.ClearProjects();

            return result
                ? "<script>window.parent.tip('添加成功!');</script>"
                : "<script>window.parent.tip('项目已存在!');</script>";
        }

        #endregion

        #region 更新项目

        protected string Edit_Get()
        {
            string projectId = request.GetParameter("projectId");
            Project prj = this.director.GetProject(projectId);
            if (prj == null)
            {
                
                var html_ =  ResourcesReader.Read(assembly, prefix+"error.html");
                var html2_ =  ResourcesReader.Read(assembly, prefix+"partial_navigator.html");
                return html_.Replace("${ct_css}", ct_css)
                    .Replace("${subtitle}", subtitle)
                    .Replace("${navigator}", html2_)
                    .Replace("${msg}", "项目不存在");
            }

            StringBuilder sb = new StringBuilder();
            int i = 0;

            foreach (string key in prj.Rules)
            {
                ++i;
                sb.Append("<p><input type=\"text\" name=\"p").Append(i.ToString()).Append("\" value=\"")
                    .Append(key).Append("\"/><textarea name=\"r").Append(i.ToString()).Append("\" class=\"rulebox2\">")
                    .Append(prj.Rules[key]).Append("</textarea>")
                    .Append(
                        "<span class=\"removeProperty\">[&nbsp;<a href=\"javascript:;\" onclick=\"removeProperty(this)\">删除</a>&nbsp;]</span></p>");
            }

            
            var html =  ResourcesReader.Read(assembly, prefix+"update_project.html");
            var html2 =  ResourcesReader.Read(assembly, prefix+"partial_navigator.html");
            
            return html
                .Replace("${ct_css}", ct_css)
                .Replace("${subtitle}", subtitle)
                .Replace("${navigator}", html2)
                .Replace("${id}", projectId)
                .Replace("${name}", prj.Name)
                .Replace("${encoding}", prj.RequestEncoding)
                .Replace("${listUriRule}", prj.ListUriRule)
                .Replace("${listBlockRule}", prj.ListBlockRule)
                .Replace("${pageUriRule}", prj.PageUriRule)
                .Replace("${filterWordsRule}", prj.FilterWordsRule)
                .Replace("${propertiesHtml}", sb.ToString());
        }

        protected string Edit_Post()
        {
            string projectId = request.GetParameter("projectId");


            bool result; //编辑项目是否成功

            Project project = new Project();
            project.Rules = new PropertyRule();

            string id = request.Form["id"],
                name = request.Form["name"],
                encoding = request.Form["encoding"],
                listRule = request.Form["listRule"],
                blockRule = request.Form["listBlockRule"],
                pageRule = request.Form["pageRule"],
                filterRule = request.Form["filterWordsRule"];


#if DEBUG
            response.WriteAsync(HttpUtility.HtmlEncode(
                $"ID:{id}<br />Name:{name}\r\nListRule:{listRule}\r\nListBlockRule:{blockRule}\r\nPageRule:{pageRule}\r\nFilterRule:{filterRule}\r\nencoding:{encoding}"));

            response.WriteAsync("<br />");
#endif

            if (String.IsNullOrEmpty(id) || String.IsNullOrEmpty(name))
            {
                return "<script>alert('编号或名称不能为空！');</script>";
            }


            project.Id = id;
            project.Name = name;
            project.RequestEncoding = encoding;
            project.ListUriRule = listRule;
            project.ListBlockRule = blockRule;
            project.PageUriRule = pageRule;
            project.FilterWordsRule = filterRule;


            //添加属性并赋值
            //客户端属性与规则匹配：p1 <-> r1
            Regex propertyNameRegex = new Regex("^p(\\d+)$");
            string propertyIndex; //属性编号

            foreach (var pair in request.Form)
            {
                if (propertyNameRegex.IsMatch(pair.Key))
                {
                    propertyIndex = propertyNameRegex.Match(pair.Key).Groups[1].Value;

                    //如果值不为空，则添加属性
                    if (request.Form[pair.Key] != String.Empty)
                    {
                        project.Rules.Add(request.Form[pair.Key], request.Form["r" + propertyIndex]);
                    }
                }
            }

            /*
            //输出添加到的属性
            foreach (string key in project.Rules)
            {
                response.Write(HttpContext.Current.Server.HtmlEncode(key + "->" + project.Rules[key]+"<br />"));
            }
             */

            result = this.director.SaveProject(projectId, project);

            //清除项目缓存
            this.director.ClearProjects();

            return result
                ? "<script>window.parent.tip('修改成功!');</script>"
                : "<script>window.parent.tip('项目编号已存在!');</script>";
        }

        #endregion

        #region 删除项目

        protected string Delete_Get()
        {
            string projectId = request.GetParameter("projectId");
            string confirm = request.GetParameter("confirm");

            string msg; //返回信息


            Project prj = this.director.GetProject(projectId);
            if (prj == null)
            {
                msg = "<span style=\"color:red\">项目不存在!\"></span>";
            }


            if (String.IsNullOrEmpty(confirm))
            {
                msg = String.Format(
                    "您确定删除项目：<span style=\"color:red\">{0}</span>&nbsp;吗？<br /><a href=\"?action=delete&confirm=ok&projectid={1}\">确定</a>&nbsp;<a href=\"?action=list\">取消</a>"
                    , prj.Name, projectId);
            }
            else
            {
                msg = "项目删除成功！";

                this.director.RemoveProject(prj);

                //更新项目缓存
                this.director.ClearProjects();
            }

            var html =  ResourcesReader.Read(assembly, prefix+"delete_project.html");
            var html2 =  ResourcesReader.Read(assembly, prefix+"partial_navigator.html");


            return html
                .Replace("${ct_css}", ct_css)
                .Replace("${subtitle}", subtitle)
                .Replace("${navigator}", html2)
                .Replace("${msg}", msg);
        }

        #endregion

        /********************************************************
         * 
         *  采集说明:
         *  
         *  1.开始采集时读取继承类返回的Html代码，供重写Invoke方法读取某些参数！
         *  
         *  2.客户端通过设置HiddenField  [typeid]的值，来识别采集单页或是列表
         *    [typeid]:1:采集单页,2:传递列表页参数采集,3:输入列表页参数采集
         *    
         *  3.通过识别采集方式，来调用继承类的采集处理代码。执行完毕，想客户端传送
         *    采集完成指令!
         * 
         */

        /// <summary>
        /// 开始执行采集
        /// </summary>
        protected string Invoke_Get()
        {
            string projectId = request.GetParameter("projectId");

            Project prj = this.director.GetProject(projectId);
            if (prj == null)
            {
                return "<script>window.parent.tip('项目不存在!');</script>";
            }

            var html =  ResourcesReader.Read(assembly, prefix+"invoke_collect.html");
            var html2 =  ResourcesReader.Read(assembly, prefix+"partial_navigator.html");

            return html
                .Replace("${ct_css}", ct_css)
                .Replace("${subtitle}", subtitle)
                .Replace("${navigator}", html2)
                .Replace("${customHtml}", Return_InvokePageHtml())
                .Replace("${pageUriRule}", HttpUtility.HtmlEncode(prj.PageUriRule))
                .Replace("${listUriRule}", HttpUtility.HtmlEncode(prj.ListUriRule)
                    .Replace("{0}", "<span style=\"color:red\">{0}</span>"))
                .Replace("${listBlockRule}", HttpUtility.HtmlEncode(prj.ListBlockRule));
        }

        protected string Invoke_Post()
        {
            string typeID = request.Form["ct_typeid"];
            string projectId = request.GetParameter("projectId");

            //执行采集返回的数据
            string invoke_returnData = String.Empty;

            Project prj = this.director.GetProject(projectId);
            if (prj == null)
            {
                return "<script>window.parent.tip('项目不存在!');</script>";
            }

            bool isReverse = request.Form["reverse"] == "true" || request.Form["reverse"] == "on";

            switch (typeID)
            {
                case "1":
                    invoke_returnData = Invoke_SinglePage(prj, request.Form["singlePageUri"]);
                    break;
                case "2":
                    invoke_returnData = Invoke_ListPage(prj, isReverse, (object)request.Form["listPageParameter"]);
                    break;
                case "3":
                    invoke_returnData = Invoke_ListPage(prj, isReverse, request.Form["listPageUri"]);
                    break;
            }

            return "<script>window.parent.invoke_compelete('" + invoke_returnData + "')</script>";
        }

        /// <summary>
        /// 返回一段Html代码，并呈现在采集页面上
        /// 如：返回一段分类的标签，并在采集中读取选中的分类，从而实现将采集的内容发布到指定分类中
        /// </summary>
        public virtual string Return_InvokePageHtml()
        {
            return "您好，欢迎使用采集系统!<br />";
        }


        /// <summary>
        /// 采集单页,并返回提示数据
        /// </summary>
        /// <param name="project"></param>
        /// <param name="pageUri"></param>
        public virtual string Invoke_SinglePage(Project project, string pageUri)
        {
            project.InvokeSingle(pageUri, dp =>
            {
#if DEBUG
                saveLog("\r\n----------------------------------------\r\n标题："+dp["title"] + "<br />\r\n内容：" + dp["content"]+"\r\n");
#endif
            });

            //重置计数
            project.ResetState();

            return null;
        }

        ///  <summary>
        /// 根据列表页的地址采集页面,并返回提示数据
        ///  </summary>
        ///  <param name="project"></param>
        /// <param name="reverse"></param>
        /// <param name="listPageUri"></param>
        ///  <returns></returns>
        public virtual string Invoke_ListPage(Project project, bool reverse, string listPageUri)
        {
            string returnData;

            int i = 0;
            object obj = String.Empty;

            project.UseMultiThread = true;

            project.InvokeList(listPageUri, reverse, dp =>
            {
                lock (obj)
                {
                   // ++i;
#if DEBUG
                    saveLog(String.Format("采集到第{0}条->{1}", i, dp["title"]));
#endif
                }
            });

            returnData = String.Format("任务总数:{0},成功：{1},失败:{2}", project.State.TotalCount, project.State.SuccessCount + 1,
                project.State.FailCount);

            //重置计数
            project.ResetState();

            return returnData;
        }

        /// <summary>
        /// 向列表页规则传递参数，并返回提示数据
        /// </summary>
        /// <param name="project"></param>
        /// <param name="reverse"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public virtual string Invoke_ListPage(Project project, bool reverse, object parameter)
        {
            string returnData;

            int i = 0;
            object obj = String.Empty;

            project.UseMultiThread = true;

            project.InvokeList(parameter, reverse, dp =>
            {
                lock (obj)
                {
                    ++i;
#if DEBUG
                    saveLog(String.Format("采集到第{0}条->{1}", i, dp["title"]));
#endif
                }
            });

            returnData = String.Format("任务总数:{0},成功：{1},失败:{2}", project.State.TotalCount, project.State.SuccessCount,
                project.State.FailCount);

            //重置计数
            project.ResetState();

            return returnData;
        }

        protected void saveLog(string str)
        {
            using (
                System.IO.StreamWriter sr =
                    new System.IO.StreamWriter(EnvUtil.GetBaseDirectory() + "collection.log", true))
            {
                sr.WriteLine(str);
                sr.Flush();
                sr.Dispose();
            }
        }

        /// <summary>
        /// 向客户端发送提示信息
        /// </summary>
        /// <param name="msg"></param>
        protected void SendTipMessage(string msg)
        {
            response.WriteAsync(String.Format("<script type=\"text/javascript\">window.parent.tip('{0}');</script>", msg));
        }
    }
}