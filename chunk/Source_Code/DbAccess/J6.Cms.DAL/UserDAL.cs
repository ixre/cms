using System;
using System.Data;
using J6.Cms.Domain.Interface.User;
using J6.Cms.Domain.Interface.Value;
using J6.Cms.IDAL;
using J6.DevFw.Data;

namespace J6.Cms.Dal
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
                new SqlQuery(base.OptimizeSql(DbSql.UserGetUserCredentialByUserName), new object[,] {{"@userName", username}}),
                func
                );
        }


        public void GetUserById(int id, DataReaderFunc func)
        {
            base.ExecuteReader(
                new SqlQuery(base.OptimizeSql(DbSql.UserGetUserById),
                    new object[,]
                    {
                        {"@id", id}
                    }),
                func
                );
        }

        /// <summary>
        /// 设置用户最后登录时间
        /// </summary>
        public void UpdateUserLastLoginDate(string username, DateTime date)
        {
            base.ExecuteNonQuery(
                new SqlQuery(base.OptimizeSql(DbSql.Member_UpdateUserLastLoginDate),
                    new object[,]
                    {
                        {"@username", username},
                        {"@LastLoginDate", String.Format("{0:yyyy-MM-dd HH:mm:ss}", date)}
                    }
                    ));
        }

        public DataTable GetAllUser()
        {
           return this.GetDataSet(new SqlQuery(
                this.OptimizeSql(@"SELECT u.id,name,avatar,user_name,phone,email,last_login_time,create_time FROM $PREFIX_user u
                LEFT JOIN  $PREFIX_credential c ON c.user_id=u.id"),null)).Tables[0];
        }

        public int GetUserIdByUserName(string userName)
        {
            object obj = base.ExecuteScalar(
                new SqlQuery(base.OptimizeSql(DbSql.UserGetUserIdByUserName),
                    new object[,]
                    {
                        {"@userName", userName}
                    })
                );
            if (obj != null && obj != DBNull.Value)
            {
                return int.Parse(obj.ToString());
            }
            return -1;
        }

        public void CreateUser(int siteId, string username, string password, string name, int groupId, bool available)
        {
            DateTime dt = DateTime.Now;
            base.ExecuteNonQuery(
                new SqlQuery(base.OptimizeSql(DbSql.UserCreateUser),
                    new object[,]
                    {
                        {"@siteId", siteId},
                        {"@Username", username},
                        {"@Password", password},
                        {"@Name", name},
                        {"@GroupId", groupId},
                        {"@available", available},
                        {"@CreateDate", String.Format("{0:yyyy-MM-dd HH:mm:ss}", dt)},
                        {"@LastLoginDate", String.Format("{0:yyyy-MM-dd HH:mm:ss}", dt)}
                    })
                );
        }

        [Obsolete]
        public void ModifyPassword(string username, string newPassword)
        {
            base.ExecuteNonQuery(
                new SqlQuery(base.OptimizeSql(""),
                    new object[,]
                    {
                        {"@Password", newPassword},
                        {"@UserName", username}
                    }));
        }

        public int SaveUser(IUser user,bool isNew)
        {
            //ame=@name,avatar=@avatar,phone=@phone,email=@email,
             //   check_code=@checkCode,role_flag=@roleFlag,create_time=@createTime,last_login_time=@loginTime WHERE id=@id

            var data = new object[,]
            {
                {"@name", user.Name},
                {"@avatar", user.Avatar},
                {"@phone", user.Phone},
                {"@email", user.Email},
                {"@checkCode", user.CheckCode},
                {"@roleFlag", user.Flag},
                {"@createTime", user.CreateTime},
                {"@loginTime", user.LastLoginTime},
                {"@id", user.Id},
            };

            if (isNew)
            {
               int row= base.ExecuteNonQuery(new SqlQuery(base.OptimizeSql(DbSql.UserCreateUser), data));
                if (row > 0)
                {
                    SqlQuery q = new SqlQuery(base.OptimizeSql("SELECT MAX(id) FROM $PREFIX_user"),null);
                    return int.Parse(this.ExecuteScalar(q).ToString());
                }
            }
            else
            {
                base.ExecuteNonQuery(new SqlQuery(base.OptimizeSql(DbSql.UserUpdateUser), data));
            }

            return user.Id;
        }

        public int DeleteUser(int userId)
        {
            return base.ExecuteNonQuery(
                new SqlQuery(base.OptimizeSql("DELETE FROM $PREFIX_user WHERE  id=@userId"),
                    new object[,]
                    {
                        {"@userId", userId}
                    }));
        }


        public bool CreateOperation(string name, string path, bool available)
        {

            //TODO:优化

            //如果存在则返回false
            object obj = base.ExecuteScalar(
                new SqlQuery(base.OptimizeSql(DbSql.Operation_CheckPathExist),
                    new object[,]
                    {
                        {"@Path", path}
                    })
                );
            if (obj != null) return false;

            base.ExecuteScalar(
                new SqlQuery(base.OptimizeSql(DbSql.Operation_CreateOperation),
                    new object[,]
                    {
                        {"@Name", name},
                        {"@Path", path},
                        {"@available", available}
                    })
                );
            return true;
        }

        public void DeleteOperation(int id)
        {
            base.ExecuteNonQuery(
                new SqlQuery(base.OptimizeSql(DbSql.Operation_DeleteOperation),
                    new object[,]
                    {
                        {"@id", id}
                    }
                    ));
        }

        public void GetOperation(int id, DataReaderFunc func)
        {
            base.ExecuteReader(
                new SqlQuery(base.OptimizeSql(DbSql.Operation_GetOperation),
                    new object[,]
                    {
                        {"@id", id}
                    }),
                func
                );
        }

        public void GetOperations(DataReaderFunc func)
        {
            base.ExecuteReader(new SqlQuery(base.OptimizeSql(DbSql.Operation_GetOperations), null), func);
        }

        public void UpdateOperation(int id, string name, string path, bool available)
        {
            base.ExecuteNonQuery(
                new SqlQuery(base.OptimizeSql(DbSql.Operation_UpdateOperation),
                    new object[,]
                    {
                        {"@Name", name},
                        {"@Path", path},
                        {"@available", available},
                        {"@id", id}
                    })
                );
        }


        public DataTable GetPagedOperationList(int pageSize, int currentPageIndex, out int recordCount,
            out int pageCount)
        {
            const string sql1 = "SELECT TOP $[pagesize] * FROM $PREFIX_operation";

            //计算页码
            recordCount =
                int.Parse(
                    base.ExecuteScalar(new SqlQuery(base.OptimizeSql(DbSql.Operation_GetOperationCount), null))
                        .ToString());
            pageCount = recordCount/pageSize;
            if (recordCount%pageSize != 0) pageCount++;

            if (currentPageIndex > pageCount && currentPageIndex != 1) currentPageIndex = pageCount;
            if (currentPageIndex < 1) currentPageIndex = 1;

            int skipCount = pageSize*(currentPageIndex - 1);

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
                new SqlQuery(sql, null)
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
                        String.Format(base.OptimizeSql(DbSql.Operation_GetOperationsCountByAvailable), condition), null)
                    ).ToString());

            //计算页码
            pageCount = recordCount/pageSize;
            if (recordCount%pageSize != 0) pageCount++;

            if (currentPageIndex > pageCount && currentPageIndex != 1) currentPageIndex = pageCount;
            if (currentPageIndex < 1) currentPageIndex = 1;


            int skipCount = pageSize*(currentPageIndex - 1);

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
                        return (pageSize*(currentPageIndex - 1)).ToString();
                    case "condition":
                        return condition;
                }
                return null;
            });

            return base.GetDataSet(new SqlQuery(sql, null)).Tables[0];
        }


        public void UpdateUserGroupPermissions(int groupId, string permissions)
        {
            base.ExecuteNonQuery(
                new SqlQuery(base.OptimizeSql(DbSql.UserGroup_UpdatePermissions),
                    new object[,]
                    {
                        {"@Permissions", permissions},
                        {"@GroupId", groupId}
                    })
                );
        }

        public void RenameUserGroup(int groupId, string groupName)
        {
            base.ExecuteNonQuery(new SqlQuery(base.OptimizeSql(DbSql.UserGroup_RenameGroup),
                new object[,]
                {
                    {"@Name", groupName},
                    {"@GroupId", groupId}
                }));
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


        public int SaveCredential(Credential credential,bool isNew)
        {
            var data = new object[,]
            {
                {"@id", credential.Id},
                {"@userId", credential.UserId},
                {"@userName", credential.UserName},
                {"@password", credential.Password},
                {"@enabled", credential.Enabled},
            };

            if (isNew)
            {
                int affer = this.ExecuteNonQuery(new SqlQuery(base.OptimizeSql(
                    "INSERT INTO $PREFIX_credential(user_id,user_name,password,enabled)VALUES(@userId,@userName,@password,@enabled)"),
                    data));
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
                this.ExecuteNonQuery(new SqlQuery(base.OptimizeSql(
                    "UPDATE $PREFIX_credential SET user_name=@userName,password=@password,enabled=@enabled WHERE user_id=@userId"),data));
            }
            return credential.Id;
        }

        public void GetUserCredentialById(int userId, DataReaderFunc func)
        {
            base.ExecuteReader(
                new SqlQuery(base.OptimizeSql(DbSql.UserGetUserCredential), new object[,] { { "@userId", userId } }),
                func
                );
        }


        public void ReadUserRoles(int userId, DataReaderFunc func)
        {
            base.ExecuteReader(
              new SqlQuery(base.OptimizeSql(DbSql.UserGetUserRole), new object[,] { { "@userId", userId } }),
              func
              );
        }


        public void SaveUserRole(int userId, int appId, int flag)
        {
            this.ExecuteNonQuery(new SqlQuery(base.OptimizeSql(
                    "INSERT INTO $PREFIX_user_role(user_id,app_id,flag)VALUES(@userId,@appId,@flag)"), new object[,]
                   {
                       {"@userId",userId},
                       {"@appId",appId},
                       {"@flag",flag},
                   }));
        }

        public void CleanUserRoleFlag(int userId, int appId)
        {
            this.ExecuteNonQuery(new SqlQuery(base.OptimizeSql(
                   "DELETE FROM $PREFIX_user_role WHERE user_id=@userId AND app_id=@appId"), new object[,]
                   {
                       {"@userId",userId},
                       {"@appId",appId},
                   }));
        }

        public string GetUserRealName(int userId)
        {
            Object obj = this.ExecuteScalar(new SqlQuery(base.OptimizeSql(
                "SELECT name FROM $PREFIX_user WHERE id=@userId"), new object[,]
                {
                    {"@userId", userId},
                }));
            return obj != null ? obj.ToString() : null;
        }

        public int GetMinUserId()
        {
            return int.Parse(this.ExecuteScalar(new SqlQuery(base.OptimizeSql(
                "SELECT MIN(id) FROM $PREFIX_user"))).ToString());
        }

        public int DeleteRoleBind(int userId)
        {
            return  base.ExecuteNonQuery(
                    new SqlQuery(base.OptimizeSql("DELETE FROM $PREFIX_user_role WHERE user_id=" + userId.ToString())));
        }

        public int DeleteCredential(int userId)
        {
            return base.ExecuteNonQuery(
                    new SqlQuery(base.OptimizeSql("DELETE FROM $PREFIX_credential WHERE user_id=" + userId.ToString())));
        }
    }
}