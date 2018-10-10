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
using System.Collections.Generic;
using T2.Cms.Domain.Interface.Site.Category;
using T2.Cms.Domain.Interface.Content.Archive;

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

            /// <summary>
            /// 成功
            /// </summary>
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
            if (String.IsNullOrEmpty(licenceName) || String.IsNullOrEmpty(licenceKey))
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
                    dbFile = "rd_" + string.Empty.RandomLetters(5) + ".db";
                }
                else if (dbFile.IndexOf(".", StringComparison.Ordinal) == -1)
                {
                    dbFile += ".db";
                }
                if (!Directory.Exists(physical + dbDirName))
                {
                    Directory.CreateDirectory(physical + dbDirName).Create();
                }
                dbStr = "Data Source=$ROOT/" + dbDirName + "/" + dbFile;
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

                if (!Directory.Exists(physical + dbDirName))
                {
                    Directory.CreateDirectory(physical + dbDirName).Create();
                }
                File.Copy(physical + FILE_DB_OLEDB, physical + dbDirName + "/" + dbFile, true);
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
            int siteId = this.InitSite(dbPrefix,siteName, siteDomain,int.Parse(siteLanguage));
            this.InitUserAndCredential(dbPrefix, userName, userPwd);
            int catId = this.InitCategory(dbPrefix, siteId);
            this.InitArchive(dbPrefix, siteId, catId);

            #endregion


            //创建安装文件
            File.Create(String.Concat(physical, INSTALL_LOCK));

            Settings.TurnOffDebug();

            Cms.Init(BootFlag.Normal, null);

            // 重启
            HttpRuntime.UnloadAppDomain();
            //AppDomain.Unload(AppDomain.CurrentDomain);

            return InstallCode.SUCCESS;
        }

        private void InitArchive(string dbPrefix, int siteId, int catId)
        {
            int unix = TimeUtils.Unix(DateTime.Now);
            IDictionary<String, Object> data = new Dictionary<String, Object>();
            data.Add("@siteId",siteId);
            data.Add("@strId", "welcome");
            data.Add("@alias", "welcome");
            data.Add("@catId", catId);
            data.Add("@flag",BuiltInArchiveFlags.Visible);
            data.Add("@path", "cms/welcome");
            data.Add("@authorId",1);
            data.Add("@title", "欢迎使用JRCms.NET");
            data.Add("@smallTitle", "");
            data.Add("@location", "");
            data.Add("@sortNumber", 1);
            data.Add("@source","");
            data.Add("@thumbnail", "");
            data.Add("@outline",  "");
            data.Add("@content", "<div style=\\\"text-align:center;font-size:30px\\\"><h2>欢迎使用JRCms.NET!</h2></div>");
            data.Add("@tags", "{st:\'0\',sc:\'0\',v:\'1\',p:\'0\'}");
            data.Add("@createTime", unix);
            data.Add("@updateTime", unix);
            String sql = String.Format(@"INSERT INTO {0}archive(
              site_id,str_id,alias,cat_id,author_id,path,flag,title,small_title,
              location,sort_number,source,thumbnail,outline,content,tags,
              agree,disagree,view_count,create_time,update_time)VALUES(
              @siteId,@strId,@alias,@catId,@authorId,@path,@flag,@title, @smallTitle,
              @location,@sortNumber, @source,@thumbnail,@outline, @content,
              @tags,0,0,1,@createTime, @updateTime)", dbPrefix);
            this.db.ExecuteNonQuery(new SqlQuery(sql, data));
        }

        private int InitCategory(string dbPrefix, int siteId)
        {
            String tag = "cms";
            IDictionary<String, object> data = new Dictionary<string, object>();
            data.Add("@tag",tag);
            data.Add("@site_id", siteId);
            data.Add("@parent_id", 0);
            data.Add("@code", "");
            data.Add("@path",tag);
            data.Add("@flag", CategoryFlag.Enabled);
            data.Add("@module_id",0);
            data.Add("@name", "cms");
            data.Add("@icon", "");
            data.Add("@page_title", "JRCMS");
            data.Add("@page_keywords", "");
            data.Add("@page_description", "");
            data.Add("@location", "");
            data.Add("@sort_number",1);
            String sql = String.Format(@"INSERT INTO {0}category (tag, site_id, parent_id, code, path, flag, 
                    module_id, name, icon, page_title, page_keywords, page_description, 
                    location, sort_number) VALUES(@tag,@site_id,@parent_id,@code,@path,@flag,@module_id,@name,
                    @icon,@page_title,@page_keywords,@page_description,@location,@sort_number)",dbPrefix);

            this.db.ExecuteNonQuery(new SqlQuery(sql, data));
            object maxId = this.db.ExecuteScalar(new SqlQuery(String.Format("SELECT MAX(id) FROM {0}category", dbPrefix)));
            return Convert.ToInt32(maxId);
        }

        /// <summary>
        /// 初始化站点
        /// </summary>
        /// <param name="dbPrefix"></param>
        /// <param name="siteName"></param>
        /// <param name="siteDomain"></param>
        /// <param name="langugage"></param>
        /// <returns></returns>
        private int InitSite(String dbPrefix,string siteName, string siteDomain,int langugage)
        {
            IDictionary<String, object> data = new Dictionary<String, Object>();
            data.Add("@site_id", 1);
            data.Add("@name", siteName);
            data.Add("@app_name", "");
            data.Add("@domain", siteDomain);
            data.Add("@location", "");
            data.Add("@language", langugage);
            data.Add("@tpl", "default");
            data.Add("@note", "");
            data.Add("@seo_title", "");
            data.Add("@seo_keywords", "");
            data.Add("@seo_description", "");
            data.Add("@state", 1);
            data.Add("@pro_tel", "021-88888888");
            data.Add("@pro_phone", "");
            data.Add("@pro_fax", "");
            data.Add("pro_address", "");
            data.Add("pro_email", "");
            data.Add("pro_im", "");
            data.Add("pro_post", "000000");
            data.Add("pro_notice", "");
            data.Add("@pro_slogan", "");
            //更新默认站点
            this.db.ExecuteNonQuery(new SqlQuery(
                String.Format(@"
                INSERT INTO {0}site(site_id,name,app_name,domain,location,language,tpl,note,
                seo_title,seo_keywords,seo_description,state,pro_tel,pro_phone,pro_fax,
                pro_address,pro_email,pro_im,pro_post,pro_notice,pro_slogan)VALUES(
                @site_id,@name,@app_name,@domain,@location,@language,@tpl,note,
                @seo_title,@seo_keywords,@seo_description,@state,@pro_tel,@pro_phone,
                @pro_fax,@pro_address,@pro_email,@pro_im,@pro_post,@pro_notice,@pro_slogan)", dbPrefix), data));
            object maxId = this.db.ExecuteScalar(new SqlQuery(String.Format("SELECT MAX(site_id) FROM {0}site", dbPrefix)));
            return Convert.ToInt32(maxId);
        }

        /// <summary>
        /// 初始化用户信息
        /// </summary>
        /// <param name="dbPrefix"></param>
        /// <param name="userName"></param>
        /// <param name="userPwd"></param>
        private void InitUserAndCredential(string dbPrefix,string userName, string userPwd)
        {
            // 创建用户组
            String sql = String.Format("INSERT INTO {0}usergroup (id,name,permissions) VALUES (@id,@name,@permissions)", dbPrefix);
            SqlQuery[] queries = new SqlQuery[5] {
                new SqlQuery(sql,CreateUserGroupParameters(1, "超级管理员", "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,"+
                "22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42")),
                new SqlQuery(sql,this.CreateUserGroupParameters(2,"管理员",
                "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,40,41,42")),
                new SqlQuery(sql,this.CreateUserGroupParameters(3,"编辑","1,2,3,4,5,6,10,11,12,13,14,15")),
                new SqlQuery(sql,this.CreateUserGroupParameters(4,"会员","1,2,3,4,5,6")),
                new SqlQuery(sql,this.CreateUserGroupParameters(5,"游客","3,4"))
            };
            this.db.ExecuteNonQuery(queries);


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
        }

        /// <summary>
        /// 创建用户组SQL参数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        private IDictionary<string, object> CreateUserGroupParameters(int id, string name, string permissions)
        {
            IDictionary<String, Object> data = new Dictionary<String, Object>();
            data.Add("@id", id);
            data.Add("@name", name);
            data.Add("@permissions", permissions);
            return data;
        }

        private DataBaseAccess db;

        /// <summary>
        /// 释放数据库
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="connStr"></param>
        /// <param name="dbPrefix"></param>
        /// <returns></returns>
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
    }
}