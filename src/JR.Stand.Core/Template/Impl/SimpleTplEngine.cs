/*
* Copyright(C) 2010-2012 S1N1.COM
* 
* File Name	: SimpleTemplateEngine
* Author	: Administrator
* Create	: 2012/10/26 23:49:52
* Description	:
* Mofify:
*   2013-04-11  10:29   newmin [+]: 添加TemplateAttribute特性判断是否为返回数据， 返回数据的方法不再限制为私有
*   2013-04-23  22:45   newmin [+]: 支持私有，公共，受保护的成员调用
*   2013-04-23  22:46   newmin [+]: 支持int参数，如$tpl(3,'c,a,r')
*   2013-04-26  17:15   newmin [+]: 支持method   
*/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace JR.Stand.Core.Template.Impl
{
    /// <summary>
    /// 简单模板解析类
    /// </summary>
    public sealed class SimpleTplEngine
    {
        /// <summary>
        /// 包含方法的类型实例
        /// </summary>
        private readonly object classInstance;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classInstance"></param>
        /// <param name="counter"></param>
        public SimpleTplEngine(object classInstance, bool counter)
        {
            this.classInstance = classInstance;
            if (counter)
            {
                this.Count = new List<String>();
            }
        }

        /// <summary>
        /// 计数
        /// </summary>
        public IList<String> Count;


        /// <summary>
        /// 数据列正则
        /// </summary>
        private static readonly Regex FieldRegex = new Regex("{([a-z0-9_\\]\\[\u4e00-\u9fa5]+)}");

        /// <summary>
        /// 编译模版方法
        /// </summary>
        /// <param name="html"></param>
        private void CompileTemplateMethod(ref string html)
        {
            //返回结果
            const string methodPattern = "\\#begin\\s([A-Za-z_0-9\u4e00-\u9fa5]+)\\(([^)]*)\\)\\s*([\\S\\s]+)#end";
            //#begin each(id){ ${id} } #end
            const string paramPattern = "\\s*'([^']*)',*|\\s*(?!=')([^,]+),*";

            Regex methodRegex = new Regex(methodPattern); //方法正则

            //如果不包括方法,则直接返回
            if (!methodRegex.IsMatch(html))return;
            Regex paramRegex = new Regex(paramPattern); //参数正则

            Type type = this.classInstance.GetType();
            MethodInfo method;
            string tagName;
            string tagFormat;
            object[] parameters;
            Type[] parameterTypes; //参数类型数组
            MatchCollection paramMcs;


            html = methodRegex.Replace(html, m =>
            {
                tagName = m.Groups[1].Value;
                tagFormat = m.Groups[3].Value;

                //获得参数
                paramMcs = paramRegex.Matches(m.Groups[2].Value);

                //参数,多添加一个tagFormat参数
                parameters = new object[paramMcs.Count + 1];

                //查找是否存在方法(方法参数均为string类型)
                parameterTypes = new Type[parameters.Length];
                for (int i = 0; i < parameterTypes.Length; i++)
                {
                    parameterTypes[i] = typeof(String);
                }

                method = type.GetMethod(
                    tagName,
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.IgnoreCase,
                    null,
                    parameterTypes,
                    null);


                //如果方法存在且包含template特性，则执行返回结果，否则返回原始值
                if (method == null || method.GetCustomAttributes(typeof(TemplateMethodAttribute), true).Length == 0)
                {
                    return m.Value;
                }
                //数字参数
                string intParamValue;
                //则给参数数组赋值
                for (int i = 0; i < paramMcs.Count; i++)
                {
                    intParamValue = paramMcs[i].Groups[2].Value;
                    if (intParamValue != String.Empty)
                    {
                        parameters[i] = intParamValue;
                    }
                    else
                    {
                        parameters[i] = paramMcs[i].Groups[1].Value;
                    }
                }

                parameters[parameters.Length - 1] = tagFormat;

                if (this.Count != null)
                    this.Count.Add(String.Format("Method:{0},{1:mmssfff}", method.Name, DateTime.Now));

                //执行方法并返回结果
                return method.Invoke(this.classInstance, parameters).ToString();
            });
        }

        /// <summary>
        /// 执行解析模板内容
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private void CompileTemplateTag(ref string html)
        {
            //返回结果
            const string tagPattern = "\\$([a-z_0-9\u4e00-\u9fa5]+)\\(([^)]*)\\)";
            const string paramPattern = "\\s*(((\\\\,)|[^,])+),*";
            //const string paramPattern = "\\s*'([^']*)',*|\\s*(?!=')([^,]+),*";

            var tagRegex = new Regex(tagPattern);

            if (!tagRegex.IsMatch(html))
            {
                return;
            }

            var paramRegex = new Regex(paramPattern);

            Type type = this.classInstance.GetType();
            MethodInfo method;
            string tagName;
            object[] parameters;
            Type[] parameterTypes; //参数类型数组
            MatchCollection paramMcs;

            html = tagRegex.Replace(html, m =>
            {
                tagName = m.Groups[1].Value;

                //获得参数
                paramMcs = paramRegex.Matches(m.Groups[2].Value);
                //
                // foreach (Match m3 in paramMcs)
                // {
                //     Console.WriteLine("-------");
                //     Console.WriteLine(m3.Value);
                // }

                //参数
                parameters = new object[paramMcs.Count];

                //查找是否存在方法(方法参数均为string类型)
                parameterTypes = new Type[parameters.Length];
                for (int i = 0; i < parameterTypes.Length; i++)
                {
                    parameterTypes[i] = typeof(String);
                }

                method = type.GetMethod(
                    tagName,
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.IgnoreCase,
                    null,
                    parameterTypes,
                    null);


                //如果方法存在且包含template特性，则执行返回结果，否则返回原始值
                if (method == null || method.GetCustomAttributes(typeof(TemplateTagAttribute), true) == null)
                {
                    return m.Value;
                }

                //数字参数
                //则给参数数组赋值
                for (int i = 0; i < paramMcs.Count; i++)
                {
                    var paramGroupValue = paramMcs[i].Groups[1].Value.Trim();
                    var value = this.GetFunctionParamValue(paramGroupValue);
                    //Console.WriteLine("---value:");
                    //Console.WriteLine(value);
                    parameters[i] = value;
                }

                Count?.Add($"Tag:{method.Name},{DateTime.Now:mmssfff}");

                //执行方法并返回结果
                return method.Invoke(this.classInstance, parameters).ToString();
            });
        }

        private string GetFunctionParamValue(string value)
        {
            var len = value.Length;
            if (len == 0) return value;
            if (value[0] == '\'')
            {
                if(len == 1 || value[len-1]!='\'')throw new TemplateException("参数末尾应包含\"'\"");
                return value.Substring(1, len - 2);
            }
            if (value[0] == '\"')
            {
                if(len == 1 || value[len-1]!='\"')throw new TemplateException("参数末尾应包含\"");
                return value.Substring(1, len - 2);
            } 
            if (value[0] == '{')
            {
                if(len == 1 || value[len-1]!='}')throw new TemplateException("参数末尾应包含\"}\"");
                return value.Substring(1, len - 2);
            }
            return value;
        }

        /// <summary>
        /// 执行解析模板内容
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public string Execute(string html)
        {
            string resultHtml = html;
            this.CompileTemplateMethod(ref resultHtml);
            this.CompileTemplateTag(ref resultHtml);
            return resultHtml;
        }

        /// <summary>
        /// 替换列中的模板字符
        /// </summary>
        /// <param name="format"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static string FieldTemplate(string format, Func<string, string> func)
        {
            return FieldRegex.Replace(format, a => { return func(a.Groups[1].Value); });
        }
    }
}