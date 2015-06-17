
using System.Text;

namespace AtNet.Cms.Domain.Interface.User
{
   public static class UserUtil
    {
       public static string GetRoleName(int roleFlag)
       {
           StringBuilder sb = new StringBuilder();
           if ((roleFlag & (int) RoleTag.Master) != 0)
           {
               AppendRoleName(sb, "超级管理员");
           }

           if ((roleFlag & (int)RoleTag.SiteOwner) != 0)
           {
               AppendRoleName(sb, "站点所有者");
           }

           if ((roleFlag & (int)RoleTag.Publisher) != 0)
           {
               AppendRoleName(sb, "编辑");
           }

           return sb.ToString();
       }

       private static void AppendRoleName(StringBuilder sb, string name)
       {
           if (sb.Length != 0)
           {
               sb.Append(",");
           }
           sb.Append(name);
       }
    }
}
