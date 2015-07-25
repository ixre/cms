/**
 * Copyright (C) 2007-2015 S1N1.COM,All rights reseved.
 * Get more infromation of this software,please visit site http://h3f.net/cms
 * 
 * name : IocObject.cs
 * author : newmin (new.min@msn.com)
 * date : 2012/12/01 23:00:00
 * description : 
 * history : 
 */

using com.plugin.sso.Core.ILogic;
using com.plugin.sso.Core.Logic;
using J6.DevFw.Data;
using StructureMap;

namespace com.plugin.sso.Core
{
    /// <summary>
    /// Description of LogicHelper.
    /// </summary>
    public class IocObject
    {
        static IocObject()
        {
            ObjectFactory.Configure(o =>
            {
                o.For<DbGenerator>().Singleton().Use(new DbGenerator());
                o.For<IDataLogic>().Singleton().Use<DataLogic>();
            }
                );

            Data = ObjectFactory.GetInstance<IDataLogic>();
            _dao = ObjectFactory.GetInstance<DbGenerator>();
        }


        internal static readonly IDataLogic Data;
        private static DbGenerator _dao;

        public static DataBaseAccess GetDao()
        {
            return _dao.New();
        }
    }
}
