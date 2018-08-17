using System;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using System.Web.Routing;
using T2.Cms.Conf;
using T2.Cms.Domain.Interface.User;
using T2.Cms.Infrastructure.Domain;
using JR.DevFw.Data;
using JR.DevFw.Framework;
using JR.DevFw.Framework.Extensions;

namespace T2.Cms.Web
{
    /// <summary>
    /// Description of CmsInstallRouteHandler.
    /// </summary>
    internal class CmsInstallHandler : IRouteHandler
    {
        private class HttpHandler : IHttpHandler
        {
            public bool IsReusable
            {
                get
                {
                    return true;
                }
            }

            public void ProcessRequest(HttpContext context)
            {
                if (context.Request.HttpMethod == "POST")
                {
                    context.Response.Write(new CmsInstallWiz().Process(context));
                }
                else
                {
                    context.Response.Redirect("/install/install.html");
                }
            }
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new HttpHandler();
        }
    }


    internal class CmsInstallWiz
    {

        public const string FILE_DB_SQLITE = "install/db/sqlite.db";
        public const string FILE_DB_OLEDB = "install/db/access.mdb";
        public const string FILE_SETTING = "config/cms.conf";
        public const string MYSQL_INSTALL_SCRIPT = "install/db/mysql.sql";
        public const string MSSQL_INSTALL_SCRIPT = "install/db/mssql.sql";
        public const string SQLite_INSTALL_SCRIPT = "install/db/sqlite.sql";

        public const string INSTALL_LOCK = "config/install.lock";

        public enum InstallCode
        {
            /// <summary>
            /// 已经安装成功
            /// </summary>
            INSTALLED = 0,

            SUCCESS = 1,

            /// <summary>
            ///  无授权信息
            /// </summary>
            NO_LICENCE = -1,

            /// <summary>
            /// 未指定用户
            /// </summary>
            NO_USER = -2,

            /// <summary>
            /// 未设置站点
            /// </summary>
            NO_SITE_NAME = -3,

            /// <summary>
            /// 未知数据库
            /// </summary>
            DB_UNKNOWN = -11,

            /// <summary>
            /// 数据库错误
            /// </summary>
            DB_ERROR = -12,

            /// <summary>
            /// 数据库初始化错误
            /// </summary>
            DB_INIT_ERROR = -13

        }

        public InstallCode Process(HttpContext context)
        {

            NameValueCollection form = context.Request.Form;
            string physical = Cms.PyhicPath;

            if (!Directory.Exists(Cms.PyhicPath + "config"))
            {
                Directory.CreateDirectory(Cms.PyhicPath + "config").Create();
            }

            if (File.Exists(String.Concat(physical, INSTALL_LOCK)))
            {
                return InstallCode.INSTALLED;
            }

            string licenceKey = form["licence_key"].Trim(),
                   licenceName = form["licence_name"].Trim(),
                   siteName = form["site_name"].Trim(),
                   siteDomain = form["site_domain"].Trim(),
                   siteLanguage = form["site_language"],
                   userName = form["user_name"].Trim(),
                   userPwd = form["user_pwd"].Trim(),
                   dbType = form["db_type"].Trim(),
                   dbServer = form["db_server"].Trim(),
                   dbPrefix = form["db_prefix"].Trim(),
                   dbPrefix1 = form["db_prefix1"].Trim(),
                   dbName = form["db_name"].Trim(),
                   dbUsr = form["db_usr"].Trim(),
                   dbPwd = form["db_pwd"].Trim(),
                   dbFile = form["db_file"].Trim();

            string dbStr = "";

            #region 检测数据
            if(String.IsNullOrEmpty(licenceName) || String.IsNullOrEmpty(licenceKey))
            {
                return InstallCode.NO_LICENCE;
            }
            if (String.IsNullOrEmpty(siteName))
            {
                return InstallCode.NO_SITE_NAME;
            }

            if (String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(userPwd))
            {
                return InstallCode.NO_USER;
            }

            #endregion

            #region 初始化数据库设置

            //数据表前缀
            if (String.IsNullOrEmpty(dbPrefix)) dbPrefix = dbPrefix1;

            const String dbDirName = "data";

            //移动Access或SQLite数据库
            if (dbType == "sqlite")
            {
                if (dbFile == "")
                {
                    dbFile ="rd_"+string.Empty.RandomLetters(5)+".db";
                }
                else if (dbFile.IndexOf(".", StringComparison.Ordinal) == -1)
                {
                    dbFile += ".db";
                }
                if (!Directory.Exists(physical + dbDirName))
                {
                    Directory.CreateDirectory(physical + dbDirName).Create();
                }
                dbStr = "Data Source=$ROOT/"+dbDirName+"/" + dbFile;
            }
            else if (dbType == "oledb")
            {
                if (dbFile == "")
                {
                    dbFile = "rd_" + string.Empty.RandomLetters(5) + ".mdb";
                }
                else if (dbFile.IndexOf(".", StringComparison.Ordinal) == -1)
                {
                    dbFile += ".mdb";
                }

                if (!Directory.Exists(physical +dbDirName))
                {
                    Directory.CreateDirectory(physical + dbDirName).Create();
                }
                File.Copy(physical + FILE_DB_OLEDB, physical + dbDirName+"/" + dbFile, true);
                dbStr = "Data Source=$ROOT/" + dbDirName + "/" + dbFile;
            }
            else
            {
                //数据库资料不全
                if (String.IsNullOrEmpty(dbServer) || String.IsNullOrEmpty(dbUsr) || String.IsNullOrEmpty(dbName) || String.IsNullOrEmpty(dbPrefix))
                {
                    return InstallCode.DB_ERROR;
                }

                if (dbType == "mysql")
                {
                    dbStr = String.Format("server={0};database={1};uid={2};pwd={3};charset=utf8", dbServer, dbName, dbUsr, dbPwd);
                }
                else if (dbType == "mssql")
                {
                    dbStr = String.Format("server={0};database={1};uid={2};pwd={3}", dbServer, dbName, dbUsr, dbPwd);
                }
                else
                {
                    return InstallCode.DB_UNKNOWN;
                }
            }

            #endregion

            #region 写入配置

            if (File.Exists(physical + FILE_SETTING))
            {
                File.Delete(physical + FILE_SETTING);
            }
            SettingFile file = new SettingFile(physical + FILE_SETTING);
            file.Set("license_key", licenceKey);
            file.Set("license_name", licenceName);
            file.Set("server_static", "");
            file.Set("server_static_enabled", "false");

            file.Set("db_type", dbType);
            file.Set("db_conn", dbStr);
            file.Set("db_prefix", dbPrefix);

            file.Set("mm_avatar_path", "/file/avatar/");
            file.Flush();

            #endregion

            #region 初始化数据库

            if (!ExtraDB(dbType, dbStr, dbPrefix))
            {
                return InstallCode.DB_INIT_ERROR;
            }

            #endregion

            #region 初始化数据

            //默认数据为:
            // cms_sites        siteid为1的站点
            // cms_category   默认的about分类
            // cms_usergroup


            //更新默认站点
            this.db.ExecuteNonQuery(new SqlQuery(
                String.Format("UPDATE {0}site SET domain=@domain,name=@name,tpl=@tpl,language=@language,seo_title=@name where site_id=1", dbPrefix),
                new object[,]{
                    {"@domain",siteDomain},
                    {"@name",siteName},
                    {"@tpl","default"},
                    {"@language",siteLanguage},
                }));

            //创建管理用户
            DateTime dt = DateTime.Now;
            this.db.ExecuteNonQuery(new SqlQuery(
                String.Format(@"INSERT INTO {0}user(name,avatar,phone,email, check_code,flag,create_time,last_login_time) 
                    VALUES(@name,@avatar,@phone,@email,@checkCode,@flag,@time,@time)", dbPrefix),
                new object[,]{
                    {"@name","管理员"},
                    {"@avatar","/public/mui/css/latest/avatar.gif"},
                    {"@phone",""},
                    {"@email",""},
                    {"@checkCode",""},
                    {"@flag",Role.Master.Flag},
                    {"@time",dt},
                }));
            int userId = int.Parse(this.db.ExecuteScalar(String.Format("SELECT max(id) FROM {0}user", dbPrefix)).ToString());
            this.db.ExecuteNonQuery(new SqlQuery(
                String.Format(@"INSERT INTO {0}credential(user_id,user_name,password,enabled)VALUES(@userId,@userName,@password,1)", dbPrefix),
                new object[,]{
                    {"@userId",userId},
                    {"@userName",userName},
                    {"@password",Generator.Sha1Pwd(userPwd, Generator.Offset)},
                }));

            #endregion


            //创建安装文件
            File.Create(String.Concat(physical, INSTALL_LOCK));

            Settings.TurnOffDebug();

            Cms.Init(BootFlag.Normal,null);

            // 重启
            HttpRuntime.UnloadAppDomain();
            //AppDomain.Unload(AppDomain.CurrentDomain);

            return InstallCode.SUCCESS;
        }


        private DataBaseAccess db;

        private bool ExtraDB(string dbType, string connStr, string dbPrefix)
        {
            DataBaseType type = DataBaseType.MySQL;
            switch (dbType)
            {
                case "mysql": type = DataBaseType.MySQL; break;
                case "mssql": type = DataBaseType.SQLServer; break;
                case "sqlite": type = DataBaseType.SQLite; break;
                case "monosqlite": type = DataBaseType.MonoSQLite; break;
                case "oledb": type = DataBaseType.OLEDB; break;
            }

            this.db = new DataBaseAccess(type, connStr.Replace("$ROOT", Cms.PyhicPath));

            string sql = null;
            string sqlScript = null;

            if (type == DataBaseType.MySQL)
            {
                sqlScript = String.Concat(Cms.PyhicPath, MYSQL_INSTALL_SCRIPT);
                return execDbScript(dbPrefix, ref sql, sqlScript);
            }
            if (type == DataBaseType.SQLServer)
            {
                sqlScript = String.Concat(Cms.PyhicPath, MSSQL_INSTALL_SCRIPT);
                return execDbScript(dbPrefix, ref sql, sqlScript);
            }
            if (type == DataBaseType.SQLite)
            {
                sqlScript = String.Concat(Cms.PyhicPath, SQLite_INSTALL_SCRIPT);
                return execDbScript(dbPrefix, ref sql, sqlScript);
            }
            return true;
            //else if (type == DataBaseType.SQLServer)
            //{
            //    sqlScript = String.Concat(Cms.PyhicPath, MSSQL_INSTALL_SCRIPT);
            //}
        }

        /// <summary>
        /// 执行数据脚本
        /// </summary>
        /// <param name="dbPrefix"></param>
        /// <param name="sql"></param>
        /// <param name="sqlScript"></param>
        /// <returns></returns>
        private bool execDbScript(string dbPrefix, ref string sql, string sqlScript)
        {
            if (sqlScript != null)
            {
                //从脚本中读取出SQL语句
                TextReader tr = new StreamReader(sqlScript);

                sql = tr.ReadToEnd().Replace("cms_", dbPrefix)
                    .Replace(",False", ",0")
                    .Replace(",True", ",1")
                    .Replace(",index", ",`index`");
                tr.Dispose();
            }

            if (sql != null)
            {
                try
                {
                    db.ExecuteScript(sql, null);
                }
                catch (Exception exc)
                {
                    return false;
                }
            }

            return true;
        }

        //public void InitDB()
        //{
        //   ExtraDB("mysql", "server=0345cc.gotoftp3.com;database=0345cc;uid=0345cc;pwd=tangweibing", "t_");
        //}
    }
}