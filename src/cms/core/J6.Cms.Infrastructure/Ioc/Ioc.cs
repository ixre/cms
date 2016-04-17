using System;
using System.Collections.Generic;
using StructureMap;

namespace J6.Cms.Infrastructure.Ioc
{
    public class Ioc
    {
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
                t = ObjectFactory.GetInstance<T>();
            }
            else
            {
                t = ObjectFactory.GetInstance<T>(new StructureMap.Pipeline.ExplicitArguments(parameters));
            }
            return t;
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
            ObjectFactory.Configure(configure);
        }
    }
}
