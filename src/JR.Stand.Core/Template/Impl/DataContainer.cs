//
// Copyright 2011 @ S1N1.COM,All right reserved.
// Name:TemplateUtility.cs
// Author:newmin
// Create:2013/09/05
//

using System;
using System.Collections.Generic;

namespace JR.Stand.Core.Template.Impl
{
    /// <summary>
    /// 基本数据容器
    /// </summary>
    public class BasicDataContainer : IDataContainer
    {
        private readonly IDataAdapter _adapter;

        public BasicDataContainer(IDataAdapter a)
        {
            this._adapter = a;
        }

        public IDictionary<string, Object> GetDefineVariable()
        {
            IDictionary<string, Object> dict = null;
            var obj = this._adapter.GetItem("__tpl_var_define__");
            if (obj != null)
            {
                dict = obj as IDictionary<string, object>;
            }

            if (dict == null)
            {
                dict = new Dictionary<string, object>();
                this._adapter.SetItem("__tpl_var_define__", dict);
            }

            return dict;
        }

        public IDataAdapter GetAdapter()
        {
            return this._adapter;
        }

        public string GetTemplatePageCacheContent(string templateId)
        {
            if (TemplateCache.templateDictionary.ContainsKey(templateId))
            {
                return this._adapter.GetCache("__tpl_cache_" + templateId) as string;
            }

            return null;
        }

        public void SetTemplatePageCacheContent(string templateId, string content, string dependFileName)
        {
            this._adapter.InsertCache("__tpl_cache_" + templateId, content, 30 * 1440 * 60, dependFileName);
            // HttpRuntime.Cache.Insert("tpl_cache_" + templateId, content,
            //     new System.Web.Caching.CacheDependency(dependFileName), DateTime.Now.AddDays(30), TimeSpan.Zero);
        }

        public void DefineVariable<T>(string key, T value)
        {
            if (value == null) return; //防止非法参数
            var dict = this.GetDefineVariable();
            /*
            IDictionary<string, object> varDict;
            object obj = HttpContext.Current.Items["__tpl_var_define__"];
            if (obj != null)
            {
                varDict = obj as IDictionary<string, object>;
                if (varDict.Keys.Contains(key))
                {
                    throw new ArgumentException("模板变量已定义。", key);
                }
            }
            else
            {
                varDict = new Dictionary<string, object>();
                HttpContext.Current.Items["__tpl_var_define__"] = varDict;
            }*/

            if (dict.Keys.Contains(key))
            {
                throw new ArgumentException("模板变量已定义。", key);
            }

            //如果不是基元类型，则保存类型
            Type t = typeof(T);
            if (t == typeof(String) || t.IsPrimitive)
            {
                dict.Add(key, value);
            }
            else
            {
                dict.Add(key, new Variable {Key = key, Value = value, Type = t});
            }

            this._adapter.SetItem("__tpl_var_define__", dict);
        }

        public void DefineVariable(string key, Variable variable)
        {
            var dict = this.GetDefineVariable();
            if (dict.Keys.Contains(key))
            {
                throw new ArgumentException("模板变量已定义。", key);
            }

            variable.Key = key;
            dict.Add(key, variable);
            this._adapter.SetItem("__tpl_var_define__", dict);
        }

        public object GetVariable(string key)
        {
            var dict = this.GetDefineVariable();
            if (dict.Keys.Contains(key))
            {
                return dict[key];
            }
            return null;
        }
    }
}