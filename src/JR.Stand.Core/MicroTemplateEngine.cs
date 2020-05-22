/*
* Copyright(C) 2010-2012 S1N1.COM
* 
* File Name	: SimpleTemplateEngine
* Author	: Administrator
* Create	: 2012/10/26 23:49:52
* Description	:
*
*/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace JR.Stand.Core
{
    /// <summary>
    /// 模板类接口
    /// </summary>
    public interface ITemplateClass
    {
        /// <summary>
        /// 解析模板函数
        /// </summary>
        /// <param name="fn"></param>
        /// <param name="paramArray"></param>
        /// <returns></returns>
        String Execute(string fn, object[] paramArray);
        /// <summary>
        /// 更新引擎,通过存储模板类与引擎的引用
        /// </summary>
        /// <param name="engine"></param>
        void UpdateEngine(MicroTemplateEngine engine);
    }

    /// <summary>
    /// 基于反射的模板类型
    /// </summary>
    public abstract class ReflectTemplateClass : ITemplateClass
    {
        private MicroTemplateEngine _engine;
        private readonly Dictionary<string, MethodInfo> _fnMap;
        /// <summary>
        /// 初始化函数
        /// </summary>
        public ReflectTemplateClass()
        {
            this._fnMap = new Dictionary<String, MethodInfo>();
            BindingFlags fnFlag = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.IgnoreCase;
            Type type = this.GetType();
            foreach (MethodInfo mi in type.GetMethods(fnFlag))
            {
                String name = mi.Name;
                switch (name)
                {
                    case "ToString":
                    case "Execute":
                    case "UpdateEngine":
                    case "GetEngine":
                    case "SetBinderFlag":
                    case "Equals":
                    case "Finalize":
                    case "GetHashCode":
                    case "GetType":
                    case "MemberwiseClone":
                        continue;
                }
                int paraLen = mi.GetParameters().Length;
                this._fnMap[$"{name.ToLower()}#{paraLen}"] = mi;
            }
        }

        /// <summary>
        /// 执行模板方法
        /// </summary>
        /// <param name="fn"></param>
        /// <param name="paramArray"></param>
        /// <returns></returns>
        public virtual string Execute(string fn, object[] paramArray)
        {
            String key = $"{fn.ToLower()}#{paramArray.Length}";
            // 不存在方法
            if (!this._fnMap.ContainsKey(key)) return null;
            // 参数类型数组
            Type[] parameterTypes = new Type[paramArray.Length];
            //查找是否存在方法(方法参数均为string类型)
            for (int i = 0; i < parameterTypes.Length; i++) parameterTypes[i] = typeof(String);
            MethodInfo mi = this._fnMap[key];
            // 执行方法并返回结果
            try
            {
                return mi.Invoke(this, paramArray).ToString();
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message + "; fn = " + key);
            }
        }

        /// <summary>
        /// 更新引擎
        /// </summary>
        /// <param name="engine"></param>
        public void UpdateEngine(MicroTemplateEngine engine)
        {
            this._engine = engine;
        }

        /// <summary>
        /// 获取引擎
        /// </summary>
        /// <returns></returns>
        protected MicroTemplateEngine GetEngine()
        {
            return this._engine;
        }
    }

    /// <summary>
    /// 微型模板引擎
    /// </summary>
    public sealed class MicroTemplateEngine
    {
        /// <summary>
        /// 包含方法的类型实例
        /// </summary>
        private readonly ITemplateClass _classInstance;

        /// <summary>
        /// 创建模板引擎实例
        /// </summary>
        /// <param name="classInstance"></param>
        public MicroTemplateEngine(ITemplateClass classInstance)
        {
            if(classInstance != null)
            {
                this._classInstance = classInstance;
                this._classInstance.UpdateEngine(this);
            }
        }

        /// <summary>
        /// 数据列正则
        /// </summary>
        //private static Regex fieldRegex = new Regex("{([A-Za-z\\[\\]0-9_\u4e00-\u9fa5]+)}");
        
        // 占位正则
        private static readonly Regex HolderRegex = new Regex("{([A-Za-z\u4e00-\u9fa5]+[^}]+?)}");
        // 字段正则
        private static readonly Regex FieldRegex = new Regex("\\${([A-Za-z\u4e00-\u9fa5]+[^}]+?)}");

        // 方法正则
        private static readonly Regex FnRegex = new Regex("\\$([A-Za-z_0-9\u4e00-\u9fa5]+)\\(([^)]*)\\)");
        // 参数正则
        private static readonly Regex paramRegex = new Regex("\\s*'([^']+)',*|\\s*(?!=')([^,]+),*");
        /// <summary>
        /// 执行解析模板内容
        /// </summary>
        /// <param name="instance">包含标签方法的类的实例</param>
        /// <param name="html"></param>
        /// <returns></returns>
        private static string Execute(ITemplateClass instance, string html)
        {
            return FnRegex.Replace(html, m =>
            {
                // 方法名称
                String fnName = m.Groups[1].Value;
                // 获得参数
                MatchCollection  paramMatches = paramRegex.Matches(m.Groups[2].Value);
                object[] paramArray = new object[paramMatches.Count];
                // 则给参数数组赋值
                for (int i = 0; i < paramMatches.Count; i++)
                {            
                    // 数字参数
                    string intParamValue = paramMatches[i].Groups[2].Value;
                    if (intParamValue != String.Empty)  paramArray[i] = intParamValue;
                    else  paramArray[i] = paramMatches[i].Groups[1].Value;
                }
                // 解析模板数据
                var s = instance.Execute(fnName, paramArray);
                return s ?? m.Value;
            });
        }
        

        /// <summary>
        /// 替换列中的模板占位字符
        /// </summary>
        /// <param name="format"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public string ResolveHolderFields(string format, Func<string, string> func)
        {
            return HolderRegex.Replace(format, a => func(a.Groups[1].Value));
        }
        
        /// <summary>
        /// 替换列中的模板数据
        /// </summary>
        /// <param name="format"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public string ResolveFields(string format, Func<string, string> func)
        {
            return FieldRegex.Replace(format, a => func(a.Groups[1].Value));
        }


        /// <summary>
        /// 执行解析模板内容
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public string Execute(string html)
        {
            return Execute(this._classInstance, html);
        }
    }
}