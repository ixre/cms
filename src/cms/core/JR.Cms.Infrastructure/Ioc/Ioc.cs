using System;
using System.Collections.Generic;
using StructureMap;

namespace JR.Cms.Infrastructure.Ioc
{
    public class Ioc
    {
        private static Container _container;
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static T GetInstance<T>(IDictionary<string, object> parameters)
        {
            T t;

            if (parameters == null)
            {
                t = GetContainer().GetInstance<T>();
            }
            else
            {
                t = GetContainer().GetInstance<T>(new StructureMap.Pipeline.ExplicitArguments(parameters));
            }
            return t;
        }

        private static Container GetContainer()
        {
            if (_container == null)
            {
                throw  new Exception("未初始化");
            }
            return _container;
        }

        public static T GetInstance<T>()
        {
            return GetInstance<T>(null);
        }
        
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="configure"></param>
        public static void Configure(Action<ConfigurationExpression> configure)
        {
            _container = new Container(configure);
        }
    }
}
