//
// Copyright 2011 @ S1N1.COM,All right reserved.
// Name:TemplateUtility.cs
// Author:newmin
// Create:2013/09/05
//

using JR.Stand.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace JR.Stand.Core.Template.Impl
{
    public static class Eval
    {
        /// <summary>
        /// 添加到数据项
        /// </summary>
        /// <param name="expression"></param>
        public static void ToItem(string expression)
        {
            //$item:x=123
        }

        /// <summary>
        /// 赋值到变量
        /// $v = form('key')
        /// </summary>
        private static string SetToVariable(IDataContainer dc, string content)
        {
            //正则模式，支持以下
            // //$menu="123456\" f" sdf
            // $menu=12 fsdf
            // $menu=item:123456
            // $menu=>getmenu(test,item,get)
            const string expressionPattern = "(/*/*|#*)\\$([_a-zA-Z][a-zA-Z0-9_]*)\\s*=\\s*(\"(\"|[^\"\\n])*\"|[^<>\\s\\n\"\\$/]+)(\\s+\\B)*";
            //const string expressionPattern = "(/*/*|#*)\\$([_a-zA-Z][a-zA-Z0-9_]*)\\s*=\\s*([^()]+)\\(((\\{[^}]*\\})|([^)]*))\\)";

            //设置表达式
            //const string specialVarPattern = "(item|cache|query|form)\\([\"']*(.+?)[\"']*\\)"; //特殊数据

            var outHtml = Regex.Replace(content, expressionPattern, m =>
            {
                //注释
                if (m.Groups[1].Value != "")
                {
                    return String.Empty;
                }


                //获取变量及表达式
                var varName = m.Groups[2].Value;
                var expName = m.Groups[3].Value;
                var expValue = TemplateUtils.GetFunctionParamValue(m.Groups[4].Value);
                var value = String.Empty;
                switch (expName)
                {
                    case "item":
                        value = dc.GetAdapter().GetItem(expValue).ToString();
                        break;
                    case "cache":
                        value = dc.GetAdapter().GetCache(expValue).ToString();
                        break;
                    case "query":
                        value = dc.GetAdapter().GetQueryParam(expValue);
                        break;
                    case "form":
                        value = dc.GetAdapter().GetFormParam(expValue);
                        break;
                    default:
                        value = expName;
                        break;
                }

                dc.DefineVariable(varName, value ?? "");

                /*
            }
            else
            {
                string varRealValue = Regex.Replace(expValue, "\\B\"|\"\\B", String.Empty);

                //如果为字符,否则读取指定值的变量
                if (Regex.IsMatch(expValue, "\\B\"|\"\\B"))
                {
                    dc.DefineVariable(varName, varRealValue);
                }
                else
                {
                    object obj = dc.GetVariable(varRealValue);
                    if (obj != null)
                    {
                        if (obj is Variable)
                        {
                            dc.DefineVariable(varName, (Variable) obj);
                        }
                        else
                        {
                            dc.DefineVariable(varName, obj.ToString());
                        }
                    }
                    else
                    {
                        string message = "";
                        int i = 0;
                        foreach (string key in dc.GetDefineVariable().Keys)
                        {
                            message += (++i == 1 ? "" : "," + key);
                        }
                        throw new NotSupportedException("数据引用键错误:" + m.Value + "\n"
                                                        +
                                                        (message != ""
                                                            ? "受支持可引用的数据键包括" + message + "\n使用\"$" + varName +
                                                              "=>键\"进行调用！"
                                                            : ""));
                    }
                }
            }
            */

                return String.Empty;
            }, RegexOptions.Singleline);
            return outHtml;
        }

        private static readonly IDictionary<String, MethodInfo> EvalMethodMap = new Dictionary<string, MethodInfo>();

        /// <summary>
        /// 执行方法并将返回值赋予变量
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dc"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private static string EvalMethodToVar(IDataContainer dc, string content, object data)
        {
            //正则模式，支持以下
            // //$menu="123456\" f" sdf
            // $menu=12 fsdf
            // $menu=item:123456
            // $menu=>getmenu(test,item,get)
            const string expressionPattern =
                "(/*/*)\\$([a-zA-Z][a-zA-Z0-9_]*)\\s*=>\\s*([a-zA-Z][a-zA-Z0-9_]*)\\((([^\\)]|\\\\\\))*)\\)(\\s+\\B)*"; //设置表达式

            string varName,
                methodName,
                paramArray;

            Type type = data.GetType();
            MethodInfo method;
            Type[] parameterTypes;
            object[] parameters;
            int parametersNum;


            var outHtml = Regex.Replace(content, expressionPattern, m =>
            {
                //注释
                if (m.Groups[1].Value != "") return String.Empty;
                //获取变量及表达式
                varName = m.Groups[2].Value;
                methodName = m.Groups[3].Value;
                paramArray = m.Groups[4].Value.Replace("\\,", "__CSP__");

                parameters = paramArray.Trim().Length == 0 ? new object[0] : paramArray.Split(',');
                parametersNum = parameters.Length;

                //查找是否存在方法(方法参数均为string类型)
                parameterTypes = new Type[parametersNum];
                for (int i = 0; i < parametersNum; i++)
                {
                    parameterTypes[i] = typeof(String);
                    parameters[i] = Regex.Replace(parameters[i].ToString(), "\\B\"|\"\\B", String.Empty).Replace("__CSP__", ",");
                }

                string key = type.FullName + ":" + methodName+"$"+parameters.Length;
                if (EvalMethodMap.ContainsKey(key))
                {
                    method = EvalMethodMap[key];
                }
                else
                {
                    var flag = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public |
                               BindingFlags.IgnoreCase;
                    method = type.GetMethod(methodName,flag, null, parameterTypes, null);
                    EvalMethodMap[key] = method;
                }

                if (method != null)
                {
                    Type returnType = method.ReturnType;
                    if (returnType != typeof(void))
                    {
                        object result = method.Invoke(data, parameters);
                        if (result != null)
                        {
                            if (returnType.IsPrimitive || returnType == typeof(String))
                            {
                                dc.DefineVariable(varName, (result ?? "").ToString());
                            }
                            else
                            {
                                dc.DefineVariable(varName, new Variable {Key = varName, Value = result, Type = returnType});
                            }
                        }
                    }
                }

                /*
                else
                {
                    //throw new MissingMethodException("在对象中找不到");

                    Config.DC.DefineVar(varName, "<span style=\"color:red\">在对象中找不到方法<b>"+methodName+"</b>"+
                        (parametersNum==0?"":"，参数"+parametersNum.ToString()+"个")
                        +"。</span>");
                }
                */

                return String.Empty;
            }, RegexOptions.Singleline);
            return outHtml;
        }

        /// <summary>
        /// 执行方法并将返回值赋予变量
        /// </summary>
        /// <param name="data"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [Obsolete]
        public static string EntityVariable(IDataContainer dc, string content)
        {
            //正则模式，支持以下
            // //$menu="123456\" f" sdf
            // $menu=12 fsdf
            // $menu=item:123456
            // $menu=>getmenu(test,item,get)
            const string expressionPattern = "(/*/*)\\$([a-zA-Z][a-zA-Z0-9_]*)=>([a-zA-Z0-9_]+)(\\s+\\B)*"; //设置表达式

            string outHtml = content,
                varName,
                entityName;


            outHtml = Regex.Replace(content, expressionPattern, m =>
            {
                //注释
                if (m.Groups[1].Value != "") return String.Empty;

                //获取变量及表达式
                varName = m.Groups[2].Value;
                entityName = m.Groups[3].Value;

                object obj = dc.GetVariable(entityName);
                if (obj != null && obj is Variable)
                {
                    Variable var = (Variable) obj;
                    obj = var.Value;
                    Type type = var.Type;

                    //解析属性的值
                    PropertyInfo[] pros = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (PropertyInfo pro in pros)
                    {
                        outHtml = Regex.Replace(outHtml,
                            String.Format("\\$\\{0}{1}\\.{1}{2}", "\\{", varName, pro.Name, "\\}"),
                            m2 => { return (pro.GetValue(obj, null) ?? "").ToString(); }, RegexOptions.IgnoreCase);
                    }
                }

                return String.Empty;
            }, RegexOptions.Singleline);
            return outHtml;
        }


        public static string Compile(IDataContainer dc, string html, object data)
        {
            //======= 设置变量 ======//
            var outHtml = SetToVariable(dc, html);
            //======= 求方法 =======//
            if (data != null)
            {
                outHtml = EvalMethodToVar(dc, outHtml, data);
            }

            // outHtml = EntityVariable(dc, outHtml);
            return outHtml;
        }

        internal static string ResolveEntityProperties(IDataContainer dc, string templateHtml)
        {
            const string keyPattern = "\\$\\{([a-zA-Z][a-zA-Z0-9_]*)\\.([A-Z_a-z][a-zA-Z0-9_]*)\\}";
            IDictionary<string, string> entityKeys = new Dictionary<string, string>();
            IDictionary<string, Variable> entityValues = new Dictionary<string, Variable>();

            string entityName, proName;
            Variable var;
            PropertyInfo pro;
            object varValue;

            templateHtml = Regex.Replace(templateHtml, keyPattern, m =>
            {
                entityName = m.Groups[1].Value;
                proName = m.Groups[2].Value;

                if (!entityValues.Keys.Contains(entityName))
                {
                    varValue = dc.GetVariable(entityName);
                    entityValues.Add(entityName, varValue != null ? (Variable) varValue : default(Variable));
                }

                var = entityValues[entityName];
                if (var.Value != null)
                {
                    //解析属性的值
                    pro = var.Type.GetProperty(entityName,
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (pro != null)
                    {
                        return (pro.GetValue(var.Value, null) ?? "").ToString();
                    }

                    string message = "";
                    int i = 0;
                    foreach (
                        PropertyInfo p in
                        var.Type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                               BindingFlags.IgnoreCase))
                    {
                        message += (++i == 1 ? "" : ",") + p.Name;
                    }

                    throw new NotSupportedException("不支持的属性调用${" + var.Key + "." + proName + "}\n" + var.Key +
                                                    "支持可选的属性：" + message + "\n使用\"${" + var.Key + ".属性}\"进行调用！");
                }

                return String.Empty;
            });
            return templateHtml;
        }

        private static readonly IDictionary<String,PropertyInfo> PropertiesMap = new Dictionary<string, PropertyInfo>();

        /// <summary>
        /// 解析变量
        /// </summary>
        /// <param name="templateHtml"></param>
        /// <param name="variable"></param>
        /// <returns></returns>
        internal static string ResolveVariable(string templateHtml, Variable variable)
        {
            if (variable.Value == null) return templateHtml;
            //
            // ${obj.name};
            // 字典方式:
            // ${obj.data(key)}
            //
            // 字典方式获取，如果为空，使用默认值：
            //   ${obj.data(key) | default }
            //
            // 不支持的属性，默认以_开头
            // a-z下划线或中文开头
            //
            string keyPattern = "\\$\\{" + variable.Key 
                + "\\.([A-Z_a-z\u4e00-\u9fa5][a-zA-Z0-9_\u4e00-\u9fa5]*|data\\(([^\\)]+)\\))"+
                "\\s*(\\|([^}]+)\\s)*"+
                "\\}";
            string proName;
            PropertyInfo pro;
            IDictionary<string, string> propDict = null;
            templateHtml = Regex.Replace(templateHtml, keyPattern, m =>
            {
                proName = m.Groups[1].Value;
                if (proName.StartsWith("data(")) proName = "data";
                string key = variable.Type.FullName + ":" + variable.Key + "$" + proName;
                // 获取${obj.data(key) | default }表达式中，default部分的默认值
                String defaultValue = m.Groups[4].Value ?? "";
                // 尝试获取属性
                if (PropertiesMap.ContainsKey(key))
                {
                    pro = PropertiesMap[key];
                }
                else
                {
                    //解析属性的值
                    pro = variable.Type.GetProperty(proName,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
                    PropertiesMap[key] = pro;
                }
                #region 获取字典
                if (proName == "data")
                {
                    if (propDict == null)
                    {
                        if (pro == null)
                        {
                            throw new TypeLoadException($"类型{variable.Type.FullName}未定义字典");
                        }
                        propDict = pro.GetValue(variable.Value, null) as IDictionary<string, string>;
                        if (propDict == null)
                        {
                            throw new TypeLoadException("data属性的类型应为IDictionary<string,string>");
                        }
                    }
                    //获取值
                    string dictKey = m.Groups[2].Value.Replace("\"","").Replace("'","");
                    if (propDict.ContainsKey(dictKey))
                    {
                        return propDict[dictKey].EmptyElse(defaultValue);
                    }
                    return defaultValue; //字典不存在值
                }
                #endregion

                // 普通属性
                if (pro != null)
                {
                    Object obj = pro.GetValue(variable.Value, null);
                    if (obj == null) return defaultValue;
                    return obj.ToString().EmptyElse(defaultValue);
                }
                string message = "";
                int i = 0;
                foreach (PropertyInfo p in variable.Type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase))
                {
                    if (!p.Name.StartsWith("_"))
                    {
                        var attr = (TemplateVariableFieldAttribute[])
                            p.GetCustomAttributes(typeof(TemplateVariableFieldAttribute), true);
                        //message += (++i == 1 ? "" : ",") + p.Name;
                        message += (++i == 1 ? "\n=================================\n" : "\n") + p.Name +
                                   "\t : \t" +
                                   (attr.Length > 0 ? attr[0].Descript : "");
                    }
                }
                throw new NotSupportedException("不支持的属性调用${" + variable.Key + "." + proName + "}\n\n" +
                                                variable.Key +
                                                "支持下列可选属性：" + message + "\n\n注：使用\"${" + variable.Key +
                                                ".属性}\"进行调用，属性不区分大小写。");
            });
            return templateHtml;
        }
    }
}