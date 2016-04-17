using J6.DevFw.Data;

namespace J6.Cms.DB
{
    public class DbAccess
    {
        private readonly DataBaseType _dbType;
        private readonly string _connectionString;  
        private DataBaseAccess dbAccess;

        public DbAccess(DataBaseType dbType,string connectionString)
        {
            this._dbType = dbType;
            this._connectionString = connectionString;

            this.dbAccess= new DataBaseAccess(this._dbType, this._connectionString);
        }

        /// <summary>
        /// 表前缀
        /// </summary>
        public string TablePrefix { get; set; }

        public DataBaseAccess CreateInstance()
        {
            //return new DataBaseAccess(this._dbType, this._connectionString);
            return this.dbAccess;
        }
    }
}
