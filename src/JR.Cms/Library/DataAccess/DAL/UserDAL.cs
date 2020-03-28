using System;
using System.Data;
using JR.Cms.Domain.Interface.User;
using JR.Cms.Domain.Interface.Value;
using JR.Cms.Library.DataAccess.IDAL;
using JR.Stand.Core.Data;

namespace JR.Cms.Library.DataAccess.DAL
{
    public class UserDal : DalBase, IUserDal
    {
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="func"></param>
        /// <returns></returns>
        public void GetUserCredential(string username, DataReaderFunc func)
        {
            ExecuteReader(
                NewQuery(DbSql.UserGetUserCredentialByUserName,
                    Db.CreateParametersFromArray(
                        new object[,] {{"@userName", username}})),
                func
            );
        }


        public void GetUserById(int id, DataReaderFunc func)
        {
            ExecuteReader(
                NewQuery(DbSql.UserGetUserById,
                    Db.CreateParametersFromArray(
                        new object[,]
                        {
                            {"@id", id}
                        })),
                func
            );
        }

        /// <summary>
        /// 设置用户最后登录时间
        /// </summary>
        public void UpdateUserLastLoginDate(string username, DateTime date)
        {
            ExecuteNonQuery(
                NewQuery(DbSql.Member_UpdateUserLastLoginDate,
                    Db.CreateParametersFromArray(
                        new object[,]
                        {
                            {"@username", username},
                            {"@LastLoginDate", string.Format("{0:yyyy-MM-dd HH:mm:ss}", date)}
                        }
                    )));
        }

        public DataTable GetAllUser()
        {
            return GetDataSet(new SqlQuery(
                OptimizeSql(
                    @"SELECT u.id,name,avatar,user_name,phone,email,last_login_time,create_time FROM $PREFIX_user u
                LEFT JOIN  $PREFIX_credential c ON c.user_id=u.id"), EmptyParameter)).Tables[0];
        }

        public int GetUserIdByUserName(string userName)
        {
            var obj = ExecuteScalar(
                NewQuery(DbSql.UserGetUserIdByUserName,
                    Db.CreateParametersFromArray(
                        new object[,]
                        {
                            {"@userName", userName}
                        }))
            );
            if (obj != null && obj != DBNull.Value) return int.Parse(obj.ToString());
            return -1;
        }

        public int SaveUser(IUser user, bool isNew)
        {
            //ame=@name,avatar=@avatar,phone=@phone,email=@email,
            //   check_code=@checkCode,role_flag=@roleFlag,create_time=@createTime,last_login_time=@loginTime WHERE id=@id


            var data = new object[,]
            {
                {"@name", user.Name},
                {"@avatar", user.Avatar ?? ""},
                {"@phone", user.Phone ?? ""},
                {"@email", user.Email ?? ""},
                {"@checkCode", user.CheckCode ?? ""},
                {"@roleFlag", user.Flag},
                {"@createTime", user.CreateTime},
                {"@loginTime", user.LastLoginTime},
                {"@id", user.GetAggregaterootId()},
            };
            var parameters = Db.CreateParametersFromArray(data);
            if (isNew)
            {
                //@"INSERT INTO $PREFIX_user(name,avatar,phone,email, check_code,
                //flag,create_time,last_login_time)VALUES(@name,@avatar,@phone,@email,@checkCode,@roleFlag,@createTime,@loginTime)";
                var row = ExecuteNonQuery(NewQuery(DbSql.UserCreateUser, parameters));
                if (row > 0)
                {
                    var q = NewQuery("SELECT MAX(id) FROM $PREFIX_user", EmptyParameter);
                    return int.Parse(ExecuteScalar(q).ToString());
                }
            }
            else
            {
                ExecuteNonQuery(NewQuery(DbSql.UserUpdateUser, parameters));
            }

            return user.GetAggregaterootId();
        }

        public int DeleteUser(int userId)
        {
            return ExecuteNonQuery(
                NewQuery("DELETE FROM $PREFIX_user WHERE  id=@userId",
                    Db.CreateParametersFromArray(
                        new object[,]
                        {
                            {"@userId", userId}
                        })));
        }


        public bool CreateOperation(string name, string path, bool available)
        {
            //TODO:优化

            //如果存在则返回false
            var obj = ExecuteScalar(
                NewQuery(DbSql.Operation_CheckPathExist,
                    Db.CreateParametersFromArray(
                        new object[,]
                        {
                            {"@Path", path}
                        }))
            );
            if (obj != null) return false;

            ExecuteScalar(
                NewQuery(DbSql.Operation_CreateOperation,
                    Db.CreateParametersFromArray(
                        new object[,]
                        {
                            {"@Name", name},
                            {"@Path", path},
                            {"@available", available}
                        }))
            );
            return true;
        }

        public void DeleteOperation(int id)
        {
            ExecuteNonQuery(
                NewQuery(DbSql.Operation_DeleteOperation,
                    Db.CreateParametersFromArray(
                        new object[,]
                        {
                            {"@id", id}
                        }
                    )));
        }

        public void GetOperation(int id, DataReaderFunc func)
        {
            ExecuteReader(
                NewQuery(DbSql.Operation_GetOperation,
                    Db.CreateParametersFromArray(
                        new object[,]
                        {
                            {"@id", id}
                        })),
                func
            );
        }

        public void GetOperations(DataReaderFunc func)
        {
            ExecuteReader(NewQuery(DbSql.Operation_GetOperations, EmptyParameter), func);
        }

        public void UpdateOperation(int id, string name, string path, bool available)
        {
            ExecuteNonQuery(
                NewQuery(DbSql.Operation_UpdateOperation,
                    Db.CreateParametersFromArray(
                        new object[,]
                        {
                            {"@Name", name},
                            {"@Path", path},
                            {"@available", available},
                            {"@id", id}
                        }))
            );
        }


        public DataTable GetPagedOperationList(int pageSize, int currentPageIndex, out int recordCount,
            out int pageCount)
        {
            const string sql1 = "SELECT TOP $[pagesize] * FROM $PREFIX_operation";

            //计算页码
            recordCount =
                int.Parse(
                    ExecuteScalar(NewQuery(DbSql.Operation_GetOperationCount, EmptyParameter))
                        .ToString());
            pageCount = recordCount / pageSize;
            if (recordCount % pageSize != 0) pageCount++;

            if (currentPageIndex > pageCount && currentPageIndex != 1) currentPageIndex = pageCount;
            if (currentPageIndex < 1) currentPageIndex = 1;

            var skipCount = pageSize * (currentPageIndex - 1);

            //如果调过记录为0条，且为OLEDB时候，则用sql1
            var sql = skipCount == 0 && DbType == DataBaseType.OLEDB
                ? OptimizeSql(sql1)
                : OptimizeSql(DbSql.Archive_GetPagedOperations);


            sql = SQLRegex.Replace(sql,
                (match) =>
                {
                    switch (match.Groups[1].Value)
                    {
                        case "pagesize":
                            return pageSize.ToString();
                        case "skipsize":
                            return skipCount.ToString();
                    }

                    return null;
                });

            return GetDataSet(
                new SqlQuery(sql, EmptyParameter)
            ).Tables[0];
        }

        public DataTable GetPagedAvailableOperationList(bool available, int pageSize, int currentPageIndex,
            out int recordCount, out int pageCount)
        {
            const string sql1 = "SELECT TOP $[pagesize] * FROM $PREFIX_operation WHERE $[condition]";

            var condition = available ? "available" : "available=false";

            recordCount = int.Parse(
                ExecuteScalar(
                    new SqlQuery(
                        string.Format(OptimizeSql(DbSql.Operation_GetOperationsCountByAvailable), condition),
                        EmptyParameter)
                ).ToString());

            //计算页码
            pageCount = recordCount / pageSize;
            if (recordCount % pageSize != 0) pageCount++;

            if (currentPageIndex > pageCount && currentPageIndex != 1) currentPageIndex = pageCount;
            if (currentPageIndex < 1) currentPageIndex = 1;


            var skipCount = pageSize * (currentPageIndex - 1);

            //如果调过记录为0条，且为OLEDB时候，则用sql1
            var sql = skipCount == 0 && DbType == DataBaseType.OLEDB
                ? OptimizeSql(sql1)
                : OptimizeSql(DbSql.Archive_GetPagedOperationsByAvialble);

            sql = SQLRegex.Replace(sql, (match) =>
            {
                switch (match.Groups[1].Value)
                {
                    case "pagesize":
                        return pageSize.ToString();
                    case "skipsize":
                        return (pageSize * (currentPageIndex - 1)).ToString();
                    case "condition":
                        return condition;
                }

                return null;
            });

            return GetDataSet(new SqlQuery(sql, EmptyParameter)).Tables[0];
        }


        public void UpdateUserGroupPermissions(int groupId, string permissions)
        {
            ExecuteNonQuery(
                NewQuery(DbSql.UserGroup_UpdatePermissions,
                    Db.CreateParametersFromArray(
                        new object[,]
                        {
                            {"@Permissions", permissions},
                            {"@GroupId", groupId}
                        }))
            );
        }

        public void RenameUserGroup(int groupId, string groupName)
        {
            ExecuteNonQuery(NewQuery(DbSql.UserGroup_RenameGroup,
                Db.CreateParametersFromArray(
                    new object[,]
                    {
                        {"@Name", groupName},
                        {"@GroupId", groupId}
                    })));
        }


        public DataTable GetMyUserTable(int appId, int userId)
        {
            return GetDataSet(new SqlQuery(
                OptimizeSql(@"SELECT u.id,name,avatar,user_name,phone,email,last_login_time,create_time,r.flag, 
                c.enabled FROM $PREFIX_user u INNER JOIN  $PREFIX_user_role r ON r.user_id = u.id
                LEFT JOIN  $PREFIX_credential c ON c.user_id=u.id WHERE @appId=0 OR (app_id=@appId AND  r.flag <
                (SELECT flag FROM $PREFIX_user_role WHERE user_id=@userId AND app_id=@appId))"),
                new object[,]
                {
                    {"@appId", appId},
                    {"@userId", userId}
                }
            )).Tables[0];
        }


        public int SaveCredential(Credential credential, bool isNew)
        {
            var data = new object[,]
            {
                {"@id", credential.Id},
                {"@userId", credential.UserId},
                {"@userName", credential.UserName},
                {"@password", credential.Password},
                {"@enabled", credential.Enabled},
            };
            var parameters = Db.CreateParametersFromArray(data);
            if (isNew)
            {
                var affer = ExecuteNonQuery(NewQuery(
                    "INSERT INTO $PREFIX_credential(user_id,user_name,password,enabled)VALUES(@userId,@userName,@password,@enabled)",
                    parameters));
                if (affer > 0)
                    credential.Id =
                        int.Parse(
                            ExecuteScalar(
                                new SqlQuery(
                                    OptimizeSql("SELECT id FROM $PREFIX_credential WHERE user_id=@userId"),
                                    new object[,]
                                    {
                                        {"@userId", credential.UserId},
                                    })).ToString());
            }
            else
            {
                ExecuteNonQuery(NewQuery(
                    "UPDATE $PREFIX_credential SET user_name=@userName,password=@password,enabled=@enabled WHERE user_id=@userId",
                    parameters));
            }

            return credential.Id;
        }

        public void GetUserCredentialById(int userId, DataReaderFunc func)
        {
            ExecuteReader(
                NewQuery(DbSql.UserGetUserCredential,
                    Db.CreateParametersFromArray(
                        new object[,] {{"@userId", userId}})),
                func
            );
        }


        public void ReadUserRoles(int userId, DataReaderFunc func)
        {
            ExecuteReader(
                NewQuery(DbSql.UserGetUserRole,
                    Db.CreateParametersFromArray(
                        new object[,] {{"@userId", userId}})),
                func
            );
        }


        public void SaveUserRole(int userId, int appId, int flag)
        {
            ExecuteNonQuery(NewQuery(
                "INSERT INTO $PREFIX_user_role(user_id,app_id,flag)VALUES(@userId,@appId,@flag)",
                Db.CreateParametersFromArray(
                    new object[,]
                    {
                        {"@userId", userId},
                        {"@appId", appId},
                        {"@flag", flag},
                    })));
        }

        public void CleanUserRoleFlag(int userId, int appId)
        {
            ExecuteNonQuery(NewQuery(
                "DELETE FROM $PREFIX_user_role WHERE user_id=@userId AND app_id=@appId",
                Db.CreateParametersFromArray(
                    new object[,]
                    {
                        {"@userId", userId},
                        {"@appId", appId},
                    })));
        }

        public string GetUserRealName(int userId)
        {
            var obj = ExecuteScalar(NewQuery(
                "SELECT name FROM $PREFIX_user WHERE id=@userId",
                Db.CreateParametersFromArray(
                    new object[,]
                    {
                        {"@userId", userId},
                    })));
            return obj != null ? obj.ToString() : null;
        }

        public int GetMinUserId()
        {
            return int.Parse(ExecuteScalar(NewQuery(
                "SELECT MIN(id) FROM $PREFIX_user", null)).ToString());
        }

        public int DeleteRoleBind(int userId)
        {
            return ExecuteNonQuery(
                NewQuery("DELETE FROM $PREFIX_user_role WHERE user_id=" + userId.ToString(), null));
        }

        public int DeleteCredential(int userId)
        {
            return ExecuteNonQuery(
                NewQuery("DELETE FROM $PREFIX_credential WHERE user_id=" + userId.ToString(), null));
        }
    }
}