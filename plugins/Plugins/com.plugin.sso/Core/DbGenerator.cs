using JR.DevFw.PluginKernel;
using JR.DevFw.Data;

namespace com.plugin.sso.Core
{
   public  class DbGenerator
    {
       private string _connString;

       internal DbGenerator()
       {
       }

       /// <summary>
       /// 数据库访问对象
       /// </summary>
       public DataBaseAccess New()
       {
           if (_connString == null)
           {
               _connString = Config.PluginAttr.Settings["db_conn"];
               
           }
           return new DataBaseAccess(
               DataBaseType.SQLServer,
               _connString
               );
       }
    }
}
