using AtNet.DevFw.Data;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using System.Web.Routing;
using AtNet.Cms.Conf;
using AtNet.DevFw.Framework;
using AtNet.DevFw.Framework.Extensions;

namespace AtNet.Cms.Web
{
    /// <summary>
    /// Description of CmsInstallRouteHandler.
    /// </summary>
    internal class CmsInstallRouteHandler : IRouteHandler
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
                    context.Response.StatusCode = 404;
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
        public const string FILE_SETTING = "config/cms.config";
        public const string MYSQL_INSTALL_SCRIPT = "install/db/mysql.sql";
        public const string MSSQL_INSTALL_SCRIPT = "install/db/mssql.sql";
        public const string INSTALL_LOCK = "config/install.lock";

        public enum InstallCode
        {
            /// <summary>
            /// 已经安装成功
            /// </summary>
            Installed = 0,

            SUCCESS = 1,

            /// <summary>
            /// 未指定用户
            /// </summary>
            NO_USER = -1,

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
            string physical = AtNet.Cms.Cms.PyhicPath;

            if (!Directory.Exists(AtNet.Cms.Cms.PyhicPath + "config"))
            {
                Directory.CreateDirectory(AtNet.Cms.Cms.PyhicPath + "config").Create();
            }

            if (System.IO.File.Exists(String.Concat(physical, INSTALL_LOCK)))
            {
                return InstallCode.Installed;
            }

            string t_key = form["t_key"],
                   t_name = form["t_name"],
                   site_name = form["site_name"],
                   site_domain = form["site_domain"],
                   site_language = form["site_language"],
                   user_name = form["user_name"],
                   user_pwd = form["user_pwd"],
                   db_type = form["db_type"],
                   db_server = form["db_server"],
                   db_prefix = form["db_prefix"],
                   db_prefix1 = form["db_prefix1"],
                   db_name = form["db_name"],
                   db_usr = form["db_usr"],
                   db_pwd = form["db_pwd"],
                   db_file = form["db_file"];

            string db_str = "";

            #region 检测数据

            if (String.IsNullOrEmpty(user_name) || String.IsNullOrEmpty(user_pwd))
            {
                return InstallCode.NO_USER;
            }

            #endregion

            #region 初始化数据库设置

            //数据表前缀
            if (String.IsNullOrEmpty(db_prefix)) db_prefix = db_prefix1;


            //移动Access或SQLite数据库
            if (db_type == "sqlite")
            {
                if (db_file == "")
                {
                    db_file ="rd_"+string.Empty.RandomLetters(5)+".db";
                }
                else if (db_file.IndexOf(".") == -1)
                {
                    db_file += ".db";
                }
                if (!Directory.Exists(physical + "sqlite.db"))
                {
                    Directory.CreateDirectory(physical + "sqlite.db").Create();
                }

                System.IO.File.Copy(physical + FILE_DB_SQLITE, physical + "sqlite.db/" + db_file, true);
                db_str = "Data Source=$ROOT/sqlite.db/" + db_file;
            }
            else if (db_type == "oledb")
            {
                if (db_file == "")
                {
                    db_file = "rd_" + string.Empty.RandomLetters(5) + ".mdb";
                }
                else if (db_file.IndexOf(".") == -1)
                {
                    db_file += ".mdb";
                }

                if (!Directory.Exists(physical + "ole.db"))
                {
                    Directory.CreateDirectory(physical + "ole.db").Create();
                }
                File.Copy(physical + FILE_DB_OLEDB, physical + "ole.db/" + db_file, true);
                db_str = "Data Source=$ROOT/ole.db/" + db_file;
            }
            else
            {
                //数据库资料不全
                if (String.IsNullOrEmpty(db_server) || String.IsNullOrEmpty(db_usr) || String.IsNullOrEmpty(db_name) || String.IsNullOrEmpty(db_prefix))
                {
                    return InstallCode.DB_ERROR;
                }

                if (db_type == "mysql")
                {
                    db_str = String.Format("server={0};database={1};uid={2};pwd={3};charset=utf8", db_server, db_name, db_usr, db_pwd);
                }
                else if (db_type == "mssql")
                {
                    db_str = String.Format("server={0};database={1};uid={2};pwd={3}", db_server, db_name, db_usr, db_pwd);
                }
                else
                {
                    return InstallCode.DB_UNKNOWN;
                }
            }

            #endregion

            #region 写入配置

            if (System.IO.File.Exists(physical + FILE_SETTING))
            {
                System.IO.File.Delete(physical + FILE_SETTING);
            }
            SettingFile file = new SettingFile(physical + FILE_SETTING);
            file.Add("license_key", t_key);
            file.Add("license_name", t_name);
            file.Add("server_static", "");
            file.Add("server_static_enabled", "false");

            file.Add("db_type", db_type);
            file.Add("db_conn", db_str);
            file.Add("db_prefix", db_prefix);

            file.Add("mm_avatar_path", "/file/avatar/");
            file.Flush();

            #endregion

            #region 初始化数据库

            if (!ExtraDB(db_type, db_str, db_prefix))
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
                String.Format("UPDATE {0}sites SET domain=@domain,name=@name,tpl=@tpl,seotitle=@name where siteid=1", db_prefix),
                new object[,]{
                    {"@domain",site_domain},
                    {"@name",site_name},
                    {"@tpl","default"}
                }));

            //创建管理用户
            this.db.ExecuteNonQuery(new SqlQuery(
                String.Format(@"
                        INSERT INTO {0}users (
                        siteid ,
                        username ,
                        password ,
                        name ,
                        groupid ,
                        available ,
                        createdate ,
                        lastlogindate ) VALUES (0,@username,@password,@name,1,1,@dt,@dt)
                    ", db_prefix),
                     new object[,]{
                         {"@username",user_name},
                         {"@password",  user_pwd.Encode16MD5().EncodeMD5()},
                         {"@name","管理员"},
                         {"@dt",DateTime.Now}
                     }));


            #endregion


            //创建安装文件
            System.IO.File.Create(String.Concat(physical, INSTALL_LOCK));

            Settings.TurnOffDebug();

            AtNet.Cms.Cms.Init();

            // 重启
            HttpRuntime.UnloadAppDomain();
            //AppDomain.Unload(AppDomain.CurrentDomain);

            return InstallCode.SUCCESS;
        }


        private DataBaseAccess db;
        public DataBaseAccess GetDB()
        {
            if (db != null)
            {
                return db;
            }
            return null;
        }

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

            this.db = new DataBaseAccess(type, connStr.Replace("$ROOT", AtNet.Cms.Cms.PyhicPath));

            string sql = null;
            string sqlScript = null;

            if (type == DataBaseType.MySQL)
            {
                sqlScript = String.Concat(AtNet.Cms.Cms.PyhicPath, MYSQL_INSTALL_SCRIPT);
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