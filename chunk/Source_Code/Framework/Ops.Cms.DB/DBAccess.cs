using Ops.Data;

namespace Ops.Cms.DB
{
    public class DBAccess
    {
        private DataBaseType _dbType;
        private string _connectionString;  
        private DataBaseAccess dbAccess;

        public DBAccess(DataBaseType dbType,string connectionString)
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
