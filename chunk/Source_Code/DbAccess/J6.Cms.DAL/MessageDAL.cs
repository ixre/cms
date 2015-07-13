//
// MessageDAL   会员数据访问
// Copryright 2011 @ S1N1.COM,All rights reseved !
// Create by newmin @ 2011/04/06
//

using System;
using System.Data;
using J6.Cms.IDAL;
using J6.DevFw.Data;

namespace J6.Cms.Dal
{
    /// <summary>
    /// 会员数据访问
    /// </summary>
    public sealed class MessageDAL:DalBase,ImessageDAL
    {
        /// <summary>
        /// 验证用户并返回
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public void GetMessage(int id, DataReaderFunc func)
        {
            base.ExecuteReader(
                new SqlQuery(base.OptimizeSql(DbSql.Message_GetMessage),
                    new object[,]{{"@id", id}
                    }),
                func
                );
        }

        public void WriteMessage(int sendUID, int receiveUID, string subject, string content)
        {
            base.ExecuteNonQuery(
               new SqlQuery(base.OptimizeSql(DbSql.Mesage_InsertMessage),
                   new object[,]{
               {"@SendUId", sendUID},
               {"@ReceiveUId", receiveUID},
               {"@Subject", subject},
               {"@Content", content},
               {"@HasRead", false},
               {"@Recycle", false},
               {"@SendDate",DateTime.Now.ToString()}
                   })
               );
        }
        
        public int SetRead(int receiveUid, int id)
        {
            return base.ExecuteNonQuery(
                new SqlQuery(base.OptimizeSql(DbSql.Message_SetRead),
                    new object[,]{
                {"@ReceiveUId", receiveUid},
                {"@id", id}
                    }));
        }

        public int SetRecycle(int receiveUid,int id)
        {
            return base.ExecuteNonQuery(
                new SqlQuery(base.OptimizeSql(DbSql.Message_SetRecycle),new object[,]{
                {"@ReceiveUId",receiveUid},
                {"@id", id}
                }));
        }

        public int Delete(int receiveUid, int id)
        {
            return base.ExecuteNonQuery(
                new SqlQuery(base.OptimizeSql(DbSql.Message_Delete),
                    new object[,]{
                 {"@ReceiveUId", receiveUid},
                 {"@id", id}
                    }));
        }

        /// <summary>
        /// 获取分页消息
        /// </summary>
        /// <param name="uid">用户ID</param>
        /// <param name="type">消息类型</param>
        /// <param name="pageSize"></param>
        /// <param name="currentPageIndex"></param>
        /// <param name="recordCount"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public DataTable GetPagedMessage(int uid, int typeID, int pageSize, ref int currentPageIndex, out int recordCount, out int pageCount)
        {
            const string sql1 = "SELECT TOP $[pagesize] * FROM $PREFIX_Message WHERE Recycle=0 AND $[condition]";

            string condition = String.Format(typeID == 1 ? "[receiveuid]={0}" : "[senduid]={0}", uid);


            recordCount = int.Parse(base.ExecuteScalar(new SqlQuery(
                String.Format(base.OptimizeSql(DbSql.Message_GetPagedMessagesCount), condition),null)
                ).ToString());


            pageCount = recordCount / pageSize;
            if (recordCount % pageSize != 0) pageCount++;

            //当前页数
            if (currentPageIndex > pageCount && currentPageIndex != 1) currentPageIndex = pageCount;
            if (currentPageIndex < 1) currentPageIndex = 1;

            //计算分页
            int skipCount = pageSize * (currentPageIndex - 1);

            //如果调过记录为0条，且为OLEDB时候，则用sql1
            string sql = skipCount == 0 && base.DbType == DataBaseType.OLEDB ?
                        base.OptimizeSql(sql1) :
                        base.OptimizeSql(DbSql.Message_GetPagedMessages);

            sql = SQLRegex.Replace(sql, match =>
            {
                switch (match.Groups[1].Value)
                {
                    case "pagesize": return pageSize.ToString();
                    case "skipsize": return skipCount.ToString();
                    case "condition": return condition.ToString();
                }
                return null;

            });

            return base.GetDataSet(new SqlQuery(sql)).Tables[0];

        }


    }
}