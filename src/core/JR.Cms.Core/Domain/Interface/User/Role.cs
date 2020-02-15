using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JR.Cms.Domain.Interface.User
{
    /// <summary>
    /// 角色标签
    /// </summary>
    /// 
    [Flags]
    internal enum RoleFlag:int
    {
        /// <summary>
        /// 内容发布者
        /// </summary>
        Publisher = 1 << 0,

        //todo: add other

        /// <summary>
        /// 站点管理员
        /// </summary>
        SiteOwner = 1 << 1,

        /// <summary>
        /// 超级管理员
        /// </summary>
        Master = 1 << 2
    }

    public static class Role
    {
        public static RoleValue Master = new RoleValue(1<<2,"超级管理员");
        public static RoleValue SiteOwner = new RoleValue(1<<1,"站长");
        public static RoleValue Publisher = new RoleValue(1<<0,"发布者");

        /// <summary>
        /// 获取内置角色名
        /// </summary>
        /// <param name="roleFlag"></param>
        /// <returns></returns>
        public static string GetRoleName(int roleFlag)
        {
            IList<RoleValue> list = GetRoles(roleFlag);
            return GetRolesName(roleFlag, list);
        }

        private static string GetRolesName(int roleFlag, IList<RoleValue> list)
        {
            StringBuilder sb = new StringBuilder();

            foreach (RoleValue rv in list)
            {
                if ((roleFlag & rv.Flag) != 0)
                {
                    AppendRoleName(sb, rv.Name);
                }
            }

            //throw new Exception(((int)(RoleTag.Master | RoleTag.Publisher)).ToString());
            return sb.ToString();
        }

        public  static  IList<RoleValue> GetRoles(int roleFlag)
        {
            IList<RoleValue> list = new List<RoleValue>();
            if ((roleFlag & Master.Flag) != 0)
            {
               list.Add(Master);
            }

            if ((roleFlag & SiteOwner.Flag) != 0)
            {
               list.Add(SiteOwner);
            }

            if ((roleFlag & Publisher.Flag) != 0)
            {
                list.Add(Publisher);
            }
            return list;
        }

        private static void AppendRoleName(StringBuilder sb, string name)
        {
            if (sb.Length != 0)
            {
                sb.Append(",");
            }
            sb.Append(name);
        }

        /// <summary>
        /// 在指定APP里是否包含了角色
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="roleValues"></param>
        /// <returns></returns>
//        public static bool ContainsApp(int appId, AppRole[] roleValues)
//        {
//            return roleValues.Any(roleValue => roleValue.AppId == 0 || roleValue.AppId == appId);
//        }
    }
}
