using System;
using System.Collections.Generic;

namespace JR.Stand.Core.Template.Impl
{ 
    public class NormalDataContainer : IDataContainer
    {
        private IDictionary<string, object> _varDict = new Dictionary<string, object>();

        public IDataAdapter GetAdapter()
        {
            throw new NotImplementedException();
        }

        public string GetTemplatePageCacheContent(string templateID)
        {
            throw new Exception();
        }

        public void SetTemplatePageCacheContent(string templateID, string content, string dependFileName)
        {
        }

        public void DefineVariable<T>(string key, T value)
        {
            if (value == null) return; //防止非法参数


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
        }

        public void DefineVariable(string key, Variable variable)
        {
            if (_varDict.Keys.Contains(key))
            {
                throw new ArgumentException("模板变量已定义。", key);
            }
            variable.Key = key;
            _varDict.Add(key, variable);
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
            if (_varDict == null)
            {
                _varDict = new Dictionary<string, object>();
            }
            return _varDict;
        }
    }
}