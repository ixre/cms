//
// Copyright 2011 @ S1N1.COM,All right reseved.
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
        private readonly IDictionary<string, object> _varDict;
        private readonly IDataAdapter _adapter;
        public BasicDataContainer(IDataAdapter a)
        {
            IDataAdapter adapter;
            this._adapter = a;
            var obj = this._adapter.GetItem("__tpl_var_define__");
            if (obj != null)
            {
                this._varDict = obj as IDictionary<string, object>;
            }
            else
            {
                this._varDict = new Dictionary<string, object>();
                this._adapter.SetItem("__tpl_var_define__",_varDict);
            }
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
            this._adapter.InsertCache("__tpl_cache_" + templateId, content,30*1440*60,dependFileName);
            // HttpRuntime.Cache.Insert("tpl_cache_" + templateId, content,
            //     new System.Web.Caching.CacheDependency(dependFileName), DateTime.Now.AddDays(30), TimeSpan.Zero);
        }

        public void DefineVariable<T>(string key, T value)
        {
            if (value == null) return; //防止非法参数

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

            if (_varDict.Keys.Contains(key))
            {
                throw new ArgumentException("模板变量已定义。", key);
            }

            //如果不是基元类型，则保存类型
            Type t = typeof (T);
            if (t == typeof (String) || t.IsPrimitive)
            {
                _varDict.Add(key, value);
            }
            else
            {
                _varDict.Add(key, new Variable {Key = key, Value = value, Type = t});
            }
            this._adapter.SetItem("__tpl_var_define__",_varDict);
        }

        public void DefineVariable(string key, Variable variable)
        {
            if (_varDict.Keys.Contains(key))
            {
                throw new ArgumentException("模板变量已定义。", key);
            }
            variable.Key = key;
            _varDict.Add(key, variable);         
            this._adapter.SetItem("__tpl_var_define__",_varDict);
        }

        public object GetVariable(string key)
        {
            if (this._varDict.Keys.Contains(key))
            {
                return _varDict[key];
            }
            return null;
        }

        public IDictionary<string, object> GetDefineVariable()
        {
            return this._varDict;
        }
    }
}