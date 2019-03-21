using System;
using System.Data;
using JR.Cms.Domain.Interface.User;
using JR.Cms.Domain.Interface.Value;
using JR.Cms.IDAL;
using JR.DevFw.Data;

namespace JR.Cms.Dal
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
            base.ExecuteReader(
                base.NewQuery(DbSql.UserGetUserCredentialByUserName,
                                base.Db.CreateParametersFromArray(
new object[,] { { "@userName", username } })),
                func
                );
        }


        public void GetUserById(int id, DataReaderFunc func)
        {
            base.ExecuteReader(
                base.NewQuery(DbSql.UserGetUserById,
                                base.Db.CreateParametersFromArray(

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
            base.ExecuteNonQuery(
                base.NewQuery(DbSql.Member_UpdateUserLastLoginDate,
                                base.Db.CreateParametersFromArray(

                    new object[,]
                    {
                        {"@username", username},
                        {"@LastLoginDate", String.Format("{0:yyyy-MM-dd HH:mm:ss}", date)}
                    }
                    )));
        }

        public DataTable GetAllUser()
        {
            return this.GetDataSet(new SqlQuery(
                 this.OptimizeSql(@"SELECT u.id,name,avatar,user_name,phone,email,last_login_time,create_time FROM $PREFIX_user u
                LEFT JOIN  $PREFIX_credential c ON c.user_id=u.id"), DalBase.EmptyParameter)).Tables[0];
        }

        public int GetUserIdByUserName(string userName)
        {
            object obj = base.ExecuteScalar(
                base.NewQuery(DbSql.UserGetUserIdByUserName,
                base.Db.CreateParametersFromArray(
                    new object[,]
                    {
                        {"@userName", userName}
                    }))
                );
            if (obj != null && obj != DBNull.Value)
            {
                return int.Parse(obj.ToString());
            }
            return -1;
        }

        public int SaveUser(IUser user, bool isNew)
        {
            //ame=@name,avatar=@avatar,phone=@phone,email=@email,
            //   check_code=@checkCode,role_flag=@roleFlag,create_time=@createTime,last_login_time=@loginTime WHERE id=@id


            var data = new object[,]
            {
                {"@name", user.Name},
                {"@avatar", user.Avatar??""},
                {"@phone", user.Phone??""},
                {"@email", user.Email??""},
                {"@checkCode", user.CheckCode??""},
                {"@roleFlag", user.Flag},
                {"@createTime", user.CreateTime},
                {"@loginTime", user.LastLoginTime},
                {"@id", user.GetAggregaterootId()},
            };
            var parameters = base.Db.CreateParametersFromArray(data);
            if (isNew)
            {
                //@"INSERT INTO $PREFIX_user(name,avatar,phone,email, check_code,
                //flag,create_time,last_login_time)VALUES(@name,@avatar,@phone,@email,@checkCode,@roleFlag,@createTime,@loginTime)";
                int row = base.ExecuteNonQuery(base.NewQuery(DbSql.UserCreateUser, parameters));
                if (row > 0)
                {
                    SqlQuery q = base.NewQuery("SELECT MAX(id) FROM $PREFIX_user", DalBase.EmptyParameter);
                    return int.Parse(this.ExecuteScalar(q).ToString());
                }
            }
            else
            {
                base.ExecuteNonQuery(base.NewQuery(DbSql.UserUpdateUser,parameters));
            }

            return user.GetAggregaterootId();
        }

        public int DeleteUser(int userId)
        {
            return base.ExecuteNonQuery(
                base.NewQuery("DELETE FROM $PREFIX_user WHERE  id=@userId",
                                base.Db.CreateParametersFromArray(

                    new object[,]
                    {
                        {"@userId", userId}
                    })));
        }


        public bool CreateOperation(string name, string path, bool available)
        {

            //TODO:优化

            //如果存在则返回false
            object obj = base.ExecuteScalar(
                base.NewQuery(DbSql.Operation_CheckPathExist,
                                base.Db.CreateParametersFromArray(

                    new object[,]
                    {
                        {"@Path", path}
                    }))
                );
            if (obj != null) return false;

            base.ExecuteScalar(
                base.NewQuery(DbSql.Operation_CreateOperation,
                                base.Db.CreateParametersFromArray(

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
            base.ExecuteNonQuery(
                base.NewQuery(DbSql.Operation_DeleteOperation,
                                base.Db.CreateParametersFromArray(

                    new object[,]
                    {
                        {"@id", id}
                    }
                    )));
        }

        public void GetOperation(int id, DataReaderFunc func)
        {
            base.ExecuteReader(
                base.NewQuery(DbSql.Operation_GetOperation,
                                base.Db.CreateParametersFromArray(
                    new object[,]
                    {
                        {"@id", id}
                    })),
                func
                );
        }

        public void GetOperations(DataReaderFunc func)
        {
            base.ExecuteReader(base.NewQuery(DbSql.Operation_GetOperations, DalBase.EmptyParameter), func);
        }

        public void UpdateOperation(int id, string name, string path, bool available)
        {
            base.ExecuteNonQuery(
                base.NewQuery(DbSql.Operation_UpdateOperation,
                                base.Db.CreateParametersFromArray(
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
                    base.ExecuteScalar(base.NewQuery(DbSql.Operation_GetOperationCount, DalBase.EmptyParameter))
                        .ToString());
            pageCount = recordCount / pageSize;
            if (recordCount % pageSize != 0) pageCount++;

            if (currentPageIndex > pageCount && currentPageIndex != 1) currentPageIndex = pageCount;
            if (currentPageIndex < 1) currentPageIndex = 1;

            int skipCount = pageSize * (currentPageIndex - 1);

            //如果调过记录为0条，且为OLEDB时候，则用sql1
            string sql = skipCount == 0 && base.DbType == DataBaseType.OLEDB
                ? base.OptimizeSql(sql1)
                : base.OptimizeSql(DbSql.Archive_GetPagedOperations);


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

            return base.GetDataSet(
                new SqlQuery(sql, DalBase.EmptyParameter)
                ).Tables[0];
        }

        public DataTable GetPagedAvailableOperationList(bool available, int pageSize, int currentPageIndex,
            out int recordCount, out int pageCount)
        {
            const string sql1 = "SELECT TOP $[pagesize] * FROM $PREFIX_operation WHERE $[condition]";

            string condition = available ? "available" : "available=false";

            recordCount = int.Parse(
                base.ExecuteScalar(
                    new SqlQuery(
                        String.Format(base.OptimizeSql(DbSql.Operation_GetOperationsCountByAvailable), condition), DalBase.EmptyParameter)
                    ).ToString());

            //计算页码
            pageCount = recordCount / pageSize;
            if (recordCount % pageSize != 0) pageCount++;

            if (currentPageIndex > pageCount && currentPageIndex != 1) currentPageIndex = pageCount;
            if (currentPageIndex < 1) currentPageIndex = 1;


            int skipCount = pageSize * (currentPageIndex - 1);

            //如果调过记录为0条，且为OLEDB时候，则用sql1
            string sql = skipCount == 0 && base.DbType == DataBaseType.OLEDB
                ? base.OptimizeSql(sql1)
                : base.OptimizeSql(DbSql.Archive_GetPagedOperationsByAvialble);

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

            return base.GetDataSet(new SqlQuery(sql, DalBase.EmptyParameter)).Tables[0];
        }


        public void UpdateUserGroupPermissions(int groupId, string permissions)
        {
            base.ExecuteNonQuery(
                base.NewQuery(DbSql.UserGroup_UpdatePermissions,
                                base.Db.CreateParametersFromArray(
                    new object[,]
                    {
                        {"@Permissions", permissions},
                        {"@GroupId", groupId}
                    }))
                );
        }

        public void RenameUserGroup(int groupId, string groupName)
        {
            base.ExecuteNonQuery(base.NewQuery(DbSql.UserGroup_RenameGroup,
                                base.Db.CreateParametersFromArray(
                new object[,]
                {
                    {"@Name", groupName},
                    {"@GroupId", groupId}
                })));
        }


        public DataTable GetMyUserTable(int appId, int userId)
        {
            return this.GetDataSet(new SqlQuery(
                this.OptimizeSql(@"SELECT u.id,name,avatar,user_name,phone,email,last_login_time,create_time,r.flag, 
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
            var parameters = base.Db.CreateParametersFromArray(data);
            if (isNew)
            {
                int affer = this.ExecuteNonQuery(base.NewQuery(
                    "INSERT INTO $PREFIX_credential(user_id,user_name,password,enabled)VALUES(@userId,@userName,@password,@enabled)",
                    parameters));
                if (affer > 0)
                {
                    credential.Id =
                        int.Parse(
                            this.ExecuteScalar(
                                new SqlQuery(
                                    base.OptimizeSql("SELECT id FROM $PREFIX_credential WHERE user_id=@userId"),
                                    new object[,]
                                    {
                                        {"@userId", credential.UserId},
                                    })).ToString());
                }
            }
            else
            {
                this.ExecuteNonQuery(base.NewQuery(
                    "UPDATE $PREFIX_credential SET user_name=@userName,password=@password,enabled=@enabled WHERE user_id=@userId", parameters));
            }
            return credential.Id;
        }

        public void GetUserCredentialById(int userId, DataReaderFunc func)
        {
            base.ExecuteReader(
                base.NewQuery(DbSql.UserGetUserCredential,
                                base.Db.CreateParametersFromArray(
new object[,] { { "@userId", userId } })),
                func
                );
        }


        public void ReadUserRoles(int userId, DataReaderFunc func)
        {
            base.ExecuteReader(
              base.NewQuery(DbSql.UserGetUserRole,
                              base.Db.CreateParametersFromArray(
new object[,] { { "@userId", userId } })),
              func
              );
        }


        public void SaveUserRole(int userId, int appId, int flag)
        {
            this.ExecuteNonQuery(base.NewQuery(
                    "INSERT INTO $PREFIX_user_role(user_id,app_id,flag)VALUES(@userId,@appId,@flag)",
                                    base.Db.CreateParametersFromArray(
new object[,]
                   {
                       {"@userId",userId},
                       {"@appId",appId},
                       {"@flag",flag},
                   })));
        }

        public void CleanUserRoleFlag(int userId, int appId)
        {
            this.ExecuteNonQuery(base.NewQuery(
                   "DELETE FROM $PREFIX_user_role WHERE user_id=@userId AND app_id=@appId",
                                   base.Db.CreateParametersFromArray(
new object[,]
                   {
                       {"@userId",userId},
                       {"@appId",appId},
                   })));
        }

        public string GetUserRealName(int userId)
        {
            Object obj = this.ExecuteScalar(base.NewQuery(
                "SELECT name FROM $PREFIX_user WHERE id=@userId",
                                base.Db.CreateParametersFromArray(
new object[,]
                {
                    {"@userId", userId},
                })));
            return obj != null ? obj.ToString() : null;
        }

        public int GetMinUserId()
        {
            return int.Parse(this.ExecuteScalar(base.NewQuery(
                "SELECT MIN(id) FROM $PREFIX_user",null)).ToString());
        }

        public int DeleteRoleBind(int userId)
        {
            return base.ExecuteNonQuery(
                    base.NewQuery("DELETE FROM $PREFIX_user_role WHERE user_id=" + userId.ToString(),null));
        }

        public int DeleteCredential(int userId)
        {
            return base.ExecuteNonQuery(
                    base.NewQuery("DELETE FROM $PREFIX_credential WHERE user_id=" + userId.ToString(),null));
        }
    }
}