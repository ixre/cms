/**
 * Copyright (C) 2007-2015 S1N1.COM,All rights reseved.
 * Get more infromation of this software,please visit site http://cms.j6.cc
 * 
 * name : IDataLogic.cs
 * author : newmin (new.min@msn.com)
 * date : 2012/12/01 23:00:00
 * description : 
 * history : 
 */

using System.Collections;
using System.Data;

namespace com.plugin.sso.Core.ILogic
{
    /// <summary>
    /// Description of ICustomer.
    /// </summary>
    public interface IDataLogic
    {
        DataTable GetQueryView(string queryName, Hashtable hash, int pageSize, int currentPageIndex, out int totalCount);

        DataRow GetTotalView(string queryName, Hashtable hash);

        string GetColumnMappingString(string queryName);

    }
}
