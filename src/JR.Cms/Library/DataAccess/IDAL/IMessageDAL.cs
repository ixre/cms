//
//
//  Generated by StarUML(tm) C# Add-In
//
//  @ Project : OSite
//  @ File Name : ImessageDAL.cs
//  @ Date : 2011/8/23
//  @ Author : 
//
//

using System.Data;
using JR.Stand.Core.Data;

namespace JR.Cms.Library.DataAccess.IDAL
{
    public interface ImessageDAL
    {
        void WriteMessage(int sendUID, int receiveUID, string subject, string content);
        void GetMessage(int id, DataReaderFunc func);
        int SetRecycle(int id, int receiveUID);
        int SetRead(int id, int receiveUID);
        int Delete(int receiveUid, int id);

        DataTable GetPagedMessage(int uid, int typeID, int pageSize, ref int currentPageIndex, out int recordCount,
            out int pageCount);
    }
}